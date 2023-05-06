namespace SemanticParser.Brokers;
public interface IFileBroker
{
    void Create(string path);
    void Delete(string path);
    bool Exists(string path);
    string ReadAllText(string path);
    void WriteAllText(string path, string contents);
}
