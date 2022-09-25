using Orleans.Hosting;
using Orleans.TestingHost;
using RpcDemo.Interfaces.Counter;

namespace CounterGrains.Tests;

public class CounterGrainTest
{
    private class TestSiloConfigurator : ISiloConfigurator
    {
        public void Configure(ISiloBuilder siloBuilder)
        {
            siloBuilder.AddMemoryGrainStorage("demo_counters");
        }
    }
    
    [Fact]
    public async Task CounterGrainTest1()
    {
        //Arrange
        var host = new TestClusterBuilder()
            .AddSiloBuilderConfigurator<TestSiloConfigurator>()
            .Build();
        await host.DeployAsync();
        
        //Act & Assert
        var counterGrain = host.GrainFactory.GetGrain<ICounterGrain>(Guid.NewGuid());
        await counterGrain.IncrementAsync();
        Assert.Equal(1, await counterGrain.GetCountAsync());
        
        await counterGrain.IncrementAsync();
        await counterGrain.IncrementAsync();
        Assert.Equal(3, await counterGrain.GetCountAsync());
        
        await counterGrain.ResetAsync();
        Assert.Equal(0, await counterGrain.GetCountAsync());
        
        await host.StopAllSilosAsync();
    }
}