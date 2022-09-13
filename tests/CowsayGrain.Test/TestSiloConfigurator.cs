using Orleans.Hosting;
using Orleans.TestingHost;
using Microsoft.Extensions.DependencyInjection;

namespace CowsayGrain.Test;
public class TestSiloConfigurator : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
       //Do silo configuration here 
       siloBuilder.ConfigureServices(services => services.AddCowsay());
    }
}