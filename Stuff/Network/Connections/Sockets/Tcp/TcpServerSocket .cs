using NetCoreServer;
using System;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Stuff.Network.Connections.Sockets.Tcp
{
    public class TcpServerSocket : TcpServer, ISocket
    {
        private readonly ISubject<byte[]> _messages = new Subject<byte[]>();

        public IObservable<byte[]> ReceivedData => _messages.Synchronize().AsObservable();

        public bool IsConnected => base.IsStarted;

        public TcpServerSocket(IPEndPoint endPoint) : base(endPoint)
        {
            OptionKeepAlive = true;
        }

        protected override void OnConnected(TcpSession session)
        {
            session.ReceiveAsync();
        }

        protected override TcpSession CreateSession() => new TcpSessionWrapper(this, _messages);

        public Task Connect()
        {
            if (base.Start())
                return Task.CompletedTask;

            throw new NotImplementedException();// todo: add exception....
        }

        public Task Disconnect()
        {
            base.DisconnectAll();
            base.Stop();
            return Task.CompletedTask;
        }

        public Task SendAsync(byte[] data)
        {
            base.Multicast(data);
            return Task.CompletedTask;
        }

        internal class TcpSessionWrapper : TcpSession // each TcpSession is client that connect to the server....
        {
            private readonly ISubject<byte[]> _messages = new Subject<byte[]>();

            public TcpSessionWrapper(TcpServerSocket server, ISubject<byte[]> subject) : base(server)
            {
                _messages = subject;
            }

            protected override void OnReceived(byte[] buffer, long offset, long size) =>
                _messages.OnNext(buffer.Skip((int)offset).Take((int)size).ToArray());
        }

    }
}
