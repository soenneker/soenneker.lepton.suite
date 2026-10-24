using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Abstract;

namespace Soenneker.Lepton.Suite;

public abstract partial class LeptonComponent : ComponentBase, ILeptonComponent
{
    private Action? _requestRender;
    private bool _parametersInitialized;

    /// <summary>Opts this component into generated parameter metadata and typed setters. Defaults to false.</summary>
    /// <remarks>Override with a constant value. Derived components inherit the choice and can explicitly opt out.</remarks>
    protected virtual bool UseSharedParameterMetadata => false;

    /// <summary>The exact component type covered by the generated binder.</summary>
    protected virtual Type GeneratedParameterType => typeof(LeptonComponent);

    /// <summary>Finds a compile-time generated parameter descriptor, including inherited parameters.</summary>
    protected virtual SharedParameterProperty? FindGeneratedParameter(string name) => null;

    /// <summary>The generated capture-unmatched parameter, if present.</summary>
    protected virtual SharedParameterProperty? GeneratedCaptureParameter => null;

    /// <summary>Applies a parameter using generated typed assignments and reports capture-unmatched assignments.</summary>
    protected virtual bool TryApplyGeneratedParameter(in ParameterValue parameter, in ParameterView parameters, out bool capture) =>
        TryApplyGeneratedParameterDescriptor(parameter, parameters, out capture);

    /// <summary>Applies a parameter through its generated descriptor, including inherited metadata.</summary>
    protected bool TryApplyGeneratedParameterDescriptor(in ParameterValue parameter, in ParameterView parameters, out bool capture)
    {
        SharedParameterProperty? property = FindGeneratedParameter(parameter.Name);
        capture = false;
        if (property is null || !(parameter.Cascading ? property.Cascading : property.Direct))
            return false;

        capture = property.Capture;
        if (!parameter.Cascading || !property.Direct || !HasDirectParameter(parameters, parameter.Name))
            property.Setter(this, parameter.Value);
        return true;
    }

    /// <summary>Checks whether a direct parameter overrides a cascading value with the same name.</summary>
    protected static bool HasDirectParameter(ParameterView parameters, string name)
    {
        foreach (ParameterValue parameter in parameters)
            if (!parameter.Cascading && string.Equals(parameter.Name, name, StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }

    /// <summary>Assigns parameters using generated setters without invoking render lifecycle callbacks.</summary>
    protected void ApplyGeneratedParameters(ParameterView parameters) => SharedParameterMetadata.Apply(this, parameters);

    internal SharedParameterProperty? CaptureParameter => GeneratedCaptureParameter;

    internal bool TryApplyParameter(in ParameterValue parameter, in ParameterView parameters, out bool capture) =>
        TryApplyGeneratedParameter(parameter, parameters, out capture);

    internal void ValidateGeneratedParameters()
    {
        // Type identity only: no member discovery, assembly inspection, or dynamic code.
        if (GetType() != GeneratedParameterType)
            throw new InvalidOperationException("This component requires generated parameter bindings. Enable the Lepton parameter generator and declare the component partial.");
    }
    protected Task RequestRender() => InvokeAsync(_requestRender ??= StateHasChanged);

    public override Task SetParametersAsync(ParameterView parameters)
    {
        if (!UseSharedParameterMetadata)
            return base.SetParametersAsync(parameters);

        ApplyGeneratedParameters(parameters);
        if (_parametersInitialized)
            return NotifyParametersAsync();

        _parametersInitialized = true;
        return InitializeParametersAsync();
    }

    private async Task InitializeParametersAsync()
    {
        OnInitialized();
        Task initialization = OnInitializedAsync();
        if (!initialization.IsCompletedSuccessfully && !initialization.IsCanceled)
        {
            if (!initialization.IsFaulted)
                StateHasChanged();
            try { await initialization; }
            catch { if (!initialization.IsCanceled) throw; }
        }
        await NotifyParametersAsync();
    }

    private Task NotifyParametersAsync()
    {
        OnParametersSet();
        Task update = OnParametersSetAsync();
        if (!update.IsFaulted)
            StateHasChanged();
        return update.IsCompletedSuccessfully || update.IsCanceled ? Task.CompletedTask : FinishParametersAsync(update);
    }

    private async Task FinishParametersAsync(Task update)
    {
        try { await update; }
        catch
        {
            if (update.IsCanceled) return;
            throw;
        }
        StateHasChanged();
    }
}
