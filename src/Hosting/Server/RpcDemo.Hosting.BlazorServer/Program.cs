using Orleans.Hosting;
using RpcDemo.Hosting.BlazorServer.Data;

var builder = WebApplication.CreateBuilder(args);
// Add Orleans co-hosting
builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.AddAdoNetGrainStorage("demo_counters", options =>
    {
        options.Invariant = "Npgsql";
        options.ConnectionString = "Server=localhost;Port=5432;Username=dev;Password=P@ss1234;Database=OrleansDemo;";
        options.UseJsonFormat = true;
    });
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
