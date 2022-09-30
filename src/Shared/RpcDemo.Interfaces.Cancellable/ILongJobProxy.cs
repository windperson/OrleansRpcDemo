using Orleans;
using Orleans.Concurrency;

namespace RpcDemo.Interfaces.Cancellable;

public interface ILongJobProxy : IGrainWithStringKey
{
    Task StartAsync(string input);
    Task<string> GetResultAsync();
    
    [AlwaysInterleave]
    Task CancelAsync();
}