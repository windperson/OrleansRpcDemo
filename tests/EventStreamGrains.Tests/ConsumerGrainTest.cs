using Moq;
using Orleans;
using Orleans.Hosting;
using Orleans.Providers;
using Orleans.TestingHost;
using Orleans.Timers;
using RpcDemo.Grains.EventStreams;
using RpcDemo.Interfaces.EventStreams;

namespace EventStreamGrains.Tests;

public class ConsumerGrainTest
{
    private static Mock<ILogger<ConsumerGrain>>? _loggerMock;

    #region Test Silo Setup
    private class TestSiloAndClientConfigurator : ISiloConfigurator, IClientBuilderConfigurator
    {
        public static Func<object, Task>? TimerTick { get; private set; }

        public void Configure(ISiloBuilder siloBuilder)
        {
            _loggerMock = new Mock<ILogger<ConsumerGrain>>();
            var loggerFactorMock = new Mock<ILoggerFactory>();
            loggerFactorMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_loggerMock.Object);

            var mockTimerRegistry = new Mock<ITimerRegistry>();
            mockTimerRegistry.Setup(x =>
                    x.RegisterTimer(It.IsAny<Grain>(),
                        It.IsAny<Func<object, Task>>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .Returns(new Mock<IDisposable>().Object)
                .Callback(
                    (Grain targetGrain, Func<object, Task>? timerTick, object _, TimeSpan _, TimeSpan _) =>
                    {
                        // Hook producer's every second message producing timer,
                        // so we can invoke it later in Test method.
                        if (targetGrain is ProducerGrain && timerTick != null)
                        {
                            TimerTick = timerTick;
                        }
                    });
            siloBuilder
                .AddMemoryGrainStorage("PubSubStore")
                .AddMemoryStreams<DefaultMemoryMessageBodySerializer>(StreamConstant.DefaultStreamProviderName)
                .ConfigureServices(services =>
                {
                    services.AddSingleton(loggerFactorMock.Object);
                    services.AddSingleton(mockTimerRegistry.Object);
                });
        }

        public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
        {
            clientBuilder.AddMemoryStreams<DefaultMemoryMessageBodySerializer>(StreamConstant.DefaultStreamProviderName);
        }
    }
    #endregion
    
    [Fact]
    public async Task Test_ConsumerGrain_Receive()
    {
        // Arrange
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TestSiloAndClientConfigurator>();
        var testCluster = builder.Build();
        await testCluster.DeployAsync();
        var key = Guid.NewGuid();
        var producer = testCluster.GrainFactory.GetGrain<IProducerGrain>("sender1");

        // Act
        await producer.StartProducing(StreamConstant.ImplicitSubscribeStreamNamespace, key);
        //Manual Invoke Timer to force produce message to consumer
        var timerTick = TestSiloAndClientConfigurator.TimerTick;
        Assert.NotNull(timerTick);
        await timerTick.Invoke(new object());
        await timerTick.Invoke(new object());
        await producer.StopProducing();
        //Give some time for stream to deliver message
        await Task.Delay(TimeSpan.FromSeconds(0.5));
        await testCluster.StopAllSilosAsync();

        // Assert
        Assert.NotNull(_loggerMock);
        _loggerMock.VerifyLog(logger =>
            logger.LogInformation("Grain {0} receive: {1}",
                It.IsAny<string>(), It.IsAny<StreamDto>()), Times.Exactly(2));
    }
}