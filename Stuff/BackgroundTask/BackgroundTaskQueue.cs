using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Stuff.Background
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<IBackgroundWorkOrder> _workOrders =
               new ConcurrentQueue<IBackgroundWorkOrder>();

        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void Queue<TWorkOrder, TWorker>(IBackgroundWorkOrder<TWorkOrder, TWorker> order)
                where TWorker : IBackgroundWorker<TWorkOrder, TWorker>
                where TWorkOrder : IBackgroundWorkOrder<TWorkOrder, TWorker>
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            _workOrders.Enqueue(order);
            _signal.Release();
        }

        public async Task<IBackgroundWorkOrder> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workOrders.TryDequeue(out var workItem);

            return workItem;
        }
    }
}
