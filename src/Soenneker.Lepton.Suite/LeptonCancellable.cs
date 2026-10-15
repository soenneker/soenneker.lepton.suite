using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

public abstract class LeptonCancellable : LeptonDisposable, ILeptonCancellable
{
    private LeptonCancellationResource _cancellation;

    protected CancellationToken CancellationToken => _cancellation.Token;

    protected bool IsCancellationRequested => _cancellation.IsCancellationRequested;

    public override ValueTask DisposeAsync()
    {
        _cancellation.Dispose();
        return base.DisposeAsync();
    }
}
