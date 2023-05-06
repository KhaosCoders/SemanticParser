using Autofac;
using SemanticParser.Brokers;
using SemanticParser.CLI;
using SemanticParser.Config;
using SemanticParser.Parser;
using SemanticParser.Serializing;
using Serilog;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

[assembly:InternalsVisibleTo("SemanticParser.Tests")]

// Setup logging
Log.Logger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Debug()
    .WriteTo.Debug()
#else
    .MinimumLevel.Information()
#endif
    .WriteTo.File("SemanticParser.log",
                  fileSizeLimitBytes: 1024 * 1024 * 100,  // 100MB
                  rollOnFileSizeLimit: true,
                  retainedFileCountLimit: 10)
    .CreateLogger();

try
{
    Log.Information("SemanticParser {Version} started", Assembly.GetExecutingAssembly().GetName().Version);

    // Enable windows-1252 encoding
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    // Setup DI
    var services = new ContainerBuilder();
    services.RegisterType<ConsoleBroker>().As<IConsoleBroker>().SingleInstance();
    services.RegisterType<PathBroker>().As<IPathBroker>().SingleInstance();
    services.RegisterType<FileBroker>().As<IFileBroker>().SingleInstance();
    services.RegisterType<ConfigurationBuilder>().As<IConfigurationBuilder>().SingleInstance();

    services.RegisterType<ConfigProvider>().As<IConfigProvider>().SingleInstance();
    services.RegisterType<Cli>().As<ICli>().SingleInstance();
    services.RegisterType<ConsoleReader>().As<IConsoleReader>().SingleInstance();
    services.RegisterType<ReadyFlagFileFactory>().As<IReadyFlagFileFactory>().SingleInstance();
    services.RegisterType<ParserLoop>().As<IParserLoop>().SingleInstance();
    services.RegisterType<Serializer>().As<ISerializer>().SingleInstance();
    services.RegisterType<Parser>().As<IParser>().SingleInstance();
    services.RegisterType<TextParser>().As<ITextParser>().SingleInstance();
    services.RegisterType<TextPositionService>().As<ITextPositionService>().SingleInstance();
    services.RegisterType<ModelMapper>().As<IModelMapper>().SingleInstance();

    // Run cli commands
    services.Build()
        .Resolve<ICli>()
        .Parse(args);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Fatal error");
    Environment.Exit(1);
}
