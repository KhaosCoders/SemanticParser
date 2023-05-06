namespace SemanticParser.Model;
public record ParsingError(string Message, int[] Location);
