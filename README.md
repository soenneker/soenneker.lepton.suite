[![](https://img.shields.io/nuget/v/soenneker.lepton.suite.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.lepton.suite/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.lepton.suite/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.lepton.suite/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.lepton.suite.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.lepton.suite/)
[![](https://img.shields.io/badge/Demo-Live-blueviolet?style=for-the-badge&logo=github)](https://soenneker.github.io/soenneker.lepton.suite)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Lepton.Suite

Small, composable Blazor component base classes for libraries that need clean defaults without buying into a UI framework.

Lepton gives component authors a consistent foundation for child content, element attributes, IDs, async disposal, and cancellation. It is not a component library. It is the base layer you build component libraries on.

## Why Lepton

- Pick exactly the component capabilities you need.
- Keep `class`, `style`, and unmatched attributes consistent across components.
- Get disposal and cancellation patterns without rewriting them in every base class.
- Type against matching interfaces when you need capability-based APIs.

## Install

```bash
dotnet add package Soenneker.Lepton.Suite
```

## Pick A Base

| Need | Use |
| --- | --- |
| Plain Blazor base | `LeptonComponent` |
| Child content | `LeptonContent` |
| Element attributes | `LeptonElement` |
| Child content + element attributes | `LeptonContentElement` |
| Id + child content + element attributes | `LeptonIdentifiableContentElement` |
| Async disposal | `LeptonDisposable` |
| Disposable content | `LeptonDisposableContent` |
| Disposable content element | `LeptonDisposableContentElement` |
| Disposable identifiable content element | `LeptonDisposableIdentifiableContentElement` |
| Disposal-bound cancellation | `LeptonCancellable` |
| Cancellable content element | `LeptonCancellableContentElement` |
| Cancellable identifiable content element | `LeptonCancellableIdentifiableContentElement` |

Every public base has a matching interface in `Soenneker.Lepton.Suite.Abstract`.

## Product Spotlight

### Consistent Element Attributes

Element bases expose `Class`, `Style`, and `AdditionalAttributes`, then build render-ready attributes for you.

```csharp
public sealed class Card : LeptonContentElement
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "section");
        builder.AddMultipleAttributes(1, BuildAttributes("data-slot", "card"));
        builder.AddContent(2, ChildContent);
        builder.CloseElement();
    }
}
```

```razor
<Card Class="card" style="padding: 1rem" data-state="open">
    Content
</Card>
```

`class` and `style` values are merged instead of clobbered.

### Disposal Without Boilerplate

Disposable bases expose `ThrowIfDisposed()` and track disposal state.

```csharp
public abstract class ModuleBackedComponent : LeptonDisposable
{
    private IAsyncDisposable? _module;

    public override async ValueTask DisposeAsync()
    {
        if (_module is not null)
            await _module.DisposeAsync();

        await base.DisposeAsync();
    }
}
```

### Cancellation That Follows Component Lifetime

Cancellable bases cancel their token during `DisposeAsync()`.

```csharp
public abstract class LoadingComponent : LeptonCancellable
{
    protected Task Load()
    {
        ThrowIfDisposed();

        return Task.Delay(TimeSpan.FromSeconds(1), CancellationToken);
    }
}
```

## Design

Cancellable types are an add-on over disposable shapes:

```text
LeptonDisposableContentElement
  -> LeptonCancellableContentElement

LeptonDisposableIdentifiableContentElement
  -> LeptonCancellableIdentifiableContentElement
```

So if you choose a cancellable element, you still get the disposable element behavior underneath it.

## Notes

- `AdditionalAttributes` is the unmatched-attribute parameter for element bases.
- Identifiable element bases add `id` when `Id` is set.
- Cancellable bases expose a protected `CancellationToken`.
- Base classes are abstract and intended for component-library authors.
