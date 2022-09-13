using Orleans;

namespace RpcDemo.Interfaces.ThrowExDemo;

public interface IThrowExGrain : IGrainWithGuidKey
{
    /// <summary>
    /// This method will throw an exception.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task CallThisWillThrowException(string message);
}