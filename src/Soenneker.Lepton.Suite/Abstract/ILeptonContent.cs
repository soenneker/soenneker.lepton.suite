using Microsoft.AspNetCore.Components;

namespace Soenneker.Lepton.Suite.Abstract;

/// <summary>
/// Defines the contract for Lepton component base types that accept child content.
/// </summary>
public interface ILeptonContent : ILeptonComponent
{
    /// <summary>
    /// Gets or sets the child content rendered by the component.
    /// </summary>
    RenderFragment? ChildContent { get; set; }
}