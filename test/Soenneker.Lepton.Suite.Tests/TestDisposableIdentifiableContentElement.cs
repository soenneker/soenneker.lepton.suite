using System.Collections.Generic;
using Soenneker.Lepton.Suite;

namespace Soenneker.Lepton.Suite.Tests;

internal sealed class TestDisposableIdentifiableContentElement : LeptonDisposableIdentifiableContentElement
{
    public IReadOnlyDictionary<string, object> Effective => EffectiveAttributes;

    public void Configure(string? id, string? cssClass, string? style, IReadOnlyDictionary<string, object>? additionalAttributes)
    {
        Id = id;
        Class = cssClass;
        Style = style;
        AdditionalAttributes = additionalAttributes;
    }
}
