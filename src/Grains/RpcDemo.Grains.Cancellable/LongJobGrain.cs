using Microsoft.Extensions.Logging;
using Orleans;
using RpcDemo.Interfaces.Cancellable;

namespace RpcDemo.Grains.Cancellable;

public class LongJobGrain : Grain, ILongJobGrain
{
    private readonly ILogger<ILongJobGrain> _logger;

    public LongJobGrain(ILogger<LongJobGrain> logger)
    {
        _logger = logger;
    }

    public Task<string>? ProcessString(string input, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start LongJob ProcessingString(\" {input} \")", input);

        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Cancellation requested");
            throw new OperationCanceledException();
        }

        cancellationToken.Register(() => _logger.LogInformation("LongJob ProcessingString(\" {input} \") cancelled", input));

        // Pretend to do some work
        var i = 0;
        var result = string.Empty;
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("i = {i}", i);
            i++;
            Task.Delay(1000, cancellationToken).Wait(cancellationToken);
            if (i > 20)
            {
                result = $"{input} done";
                break;
            }
        }

        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Cancellation requested");
            throw new OperationCanceledException();
        }

        // Result gets sent back to the caller
        return Task.FromResult(result);
    }
}