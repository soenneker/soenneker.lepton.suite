using System.Runtime.CompilerServices;
using Soenneker.Atomics.ValueResources;

namespace Soenneker.Lepton.Suite;

internal struct LeptonCancellationResource
{
    private static readonly Func<LeptonCancellationSource> _factory = CreateSource;
    private static readonly Func<LeptonCancellationSource, ValueTask> _teardown = DisposeSource;

    private ValueAtomicResource<LeptonCancellationSource> _source;

    internal CancellationToken Token
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            LeptonCancellationSource? source = _source.TryGet();
            return source is not null ? source.CachedToken : CreateToken();
        }
    }

    internal bool IsCancellationRequested => _source.TryGet()?.IsCancellationRequested == true;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private CancellationToken CreateToken() => _source.GetOrCreate(_factory, _teardown)?.CachedToken ?? CancellationToken.None;

    private static LeptonCancellationSource CreateSource() => new();

    // This helper's teardown always completes synchronously.
    internal void Dispose() => _source.DisposeAsync(_teardown).GetAwaiter().GetResult();

    private static ValueTask DisposeSource(LeptonCancellationSource source)
    {
        try
        {
            source.Cancel();
        }
        finally
        {
            source.Dispose();
        }

        return default;
    }
}
