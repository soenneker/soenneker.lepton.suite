using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonElement" />
public abstract class LeptonElement : LeptonComponent, ILeptonElement
{
    /// <exclude />
    [Parameter]
    public string? Class { get; set; }

    /// <exclude />
    [Parameter]
    public string? Style { get; set; }

    /// <exclude />
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    protected Dictionary<string, object> BuildAttributes(params (string Key, object? Value)[] values)
    {
        var attributes = new Dictionary<string, object>();

        MergeClassAttribute(attributes, Class);
        MergeStyleAttribute(attributes, Style);

        foreach ((string key, object? value) in values)
            SetAttribute(attributes, key, value);

        if (AdditionalAttributes is null || AdditionalAttributes.Count == 0)
            return attributes;

        foreach ((string key, object? value) in AdditionalAttributes)
        {
            if (string.Equals(key, "class", StringComparison.OrdinalIgnoreCase))
            {
                MergeClassAttribute(attributes, value?.ToString());
                continue;
            }

            if (string.Equals(key, "style", StringComparison.OrdinalIgnoreCase))
            {
                MergeStyleAttribute(attributes, value?.ToString());
                continue;
            }

            SetAttribute(attributes, key, value);
        }

        return attributes;
    }

    protected static void MergeClassAttribute(IDictionary<string, object> attributes, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        if (attributes.TryGetValue("class", out object? existing) && existing is not null)
        {
            string existingText = existing.ToString() ?? string.Empty;
            attributes["class"] = string.IsNullOrWhiteSpace(existingText) ? value : $"{existingText} {value}";
            return;
        }

        attributes["class"] = value;
    }

    protected static void MergeStyleAttribute(IDictionary<string, object> attributes, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        if (attributes.TryGetValue("style", out object? existing) && existing is not null)
        {
            attributes["style"] = MergeStyleValues(existing.ToString(), value)!;
            return;
        }

        attributes["style"] = value;
    }

    protected static string? MergeStyleValues(string? existingValue, string? newValue)
    {
        if (string.IsNullOrWhiteSpace(existingValue))
            return newValue;

        if (string.IsNullOrWhiteSpace(newValue))
            return existingValue;

        string trimmed = existingValue.TrimEnd();
        return trimmed.EndsWith(';') ? $"{trimmed} {newValue}" : $"{trimmed}; {newValue}";
    }

    protected static void SetAttribute(IDictionary<string, object> attributes, string key, object? value)
    {
        if (value is null)
            return;

        attributes[key] = value;
    }
}
