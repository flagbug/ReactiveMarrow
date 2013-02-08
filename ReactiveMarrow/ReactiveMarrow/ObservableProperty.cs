using System;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveMarrow
{
    /// <summary>
    /// Provides an implementation of <see cref="IObservable{T}"/> that is also a read-write property.
    /// </summary>
    /// <typeparam name="T">The type of the object that should be exposed.</typeparam>
    public class ObservableProperty<T> : IObservable<T>
    {
        private readonly BehaviorSubject<T> backingField;
        private readonly Func<T, T> setter;

        /// <summary>
        /// Initializes the <see cref="ObservableProperty{T}"/> with the default value of <see cref="T"/>
        /// </summary>
        public ObservableProperty()
            : this(default(T))
        { }

        /// <summary>
        /// Initializes the <see cref="ObservableProperty{T}"/> with s specified value for <see cref="T"/>.
        /// </summary>
        /// <param name="value">The value for <see cref="T"/></param>
        public ObservableProperty(T value)
        {
            this.backingField = new BehaviorSubject<T>(value);
        }

        public ObservableProperty(Func<T, T> setter)
            : this(default(T))
        {
            Contract.Requires(setter != null);

            this.setter = setter;
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="ObservableProperty{T}"/>'s backing field.
        /// </summary>
        public T Value
        {
            get { return this.backingField.First(); }
            set
            {
                T transformedValue = value;

                if (setter != null)
                {
                    transformedValue = this.setter(value);
                }

                this.backingField.OnNext(transformedValue);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return this.backingField.Subscribe(observer);
        }
    }
}