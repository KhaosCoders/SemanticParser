using SemanticParser.Parser;

namespace SemanticParser.Tests.Parser;
[TestClass]
public class ModelMapperTests
{
    [TestMethod]
    public void Map_ShouldMapSpansToNodes()
    {
        // Arrange
        var ctx = TestData.CreateCxt();
        ctx.NodeSpans.Add(new ParserNodeSpan(TestData.PrgNode, 21, "prg", "prg")
        {
            EndIndex = 312,
            BeginLine = 2,
            BeginCharPos = 0,
            EndLine = 23,
            EndCharPos = 8
        });
        ctx.NodeSpans.Add(new ParserNodeSpan(TestData.ProcNode, 73, "func", "proc")
        {
            EndIndex = 139,
            BeginLine = 7,
            BeginCharPos = 0,
            EndLine = 11,
            EndCharPos = 14
        });
        ctx.NodeSpans.Add(new ParserNodeSpan(TestData.DefineNode, 140, "test", "class")
        {
            EndIndex = 311,
            BeginLine = 12,
            BeginCharPos = 0,
            EndLine = 23,
            EndCharPos = 8
        });
        ctx.NodeSpans.Add(new ParserNodeSpan(TestData.ProcNode, 230, "init", "proc")
        {
            EndIndex = 266,
            BeginLine = 16,
            BeginCharPos = 0,
            EndLine = 19,
            EndCharPos = 2
        });
        ctx.NodeSpans.Add(new ParserNodeSpan(TestData.ProcNode, 267, "method", "proc")
        {
            EndIndex = 303,
            BeginLine = 20,
            BeginCharPos = 0,
            EndLine = 22,
            EndCharPos = 13
        });

        ctx.NodeSpans[0].AddSubNode(ctx.NodeSpans[1]);
        ctx.NodeSpans[0].AddSubNode(ctx.NodeSpans[2]);
        ctx.NodeSpans[2].AddSubNode(ctx.NodeSpans[3]);
        ctx.NodeSpans[2].AddSubNode(ctx.NodeSpans[4]);

        ModelMapper mapper = new();

        // Act
        mapper.Map(ref ctx);

        // Assert File
        Assert.IsNotNull(ctx.File);
        AssertLocation(ctx.File.LocationSpan, 1, 0, 23, 8);

        // Assert Prg node
        Assert.IsNotNull(ctx.File.Children);
        Assert.AreEqual(1, ctx.File.Children.Count);
        var prgNode = ctx.File.Children[0] as Model.Container;
        Assert.IsNotNull(prgNode);
        AssertLocation(prgNode.LocationSpan, 2, 0, 23, 8);

        // Assert Func node
        Assert.IsNotNull(prgNode.Children);
        Assert.AreEqual(2, prgNode.Children.Count);
        var funcNode = prgNode.Children[0] as Model.Node;
        Assert.IsNotNull(funcNode);
        AssertLocation(funcNode.LocationSpan, 7, 0, 11, 14);

        // Assert Class node
        var classNode = prgNode.Children[1] as Model.Container;
        Assert.IsNotNull(classNode);
        AssertLocation(classNode.LocationSpan, 12, 0, 23, 8);

        // Assert Init node
        Assert.IsNotNull(classNode.Children);
        Assert.AreEqual(2, classNode.Children.Count);
        var initNode = classNode.Children[0] as Model.Node;
        Assert.IsNotNull(initNode);
        AssertLocation(initNode.LocationSpan, 16, 0, 19, 2);

        // Assert Method node
        var methodNode = classNode.Children[1] as Model.Node;
        Assert.IsNotNull(methodNode);
        AssertLocation(methodNode.LocationSpan, 20, 0, 22, 13);
    }

    public static void AssertLocation(
        Model.LocationSpan location,
        int startLine,
        int startColumn,
        int endLine,
        int endColumn)
    {
        Assert.AreEqual(startLine, location.StartLine);
        Assert.AreEqual(startColumn, location.StartColumn);
        Assert.AreEqual(endLine, location.EndLine);
        Assert.AreEqual(endColumn, location.EndColumn);
    }
}
