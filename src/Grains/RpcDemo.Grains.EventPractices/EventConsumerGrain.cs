using Microsoft.Extensions.Logging;
using Orleans;
using RpcDemo.Interfaces.EventPractices;

namespace RpcDemo.Grains.EventPractices;

public class EventConsumerGrain : Grain, IConsumerGrain
{
    private readonly ILogger<EventConsumerGrain> _logger;

    private ProducerInfo? subscribed_producerInfo;

    public EventConsumerGrain(ILogger<EventConsumerGrain> logger)
    {
        _logger = logger;
    }

    public void ReceiveEvent(string message)
    {
        _logger.LogInformation("Subscriber {id} Received message: {message}", this.GetPrimaryKey(), message);
    }

    public async Task SubscribeTo(ProducerInfo producerInfo)
    {
        if (subscribed_producerInfo is not null)
        {
            throw new Exception("Already subscribed to a producer");
        }

        var producer =
            GrainFactory.GetGrain<ISubscribeProducerGrain>(producerInfo.Id, producerInfo.Namespace);

        await producer.Subscribe(this.AsReference<IConsumerGrain>());
        subscribed_producerInfo = producerInfo;
    }

    public async Task Unsubscribe()
    {
        if (subscribed_producerInfo is not null)
        {
            var producer =
                GrainFactory.GetGrain<ISubscribeProducerGrain>(subscribed_producerInfo.Id,subscribed_producerInfo.Namespace);

            await producer.Unsubscribe(this.AsReference<IConsumerGrain>());
        }
    }
}
