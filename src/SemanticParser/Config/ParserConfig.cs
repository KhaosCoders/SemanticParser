namespace SemanticParser.Config;
internal class ParserConfig
{
    private readonly ParserSetting _settings;
    private readonly Dictionary<string, RuleSet> _ruleSets = new();
    private readonly Dictionary<string, NodeDefinition> _nodeDefinitions = new();

    public ParserConfig(ParserSetting settings)
    {
        this._settings = settings;
    }

    public RuleSet GetRuleSet(string extension)
    {
        if (this._ruleSets.TryGetValue(extension, out var ruleSet))
        {
            return ruleSet;
        }

        return this.LoadRuleSet(extension);
    }

    private RuleSet LoadRuleSet(string extension)
    {
        var ruleSetSetting = this._settings.RuleSets.Find(x =>
            x.Extensions?.Any(ext =>
                ext.Equals(extension, StringComparison.InvariantCultureIgnoreCase)) == true)
            ?? throw new NotSupportedException($"No rules defined for files with extension {extension}");

        ArgumentNullException.ThrowIfNull(ruleSetSetting.RootNodes, nameof(ruleSetSetting.RootNodes));

        RuleSet ruleSet = new(ruleSetSetting);
        this._ruleSets[extension] = ruleSet;

        ruleSetSetting.RootNodes.ForEach(key => ruleSet.RootNodes.Add(this.GetNodeDefinition(key)));

        return ruleSet;
    }

    private NodeDefinition GetNodeDefinition(string key)
    {
        if (this._nodeDefinitions.TryGetValue(key, out var nodeDefinition))
        {
            return nodeDefinition;
        }
        return this.LoadNodeDefinition(key);
    }

    private NodeDefinition LoadNodeDefinition(string key)
    {
        var nodeSetting = this._settings.Nodes.Find(x => x.Key == key)
            ?? throw new NotSupportedException($"No node definition found for key {key}");

        NodeDefinition nodeDefinition = new(nodeSetting);
        this._nodeDefinitions[key] = nodeDefinition;

        nodeSetting.SubNodes?.ForEach(key => nodeDefinition.SubNodes.Add(this.GetNodeDefinition(key)));
        nodeSetting.EndOn?.ForEach(key => nodeDefinition.EndOn.Add(this.GetNodeDefinition(key)));

        return nodeDefinition;
    }
}
