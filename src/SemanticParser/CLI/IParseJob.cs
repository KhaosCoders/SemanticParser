namespace SemanticParser.CLI;

public interface IParseJob
{
    string? Encoding { get; set; }
    string? InputFile { get; set; }
    string? OutputFile { get; set; }

    void Validate();
}