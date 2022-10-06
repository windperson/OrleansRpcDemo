namespace RpcDemo.Interfaces.EventStreams;

[Serializable]
public record struct StreamDto(int Serial, string Message, DateTimeOffset Timestamp);

public static class StreamConstant
{
    public const string DefaultStreamProviderName = "MyDefaultStreamProvider";
}