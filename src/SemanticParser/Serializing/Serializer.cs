using Serilog;

namespace SemanticParser.Serializing;
internal class Serializer : ISerializer
{
    private readonly YamlContext _serializeIt = new();

    public string Serialize(Model.File file)
    {
        Log.Debug("Serialize file: {@File}", file);
        string yaml = this._serializeIt.File.SerializeDocument(file);
        Log.Debug("Output file size: {OutputSize}", yaml.Length);
        return yaml;
    }

    public void Serialize(Model.File file, string filePath)
    {
        string text = this.Serialize(file);
        File.WriteAllText(filePath, text);
    }
}
