using Orleans;

namespace RpcDemo.Interfaces.Cancellable;

public interface ILongJobGrain : IGrainWithStringKey
{
    public Task<string>? ProcessString(string input, CancellationToken cancellationToken);
}