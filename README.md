# Deferred Events [![Build status](https://ci.appveyor.com/api/projects/status/xuhdrwyjra7v5ra2?svg=true)](https://ci.appveyor.com/project/PedroLamas/deferredevents "Build Status")

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

The `InvokeAsync` is an extension method that will enable you to ensure we wait for the event handlers to finish their work before we proceed.

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
    
    e.Complete();
}
```