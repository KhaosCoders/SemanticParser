namespace SemanticParser.Model;
internal class Node : NodeBase
{
    [SerializIt.Order(3)]
    [SerializIt.Inline]
    public int[]? Span { get; set; }

    public Node(string type, string name)
        : base(type, name)
    { }
}
