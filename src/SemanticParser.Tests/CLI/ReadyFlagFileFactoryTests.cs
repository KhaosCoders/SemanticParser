using Moq;
using SemanticParser.Brokers;
using SemanticParser.CLI;

namespace SemanticParser.Tests.CLI;
[TestClass]
public class ReadyFlagFileFactoryTests
{
    [TestMethod]
    public void ReadyFlagFileFactory_Create_ShuldReturnReadyFlagFile()
    {
        bool createdCalled = false;

        // Arrange
        var fileBroker = new Mock<IFileBroker>();
        fileBroker.Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(() => createdCalled)
            .Verifiable();
        fileBroker.Setup(x => x.Create(It.IsAny<string>()))
            .Callback(() => createdCalled = true)
            .Verifiable();
        fileBroker.Setup(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback(() => createdCalled = true)
            .Verifiable();
        fileBroker.Setup(x => x.Delete(It.IsAny<string>()))
            .Verifiable();

        var factory = new ReadyFlagFileFactory(fileBroker.Object);

        // Act
        var file = factory.Create("test");

        // Assert
        Assert.IsInstanceOfType(file, typeof(ReadyFlagFile));
        Assert.IsTrue(createdCalled);

        fileBroker.Verify(x => x.Exists(It.IsAny<string>()), Times.Between(1, 2, Moq.Range.Inclusive));
        fileBroker.Verify(x => x.Create(It.IsAny<string>()), Times.Never);
        fileBroker.Verify(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        fileBroker.Verify(x => x.Delete(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void ReadyFlagFile_Dispose_ShouldDeleteFile()
    {
        bool createdCalled = false;

        // Arrange
        var fileBroker = new Mock<IFileBroker>();
        fileBroker.Setup(x => x.Exists(It.IsAny<string>())).Returns(() => createdCalled);
        fileBroker.Setup(x => x.Create(It.IsAny<string>())).Callback(() => createdCalled = true);
        fileBroker.Setup(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>())).Callback(() => createdCalled = true);
        fileBroker.Setup(x => x.Delete(It.IsAny<string>())).Verifiable();

        var factory = new ReadyFlagFileFactory(fileBroker.Object);
        var file = factory.Create("test");

        // Act
        file.Dispose();

        // Assert
        fileBroker.Verify(x => x.Exists(It.IsAny<string>()), Times.Between(1, 2, Moq.Range.Inclusive));
        fileBroker.Verify(x => x.Create(It.IsAny<string>()), Times.Never);
        fileBroker.Verify(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        fileBroker.Verify(x => x.Delete(It.IsAny<string>()), Times.Once);
    }
}
