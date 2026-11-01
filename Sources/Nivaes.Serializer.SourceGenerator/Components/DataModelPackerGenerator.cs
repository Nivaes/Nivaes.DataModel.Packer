using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Nivaes.DataModel.Packer.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class DataModelPackerGenerator : IIncrementalGenerator
{
    private sealed class InterceptInfo
    {
        public int Version { get; }
        public string LocationData { get; }
        public string ModelType { get; }
        public string SerializerType { get; }

        public InterceptInfo(
            int version,
            string locationData,
            string modelType,
            string serializerType)
        {
            Version = version;
            LocationData = locationData;
            ModelType = modelType;
            SerializerType = serializerType;
        }
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var calls = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) =>
            {
                if (node is not InvocationExpressionSyntax invocation)
                    return false;

                // DataModelPacker.Serialize<T>(...)
                if (invocation.Expression is MemberAccessExpressionSyntax member)
                {
                    return member.Name is GenericNameSyntax genericName
                        && genericName.Identifier.ValueText == "Serialize"
                        && genericName.TypeArgumentList.Arguments.Count == 1;
                }

                // Serialize<T>(...) sin calificador.
                return invocation.Expression is GenericNameSyntax directName
                    && directName.Identifier.ValueText == "Serialize"
                    && directName.TypeArgumentList.Arguments.Count == 1;
            },

            transform: static (ctx, ct) =>
            {
                var invocation = (InvocationExpressionSyntax)ctx.Node;

                if (ctx.SemanticModel.GetSymbolInfo(invocation, ct).Symbol
                    is not IMethodSymbol symbol)
                {
                    return null;
                }

                if (symbol.Name != "Serialize"
                    || symbol.Arity != 1
                    || symbol.ContainingType.ToDisplayString()
                        != "Nivaes.DataModel.Packer.DataModelPacker")
                {
                    return null;
                }

                var location = ctx.SemanticModel
                    .GetInterceptableLocation(invocation, ct);

                if (location is null)
                    return null;

                if (symbol.TypeArguments[0] is not INamedTypeSymbol model)
                    return null;

                var modelType = model.ToDisplayString(
                    SymbolDisplayFormat.FullyQualifiedFormat);

                var ns = model.ContainingNamespace.IsGlobalNamespace
                    ? "global::"
                    : "global::" + model.ContainingNamespace.ToDisplayString() + ".";

                var serializerType = /*ns + model.Name +*/ modelType + "Serializer";

                return new InterceptInfo(
                    location.Version,
                    location.Data,
                    modelType,
                    serializerType);
            })
            .Where(static x => x is not null)
            .Collect();

        context.RegisterSourceOutput(calls, static (spc, items) =>
        {
            var source = new StringBuilder();

            source.AppendLine(Header);
            source.AppendLine(InterceptorClassTemplate);

            source.AppendLine();
            source.AppendLine("namespace Nivaes.DataModel.Packer.SourceGenerator");
            source.AppendLine("{");
            source.AppendLine("    internal static class DataModelPackerInterceptors");
            source.AppendLine("    {");

            var index = 0;

            foreach (var item in items)
            {
                if (item is null)
                    continue;

                var locationData = SymbolDisplay.FormatLiteral(
                    item.LocationData, quote: true);

                source.AppendLine(
                    $"        [global::System.Runtime.CompilerServices.InterceptsLocation({item.Version}, {locationData})]");

                source.AppendLine(
                    $"        public static global::System.ReadOnlySpan<byte> Serialize_{index}(");

                source.AppendLine(
                    $"            in {item.ModelType} value)");

                source.AppendLine("        {");

                source.AppendLine(
                    $"            return global::Nivaes.DataModel.Packer.DataModelPacker.Serialize<{item.ModelType}, {item.SerializerType}>(in value);");

                source.AppendLine("        }");
                source.AppendLine();

                index++;
            }

            source.AppendLine("    }");
            source.AppendLine("}");

            spc.AddSource(
                "DataModelPackerInterceptors.g.cs",
                source.ToString());
        });


    }

    private const string Header =
       """
        //------------------------------------------------------------------------------
        // <auto-generated>
        //     This code was generated by the Nivaes.DataModel.Packer.SourceGenerator source generator
        //
        //     Changes to this file may cause incorrect behavior and will be lost if
        //     the code is regenerated.
        // </auto-generated>
        //------------------------------------------------------------------------------

        #nullable enable
        """;

    private const string InterceptorClassTemplate =
        """
        namespace System.Runtime.CompilerServices
        {
            // this type is needed by the compiler to implement interceptors - it doesn't need to
            // come from the runtime itself, though
        
            [global::System.Diagnostics.Conditional("DEBUG")] // not needed post-build, so: evaporate
            [global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = true)]
            sealed file class InterceptsLocationAttribute : global::System.Attribute
            {
                public InterceptsLocationAttribute(int version, string data)
                {
                    _ = version;
                    _ = data;
                }
            }
        }
        """;
}