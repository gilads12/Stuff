using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Stuff.Communication
{
    public interface IReciverDispathcer
    {
        Task DispatchAsync<TMessage, TReciver>(TMessage message)
            where TMessage : class
            where TReciver : class, IReciveveAsync<TMessage>;
    }

    public class DefaultAutoSubscriberMessageDispatcher : IReciverDispathcer
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DefaultAutoSubscriberMessageDispatcher(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task DispatchAsync<TMessage, TReciver>(TMessage message)
            where TMessage : class
            where TReciver : class, IReciveveAsync<TMessage>
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var reciver = Activator.CreateInstance<TReciver>();
                await reciver.ReciveveAsync(message);
            }
        }
    }
}
