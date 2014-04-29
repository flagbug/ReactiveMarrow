using System;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace ReactiveMarrow
{
    public static class RateLimitedQueueExtensions
    {
        public static Task<T> EnqueueOperation<T>(this RateLimitedOperationQueue queue, Func<Task<T>> asyncFunction)
        {
            return queue.EnqueueOperation(() => asyncFunction().ToObservable()).ToTask();
        }

        public static Task EnqueueOperation(this RateLimitedOperationQueue queue, Func<Task> asyncFunction)
        {
            return queue.EnqueueOperation(() => asyncFunction().ToObservable()).ToTask();
        }
    }
}