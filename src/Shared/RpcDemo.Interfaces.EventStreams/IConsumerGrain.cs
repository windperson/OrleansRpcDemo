using Orleans;
using Orleans.Streams.Core;

namespace RpcDemo.Interfaces.EventStreams;

public interface IConsumerGrain : IGrainWithGuidKey, IStreamSubscriptionObserver
{
}