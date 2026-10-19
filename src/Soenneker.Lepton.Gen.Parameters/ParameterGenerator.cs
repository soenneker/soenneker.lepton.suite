using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Soenneker.Lepton.Gen.Parameters;

[Generator]
public sealed class ParameterGenerator : IIncrementalGenerator
{
    private const string Root = "Soenneker.Lepton.Suite.LeptonComponent";
    private const string Descriptor = "global::Soenneker.Lepton.Suite.SharedParameterProperty";
    private static readonly SymbolDisplayFormat QualifiedType = SymbolDisplayFormat.FullyQualifiedFormat.WithMiscellaneousOptions(
        SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
    private static readonly DiagnosticDescriptor PartialRequired = new("LPT001", "Partial component required",
        "Component '{0}' must be partial to generate AOT-safe parameter bindings", "Lepton", DiagnosticSeverity.Error, true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var candidates = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, _) => node is ClassDeclarationSyntax,
            static (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)ctx.Node) as INamedTypeSymbol);
        context.RegisterSourceOutput(candidates.Collect().Combine(context.CompilationProvider), static (ctx, input) =>
        {
            var seen = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
            foreach (INamedTypeSymbol? type in input.Left)
            {
                if (type is null || !seen.Add(type) || !IsComponent(type)) continue;
                Emit(ctx, type, input.Right);
            }
        });
    }

    private static bool IsComponent(INamedTypeSymbol type)
    {
        for (INamedTypeSymbol? current = type.BaseType; current is not null; current = current.BaseType)
            if (current.ToDisplayString() == Root) return true;
        return false;
    }

    private static AttributeData? Attribute(IPropertySymbol property, string name)
    {
        for (IPropertySymbol? current = property; current is not null; current = current.OverriddenProperty)
            foreach (AttributeData attribute in current.GetAttributes())
                for (INamedTypeSymbol? type = attribute.AttributeClass; type is not null; type = type.BaseType)
                    if (type.ToDisplayString() == name) return attribute;
        return null;
    }

    private static string TypeName(INamedTypeSymbol type) => "@" + type.Name +
        (type.TypeParameters.Length == 0 ? "" : "<" + string.Join(", ", type.TypeParameters.Select(p => "@" + p.Name)) + ">");

    private static void Emit(SourceProductionContext context, INamedTypeSymbol type, Compilation compilation)
    {
        var containers = new Stack<INamedTypeSymbol>();
        for (INamedTypeSymbol? current = type; current is not null; current = current.ContainingType)
        {
            if (current.DeclaringSyntaxReferences.Any(r => r.GetSyntax() is ClassDeclarationSyntax c && !c.Modifiers.Any(SyntaxKind.PartialKeyword)))
            {
                if (RequiresBindings(type))
                    context.ReportDiagnostic(Diagnostic.Create(PartialRequired, current.Locations.FirstOrDefault(), current.Name));
                return;
            }
            containers.Push(current);
        }

        var properties = new List<IPropertySymbol>();
        var names = new Dictionary<string, IPropertySymbol>(StringComparer.OrdinalIgnoreCase);
        string? invalid = null;
        string? captureName = null;
        for (INamedTypeSymbol? current = type; current is not null && current.ToDisplayString() != Root; current = current.BaseType)
        {
            foreach (IPropertySymbol property in current.GetMembers().OfType<IPropertySymbol>())
            {
                AttributeData? direct = Attribute(property, "Microsoft.AspNetCore.Components.ParameterAttribute");
                AttributeData? cascade = Attribute(property, "Microsoft.AspNetCore.Components.CascadingParameterAttributeBase");
                if (direct is null && cascade is null) continue;
                if (names.TryGetValue(property.Name, out IPropertySymbol? derived))
                {
                    IPropertySymbol? overridden = derived.OverriddenProperty;
                    while (overridden is not null && !SymbolEqualityComparer.Default.Equals(overridden, property)) overridden = overridden.OverriddenProperty;
                    if (overridden is null) invalid = "Duplicate parameter '" + property.Name + "'. Parameter names are case-insensitive.";
                    continue;
                }
                names.Add(property.Name, property);
                if (property.IsStatic || property.IsIndexer || property.SetMethod is null || property.SetMethod.IsInitOnly ||
                    (direct is not null && property.SetMethod.DeclaredAccessibility != Accessibility.Public))
                    invalid = "Parameter '" + property.Name + "' requires an accessible setter.";
                bool capture = direct?.NamedArguments.Any(a => a.Key == "CaptureUnmatchedValues" && a.Value.Value is true) == true;
                if (capture)
                {
                    if (captureName is not null) invalid = "Multiple capture-unmatched parameters.";
                    captureName = property.Name;
                    INamedTypeSymbol? dictionary = compilation.GetTypeByMetadataName("System.Collections.Generic.Dictionary`2")?.Construct(
                        compilation.GetSpecialType(SpecialType.System_String), compilation.GetSpecialType(SpecialType.System_Object));
                    if (dictionary is not null)
                    {
                        Conversion conversion = ((CSharpCompilation)compilation).ClassifyConversion(dictionary, property.Type);
                        if (!conversion.IsIdentity && !(conversion.IsImplicit && conversion.IsReference))
                            invalid = "Invalid capture-unmatched parameter type for '" + property.Name + "'.";
                    }
                }
                if (SymbolEqualityComparer.Default.Equals(current, type)) properties.Add(property);
                else if (current.DeclaringSyntaxReferences.Length == 0
                    ? !current.GetMembers("FindGeneratedParameter").Any()
                    : current.DeclaringSyntaxReferences.Any(r => r.GetSyntax() is ClassDeclarationSyntax c && !c.Modifiers.Any(SyntaxKind.PartialKeyword)))
                    invalid = "Base component '" + current.Name + "' declares parameters without generated bindings.";
            }
        }

        var source = new StringBuilder("// <auto-generated/>\n#nullable enable\n#pragma warning disable BL0005\n");
        if (!type.ContainingNamespace.IsGlobalNamespace) source.Append("namespace ").Append(type.ContainingNamespace.ToDisplayString()).AppendLine(" {");
        foreach (INamedTypeSymbol container in containers) source.Append("partial class ").Append(TypeName(container)).AppendLine(" {");
        source.Append("protected override global::System.Type GeneratedParameterType => ");
        if (invalid is not null)
        {
            source.Append("throw new global::System.InvalidOperationException(").Append(SymbolDisplay.FormatLiteral(invalid, true)).AppendLine(");");
        }
        else
        {
            source.Append("typeof(").Append(type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)).AppendLine(");");
            if (properties.Count != 0)
            {
                source.Append("private static readonly global::System.Collections.Generic.Dictionary<string, ").Append(Descriptor)
                    .AppendLine("> __leptonParameters = new(global::System.StringComparer.OrdinalIgnoreCase) {");
                foreach (IPropertySymbol property in properties)
                {
                    AttributeData? direct = Attribute(property, "Microsoft.AspNetCore.Components.ParameterAttribute");
                    AttributeData? cascade = Attribute(property, "Microsoft.AspNetCore.Components.CascadingParameterAttributeBase");
                    bool acceptsDirect = direct is not null && (cascade is null || cascade.AttributeClass?.ToDisplayString() == "Microsoft.AspNetCore.Components.SupplyParameterFromQueryAttribute");
                    bool capture = direct?.NamedArguments.Any(a => a.Key == "CaptureUnmatchedValues" && a.Value.Value is true) == true;
                    string name = SymbolDisplay.FormatLiteral(property.Name, true);
                    source.Append('[').Append(name).Append("] = new(").Append(name).Append(", static (component, value) => ((")
                        .Append(type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)).Append(")component).@")
                        .Append(property.Name).Append(" = value is null ? default! : (")
                        .Append(property.Type.ToDisplayString(QualifiedType)).Append(")value, ")
                        .Append(cascade is not null ? "true" : "false").Append(", ").Append(acceptsDirect ? "true" : "false").Append(", ")
                        .Append(capture ? "true" : "false").AppendLine("),");
                }
                source.AppendLine("};");
                source.Append("protected override ").Append(Descriptor).AppendLine("? FindGeneratedParameter(string name) => __leptonParameters.TryGetValue(name, out var parameter) ? parameter : base.FindGeneratedParameter(name);");
                if (captureName is not null && properties.Any(p => p.Name == captureName))
                    source.Append("protected override ").Append(Descriptor).Append("? GeneratedCaptureParameter => __leptonParameters[")
                        .Append(SymbolDisplay.FormatLiteral(captureName, true)).AppendLine("];");
            }
        }
        foreach (INamedTypeSymbol _ in containers) source.AppendLine("}");
        if (!type.ContainingNamespace.IsGlobalNamespace) source.AppendLine("}");
        string hint = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        ulong hash = 14695981039346656037UL;
        foreach (char c in hint) hash = unchecked((hash ^ c) * 1099511628211UL);
        context.AddSource(type.Name + "_" + hash.ToString("x16") + ".Parameters.g.cs", source.ToString());
    }

    private static bool RequiresBindings(INamedTypeSymbol type)
    {
        for (INamedTypeSymbol? current = type; current is not null && current.ToDisplayString() != Root; current = current.BaseType)
        {
            var optIn = current.GetMembers("UseSharedParameterMetadata").OfType<IPropertySymbol>().FirstOrDefault();
            if (optIn is null) continue;
            return !optIn.DeclaringSyntaxReferences.Any(r => r.GetSyntax() is PropertyDeclarationSyntax p &&
                p.ExpressionBody?.Expression.IsKind(SyntaxKind.FalseLiteralExpression) == true);
        }
        return false;
    }
}
