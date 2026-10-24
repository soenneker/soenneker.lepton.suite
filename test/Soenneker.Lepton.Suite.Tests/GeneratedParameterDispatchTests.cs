using System;
using System.Collections.Generic;
using AwesomeAssertions;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Lepton.Suite.Tests;

public sealed class GeneratedParameterDispatchTests
{
    [Test]
    public void Typed_dispatch_preserves_case_null_omitted_inherited_and_unmatched_values()
    {
        var component = new ParameterDispatchProbe();
        component.Bind(ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            ["tExT"] = "value", ["Count"] = 4, ["état"] = "active", ["Class"] = "inherited",
            ["Query"] = "query", ["Context"] = "attribute", ["data-test"] = "captured"
        }));
        component.Bind(ParameterView.FromDictionary(new Dictionary<string, object?> { ["Count"] = null }));

        component.Text.Should().Be("value");
        component.Count.Should().Be(0);
        component.État.Should().Be("active");
        component.Class.Should().Be("inherited");
        component.Query.Should().Be("query");
        component.Context.Should().BeNull();
        component.AdditionalAttributes!["Context"].Should().Be("attribute");
        component.AdditionalAttributes["data-test"].Should().Be("captured");
    }

    [Test]
    public void Large_derived_dispatch_uses_overridden_metadata_and_inherited_setters()
    {
        var component = new LargeParameterDispatchProbe();
        component.Bind(ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            ["Query"] = "attribute", ["Text"] = "inherited", ["value8"] = 8
        }));
        component.Query.Should().BeNull();
        component.Text.Should().Be("inherited");
        component.Value8.Should().Be(8);
        component.AdditionalAttributes!["Query"].Should().Be("attribute");
    }

    [Test]
    public void Typed_dispatch_wraps_conversion_and_setter_failures_with_parameter_context()
    {
        var component = new ParameterDispatchProbe();
        Action wrongType = () => component.Bind(ParameterView.FromDictionary(new Dictionary<string, object?> { ["Count"] = "invalid" }));
        wrongType.Should().Throw<InvalidOperationException>().WithMessage("*Count*").WithInnerException<InvalidCastException>();
        Action setter = () => component.Bind(ParameterView.FromDictionary(new Dictionary<string, object?> { ["Failing"] = "value" }));
        setter.Should().Throw<InvalidOperationException>().WithMessage("*Failing*").WithInnerException<ArgumentException>();
    }

    [Test]
    public void Explicit_capture_cannot_be_combined_with_unmatched_parameters()
    {
        var component = new ParameterDispatchProbe();
        Action bind = () => component.Bind(ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            ["AdditionalAttributes"] = new Dictionary<string, object>(), ["data-test"] = "value"
        }));
        bind.Should().Throw<InvalidOperationException>().WithMessage("*explicitly together with unmatched*");
    }
}
