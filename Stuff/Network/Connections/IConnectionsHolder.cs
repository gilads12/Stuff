using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Stuff.Network.Connections.Sockets
{
    public interface IConnectionsHolder
    {
        IDictionary<string, IConnection> Connections { get; }
    }

    public class ConnectionsHolder : IConnectionsHolder
    {
        public IDictionary<string, IConnection> Connections { get; } = new ConcurrentDictionary<string, IConnection>();
    }
}
