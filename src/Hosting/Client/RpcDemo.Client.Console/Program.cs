using Orleans;
using Orleans.Configuration;
using RpcDemo.Interfaces.ASCIIArt;
using RpcDemo.Interfaces.Cancellable;
using RpcDemo.Interfaces.Hello;
using RpcDemo.Interfaces.ThrowExDemo;
using static System.Console;

WriteLine("\r\n---Orleans RPCDemo Client---");
WriteLine("\r\n---\r\nInitializing Orleans Client...\r\n---");
var client = new ClientBuilder()
    .UseLocalhostClustering()
    .Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "console-client-01";
        options.ServiceId = "Demo Greeting Service";
    })
    .ConfigureApplicationParts(parts =>
    {
        parts.AddApplicationPart(typeof(IHelloGrain).Assembly).WithReferences();
        parts.AddApplicationPart(typeof(ICowsayGrain).Assembly).WithReferences();
        parts.AddApplicationPart(typeof(IThrowExDemoGrain).Assembly).WithReferences();
        parts.AddApplicationPart(typeof(ILongJobProxy).Assembly).WithReferences();
    })
    .Build();

WriteLine(
    "Please wait until Orleans Server is started and ready for connections, then press any key to start connect...");
ReadKey();
await client.Connect();
WriteLine("\r\n---\r\nOrleans Client connected\r\n---");

var helloGrain = client.GetGrain<IHelloGrain>(0);
var helloResult = await helloGrain.SayHello("Orleans");
WriteLine($"\r\n---\r\nCall HelloGrain.SayHello(\"Orleans\") =\r\n{helloResult}\r\n---");

var cowsayGrain = client.GetGrain<ICowsayGrain>("default");
var cowsayResult = await cowsayGrain.Say("Orleans");
WriteLine($"\r\n---\r\nCall CowsayGrain.Say(\"Orleans\") =\r\n{cowsayResult}\r\n---");

WriteLine("\r\n\r\n---\r\n\r\nPress any key to run simple try & catch demo:\r\n");
ReadKey();

try
{
    var throwDemoGrain = client.GetGrain<IThrowExDemoGrain>(0);
    _ = await throwDemoGrain.CallWillThrowIfEmptyInput(null!);
}
catch (Exception e)
{
    WriteLine(e);
}

WriteLine("\r\n\r\n---\r\n\r\nPress any key to run catch ex inside grain to grain RPC:\r\n");
ReadKey();
try
{
    _ = await helloGrain.SayHello(string.Empty);
}
catch (Exception e)
{
    WriteLine(e);
}

WriteLine("\r\n\r\n---\r\n\r\nPress any key to run throw custom exception RPC:\r\n");
ReadKey();
try
{
    var throwDemoGrain = client.GetGrain<IThrowExDemoGrain>(0);
    await throwDemoGrain.CallWllThrowMyCustomException();
}
catch (Exception e)
{
    WriteLine(e);
}

WriteLine("\r\n\r\n---\r\n\r\nPress any key to run cancel RPC demo:\r\n");
ReadKey();

var longJobProxy = client.GetGrain<ILongJobProxy>("job_proxy");
Console.CancelKeyPress += async (source, args) =>
{
    args.Cancel = true;
    WriteLine("Cancelling ProcessString()...");
    await longJobProxy.CancelAsync();
};

WriteLine("Start ProcessString()..., press Ctrl+C to cancel");
try
{
    await longJobProxy.StartAsync("long job demo");
    var longJobResult = await longJobProxy.GetResultAsync();

    if (!string.IsNullOrEmpty(longJobResult))
    {
        WriteLine($"\r\n---\r\nCall LongJobGrain.ProcessString(\"Cancellable RPC Demo\") =\r\n{longJobResult}\r\n---");
    }
}
catch (Exception e)
{
    WriteLine(e);
}

WriteLine("\r\n---\r\nDemonstration finished, press any key to exit...");
ReadKey();

await client.Close();
client.Dispose();