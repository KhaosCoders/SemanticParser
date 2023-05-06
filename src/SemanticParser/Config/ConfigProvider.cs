using Microsoft.Extensions.Configuration;

namespace SemanticParser.Config;
internal class ConfigProvider : IConfigProvider
{
    private readonly IConfigurationBuilder _configurationBuilder;

    public ConfigProvider(IConfigurationBuilder configurationBuilder)
    {
        this._configurationBuilder = configurationBuilder;
    }

    private ParserSetting? parserSetting;
    private ParserConfig? parserConfig;

    public ParserSetting ParserSetting => this.parserSetting ??= this.LoadParserSetting();

    public ParserConfig Config => this.parserConfig ??= new(this.ParserSetting);

    private ParserSetting LoadParserSetting()
    {
        // Setup appsettings
        var config = this._configurationBuilder.Build();

        // Bind parser settings
        ParserSetting parserSettings = new();
        config.GetSection("Parser").Bind(parserSettings);

        return parserSettings;
    }
}
