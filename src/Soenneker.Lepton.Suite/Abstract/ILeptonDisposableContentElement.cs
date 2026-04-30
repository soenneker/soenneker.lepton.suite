namespace Soenneker.Lepton.Suite.Abstract;

/// <summary>
/// Defines the contract for disposable Lepton component base types that accept unmatched attributes and child content.
/// </summary>
public interface ILeptonDisposableContentElement : ILeptonDisposable, ILeptonContent
{
    /// <summary>
    /// Gets or sets unmatched attributes applied to the component element.
    /// </summary>
    IReadOnlyDictionary<string, object>? Attributes { get; set; }
}
