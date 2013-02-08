using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Marrow
{
    /// <summary>
    /// Provides an implementation of <see cref="IObservable{T}"/> that is also a read-write property.
    /// </summary>
    /// <typeparam name="T">The type of the object that should be exposed.</typeparam>
    public class PropertyObservable<T> : IObservable<T>
    {
        private readonly BehaviorSubject<T> backingField;

        /// <summary>
        /// Initializes the <see cref="PropertyObservable{T}"/> with the default value of <see cref="T"/>
        /// </summary>
        public PropertyObservable()
            : this(default(T))
        { }

        /// <summary>
        /// Initializes the <see cref="PropertyObservable{T}"/> with s specified value for <see cref="T"/>.
        /// </summary>
        /// <param name="value">The value for <see cref="T"/></param>
        public PropertyObservable(T value)
        {
            this.backingField = new BehaviorSubject<T>(value);
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="PropertyObservable{T}"/>'s backing field.
        /// </summary>
        public T Value
        {
            get { return this.backingField.First(); }
            set { this.backingField.OnNext(value); }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return this.backingField.Subscribe(observer);
        }
    }
}