using System.Threading.Tasks;

namespace Stuff.Communication
{
    public interface IReciveveAsync<in T> where T : class
    {
        Task ReciveveAsync(T message);
    }
}