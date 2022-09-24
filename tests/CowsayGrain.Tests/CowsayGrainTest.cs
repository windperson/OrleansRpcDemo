using Cowsay.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Orleans.Hosting;
using Orleans.TestingHost;
using RpcDemo.Interfaces.ASCIIArt;
using SUT = RpcDemo.Grains.Cowsay;

namespace CowsayGrain.Tests;

public class CowsayGrainTest
{
    private static Mock<ILogger<SUT.CowsayGrain>>? _mockLogger;

    private class TestSiloConfigurator : ISiloConfigurator
    {
        public void Configure(ISiloBuilder siloBuilder)
        {
            _mockLogger = new Mock<ILogger<SUT.CowsayGrain>>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory
                .Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(() => _mockLogger.Object);

            var mockCow = new Mock<ICow>();
            mockCow
                .Setup(x => x.Say(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                    It.IsAny<bool>()))
                .Returns((string message, string eyes, string tongue, int cols, bool isThought) => message);

            var mockCattleFarmer = new Mock<ICattleFarmer>();
            mockCattleFarmer.Setup(x => x.RearCowAsync(It.IsAny<string>())).ReturnsAsync(mockCow.Object);

            siloBuilder
                .ConfigureServices(services =>
                {
                    services.AddSingleton(mockCattleFarmer.Object);
                    services.AddSingleton(mockLoggerFactory.Object);
                });
        }
    }

    [Fact]
    public async Task SayTest()
    {
        //Arrange
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurator>();
        var cluster = builder.Build();
        await cluster.DeployAsync();
        const string expected = "Hello Orleans!";

        //Act
        var grain = cluster.GrainFactory.GetGrain<ICowsayGrain>("default");
        var result = await grain.Say(expected);

        //Assert
        Assert.Equal(expected, result);

        _mockLogger!.VerifyLog(logger =>
            // ReSharper disable once ComplexObjectDestructuringProblem
            logger.LogInformation("Cow {0} said:\r\n{1}", It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
    }
}
