namespace SemanticParser.Model;
public class File : NodeBase
{
    [SerializIt.Order(3)]
    [SerializIt.Inline]
    public int[] FooterSpan { get; set; } = { 0, -1 };

    [SerializIt.Order(4)]
    public bool ParsingErrorsDetected => this.ParsingError?.Count > 0;

    [SerializIt.Skip(true)]
    [SerializIt.Order(5)]
    public List<NodeBase>? Children { get; set; }

    [SerializIt.Skip(true)]
    [SerializIt.Order(6)]
    public List<ParsingError>? ParsingError { get; set; }

    public File(string name)
        : base("file", name)
    { }

    public void AddChild(NodeBase child) =>
        (this.Children ??= new())
            .Add(child);
}
