using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace ReactiveMarrow
{
    public class ReactiveList<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly List<T> list;
        private readonly double resetThreshold;

        public ReactiveList()
        {
            this.list = new List<T>();
            this.resetThreshold = 0.3;

            // WPF handshakes
            this.CountChanged.Subscribe(x => this.OnPropertyChanged("Count"));
            this.Changed.Subscribe(x => this.OnPropertyChanged("Item[]"));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Capacity
        {
            get { return this.list.Capacity; }
            set { this.list.Capacity = value; }
        }

        public IObservable<NotifyCollectionChangedEventArgs> Changed
        {
            get { return this.Changed(); }
        }

        public int Count
        {
            get { return this.list.Count; }
        }

        public IObservable<int> CountChanged
        {
            get
            {
                return this.Changed
                    .Where(x =>
                        x.Action == NotifyCollectionChangedAction.Add ||
                        x.Action == NotifyCollectionChangedAction.Remove ||
                        x.Action == NotifyCollectionChangedAction.Reset)
                    .Select(x => this.Count)
                    .DistinctUntilChanged();
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IObservable<Tuple<int, T>> ItemAdded
        {
            get
            {
                return this.Changed
                    .Where(x => x.Action == NotifyCollectionChangedAction.Add)
                    .SelectMany(x => x.NewItems.Cast<T>()
                        .Select((item, i) => new Tuple<int, T>(x.NewStartingIndex + i, item)));
            }
        }

        public IObservable<Tuple<int, T>> ItemRemoved
        {
            get
            {
                return this.Changed
                    .Where(x => x.Action == NotifyCollectionChangedAction.Remove)
                    .SelectMany(x => x.OldItems.Cast<T>()
                        .Select((item, i) => new Tuple<int, T>(x.OldStartingIndex + i, item)));
            }
        }

        public IObservable<Unit> Reset
        {
            get
            {
                return this.Changed
                    .Where(x => x.Action == NotifyCollectionChangedAction.Reset)
                    .Select(x => Unit.Default);
            }
        }

        public T this[int index]
        {
            get { return this.list[index]; }
            set
            {
                T objectToReplace = this.list[index];

                this.list[index] = value;

                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, objectToReplace, index));
            }
        }

        public void Add(T item)
        {
            this.list.Add(item);

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, this.list.Count - 1));
        }

        public void AddRange(IEnumerable<T> collection)
        {
            IList<T> itemsToAdd = collection.ToList();

            if (itemsToAdd.Count != 0)
            {
                if (this.ShouldReset(itemsToAdd.Count, this.Count))
                {
                    this.list.AddRange(itemsToAdd);
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }

                else
                {
                    foreach (T item in itemsToAdd)
                    {
                        this.Add(item);
                    }
                }
            }
        }

        public void Clear()
        {
            if (this.Count > 0)
            {
                this.list.Clear();

                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public bool Contains(T item)
        {
            return this.list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            Contract.Ensures(Contract.Result<int>() >= -1);
            Contract.Ensures(Contract.Result<int>() < this.Count);

            return this.list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(index <= this.Count);

            this.list.Insert(index, item);

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(index <= this.Count);
            Contract.Requires(collection != null);

            IList<T> collectionToInsert = collection.ToList();

            this.list.InsertRange(index, collectionToInsert);

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)collectionToInsert, index));
        }

        public bool Remove(T item)
        {
            int index = this.IndexOf(item);

            if (index > -1)
            {
                this.RemoveAt(index);
                return true;
            }

            return false;
        }

        public int RemoveAll(Func<T, bool> match)
        {
            Contract.Requires(match != null);
            Contract.Ensures(Contract.Result<int>() >= 0);
            Contract.Ensures(Contract.Result<int>() <= Contract.OldValue(this.Count));

            var removedList = new List<KeyValuePair<int, T>>(this.list.Capacity);
            int previousCount = this.Count;

            for (int i = 0; i < this.list.Count; i++)
            {
                T item = this.list[i];

                if (match(item))
                {
                    this.list.RemoveAt(i);
                    removedList.Add(new KeyValuePair<int, T>(i, item));
                    i--;
                }
            }

            if (removedList.Count != 0)
            {
                if (this.ShouldReset(removedList.Count, previousCount))
                {
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }

                else
                {
                    foreach (KeyValuePair<int, T> pair in removedList)
                    {
                        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, pair.Value, pair.Key));
                    }
                }
            }

            return removedList.Count;
        }

        public void RemoveAt(int index)
        {
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(index < this.Count);

            T objectToRemove = this.list[index];

            this.list.RemoveAt(index);

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, objectToRemove, index));
        }

        public void Reverse()
        {
            this.list.Reverse();

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Shuffle()
        {
            this.Shuffle(x => Guid.NewGuid());
        }

        public void Shuffle<TKey>(Func<T, TKey> keySelector)
        {
            Contract.Requires(keySelector != null);

            var newList = new List<T>(this.Capacity);
            newList.AddRange(this.OrderBy(keySelector));

            this.list.Clear();
            this.list.AddRange(newList);

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Sort()
        {
            this.list.Sort();

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Sort(Func<T, T, int> comparison)
        {
            Contract.Requires(comparison != null);

            this.list.Sort((x, y) => comparison(x, y));

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool ShouldReset(int changeLength, int currentLength)
        {
            return (double)changeLength / currentLength > this.resetThreshold;
        }
    }
}