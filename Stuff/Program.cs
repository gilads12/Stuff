using ConsoleApp4;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stuff.Background;
using Stuff.Sample;
using System;
using System.Threading.Tasks;

namespace Stuff
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddBackgroundTask();
                        services.AddScoped<PingWorker>();
                        services.AddTransient<SampleMiddelware>();
                    }).Build();

            builder.RunAsync();

            var backgroundTaskQueue = builder.Services.GetRequiredService<IBackgroundTaskQueue>();
            var pipelineBuilder = builder.Services.GetRequiredService<IPipelineBuilder<OrderContext, Task>>();

            pipelineBuilder.Add<SampleMiddelware>();

            var pingOrder = new PingOrder { Counter = 1 };

            backgroundTaskQueue.Queue(pingOrder);
            backgroundTaskQueue.Queue(pingOrder);

            backgroundTaskQueue.Queue(pingOrder);
            backgroundTaskQueue.QueueBackgroundWorkItem(ct => { Console.WriteLine("Hello world"); return Task.CompletedTask; });

            await Task.Delay(100000);

            Console.WriteLine("Hello World!");
        }
    }
}
