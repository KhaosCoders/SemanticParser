namespace SemanticParser.Model;
public abstract class NodeBase
{
    [SerializIt.Order(0)]
    public string Type { get; set; }

    [SerializIt.Order(1)]
    public string Name { get; set; }

    [SerializIt.Order(2)]
    [SerializIt.Inline]
    public LocationSpan LocationSpan { get; set; } = new();

    protected NodeBase(string type, string name)
    {
        this.Type = type;
        this.Name = name;
    }
}
