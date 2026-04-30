using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonIdentifiableElement" />
public abstract class LeptonIdentifiableElement : LeptonElement, ILeptonIdentifiableElement
{
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
}