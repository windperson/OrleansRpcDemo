using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using RpcDemo.Grains.CowsayGrain;
using RpcDemo.Grains.GreetingGrain;

var siloHost = new SiloHostBuilder()
    .ConfigureServices(services => { services.AddCowsay(); })
    .UseLocalhostClustering()
    .Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "console-host-01";
        options.ServiceId = "Demo Greeting Service";
    })
    .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
    .ConfigureApplicationParts(parts =>
    {
        parts.AddApplicationPart(typeof(Hello).Assembly).WithReferences();
        parts.AddApplicationPart(typeof(GreetingCow).Assembly).WithReferences();
    })
    .ConfigureLogging(logging =>
    {
        logging.AddConsole();
        logging.AddDebug();
    })
    .Build();

var waitForProcessShutdown = new ManualResetEvent(false);
Console.CancelKeyPress += (sender, eventArgs) =>
{
    eventArgs.Cancel = true;
    waitForProcessShutdown.Set();
};

await siloHost.StartAsync();
Console.WriteLine("===\r\nOrleans Silo started and able to connect,\r\nPress Ctrl+C to shut down when client finish demonstration...\r\n===");
waitForProcessShutdown.WaitOne();

Console.WriteLine("Shutting down Silo...");
await siloHost.StopAsync().ConfigureAwait(false);
Console.WriteLine("===\r\nSilo shutdown complete, exiting...\r\n===");
Environment.Exit(0);