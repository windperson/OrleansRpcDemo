using Orleans;
using RpcDemo.Common;
using RpcDemo.Interfaces.ThrowExDemo;

namespace RpcDemo.Grains.Cowsay;

public class ThrowExDemoGrain : Grain, IThrowExGrain
{
    public Task CallThisWillThrowException(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentNullException(nameof(message));
        }

        throw new DemoException(message);
    }
}