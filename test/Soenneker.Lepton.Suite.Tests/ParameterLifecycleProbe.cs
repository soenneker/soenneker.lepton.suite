using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Soenneker.Lepton.Suite.Tests;

public partial class ParameterLifecycleProbe : LeptonDisposableIdentifiableContentElement
{
    protected override bool UseSharedParameterMetadata => true;
    [Parameter] public virtual string Text { get; set; } = "initial";
    [Parameter] public Task Initialization { get; set; } = Task.CompletedTask;
    [Parameter] public Task ParameterUpdate { get; set; } = Task.CompletedTask;
    [CascadingParameter] public string? Label { get; set; }
    public int InitializationCount { get; private set; }
    public int ParameterCount { get; private set; }
    public int CompletedUpdates { get; private set; }
    protected override void OnInitialized() => InitializationCount++;
    protected override Task OnInitializedAsync() => Initialization;
    protected override void OnParametersSet() => ParameterCount++;
    protected override async Task OnParametersSetAsync()
    {
        await ParameterUpdate;
        CompletedUpdates++;
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddContent(2, $"{Text}:{Label}:{CompletedUpdates}");
        builder.CloseElement();
    }
}
