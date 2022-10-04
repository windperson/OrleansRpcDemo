using Orleans;
using Orleans.Hosting;
using RpcDemo.Grains.EventPractices;
using RpcDemo.Hosting.MinimalApi;
using RpcDemo.Interfaces.EventPractices;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Debug()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

#region Configure Orleans Silo

builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.ConfigureApplicationParts(parts =>
    {
        parts.AddApplicationPart(typeof(TimerProducerGrain).Assembly).WithReferences();

        parts.AddApplicationPart(typeof(EventProducerGrain).Assembly).WithReferences();
        parts.AddApplicationPart(typeof(EventConsumerGrain).Assembly).WithReferences();
    });
});

#endregion

var app = builder.Build();

app.MapGet("/", () =>
    Results.Extensions.Html("<html><body>go to <b>/start_timer</b> or <b>/stop_timer</b> to start or stop the timer,<br/>" +
                            "go to <b>/consumer/subscribe/{id}</b> to subscribe to the event,<br/>" +
                            "go to <b>/consumer/unsubscribe/{id}</b> to unsubscribe from the event,<br/>" +
                            "go to <b>/producer_start</b> & <b>/producer_stop</b> to start & stop publish the event</body></html>"));

app.MapGet("/start_timer", async (IClusterClient clusterClient) =>
{
    var timerGrain =
        clusterClient.GetGrain<IProducerGrain>(0, "timer_demo", $"{typeof(TimerProducerGrain).Namespace}.{nameof(TimerProducerGrain)}");
    try
    {
        await timerGrain.StartProducing();
        return Results.Ok("Timer started");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});
app.MapGet("/stop_timer", async (IClusterClient clusterClient) =>
{
    var timerGrain =
        clusterClient.GetGrain<IProducerGrain>(0, "timer_demo", $"{typeof(TimerProducerGrain).Namespace}.{nameof(TimerProducerGrain)}");
    await timerGrain.StopProducing();

    return Results.Ok("Timer stopped");
});

var producerInfo = new ProducerInfo(0, "producer_demo");

app.MapGet("/consumer/subscribe/{id}", async (IClusterClient clusterClient, int id) =>
{
    var consumerGrain = clusterClient.GetGrain<IConsumerGrain>(id);
    await consumerGrain.SubscribeTo(producerInfo);
    return Results.Ok($"Consumer {id} subscribed");
});
app.MapGet("/consumer/unsubscribe/{id}", async (IClusterClient clusterClient, int id) =>
{
    var consumerGrain = clusterClient.GetGrain<IConsumerGrain>(id);
    await consumerGrain.Unsubscribe();
    return Results.Ok($"Consumer {id} unsubscribed");
});

app.MapGet("/producer_start", async (IClusterClient clusterClient) =>
{
    var producerGrain = clusterClient.GetGrain<ISubscribeProducerGrain>(producerInfo.Id, producerInfo.Namespace);
    await producerGrain.StartProducing();
    return Results.Ok("Producer started");
});
app.MapGet("/producer_stop", async (IClusterClient clusterClient) =>
{
    var producerGrain = clusterClient.GetGrain<ISubscribeProducerGrain>(producerInfo.Id, producerInfo.Namespace);
    await producerGrain.StopProducing();
    return Results.Ok("Producer stopped");
});

app.Run();