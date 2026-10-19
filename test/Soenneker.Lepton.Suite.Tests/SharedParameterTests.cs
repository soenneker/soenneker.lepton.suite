using System;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Lepton.Suite.Tests;

public sealed class SharedParameterTests : BunitContext
{
    [Test]
    public void Inherited_opt_in_rejects_hidden_duplicate_parameters()
    {
        Action render = () => Render<HiddenParameterProbe>();
        render.Should().Throw<InvalidOperationException>().WithMessage("*Duplicate parameter*Text*");
    }

    [Test]
    public void Inherited_opt_in_supports_virtual_parameter_overrides()
    {
        var cut = Render<OverriddenParameterProbe>(p => p.Add(c => c.Text, "derived"));
        cut.Markup.Should().Contain("derived");
        cut.Instance.ParameterCount.Should().Be(1);
    }

    [Test]
    public async Task Explicit_opt_out_preserves_async_initialization_and_parameter_delivery()
    {
        var completion = new TaskCompletionSource();
        var cut = Render<FrameworkParameterProbe>(p => p.Add(c => c.Initialization, completion.Task));
        cut.Instance.ParameterCount.Should().Be(0);
        await cut.InvokeAsync(() => completion.SetResult());
        cut.WaitForAssertion(() => cut.Instance.ParameterCount.Should().Be(1));
    }

    [Test]
    public void Cascading_values_bind_and_same_named_direct_values_are_captured()
    {
        var cut = Render<CascadingValue<string>>(p => p.Add(c => c.Value, "cascade")
            .AddChildContent<ParameterLifecycleProbe>(child => child.AddUnmatched("Label", "attribute")));
        var probe = cut.FindComponent<ParameterLifecycleProbe>();
        probe.Instance.Label.Should().Be("cascade");
        probe.Find("div").GetAttribute("Label").Should().Be("attribute");
    }

    [Test]
    public async Task Async_initialization_renders_then_sets_parameters_once_complete()
    {
        var completion = new TaskCompletionSource();
        var cut = Render<ParameterLifecycleProbe>(p => p.Add(c => c.Initialization, completion.Task));
        cut.Instance.InitializationCount.Should().Be(1);
        cut.Instance.ParameterCount.Should().Be(0);
        await cut.InvokeAsync(() => completion.SetResult());
        cut.WaitForAssertion(() => cut.Instance.ParameterCount.Should().Be(1));
        cut.Render(p => p.Add(c => c.Text, "updated"));
        cut.Instance.InitializationCount.Should().Be(1);
        cut.Instance.ParameterCount.Should().Be(2);
        cut.Markup.Should().Contain("updated");
    }

    [Test]
    public async Task Async_parameter_completion_renders_updated_state()
    {
        var cut = Render<ParameterLifecycleProbe>();
        var completion = new TaskCompletionSource();
        cut.Render(p => p.Add(c => c.ParameterUpdate, completion.Task));
        cut.Instance.CompletedUpdates.Should().Be(1);
        await cut.InvokeAsync(() => completion.SetResult());
        cut.WaitForAssertion(() => cut.Markup.Should().Contain(":2"));
    }

    [Test]
    public void Canceled_initialization_still_delivers_parameters()
    {
        var cut = Render<ParameterLifecycleProbe>(p => p.Add(c => c.Initialization, Task.FromCanceled(new CancellationToken(true))));
        cut.Instance.ParameterCount.Should().Be(1);
        cut.Instance.CompletedUpdates.Should().Be(1);
    }

    [Test]
    public async Task Cancellation_after_initial_render_still_delivers_parameters()
    {
        var completion = new TaskCompletionSource();
        var cut = Render<ParameterLifecycleProbe>(p => p.Add(c => c.Initialization, completion.Task));
        await cut.InvokeAsync(() => completion.SetCanceled());
        cut.WaitForAssertion(() => cut.Instance.ParameterCount.Should().Be(1));
    }

    [Test]
    public void Faulted_initialization_propagates_to_the_renderer()
    {
        Action render = () => Render<ParameterLifecycleProbe>(p => p.Add(c => c.Initialization,
            Task.FromException(new InvalidOperationException("initialization failed"))));
        render.Should().Throw<InvalidOperationException>().WithMessage("initialization failed");
    }

    [Test]
    public async Task Canceled_parameter_update_does_not_issue_a_completion_render()
    {
        var cut = Render<ParameterLifecycleProbe>();
        var completion = new TaskCompletionSource();
        cut.Render(p => p.Add(c => c.ParameterUpdate, completion.Task));
        int renders = cut.RenderCount;
        await cut.InvokeAsync(() => completion.SetCanceled());
        cut.RenderCount.Should().Be(renders);
        cut.Instance.CompletedUpdates.Should().Be(1);
    }
}
