using Soenneker.Extensions.ValueTask;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonCancellableContentElement" />
public abstract class LeptonCancellableContentElement : LeptonDisposableContentElement, ILeptonCancellableContentElement
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
