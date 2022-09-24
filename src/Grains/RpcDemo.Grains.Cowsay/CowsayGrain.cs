using Cowsay.Abstractions;
using Microsoft.Extensions.Logging;
using Orleans;
using RpcDemo.Interfaces.ASCIIArt;

namespace RpcDemo.Grains.Cowsay;

public class CowsayGrain : Grain, ICowsayGrain
{
    private readonly ICattleFarmer _cattleFarmer;
    private string? _grainId;
    private readonly ILogger<CowsayGrain> _logger;

    public CowsayGrain(ICattleFarmer cattleFarmer, ILogger<CowsayGrain> logger)
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
}