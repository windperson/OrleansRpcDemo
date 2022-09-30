using Orleans;
using RpcDemo.Interfaces.ThrowExDemo;

namespace RpcDemo.Grains.Greeting;

public class ThrowExDemoGrain : Grain, IThrowExDemoGrain
{
    public Task<string> CallWillThrowIfEmptyInput(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentNullException(nameof(message));
        }

        return Task.FromResult(message);
    }

    public Task CallWllThrowMyCustomException()
    {
        throw new MyCustomException();
    }
}