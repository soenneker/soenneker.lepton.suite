namespace Soenneker.Lepton.Suite.Abstract;

/// <summary>
/// Defines the contract for Lepton component base types that accept common element attributes.
/// </summary>
public interface ILeptonElement : ILeptonComponent
{
    /// <summary>
    /// Gets or sets the element CSS class attribute.
    /// </summary>
    string? Class { get; set; }

    /// <summary>
    /// Gets or sets the element inline style attribute.
    /// </summary>
    string? Style { get; set; }

    /// <summary>
    /// Gets or sets additional unmatched attributes applied to the component element.
    /// </summary>
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
