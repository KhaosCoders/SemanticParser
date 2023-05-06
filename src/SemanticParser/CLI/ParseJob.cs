namespace SemanticParser.CLI;
internal class ParseJob : IParseJob
{
    public string? InputFile { get; set; }
    public string? OutputFile { get; set; }
    public string? Encoding { get; set; }

    public void Validate()
    {
        ArgumentException.ThrowIfNullOrEmpty(this.InputFile, nameof(this.InputFile));
        ArgumentException.ThrowIfNullOrEmpty(this.OutputFile, nameof(this.OutputFile));
        ArgumentException.ThrowIfNullOrEmpty(this.Encoding, nameof(this.Encoding));

        if (!File.Exists(this.InputFile))
        {
            throw new FileNotFoundException($"Input file {this.InputFile} does not exist.");
        }

        System.Text.Encoding.GetEncoding(this.Encoding);
    }
}
