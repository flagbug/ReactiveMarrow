using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactiveMarrow
{
    public static class ObservableExtensions
    {
        /// <summary>
        /// Limits the rate at which items are pushed. A time interval of one second means one item
        /// per second is pushed out. This method differs from <see
        /// cref="Observable.Sample{TSource}(System.IObservable{TSource},System.TimeSpan)" />, as
        /// items aren't dropped and if items come slower than the maximum rate, they are processed
        /// imemdiately, instead of waiting on the next "tick".
        /// </summary>
        /// <param name="source"></param>
        /// <param name="maximumRate">
        /// The rate at which the items should be processed. A timespan of 2 seconds would mean the
        /// items are processed at a maximum rate of one item every two seconds.
        /// </param>
        /// <param name="scheduler">An optional scheduler to schedule the timing on.</param>
        /// <returns></returns>
        public static IObservable<T> LimitRate<T>(this IObservable<T> source, TimeSpan maximumRate, IScheduler scheduler = null)
        {
            scheduler = scheduler ?? DefaultScheduler.Instance;

            // Here, we are immediately starting with the item, but then we delay the actual
            // completion of the sequence, so the initial item doesn't have to wait.
            return source.Select(i => Observable.Empty<T>()
                .Delay(maximumRate, scheduler)
                .StartWith(i)).Concat();
        }

        /// <summary>
        /// Matches pairs in a sequence, based on a key selector.
        /// </summary>
        /// <param name="left">The left observable sequence.</param>
        /// <param name="right">The right observable sequence.</param>
        /// <param name="keySelector">A function to compute the comparison key for the elements.</param>
        /// <returns>An observable sequence of pairs with an equal left and right element.</returns>
        public static IObservable<Pair<T>> MatchPair<T, TKey>(this IObservable<T> left, IObservable<T> right, Func<T, TKey> keySelector)
        {
            return Observable.Create<Pair<T>>(o =>
            {
                var leftCache = new List<T>();
                var rightCache = new List<T>();
                var gate = new object();
                var disposable = new CompositeDisposable();
                bool leftDone = false;
                bool rightDone = false;

                left.Subscribe(l =>
                {
                    lock (gate)
                    {
                        // Look for the last element, as we want FIFO
                        int lastIndex = rightCache.FindLastIndex(x => EqualityComparer<TKey>.Default.Equals(keySelector(x), keySelector(l)));

                        if (lastIndex > -1)
                        {
                            T element = rightCache[lastIndex];
                            rightCache.RemoveAt(lastIndex);

                            o.OnNext(new Pair<T>(l, element));
                        }

                        else
                        {
                            leftCache.Add(l);
                        }
                    }
                }, ex =>
                {
                    o.OnError(ex);
                    disposable.Dispose();
                }, () =>
                {
                    lock (gate)
                    {
                        leftDone = true;

                        if (rightDone)
                        {
                            o.OnCompleted();
                        }
                    }
                }).DisposeWith(disposable);

                right.Subscribe(r =>
                {
                    lock (gate)
                    {
                        // Look for the last element, as we want FIFO
                        int lastIndex = leftCache.FindLastIndex(x => EqualityComparer<TKey>.Default.Equals(keySelector(x), keySelector(r)));

                        if (lastIndex > -1)
                        {
                            T element = leftCache[lastIndex];
                            leftCache.RemoveAt(lastIndex);

                            o.OnNext(new Pair<T>(element, r));
                        }

                        else
                        {
                            rightCache.Add(r);
                        }
                    }
                }, ex =>
                {
                    o.OnError(ex);
                    disposable.Dispose();
                }, () =>
                {
                    lock (gate)
                    {
                        rightDone = true;

                        if (leftDone)
                        {
                            o.OnCompleted();
                        }
                    }
                }).DisposeWith(disposable);

                return disposable;
            });
        }

        /// <summary>
        /// Takes the latest value of the left observable and combines it with the latest value of
        /// the right observable whenever the right sequence produces an element.
        ///
        /// This method is effectively a combination of the
        /// <see cref="Observable.CombineLatest{TSource1,TSource2,TResult}" /> and
        /// <see cref="Observable.Sample{TSource,TSample}" /> method.
        /// </summary>
        public static IObservable<TResult> SampleAndCombineLatest<TLeft, TRight, TResult>(this IObservable<TLeft> left, IObservable<TRight> right, Func<TLeft, TRight, TResult> resultSelector)
        {
            if (left == null)
                throw new ArgumentNullException("left");

            if (right == null)
                throw new ArgumentNullException("right");

            if (resultSelector == null)
                throw new ArgumentNullException("resultSelector");

            TLeft latest = default(TLeft);
            bool initialized = false;

            var disp = new CompositeDisposable(2);

            left.Subscribe(x =>
            {
                latest = x;
                initialized = true;
            }).DisposeWith(disp);

            return Observable.Create<TResult>(o =>
            {
                right.Where(_ => initialized)
                    .Select(x => resultSelector(latest, x))
                    .Subscribe(o)
                    .DisposeWith(disp);

                return disp;
            });
        }

        public static IObservable<Unit> ToUnit<T>(this IObservable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.Select(_ => Unit.Default);
        }
    }
}