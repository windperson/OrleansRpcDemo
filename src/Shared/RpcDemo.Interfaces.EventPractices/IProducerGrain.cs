using Orleans;

namespace RpcDemo.Interfaces.EventPractices;

public interface IProducerGrain : IGrainWithIntegerCompoundKey
{
    public Task StartProducing();
    
    public Task StopProducing();
}
