using System.Collections.Generic;
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
}
