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
        ctx.NodeSpans.Add(new ParserNodeSpan(TestData.PrgNode, 21, "prg", "prg") { EndIndex = 333 });
        ctx.NodeSpans.Add(new ParserNodeSpan(TestData.ProcNode, 82, "func", "proc") { EndIndex = 148 });
        ctx.NodeSpans.Add(new ParserNodeSpan(TestData.DefineNode, 149, "test", "class") { EndIndex = 333 });
        ctx.NodeSpans.Add(new ParserNodeSpan(TestData.PropertyNode, 177, "nProperty1", "property") { EndIndex = 196 });
        ctx.NodeSpans.Add(new ParserNodeSpan(TestData.PropertyNode, 197, "cProperty2", "property") { EndIndex = 238 });
        ctx.NodeSpans.Add(new ParserNodeSpan(TestData.ProcNode, 239, "init", "proc") { EndIndex = 288 });
        ctx.NodeSpans.Add(new ParserNodeSpan(TestData.ProcNode, 289, "method", "proc") { EndIndex = 325 });

        TextPositionService service = new();

        // Act
        service.CalculateSpans(ref ctx);

        // Assert
        Assert.AreEqual(7, ctx.NodeSpans.Count);
        AssertPosition(ctx.NodeSpans[0], 2, 0, 23, 8);
        AssertPosition(ctx.NodeSpans[1], 7, 0, 11, 14);
        AssertPosition(ctx.NodeSpans[2], 12, 0, 23, 8);
        AssertPosition(ctx.NodeSpans[3], 13, 0, 13, 20);
        AssertPosition(ctx.NodeSpans[4], 14, 0, 15, 2);
        AssertPosition(ctx.NodeSpans[5], 16, 0, 19, 2);
        AssertPosition(ctx.NodeSpans[6], 20, 0, 22, 13);
    }

    private static void AssertPosition(ParserNodeSpan span, int startLine, int startChar, int endLine, int endChar)
    {
        Assert.AreEqual((startLine, startChar), (span.BeginLine, span.BeginCharPos));
        Assert.AreEqual((endLine, endChar), (span.EndLine, span.EndCharPos));
    }
}
