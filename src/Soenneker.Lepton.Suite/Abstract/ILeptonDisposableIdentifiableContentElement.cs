namespace Soenneker.Lepton.Suite.Abstract;

/// <summary>
/// Defines the contract for disposable Lepton component base types that accept unmatched attributes, an identifier, and child content.
/// </summary>
public interface ILeptonDisposableIdentifiableContentElement : ILeptonDisposable, ILeptonIdentifiableElement, ILeptonContent;
