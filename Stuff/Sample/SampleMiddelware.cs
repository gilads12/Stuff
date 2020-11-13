using ConsoleApp4;
using Stuff.Background;
using System;
using System.Threading.Tasks;

namespace Stuff.Sample
{
    public class SampleMiddelware : IMiddleware<OrderContext, Task>
    {
        public async Task<Task> Run(OrderContext parameter, Func<OrderContext, Task<Task>> next)
        {
            Console.WriteLine($"Starting {nameof(parameter.Order)}...");
            var res = await next(parameter);
            Console.WriteLine($"Ending {nameof(parameter.Order)}...");
            return res;
        }
    }
}
