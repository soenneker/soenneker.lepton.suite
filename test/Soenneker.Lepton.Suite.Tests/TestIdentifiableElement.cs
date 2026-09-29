using System.Collections.Generic;
using Soenneker.Lepton.Suite;

namespace Soenneker.Lepton.Suite.Tests;

internal sealed class TestIdentifiableElement : LeptonIdentifiableElement
{
    public void Configure(string? id, string? cssClass, string? style, IReadOnlyDictionary<string, object>? additionalAttributes)
    {
        Id = id;
        Class = cssClass;
        Style = style;
        AdditionalAttributes = additionalAttributes;
    }

    public Dictionary<string, object> Attributes(string key, object? value)
    {
        return BuildAttributes(key, value);
    }

    public Dictionary<string, object> TupleAttributes() => BuildAttributes(("data-state", "open"));

    public Dictionary<string, object> TuplePairAttributes() => BuildAttributes(("data-state", "open"), ("data-optional", null));
}
