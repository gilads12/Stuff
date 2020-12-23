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
        private bool _reconnecting = false;

        public IObservable<byte[]> ReceivedData => _messages.Synchronize().AsObservable();

        public TcpClientSocket(IPEndPoint endPoint) : base(endPoint)
        {
            OptionKeepAlive = true;
        }

        protected override void OnReceived(byte[] buffer, long offset, long size) =>
          _messages.OnNext(buffer.Skip((int)offset).Take((int)size).ToArray());

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Client disconnected. Endpoint: {Endpoint}, {DateTime.Now}");
            if (_reconnecting) return;
            Policy.HandleResult(false)
                  .WaitAndRetry(50, i => TimeSpan.FromSeconds(1))
                  .Execute(ReconnectPolly);
        }

        private bool ReconnectPolly()
        {
            _reconnecting = true;

            DisconnectAsync();
            if (base.Connect())
            {
                _reconnecting = false;
                base.ReceiveAsync();
                return true;
            }
            return false;
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

        public new Task SendAsync(byte[] data)
        {
            base.SendAsync(data);
            return Task.CompletedTask;
        }
    }
}
