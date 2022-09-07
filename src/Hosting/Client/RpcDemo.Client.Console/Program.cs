using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using RpcDemo.Interfaces.ASCIIArt;
using RpcDemo.Interfaces.Hello;

namespace RpcDemo.Client.Console
{
    using static System.Console;

    class Program
    {
        static async Task Main(string[] args)
        {
            WriteLine("\r\n---Orleans RPCDemo Client---");
            WriteLine("\r\n---\r\nInitializing Orleans Client...\r\n---");
            var client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "console-client-01";
                    options.ServiceId = "Demo Greeting Service";
                })
                .ConfigureApplicationParts(parts =>
                {
                    parts.AddApplicationPart(typeof(IHelloGrain).Assembly).WithReferences();
                    parts.AddApplicationPart(typeof(ICowsayGrain).Assembly).WithReferences();
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();
            WriteLine(
                "Please wait until Orleans Server is started and ready for connections, then press any key to start connect...");
            ReadKey();
            await client.Connect();
            WriteLine("\r\n---\r\nOrleans Client connected\r\n---");

            var helloGrain = client.GetGrain<IHelloGrain>(0);
            var helloResult = await helloGrain.SayHello("Orleans");
            WriteLine($"\r\n---\r\nCall HelloGrain.SayHello(\"Orleans\") =\r\n{helloResult}\r\n---");

            var cowsayGrain = client.GetGrain<ICowsayGrain>("default");
            var simpleCowsay = await cowsayGrain.Say("Cowsay in Orleans!");
            WriteLine($"\r\n---\r\nCall CowsayGrain.Say(\"Cowsay in Orleans!\") =\r\n{simpleCowsay}\r\n---");
            WriteLine("\r\n>>> Press any key to continue to run the complex cowsay demo <<<\r\n");
            ReadKey();

            var complexCowsayGrain = client.GetGrain<ICowsayGrain>("goat2");
            var complexCowsay = await complexCowsayGrain.Greet(message: "Awesome Orleans!", "Isak Pao");
            WriteLine($"\r\n---\r\nCall CowsayGrain.Greet(\"Awesome Orleans!\", \"Isak Pao\") =\r\n{complexCowsay}\r\n---");

            WriteLine("Demonstration finished, press any key to exit...");
            ReadKey();

            await client.Close();
            client.Dispose();
        }
    }
}