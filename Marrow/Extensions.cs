using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Marrow
{
    public static class Extensions
    {
        public static IObservable<NotifyCollectionChangedEventArgs> Changed<T>(this T source)
            where T : class, INotifyCollectionChanged
        {
            Contract.Requires(source != null);

            return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                handler => source.CollectionChanged += handler,
                handler => source.CollectionChanged -= handler)
                .Select(x => x.EventArgs);
        }

        public static T DisposeWith<T>(this T disposable, CompositeDisposable with) where T : class, IDisposable
        {
            Contract.Requires(disposable != null);
            Contract.Requires(with != null);
            Contract.Ensures(ReferenceEquals(disposable, Contract.Result<T>()));

            with.Add(disposable);

            return disposable;
        }

        public static IObservable<TSource> ItemsAdded<TSource, T>(this T source)
            where T : class, INotifyCollectionChanged, IEnumerable<TSource>
        {
            Contract.Requires(source != null);

            return source.Changed()
                .Where(x => x.Action == NotifyCollectionChangedAction.Add)
                .SelectMany(x => x.NewItems.Cast<TSource>());
        }

        public static IObservable<Unit> Reset<T>(this T source)
            where T : class, INotifyCollectionChanged
        {
            Contract.Requires(source != null);

            return source.Changed()
                .Where(x => x.Action == NotifyCollectionChangedAction.Reset)
                .Select(x => Unit.Default);
        }
    }
}