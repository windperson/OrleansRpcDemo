using Orleans;

namespace RpcDemo.Interfaces.Counter;

public interface ICounterGrain : IGrainWithGuidKey
{
    Task<int> GetCountAsync();
    Task IncrementAsync();
    Task ResetAsync();
}