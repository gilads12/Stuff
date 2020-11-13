using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stuff.Background
{
    public static class BackgroundWorkItem
    {
        // keep supporting `IBackgroundTask` without order
        // todo add ServiceProvider
        public static void QueueBackgroundWorkItem(
                this IBackgroundTaskQueue queue,
                Func<CancellationToken, Task> method)
        {
            queue.Queue(new WorkOrder(method));
        }

        public class WorkOrder : IBackgroundWorkOrder<WorkOrder, Worker>
        {
            public WorkOrder(Func<CancellationToken, Task> method)
            {
                Method = method;
            }

            public Func<CancellationToken, Task> Method { get; }
        }

        public class Worker : IBackgroundWorker<WorkOrder, Worker>
        {
            public async Task DoWork(WorkOrder order, CancellationToken cancellationToken)
            {
                await order.Method.Invoke(cancellationToken);
            }
        }
    }
}
