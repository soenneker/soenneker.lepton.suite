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
    public void Element_interfaces_track_element_parameters()
    {
        typeof(ILeptonElement).GetProperty("Class").Should().NotBeNull();
        typeof(ILeptonElement).GetProperty("Style").Should().NotBeNull();
        typeof(ILeptonElement).GetProperty("AdditionalAttributes").Should().NotBeNull();
        typeof(ILeptonIdentifiableElement).GetProperty("Id").Should().NotBeNull();
        typeof(ILeptonContent).GetProperty("ChildContent").Should().NotBeNull();
    }

    [Test]
    public void Disposable_and_cancellable_element_interfaces_use_element_contracts()
    {
        typeof(ILeptonContentElement).IsAssignableFrom(typeof(ILeptonDisposableContentElement)).Should().BeTrue();
        typeof(ILeptonIdentifiableContentElement).IsAssignableFrom(typeof(ILeptonDisposableIdentifiableContentElement)).Should().BeTrue();
        typeof(ILeptonIdentifiableContentElement).IsAssignableFrom(typeof(ILeptonCancellableIdentifiableContentElement)).Should().BeTrue();
        typeof(ILeptonCancellable).IsAssignableFrom(typeof(ILeptonCancellableIdentifiableContentElement)).Should().BeTrue();
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
    public void LeptonCancellableIdentifiableContentElement_supports_id_attributes_content_and_cancellation()
    {
        var component = new TestCancellableIdentifiableContentElement();
        component.Configure("primary", "base", "display:block", new Dictionary<string, object>
        {
            ["class"] = "additional",
            ["style"] = "color:red",
            ["role"] = "button"
        });

        _ = component.Token;

        IReadOnlyDictionary<string, object>? attributes = component.Effective;

        attributes.Should().NotBeNull();
        attributes!["id"].Should().Be("primary");
        attributes["class"].Should().Be("base additional");
        attributes["style"].Should().Be("display:block; color:red");
        attributes["role"].Should().Be("button");
        component.Should().BeAssignableTo<ILeptonCancellableIdentifiableContentElement>();

        component.DisposeAsync().AsTask().GetAwaiter().GetResult();

        component.CancellationRequested.Should().BeFalse();
    }

    [Test]
    public void LeptonDisposableContentElement_builds_element_attributes()
    {
        var component = new TestDisposableContentElement();
        component.Configure("base", "display:block", new Dictionary<string, object>
        {
            ["class"] = "additional",
            ["style"] = "color:red",
            ["role"] = "region"
        });

        IReadOnlyDictionary<string, object> attributes = component.Effective;

        attributes.Should().NotContainKey("id");
        attributes["class"].Should().Be("base additional");
        attributes["style"].Should().Be("display:block; color:red");
        attributes["role"].Should().Be("region");
        component.Should().BeAssignableTo<ILeptonDisposableContentElement>();
        component.Should().BeAssignableTo<ILeptonContentElement>();
    }

    [Test]
    public void LeptonDisposableIdentifiableContentElement_builds_id_aware_element_attributes()
    {
        var component = new TestDisposableIdentifiableContentElement();
        component.Configure("primary", "base", "display:block", new Dictionary<string, object>
        {
            ["class"] = "additional",
            ["style"] = "color:red",
            ["role"] = "region"
        });

        IReadOnlyDictionary<string, object> attributes = component.Effective;

        attributes["id"].Should().Be("primary");
        attributes["class"].Should().Be("base additional");
        attributes["style"].Should().Be("display:block; color:red");
        attributes["role"].Should().Be("region");
        component.Should().BeAssignableTo<ILeptonDisposableIdentifiableContentElement>();
        component.Should().BeAssignableTo<ILeptonIdentifiableContentElement>();
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
