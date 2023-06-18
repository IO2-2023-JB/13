using System.Threading.Channels;

namespace MyWideIO.API.BackgroundProcessing
{
    public interface IBackgroundTaskQueue<T>
    {
        ValueTask QueueBackgroundWorkItemAsync(T workItem);

        ValueTask<T> DequeueAsync(CancellationToken cancellationToken);
    }

    public class BackgroundTaskQueue<T> : IBackgroundTaskQueue<T>
    {
        private readonly Channel<T> _queue;

        public BackgroundTaskQueue(int capacity)
        {
            // Capacity should be set based on the expected application load and
            // number of concurrent threads accessing the queue.            
            // BoundedChannelFullMode.Wait will cause calls to WriteAsync() to return a task,
            // which completes only when space became available. This leads to backpressure,
            // in case too many publishers/calls start accumulating.
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<T>(options);
        }

        public async ValueTask QueueBackgroundWorkItemAsync(T workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            // await _queue.Writer.WaitToWriteAsync(); // mozna poczekac az sie zwolni miejsce - wtedy user tez musi czekac

            await _queue.Writer.WriteAsync(workItem);
        }

        public async ValueTask<T> DequeueAsync(CancellationToken cancellationToken)
        {
            var workItem = await _queue.Reader.ReadAsync(cancellationToken);

            return workItem;
        }
    }
}
