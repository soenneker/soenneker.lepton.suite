using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;

namespace Soenneker.Lepton.Suite.Benchmarks;

public partial class ParameterBindingProbe : LeptonComponent
{
    protected override bool UseSharedParameterMetadata => true;

    [Parameter] public string? Text { get; set; }
    [Parameter] public int Count { get; set; }
    [Parameter] public bool Enabled { get; set; }
    [Parameter] public EventCallback Changed { get; set; }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void BindGenerated(ParameterView parameters) => ApplyGeneratedParameters(parameters);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void BindFramework(ParameterView parameters) => parameters.SetParameterProperties(this);
}
