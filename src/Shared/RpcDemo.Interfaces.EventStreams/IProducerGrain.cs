using Orleans;

namespace RpcDemo.Interfaces.EventStreams;

public interface IProducerGrain : IGrainWithStringKey
{
    Task StartProducing(string streamNameSpace, Guid key);

    Task StopProducing();
}