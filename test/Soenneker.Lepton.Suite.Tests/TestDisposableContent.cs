using Soenneker.Lepton.Suite;

namespace Soenneker.Lepton.Suite.Tests;

internal sealed class TestDisposableContent : LeptonDisposableContent
{
    public bool Disposed => IsDisposed;
}
