namespace SemanticParser.Brokers;
internal class FileBroker : IFileBroker
{
    public bool Exists(string path) => File.Exists(path);

    public void Create(string path) => File.Create(path);

    public void Delete(string path) => File.Delete(path);

    public string ReadAllText(string path) => File.ReadAllText(path);

    public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);
}
