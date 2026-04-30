using System.Threading;
using Soenneker.Lepton.Suite;

namespace Soenneker.Lepton.Suite.Tests;

internal sealed class TestCancellableContent : LeptonCancellableContent
{
    public CancellationToken Token => CancellationToken;

    public bool CancellationRequested => IsCancellationRequested;
}
