using System.Text.RegularExpressions;

namespace SemanticParser.Config;
internal class NodeDefinition
{
    private readonly NodeSetting _setting;

    public NodeDefinition(NodeSetting setting)
    {
        ArgumentNullException.ThrowIfNull(setting, nameof(setting));
        ArgumentNullException.ThrowIfNull(setting.Key, nameof(setting.Key));
        ArgumentNullException.ThrowIfNull(setting.BeginPattern, nameof(setting.BeginPattern));

        this._setting = setting;
        this.BeginPattern = new Regex(setting.BeginPattern, RegexOptions.Compiled);
        if (!string.IsNullOrWhiteSpace(setting.EndPattern))
        {
            this.EndPattern = new Regex(setting.EndPattern, RegexOptions.Compiled);
        }

        // Type or TypePattern must be set
        if (!string.IsNullOrWhiteSpace(setting.TypePattern))
        {
            this.TypePattern = new Regex(setting.TypePattern, RegexOptions.Compiled);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(setting.Type, nameof(setting.Type));
        }

        // Name or NamePattern must be set
        if (!string.IsNullOrWhiteSpace(setting.NamePattern))
        {
            this.NamePattern = new Regex(setting.NamePattern, RegexOptions.Compiled);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(setting.Name, nameof(setting.Name));
        }

        if(setting.OnlyWithin != null)
        {
            this.OnlyWithin = new ContainerDefinition(setting.OnlyWithin);
        }
    }

    public string Key => this._setting.Key!;

    public string? Type => this._setting.Type;

    public string? Name => this._setting.Name;

    public bool OnlyFirst => this._setting.OnlyFirst;

    public Regex? TypePattern { get; }

    public Regex BeginPattern { get; }

    public Regex? NamePattern { get; }

    public Regex? EndPattern { get; }

    public List<NodeDefinition> EndOn { get; } = new();

    public List<NodeDefinition> SubNodes { get; } = new();

    public ContainerDefinition? OnlyWithin { get; set; }
}
