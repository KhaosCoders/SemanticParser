using CommandLine;

namespace SemanticParser.CLI;

[Verb("shell", HelpText = "First verb is always shell.")]
public class ShellOptions
{
    [Value(0, MetaName = "FlagFile", Required = true, HelpText = "Path to file used as flag.")]
    public string? FlagFilePath { get; set; }
}
