using Orleans;

namespace RpcDemo.Interfaces.EventStreams;

public interface IManualConsumerGrain : IGrainWithStringKey
{
    Task Subscribe(string streamNameSpace, Guid key);

    Task UnSubscribe();
}