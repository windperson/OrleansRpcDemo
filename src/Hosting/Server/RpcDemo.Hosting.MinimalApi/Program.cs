using Orleans;
using Orleans.Hosting;
using RpcDemo.Grains.EventPractices;
using RpcDemo.Interfaces.EventPractices;

var builder = WebApplication.CreateBuilder(args);

#region Configure Orleans Silo

builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.ConfigureApplicationParts(parts =>
        parts.AddApplicationPart(typeof(TimerProducerGrain).Assembly).WithReferences());
});

#endregion

var app = builder.Build();

app.MapGet("/", () => Results.Ok("go to /start_timer or /stop_timer to start or stop the timer"));
app.MapGet("/start_timer", async (IClusterClient clusterClient) =>
{
    var timerGrain = clusterClient.GetGrain<IProducerGrain>(0, "timer_demo");
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
    var timerGrain = clusterClient.GetGrain<IProducerGrain>(0, "timer_demo");
    await timerGrain.StopProducing();

    return Results.Ok("Timer stopped");
});

app.Run();