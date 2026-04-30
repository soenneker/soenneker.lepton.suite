using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonIdentifiableElement" />
public abstract class LeptonIdentifiableElement : LeptonElement, ILeptonIdentifiableElement
{
    [Parameter]
    public string? Id { get; set; }

    protected IReadOnlyDictionary<string, object> EffectiveAttributes => BuildAttributes();

    protected new Dictionary<string, object> BuildAttributes()
    {
        Dictionary<string, object> attributes = base.BuildAttributes();

        SetAttribute(attributes, "id", Id);

        return attributes;
    }

    protected new Dictionary<string, object> BuildAttributes(string key, object? value)
    {
        Dictionary<string, object> attributes = base.BuildAttributes(key, value);

        SetAttribute(attributes, "id", Id);

        return attributes;
    }

    protected new Dictionary<string, object> BuildAttributes(string key1, object? value1, string key2, object? value2)
    {
        Dictionary<string, object> attributes = base.BuildAttributes(key1, value1, key2, value2);

        SetAttribute(attributes, "id", Id);

        return attributes;
    }

    protected new Dictionary<string, object> BuildAttributes(ReadOnlySpan<KeyValuePair<string, object?>> values)
    {
        Dictionary<string, object> attributes = base.BuildAttributes(values);

        SetAttribute(attributes, "id", Id);

        return attributes;
    }
}