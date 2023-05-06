using SemanticParser.Brokers;
using Serilog;

namespace SemanticParser.CLI;
internal class ConsoleReader : IConsoleReader
{
    internal const string EndCommand = "end";

    private readonly IConsoleBroker _console;
    private readonly IPathBroker _path;

    public ConsoleReader(IConsoleBroker console, IPathBroker path)
    {
        this._console = console;
        this._path = path;
    }

    public IParseJob? ReadJobOrEnd()
    {
        Log.Debug("Waiting for next parse job...");
        string? firstLine = this._console.ReadLine();
        if (firstLine == EndCommand)
        {
            Log.Debug("End command received.");
            return null;
        }

        return new ParseJob()
        {
            InputFile = this.NormalizePath(firstLine),
            Encoding = this._console.ReadLine(),
            OutputFile = this.NormalizePath(this._console.ReadLine()),
        };
    }

    private string? NormalizePath(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return path;
        }

        if (path.StartsWith('"') && path.EndsWith('"'))
        {
            path = path[1..^1];
        }
        else if (path.StartsWith('\'') && path.EndsWith('\''))
        {
            path = path[1..^1];
        }

        return this._path.GetFullPath(path);
    }
}
