using System;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    public interface IMiddleware<TParameter>
    {
        Task Run(TParameter parameter, Func<TParameter, Task> next);
    }
}
