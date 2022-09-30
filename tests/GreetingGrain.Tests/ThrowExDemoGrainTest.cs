using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Hosting;
using Orleans.TestingHost;
using RpcDemo.Grains.Greeting;
using RpcDemo.Interfaces.ThrowExDemo;

namespace GreetingGrain.Tests;

public class ThrowExDemoGrainTest
{
    private TestClusterBuilder _builder;

    public ThrowExDemoGrainTest()
    {
        _builder = SetupTestClusterBuilder<TwoTestSiloConfigurator, TestClientConfigurator>();
    }

    private class TwoTestSiloConfigurator : ISiloConfigurator
    {
        public void Configure(ISiloBuilder siloBuilder)
        {
        }
    }

    private class TestClientConfigurator : IClientBuilderConfigurator
    {
        public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
        {
            clientBuilder.ConfigureApplicationParts(
                parts =>
                {
                    parts.AddApplicationPart(typeof(IThrowExDemoGrain).Assembly).WithReferences();
                });
        }
    }

    private TestClusterBuilder SetupTestClusterBuilder<TSiloConfigurator, TClientConfigurator>()
        where TSiloConfigurator : ISiloConfigurator, new()
        where TClientConfigurator : IClientBuilderConfigurator, new()
    {
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TSiloConfigurator>();
        builder.AddClientBuilderConfigurator<TClientConfigurator>();
        return builder;
    }

    [Fact]
    public async Task Test_CallWillThrowIfEmptyInput()
    {
        // Arrange
        await using var cluster = _builder.Build();
        await cluster.DeployAsync();
        var grain = cluster.GrainFactory.GetGrain<IThrowExDemoGrain>(0);
        const string input = "orleans";

        //Act & Assert
        var normalOutput = await grain.CallWillThrowIfEmptyInput(input);
        Assert.Equal(input, normalOutput);
        await Assert.ThrowsAsync<ArgumentNullException>(() => grain.CallWillThrowIfEmptyInput(string.Empty));
        await Assert.ThrowsAsync<ArgumentNullException>(() => grain.CallWillThrowIfEmptyInput(null!));
    }

    [Fact]
    public async Task Test_CallWllThrowMyCustomException()
    {
        // Arrange
        await using var cluster = _builder.Build();
        await cluster.DeployAsync();
        var grain = cluster.Client.GetGrain<IThrowExDemoGrain>(0);

        //Act & Assert
        await Assert.ThrowsAsync<MyCustomException>(() => grain.CallWllThrowMyCustomException());
    }
}