using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

public abstract class LeptonIdentifiableContentElement : LeptonIdentifiableElement, ILeptonIdentifiableContentElement
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
