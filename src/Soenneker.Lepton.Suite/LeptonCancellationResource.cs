namespace Soenneker.Lepton.Suite;

internal struct LeptonCancellationResource
{
    private static readonly object _disposed = new();

    // null, a source, or the terminal sentinel. Keep the lazy state inline so
    // components that never request cancellation need no resource allocation.
    private object? _source;

    internal CancellationToken Token
    {
        get
        {
            object? source = Volatile.Read(ref _source);
            return source is null ? CreateToken() : GetToken(source);
        }
    }

    internal bool IsCancellationRequested => Volatile.Read(ref _source) is LeptonCancellationSource { IsCancellationRequested: true };

    private CancellationToken CreateToken()
    {
        var created = new LeptonCancellationSource();
        object? observed = Interlocked.CompareExchange(ref _source, created, null);
        if (observed is null)
            return created.CachedToken;

        // A losing candidate has never escaped and cannot have registrations.
        created.Dispose();
        return GetToken(observed);
    }

    private static CancellationToken GetToken(object source)
    {
        return source is LeptonCancellationSource cancellation ? cancellation.CachedToken : CancellationToken.None;
    }

    internal void Dispose()
    {
        if (Interlocked.Exchange(ref _source, _disposed) is not LeptonCancellationSource source)
            return;

        try
        {
            if (!source.IsCancellationRequested)
                source.Cancel();
        }
        catch
        {
            // Preserve the resource's best-effort cleanup when callbacks throw.
        }
        finally
        {
            source.Dispose();
        }
    }
}
