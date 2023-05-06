using System.Text;

namespace SemanticParser.Parser;
public interface IParser
{
    Model.File Parse(string filePath, Encoding encoding);
}
