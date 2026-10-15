using System.Collections.Generic;
using System;
using Soenneker.Lepton.Suite;

namespace Soenneker.Lepton.Suite.Tests;

internal sealed class TestElement : LeptonElement
{
    public void Configure(string? cssClass, string? style, IReadOnlyDictionary<string, object>? additionalAttributes)
    {
        Class = cssClass;
        Style = style;
        AdditionalAttributes = additionalAttributes;
    }

    public Dictionary<string, object> Attributes(params (string Key, object? Value)[] values)
    {
        return BuildAttributes(values);
    }

    public Dictionary<string, object> NullSpanAttributes() => BuildAttributes(("a", null), ("b", null), ("c", null), ("d", null));

    public Dictionary<string, object> KeyValueAttributes(ReadOnlySpan<KeyValuePair<string, object?>> values) => BuildAttributes(values);
}
