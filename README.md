# Soenneker.Lepton.Suite

Fundamental building blocks for Blazor.

`Soenneker.Lepton.Suite` is a small foundational library of abstract Blazor component base types. It does not ship UI components, styling, JavaScript interop, validation, or form helpers. It exists so other Blazor component libraries can share a minimal, composable base layer.

## Installation

```bash
dotnet add package Soenneker.Lepton.Suite
```

## Examples

Create a content component base:

```csharp
using Soenneker.Lepton.Abstract;

public abstract class MyCardBase : LeptonContentElement
{
}
```

Use disposal and cancellation only when a component needs it:

```csharp
using Soenneker.Lepton.Abstract;

public abstract class MyAsyncComponentBase : LeptonCancellable
{
    protected async Task LoadAsync()
    {
        ThrowIfDisposed();

        await Task.Delay(TimeSpan.FromSeconds(1), CancellationToken);
    }
}
```