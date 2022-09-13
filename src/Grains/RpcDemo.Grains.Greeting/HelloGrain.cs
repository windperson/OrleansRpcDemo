using Orleans;
using RpcDemo.Common;
using RpcDemo.Interfaces.Hello;

namespace RpcDemo.Grains.Greeting;

public class HelloGrain : Grain, IHelloGrain
{
    public Task<string> SayHello(string greeting)
    {
        return Task.FromResult($"Hello {greeting}!");
    }

    public ValueTask<HelloRecord> SayHello(string greeting, string receiver)
    {
        return new ValueTask<HelloRecord>(new HelloRecord
        {
            greeting = $"Hello {greeting}!",
            receiverName = receiver,
            timestamp = DateTimeOffset.UtcNow
        });
    }
}