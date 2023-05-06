namespace SemanticParser.Config;
internal class ParserSetting
{
    public List<NodeSetting> Nodes { get; set; } = new();

    public List<RuleSetSetting> RuleSets { get; set; } = new();
}
