using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp4
{
    public class OrderPipeline<TParameter, TReturn> : IPipeline<TParameter, TReturn>
    {
        private readonly IList<Type> _middlewareTypes = new List<Type>();
        private readonly IServiceProvider _provider;

        private static readonly TypeInfo MiddlewareTypeInfo = typeof(IMiddleware<TParameter, TReturn>).GetTypeInfo();

        private Func<TParameter, Task<TReturn>> _finallyFunc;

        public OrderPipeline(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IPipeline<TParameter, TReturn> Add(Type middlewareType)
        {
            if (!MiddlewareTypeInfo.IsAssignableFrom(middlewareType.GetTypeInfo()))
                throw new ArgumentException($"The middleware type must implement \"{middlewareType}\".");

            _middlewareTypes.Add(middlewareType);

            return this;
        }

        public async Task<TReturn> Execute(TParameter parameter)
        {
            if (!_middlewareTypes.Any())
                return _finallyFunc == null ? default : await _finallyFunc(parameter);

            int index = 0;
            Func<TParameter, Task<TReturn>> func = null;
            func = (param) =>
            {
                var type = _middlewareTypes[index];
                var middleware = (IMiddleware<TParameter, TReturn>)_provider.GetRequiredService(type);

                index++;

                if (index == _middlewareTypes.Count)
                    func = _finallyFunc ?? ((p) => Task.FromResult(default(TReturn)));

                return middleware.Run(param, func);
            };

            return await func(parameter).ConfigureAwait(false);
        }

        public IPipeline<TParameter, TReturn> Finally(Func<TParameter, Task<TReturn>> finallyFunc)
        {
            _finallyFunc = finallyFunc;
            return this;
        }
    }
}
