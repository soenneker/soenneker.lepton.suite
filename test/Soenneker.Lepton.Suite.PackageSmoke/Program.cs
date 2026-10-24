using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.PackageSmoke;
using Soenneker.Lepton.Suite.PackageSmoke.Dependency;
using Soenneker.Lepton.Suite.PackageSmoke.TransitiveDependency;

if (SmokeDependency.Message != "project reference")
    throw new InvalidOperationException("Project reference resolution failed.");

if (TransitiveSmokeDependency.Message != "transitive project reference")
    throw new InvalidOperationException("Transitive project reference resolution failed.");

if (RawStringPage.Sample != "await client.Enqueue(\"message\", new Message(\"Hello\"));")
    throw new InvalidOperationException("Razor raw string content changed.");

var component = new ParameterPage();
component.Bind(ParameterView.FromDictionary(new Dictionary<string, object?> { [nameof(ParameterPage.Text)] = "bound" }));
if (component.Text != "bound")
    throw new InvalidOperationException("Packaged parameter generator did not bind the component parameter.");

var inline = new InlineParameterPage<int>();
inline.Bind(ParameterView.FromDictionary(new Dictionary<string, object?>
{
    ["vAlUe"] = 42, ["Count"] = 7, ["Class"] = "inline", ["data-test"] = "captured"
}));
if (inline.Value != 42 || inline.Count != 7 || inline.Class != "inline" ||
    !Equals(inline.AdditionalAttributes?["data-test"], "captured"))
    throw new InvalidOperationException("Inline Razor generic or inherited parameter binding failed.");

inline.Bind(ParameterView.FromDictionary(new Dictionary<string, object?> { ["Count"] = null }));
if (inline.Count != 0 || inline.Value != 42)
    throw new InvalidOperationException("Inline Razor null or omitted parameter binding failed.");

var mixed = new MixedParameterPage();
mixed.Bind(ParameterView.FromDictionary(new Dictionary<string, object?>
{
    [nameof(MixedParameterPage.CodeBehindText)] = "code-behind", [nameof(MixedParameterPage.InlineText)] = "inline"
}));
if (mixed.CodeBehindText != "code-behind" || mixed.InlineText != "inline")
    throw new InvalidOperationException("Mixed Razor and code-behind parameter binding failed.");

var template = new TemplateParameterPage();
template.Bind(ParameterView.FromDictionary(new Dictionary<string, object?> { ["Text"] = "template" }));
if (template.Text != "template")
    throw new InvalidOperationException("Razor template parameter binding failed.");

Console.WriteLine("Razor raw strings and generated bindings passed: code-behind, inline, mixed, template, generic, inherited, capture, null, omitted, case-insensitive.");
