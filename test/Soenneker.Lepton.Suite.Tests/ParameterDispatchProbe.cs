using System;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Lepton.Suite.Tests;

public partial class ParameterDispatchProbe : LeptonContentElement
{
    protected override bool UseSharedParameterMetadata => true;

    [Parameter] public string? Text { get; set; }
    [Parameter] public int Count { get; set; }
    [Parameter] public string? État { get; set; }
    [CascadingParameter] public string? Context { get; set; }
    [Parameter, SupplyParameterFromQuery] public virtual string? Query { get; set; }
#pragma warning disable BL0007 // Exercise exception handling for a user-defined parameter setter.
    [Parameter] public string? Failing { get => null; set => throw new ArgumentException("setter failed"); }
#pragma warning restore BL0007

    public void Bind(ParameterView parameters) => ApplyGeneratedParameters(parameters);
}
