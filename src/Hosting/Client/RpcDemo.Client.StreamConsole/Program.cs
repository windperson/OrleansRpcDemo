using Orleans;
using Orleans.Configuration;
using RpcDemo.Interfaces.EventStreams;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var clientBuilder = new ClientBuilder()
    .UseLocalhostClustering()
    .Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "client1";
        options.ServiceId = "Stream-Demo";
    }).ConfigureApplicationParts(parts =>
    {
        parts.AddApplicationPart(typeof(IProducerGrain).Assembly).WithReferences();
        parts.AddApplicationPart(typeof(IManualConsumerGrain).Assembly).WithReferences();
        parts.AddApplicationPart(typeof(IConsumerGrain).Assembly).WithReferences();
    })
    .ConfigureLogging(logging => logging.AddSerilog());

var client = clientBuilder.Build();

Log.Logger.Information("Press any key to start connecting to Silo");
Console.ReadKey();

await client.Connect();
Log.Logger.Information("\r\nConnected to Silo, press any key to start Orleans stream demo\r\n");
Console.ReadKey();

var producer = client.GetGrain<IProducerGrain>("sender1");
var key = Guid.NewGuid();
const string streamNamespace = "demo";
await producer.StartProducing(streamNamespace, key);
Log.Logger.Information("\r\nProducer Grain (sender1) is starting to produce messages in stream every second," +
                       "\r\nPress any key to create Consumer Grain (receiver1) and subscribe the stream\r\n");
Console.ReadKey();
var receiver1 = client.GetGrain<IManualConsumerGrain>("receiver1");
await receiver1.Subscribe(streamNamespace, key);
Log.Logger.Information("\r\nConsumer Grain (receiver1) is subscribing the stream," +
                       "\r\nPress any key to creat another Consumer Grain (receiver2) and subscribe the stream\r\n");

Console.ReadKey();
var receiver2 = client.GetGrain<IManualConsumerGrain>("receiver2");
await receiver2.Subscribe(streamNamespace, key);
Log.Logger.Information("\r\nConsumer Grain (receiver2) is subscribing the stream," +
                       "\r\nPress any key to stop producing messages\r\n");

Console.ReadKey();
await producer.StopProducing();
await receiver1.UnSubscribe();
await receiver2.UnSubscribe();

Log.Logger.Information("\r\nPress any key to demo implicit stream subscription\r\n");
Console.ReadKey();
await producer.StartProducing(StreamConstant.ImplicitSubscribeStreamNamespace, key);

Log.Logger.Information("\r\nPress any key to stop streaming in Producer Grain\r\n");
Console.ReadKey();
await producer.StopProducing();

Log.Logger.Information("Stopped streaming in Producer Grain, press any key to disconnect from Silo and exit");
Console.ReadKey();
await client.Close();