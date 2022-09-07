using Orleans.Hosting;
using Orleans.TestingHost;

namespace GreetingGrain.Test;

public class TestSiloConfigurator : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
       //Do silo configuration here 
    }
}