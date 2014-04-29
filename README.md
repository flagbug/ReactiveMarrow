# Overview

ReactiveMarrow adds some helpers and extensions for [Reactive Extensions](https://github.com/Reactive-Extensions/Rx.NET)

# IObservable Extensions

## `SampleAndCombineLatest`

`SampleAndCombineLatest`, as its name says, is a mix of `Sample` and `CombineLatest`.

It takes the latest value of the left observable and combines it with the latest value of the right observable whenever the right sequence produces an element.

## `MatchPair`

Matches equal pairs in two observable sequences, based on a key selector.

## `LimitRate`

Limits the rate at which items are pushed based on a `TimeSpan`.

This method differs from `Sample`, as items aren't dropped and if items come slower than the maximum rate, 
they are processed imemdiately, instead of waiting on the next "tick".

A timespan of 2 seconds would mean the items are processed at a maximum rate of one item every two seconds.

## `ToUnit`

A selector over an `IObservable` that converts it to `IObservable<Unit>`

# `RateLimitedOperationQueue`

A operation processing queue that can limit the number of operations per given timespan.

This differs from the `LimitRate` method, as `RateLimitedOperationQueue` is a class and can be used in a non-functional way.

# `ReactiveProperty`

This class provides a read-write property through the `Value` property 
and is at the same time a `IObservable` backed by a `BehaviourSubject`
