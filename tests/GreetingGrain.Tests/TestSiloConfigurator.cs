using Orleans.Hosting;
using Orleans.TestingHost;

namespace GreetingGrain.Tests;

public class TestSiloConfigurator : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        //Do silo configuration here 
    }
}