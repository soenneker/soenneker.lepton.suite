[![](https://img.shields.io/nuget/v/soenneker.lepton.suite.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.lepton.suite/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.lepton.suite/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.lepton.suite/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.lepton.suite.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.lepton.suite/)
[![](https://img.shields.io/badge/Demo-Live-blueviolet?style=for-the-badge&logo=github)](https://soenneker.github.io/soenneker.lepton.suite)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.lepton.suite/codeql.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.lepton.suite/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Lepton.Suite

Fundamental building blocks for Blazor.

`Soenneker.Lepton.Suite` is a small foundational library of abstract Blazor component base types. It does not ship UI components, styling, JavaScript interop, validation, or form helpers. It exists so other Blazor component libraries can share a minimal, composable base layer.

## Installation

```bash
dotnet add package Soenneker.Lepton.Suite
```

## Examples

Create a content component base:

```csharp
using Soenneker.Lepton.Suite;

public abstract class MyCardBase : LeptonContentElement
{
}
```

Use disposal and cancellation only when a component needs it:

```csharp
using Soenneker.Lepton.Suite;

public abstract class MyAsyncComponentBase : LeptonCancellable
{
    protected async Task LoadAsync()
    {
        ThrowIfDisposed();

        await Task.Delay(TimeSpan.FromSeconds(1), CancellationToken);
    }
}
```
