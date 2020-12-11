using Stuff.Network.Connections.Protocol;
using Stuff.Network.Connections.Sockets;
using System;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace Stuff.Network.Connections
{
    public interface IConnection // todo: remove
    {
        bool IsConnected { get; }
        IObservable<object> Messages { get; }
        Task SendAsync(object message);
        Task Connect();
        Task Disconnect();
    }

    public class Connection : IConnection
    {
        private readonly ConnectionContext _connectionContext;
        private readonly ISocket _socket;

        public string ConnectionId { get; } = Guid.NewGuid().ToString();

        public IObservable<object> Messages
            => _socket.ReceivedData.SelectMany(data =>
                    _connectionContext.Features.Get<IMessageProtocol>().ParseMessage(data, _connectionContext));

        public bool IsConnected => _socket.IsConnected;

        public Connection(ISocket socket, ConnectionContext context)
        {
            _socket = socket;
            _connectionContext = context;
        }

        public Task SendAsync(object message)
        {
            var data = _connectionContext.Features.Get<IMessageProtocol>().BuildMessage(message, _connectionContext);
            return _socket.SendAsync(data);
        }

        public Task Connect() => _socket.Connect();

        public Task Disconnect() => _socket.Disconnect();
    }

}
