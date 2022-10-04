using Orleans;

namespace RpcDemo.Interfaces.EventPractices;

public interface IConsumer : IGrainObserver
{
    void ReceiveEvent(string message);
}

public interface IConsumerGrain : IGrainWithIntegerKey, IConsumer
{
    Task SubscribeTo(ProducerInfo producerInfo);
    Task Unsubscribe();
}