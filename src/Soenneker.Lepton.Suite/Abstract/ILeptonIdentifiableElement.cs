namespace Soenneker.Lepton.Suite.Abstract;

/// <summary>
/// Defines the contract for Lepton element base types with an optional identifier.
/// </summary>
public interface ILeptonIdentifiableElement : ILeptonElement
{
    /// <summary>
    /// Gets or sets the element identifier.
    /// </summary>
    string? Id { get; set; }
}