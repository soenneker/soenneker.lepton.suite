using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Soenneker.Lepton.Suite.PackageSmoke;

if (RawStringPage.Sample != "await client.Enqueue(\"message\", new Message(\"Hello\"));")
    throw new InvalidOperationException("Razor raw string content changed.");

var component = new ParameterPage();
component.Bind(ParameterView.FromDictionary(new Dictionary<string, object?> { [nameof(ParameterPage.Text)] = "bound" }));
if (component.Text != "bound")
    throw new InvalidOperationException("Packaged parameter generator did not bind the component parameter.");

Console.WriteLine("Razor raw strings and generated parameter bindings passed.");
