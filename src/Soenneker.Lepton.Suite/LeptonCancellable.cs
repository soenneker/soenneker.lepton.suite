using Soenneker.Atomics.Resources;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonCancellable" />
public abstract class LeptonCancellable : LeptonDisposable, ILeptonCancellable
{
    private readonly AtomicResource<CancellationTokenSource> _cancellationTokenSource;

    protected LeptonCancellable()
    {
        _cancellationTokenSource = new AtomicResource<CancellationTokenSource>(
            factory: static () => new CancellationTokenSource(),
            teardown: static cancellationTokenSource =>
            {
                if (!cancellationTokenSource.IsCancellationRequested)
                    cancellationTokenSource.Cancel();

                cancellationTokenSource.Dispose();

                return ValueTask.CompletedTask;
            });
    }

    protected CancellationToken CancellationToken => _cancellationTokenSource.GetOrCreate()?.Token ?? CancellationToken.None;

    protected bool IsCancellationRequested => _cancellationTokenSource.TryGet()?.IsCancellationRequested == true;

    public override ValueTask DisposeAsync()
    {
        return DisposeAsyncCore();
    }

    private async ValueTask DisposeAsyncCore()
    {
        await _cancellationTokenSource.DisposeAsync().ConfigureAwait(false);
        await base.DisposeAsync().ConfigureAwait(false);
    }
}
