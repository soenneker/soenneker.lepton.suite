using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonDisposableContentElement" />
public abstract class LeptonDisposableContentElement : LeptonDisposable, ILeptonDisposableContentElement
{
    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    protected IReadOnlyDictionary<string, object> EffectiveAttributes => BuildAttributes();

    protected Dictionary<string, object> BuildAttributes()
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes);
    }

    protected Dictionary<string, object> BuildAttributes(string key, object? value)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, key, value);
    }

    protected Dictionary<string, object> BuildAttributes(string key1, object? value1, string key2, object? value2)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, key1, value1, key2, value2);
    }

    protected Dictionary<string, object> BuildAttributes(ReadOnlySpan<KeyValuePair<string, object?>> values)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, values);
    }

    protected Dictionary<string, object> BuildAttributes(params (string Key, object? Value)[] values)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, values);
    }
}