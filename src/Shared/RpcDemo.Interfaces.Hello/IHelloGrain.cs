using System.Threading.Tasks;
using Orleans;
using RpcDemo.Common;

namespace RpcDemo.Interfaces.Hello;

public interface IHelloGrain : IGrainWithIntegerKey
{
    Task<string> SayHello(string greeting);
    
    ValueTask<HelloRecord> SayHello(string greeting, string receiver);
}
