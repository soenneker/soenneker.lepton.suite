using Microsoft.AspNetCore.Components;

namespace Soenneker.Lepton.Suite.Tests;

public sealed partial class OverriddenParameterProbe : ParameterLifecycleProbe
{
    [Parameter] public override string Text { get; set; } = "override";
}
