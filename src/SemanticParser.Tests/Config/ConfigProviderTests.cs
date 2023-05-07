using Microsoft.Extensions.Configuration;
using Moq;
using System.Text;

namespace SemanticParser.Tests.Config;
[TestClass]
public class ConfigProviderTests
{
    private const string DummySettings =
"""
{
    "Parser": {
        "Nodes": [
            {
                "Key": "procedure",
                "Type": "procedure",
                "BeginPattern": "(?i)(?m)^[\\s]*proc(?:e(?:d(?:u(?:r(?:e)?)?)?)?)?\\b",
                "NamePattern": "(?i)(?m)^[\\s]*proc(?:e(?:d(?:u(?:r(?:e)?)?)?)?)?[\\s]+([\\w]+)",
                "EndPattern": "(?i)(?m)^[\\s]*endp(?:r(?:o(?:c)?)?)?\\b",
                "EndOn": [ "procedure", "function" ]
            },
            {
                "Key": "function",
                "Type": "function",
                "BeginPattern": "(?i)(?m)^[\\s]*func(?:t(?:i(?:o(?:n)?)?)?)?\\b",
                "NamePattern": "(?i)(?m)^[\\s]*func(?:t(?:i(?:o(?:n)?)?)?)?[\\s]+([\\w]+)",
                "EndPattern": "(?i)(?m)^[\\s]*endfu(?:n(?:c)?)?\\b",
                "EndOn": [ "procedure", "function" ]
            },
            {
                "Key": "property",
                "Type": "property",
                "BeginPattern": "(?i)(?m)^[ \\t]*[\\w]+[\\s]*=",
                "NamePattern": "(?i)(?m)^[ \\t]*([\\w]+)[\\s]*=",
                "OnlyWithin": {
                    "BeginPattern": "(?i)(?m)^[ \\t]*defi(?:n(?:e)?)?\\b",
                    "EndPattern": "(?i)(?m)^[ \\t]*(?:endde(?:f(?:i(?:n(?:e)?)?)?)?|func(?:t(?:i(?:o(?:n)?)?)?)?|proc(?:e(?:d(?:u(?:r(?:e)?)?)?)?)?)\\b"
                },
                "EndOn": [ "procedure", "function", "property" ]
            },
            {
                "Key": "define",
                "BeginPattern": "(?i)(?m)^[\\s]*defi(?:n(?:e)?)?\\b",
                "TypePattern": "(?i)(?m)^[\\s]*defi(?:n(?:e)?)?[\\s]+([\\w]+)\\b",
                "NamePattern": "(?i)(?m)^[\\s]*defi(?:n(?:e)?)?[\\s]+[\\w]+[\\s]+([\\w]+)",
                "EndPattern": "(?i)(?m)^[\\s]*endde(?:f(?:i(?:n(?:e)?)?)?)?\\b",
                "SubNodes": [ "procedure", "function", "property" ]
            },
            {
                "Key": "prg",
                "Type": "prg",
                "Name": "{FileName}",
                "BeginPattern": "(?i)(?m)^[\\s][^\\*&]+",
                "EndPattern": "$",
                "SubNodes": [ "procedure", "function", "define" ]
            },
            {
                "Key": "vfp_record",
                "Type": "vfp_record",
                "BeginPattern": "(?m)^\\[ RECORD\\]\\r?\\n\\[PLATFORM\\] WINDOWS \\r?\\n",
                "NamePattern": "(?m)^\\[OBJNAME\\] ([\\w]+)\\r?\\n",
                "EndPattern": null,
                "EndOn": [ "vfp_record" ],
                "SubNodes": [ "procedure", "function" ]
            }
        ],
        "RuleSets": [
            {
                "Name": "VFP Projectfile",
                "Extensions": [ ".pja", ".sca", ".vca" ],
                "RootNodes": [ "vfp_record" ]
            },
            {
                "Name": "VFP Prg",
                "Extensions": [ ".prg" ],
                "RootNodes": [ "prg" ]
            }
        ]
    }
}
""";

    private IConfigurationRoot? config;

    [TestInitialize]
    public void TestInitialize() =>
        this.config = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(DummySettings)))
            .Build();

    [TestMethod]
    public void ParserSetting_ShouldReturnParserSetting()
    {
        // Arrange
        var configurationBuilderMock = new Mock<SemanticParser.Config.IConfigurationBuilder>();
        configurationBuilderMock.Setup(x => x.Build())
            .Returns(this.config!);

        var configProvider = new SemanticParser.Config.ConfigProvider(configurationBuilderMock.Object);

        // Act
        var result = configProvider.ParserSetting;

        // Assert
        Assert.IsNotNull(result);

        // Nodes
        Assert.AreEqual(6, result.Nodes.Count);
        AssertNode(result, 0, "procedure", "procedure", null, "(?i)(?m)^[\\s]*proc(?:e(?:d(?:u(?:r(?:e)?)?)?)?)?\\b",
            null, "(?i)(?m)^[\\s]*proc(?:e(?:d(?:u(?:r(?:e)?)?)?)?)?[\\s]+([\\w]+)",
            "(?i)(?m)^[\\s]*endp(?:r(?:o(?:c)?)?)?\\b",
            "procedure", null, null);
        AssertNode(result, 1, "function", "function", null, "(?i)(?m)^[\\s]*func(?:t(?:i(?:o(?:n)?)?)?)?\\b",
            null, "(?i)(?m)^[\\s]*func(?:t(?:i(?:o(?:n)?)?)?)?[\\s]+([\\w]+)",
            "(?i)(?m)^[\\s]*endfu(?:n(?:c)?)?\\b",
            "procedure", null, null);
        AssertNode(result, 2, "property", "property", null, "(?i)(?m)^[ \\t]*[\\w]+[\\s]*=",
            null, "(?i)(?m)^[ \\t]*([\\w]+)[\\s]*=",
            null, "procedure", null,
            new()
            {
                BeginPattern = "(?i)(?m)^[ \\t]*defi(?:n(?:e)?)?\\b",
                EndPattern = "(?i)(?m)^[ \\t]*(?:endde(?:f(?:i(?:n(?:e)?)?)?)?|func(?:t(?:i(?:o(?:n)?)?)?)?|proc(?:e(?:d(?:u(?:r(?:e)?)?)?)?)?)\\b"
            });
        AssertNode(result, 3, "define", null, "(?i)(?m)^[\\s]*defi(?:n(?:e)?)?[\\s]+([\\w]+)\\b",
            "(?i)(?m)^[\\s]*defi(?:n(?:e)?)?\\b",
            null, "(?i)(?m)^[\\s]*defi(?:n(?:e)?)?[\\s]+[\\w]+[\\s]+([\\w]+)",
            "(?i)(?m)^[\\s]*endde(?:f(?:i(?:n(?:e)?)?)?)?\\b",
            null, "procedure", null);
        AssertNode(result, 4, "prg", "prg", null, "(?i)(?m)^[\\s][^\\*&]+",
            "{FileName}", null, "$", null,
            "procedure", null);
        AssertNode(result, 5, "vfp_record", "vfp_record", null, "(?m)^\\[ RECORD\\]\\r?\\n\\[PLATFORM\\] WINDOWS \\r?\\n",
            null, "(?m)^\\[OBJNAME\\] ([\\w]+)\\r?\\n", "", "vfp_record",
            "procedure", null);

        // RuleSets
        Assert.AreEqual(2, result.RuleSets.Count);
        AssertRuleSet(result, 0, "VFP Projectfile", ".pja", "vfp_record");
        AssertRuleSet(result, 1, "VFP Prg", ".prg", "prg");
    }

    private static void AssertNode(SemanticParser.Config.ParserSetting result,
                            int index,
                            string key,
                            string? type,
                            string? typePattern,
                            string beginPattern,
                            string? name,
                            string? namePattern,
                            string? endPatter,
                            string? endOn, string? subNodes,
                            SemanticParser.Config.ContainerSetting? container) =>
        Assert.AreEqual((key, type, typePattern, beginPattern, name, namePattern, endPatter, endOn, subNodes, container?.BeginPattern, container?.EndPattern),
                                       (result.Nodes[index].Key,
                                       result.Nodes[index].Type,
                                       result.Nodes[index].TypePattern,
                                       result.Nodes[index].BeginPattern,
                                       result.Nodes[index].Name,
                                       result.Nodes[index].NamePattern,
                                       result.Nodes[index].EndPattern,
                                       result.Nodes[index].EndOn?.FirstOrDefault(),
                                       result.Nodes[index].SubNodes?.FirstOrDefault(),
                                       result.Nodes[index].OnlyWithin?.BeginPattern,
                                       result.Nodes[index].OnlyWithin?.EndPattern
            ));

    private static void AssertRuleSet(SemanticParser.Config.ParserSetting result,
                                      int index,
                                      string name,
                                      string extension,
                                      string node) =>
        Assert.AreEqual((name, extension, node),
                            (result.RuleSets[index].Name,
                            result.RuleSets[index].Extensions?.FirstOrDefault(),
                            result.RuleSets[index].RootNodes?.FirstOrDefault()));
}
