using System;
using System.Reactive.Disposables;

namespace Marrow
{
    public static class Extensions
    {
        public static T DisposeWith<T>(this T disposable, CompositeDisposable with) where T : IDisposable
        {
            with.Add(disposable);

            return disposable;
        }
    }
}