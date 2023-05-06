namespace SemanticParser.Brokers;
internal class ConsoleBroker : IConsoleBroker
{
    public string? ReadLine() => Console.ReadLine();
}
