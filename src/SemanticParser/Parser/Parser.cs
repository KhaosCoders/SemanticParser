using SemanticParser.Config;
using Serilog;
using System.Text;

namespace SemanticParser.Parser;
internal class Parser : IParser
{
    private readonly IConfigProvider _configProvider;
    private readonly ITextParser _textParser;
    private readonly ITextPositionService _textPositionService;
    private readonly IModelMapper _modelMapper;

    public Parser(
        IConfigProvider configProvider,
        ITextParser textParser,
        ITextPositionService textPositionService,
        IModelMapper modelMapper)
    {
        this._configProvider = configProvider;
        this._textParser = textParser;
        this._textPositionService = textPositionService;
        this._modelMapper = modelMapper;
    }

    public Model.File Parse(string filePath, Encoding encoding)
    {
        string input = File.ReadAllText(filePath, encoding);
        Log.Debug("Input file size: {InputSize}", input.Length);

        ParserContext ctx = new(input, filePath, this.FindRuleSet(filePath));

        // Step 1: Parse text file to nodes
        this._textParser.ParseNodes(ref ctx);

        // Step 2: Calulate lines and char positions
        this._textPositionService.CalculateSpans(ref ctx);

        // Step 3: Map to final models
        this._modelMapper.Map(ref ctx);

        return ctx.File!;
    }

    private RuleSet FindRuleSet(string fileName)
    {
        string extension = Path.GetExtension(fileName);
        return this._configProvider.Config.GetRuleSet(extension);
    }
}
