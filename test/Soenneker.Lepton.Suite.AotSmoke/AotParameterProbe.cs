using Microsoft.AspNetCore.Components;

namespace Soenneker.Lepton.Suite.AotSmoke;

internal sealed partial class AotParameterProbe<T> : LeptonDisposableIdentifiableContentElement
{
    protected override bool UseSharedParameterMetadata => true;

    [Parameter] public T? Value { get; set; }
    [Parameter] public int Count { get; set; }
    [Parameter] public EventCallback<T> Changed { get; set; }
    [Parameter] public RenderFragment<T>? Template { get; set; }

    public void Bind(ParameterView parameters) => ApplyGeneratedParameters(parameters);
}
