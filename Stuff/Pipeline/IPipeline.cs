using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    public interface IPipeline<TParameter>
    {
        IPipeline<TParameter> Finally(Func<TParameter, Task> finallyFunc);

        IPipeline<TParameter> Add(Type middlewareType);

        Task Execute(TParameter parameter);
    }

    public interface IPipelineBuilder<TParameter>
    {
        IPipelineBuilder<TParameter> Add<TMiddleware>()
            where TMiddleware : IMiddleware<TParameter>;

        IPipeline<TParameter> Build();
    }

    public class PipelineBuilder<TParameter> : IPipelineBuilder<TParameter>
    {
        private readonly IServiceProvider _provider;
        private readonly IList<Type> _middlewareTypes = new List<Type>();

        public PipelineBuilder(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IPipelineBuilder<TParameter> Add<TMiddleware>()
            where TMiddleware : IMiddleware<TParameter>
        {
            _middlewareTypes.Add(typeof(TMiddleware));

            return this;
        }

        public IPipeline<TParameter> Build()
        {
            var pipe = _provider.GetRequiredService<IPipeline<TParameter>>(); // todo: think! may should be using serviceScopeProvider

            foreach (var middelware in _middlewareTypes)
            {
                pipe.Add(middelware);
            }

            return pipe;
        }
    }
}
