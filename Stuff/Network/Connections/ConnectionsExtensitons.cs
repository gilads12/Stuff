using Microsoft.Extensions.DependencyInjection;
using Stuff.Communication;
using Stuff.Network.Connections.Protocol;
using Stuff.Network.Connections.Sockets;
using Stuff.Network.Connections.Sockets.Tcp;
using System.Collections.Generic;
using System.Net;

namespace Stuff.Network.Connections
{
    public static partial class ConnectionsExtensitons
    {
        public static IServiceCollection AddConnections(this IServiceCollection service)
        {
            service.AddSingleton<IConnectionsHolder, ConnectionsHolder>();
            service.AddSingleton<IReceiverBus, ReceiverBus>();
            service.AddSingleton<IReciverDispathcer, DefaultAutoSubscriberMessageDispatcher>();
            service.AddSingleton<IAutoSebscriber, AutoSubscriber>();

            service.AddTransient<IConnectionsManager, ConnectionsManager>();
            service.AddTransient<ISender, Sender>();

            return service;
        }

        public static Connection CreateTcpClient(this IPEndPoint endPoint, ConnectionContext context) =>
            new Connection(new TcpClientSocket(endPoint), context);

        public static Connection CreateTcpServer(this IPEndPoint endPoint, ConnectionContext context) =>
            new Connection(new TcpServerSocket(endPoint), context);

        public static ConnectionContext CreateConnectionContext(IMessageProtocol protocol, params KeyValuePair<string, object>[] items)
        {
            var context = new ConnectionContext();
            context.Features.Set(protocol);
            items.ForEach(x => context.Items.Add(x.Key, x.Value));
            return context;
        }
    }
}
