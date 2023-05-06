using Moq;
using SemanticParser.CLI;
using SemanticParser.Parser;
using SemanticParser.Serializing;
using System.Text;

namespace SemanticParser.Tests.Parser;
[TestClass]
public class ParserLoopTests
{
    [TestMethod]
    public async Task ParserLoop_LoopAsync()
    {
        // Arrange
        const string flagFile = "flagFile";

        var job1 = new Mock<IParseJob>();
        job1.SetupGet(x => x.InputFile).Returns("job1-file.prg");
        job1.SetupGet(x => x.Encoding).Returns("utf-8");
        job1.SetupGet(x => x.OutputFile).Returns("outputFile1.yaml");
        job1.Setup(x => x.Validate()).Verifiable();

        var job2 = new Mock<IParseJob>();
        job2.SetupGet(x => x.InputFile).Returns("job2-file.prg");
        job2.SetupGet(x => x.Encoding).Returns("utf-8");
        job2.SetupGet(x => x.OutputFile).Returns("outputFile2.yaml");
        job2.Setup(x => x.Validate()).Verifiable();

        var consoleReader = new Mock<IConsoleReader>();
        consoleReader.SetupSequence(x => x.ReadJobOrEnd())
            .Returns(job1.Object)
            .Returns(job2.Object)
            .Returns((IParseJob)null!);

        var parser = new Mock<IParser>();
        parser.Setup(x => x.Parse(It.IsAny<string>(), It.IsAny<Encoding>()))
            .Returns(new Model.File("dummy"))
            .Verifiable();

        var serializer = new Mock<ISerializer>();
        serializer.Setup(x => x.Serialize(It.IsAny<Model.File>(), It.IsAny<string>()))
            .Verifiable();

        var readyFlagFileFactory = new Mock<IReadyFlagFileFactory>();
        readyFlagFileFactory.Setup(x => x.Create(flagFile))
            .Returns(new DummyFlagFile())
            .Verifiable();

        var loop = new ParserLoop(parser.Object,
            consoleReader.Object,
            serializer.Object,
            readyFlagFileFactory.Object);

        // Act with timeout
        CancellationTokenSource cts = new(TimeSpan.FromSeconds(5));
        await Task.Run(() => loop.Loop(flagFile), cts.Token);

        // Assert
        readyFlagFileFactory.Verify(x => x.Create(flagFile), Times.Once());
        consoleReader.Verify(x => x.ReadJobOrEnd(), Times.Exactly(3));
        job1.Verify(x => x.Validate(), Times.Once());
        job2.Verify(x => x.Validate(), Times.Once());
        parser.Verify(x => x.Parse(It
            .IsAny<string>(), It.IsAny<Encoding>()), Times.Exactly(2));
        serializer.Verify(x => x.Serialize(It.IsAny<Model.File>(), It.IsAny<string>()), Times.Exactly(2));
    }

    private class DummyFlagFile : IReadyFlagFile
    {
        public void Dispose() {}
    }
}
