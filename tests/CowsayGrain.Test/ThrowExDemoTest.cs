using Orleans.Runtime;
using Orleans.TestingHost;
using RpcDemo.Common;
using RpcDemo.Interfaces.ASCIIArt;
using RpcDemo.Interfaces.ThrowExDemo;

namespace CowsayGrain.Test;

public class ThrowExDemoTest
{
    [Fact]
    public async Task TestThrowException()
    {
        
        //Arrange
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurator>();
        var cluster = builder.Build();
        await cluster.DeployAsync();
        
        //Act
        var client = cluster.Client;
        var grain = client.GetGrain<IThrowExGrain>(Guid.NewGuid());
        
        //Assert
        var argumentNullException = await Assert.ThrowsAsync<ArgumentNullException>(() => grain.CallThisWillThrowException(string.Empty));
        Assert.Equal("Value cannot be null. (Parameter 'message') (Parameter 'message')", argumentNullException.Message);
        
        var demoException = await Assert.ThrowsAsync<DemoException>(() => grain.CallThisWillThrowException("demo"));
        Assert.Equal("demo", demoException.Message);
    }
}