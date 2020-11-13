using ConsoleApp4;
using Microsoft.Extensions.DependencyInjection;

namespace Stuff.Background
{
    public static class BackgroundTaskExtenstions
    {
        public static IServiceCollection AddBackgroundTask(this IServiceCollection services)
        {
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<QueuedHostedService>();
            services.AddScoped<BackgroundWorkItem.Worker>();
            services.AddPipeline<OrderContext>();

            return services;
        }
    }
}
