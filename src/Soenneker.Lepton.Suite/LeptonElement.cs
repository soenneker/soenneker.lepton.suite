using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;
using System.Runtime.InteropServices;
using Soenneker.Extensions.String;

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
        Dictionary<string, object> attributes = CreateAttributes(AdditionalAttributes?.Count ?? 0);

        MergeClassAttribute(attributes, Class);
        MergeStyleAttribute(attributes, Style);
        MergeAdditionalAttributes(attributes);

        return attributes;
    }

    protected Dictionary<string, object> BuildAttributes(string key, object? value)
    {
        Dictionary<string, object> attributes = CreateAttributes((AdditionalAttributes?.Count ?? 0) + 1);

        MergeClassAttribute(attributes, Class);
        MergeStyleAttribute(attributes, Style);
        SetAttribute(attributes, key, value);
        MergeAdditionalAttributes(attributes);

        return attributes;
    }

    protected Dictionary<string, object> BuildAttributes(string key1, object? value1, string key2, object? value2)
    {
        Dictionary<string, object> attributes = CreateAttributes((AdditionalAttributes?.Count ?? 0) + 2);

        MergeClassAttribute(attributes, Class);
        MergeStyleAttribute(attributes, Style);

        SetAttribute(attributes, key1, value1);
        SetAttribute(attributes, key2, value2);

        MergeAdditionalAttributes(attributes);

        return attributes;
    }

    protected Dictionary<string, object> BuildAttributes(ReadOnlySpan<KeyValuePair<string, object?>> values)
    {
        Dictionary<string, object> attributes = CreateAttributes((AdditionalAttributes?.Count ?? 0) + values.Length);

        MergeClassAttribute(attributes, Class);
        MergeStyleAttribute(attributes, Style);

        foreach (KeyValuePair<string, object?> pair in values)
            SetAttribute(attributes, pair.Key, pair.Value);

        MergeAdditionalAttributes(attributes);

        return attributes;
    }

    protected Dictionary<string, object> BuildAttributes(params (string Key, object? Value)[] values)
    {
        return values.Length switch
        {
            0 => BuildAttributes(),
            1 => BuildAttributes(values[0].Key, values[0].Value),
            2 => BuildAttributes(values[0].Key, values[0].Value, values[1].Key, values[1].Value),
            _ => BuildAttributes(ToKeyValuePairs(values))
        };
    }

    private static Dictionary<string, object> CreateAttributes(int additionalCapacity)
    {
        // +2 for Class and Style.
        return new Dictionary<string, object>(additionalCapacity + 2, StringComparer.OrdinalIgnoreCase);
    }

    protected void MergeAdditionalAttributes(Dictionary<string, object> attributes)
    {
        if (AdditionalAttributes is not { Count: > 0 })
            return;

        foreach ((string key, object? value) in AdditionalAttributes)
        {
            if (value is null)
                continue;

            if (key.Equals("class", StringComparison.OrdinalIgnoreCase))
            {
                MergeClassAttribute(attributes, value as string ?? value.ToString());
                continue;
            }

            if (key.Equals("style", StringComparison.OrdinalIgnoreCase))
            {
                MergeStyleAttribute(attributes, value as string ?? value.ToString());
                continue;
            }

            attributes[key] = value;
        }
    }

    protected static void MergeClassAttribute(Dictionary<string, object> attributes, string? value)
    {
        if (value.IsNullOrWhiteSpace())
            return;

        ref object? slot = ref CollectionsMarshal.GetValueRefOrAddDefault(attributes, "class", out bool exists);

        if (!exists || slot is null)
        {
            slot = value;
            return;
        }

        string? existingText = slot as string ?? slot.ToString();

        slot = existingText.IsNullOrWhiteSpace() ? value : string.Concat(existingText, " ", value);
    }

    protected static void MergeStyleAttribute(Dictionary<string, object> attributes, string? value)
    {
        if (value.IsNullOrWhiteSpace())
            return;

        ref object? slot = ref CollectionsMarshal.GetValueRefOrAddDefault(attributes, "style", out bool exists);

        if (!exists || slot is null)
        {
            slot = value;
            return;
        }

        slot = MergeStyleValues(slot as string ?? slot.ToString(), value);
    }

    protected static string? MergeStyleValues(string? existingValue, string? newValue)
    {
        if (existingValue.IsNullOrWhiteSpace())
            return newValue;

        if (newValue.IsNullOrWhiteSpace())
            return existingValue;

        ReadOnlySpan<char> trimmed = existingValue.AsSpan().TrimEnd();

        return trimmed.Length > 0 && trimmed[^1] == ';' ? string.Concat(trimmed, " ", newValue) : string.Concat(trimmed, "; ", newValue);
    }

    protected static void SetAttribute(Dictionary<string, object> attributes, string key, object? value)
    {
        if (value is not null)
            attributes[key] = value;
    }

    private static KeyValuePair<string, object?>[] ToKeyValuePairs((string Key, object? Value)[] values)
    {
        var pairs = new KeyValuePair<string, object?>[values.Length];

        for (var i = 0; i < values.Length; i++)
            pairs[i] = new KeyValuePair<string, object?>(values[i].Key, values[i].Value);

        return pairs;
    }
}
