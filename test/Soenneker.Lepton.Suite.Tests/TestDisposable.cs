using Soenneker.Lepton.Suite;

namespace Soenneker.Lepton.Suite.Tests;

internal sealed class TestDisposable : LeptonDisposable
{
    public bool Disposed => IsDisposed;
}
