using SerializIt;

namespace SemanticParser.Model;
public class LocationSpan
{
    public int[] Start { get; set; } = new int[] { 0, 0 };

    public int[] End { get; set; } = new int[] { 0, 0 };

    [Skip]
    public int StartLine
    {
        get => this.Start[0];
        set => this.Start[0] = value;
    }

    [Skip]
    public int StartColumn
    {
        get => this.Start[1];
        set => this.Start[1] = value;
    }

    [Skip]
    public int EndLine
    {
        get => this.End[0];
        set => this.End[0] = value;
    }

    [Skip]
    public int EndColumn
    {
        get => this.End[1];
        set => this.End[1] = value;
    }
}