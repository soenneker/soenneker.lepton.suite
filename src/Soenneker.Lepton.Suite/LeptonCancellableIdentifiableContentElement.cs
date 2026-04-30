using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonCancellableIdentifiableContentElement" />
public abstract class LeptonCancellableIdentifiableContentElement : LeptonCancellable, ILeptonCancellableIdentifiableContentElement
{
    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    [Parameter]
    public string? Id { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected IReadOnlyDictionary<string, object> EffectiveAttributes => BuildAttributes();

    protected Dictionary<string, object> BuildAttributes()
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, id: Id);
    }

    protected Dictionary<string, object> BuildAttributes(string key, object? value)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, key, value, Id);
    }

    protected Dictionary<string, object> BuildAttributes(string key1, object? value1, string key2, object? value2)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, key1, value1, key2, value2, Id);
    }

    protected Dictionary<string, object> BuildAttributes(ReadOnlySpan<KeyValuePair<string, object?>> values)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, values, Id);
    }

    protected Dictionary<string, object> BuildAttributes(params (string Key, object? Value)[] values)
    {
        return LeptonAttributeBuilder.Build(Class, Style, AdditionalAttributes, values, Id);
    }
}