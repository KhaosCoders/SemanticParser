namespace SemanticParser.Model;
internal class Container : NodeBase
{
    [SerializIt.Order(3)]
    [SerializIt.Inline]
    public int[]? HeaderSpan { get; set; }

    [SerializIt.Order(4)]
    [SerializIt.Inline]
    public int[]? FooterSpan { get; set; }

    [SerializIt.Skip(true)]
    [SerializIt.Order(5)]
    public List<NodeBase>? Children { get; set; }

    public Container(string type, string name)
        : base(type, name)
    { }

    public void AddChild(NodeBase child) =>
        (this.Children ??= new())
            .Add(child);
}
