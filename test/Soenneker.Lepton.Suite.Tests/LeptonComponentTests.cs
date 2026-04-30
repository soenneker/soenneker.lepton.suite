using System.Collections.Generic;
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

    [Test]
    public void LeptonElement_builds_attributes_with_merged_class_and_style()
    {
        var component = new TestElement();
        component.Configure(
            "base",
            "display:block",
            new Dictionary<string, object>
            {
                ["class"] = "additional",
                ["style"] = "color:red",
                ["role"] = "button"
            });

        Dictionary<string, object> attributes = component.Attributes(("data-state", "open"));

        attributes.Should().NotContainKey("id");
        attributes["class"].Should().Be("base additional");
        attributes["style"].Should().Be("display:block; color:red");
        attributes["role"].Should().Be("button");
        attributes["data-state"].Should().Be("open");
    }

    [Test]
    public void LeptonIdentifiableElement_builds_id_aware_attributes()
    {
        var component = new TestIdentifiableElement();
        component.Configure("primary", "base", null, null);

        Dictionary<string, object> attributes = component.Attributes("data-state", "open");

        attributes["id"].Should().Be("primary");
        attributes["class"].Should().Be("base");
        attributes["data-state"].Should().Be("open");
    }
}
