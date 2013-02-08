using System;
using System.Collections.Generic;
using Xunit;

namespace Marrow.Tests
{
    public class ReactiveListTest
    {
        [Fact]
        public void AddFiresChanged()
        {
            var list = new ReactiveList<int>();
            bool fired = false;

            list.Changed.Subscribe(x => fired = true);

            list.Add(1);

            Assert.True(fired);
        }

        [Fact]
        public void AddFiresCountChanged()
        {
            var list = new ReactiveList<int>();
            bool fired = false;

            list.CountChanged.Subscribe(x => fired = true);

            list.Add(1);

            Assert.True(fired);
        }

        [Fact]
        public void AddFiresItemAdded()
        {
            var list = new ReactiveList<int>();
            bool fired = false;

            list.ItemAdded.Subscribe(x => fired = true);

            list.Add(1);

            Assert.True(fired);
        }

        [Fact]
        public void AddRangeFiresSingleItemAddEvents()
        {
            var list = new ReactiveList<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            int fireCount = 0;

            list.ItemAdded.Subscribe(x => fireCount++);

            list.AddRange(new[] { 0, 1, 2 });

            Assert.Equal(3, fireCount);
        }

        [Fact]
        public void ClearDoesNotFireChangedIfListIsEmpty()
        {
            var list = new ReactiveList<int>();
            bool fired = false;

            list.Changed.Subscribe(x => fired = true);

            list.Clear();

            Assert.False(fired);
        }

        [Fact]
        public void ClearDoesNotFireResetIfListIsEmpty()
        {
            var list = new ReactiveList<int>();
            bool fired = false;

            list.Reset.Subscribe(x => fired = true);

            list.Clear();

            Assert.False(fired);
        }

        [Fact]
        public void ClearFiresChanged()
        {
            var list = new ReactiveList<int> { 1 };
            bool fired = false;

            list.Changed.Subscribe(x => fired = true);

            list.Clear();

            Assert.True(fired);
        }

        [Fact]
        public void ClearFiresReset()
        {
            var list = new ReactiveList<int> { 1 };
            bool fired = false;

            list.Reset.Subscribe(x => fired = true);

            list.Clear();

            Assert.True(fired);
        }

        [Fact]
        public void CountChangedIsHotObservable()
        {
            var list = new ReactiveList<int>();

            bool fired1 = false;
            bool fired2 = false;

            list.CountChanged.Subscribe(x => fired1 = true);
            list.CountChanged.Subscribe(x => fired2 = true);

            list.Add(1);

            Assert.True(fired1);
            Assert.True(fired2);
        }

        [Fact]
        public void ItemAddedIndexesAreCorrect()
        {
            var list = new ReactiveList<int>();
            var indexes = new List<int>();

            list.ItemAdded.Subscribe(x => indexes.Add(x.Item1));

            list.Add(1);
            list.Add(2);
            list.Add(3);

            Assert.Equal(new[] { 0, 1, 2 }, indexes);
        }

        [Fact]
        public void ItemRemovedIndexesAreCorrect()
        {
            var list = new ReactiveList<int> { 1, 2, 3 };
            var indexes = new List<int>();

            list.ItemRemoved.Subscribe(x => indexes.Add(x.Item1));

            list.Remove(3);
            list.Remove(2);
            list.Remove(1);

            Assert.Equal(new[] { 2, 1, 0 }, indexes);
        }

        [Fact]
        public void RemoveDoesNotFireChangedIfListDoesNotContainItem()
        {
            var list = new ReactiveList<int>();
            bool fired = false;

            list.Changed.Subscribe(x => fired = true);

            list.Remove(1);

            Assert.False(fired);
        }

        [Fact]
        public void RemoveDoesNotFireItemRemovedIfListDoesNotContainItem()
        {
            var list = new ReactiveList<int>();
            bool fired = false;

            list.ItemRemoved.Subscribe(x => fired = true);

            list.Remove(1);

            Assert.False(fired);
        }

        [Fact]
        public void RemoveFiresChanged()
        {
            var list = new ReactiveList<int> { 1 };
            bool fired = false;

            list.Changed.Subscribe(x => fired = true);

            list.Remove(1);

            Assert.True(fired);
        }

        [Fact]
        public void RemoveFiresItemRemoved()
        {
            var list = new ReactiveList<int> { 1 };
            bool fired = false;

            list.ItemRemoved.Subscribe(x => fired = true);

            list.Remove(1);

            Assert.True(fired);
        }

        [Fact]
        public void ResetIsHotObservable()
        {
            var list = new ReactiveList<int> { 1 };

            bool fired1 = false;
            bool fired2 = false;

            list.Reset.Subscribe(x => fired1 = true);
            list.Reset.Subscribe(x => fired2 = true);

            list.Clear();

            Assert.True(fired1);
            Assert.True(fired2);
        }
    }
}