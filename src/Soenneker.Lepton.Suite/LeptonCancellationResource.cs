using Soenneker.Atomics.Resources;
using Soenneker.Extensions.ValueTask;

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
                if (!source.IsCancellationRequested)
                    source.Cancel();

                source.Dispose();

                return ValueTask.CompletedTask;
            });

        _source.GetOrCreate();
    }

    internal CancellationToken Token => _source.TryGet()?.Token ?? CancellationToken.None;

    internal bool IsCancellationRequested => _source.TryGet()?.IsCancellationRequested == true;

    public async ValueTask DisposeAsync()
    {
        await _source.DisposeAsync().NoSync();
    }
}
