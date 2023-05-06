namespace SemanticParser.Config;
internal class NodeSetting
{
    public string? Key { get; set; }

    public string? Type { get; set; }

    public string? Name { get; set; }

    public bool OnlyFirst { get; set; }

    public string? BeginPattern { get; set; }

    public string? NamePattern { get; set; }

    public string? TypePattern { get; set; }

    public string? EndPattern { get; set; }

    public List<string>? EndOn { get; set; }

    public List<string>? SubNodes { get; set; }
}
