using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonDisposableContentElement" />
public abstract class LeptonDisposableContentElement : LeptonDisposableContent, ILeptonDisposableContentElement
{
    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    protected virtual IReadOnlyDictionary<string, object> EffectiveAttributes => BuildAttributes();

    protected virtual Dictionary<string, object> BuildAttributes()
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes);
    }

    protected virtual Dictionary<string, object> BuildAttributes(string key, object? value)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, key, value);
    }

    protected virtual Dictionary<string, object> BuildAttributes(string key1, object? value1, string key2, object? value2)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, key1, value1, key2, value2);
    }

    protected virtual Dictionary<string, object> BuildAttributes(ReadOnlySpan<KeyValuePair<string, object?>> values)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, values);
    }

    protected Dictionary<string, object> BuildAttributes((string Key, object? Value) value)
    {
        return BuildAttributes(value.Key, value.Value);
    }

    protected Dictionary<string, object> BuildAttributes((string Key, object? Value) first, (string Key, object? Value) second)
    {
        return BuildAttributes(first.Key, first.Value, second.Key, second.Value);
    }

    protected virtual Dictionary<string, object> BuildAttributes(params (string Key, object? Value)[] values)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, values);
    }

    protected virtual Dictionary<string, object> BuildAttributes(params ReadOnlySpan<(string Key, object? Value)> values)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, values);
    }
}
