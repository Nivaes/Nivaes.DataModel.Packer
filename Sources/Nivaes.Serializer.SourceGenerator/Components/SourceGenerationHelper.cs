using Microsoft.CodeAnalysis;

namespace Nivaes.Serializer.SourceGenerator;

internal static class SourceGenerationHelper
{
    private static string GetPrefixSerializerClassName(INamedTypeSymbol symbol)
    {
        var names = new List<string>();

        for (var current = symbol;
             current is not null;
             current = current.ContainingType)
        {
            names.Add(current.Name);
        }

        names.Reverse();

        return string.Join("_", names);
    }

    public static string GetSerializerClassName(this INamedTypeSymbol symbol)
    {
        return GetPrefixSerializerClassName(symbol) + "Serializer";
    }

    public static string GetDeserializerClassName(this INamedTypeSymbol symbol)
    {
        return GetPrefixSerializerClassName(symbol) + "Deserializer";
    }
}