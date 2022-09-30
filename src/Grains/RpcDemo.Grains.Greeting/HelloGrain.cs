using System.Threading.Tasks;
using Orleans;
using RpcDemo.Interfaces.Hello;
using RpcDemo.Interfaces.ThrowExDemo;

namespace RpcDemo.Grains.Greeting;

public class HelloGrain : Grain, IHelloGrain
{
    public async Task<string> SayHello(string greeting)
    {
        var checkGrain = GrainFactory.GetGrain<IThrowExDemoGrain>(this.GetPrimaryKeyLong());
        greeting = await checkGrain.CallWillThrowIfEmptyInput(greeting);
        return $"Hello {greeting}!";
    }
}