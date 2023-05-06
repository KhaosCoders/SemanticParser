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
                    Key = "define",
                    BeginPattern = @"(?i)(?m)^\s*defi(?:n(?:e)?)?\b",
                    TypePattern = @"(?i)(?m)^\s*defi(?:n(?:e)?)?\s+([\w]+)\b",
                    NamePattern = @"(?i)(?m)^\s*defi(?:n(?:e)?)?\s+[\w]+\s+([\w]+)",
                    EndPattern = @"(?i)(?m)^\s*endde(?:f(?:i(?:n(?:e)?)?)?)?\b",
                    SubNodes = new List<string> { "procedure", "function" }
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
        Assert.AreEqual("prg", ruleSet.RootNodes[0].Key);
        Assert.AreEqual("prg", ruleSet.RootNodes[0].Type);
        Assert.AreEqual("{FileName}", ruleSet.RootNodes[0].Name);
        Assert.IsNotNull(ruleSet.RootNodes[0].BeginPattern);
        Assert.IsNull(ruleSet.RootNodes[0].TypePattern);
        Assert.IsNull(ruleSet.RootNodes[0].NamePattern);
        Assert.IsNull(ruleSet.RootNodes[0].EndPattern);
        Assert.AreEqual(0, ruleSet.RootNodes[0].EndOn.Count);
        Assert.AreEqual(3, ruleSet.RootNodes[0].SubNodes.Count);
        Assert.AreEqual("procedure", ruleSet.RootNodes[0].SubNodes[0].Key);
        Assert.AreEqual("function", ruleSet.RootNodes[0].SubNodes[1].Key);
        Assert.AreEqual("define", ruleSet.RootNodes[0].SubNodes[2].Key);
    }
}
