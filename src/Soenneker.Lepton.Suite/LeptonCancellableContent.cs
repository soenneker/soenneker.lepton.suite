using Soenneker.Extensions.ValueTask;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonCancellableContent" />
public abstract class LeptonCancellableContent : LeptonDisposableContent, ILeptonCancellableContent
{
    private readonly LeptonCancellationResource _cancellation = new();

    protected CancellationToken CancellationToken => _cancellation.Token;

    protected bool IsCancellationRequested => _cancellation.IsCancellationRequested;

    public override async ValueTask DisposeAsync()
    {
        await _cancellation.DisposeAsync().NoSync();
        await base.DisposeAsync().NoSync();
    }
}
