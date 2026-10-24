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
            try
            {
                if (component.TryApplyParameter(parameter, parameters, out bool captures))
                {
                    explicitCapture |= captures;
                    continue;
                }
            }
            catch (Exception error)
            {
                throw new InvalidOperationException($"Unable to set property '{parameter.Name}' on '{component.GetType().FullName}'.", error);
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

}
