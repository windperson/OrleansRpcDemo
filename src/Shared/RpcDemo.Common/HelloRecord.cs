namespace RpcDemo.Common;

public readonly record struct HelloRecord(string greeting, string receiverName, DateTimeOffset timestamp)
{
    public override string ToString()
    {
        return $"\"{greeting}\"\r\n{receiverName} at {timestamp}";
    }
}