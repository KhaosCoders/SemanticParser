using Microsoft.Extensions.Configuration;

namespace SemanticParser.Config;
public interface IConfigurationBuilder
{
    IConfigurationRoot Build();
}
