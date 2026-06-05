using Soenneker.Atomics.ValueBools;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

/// <inheritdoc cref="ILeptonDisposable" />
public abstract class LeptonDisposable : LeptonComponent, ILeptonDisposable
{
    private ValueAtomicBool _isDisposed;
    
    protected bool IsDisposed => _isDisposed.Read();

    /// <summary>
    /// Asynchronously releases resources used by the current instance.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual ValueTask DisposeAsync()
    {
        _isDisposed.TrySetTrue();

        return new ValueTask();
    }

    protected void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
    }
}