using Orleans.TestingHost;
using RpcDemo.Interfaces.ASCIIArt;

namespace CowsayGrain.Test;

public class CowsayTest
{
    [Fact]
    public async Task TestCowSay()
    {
        //Arrange
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurator>();
        var cluster = builder.Build();
        await cluster.DeployAsync();
        
        //Act
        var client = cluster.Client;
        var grain = client.GetGrain<ICowsayGrain>("default");
        var result = await grain.Say("Hello World");
        
        //Assert
        const string expected =
            @" _____________ 
< Hello World >
 ------------- 
        \   ^__^
         \  (oo)\_______
            (__)\       )\/\
                ||----w |
                ||     ||";
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task TestCowGreeting()
    {
        //Arrange
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurator>();
        var cluster = builder.Build();
        await cluster.DeployAsync();
        
        //Act
        var client = cluster.Client;
        var grain = client.GetGrain<ICowsayGrain>("goat2");
        var result = await grain.Greet("Orleans", "Tom");
        
        //Assert
        const string startString =
            @" ____________________________________
/ ""Hello Orleans!""                   \
\ Tom at";
        const string endString =
            @"
        \
         \
          )__(
         '|oo|'________/
          |__|         |
             ||""""""""""""""||
             ||       ||";
        Assert.StartsWith(startString, result);
        Assert.EndsWith(endString, result);
    }
}