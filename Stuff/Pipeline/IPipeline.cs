using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    public interface IPipeline<TParameter, TReturn>
    {
        IPipeline<TParameter, TReturn> Finally(Func<TParameter, Task<TReturn>> finallyFunc);

        IPipeline<TParameter, TReturn> Add(Type middlewareType);

        Task<TReturn> Execute(TParameter parameter);
    }

    public interface IPipelineBuilder<TParameter, TReturn>
    {
        IPipelineBuilder<TParameter, TReturn> Add<TMiddleware>()
            where TMiddleware : IMiddleware<TParameter, TReturn>;

        IPipeline<TParameter, TReturn> Build();
    }

    public class PipelineBuilder<TParameter, TReturn> : IPipelineBuilder<TParameter, TReturn>
    {
        private readonly IServiceProvider _provider;
        private readonly IList<Type> _middlewareTypes = new List<Type>();

        public PipelineBuilder(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IPipelineBuilder<TParameter, TReturn> Add<TMiddleware>()
            where TMiddleware : IMiddleware<TParameter, TReturn>
        {
            _middlewareTypes.Add(typeof(TMiddleware));

            return this;
        }

        public IPipeline<TParameter, TReturn> Build()
        {
            var pipe = _provider.GetRequiredService<IPipeline<TParameter, TReturn>>(); // todo: think! may should be using serviceScopeProvider

            foreach (var middelware in _middlewareTypes)
            {
                pipe.Add(middelware);
            }

            return pipe;
        }
    }
}
