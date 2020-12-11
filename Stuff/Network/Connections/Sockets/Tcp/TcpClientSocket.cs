using NetCoreServer;
using Polly;
using System;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Stuff.Network.Connections.Sockets.Tcp
{
    public class TcpClientSocket : TcpClient, ISocket
    {
        private readonly Subject<byte[]> _messages = new Subject<byte[]>();

        public IObservable<byte[]> ReceivedData => _messages.Synchronize().AsObservable();

        public TcpClientSocket(IPEndPoint endPoint) : base(endPoint)
        {
            OptionKeepAlive = true;
        }

        protected override void OnReceived(byte[] buffer, long offset, long size) =>
          _messages.OnNext(buffer.Skip((int)offset).Take((int)size).ToArray());

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Client disconnected. Endpoint: {Endpoint}");
            Policy.HandleResult(ConnectAsync()).WaitAndRetryAsync(10, retryAttempt => Math.Pow(2, retryAttempt) * TimeSpan.FromMilliseconds(100));
            //base.ReceiveAsync(); // needed ??
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            Console.WriteLine($"Client connected. Endpoint: {Endpoint}");
        }

        public new Task Connect()
        {
            if (base.ConnectAsync())
            {
                base.ReceiveAsync();
                return Task.CompletedTask;
            }
            throw new NotImplementedException();
        }

        public new Task Disconnect()
        {
            base.DisconnectAsync();
            return Task.CompletedTask;
        }

        Task ISocket.SendAsync(byte[] data)
        {
            base.SendAsync(data);
            return Task.CompletedTask;
        }
    }
}
