using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.Benchmarks;

// Run each cold sample in a separate process; framework metadata and generated statics are process-wide.
if (args.Length is < 2 or > 3 || args[0] is not ("generated" or "framework") || args[1] is not ("cold" or "warm") ||
    (args.Length == 3 && args[2] is not ("small" or "wide")))
    throw new ArgumentException("Usage: <generated|framework> <cold|warm> [small|wide]");

bool generated = args[0] == "generated";
bool cold = args[1] == "cold";
bool wide = args.Length == 3 && args[2] == "wide";
var values = new Dictionary<string, object?>
{
    [nameof(ParameterBindingProbe.Text)] = "message",
    [nameof(ParameterBindingProbe.Count)] = 42,
    [nameof(ParameterBindingProbe.Enabled)] = true,
    [nameof(ParameterBindingProbe.Changed)] = default(EventCallback)
};
if (wide)
    for (int i = 0; i < 64; i++) values[$"Value{i:D2}"] = i;
ParameterView parameters = ParameterView.FromDictionary(values);

ParameterBindingProbe? component = null;
if (!cold)
{
    component = wide ? new WideParameterBindingProbe() : new ParameterBindingProbe();
    for (int i = 0; i < 100_000; i++)
    {
        if (generated) component.BindGenerated(parameters);
        else component.BindFramework(parameters);
    }
    // Allow tiered compilation to finish before measuring steady-state work.
    Thread.Sleep(250);
    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();
}

int iterations = cold ? 1 : 1_000_000;
long allocated = GC.GetAllocatedBytesForCurrentThread();
long started = Stopwatch.GetTimestamp();
component ??= wide ? new WideParameterBindingProbe() : new ParameterBindingProbe();
for (int i = 0; i < iterations; i++)
{
    if (generated) component.BindGenerated(parameters);
    else component.BindFramework(parameters);
}
TimeSpan elapsed = Stopwatch.GetElapsedTime(started);
allocated = GC.GetAllocatedBytesForCurrentThread() - allocated;

if (component.Text != "message" || component.Count != 42 || !component.Enabled)
    throw new InvalidOperationException("Parameter binding produced an incorrect result.");
if (component is WideParameterBindingProbe wideComponent && wideComponent.Value63 != 63)
    throw new InvalidOperationException("Wide parameter binding produced an incorrect result.");

Console.WriteLine(JsonSerializer.Serialize(new
{
    mode = args[0], phase = args[1], parameters = values.Count, iterations,
    nanosecondsPerBinding = elapsed.TotalNanoseconds / iterations,
    bytesPerBinding = (double)allocated / iterations,
    runtime = Environment.Version.ToString()
}));
