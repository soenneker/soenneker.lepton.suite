using Microsoft.AspNetCore.Components;

namespace Soenneker.Lepton.Suite;

internal static class SharedParameterMetadata
{
    public static void Apply(LeptonComponent component, ParameterView parameters)
    {
        component.ValidateGeneratedParameters();
        SharedParameterProperty? capture = component.CaptureParameter;
        Dictionary<string, object>? unmatched = null;
        bool explicitCapture = false;
        foreach (ParameterValue parameter in parameters)
        {
            SharedParameterProperty? property = component.FindParameter(parameter.Name);
            if (property is not null && (parameter.Cascading ? property.Cascading : property.Direct))
            {
                explicitCapture |= property.Capture;
                if (parameter.Cascading && property.Direct && HasDirectParameter(parameters, parameter.Name)) continue;
                try { property.Setter.Invoke(component, parameter.Value); }
                catch (Exception error)
                {
                    throw new InvalidOperationException($"Unable to set property '{parameter.Name}' on '{component.GetType().FullName}'.", error);
                }
                continue;
            }
            if (parameter.Cascading || capture is null)
                throw new InvalidOperationException($"Cannot supply parameter '{parameter.Name}' to '{component.GetType().FullName}'.");
            (unmatched ??= new(StringComparer.OrdinalIgnoreCase))[parameter.Name] = parameter.Value!;
        }
        if (unmatched is null) return;
        if (explicitCapture)
            throw new InvalidOperationException($"Cannot set '{capture!.Name}' explicitly together with unmatched parameters.");
        capture!.Setter.Invoke(component, unmatched);
    }

    private static bool HasDirectParameter(ParameterView parameters, string name)
    {
        foreach (ParameterValue parameter in parameters)
            if (!parameter.Cascading && string.Equals(parameter.Name, name, StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }
}
