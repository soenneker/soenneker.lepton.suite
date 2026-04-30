using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonElement" />
public abstract class LeptonElement : LeptonComponent, ILeptonElement
{
    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

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

    protected void MergeAdditionalAttributes(Dictionary<string, object> attributes)
    {
        LeptonAttributeBuilder.MergeAdditional(attributes, AdditionalAttributes);
    }

    protected static void MergeClassAttribute(Dictionary<string, object> attributes, string? value)
    {
        LeptonAttributeBuilder.MergeClass(attributes, value);
    }

    protected static void MergeStyleAttribute(Dictionary<string, object> attributes, string? value)
    {
        LeptonAttributeBuilder.MergeStyle(attributes, value);
    }

    protected static string? MergeStyleValues(string? existingValue, string? newValue)
    {
        return LeptonAttributeBuilder.MergeStyleValues(existingValue, newValue);
    }

    protected static void SetAttribute(Dictionary<string, object> attributes, string key, object? value)
    {
        LeptonAttributeBuilder.Set(attributes, key, value);
    }
}
