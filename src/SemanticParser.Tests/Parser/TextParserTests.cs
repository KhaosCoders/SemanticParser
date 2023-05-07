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
        Assert.AreEqual(7, ctx.NodeSpans.Count);
        var prg = ctx.NodeSpans[0];
        Assert.AreEqual("prg", prg.Type);
        Assert.AreEqual("{FileName}", prg.Name);
        Assert.AreEqual(21, prg.BeginIndex);
        Assert.AreEqual(334, prg.EndIndex);

        var procFunc = ctx.NodeSpans[1];
        Assert.AreEqual("proc", procFunc.Type);
        Assert.AreEqual("func", procFunc.Name);
        Assert.AreEqual(82, procFunc.BeginIndex);
        Assert.AreEqual(148, procFunc.EndIndex);

        var classTest = ctx.NodeSpans[2];
        Assert.AreEqual("class", classTest.Type);
        Assert.AreEqual("test", classTest.Name);
        Assert.AreEqual(149, classTest.BeginIndex);
        Assert.AreEqual(333, classTest.EndIndex);

        var prop1 = ctx.NodeSpans[3];
        Assert.AreEqual("property", prop1.Type);
        Assert.AreEqual("nProperty1",prop1.Name);
        Assert.AreEqual(177, prop1.BeginIndex);
        Assert.AreEqual(196, prop1.EndIndex);

        var prop2 = ctx.NodeSpans[4];
        Assert.AreEqual("property", prop2.Type);
        Assert.AreEqual("cProperty2", prop2.Name);
        Assert.AreEqual(197, prop2.BeginIndex);
        Assert.AreEqual(238, prop2.EndIndex);

        var procInit = ctx.NodeSpans[5];
        Assert.AreEqual("proc", procInit.Type);
        Assert.AreEqual("init", procInit.Name);
        Assert.AreEqual(239, procInit.BeginIndex);
        Assert.AreEqual(288, procInit.EndIndex);

        var procMethod = ctx.NodeSpans[6];
        Assert.AreEqual("proc", procMethod.Type);
        Assert.AreEqual("method", procMethod.Name);
        Assert.AreEqual(289, procMethod.BeginIndex);
        Assert.AreEqual(325, procMethod.EndIndex);

        // Assert hierarchy
        Assert.AreEqual(2, prg.SubNodes.Count);
        Assert.AreEqual(procFunc, prg.SubNodes[0]);
        Assert.AreEqual(prg, procFunc.ParentNode);

        Assert.AreEqual(classTest, prg.SubNodes[1]);
        Assert.AreEqual(prg, classTest.ParentNode);

        Assert.AreEqual(4, classTest.SubNodes.Count);
        Assert.AreEqual(prop1, classTest.SubNodes[0]);
        Assert.AreEqual(classTest, prop1.ParentNode);
        Assert.AreEqual(prop2, classTest.SubNodes[1]);
        Assert.AreEqual(classTest, prop2.ParentNode);
        Assert.AreEqual(procInit, classTest.SubNodes[2]);
        Assert.AreEqual(classTest, procInit.ParentNode);
        Assert.AreEqual(procMethod, classTest.SubNodes[3]);
        Assert.AreEqual(classTest, procMethod.ParentNode);
    }
}
