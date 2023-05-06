using Moq;
using SemanticParser.Brokers;
using SemanticParser.CLI;

namespace SemanticParser.Tests.CLI;
[TestClass]
public class ConsoleReaderTests
{
    [TestMethod]
    public void ReadJobOrEnd_EndCommand_ReturnsNull()
    {
        // Arrange
        var mockConsole = new Mock<IConsoleBroker>();
        mockConsole.SetupSequence(c => c.ReadLine())
            .Returns(ConsoleReader.EndCommand);

        var parser = new ConsoleReader(mockConsole.Object, null!);

        // Act
        var result = parser.ReadJobOrEnd();

        // Assert
        Assert.IsNull(result);
    }

    [DataTestMethod]
    [DataRow("input.txt", "utf-8", "output.yaml", "input.txt", "utf-8", "output.yaml")]
    [DataRow("\"input.txt\"", "utf-8", "\"output.yaml\"", "input.txt", "utf-8", "output.yaml")]
    [DataRow("'input.txt'", "utf-8", "'output.yaml'", "input.txt", "utf-8", "output.yaml")]
    public void ReadJobOrEnd_ValidInput_ReturnsParseJob(string inFile,
                                                        string encoding,
                                                        string outFile,
                                                        string expIn,
                                                        string expEnc,
                                                        string expOut)
    {
        // Arrange
        var mockConsole = new Mock<IConsoleBroker>();
        mockConsole.SetupSequence(c => c.ReadLine())
            .Returns(inFile)
            .Returns(encoding)
            .Returns(outFile);

        var mockPath = new Mock<IPathBroker>();
        mockPath.Setup(c => c.GetFullPath(It.IsAny<string>()))
            .Returns<string>(path => path);

        var parser = new ConsoleReader(mockConsole.Object, mockPath.Object);

        // Act
        var result = parser.ReadJobOrEnd();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expIn, result.InputFile);
        Assert.AreEqual(expEnc, result.Encoding);
        Assert.AreEqual(expOut, result.OutputFile);
    }
}
