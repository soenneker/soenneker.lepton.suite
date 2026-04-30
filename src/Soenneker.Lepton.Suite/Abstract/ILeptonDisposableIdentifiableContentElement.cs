namespace Soenneker.Lepton.Suite.Abstract;

/// <summary>
/// Defines the contract for disposable Lepton component base types that accept unmatched attributes, an identifier, and child content.
/// </summary>
public interface ILeptonDisposableIdentifiableContentElement : ILeptonDisposable, ILeptonContent
{
    /// <summary>
    /// Gets or sets unmatched attributes applied to the component element.
    /// </summary>
    IReadOnlyDictionary<string, object>? Attributes { get; set; }

    /// <summary>
    /// Gets or sets the element identifier.
    /// </summary>
    string? Id { get; set; }
}
