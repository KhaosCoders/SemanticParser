namespace SemanticParser.Config;
internal class RuleSet
{
    private readonly RuleSetSetting _setting;

    public RuleSet(RuleSetSetting setting)
    {
        ArgumentNullException.ThrowIfNull(setting, nameof(setting));
        ArgumentNullException.ThrowIfNull(setting.Name, nameof(setting.Name));
        ArgumentNullException.ThrowIfNull(setting.Extensions, nameof(setting.Extensions));

        this._setting = setting;
    }

    public string Name => this._setting.Name!;

    public List<string> Extensions => this._setting.Extensions!;

    public List<NodeDefinition> RootNodes { get; } = new();

    public IEnumerable<NodeDefinition> AllDistinctNodes()
    {
        HashSet<string> nodeKeys = new();
        foreach (var node in this.RootNodes)
        {
            foreach (var subNode in this.DistinctNodes(node, nodeKeys))
            {
                yield return subNode;
            }
        }
    }

    private IEnumerable<NodeDefinition> DistinctNodes(NodeDefinition node, HashSet<string> nodeKeys)
    {
        if (!nodeKeys.Contains(node.Key))
        {
            nodeKeys.Add(node.Key);
            yield return node;
        }

        foreach (var sub in node.SubNodes)
        {
            foreach (var subNode in this.DistinctNodes(sub, nodeKeys))
            {
                yield return subNode;
            }
        }
    }
}
