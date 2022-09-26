using Microsoft.AspNetCore.Mvc;
using Orleans;
using RpcDemo.Interfaces.Counter;

namespace RpcDemo.Hosting.AspNetCoreWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CounterController
{
    private readonly IGrainFactory _grainFactory;

    public CounterController(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    [HttpGet]
    public async Task<int> GetCurrent(Guid id)
    {
        var counter = _grainFactory.GetGrain<ICounterGrain>(id);
        return await counter.GetCountAsync();
    }

    [HttpPost]
    public async Task Increment(Guid id)
    {
        var counter = _grainFactory.GetGrain<ICounterGrain>(id);
        await counter.IncrementAsync();
    }

    [HttpPatch]
    public async Task Reset(Guid id)
    {
        var counter = _grainFactory.GetGrain<ICounterGrain>(id);
        await counter.ResetAsync();
    }
}