using SemanticParser.Config;

namespace SemanticParser.Tests.Config;
[TestClass]
public class ParserConfigTests
{
    [TestMethod]
    public void ParserConfig_ShouldReturnLoadedRuleSet()
    {
        // Arrange
        var parserSetting = new ParserSetting
        {
            Nodes = new List<NodeSetting>
            {
                new NodeSetting
                {
                    Key = "procedure",
                    Type = "procedure",
                    BeginPattern = @"(?i)(?m)^\s*proc(?:e(?:d(?:u(?:r(?:e)?)?)?)?)?\b",
                    NamePattern = @"(?i)(?m)^\s*proc(?:e(?:d(?:u(?:r(?:e)?)?)?)?)?\s+([\w]+)",
                    EndPattern = @"(?i)(?m)^\s*endp(?:r(?:o(?:c)?)?)?\b",
                    EndOn = new List<string> { "procedure", "function" }
                },
                new NodeSetting
                {
                    Key = "function",
                    Type = "function",
                    BeginPattern = @"(?i)(?m)^\s*func(?:t(?:i(?:o(?:n)?)?)?)?\b",
                    NamePattern = @"(?i)(?m)^\s*func(?:t(?:i(?:o(?:n)?)?)?)?\s+([\w]+)",
                    EndPattern = @"(?i)(?m)^\s*endfu(?:n(?:c)?)?\b",
                    EndOn = new List<string> { "procedure", "function" }
                },
                new NodeSetting
                {
                    Key = "property",
                    Type = "property",
                    BeginPattern = @"(?i)(?m)^[ \t]*[\w]+[\s]*=",
                    NamePattern = @"(?i)(?m)^[ \t]*([\w]+)[\s]*=",
                    OnlyWithin = new ContainerSetting
                    {
                        BeginPattern = @"(?i)(?m)^[ \t]*defi(?:n(?:e)?)?\b",
                        EndPattern = @"(?i)(?m)^[ \t]*(?:endde(?:f(?:i(?:n(?:e)?)?)?)?|func(?:t(?:i(?:o(?:n)?)?)?)?|proc(?:e(?:d(?:u(?:r(?:e)?)?)?)?)?)\b"
                    },
                    EndOn = new List<string> { "procedure", "function", "property" }
                },
                new NodeSetting
                {
                    Key = "define",
                    BeginPattern = @"(?i)(?m)^\s*defi(?:n(?:e)?)?\b",
                    TypePattern = @"(?i)(?m)^\s*defi(?:n(?:e)?)?\s+([\w]+)\b",
                    NamePattern = @"(?i)(?m)^\s*defi(?:n(?:e)?)?\s+[\w]+\s+([\w]+)",
                    EndPattern = @"(?i)(?m)^\s*endde(?:f(?:i(?:n(?:e)?)?)?)?\b",
                    SubNodes = new List<string> { "procedure", "function", "property" }
                },
                new NodeSetting
                {
                    Key = "prg",
                    Type = "prg",
                    Name = "{FileName}",
                    BeginPattern = @"(?i)(?m)^\s[^\*&]+",
                    SubNodes = new List<string> { "procedure", "function", "define" }
                }
            },
            RuleSets = new List<RuleSetSetting>
            {
                new RuleSetSetting
                {
                    Name = "prg",
                    Extensions = new List<string> { "prg" },
                    RootNodes = new List<string> { "prg" }
                }
            }
        };
        var parserConfig = new ParserConfig(parserSetting);

        // Act
        var ruleSet = parserConfig.GetRuleSet("prg");

        // Assert
        Assert.IsNotNull(ruleSet);
        Assert.AreEqual(1, ruleSet.RootNodes.Count);

        NodeDefinition prg = ruleSet.RootNodes[0];
        Assert.AreEqual("prg", prg.Key);
        Assert.AreEqual("prg", prg.Type);
        Assert.AreEqual("{FileName}", prg.Name);
        Assert.IsNotNull(prg.BeginPattern);
        Assert.IsNull(prg.TypePattern);
        Assert.IsNull(prg.NamePattern);
        Assert.IsNull(prg.EndPattern);
        Assert.AreEqual(0, prg.EndOn.Count);
        Assert.AreEqual(3, prg.SubNodes.Count);
        Assert.AreEqual("procedure", prg.SubNodes[0].Key);
        Assert.AreEqual("function", prg.SubNodes[1].Key);

        NodeDefinition define = prg.SubNodes[2];
        Assert.AreEqual("define", define.Key);
        Assert.AreEqual(3, define.SubNodes.Count);
        Assert.AreEqual("procedure", define.SubNodes[0].Key);
        Assert.AreEqual("function", define.SubNodes[1].Key);

        NodeDefinition property = define.SubNodes[2];
        Assert.AreEqual("property", property.Key);
        Assert.IsNotNull(property.OnlyWithin);
    }
}
