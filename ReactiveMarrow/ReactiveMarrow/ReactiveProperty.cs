using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveMarrow
{
    /// <summary>
    /// Provides an implementation of <see cref="IObservable{T}"/> that is also a read-write property.
    /// </summary>
    /// <typeparam name="T">The type of the object that should be exposed.</typeparam>
    public class ReactiveProperty<T> : IObservable<T>
    {
        private readonly BehaviorSubject<T> backingField;
        private readonly Func<T> getter;
        private readonly Func<T, T> setter;
        private readonly Expression<Func<T, bool>> setterContract;

        public ReactiveProperty(Func<T> getter, Func<T, T> setter = null, Expression<Func<T, bool>> setterContract = null)
            : this(getter())
        {
            this.getter = getter;
            this.setter = setter;
            this.setterContract = setterContract;
        }

        /// <summary>
        /// Initializes the <see cref="ReactiveProperty{T}"/> with an optional default value for <see cref="T"/>.
        /// </summary>
        /// <param name="value">The value for <see cref="T"/></param>
        public ReactiveProperty(T value = default(T))
        {
            this.backingField = new BehaviorSubject<T>(value);
        }

        public ReactiveProperty(Expression<Func<T, bool>> setterContract)
            : this()
        {
            Contract.Requires(setterContract != null);

            this.setterContract = setterContract;
        }

        public ReactiveProperty(Func<T, T> setter, Expression<Func<T, bool>> setterContract = null)
            : this()
        {
            Contract.Requires(setter != null);

            this.setter = setter;
            this.setterContract = setterContract;
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="ReactiveProperty{T}"/>'s backing field.
        /// </summary>
        public T Value
        {
            get
            {
                if (getter == null)
                {
                    return this.backingField.First();
                }

                return this.getter();
            }

            set
            {
                if (this.setterContract != null && !this.setterContract.Compile()(value))
                {
                    throw new Exception(ExpressionToString(this.setterContract));
                }

                T transformedValue = value;

                if (this.setter != null)
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

        private static string ExpressionToString<TExpression>(Expression<TExpression> expression)
        {
            string expBody = expression.Body.ToString();

            var paramName = expression.Parameters[0].Name;
            var paramTypeName = expression.Parameters[0].Type.Name;

            expBody = expBody.Replace(paramName + ".", paramTypeName + ".");

            return expBody;
        }
    }
}