using SemanticParser.Config;
using SemanticParser.Parser;

namespace SemanticParser.Tests.Parser;
internal static class TestData
{
    public static string TestFileContent { get; } =
"""
* Dummy File Header
parameters lnP1

return lnP1

* test procedure
procedure func
parameters p1, p2
return p1 + p2

* test class
define class test as cusom
    nProperty1 = 1
    cProperty2 = "2" && inline comment

    procedure init

    endproc

    procedure method

    endproc
enddef

""";

    public static NodeDefinition PrgNode = new(new()
    {
        Key = "prg",
        Type = "prg",
        Name = "{FileName}",
        OnlyFirst = true,
        BeginPattern = "(?i)(?m)^[\\s]*[^\\*&]+",
        EndPattern = "\\n$"
    });

    public static NodeDefinition ProcNode = new(new()
    {
        Key = "proc",
        Type = "proc",
        BeginPattern = "(?i)(?m)^[ \\t]*proc(?:e(?:d(?:u(?:r(?:e)?)?)?)?)?\\b",
        NamePattern = "(?i)(?m)^[ \\t]*proc(?:e(?:d(?:u(?:r(?:e)?)?)?)?)?[\\s]+([\\w]+)",
        EndPattern = "(?i)(?m)^[\\s]*endproc\\b.*?(?=[\\n$])"
    });

    public static NodeDefinition DefineNode = new(new()
    {
        Key = "define",
        BeginPattern = "(?i)(?m)^[ \\t]*defi(?:n(?:e)?)?\\b",
        TypePattern = "(?i)(?m)^[ \\t]*defi(?:n(?:e)?)?[\\s]+([\\w]+)\\b",
        NamePattern = "(?i)(?m)^[ \\t]*defi(?:n(?:e)?)?[\\s]+[\\w]+[\\s]+([\\w]+)",
        EndPattern = "(?i)(?m)^[\\s]*endde(?:f(?:i(?:n(?:e)?)?)?)?\\b.*?(?=[\\n$])"
    });

    public static RuleSet Ruleset = new(new()
    {
        Name = "TestRuleSet",
        Extensions = new() { ".txt" }
    });

    static TestData()
    {
        DefineNode.SubNodes.Add(ProcNode);
        ProcNode.EndOn.AddRange(new[] { ProcNode, DefineNode });
        PrgNode.SubNodes.AddRange(new[] { ProcNode, DefineNode });
        Ruleset.RootNodes.Add(PrgNode);
    }

    public static ParserContext CreateCxt() =>
        new(TestFileContent, "TestFile.txt", Ruleset);
}
