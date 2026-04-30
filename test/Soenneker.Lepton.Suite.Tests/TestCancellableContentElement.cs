using System.Collections.Generic;
using System.Threading;
using Soenneker.Lepton.Suite;

namespace Soenneker.Lepton.Suite.Tests;

internal sealed class TestCancellableContentElement : LeptonCancellableContentElement
{
    public CancellationToken Token => CancellationToken;

    public bool CancellationRequested => IsCancellationRequested;

    public IReadOnlyDictionary<string, object> Effective => EffectiveAttributes;

    public void Configure(string? cssClass, string? style, IReadOnlyDictionary<string, object>? additionalAttributes)
    {
        Class = cssClass;
        Style = style;
        AdditionalAttributes = additionalAttributes;
    }
}
