# Deferred Events

[![Project Maintenance](https://img.shields.io/maintenance/yes/2021.svg)](https://github.com/pedrolamas/DeferredEvents 'GitHub Repository')
[![License](https://img.shields.io/github/license/pedrolamas/DeferredEvents.svg)](https://github.com/pedrolamas/DeferredEvents/blob/master/LICENSE 'License')

[![CI](https://github.com/pedrolamas/DeferredEvents/workflows/CI/badge.svg)](https://github.com/pedrolamas/DeferredEvents/actions 'Build Status')
[![Latest stable version](https://img.shields.io/nuget/v/DeferredEvents.svg?style=flat)](https://www.nuget.org/packages/DeferredEvents/ "Latest stable version")

[![Twitter Follow](https://img.shields.io/twitter/follow/pedrolamas?style=social)](https://twitter.com/pedrolamas '@pedrolamas')

This repository contains a .NET Standard 1.0 implementation for Deferred Events.

Deferred Events allows event invocators to await for the asynchronous completion of all event handlers.

## Installation

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
await MyEvent.InvokeAsync(sender, new DeferredEventArgs());
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

You can also use the `using` pattern if you prefer:

```csharp
// after
public async void OnMyEvent(object sender, DeferredEventArgs e)
{
    using (e.GetDeferral())
    {
        // awaiteable code
    }
}
```

You only need to call `e.GetDeferral()` if you actually want to the event caller to wait for the completion of the event handler; if you don't call it, it will just behave as a regular event handler.

You **must** call `e.GetDeferral()` to get an `EventDeferral` instance before any `await` call in your code to ensure that the event caller knows that it should wait for `deferral.Complete()`; ideally, it should be the first thing you do in the event handler code.

If you have indeed called `e.GetDeferral()`, then you **must** call `deferral.Complete()` to signal that the event handler has finished.
