using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonDisposableContentElement" />
public abstract class LeptonDisposableContentElement : LeptonDisposable, ILeptonDisposableContentElement
{
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? Attributes { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}