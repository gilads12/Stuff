using Stuff.Network.Connections;
using Stuff.Network.Connections.Protocol;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stuff.Example
{
    public class StringMessageProtocol : IMessageProtocol
    {
        public IEnumerable<object> ParseMessage(byte[] data, ConnectionContext context)
        {
            var mc = context.Items["Buffer"] as List<byte>;

            mc.AddRange(data);

            int index = 0;

            foreach (var rawData in ReadData(mc))
            {
                index += rawData.Length;

                yield return new StringMessage
                {
                    Str = Encoding.ASCII.GetString(rawData).Trim('\n'),
                    InterfaceIndex = context.Items["InterfaceIndex"].ToString()
                };
            }

            context.Items["Buffer"] = mc.Skip(index).ToList();
        }

        private static IEnumerable<byte[]> ReadData(List<byte> data)
        {
            int i = 0;

            foreach (var index in data.FindAllIndexof((byte)'\n'))
            {
                yield return data.Skip(i).Take(index).ToArray();
                i += index;
            }
        }

        public byte[] BuildMessage(object obj, ConnectionContext context)
        {
            return Encoding.ASCII.GetBytes(obj as string);
        }
    }
}
