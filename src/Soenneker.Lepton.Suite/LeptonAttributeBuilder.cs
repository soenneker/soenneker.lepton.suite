using Soenneker.Extensions.String;
using System.Runtime.InteropServices;

namespace Soenneker.Lepton.Suite;

internal static class LeptonAttributeBuilder
{
    internal static Dictionary<string, object> Build(string? cssClass, string? style, IReadOnlyDictionary<string, object>? additionalAttributes, string? id = null, bool idOverridesAdditionalAttributes = false)
    {
        Dictionary<string, object> attributes = Create(additionalAttributes?.Count ?? 0, cssClass, style, id);
        return Complete(attributes, additionalAttributes, id, idOverridesAdditionalAttributes);
    }

    internal static Dictionary<string, object> Build(string? cssClass, string? style, IReadOnlyDictionary<string, object>? additionalAttributes, string key, object? value, string? id = null, bool idOverridesAdditionalAttributes = false)
    {
        Dictionary<string, object> attributes = Create((additionalAttributes?.Count ?? 0) + (value is null ? 0 : 1), cssClass, style, id);

        Set(attributes, key, value);
        return Complete(attributes, additionalAttributes, id, idOverridesAdditionalAttributes);
    }

    internal static Dictionary<string, object> Build(string? cssClass, string? style, IReadOnlyDictionary<string, object>? additionalAttributes, string key1, object? value1, string key2, object? value2, string? id = null, bool idOverridesAdditionalAttributes = false)
    {
        Dictionary<string, object> attributes = Create((additionalAttributes?.Count ?? 0) + (value1 is null ? 0 : 1) + (value2 is null ? 0 : 1), cssClass, style, id);

        Set(attributes, key1, value1);
        Set(attributes, key2, value2);
        return Complete(attributes, additionalAttributes, id, idOverridesAdditionalAttributes);
    }

    internal static Dictionary<string, object> Build(string? cssClass, string? style, IReadOnlyDictionary<string, object>? additionalAttributes, ReadOnlySpan<KeyValuePair<string, object?>> values, string? id = null, bool idOverridesAdditionalAttributes = false)
    {
        Dictionary<string, object> attributes = Create((additionalAttributes?.Count ?? 0) + CountValues(values), cssClass, style, id);

        foreach (KeyValuePair<string, object?> pair in values)
            Set(attributes, pair.Key, pair.Value);
        return Complete(attributes, additionalAttributes, id, idOverridesAdditionalAttributes);
    }

    internal static Dictionary<string, object> Build(string? cssClass, string? style, IReadOnlyDictionary<string, object>? additionalAttributes, (string Key, object? Value)[] values, string? id = null, bool idOverridesAdditionalAttributes = false)
    {
        return Build(cssClass, style, additionalAttributes, values.AsSpan(), id, idOverridesAdditionalAttributes);
    }

    internal static Dictionary<string, object> Build(string? cssClass, string? style, IReadOnlyDictionary<string, object>? additionalAttributes, ReadOnlySpan<(string Key, object? Value)> values, string? id = null, bool idOverridesAdditionalAttributes = false)
    {
        switch (values.Length)
        {
            case 0:
                return Build(cssClass, style, additionalAttributes, id: id, idOverridesAdditionalAttributes: idOverridesAdditionalAttributes);
            case 1:
                return Build(cssClass, style, additionalAttributes, values[0].Key, values[0].Value, id, idOverridesAdditionalAttributes);
            case 2:
                return Build(cssClass, style, additionalAttributes, values[0].Key, values[0].Value, values[1].Key, values[1].Value, id, idOverridesAdditionalAttributes);
        }

        Dictionary<string, object> attributes = Create((additionalAttributes?.Count ?? 0) + CountValues(values), cssClass, style, id);

        foreach ((string key, object? value) in values)
            Set(attributes, key, value);
        return Complete(attributes, additionalAttributes, id, idOverridesAdditionalAttributes);
    }

    internal static void MergeAdditional(Dictionary<string, object> attributes, IReadOnlyDictionary<string, object>? additionalAttributes)
    {
        if (additionalAttributes is not { Count: > 0 })
            return;

        // Enumerating the concrete dictionary keeps its struct enumerator off the heap.
        if (additionalAttributes is Dictionary<string, object> dictionary)
        {
            foreach (KeyValuePair<string, object> pair in dictionary)
                MergeAdditionalAttribute(attributes, pair.Key, pair.Value);

            return;
        }

        foreach ((string key, object? value) in additionalAttributes)
            MergeAdditionalAttribute(attributes, key, value);
    }

    private static void MergeAdditionalAttribute(Dictionary<string, object> attributes, string key, object? value)
    {
        if (value is null)
            return;

        if (key.Equals("class", StringComparison.OrdinalIgnoreCase))
        {
            MergeClass(attributes, value as string ?? value.ToString());
            return;
        }

        if (key.Equals("style", StringComparison.OrdinalIgnoreCase))
        {
            MergeStyle(attributes, value as string ?? value.ToString());
            return;
        }

        attributes[key] = value;
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

    private static Dictionary<string, object> Complete(Dictionary<string, object> attributes, IReadOnlyDictionary<string, object>? additionalAttributes,
        string? id, bool idOverridesAdditionalAttributes)
    {
        if (!idOverridesAdditionalAttributes)
            Set(attributes, "id", id);

        MergeAdditional(attributes, additionalAttributes);

        if (idOverridesAdditionalAttributes)
            Set(attributes, "id", id);

        return attributes;
    }

    private static int CountValues(ReadOnlySpan<KeyValuePair<string, object?>> values)
    {
        var count = 0;
        foreach (KeyValuePair<string, object?> pair in values)
        {
            if (pair.Value is not null)
                count++;
        }

        return count;
    }

    private static int CountValues(ReadOnlySpan<(string Key, object? Value)> values)
    {
        var count = 0;
        foreach ((string _, object? value) in values)
        {
            if (value is not null)
                count++;
        }

        return count;
    }

    private static Dictionary<string, object> Create(int additionalCapacity, string? cssClass, string? style, string? id)
    {
        bool hasClass = !cssClass.IsNullOrWhiteSpace();
        bool hasStyle = !style.IsNullOrWhiteSpace();
        int capacity = additionalCapacity + (hasClass ? 1 : 0) + (hasStyle ? 1 : 0) + (id is null ? 0 : 1);
        var attributes = new Dictionary<string, object>(capacity, StringComparer.OrdinalIgnoreCase);

        if (hasClass)
            attributes["class"] = cssClass!;
        if (hasStyle)
            attributes["style"] = style!;

        return attributes;
    }
}