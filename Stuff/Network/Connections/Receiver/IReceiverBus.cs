using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Stuff.Communication
{
    public interface IReceiverBus
    {
        IDisposable SubscribeAsync<T>(Func<T, Task> onMessage) where T : class;

        Task OnNext<T>(T message) where T : class; 
    }

    public class ReceiverBus : IReceiverBus
    {
        private readonly Subject<object> _messages = new Subject<object>();

        public Task OnNext<T>(T message)
            where T : class
        {
            _messages.OnNext(message); // todo: make task
            return Task.CompletedTask;
        }

        public IDisposable SubscribeAsync<T>(Func<T, Task> onMessage) where T : class
        {
            return _messages.Subscribe(msg =>
            {
                onMessage(msg as T);
            });
        }
    }
}