namespace Soenneker.Lepton.Suite.Tests;

public sealed partial class FrameworkParameterProbe : ParameterLifecycleProbe
{
    protected override bool UseSharedParameterMetadata => false;
}
