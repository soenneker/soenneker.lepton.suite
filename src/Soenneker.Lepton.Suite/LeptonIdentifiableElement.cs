using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonIdentifiableElement" />
public abstract class LeptonIdentifiableElement : LeptonElement, ILeptonIdentifiableElement
{
    /// <exclude />
    [Parameter]
    public string? Id { get; set; }

    /// <exclude />
    protected IReadOnlyDictionary<string, object> EffectiveAttributes => BuildAttributes();

    protected new Dictionary<string, object> BuildAttributes(params (string Key, object? Value)[] values)
    {
        var attributes = base.BuildAttributes(values);

        SetAttribute(attributes, "id", Id);

        return attributes;
    }
}
