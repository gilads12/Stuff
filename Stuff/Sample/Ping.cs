using Stuff.Background;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stuff.Sample
{
    public class PingOrder : IBackgroundWorkOrder<PingOrder, PingWorker>
    {
        public int Counter { get; set; }
    }

    public class PingWorker : IBackgroundWorker<PingOrder, PingWorker>
    {
        public async Task DoWork(PingOrder order, CancellationToken cancellationToken)
        {
            Console.WriteLine(order.Counter);
            await Task.Delay(250, cancellationToken);
        }
    }
}
