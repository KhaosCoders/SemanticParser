namespace SemanticParser.CLI;
public interface IReadyFlagFileFactory
{
    IReadyFlagFile Create(string flagFilePath);
}
