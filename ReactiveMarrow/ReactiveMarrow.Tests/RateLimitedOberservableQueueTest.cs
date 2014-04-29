namespace ReactiveMarrow.Tests
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Threading.Tasks;
    using Microsoft.Reactive.Testing;
    using ReactiveUI.Testing;
    using Xunit;

    namespace ReactiveMarrow.Tests
    {
        public class RateLimitedOperationQueueTest
        {
            [Fact]
            public void DoesntDieFromOperationError()
            {
                new TestScheduler().With(scheduler =>
                {
                    var queue = new RateLimitedOperationQueue(TimeSpan.FromSeconds(1), scheduler);
                    Task<int> task1 = queue.EnqueueOperation(() => Observable.Throw<int>(new Exception()).ToTask());

                    Assert.True(task1.IsFaulted);

                    Task<int> task2 = queue.EnqueueOperation(() => Task.FromResult(1));

                    scheduler.AdvanceByMs(1001);

                    Assert.True(task2.IsCompleted);
                });
            }

            [Fact]
            public void LimitsRate()
            {
                new TestScheduler().With(scheduler =>
                {
                    var queue = new RateLimitedOperationQueue(TimeSpan.FromSeconds(1), scheduler);
                    Task task1 = queue.EnqueueOperation(() => Task.FromResult(1));
                    Task task2 = queue.EnqueueOperation(() => Task.FromResult(1));

                    Assert.True(task1.IsCompleted);
                    Assert.False(task2.IsCompleted);

                    scheduler.AdvanceByMs(1001);

                    Assert.True(task2.IsCompleted);

                    Task task3 = queue.EnqueueOperation(() => Task.FromResult(1));

                    Assert.False(task3.IsCompleted);

                    scheduler.AdvanceByMs(1001);

                    Assert.True(task3.IsCompleted);
                });
            }

            [Fact]
            public void QueuesItemsAndDoesntBlock()
            {
                new TestScheduler().With(scheduler =>
                {
                    var queue = new RateLimitedOperationQueue(TimeSpan.FromSeconds(1), scheduler);

                    Task task1 = queue.EnqueueOperation(() => Task.FromResult(1));
                    Task task2 = queue.EnqueueOperation(() => Task.FromResult(1));
                    Task task3 = queue.EnqueueOperation(() => Task.FromResult(1));
                    Task task4 = queue.EnqueueOperation(() => Task.FromResult(1));

                    Assert.True(task1.IsCompleted);
                    Assert.False(task2.IsCompleted);
                    Assert.False(task3.IsCompleted);
                    Assert.False(task4.IsCompleted);

                    scheduler.AdvanceByMs(5000);

                    Assert.True(task2.IsCompleted);
                    Assert.True(task3.IsCompleted);
                    Assert.True(task4.IsCompleted);
                });
            }
        }
    }
}