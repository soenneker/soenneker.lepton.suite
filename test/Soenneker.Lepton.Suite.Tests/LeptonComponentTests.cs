using AwesomeAssertions;
using Soenneker.Lepton.Suite;
using Soenneker.Lepton.Suite.Abstract;
using Soenneker.Tests.Unit;

namespace Soenneker.Lepton.Suite.Tests;

public sealed class LeptonComponentTests : UnitTest
{
    [Test]
    public void LeptonComponent_does_not_define_common_element_parameters()
    {
        typeof(LeptonComponent).GetProperty("ChildContent").Should().BeNull();
        typeof(LeptonComponent).GetProperty("Attributes").Should().BeNull();
        typeof(LeptonComponent).GetProperty("Id").Should().BeNull();
    }

    [Test]
    public void LeptonDisposable_dispose_is_idempotent()
    {
        var component = new TestDisposable();

        component.DisposeAsync().AsTask().GetAwaiter().GetResult();
        component.DisposeAsync().AsTask().GetAwaiter().GetResult();

        component.Disposed.Should().BeTrue();
        component.Should().BeAssignableTo<ILeptonDisposable>();
    }

    [Test]
    public void LeptonCancellable_creates_token_lazily()
    {
        var component = new TestCancellable();

        component.CancellationRequested.Should().BeFalse();

        _ = component.Token;

        component.DisposeAsync().AsTask().GetAwaiter().GetResult();

        component.CancellationRequested.Should().BeFalse();
    }
}
