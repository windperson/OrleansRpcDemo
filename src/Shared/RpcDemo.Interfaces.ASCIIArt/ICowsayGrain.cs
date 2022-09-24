using Orleans;

namespace RpcDemo.Interfaces.ASCIIArt;

public interface ICowsayGrain : IGrainWithStringKey
{
    Task<string> Say(string message);
}
