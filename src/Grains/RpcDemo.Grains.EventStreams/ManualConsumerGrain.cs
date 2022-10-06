using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using RpcDemo.Interfaces.EventStreams;

namespace RpcDemo.Grains.EventStreams;

public record struct StreamInfo(Guid StreamId, string StreamNamespace);

public class ManualConsumerGrain : Grain, IManualConsumerGrain
{
    private readonly ILogger<ManualConsumerGrain> _logger;
    private StreamSubscriptionHandle<StreamDto>? _handle;
    private readonly IPersistentState<StreamInfo> _streamInfo;

    public ManualConsumerGrain([PersistentState("subscribe_stream", "consumer_grain")] IPersistentState<StreamInfo> streamInfo,
        ILogger<ManualConsumerGrain> logger)
    {
        _streamInfo = streamInfo;
        _logger = logger;
    }

    public override async Task OnActivateAsync()
    {
        await base.OnActivateAsync();
        if (_streamInfo.RecordExists)
        {
            var stream = GetStreamProvider(StreamConstant.DefaultStreamProviderName)
                .GetStream<StreamDto>(_streamInfo.State.StreamId, _streamInfo.State.StreamNamespace);
            var allHandles = await stream.GetAllSubscriptionHandles();
            if (allHandles is not null)
            {
                _handle = allHandles.FirstOrDefault();
                if (_handle is not null)
                {
                    _handle = await _handle.ResumeAsync(_onNext);
                }
            }
        }
    }

    public async Task Subscribe(string streamNameSpace, Guid key)
    {
        if (_handle is not null)
        {
            throw new Exception("Already subscribed");
        }

        var stream = GetStreamProvider(StreamConstant.DefaultStreamProviderName).GetStream<StreamDto>(key, streamNameSpace);
        _handle = await stream.SubscribeAsync(_onNext);
        _streamInfo.State = new StreamInfo(key, streamNameSpace);
        await _streamInfo.WriteStateAsync();
    }

    private Func<StreamDto, StreamSequenceToken, Task> _onNext => (dto, _) =>
    {
        _logger.LogInformation("Grain {0} receive: {1}", this.GetPrimaryKeyString(), dto);
        return Task.CompletedTask;
    };

    public async Task UnSubscribe()
    {
        if (_handle is not null)
        {
            await _handle.UnsubscribeAsync();
            _handle = null;
            await _streamInfo.ClearStateAsync();
        }
    }
}