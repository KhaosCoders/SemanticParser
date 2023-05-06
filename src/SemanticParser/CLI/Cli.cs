using CommandLine;
using SemanticParser.Parser;
using Serilog;

namespace SemanticParser.CLI;
internal class Cli : ICli
{
    private readonly IParserLoop _parserLoop;

    public Cli(IParserLoop parserLoop)
    {
        this._parserLoop = parserLoop;
    }

    public void Parse(string[] args) =>
        CommandLine.Parser.Default.ParseArguments<ShellOptions, OtherOptions>(args)
            .WithParsed<ShellOptions>(opts =>
            {
                this._parserLoop.Loop(opts.FlagFilePath!);
                Environment.Exit(0);
            })
            .WithParsed<OtherOptions>(_ => {
                Console.WriteLine("Not supported");
                Environment.Exit(1);
            })
            .WithNotParsed(errors =>
            {
                errors.ToList().ForEach(error => Log.Error("Error: {Tag}", error.Tag));
                Environment.Exit(1);
            });
}
