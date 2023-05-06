namespace SemanticParser.Parser;
internal class TextPositionService : ITextPositionService
{
    enum EPosition { Start, End }

    public void CalculateSpans(ref ParserContext ctx)
    {
        int[] lineBreaks = FindLineBreaks(ctx);

        foreach(var nodeSpan in ctx.NodeSpans)
        {
            // Start position
            (nodeSpan.BeginLine, nodeSpan.BeginCharPos) = GetPosition(lineBreaks, nodeSpan.BeginIndex, EPosition.Start);

            // End position
            int endIndex = nodeSpan.EndIndex ?? ctx.InputText.Length;
            if (endIndex == ctx.InputText.Length)
            {
                endIndex--;
            }

            (nodeSpan.EndLine, nodeSpan.EndCharPos) = GetPosition(lineBreaks, endIndex, EPosition.End);
        }
    }

    private static (int Line, int CharPos) GetPosition(int[] lineBreaks, int characterPosIndex, EPosition position)
    {
        if (position == EPosition.Start)
        {
            // We want the line number before the character for the beginning of the span
            characterPosIndex--;
        }

        int index = Array.BinarySearch(lineBreaks, characterPosIndex);
        if (index < -1)
        {
            // Positions between line breaks are returned as negative numbers
            index = ~index - 1;
        }
        else if (position == EPosition.End && index >= 0)
        {
            // Positions of line breaks are returned as positive numbers
            // We want the index before the line break for the end of the span
            index--;
        }

        if (index < 0)
        {
            throw new ParserException($"Unable to find line number for index: {characterPosIndex}");
        }

        int prevEndIndex = lineBreaks[index];
        return new (index + 1, characterPosIndex - prevEndIndex);
    }

    private static int[] FindLineBreaks(ParserContext ctx)
    {
        List<int> lineBreaks = new() { 0 };

        // All \n are line breaks
        for (int i = 0; i < ctx.InputSpan.Length; i++)
        {
            if (ctx.InputSpan[i] == '\n')
            {
                lineBreaks.Add(i);
            }
        }

        // Add file end (for BinarySearch compatibility)
        lineBreaks.Add(ctx.InputSpan.Length);

        return lineBreaks.ToArray();
    }
}
