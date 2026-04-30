namespace Soenneker.Lepton.Suite.Abstract;

/// <summary>
/// Defines the contract for Lepton component base types that accept unmatched element attributes.
/// </summary>
public interface ILeptonElement : ILeptonComponent
{
    /// <summary>
    /// Gets or sets unmatched attributes applied to the component element.
    /// </summary>
    IReadOnlyDictionary<string, object>? Attributes { get; set; }
}