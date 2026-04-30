using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonDisposableIdentifiableContentElement" />
public abstract class LeptonDisposableIdentifiableContentElement : LeptonDisposable, ILeptonDisposableIdentifiableContentElement
{
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? Attributes { get; set; }

    [Parameter]
    public string? Id { get; set; }

    protected IReadOnlyDictionary<string, object>? EffectiveAttributes
    {
        get
        {
            if (Id is null)
                return Attributes;

            Dictionary<string, object> attributes = Attributes is null
                ? new Dictionary<string, object>(StringComparer.Ordinal)
                : new Dictionary<string, object>(Attributes, StringComparer.Ordinal);

            attributes["id"] = Id;

            return attributes;
        }
    }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}