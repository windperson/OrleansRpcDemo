using Orleans.Hosting;

var builder = WebApplication.CreateBuilder(args);
// Add Orleans co-hosting
builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.AddAdoNetGrainStorage("demo_counters", options =>
    {
        options.Invariant = "System.Data.SqlClient";
        options.ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=OrleansDemo;Trusted_Connection=True;";
        options.UseJsonFormat = true;
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();