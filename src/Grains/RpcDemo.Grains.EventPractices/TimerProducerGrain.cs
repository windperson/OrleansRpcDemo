using Microsoft.Extensions.Logging;
using Orleans;
using RpcDemo.Interfaces.EventPractices;

namespace RpcDemo.Grains.EventPractices;

public class TimerProducerGrain : Grain, IProducerGrain
{
    private readonly ILogger<TimerProducerGrain> _logger;

    private IDisposable? _timer;

    private int _counter = 0;

    public TimerProducerGrain(ILogger<TimerProducerGrain> logger)
    {
        _logger = logger;
    }

    public Task StartProducing()
    {
        if (_timer is not null)
        {
            throw new Exception("This grain is already producing events");
        }

        //Register a timer that produce an event every second
        var period = TimeSpan.FromSeconds(1);
        _timer = RegisterTimer(TimerTick, null, period, period);

        _logger.LogInformation("I will produce a new event every {Period}", period);
        return Task.CompletedTask;
    }

    private Task TimerTick(object _)
    {
        var value = _counter++;
        _logger.LogInformation("Producing event {EventNumber}", value);
        return Task.CompletedTask;
    }

    public Task StopProducing()
    {
        if (_timer is not null)
        {
            _timer.Dispose();
            _timer = null;
        }

        return Task.CompletedTask;
    }
}