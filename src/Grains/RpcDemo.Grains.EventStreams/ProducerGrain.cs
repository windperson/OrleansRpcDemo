using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using RpcDemo.Interfaces.EventStreams;

namespace RpcDemo.Grains.EventStreams;

public class ProducerGrain : Grain, IProducerGrain
{
    private readonly ILogger<ProducerGrain> _logger;
    private IDisposable? _timer;
    private IAsyncStream<StreamDto>? _stream;
    private int _counter;

    public ProducerGrain(ILogger<ProducerGrain> logger)
    {
        _logger = logger;
    }

    public Task StartProducing(string streamNameSpace, Guid key)
    {
        if (_timer is not null)
        {
            throw new Exception("This grain is already producing events");
        }

        //Get a reference to the stream
        _stream = GetStreamProvider(StreamConstant.DefaultStreamProviderName).GetStream<StreamDto>(key, streamNameSpace);

        //Create a timer that will send a message every second
        var period = TimeSpan.FromSeconds(1);
        _timer = RegisterTimer(TimerTick, null, period, period);

        _logger.LogInformation("Started producing events for stream {StreamNameSpace}/{Key} every {period}", streamNameSpace,
            key, period);
        return Task.CompletedTask;
    }

    private async Task TimerTick(object _)
    {
        _counter++;
        if (_stream is not null)
        {
            var data = new StreamDto
            {
                Serial = _counter,
                Message = $"#{_counter:0000} from {nameof(ProducerGrain)}:{this.GetPrimaryKeyString()}",
                Timestamp = DateTime.UtcNow
            };
            _logger.LogInformation("Sending event {Event}", data);
            await _stream.OnNextAsync(data);
        }
    }

    public async Task StopProducing()
    {
        if (_timer is not null)
        {
            _timer.Dispose();
            _timer = null;
        }

        if (_stream is not null)
        {
            try
            {
                await _stream.OnCompletedAsync();
            }
            catch (NotImplementedException)
            {
                _logger.LogWarning("Stream does not support OnCompletedAsync()");
            }

            _stream = null;
        }

        _logger.LogInformation("Stopped producing events");
    }
}