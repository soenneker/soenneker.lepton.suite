using System.Threading;
using Soenneker.Lepton.Suite;

namespace Soenneker.Lepton.Suite.Tests;

internal sealed class TestCancellable : LeptonCancellable
{
    public CancellationToken Token => CancellationToken;

    public bool CancellationRequested => IsCancellationRequested;
}
