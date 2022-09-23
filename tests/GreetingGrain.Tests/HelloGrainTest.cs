using Orleans.TestingHost;
using RpcDemo.Interfaces.Hello;

namespace GreetingGrain.Tests;

public class HelloGrainTest
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
}