using Stuff.Communication;
using Stuff.Network.Connections;
using Stuff.Network.Connections.Protocol;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stuff.Example
{
    public class TestReceiver : IReciveveAsync<Frame>
    {
        public Task ReciveveAsync(Frame message)
        {
            Console.WriteLine($" Receive Frame : {message.Data.ToObject()}. From {message.interfaceIndex}.");
            return Task.CompletedTask;
        }
    }

    public class FrameProtocl : IMessageProtocol
    {
        public byte[] BuildMessage(object obj, ConnectionContext context)
            => obj.ToBytes();

        public IEnumerable<object> ParseMessage(byte[] buffer, ConnectionContext context)
        {
            yield return new Frame
            {
                Data = buffer,
                interfaceIndex = context.Items["InterfaceIndex"].ToString()
            };
        }
    }

    public class Frame
    {
        public byte[] Data { get; set; }
        public string interfaceIndex { get; set; }
    }
}
