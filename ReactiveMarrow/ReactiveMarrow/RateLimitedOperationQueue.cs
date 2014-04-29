using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveMarrow
{
    /// <summary>
    /// A operation processing queue that can limit the number of operations per given timespan.
    ///
    /// This queue will also process items immediately, if the items come at a lower interval than
    /// the processing rate, and doesn't wait on the next "tick", unlike operators like <see
    /// cref="Observable.Sample{TSource}(System.IObservable{TSource},System.TimeSpan)" />.
    /// </summary>
    public class RateLimitedOperationQueue
    {
        private readonly object gate;
        private readonly Subject<IOperation> processingQueue;
        private readonly IConnectableObservable<Unit> resultStream;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="rate">
        /// The rate at which the items should be processed. A timespan of 2 seconds would mean the
        /// items are processed at a maximum rate of one item every two seconds.
        /// </param>
        /// <param name="scheduler">An optional scheduler to schedule the timing on.</param>
        public RateLimitedOperationQueue(TimeSpan rate, IScheduler scheduler = null)
        {
            this.gate = new object();

            this.processingQueue = new Subject<IOperation>();
            this.resultStream = processingQueue.LimitRate(rate, scheduler)
                .Select(x => x.Evaluate())
                .Concat()
                .Publish();

            this.resultStream.Connect();
        }

        private interface IOperation
        {
            IObservable<Unit> Evaluate();
        }

        public IObservable<T> EnqueueOperation<T>(Func<IObservable<T>> asyncFunction)
        {
            var operation = new Operation<T>(asyncFunction);

            lock (this.gate)
            {
                processingQueue.OnNext(operation);
            }

            return operation.Result;
        }

        private class Operation<T> : IOperation
        {
            private readonly Func<IObservable<T>> calculate;

            public Operation(Func<IObservable<T>> calculate)
            {
                this.calculate = calculate;
                this.Result = new AsyncSubject<T>();
            }

            public AsyncSubject<T> Result { get; private set; }

            public IObservable<Unit> Evaluate()
            {
                var ret = this.calculate().Multicast(this.Result);
                ret.Connect();

                return ret.Select(_ => Unit.Default).Catch(Observable.Empty<Unit>());
            }
        }
    }
}