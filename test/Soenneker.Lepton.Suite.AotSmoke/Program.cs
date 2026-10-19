using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.AotSmoke;
using System.Runtime.CompilerServices;

if (RuntimeFeature.IsDynamicCodeSupported)
    throw new InvalidOperationException("Run the published NativeAOT executable, not dotnet run.");

var number = new AotParameterProbe<int>();
var callback = EventCallback.Factory.Create<int>(number, static (int _) => { });
RenderFragment<int> template = value => builder => builder.AddContent(0, value);
number.Bind(ParameterView.FromDictionary(new Dictionary<string, object?>
{
    ["vAlUe"] = 42, ["Id"] = "native", ["Count"] = 7,
    ["Changed"] = callback, ["Template"] = template, ["data-test"] = "captured"
}));
if (number.Value != 42 || number.Count != 7 || number.Id != "native" ||
    !number.Changed.Equals(callback) || number.Template != template ||
    !Equals(number.AdditionalAttributes?["data-test"], "captured"))
    throw new InvalidOperationException("Generated binding failed.");

number.Bind(ParameterView.FromDictionary(new Dictionary<string, object?> { ["Count"] = null }));
if (number.Count != 0 || number.Value != 42) throw new InvalidOperationException("Null/default or omitted parameter handling failed.");

var text = new AotParameterProbe<string>();
text.Bind(ParameterView.FromDictionary(new Dictionary<string, object?> { ["Value"] = "AOT" }));
if (text.Value != "AOT") throw new InvalidOperationException("Reference-type generic binding failed.");

try
{
    number.Bind(ParameterView.FromDictionary(new Dictionary<string, object?> { ["Value"] = "wrong type" }));
    throw new Exception("Incorrect parameter type was accepted.");
}
catch (InvalidOperationException error) when (error.InnerException is InvalidCastException) { }

Console.WriteLine("NativeAOT generated bindings passed: inherited, generic, callback, fragment, capture, case-insensitive, null, omitted, wrong type.");
