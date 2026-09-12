using Soenneker.Atomics.Resources;

namespace Soenneker.Lepton.Suite;

internal sealed class LeptonCancellationResource : IAsyncDisposable
{
    private readonly AtomicResource<CancellationTokenSource> _source;

    internal LeptonCancellationResource()
    {
        _source = new AtomicResource<CancellationTokenSource>(
            factory: static () => new CancellationTokenSource(),
            teardown: static source =>
            {
                try
                {
                    if (!source.IsCancellationRequested)
                        source.Cancel();
                }
                finally
                {
                    source.Dispose();
                }

                return ValueTask.CompletedTask;
            });

    }

    internal CancellationToken Token => _source.GetOrCreate()?.Token ?? CancellationToken.None;

    internal bool IsCancellationRequested => _source.TryGet()?.IsCancellationRequested == true;

    public ValueTask DisposeAsync()
    {
        return _source.DisposeAsync();
    }
}
