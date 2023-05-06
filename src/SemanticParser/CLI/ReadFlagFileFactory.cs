using SemanticParser.Brokers;
using Serilog;

namespace SemanticParser.CLI;
internal class ReadyFlagFileFactory : IReadyFlagFileFactory
{
    private readonly IFileBroker _fileBroker;

    public ReadyFlagFileFactory(IFileBroker fileBroker)
    {
        this._fileBroker = fileBroker;
    }

    public IReadyFlagFile Create(string flagFilePath)
    {
        Log.Debug("Ready-Flag file: {FlagFile}", flagFilePath);
        ReadyFlagFile file = new(flagFilePath, this._fileBroker);
        file.Create();
        return file;
    }
}
