using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp4
{
    public static class PipelineExtenstions
    {
        public static IServiceCollection AddPipeline<TParameter, TReturn>(this IServiceCollection services)
        {
            services.AddSingleton<IPipelineBuilder<TParameter, TReturn>, PipelineBuilder<TParameter, TReturn>>();
            services.AddTransient<IPipeline<TParameter, TReturn>, OrderPipeline<TParameter, TReturn>>();
            return services;
        }
    }
}
