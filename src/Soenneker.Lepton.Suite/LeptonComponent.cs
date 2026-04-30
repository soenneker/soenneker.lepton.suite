using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonComponent" />
public abstract class LeptonComponent : ComponentBase, ILeptonComponent
{
    protected Task RequestRender()
    {
        return InvokeAsync(StateHasChanged);
    }
}