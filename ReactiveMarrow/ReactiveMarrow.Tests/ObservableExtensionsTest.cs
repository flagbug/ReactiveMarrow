using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using Xunit;

namespace ReactiveMarrow.Tests
{
    public class ObservableExtensionsTest
    {
        public class TheMatchPairMethod
        {
            [Fact]
            public void CompletesWhenBothSequencesComplete()
            {
                var left = new Subject<int>();
                var right = new Subject<int>();

                var result = left.MatchPair(right, x => x).Materialize().CreateCollection();

                left.OnNext(1);
                left.OnCompleted();

                right.OnNext(1);
                right.OnCompleted();

                Assert.Equal(new[] { Notification.CreateOnNext(new Pair<int>(1, 1)), Notification.CreateOnCompleted<Pair<int>>() }, result);
            }

            [Fact]
            public void DoesntPairValuesFromSameStream()
            {
                var left = new[] { 1, 1 };
                var right = new[] { 2, 2 };

                var results = left.ToObservable().MatchPair(right.ToObservable(), x => x).ToEnumerable().ToList();

                Assert.Empty(results);
            }

            [Fact]
            public void IgnoresSingleValuesAfterCompletion()
            {
                var left = new[] { 1, 2, 1 };
                var right = new[] { 2, 1, 2 };
                var expected = new HashSet<Pair<int>>(Enumerable.Range(1, 2).Select(x => new Pair<int>(x, x)));

                var results = left.ToObservable().MatchPair(right.ToObservable(), x => x).ToEnumerable().ToList();

                Assert.True(expected.SetEquals(results));
            }

            [Fact]
            public void PairsValues()
            {
                var left = new[] { 0, 2, 4, 1, 3, 5 }.ToObservable();
                var right = new[] { 0, 3, 5, 4, 2, 1 }.ToObservable();
                var expected = new HashSet<Pair<int>>(Enumerable.Range(0, 6).Select(x => new Pair<int>(x, x)));

                var results = left.MatchPair(right, x => x).ToEnumerable();

                Assert.True(expected.SetEquals(results));
            }
        }

        public class TheSampleAndCombineLatestMethod
        {
            [Fact]
            public void CombinesValuesWhenRightObservableFires()
            {
                var left = new Subject<int>();
                var right = new Subject<int>();

                var result = left.SampleAndCombineLatest(right, (l, r) => l + r).CreateCollection();

                left.OnNext(1);
                right.OnNext(2);

                Assert.Equal(3, result[0]);

                right.OnNext(3);

                Assert.Equal(4, result[1]);

                Assert.Equal(2, result.Count);
            }

            [Fact]
            public void CompletesWhenBothSequencesComplete()
            {
                var left = new Subject<int>();
                var right = new Subject<int>();

                var result = left.SampleAndCombineLatest(right, (l, r) => l + r).Materialize().CreateCollection();

                left.OnNext(1);
                left.OnCompleted();

                right.OnNext(1);
                right.OnCompleted();

                Assert.Equal(new[] { Notification.CreateOnNext(2), Notification.CreateOnCompleted<int>() }, result);
            }
        }

        public class TheToUnitMethod
        {
            [Fact]
            public void ReturnsUnitForEachPush()
            {
                var input = new[] { 1, 2, 3, 4 }.ToObservable();

                Assert.Equal(4, input.ToUnit().CreateCollection().Count);
            }
        }

        public class Verifications
        {
            /// <summary>
            /// This method is just a verification that <see
            /// cref="Observable.CombineLatest{TSource1,TSource2,TResult}" /> only completes when
            /// both sequences complete.
            /// </summary>
            [Fact]
            public void CombineLatestCompletesWhenBothSequencesComplete()
            {
                var left = new Subject<int>();
                var right = new Subject<int>();

                var result = left.CombineLatest(right, (l, r) => l + r).Materialize().CreateCollection();

                left.OnNext(1);
                left.OnCompleted();

                right.OnNext(1);
                right.OnCompleted();

                Assert.Equal(new[] { Notification.CreateOnNext(2), Notification.CreateOnCompleted<int>() }, result);
            }

            /// <summary>
            /// This method is just a verification that <see
            /// cref="Observable.Sample{TSource,TSample}" /> only completes when both sequences complete.
            /// </summary>
            [Fact]
            public void SampleCompletesWhenBothSequencesComplete()
            {
                var left = new Subject<int>();
                var right = new Subject<int>();

                var result = left.Sample(right).Materialize().CreateCollection();

                left.OnNext(1);
                left.OnCompleted();

                right.OnNext(1);
                right.OnCompleted();

                Assert.Equal(new[] { Notification.CreateOnNext(1), Notification.CreateOnCompleted<int>() }, result);
            }
        }
    }
}