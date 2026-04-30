using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonCancellable" />
public abstract class LeptonCancellable : LeptonDisposable, ILeptonCancellable
{
    private CancellationTokenSource? _cancellationTokenSource;

    protected CancellationToken CancellationToken => (_cancellationTokenSource ??= new CancellationTokenSource()).Token;

    protected bool IsCancellationRequested => _cancellationTokenSource?.IsCancellationRequested == true;

    public override ValueTask DisposeAsync()
    {
        CancellationTokenSource? cancellationTokenSource = _cancellationTokenSource;

        if (cancellationTokenSource is not null)
        {
            if (!cancellationTokenSource.IsCancellationRequested)
                cancellationTokenSource.Cancel();

            cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        return base.DisposeAsync();
    }
}