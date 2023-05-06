using SemanticParser.Brokers;

namespace SemanticParser.CLI;
internal class ReadyFlagFile : IReadyFlagFile
{
    private readonly IFileBroker _fileBroker;

    public string FilePath { get; set; }

    public ReadyFlagFile(string filePath, Brokers.IFileBroker fileBroker)
    {
        this.FilePath = filePath;
        this._fileBroker = fileBroker;
    }

    public void Create()
    {
        this.Delete();
        this._fileBroker.WriteAllText(this.FilePath, "ready");
    }

    public void Delete()
    {
        if (this.FilePath != null && this._fileBroker.Exists(this.FilePath))
        {
            this._fileBroker.Delete(this.FilePath);
        }
    }

    public void Dispose() => this.Delete();
}
