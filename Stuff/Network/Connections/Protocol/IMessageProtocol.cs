using System.Collections.Generic;

namespace Stuff.Network.Connections.Protocol
{
    public interface IMessageProtocol
    {
        IEnumerable<object> ParseMessage(byte[] data, ConnectionContext context);
        byte[] BuildMessage(object obj, ConnectionContext context);
    }
}
