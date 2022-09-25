using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using RpcDemo.Interfaces.Counter;

namespace RpcDemo.Grains.Counters;

public class CounterGrain : Grain, ICounterGrain
{
    private readonly ILogger<CounterGrain> _logger;
    private readonly IPersistentState<CounterState> _counterStore;

    public CounterGrain(
        [PersistentState("counter_grain", "demo_counters")] IPersistentState<CounterState> counterStore,
        ILogger<CounterGrain> logger)
    {
        _logger = logger;
        _counterStore = counterStore;
        logger.LogInformation("CounterGrain created");
    }
    
    public override async Task OnActivateAsync()
    {
        await base.OnActivateAsync();
        _logger.LogInformation("CounterGrain activated");
        _logger.LogInformation("Current Count={Count}", _counterStore.State.Count);
    }
    
    public override async Task OnDeactivateAsync()
    {
        //NOTE: This is not always being called
        await _counterStore.WriteStateAsync();
        await base.OnDeactivateAsync();
        _logger.LogInformation("CounterGrain deactivated");
    }
    
    #region ICounterGrain implementation

    public Task<int> GetCountAsync()
    {
        _logger.LogInformation("GetCountAsync called");
        return Task.FromResult(_counterStore.State.Count);
    }

    public async Task IncrementAsync()
    {
        _counterStore.State.Count++;
        _logger.LogInformation("IncrementAsync called, new count: {Count}", _counterStore.State.Count);
        await _counterStore.WriteStateAsync();
    }

    public async Task ResetAsync()
    {
        _logger.LogInformation("ResetAsync called");
        await _counterStore.ClearStateAsync();
    }
    
    #endregion
}

[Serializable]
public class CounterState
{
    public int Count { get; set; } = 0;
}
