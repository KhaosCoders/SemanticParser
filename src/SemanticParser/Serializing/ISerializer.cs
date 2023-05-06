namespace SemanticParser.Serializing;
public interface ISerializer
{
    string Serialize(Model.File file);
    void Serialize(Model.File file, string filePath);
}
