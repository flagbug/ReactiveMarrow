using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactiveMarrow
{
    public static class DisposableExtensions
    {
        public static T DisposeWith<T>(this T disposable, CompositeDisposable with) where T : class, IDisposable
        {
            if(disposable == null)
                throw new ArgumentNullException("disposable");
            
            if(with == null)
                throw new ArgumentNullException("with");

            with.Add(disposable);

            return disposable;
        }
    }
}