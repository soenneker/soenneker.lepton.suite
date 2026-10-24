using Microsoft.AspNetCore.Components;

namespace Soenneker.Lepton.Suite.PackageSmoke;

public partial class MixedParameterPage : LeptonComponent
{
    protected override bool UseSharedParameterMetadata => true;

    [Parameter]
    public string? CodeBehindText { get; set; }

    public void Bind(ParameterView parameters) => ApplyGeneratedParameters(parameters);
}
