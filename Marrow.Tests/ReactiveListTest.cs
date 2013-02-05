using System;
using Xunit;

namespace Marrow.Tests
{
    public class ReactiveListTest
    {
        [Fact]
        public void AddFiresCountchanged()
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
        public void AddFiresChanged()
        {
            var list = new ReactiveList<int>();
            bool fired = false;

            list.Changed.Subscribe(x => fired = true);

            list.Add(1);

            Assert.True(fired);
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
        public void ClearDoesNotFireChangedIfListIsEmpty()
        {
            var list = new ReactiveList<int>();
            bool fired = false;

            list.Changed.Subscribe(x => fired = true);

            list.Clear();

            Assert.False(fired);
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
        public void ClearDoesNotFireResetIfListIsEmpty()
        {
            var list = new ReactiveList<int>();
            bool fired = false;

            list.Reset.Subscribe(x => fired = true);

            list.Clear();

            Assert.False(fired);
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
        public void RemoveFiresItemRemoved()
        {
            var list = new ReactiveList<int> { 1 };
            bool fired = false;

            list.ItemRemoved.Subscribe(x => fired = true);

            list.Remove(1);

            Assert.True(fired);
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
        public void RemoveDoesNotFireChangedIfListDoesNotContainItem()
        {
            var list = new ReactiveList<int>();
            bool fired = false;

            list.Changed.Subscribe(x => fired = true);

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
        public void ResetIsHotObservable()
        {
            var list = new ReactiveList<int>();

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