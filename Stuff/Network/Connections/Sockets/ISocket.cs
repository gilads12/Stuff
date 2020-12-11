using System;
using System.Threading.Tasks;

namespace Stuff.Network.Connections.Sockets
{
    public interface ISocket
    {
        bool IsConnected { get; }
        IObservable<byte[]> ReceivedData { get; }
        Task SendAsync(byte[] data);
        Task Connect();
        Task Disconnect();
    }
}
