namespace Soenneker.Lepton.Suite.Abstract;

/// <summary>
/// Defines the contract for asynchronously disposable Lepton component base types.
/// </summary>
public interface ILeptonDisposable : ILeptonComponent, IAsyncDisposable;