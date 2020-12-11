using Stuff.Communication;
using System;
using System.Threading.Tasks;

namespace Stuff.Example
{
    public class StringMessageReceiver : IReciveveAsync<StringMessage>
    {
        public Task ReciveveAsync(StringMessage message)
        {
            Console.WriteLine($" Receive messsage : {message.Str}. From  {message.InterfaceIndex}.");

            return Task.CompletedTask;
        }
    }
}
