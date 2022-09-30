using Orleans;

namespace RpcDemo.Interfaces.ThrowExDemo;

public interface IThrowExDemoGrain : IGrainWithIntegerKey
{
    public Task<string> CallWillThrowIfEmptyInput(string message);
    
    public Task CallWllThrowMyCustomException();
}