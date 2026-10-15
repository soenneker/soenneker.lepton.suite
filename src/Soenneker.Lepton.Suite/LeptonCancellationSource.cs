namespace Soenneker.Lepton.Suite;

internal sealed class LeptonCancellationSource : CancellationTokenSource
{
    // Reading CancellationTokenSource.Token after a concurrent disposal throws.
    // Capture the value before publication so readers can safely finish a race.
    internal CancellationToken CachedToken { get; }

    internal LeptonCancellationSource()
    {
        CachedToken = Token;
    }
}
