using Microsoft.Extensions.Configuration;

namespace SemanticParser.Config;
internal class ConfigurationBuilder : IConfigurationBuilder
{
    public IConfigurationRoot Build() =>
        new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
}
