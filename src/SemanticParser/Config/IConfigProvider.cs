namespace SemanticParser.Config;
internal interface IConfigProvider
{
    ParserSetting ParserSetting { get; }
    ParserConfig Config { get; }
}
