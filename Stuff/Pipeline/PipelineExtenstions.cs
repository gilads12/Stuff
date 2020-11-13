using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp4
{
    public static class PipelineExtenstions
    {
        public static IServiceCollection AddPipeline<TParameter>(this IServiceCollection services)
        {
            services.AddSingleton<IPipelineBuilder<TParameter>, PipelineBuilder<TParameter>>();
            services.AddTransient<IPipeline<TParameter>, OrderPipeline<TParameter>>();
            return services;
        }
    }
}
