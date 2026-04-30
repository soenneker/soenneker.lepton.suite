namespace Soenneker.Lepton.Suite.Abstract;

/// <summary>
/// Defines the contract for identifiable Lepton element base types.
/// </summary>
public interface ILeptonIdentifiableElement : ILeptonElement
{
    /// <summary>
    /// Gets or sets the element identifier.
    /// </summary>
    string? Id { get; set; }
}
