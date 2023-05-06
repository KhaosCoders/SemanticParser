using SemanticParser.Config;

namespace SemanticParser.Tests.Config;
[TestClass]
public class RuleSetTests
{
    [TestMethod]
    public void Constructor_ThrowsArgumentNullExceptionForNullSetting()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => new RuleSet(null!));
    }

    [TestMethod]
    public void Constructor_ThrowsArgumentNullExceptionForNullSettingName()
    {
        // Arrange
        var setting = new RuleSetSetting { Name = null, Extensions = new List<string>() };

        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => new RuleSet(setting));
    }

    [TestMethod]
    public void Constructor_ThrowsArgumentNullExceptionForNullExtensions()
    {
        // Arrange
        var setting = new RuleSetSetting { Name = "Test", Extensions = null };

        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => new RuleSet(setting));
    }

    [TestMethod]
    public void Name_ReturnsNameFromSetting()
    {
        // Arrange
        var setting = new RuleSetSetting { Name = "Test", Extensions = new List<string>() };
        var ruleSet = new RuleSet(setting);

        // Act
        var name = ruleSet.Name;

        // Assert
        Assert.AreEqual(setting.Name, name);
    }

    [TestMethod]
    public void Extensions_ReturnsExtensionsFromSetting()
    {
        // Arrange
        var expectedExtensions = new List<string> { "txt", "md" };
        var setting = new RuleSetSetting { Name = "Test", Extensions = expectedExtensions };
        var ruleSet = new RuleSet(setting);

        // Act
        var extensions = ruleSet.Extensions;

        // Assert
        CollectionAssert.AreEqual(expectedExtensions, extensions);
    }

    [TestMethod]
    public void AllDistinctNodes_ReturnsEmptyListForEmptyRootNodes()
    {
        // Arrange
        var setting = new RuleSetSetting { Name = "Test", Extensions = new List<string>() };
        var ruleSet = new RuleSet(setting);

        // Act
        var nodes = ruleSet.AllDistinctNodes();

        // Assert
        Assert.AreEqual(0, nodes.Count());
    }

    [TestMethod]
    public void AllDistinctNodes_ReturnsDistinctNodesForRootNode()
    {
        // Arrange
        var node1 = this.CreateNode("node1", new string[0]);
        var node2 = this.CreateNode("node2", new string[0]);
        var root = this.CreateNode("root", new string[] { "node1", "node2" });
        root.SubNodes.Add(node1);
        root.SubNodes.Add(node2);
        node1.EndOn.Add(node2);
        node2.EndOn.Add(node1);

        var setting = new RuleSetSetting { Name = "Test", Extensions = new List<string>() };
        var ruleSet = new RuleSet(setting);
        ruleSet.RootNodes.Add(root);

        // Act
        var nodes = ruleSet.AllDistinctNodes();

        // Assert
        Assert.AreEqual(3, nodes.Count());
        Assert.IsTrue(nodes.Contains(root));
        Assert.IsTrue(nodes.Contains(node1));
        Assert.IsTrue(nodes.Contains(node2));
    }

    private NodeDefinition CreateNode(string key, params string[] subNodes) =>
        new (new()
        {
            Key = key,
            SubNodes = subNodes.ToList(),
            BeginPattern = "",
            Name = key,
            EndPattern = "",
            Type = "test",
        });
}
