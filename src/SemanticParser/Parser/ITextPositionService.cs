namespace SemanticParser.Parser;
internal interface ITextPositionService
{
    void CalculateSpans(ref ParserContext ctx);
}
