using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonDisposableIdentifiableContentElement" />
public abstract class LeptonDisposableIdentifiableContentElement : LeptonDisposableContentElement, ILeptonDisposableIdentifiableContentElement
{
    [Parameter]
    public string? Id { get; set; }

    protected override IReadOnlyDictionary<string, object> EffectiveAttributes => BuildAttributes();

    protected override Dictionary<string, object> BuildAttributes()
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, id: Id);
    }

    protected override Dictionary<string, object> BuildAttributes(string key, object? value)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, key, value, Id);
    }

    protected override Dictionary<string, object> BuildAttributes(string key1, object? value1, string key2, object? value2)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, key1, value1, key2, value2, Id);
    }

    protected override Dictionary<string, object> BuildAttributes(ReadOnlySpan<KeyValuePair<string, object?>> values)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, values, Id);
    }

    protected override Dictionary<string, object> BuildAttributes(params (string Key, object? Value)[] values)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, values, Id);
    }
}
