using Orleans.TestingHost;
using RpcDemo.Interfaces.Hello;

namespace GreetingGrain.Test;

public class HelloTest
{
    [Fact]
    public async Task TestSimpleSayHello()
    {
        //Arrange
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurator>();
        var cluster = builder.Build();
        await cluster.DeployAsync();
        
        //Act
        var helloGrain = cluster.GrainFactory.GetGrain<IHelloGrain>(0);
        var greeting = await helloGrain.SayHello("world");
        
        //Assert
        Assert.Equal("Hello world!", greeting);
    }

    [Fact]
    public async Task TestRecordStructSayHello()
    {
        //Arrange
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurator>();
        var cluster = builder.Build();
        await cluster.DeployAsync();
        
        //Act
        var helloGrain = cluster.GrainFactory.GetGrain<IHelloGrain>(0);
        var helloRecord = await helloGrain.SayHello("world", "tester");
        
        //Assert
        Assert.Equal("Hello world!", helloRecord.greeting);
        Assert.Equal("tester", helloRecord.receiverName);
        var now = DateTimeOffset.UtcNow;
        Assert.Equal(now.UtcDateTime, helloRecord.timestamp.UtcDateTime, TimeSpan.FromSeconds(1));
    }
}