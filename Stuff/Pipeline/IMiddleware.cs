using System;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    public interface IMiddleware<TParameter, TReturn>
    {
        Task<TReturn> Run(TParameter parameter, Func<TParameter, Task<TReturn>> next);
    }
}
