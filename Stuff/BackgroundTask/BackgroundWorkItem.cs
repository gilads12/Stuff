using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stuff.Background
{
    public static class BackgroundWorkItem
    {
        // keep supporting `IBackgroundTask` without order
        public static void QueueBackgroundWorkItem(
                this IBackgroundTaskQueue queue,
                Func<IServiceProvider, CancellationToken, Task> method)
        {
            queue.Queue(new WorkOrder(method));
        }

        public class WorkOrder : IBackgroundWorkOrder<WorkOrder, Worker>
        {
            public WorkOrder(Func<IServiceProvider, CancellationToken, Task> method)
            {
                Method = method;
            }

            public Func<IServiceProvider, CancellationToken, Task> Method { get; }
        }

        public class Worker : IBackgroundWorker<WorkOrder, Worker>
        {
            private readonly IServiceProvider _provider;

            public Worker(IServiceProvider provider)
            {
                _provider = provider;
            }

            public async Task DoWork(WorkOrder order, CancellationToken cancellationToken)
            {
                await order.Method.Invoke(_provider, cancellationToken);
            }
        }
    }
}
