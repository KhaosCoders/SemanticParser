using SemanticParser.CLI;
using SemanticParser.Serializing;
using Serilog;
using System.Text;

namespace SemanticParser.Parser;
internal class ParserLoop : IParserLoop
{
    private const string OkResult = "OK";
    private const string KoResult = "KO";

    private readonly IParser _parser;
    private readonly IConsoleReader _consoleReader;
    private readonly ISerializer _serializer;
    private readonly IReadyFlagFileFactory _readyFlagFileFactory;

    public ParserLoop(IParser parser,
                      IConsoleReader consoleReader,
                      ISerializer serializer,
                      IReadyFlagFileFactory readyFlagFileFactory)
    {
        this._parser = parser;
        this._consoleReader = consoleReader;
        this._serializer = serializer;
        this._readyFlagFileFactory = readyFlagFileFactory;
    }

    public void Loop(string readyFlagFilePath)
    {
        Log.Debug("Start parsing loop.");

        using var readyFlagFile = this._readyFlagFileFactory.Create(readyFlagFilePath);

        while (true)
        {
            IParseJob? job = null;
            try
            {
                job = this._consoleReader.ReadJobOrEnd();
                if (job == null)
                {
                    Log.Information("End parsing loop.");
                    break;
                }

                this.Execute(job);
                Console.WriteLine(OkResult);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while parsing job: {@Job}", job);
                Console.WriteLine(KoResult);
            }
        }
    }

    private void Execute(IParseJob job)
    {
        Log.Debug("Execute parse job: {@Job}", job);

        job.Validate();

        var file = this._parser.Parse(job.InputFile!, Encoding.GetEncoding(job.Encoding!));

        this._serializer.Serialize(file, job.OutputFile!);
    }
}
