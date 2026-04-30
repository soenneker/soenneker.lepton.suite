using Soenneker.Extensions.String;
using System.Runtime.InteropServices;

namespace Soenneker.Lepton.Suite;

internal static class LeptonAttributeBuilder
{
    internal static Dictionary<string, object> Build(string? cssClass, string? style, IReadOnlyDictionary<string, object>? additionalAttributes, string? id = null)
    {
        Dictionary<string, object> attributes = Create(additionalAttributes?.Count ?? 0, id is null ? 2 : 3);

        MergeClass(attributes, cssClass);
        MergeStyle(attributes, style);
        Set(attributes, "id", id);
        MergeAdditional(attributes, additionalAttributes);

        return attributes;
    }

    internal static Dictionary<string, object> Build(string? cssClass, string? style, IReadOnlyDictionary<string, object>? additionalAttributes, string key, object? value, string? id = null)
    {
        Dictionary<string, object> attributes = Create((additionalAttributes?.Count ?? 0) + 1, id is null ? 2 : 3);

        MergeClass(attributes, cssClass);
        MergeStyle(attributes, style);
        Set(attributes, key, value);
        Set(attributes, "id", id);
        MergeAdditional(attributes, additionalAttributes);

        return attributes;
    }

    internal static Dictionary<string, object> Build(string? cssClass, string? style, IReadOnlyDictionary<string, object>? additionalAttributes, string key1, object? value1, string key2, object? value2, string? id = null)
    {
        Dictionary<string, object> attributes = Create((additionalAttributes?.Count ?? 0) + 2, id is null ? 2 : 3);

        MergeClass(attributes, cssClass);
        MergeStyle(attributes, style);
        Set(attributes, key1, value1);
        Set(attributes, key2, value2);
        Set(attributes, "id", id);
        MergeAdditional(attributes, additionalAttributes);

        return attributes;
    }

    internal static Dictionary<string, object> Build(string? cssClass, string? style, IReadOnlyDictionary<string, object>? additionalAttributes, ReadOnlySpan<KeyValuePair<string, object?>> values, string? id = null)
    {
        Dictionary<string, object> attributes = Create((additionalAttributes?.Count ?? 0) + values.Length, id is null ? 2 : 3);

        MergeClass(attributes, cssClass);
        MergeStyle(attributes, style);

        foreach (KeyValuePair<string, object?> pair in values)
            Set(attributes, pair.Key, pair.Value);

        Set(attributes, "id", id);
        MergeAdditional(attributes, additionalAttributes);

        return attributes;
    }

    internal static Dictionary<string, object> Build(string? cssClass, string? style, IReadOnlyDictionary<string, object>? additionalAttributes, (string Key, object? Value)[] values, string? id = null)
    {
        return values.Length switch
        {
            0 => Build(cssClass, style, additionalAttributes, id: id),
            1 => Build(cssClass, style, additionalAttributes, values[0].Key, values[0].Value, id),
            2 => Build(cssClass, style, additionalAttributes, values[0].Key, values[0].Value, values[1].Key, values[1].Value, id),
            _ => Build(cssClass, style, additionalAttributes, ToKeyValuePairs(values), id)
        };
    }

    internal static void MergeAdditional(Dictionary<string, object> attributes, IReadOnlyDictionary<string, object>? additionalAttributes)
    {
        if (additionalAttributes is not { Count: > 0 })
            return;

        foreach ((string key, object? value) in additionalAttributes)
        {
            if (value is null)
                continue;

            if (key.Equals("class", StringComparison.OrdinalIgnoreCase))
            {
                MergeClass(attributes, value as string ?? value.ToString());
                continue;
            }

            if (key.Equals("style", StringComparison.OrdinalIgnoreCase))
            {
                MergeStyle(attributes, value as string ?? value.ToString());
                continue;
            }

            attributes[key] = value;
        }
    }

    internal static void MergeClass(Dictionary<string, object> attributes, string? value)
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

    internal static void MergeStyle(Dictionary<string, object> attributes, string? value)
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

    internal static string? MergeStyleValues(string? existingValue, string? newValue)
    {
        if (existingValue.IsNullOrWhiteSpace())
            return newValue;

        if (newValue.IsNullOrWhiteSpace())
            return existingValue;

        ReadOnlySpan<char> trimmed = existingValue.AsSpan().TrimEnd();

        return trimmed.Length > 0 && trimmed[^1] == ';' ? string.Concat(trimmed, " ", newValue) : string.Concat(trimmed, "; ", newValue);
    }

    internal static void Set(Dictionary<string, object> attributes, string key, object? value)
    {
        if (value is not null)
            attributes[key] = value;
    }

    private static Dictionary<string, object> Create(int additionalCapacity, int baseCapacity)
    {
        return new Dictionary<string, object>(additionalCapacity + baseCapacity, StringComparer.OrdinalIgnoreCase);
    }

    private static KeyValuePair<string, object?>[] ToKeyValuePairs((string Key, object? Value)[] values)
    {
        var pairs = new KeyValuePair<string, object?>[values.Length];

        for (var i = 0; i < values.Length; i++)
            pairs[i] = new KeyValuePair<string, object?>(values[i].Key, values[i].Value);

        return pairs;
    }
}
