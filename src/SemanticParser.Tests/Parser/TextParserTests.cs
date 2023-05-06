using SemanticParser.Parser;

namespace SemanticParser.Tests.Parser;
[TestClass]
public class TextParserTests
{
    [TestMethod]
    public void ParseNodes_ShouldParseNodes()
    {
        // Arrange
        var ctx = TestData.CreateCxt();
        TextParser parser = new();

        // Act
        parser.ParseNodes(ref ctx);

        // Assert nodes
        Assert.AreEqual(5, ctx.NodeSpans.Count);
        var prg = ctx.NodeSpans[0];
        Assert.AreEqual("prg", prg.Type);
        Assert.AreEqual(21, prg.BeginIndex);
        Assert.AreEqual(312, prg.EndIndex);

        var procFunc = ctx.NodeSpans[1];
        Assert.AreEqual("proc", procFunc.Type);
        Assert.AreEqual(73, procFunc.BeginIndex);
        Assert.AreEqual(139, procFunc.EndIndex);

        var classTest = ctx.NodeSpans[2];
        Assert.AreEqual("class", classTest.Type);
        Assert.AreEqual(140, classTest.BeginIndex);
        Assert.AreEqual(311, classTest.EndIndex);

        var procInit = ctx.NodeSpans[3];
        Assert.AreEqual("proc", procInit.Type);
        Assert.AreEqual(230, procInit.BeginIndex);
        Assert.AreEqual(266, procInit.EndIndex);

        var procMethod = ctx.NodeSpans[4];
        Assert.AreEqual("proc", procMethod.Type);
        Assert.AreEqual(267, procMethod.BeginIndex);
        Assert.AreEqual(303, procMethod.EndIndex);

        // Assert hierarchy
        Assert.AreEqual(2, prg.SubNodes.Count);
        Assert.AreEqual(procFunc, prg.SubNodes[0]);
        Assert.AreEqual(prg, procFunc.ParentNode);

        Assert.AreEqual(classTest, prg.SubNodes[1]);
        Assert.AreEqual(prg, classTest.ParentNode);

        Assert.AreEqual(2, classTest.SubNodes.Count);
        Assert.AreEqual(procInit, classTest.SubNodes[0]);
        Assert.AreEqual(classTest, procInit.ParentNode);

        Assert.AreEqual(procMethod, classTest.SubNodes[1]);
        Assert.AreEqual(classTest, procMethod.ParentNode);
    }
}
