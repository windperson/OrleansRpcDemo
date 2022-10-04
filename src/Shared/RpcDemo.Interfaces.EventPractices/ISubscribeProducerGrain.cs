namespace RpcDemo.Interfaces.EventPractices;

public interface ISubscribeProducerGrain : IProducerGrain
{
    /// <summary>
    /// Clients can subscribe to this grain to receive notifications when the producer has new data.
    /// </summary>
    /// <param name="consumer"></param>
    /// <returns></returns>
    public Task Subscribe(IConsumer consumer);
    
    /// <summary>
    /// Client must call this method to unsubscribe from the producer.
    /// </summary>
    /// <param name="consumer"></param>
    /// <returns></returns>
    public Task Unsubscribe(IConsumer consumer);
}