namespace SemanticParser.Brokers;
internal class PathBroker : IPathBroker
{
    public string GetFullPath(string path) => Path.GetFullPath(path);
}
