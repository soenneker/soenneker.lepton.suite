using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

public abstract class LeptonComponent : ComponentBase, ILeptonComponent
{
    private Action? _requestRender;

    protected Task RequestRender()
    {
        return InvokeAsync(_requestRender ??= StateHasChanged);
    }
}
