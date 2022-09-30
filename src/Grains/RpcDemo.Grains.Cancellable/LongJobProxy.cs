using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;
using RpcDemo.Interfaces.Cancellable;

namespace RpcDemo.Grains.Cancellable;

[Reentrant]
public class LongJobProxy : Grain, ILongJobProxy
{
    private GrainCancellationTokenSource? tokenSource ;
    private Task<string>? processStringTask = null;
    private ILongJobGrain? grain;
    private readonly ILogger<LongJobProxy> _logger;

    public LongJobProxy(ILogger<LongJobProxy> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(string input)
    {
        tokenSource = new GrainCancellationTokenSource();
        grain = GrainFactory.GetGrain<ILongJobGrain>(this.GetPrimaryKeyString());
        processStringTask = grain.ProcessString(input, tokenSource.Token.CancellationToken);
        return Task.CompletedTask;
    }

    public async Task<string> GetResultAsync()
    {
        if (processStringTask is null)
        {
            throw new InvalidOperationException("StartAsync must be called before GetResultAsync");
        }

        return await processStringTask;
    }

    public async Task CancelAsync()
    {
        if(tokenSource is null)
        {
            throw new InvalidOperationException("StartAsync must be called before CancelAsync");
        }
        if(tokenSource.IsCancellationRequested)
        {
           _logger.LogWarning("CancelAsync called but cancellation has already been requested"); 
        }
        await tokenSource.Cancel();
    }
}