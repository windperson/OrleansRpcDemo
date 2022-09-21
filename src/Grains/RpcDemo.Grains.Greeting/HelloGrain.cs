using System.Threading.Tasks;
using Orleans;
using RpcDemo.Interfaces.Hello;

namespace RpcDemo.Grains.Greeting;

public class HelloGrain : Grain, IHelloGrain
{
    public Task<string> SayHello(string greeting)
    {
        return Task.FromResult($"Hello {greeting}!");
    }
}
