using Stuff.Network.Connections.Sockets;
using System.Threading.Tasks;

namespace Stuff.Network.Connections
{
    public interface ISender
    {
        Task SendAsync(object obj, string connectionId);
    }

    public class Sender : ISender
    {
        private IConnectionsHolder _connectionsHolder;

        public Sender(IConnectionsHolder connectionsHolder) => _connectionsHolder = connectionsHolder;

        public Task SendAsync(object obj, string connectionId) =>
            _connectionsHolder.Connections[connectionId].SendAsync(obj);
    }
}
