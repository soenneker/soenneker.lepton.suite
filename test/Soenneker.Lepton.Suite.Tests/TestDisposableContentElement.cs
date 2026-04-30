using System.Collections.Generic;
using Soenneker.Lepton.Suite;

namespace Soenneker.Lepton.Suite.Tests;

internal sealed class TestDisposableContentElement : LeptonDisposableContentElement
{
    public IReadOnlyDictionary<string, object> Effective => EffectiveAttributes;

    public void Configure(string? cssClass, string? style, IReadOnlyDictionary<string, object>? additionalAttributes)
    {
        Class = cssClass;
        Style = style;
        AdditionalAttributes = additionalAttributes;
    }
}
