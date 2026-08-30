using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

public abstract class LeptonContentElement : LeptonElement, ILeptonContentElement
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
