using SemanticParser.Config;

namespace SemanticParser.Parser;
internal ref struct ParserContext
{
    public RuleSet Rules { get; }

    public Model.File? File { get; set; }

    public ReadOnlySpan<char> InputSpan { get; }
    public string InputText { get; }

    public string FilePath { get; }

    public int CurrentLine { get; } = 1;

    public int CurrentColum { get; set; }

    public int CurrentIndex { get; set; }

    public List<ParserNodeSpan> NodeSpans { get; set; } = new();

    public ParserContext(string input, string filePath, RuleSet rules)
    {
        this.InputText = input;
        this.InputSpan = input.AsSpan();
        this.FilePath = filePath;
        this.Rules = rules;
    }
}
