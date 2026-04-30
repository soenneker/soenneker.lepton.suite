using Soenneker.Atomics.Resources;
using Soenneker.Extensions.ValueTask;
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
            teardown: static source =>
            {
                if (!source.IsCancellationRequested)
                    source.Cancel();

                source.Dispose();

                return ValueTask.CompletedTask;
            });

        _cancellationTokenSource.GetOrCreate();
    }

    protected CancellationToken CancellationToken =>
        _cancellationTokenSource.TryGet()?.Token ?? CancellationToken.None;

    protected bool IsCancellationRequested =>
        _cancellationTokenSource.TryGet()?.IsCancellationRequested == true;

    public override async ValueTask DisposeAsync()
    {
        await _cancellationTokenSource.DisposeAsync().NoSync();
        await base.DisposeAsync().NoSync();
    }
}