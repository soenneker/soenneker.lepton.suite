using Microsoft.AspNetCore.Components;

namespace Soenneker.Lepton.Suite.Tests;

public sealed partial class HiddenParameterProbe : ParameterLifecycleProbe
{
    [Parameter] public new string Text { get; set; } = "hidden";
}
