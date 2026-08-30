using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

public abstract class LeptonContent : LeptonComponent, ILeptonContent
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
