using Microsoft.AspNetCore.Components;

namespace Soenneker.Lepton.Suite.Tests;

public partial class LargeParameterDispatchProbe : ParameterDispatchProbe
{
    [CascadingParameter] public override string? Query { get; set; }
    [Parameter] public int Value0 { get; set; }
    [Parameter] public int Value1 { get; set; }
    [Parameter] public int Value2 { get; set; }
    [Parameter] public int Value3 { get; set; }
    [Parameter] public int Value4 { get; set; }
    [Parameter] public int Value5 { get; set; }
    [Parameter] public int Value6 { get; set; }
    [Parameter] public int Value7 { get; set; }
    [Parameter] public int Value8 { get; set; }
}
