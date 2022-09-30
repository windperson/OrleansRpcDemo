using Orleans.TestingHost;
using RpcDemo.Interfaces.Hello;

namespace GreetingGrain.Tests;

public class HelloGrainTest
{
    private TestClusterBuilder _builder;

    public HelloGrainTest()
    {
        _builder = SetupTestClusterBuilder<TestSiloConfigurator>();
    }

    private TestClusterBuilder SetupTestClusterBuilder<T>() where T : ISiloConfigurator, new()
    {
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<T>();
        return builder;
    }

    [Fact]
    public async Task TestSimpleSayHello()
    {
        //Arrange
        var cluster = _builder.Build();
        await cluster.DeployAsync();

        //Act
        var helloGrain = cluster.GrainFactory.GetGrain<IHelloGrain>(0);
        var greeting = await helloGrain.SayHello("world");

        //Assert
        Assert.Equal("Hello world!", greeting);
    }

    [Fact]
    public async Task TestNullOrEmptyInput()
    {
        //Arrange
        var cluster = _builder.Build();
        await cluster.DeployAsync();

        //Act & Assert
        var helloGrain = cluster.GrainFactory.GetGrain<IHelloGrain>(0);
        await Assert.ThrowsAsync<ArgumentNullException>(() => helloGrain.SayHello(""));
        
        var clientSideHello = cluster.Client.GetGrain<IHelloGrain>(0);
        await Assert.ThrowsAsync<ArgumentNullException>(() => clientSideHello.SayHello(null!));
    }
}