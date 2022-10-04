using Microsoft.Extensions.Logging;
using Orleans;
using RpcDemo.Interfaces.EventPractices;

namespace RpcDemo.Grains.EventPractices;

public class EventProducerGrain : Grain, ISubscribeProducerGrain
{
    private readonly ILogger<EventProducerGrain> _logger;
    private IDisposable? _timer;
    private int _counter = 0;
    private readonly ObserverManager<IConsumer> _observers;

    public EventProducerGrain(ILogger<EventProducerGrain> logger)
    {
        _logger = logger;
        _observers = new ObserverManager<IConsumer>(TimeSpan.FromMinutes(5), _logger, "event_demo");
    }

    public Task StartProducing()
    {
        if (_timer is not null)
        {
            throw new Exception("Already producing");
        }

        //Register a timer that produce an event every second
        var period = TimeSpan.FromSeconds(1);
        _timer = RegisterTimer(TimerTick, null, period, period);

        _logger.LogInformation("I will produce a new event every {Period}", period);
        return Task.CompletedTask;
    }

    private Task TimerTick(object _)
    {
        var msg = $"Producing event {_counter++}";
        _logger.LogInformation(msg);
        
        //Notify all the observers
        _observers.Notify(o => o.ReceiveEvent(msg));
        
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

    public Task Subscribe(IConsumer consumer)
    {
        _observers.Subscribe(consumer, consumer);
        return Task.CompletedTask;
    }

    public Task Unsubscribe(IConsumer consumer)
    {
        _observers.Unsubscribe(consumer);
        return Task.CompletedTask;
    }
}
