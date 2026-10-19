using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonIdentifiableContentElement" />
public abstract partial class LeptonIdentifiableContentElement : LeptonIdentifiableElement, ILeptonIdentifiableContentElement
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
