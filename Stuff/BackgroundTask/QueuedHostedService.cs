using ConsoleApp4;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stuff.Background
{
    public class QueuedHostedService : IHostedService
    {
        private readonly IServiceProvider _services;

        private readonly CancellationTokenSource _shutdown = new CancellationTokenSource();
        private readonly ILogger _logger;
        private Task _backgroundTask;

        public QueuedHostedService(
            IServiceProvider services,
            IBackgroundTaskQueue taskQueue,
            ILoggerFactory loggerFactory)
        {
            _services = services;
            TaskQueue = taskQueue;
            _logger = loggerFactory.CreateLogger<QueuedHostedService>();
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Hosted Service is starting.");

            _backgroundTask = Task.Run(BackgroundProceessing);

            return Task.CompletedTask;
        }

        private async Task BackgroundProceessing()
        {
            while (!_shutdown.IsCancellationRequested)
            {
                var workOrder = await TaskQueue.DequeueAsync(_shutdown.Token);

                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var workerType = workOrder
                            .GetType()
                            .GetInterfaces()
                            .First(t => t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(IBackgroundWorkOrder<,>))
                            .GetGenericArguments()
                            .Last();

                        var worker = scope.ServiceProvider.GetRequiredService(workerType);

                        var ctx = new OrderContext { Order = workOrder };

                        var glx = scope.ServiceProvider.GetRequiredService<IPipelineBuilder<OrderContext, Task>>().Build();

                        glx.Finally((gCtx) =>
                        {
                            // todo remvoe task...
                            return Task.FromResult(
                            (Task)workerType.GetMethod("DoWork")
                            .Invoke(worker, new object[] { gCtx.Order, _shutdown.Token }));
                        });

                        await glx.Execute(ctx);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        $"Error occurred executing {nameof(workOrder)}.");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            _shutdown.Cancel();

            return Task.WhenAny(
                _backgroundTask,
                Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }
}
