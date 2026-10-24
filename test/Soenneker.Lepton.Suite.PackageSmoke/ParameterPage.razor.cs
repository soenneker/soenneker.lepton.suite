using Microsoft.AspNetCore.Components;

namespace Soenneker.Lepton.Suite.PackageSmoke;

public partial class ParameterPage : LeptonComponent
{
    protected override bool UseSharedParameterMetadata => true;

    [Parameter]
    public string? Text { get; set; }

    public void Bind(ParameterView parameters) => ApplyGeneratedParameters(parameters);
}
