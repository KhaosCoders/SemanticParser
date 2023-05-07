using SemanticParser.Config;
using Serilog;
using System.Text.RegularExpressions;

namespace SemanticParser.Parser;
internal class TextParser : ITextParser
{
    public void ParseNodes(ref ParserContext ctx)
    {
        var nodeDefinitions = ctx.Rules.AllDistinctNodes();

        // Mark all beginnings of nodes
        foreach (var nodeDefinition in nodeDefinitions)
        {
            FindNodeBeginnings(ctx, nodeDefinition);
        }

        // Order nodes by their beginning
        OrderNodesByIndex(ctx);

        // Match all endings
        FindNodeEndings(ctx, nodeDefinitions);

        // Assign sub nodes
        AssignSubNodes(ctx);
    }

    private static void AssignSubNodes(ParserContext ctx)
    {
        for (int i = 0; i < ctx.NodeSpans.Count; i++)
        {
            AssignSubNodes(ctx, ref i);
        }
    }

    private static void AssignSubNodes(ParserContext ctx, ref int i)
    {
        if (ctx.NodeSpans.Count <= i)
        {
            return;
        }

        ParserNodeSpan? currentnode = ctx.NodeSpans[i];
        for (int j = i + 1; j < ctx.NodeSpans.Count; j++)
        {
            ParserNodeSpan? nextNode = ctx.NodeSpans[j];
            if (currentnode.NodeDefinition.SubNodes.Contains(nextNode.NodeDefinition))
            {
                currentnode.AddSubNode(nextNode);
                AssignSubNodes(ctx, ref j);
                i = j;
            }
            else
            {
                break;
            }
        }
    }

    private static void FindNodeEndings(ParserContext ctx, IEnumerable<NodeDefinition> nodeDefinitions)
    {
        var endingsByNodeType = FindNodeEndingsByNodeType(ctx, nodeDefinitions);

        for (int i = 0; i < ctx.NodeSpans.Count; i++)
        {
            ParserNodeSpan? nodeSpan = ctx.NodeSpans[i];
            var nodeDefinition = nodeSpan.NodeDefinition;

            // Check next node for ending
            if (nodeDefinition.EndOn?.Count > 0 && i + 1 < ctx.NodeSpans.Count)
            {
                ParserNodeSpan? subNodeSpan = ctx.NodeSpans[i + 1];
                if (nodeDefinition.EndOn.Contains(subNodeSpan.NodeDefinition))
                {
                    nodeSpan.EndIndex = subNodeSpan.BeginIndex - 1;
                    continue;
                }
            }

            // Otherwise check for ending pattern
            if (!endingsByNodeType.TryGetValue(nodeSpan.NodeDefinition, out var endingIndexes))
            {
                continue;
            }

            int endIndex = endingIndexes.Find(e => e >= nodeSpan.BeginIndex);
            if (endIndex > 0)
            {
                nodeSpan.EndIndex = endIndex;
            }
        }
    }

    private static IDictionary<NodeDefinition, List<int>> FindNodeEndingsByNodeType(ParserContext ctx, IEnumerable<NodeDefinition> nodeDefinitions)
    {
        Dictionary<NodeDefinition, List<int>> endingsByNodeType = new();

        foreach (var nodeDefinition in nodeDefinitions)
        {
            if (nodeDefinition.EndPattern == null)
            {
                continue;
            }

            foreach (var end in nodeDefinition.EndPattern.EnumerateMatches(ctx.InputSpan))
            {
                Log.Debug("Found node ending {Node} at index {Index}", nodeDefinition.Key, end.Index);
                if (!endingsByNodeType.TryGetValue(nodeDefinition, out var endingIndexes))
                {
                    endingIndexes = new();
                    endingsByNodeType.Add(nodeDefinition, endingIndexes);
                }

                endingIndexes.Add(end.Index + end.Length);
            }
        }

        return endingsByNodeType;
    }

    private static void OrderNodesByIndex(ParserContext ctx) =>
        ctx.NodeSpans.Sort((a, b) => a.BeginIndex.CompareTo(b.BeginIndex));

    private static void FindNodeBeginnings(ParserContext ctx, NodeDefinition nodeDefinition)
    {
        var containerBounds = FindContainerBounds(ctx, nodeDefinition.OnlyWithin);

        var names = FindNodeValues(ctx, nodeDefinition.NamePattern);
        var types = FindNodeValues(ctx, nodeDefinition.TypePattern);

        foreach (var begin in nodeDefinition.BeginPattern.EnumerateMatches(ctx.InputSpan))
        {
            int index = begin.Index;
            if (!containerBounds.Any(b => b.Start <= index && b.End >= index))
            {
                continue;
            }

            string name = nodeDefinition.Name ?? names.First(n => n.Index >= index).Value;
            string type = nodeDefinition.Type ?? types.First(n => n.Index >= index).Value;
            Log.Debug("Found node {Node}: {Type} {Name} at index {Index}", nodeDefinition.Key, type, name, index);
            ctx.NodeSpans.Add(new(nodeDefinition, index, name, type));

            if (nodeDefinition.OnlyFirst)
            {
                break;
            }
        }
    }

    private static IList<(int Index, string Value)> FindNodeValues(ParserContext ctx, Regex? pattern)
    {
        List<(int Index, string Value)> names = new();
        if (pattern != null)
        {
            var matches = pattern.Matches(ctx.InputText);
            names.AddRange(matches.Select<Match, (int Index, string Value)>(match =>
                new(match.Index, match.Groups[1].Value)));
        }

        return names;
    }

    private static List<(int Start, int End)> FindContainerBounds(ParserContext ctx, ContainerDefinition? containerDefinition)
    {
        if (containerDefinition == null)
        {
            return new()
            {
                new(0, ctx.InputSpan.Length)
            };
        }

        // Find all endings
        List<int> endings = new();
        foreach (var end in containerDefinition.EndPattern.EnumerateMatches(ctx.InputSpan))
        {
            endings.Add(end.Index);
        }

        // Find all ranges
        List<(int Start, int End)> bounds = new();
        foreach (var begin in containerDefinition.BeginPattern.EnumerateMatches(ctx.InputSpan))
        {
            int startIndex = begin.Index;
            int endIndex = endings.Find(e => e >= startIndex);
            if (endIndex > 0)
            {
                bounds.Add(new (startIndex, endIndex));
            }
        }

        return bounds;
    }
}
