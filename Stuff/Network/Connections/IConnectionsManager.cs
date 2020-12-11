using Stuff.Communication;
using Stuff.Network.Connections.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stuff.Network.Connections
{
    public interface IConnectionsManager
    {
        Task AddConnection(Connection connection);
        Task RemoveConnection(string id);
    }


    public class ConnectionsManager : IConnectionsManager
    {
        private IConnectionsHolder _connectionsHolder;
        private readonly IReceiverBus _bus;
        private readonly List<KeyValuePair<string, IDisposable>> _disposibles = new List<KeyValuePair<string, IDisposable>>();

        public ConnectionsManager(IReceiverBus bus, IConnectionsHolder connectionsHolder)
        {
            _bus = bus;
            _connectionsHolder = connectionsHolder;
        }

        public IConnection GetConnection(string id)
        {
            if (_connectionsHolder.Connections.ContainsKey(id))
                return _connectionsHolder.Connections[id];
            throw new Exception();
        }

        public async Task AddConnection(Connection connection)
        {
            if (!connection.IsConnected)
                await connection.Connect();

            _connectionsHolder.Connections.Add(connection.ConnectionId, connection);

            var disposible = connection.Messages.SubscribeAsync(x => _bus.OnNext(x));

            _disposibles.Add(new KeyValuePair<string, IDisposable>(connection.ConnectionId, disposible)); // todo: clean...
        }

        public async Task RemoveConnection(string id)
        {
            if (_connectionsHolder.Connections.ContainsKey(id))
            {
                var connection = _connectionsHolder.Connections[id];

                if (connection.IsConnected)
                    await _connectionsHolder.Connections[id].Disconnect();
                _connectionsHolder.Connections.Remove(id);
                return;
            }

            throw new NotImplementedException();
        }
    }
}
