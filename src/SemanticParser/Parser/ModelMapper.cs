﻿namespace SemanticParser.Parser;
internal class ModelMapper : IModelMapper
{
    public void Map(ref ParserContext ctx)
    {
        MapFileNode(ref ctx);

        MapNodes(ref ctx);
    }

    private static void MapNodes(ref ParserContext ctx)
    {
        Dictionary<ParserNodeSpan, Model.NodeBase> mappedNodes = new();

        foreach (var nodeSpan in ctx.NodeSpans)
        {
            // Map node
            Model.NodeBase? node;
            if (nodeSpan.SubNodes?.Count > 0)
            {
                node = MapToContainer(nodeSpan);
            }
            else
            {
                node = MapToTerminalNode(nodeSpan);
            }

            node.LocationSpan.StartLine = nodeSpan.BeginLine;
            node.LocationSpan.StartColumn = nodeSpan.BeginCharPos;
            node.LocationSpan.EndLine = nodeSpan.EndLine;
            node.LocationSpan.EndColumn = nodeSpan.EndCharPos;

            mappedNodes.Add(nodeSpan, node);

            // Assign parent
            if (nodeSpan.ParentNode != null)
            {
                if (!mappedNodes.TryGetValue(nodeSpan.ParentNode, out Model.NodeBase? parentNode))
                {
                    throw new ParserException($"Unable to find parent node for node: {nodeSpan.Name}");
                }

                if (parentNode is Model.Container container)
                {
                    container.AddChild(node);
                    continue;
                }

                throw new ParserException($"Parent node is not a container: {nodeSpan.ParentNode.Name}");
            }

            // Assign to file
            ctx.File?.AddChild(node);
        }
    }

    private static Model.Node MapToTerminalNode(ParserNodeSpan nodeSpan) =>
        new(nodeSpan.Name, nodeSpan.Type)
        {
            Span = new int[] { nodeSpan.BeginIndex, nodeSpan.EndIndex ?? nodeSpan.BeginIndex }
        };

    private static Model.Container MapToContainer(ParserNodeSpan nodeSpan)
    {
        int beforeFirstChildIndex = nodeSpan.SubNodes[0].BeginIndex - 1;
        if (beforeFirstChildIndex < nodeSpan.BeginIndex)
        {
            beforeFirstChildIndex = nodeSpan.BeginIndex;
        }

        int afterLastChildIndex = nodeSpan.SubNodes[^1].EndIndex ?? nodeSpan.EndIndex ?? nodeSpan.BeginIndex;
        if (afterLastChildIndex > (nodeSpan.EndIndex ?? nodeSpan.BeginIndex))
        {
            afterLastChildIndex = nodeSpan.EndIndex ?? nodeSpan.BeginIndex;
        }

        return new(nodeSpan.Name, nodeSpan.Type)
        {
            HeaderSpan = new int[] { nodeSpan.BeginIndex, beforeFirstChildIndex },
            FooterSpan = new int[] { afterLastChildIndex, nodeSpan.EndIndex ?? nodeSpan.BeginIndex },
        };
    }

    private static void MapFileNode(ref ParserContext ctx)
    {
        var (endLine, endCharPos) = FindFileEnd(ctx);
        ctx.File = new(ctx.FilePath);
        ctx.File.LocationSpan.StartLine = 1;
        ctx.File.LocationSpan.StartColumn = 0;
        ctx.File.LocationSpan.EndLine = endLine;
        ctx.File.LocationSpan.EndColumn = endCharPos;
    }

    private static (int endLine, int endCharPos) FindFileEnd(ParserContext ctx)
    {
        int endLine = 0;
        int endCharPos = 0;

        foreach (var span in ctx.NodeSpans)
        {
            if (span.EndLine > endLine)
            {
                endLine = span.EndLine;
                endCharPos = span.EndCharPos;
            }
            else if (span.EndLine == endLine && span.EndCharPos > endCharPos)
            {
                endCharPos = span.EndCharPos;
            }
        }

        return (endLine, endCharPos);
    }
}