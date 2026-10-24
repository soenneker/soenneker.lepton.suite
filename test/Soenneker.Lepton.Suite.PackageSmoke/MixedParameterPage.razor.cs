using Microsoft.AspNetCore.Components;

namespace Soenneker.Lepton.Suite.PackageSmoke;

public partial class MixedParameterPage : LeptonComponent
{
    [Parameter]
    public string? CodeBehindText { get; set; }

    public void Bind(ParameterView parameters) => parameters.SetParameterProperties(this);
}
