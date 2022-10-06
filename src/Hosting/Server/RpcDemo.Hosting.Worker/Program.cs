using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using RpcDemo.Grains.EventStreams;
using RpcDemo.Interfaces.EventStreams;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Debug()
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .UseOrleans((ISiloBuilder siloBuilder) =>
    {
        siloBuilder.UseLocalhostClustering()
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "silo1";
                options.ServiceId = "Stream-Demo";
            })
            .AddAzureTableGrainStorage("consumer_grain", options =>
            {
                options.ConfigureTableServiceClient("UseDevelopmentStorage=true");
            })
            .AddAzureTableGrainStorage("PubSubStore", options =>
            {
                options.ConfigureTableServiceClient("UseDevelopmentStorage=true");
            })
            .AddAzureQueueStreams(StreamConstant.DefaultStreamProviderName,
                (OptionsBuilder<AzureQueueOptions> optionsBuilder) =>
                {
                    optionsBuilder.Configure(options => { options.ConfigureQueueServiceClient("UseDevelopmentStorage=true"); });
                });
        siloBuilder.ConfigureApplicationParts(parts =>
        {
            parts.AddApplicationPart(typeof(ManualConsumerGrain).Assembly).WithReferences();
            parts.AddApplicationPart(typeof(ProducerGrain).Assembly).WithReferences();
        });
    })
    .ConfigureServices(services =>
    {
    })
    .Build();

await host.RunAsync();