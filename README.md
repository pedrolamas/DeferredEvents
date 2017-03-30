# Deferred Events [![Build status](https://ci.appveyor.com/api/projects/status/xuhdrwyjra7v5ra2?svg=true)](https://ci.appveyor.com/project/PedroLamas/deferredevents "Build Status") ![Latest stable version](https://img.shields.io/nuget/v/DeferredEvents.svg?style=flat "Latest stable version")

This repository contains a .NET Standard 1.0 implementation for Deferred Events.

Deferred Events allows event invocators to await for the asynchronous completion of all event handlers.

## Usage

Install the NuGet package by running the following command:

```
Install-Package DeferredEvents
```

## Usage

Start by adding the namespace to your code files:

```csharp
using DeferredEvents;
```

Change your events signature so that they can use `DeferredEventArgs` instead of `EventArgs`:

```csharp
// before
public event EventHandler MyEvent;
```

```csharp
// after
public event EventHandler<DeferredEventArgs> MyEvent;
```

If you have a custom event arguments class inheriting from `EventArgs`, just make it inherit from `DeferredEventArgs` instead.

Last step is to change the way you make your invocations:

```csharp
// before
MyEvent?.Invoke(sender, EventArgs.Empty);
```

```csharp
// after
await MyEvent.InvokeAsync(sender, DeferredEventArgs.Empty);
```

The `InvokeAsync()` method is an extension method that will enable you to ensure we wait for the event handlers to finish their work before we proceed.

Last step will be to change the event handlers code so it can take advantage of the deferred execution:

```csharp
// before
public void OnMyEvent(object sender, EventArgs e)
{
    // code
}
```

```csharp
// after
public async void OnMyEvent(object sender, DeferredEventArgs e)
{
    var deferral = e.GetDeferral();
    
    // awaiteable code
    
    deferral.Complete();
}
```

You only need to call `e.GetDeferral()` if you actually want to the event caller to wait for the completion of the event handler; if you don't call it, it will just behave as a regular event handler.

You **must** call `e.GetDeferral()` to get an `EventDeferral` instance before any `await` call in your code to ensure that the event caller knows that it should wait for `deferral.Complete()`; ideally, it should be the first thing you do in the event handler code.

If you have indeed called `e.GetDeferral()`, then you **must** call `deferral.Complete()` to signal that the event handler has finished.
