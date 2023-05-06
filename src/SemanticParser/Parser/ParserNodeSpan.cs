using SemanticParser.Config;

namespace SemanticParser.Parser;
internal class ParserNodeSpan
{
    public NodeDefinition NodeDefinition { get; }

    public int BeginIndex { get; }

    public int? EndIndex { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }

    public List<ParserNodeSpan> SubNodes { get; } = new();

    public ParserNodeSpan? ParentNode { get; set; }

    public int BeginLine { get; internal set; }

    public int BeginCharPos { get; internal set; }

    public int EndLine { get; internal set; }

    public int EndCharPos { get; internal set; }

    public ParserNodeSpan(NodeDefinition nodeDefinition, int beginIndex, string name, string type)
    {
        this.NodeDefinition = nodeDefinition;
        this.BeginIndex = beginIndex;
        this.Name = name;
        this.Type = type;
    }

    public void AddSubNode(ParserNodeSpan subNode)
    {
        this.SubNodes.Add(subNode);
        subNode.ParentNode = this;
    }
}
