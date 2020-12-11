using ConsoleApp4;
using Stuff.Background;
using System;
using System.Threading.Tasks;

namespace Stuff.Sample
{
    public class SampleMiddelware : IMiddleware<OrderContext>
    {
        public async Task Run(OrderContext parameter, Func<OrderContext, Task> next)
        {
            Console.WriteLine($"Starting {parameter.Order.GetType()}...");
            await next(parameter);
            Console.WriteLine($"Ending {parameter.Order.GetType()}...");
        }
    }
}
