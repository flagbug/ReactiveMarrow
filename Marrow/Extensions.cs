using System;
using System.Diagnostics.Contracts;
using System.Reactive.Disposables;

namespace Marrow
{
    public static class Extensions
    {
        public static T DisposeWith<T>(this T disposable, CompositeDisposable with) where T : class, IDisposable
        {
            Contract.Requires(disposable != null);
            Contract.Requires(with != null);

            with.Add(disposable);

            return disposable;
        }
    }
}