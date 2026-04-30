namespace Soenneker.Lepton.Suite.Abstract;

/// <summary>
/// Defines the contract for disposable Lepton component base types that accept unmatched attributes and child content.
/// </summary>
public interface ILeptonDisposableContentElement : ILeptonDisposable, ILeptonElement, ILeptonContent;