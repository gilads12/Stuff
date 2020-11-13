using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp4
{
    public class OrderPipeline<TParameter> : IPipeline<TParameter>
    {
        private readonly IList<Type> _middlewareTypes = new List<Type>();
        private readonly IServiceProvider _provider;

        private static readonly TypeInfo MiddlewareTypeInfo = typeof(IMiddleware<TParameter>).GetTypeInfo();

        private Func<TParameter, Task> _finallyFunc;

        public OrderPipeline(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IPipeline<TParameter> Add(Type middlewareType)
        {
            if (!MiddlewareTypeInfo.IsAssignableFrom(middlewareType.GetTypeInfo()))
                throw new ArgumentException($"The middleware type must implement \"{middlewareType}\".");

            _middlewareTypes.Add(middlewareType);

            return this;
        }

        public async Task Execute(TParameter parameter)
        {
            if (!_middlewareTypes.Any() && _finallyFunc != null)
                await _finallyFunc(parameter);

            int index = 0;
            Func<TParameter, Task> func = null;
            func = (param) =>
            {
                var type = _middlewareTypes[index];
                var middleware = (IMiddleware<TParameter>)_provider.GetRequiredService(type);

                index++;

                if (index == _middlewareTypes.Count)
                    func = _finallyFunc ?? ((p) => Task.CompletedTask);

                return middleware.Run(param, func);
            };

            await func(parameter).ConfigureAwait(false);
        }

        public IPipeline<TParameter> Finally(Func<TParameter, Task> finallyFunc)
        {
            _finallyFunc = finallyFunc;
            return this;
        }
    }
}
