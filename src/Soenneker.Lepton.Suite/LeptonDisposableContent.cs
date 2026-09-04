using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonDisposableContent" />
public abstract class LeptonDisposableContent : LeptonDisposable, ILeptonDisposableContent
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
