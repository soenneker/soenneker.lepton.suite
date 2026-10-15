using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

public abstract class LeptonIdentifiableElement : LeptonElement, ILeptonIdentifiableElement
{
    [Parameter]
    public string? Id { get; set; }

    protected IReadOnlyDictionary<string, object> EffectiveAttributes => BuildAttributes();

    protected override Dictionary<string, object> BuildAttributes()
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, id: Id, idOverridesAdditionalAttributes: true);
    }

    protected override Dictionary<string, object> BuildAttributes(string key, object? value)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, key, value, Id, idOverridesAdditionalAttributes: true);
    }

    protected override Dictionary<string, object> BuildAttributes(string key1, object? value1, string key2, object? value2)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, key1, value1, key2, value2, Id, idOverridesAdditionalAttributes: true);
    }

    protected override Dictionary<string, object> BuildAttributes(ReadOnlySpan<KeyValuePair<string, object?>> values)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, values, Id, idOverridesAdditionalAttributes: true);
    }

    protected override Dictionary<string, object> BuildAttributes(params (string Key, object? Value)[] values)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, values, Id, idOverridesAdditionalAttributes: true);
    }

    protected override Dictionary<string, object> BuildAttributes(params ReadOnlySpan<(string Key, object? Value)> values)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, values, Id, idOverridesAdditionalAttributes: true);
    }
}
