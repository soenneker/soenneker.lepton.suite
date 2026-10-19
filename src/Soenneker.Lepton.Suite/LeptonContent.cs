using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonContent" />
public abstract partial class LeptonContent : LeptonComponent, ILeptonContent
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
