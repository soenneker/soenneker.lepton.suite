using System;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;

namespace Soenneker.Lepton.Suite.Tests;

public sealed class LeptonCancellationRaceTests
{
    [Test]
    public async Task Disposal_completes_registered_callbacks_before_returning()
    {
        var component = new TestCancellable();
        using var callbackEntered = new ManualResetEventSlim();
        using var releaseCallback = new ManualResetEventSlim();
        var callbackReleased = false;
        using CancellationTokenRegistration registration = component.Token.Register(() =>
        {
            callbackEntered.Set();
            callbackReleased = releaseCallback.Wait(TimeSpan.FromSeconds(10));
        });

        Task disposal = Task.Run(() => component.DisposeAsync().GetAwaiter().GetResult());
        try
        {
            callbackEntered.Wait(TimeSpan.FromSeconds(10)).Should().BeTrue();
            disposal.IsCompleted.Should().BeFalse();
            component.Token.Should().Be(CancellationToken.None);
        }
        finally
        {
            releaseCallback.Set();
        }

        await disposal.WaitAsync(TimeSpan.FromSeconds(10));
        callbackReleased.Should().BeTrue();
        component.Disposed.Should().BeTrue();
    }

    [Test]
    public void Concurrent_readers_and_disposal_cancel_every_published_token()
    {
        Parallel.For(0, 512, _ =>
        {
            var component = new TestCancellable();
            var tokens = new CancellationToken[64];

            Parallel.For(0, 9, participant =>
            {
                if (participant == 0)
                {
                    Thread.Yield();
                    component.DisposeAsync().GetAwaiter().GetResult();
                    return;
                }

                int offset = (participant - 1) * 8;
                for (var index = 0; index < 8; index++)
                {
                    Thread.Yield();
                    tokens[offset + index] = component.Token;
                }
            });

            foreach (CancellationToken token in tokens)
            {
                if (token.CanBeCanceled)
                    token.IsCancellationRequested.Should().BeTrue();
            }

            component.Token.Should().Be(CancellationToken.None);
            component.CancellationRequested.Should().BeFalse();
            component.Disposed.Should().BeTrue();
        });
    }

    [Test]
    public void Disposal_state_is_independent_for_each_component_instance()
    {
        var unused = new TestCancellable();
        var first = new TestCancellable();
        var second = new TestCancellable();
        unused.DisposeAsync().GetAwaiter().GetResult();

        CancellationToken firstToken = first.Token;
        CancellationToken secondToken = second.Token;
        firstToken.CanBeCanceled.Should().BeTrue();
        secondToken.CanBeCanceled.Should().BeTrue();
        secondToken.Should().NotBe(firstToken);

        first.DisposeAsync().GetAwaiter().GetResult();

        first.Token.Should().Be(CancellationToken.None);
        firstToken.IsCancellationRequested.Should().BeTrue();
        second.Token.Should().Be(secondToken);
        secondToken.IsCancellationRequested.Should().BeFalse();
        second.Disposed.Should().BeFalse();

        second.DisposeAsync().GetAwaiter().GetResult();

        secondToken.IsCancellationRequested.Should().BeTrue();
        second.Token.Should().Be(CancellationToken.None);
        unused.Token.Should().Be(CancellationToken.None);
        Action readWaitHandle = () => _ = secondToken.WaitHandle;
        readWaitHandle.Should().Throw<ObjectDisposedException>();
    }
}
