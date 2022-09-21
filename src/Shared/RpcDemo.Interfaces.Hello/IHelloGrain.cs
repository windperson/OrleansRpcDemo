using System.Threading.Tasks;
using Orleans;

namespace RpcDemo.Interfaces.Hello;

public interface IHelloGrain : IGrainWithIntegerKey
{
    Task<string> SayHello(string greeting);
}