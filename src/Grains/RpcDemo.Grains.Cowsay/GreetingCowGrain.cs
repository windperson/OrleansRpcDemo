using Cowsay.Abstractions;
using Microsoft.Extensions.Logging;
using Orleans;
using RpcDemo.Interfaces.ASCIIArt;
using RpcDemo.Interfaces.Hello;

namespace RpcDemo.Grains.Cowsay;

public class GreetingCowGrain : Grain, ICowsayGrain
{
    private readonly ICattleFarmer _cattleFarmer;
    private string? _grainId;
    private readonly ILogger<GreetingCowGrain> _logger;

    public GreetingCowGrain(ICattleFarmer cattleFarmer, ILogger<GreetingCowGrain> logger)
    {
        _cattleFarmer = cattleFarmer;
        _logger = logger;
    }

    public override Task OnActivateAsync()
    {
        _grainId = this.GetPrimaryKeyString();
        return base.OnActivateAsync();
    }

    //Demo using injected service
    public async Task<string> Say(string message)
    {
        var cattle = await _cattleFarmer.RearCowAsync(_grainId ?? "default");
        var saying = cattle.Say(message);
        _logger.LogInformation("Cow {0} said:\r\n{1}", _grainId, saying);
        return saying;
    }

    public async Task<string> Greet(string message, string name)
    {
        var helloGrain = GrainFactory.GetGrain<IHelloGrain>(0);
        var greeting = await helloGrain.SayHello(message, name);
        _logger.LogInformation("Greeting:\r\n{0}",greeting);
        
        var cattle = await _cattleFarmer.RearCowAsync(_grainId ?? "default");
        var saying = cattle.Say(greeting.ToString());
        _logger.LogInformation("Cow {0} said\r\n===\r\n{1}\r\n===", _grainId, saying);
        return saying;
    }
}