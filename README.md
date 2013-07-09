ReactiveMarrow
==============

ReactiveMarrow helps to make the backend of an application reactive

This is a very early alpha version and I may drop the whole concept if 
it cuts too deep into the whole concept of Rx (I'm looking at you, `ReactiveProperty`)

## ReactiveProperty

This class provides a read-write property through the `Value` property 
and is at the same time a `IObservable` backed by a `BehaviourSubject`
