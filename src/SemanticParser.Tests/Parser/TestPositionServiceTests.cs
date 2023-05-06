using SemanticParser.Parser;

namespace SemanticParser.Tests.Parser;
[TestClass]
public class TestPositionServiceTests
{
    [TestMethod]
    public void CalculateSpans_ShouldFindStartAndEndPositions()
    {
        // Arrange
        var ctx = TestData.CreateCxt();
        ctx.NodeSpans.Add(new ParserNodeSpan(TestData.PrgNode, 21, "prg", "prg") { EndIndex = 312 });
        ctx.NodeSpans.Add(new ParserNodeSpan(TestData.ProcNode, 73, "func", "proc") { EndIndex = 139 });
        ctx.NodeSpans.Add(new ParserNodeSpan(TestData.DefineNode, 140, "test", "class") { EndIndex = 311 });
        ctx.NodeSpans.Add(new ParserNodeSpan(TestData.ProcNode, 230, "init", "proc") { EndIndex = 266 });
        ctx.NodeSpans.Add(new ParserNodeSpan(TestData.ProcNode, 267, "method", "proc") { EndIndex = 303 });

        TextPositionService service = new();

        // Act
        service.CalculateSpans(ref ctx);

        // Assert
        AssertPosition(ctx.NodeSpans[0], 2, 0, 23, 8);
        AssertPosition(ctx.NodeSpans[1], 7, 0, 11, 14);
        AssertPosition(ctx.NodeSpans[2], 12, 0, 23, 8);
        AssertPosition(ctx.NodeSpans[3], 16, 0, 19, 2);
        AssertPosition(ctx.NodeSpans[4], 20, 0, 22, 13);
    }

    private static void AssertPosition(ParserNodeSpan span, int startLine, int startChar, int endLine, int endChar)
    {
        Assert.AreEqual((startLine, startChar), (span.BeginLine, span.BeginCharPos));
        Assert.AreEqual((endLine, endChar), (span.EndLine, span.EndCharPos));
    }
}
