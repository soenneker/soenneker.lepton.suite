using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;

namespace Soenneker.Lepton.Suite.Tests;

public sealed class LeptonRenderingAllocationTests
{
    [Test]
    public void Null_only_spans_do_not_allocate_dictionary_storage()
    {
        var component = new TestElement();

        component.NullSpanAttributes().EnsureCapacity(0).Should().Be(0);
        component.KeyValueAttributes([
            new("a", null), new("b", null), new("c", null), new("d", null)
        ]).EnsureCapacity(0).Should().Be(0);
        component.Attributes(("a", null), ("b", null), ("c", null), ("d", null)).EnsureCapacity(0).Should().Be(0);
    }

    [Test]
    public void Optional_attributes_only_reserve_storage_for_present_values()
    {
        var component = new TestElement();
        component.Configure("base", null, null);
        Dictionary<string, object> attributes = component.Attributes(("a", "value"), ("b", null), ("c", null), ("d", null));

        attributes.EnsureCapacity(0).Should().Be(new Dictionary<string, object>(2).EnsureCapacity(0));
        attributes.Should().HaveCount(2);
        attributes["a"].Should().Be("value");
        attributes["class"].Should().Be("base");
    }

    [Test]
    public void Attribute_builds_observe_in_place_changes_and_keep_previous_results_independent()
    {
        var additional = new Dictionary<string, object> { ["CLASS"] = "extra", ["role"] = "button" };
        var component = new TestElement();
        component.Configure("base", "display:block", additional);
        Dictionary<string, object> first = component.Attributes();

        additional["CLASS"] = "changed";
        additional.Remove("role");
        additional["STYLE"] = "color:red";
        Dictionary<string, object> second = component.Attributes();

        first["class"].Should().Be("base extra");
        first["role"].Should().Be("button");
        second["class"].Should().Be("base changed");
        second["style"].Should().Be("display:block; color:red");
        second.Should().NotContainKey("role");
    }

    [Test]
    public void Identifiable_overloads_preserve_identifier_precedence_key_casing_and_attribute_order()
    {
        var component = new TestIdentifiableElement();
        component.Configure("primary", "base", null, new Dictionary<string, object> { ["ID"] = "additional", ["role"] = "link" });

        Dictionary<string, object>[] results = [
            component.EmptyAttributes(), component.Attributes("id", "argument"), component.TupleAttributes(),
            component.TuplePairAttributes(), component.TupleManyAttributes(), component.ArrayAttributes(),
            component.KeyValueAttributes([new("data-state", "open"), new("data-optional", null)])
        ];

        foreach (Dictionary<string, object> result in results)
        {
            result["id"].Should().Be("primary");
            result["role"].Should().Be("link");
            result.Should().NotContainKey("data-optional");
        }

        results[0].Keys.Should().Equal("class", "ID", "role");
        results[1].Keys.Should().Equal("class", "id", "role");
    }

    [Test]
    public void Disposable_identifiable_attributes_preserve_additional_identifier_precedence()
    {
        var component = new TestDisposableIdentifiableContentElement();
        component.Configure("primary", null, null, new Dictionary<string, object> { ["ID"] = "additional" });

        component.Effective["id"].Should().Be("additional");
    }

    [Test]
    public void Concurrent_token_readers_share_one_cancellation_source()
    {
        var component = new TestCancellable();
        var tokens = new CancellationToken[32];
        Parallel.For(0, tokens.Length, i => tokens[i] = component.Token);

        tokens.All(token => token == tokens[0]).Should().BeTrue();
        component.DisposeAsync().GetAwaiter().GetResult();
        tokens.All(token => token.IsCancellationRequested).Should().BeTrue();
        component.Token.Should().Be(CancellationToken.None);
    }

    [Test]
    public void Token_creation_racing_disposal_never_recreates_or_reads_a_disposed_source()
    {
        Parallel.For(0, 256, _ =>
        {
            var component = new TestCancellable();
            CancellationToken token = default;
            Parallel.Invoke(() => token = component.Token, () => component.DisposeAsync().GetAwaiter().GetResult());

            if (token.CanBeCanceled)
                token.IsCancellationRequested.Should().BeTrue();

            component.Token.Should().Be(CancellationToken.None);
            component.Disposed.Should().BeTrue();
        });
    }

    [Test]
    public void Reentrant_disposal_cancels_each_registration_once_and_completes_base_disposal()
    {
        var component = new TestCancellable();
        CancellationToken token = component.Token;
        var calls = 0;
        using CancellationTokenRegistration registration = token.Register(() =>
        {
            calls++;
            component.DisposeAsync().GetAwaiter().GetResult();
            component.Token.Should().Be(CancellationToken.None);
        });

        component.DisposeAsync().GetAwaiter().GetResult();
        component.DisposeAsync().GetAwaiter().GetResult();

        calls.Should().Be(1);
        component.Disposed.Should().BeTrue();
        Action readWaitHandle = () => _ = token.WaitHandle;
        readWaitHandle.Should().Throw<ObjectDisposedException>();
    }
}
