using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using Orleans.Streams.Core;
using RpcDemo.Interfaces.EventStreams;

namespace RpcDemo.Grains.EventStreams;

[ImplicitStreamSubscription(StreamConstant.ImplicitSubscribeStreamNamespace)]
// ReSharper disable once ClassNeverInstantiated.Global
public class ConsumerGrain : Grain, IConsumerGrain
{
    private readonly ILogger<ConsumerGrain> _logger;

    public ConsumerGrain(ILogger<ConsumerGrain> logger)
    {
        _logger = logger;
    }

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<StreamDto>();
        var _observer = new LoggerObserver(this.GetPrimaryKey().ToString(), _logger);
        await handle.ResumeAsync(_observer);
    }
}

internal class LoggerObserver : IAsyncObserver<StreamDto>
{
    private readonly ILogger _logger;
    private readonly string _grainPrimaryKey;

    public LoggerObserver(string grainPrimaryKey, ILogger logger)
    {
        _grainPrimaryKey = grainPrimaryKey;
        _logger = logger;
    }

    public Task OnNextAsync(StreamDto item, StreamSequenceToken? token = null)
    {
        _logger.LogInformation("Grain {0} receive: {1}", _grainPrimaryKey, item);
        return Task.CompletedTask;
    }

    public Task OnCompletedAsync()
    {
        _logger.LogInformation("call OnCompletedAsync()");
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
        _logger.LogError(ex, "call OnErrorAsync()");
        return Task.CompletedTask;
    }
}