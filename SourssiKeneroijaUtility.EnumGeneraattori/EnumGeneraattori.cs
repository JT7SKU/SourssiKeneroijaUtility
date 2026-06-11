using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace SourssiKeneroijaUtility.EnumGeneraattori
{
    [Generator]
    public class EnumGeneraattori : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext konteksti)
        {
            // Add the marker attribute to the compilation
            konteksti.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                "EnumExtensionsAttribute.g.cs",
                SourceText.From(SourssiGeneroijaApulainen.Attribuutti, Encoding.UTF8)));

            // Do a simple filter for enums
            IncrementalValuesProvider<Enum2Generoi?> enums2Generoi = konteksti.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxTargetForGeneration(s), // select enums with attributes
                    transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx)) // select enums with the [EnumExtensions] attribute and extract details
                .Where(static m => m is not null); // Filter out errors that we don't care about

            //If you're targeting the .NET 7 SDK, use this version instead:
            //IncrementalValuesProvider<Enum2Generoi?> enumsToGenerate = konteksti.SyntaxProvider
            //    .ForAttributeWithMetadataName(
            //        "SourssiKeneroijaUtility.EnumGeneraattori.EnumAttribuutti",
            //        predicate: static (s, _) => true,
            //        transform: static (ctx, _) => HaeEnum2Generoi(ctx.SemanticModel, ctx.TargetNode))
            //    .Where(static m => m is not null);

            static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    => node is EnumDeclarationSyntax m && m.AttributeLists.Count > 0;

            // Generate source code for each enum found
            konteksti.RegisterSourceOutput(enums2Generoi,
                static (spc, sourssi) => Execute(sourssi, spc));
        }
        static void Execute(Enum2Generoi? enumToGenerate, SourceProductionContext konteksti)
        {
            if (enumToGenerate is { } value)
            {
                // generate the source code and add it to the output
                string result = SourssiGeneroijaApulainen.GeneroiLaajennusLuokka(value);
                // Create a separate partial class file for each enum
                konteksti.AddSource($"EnumLaajennukset.{value.Nimi}.g.cs", SourceText.From(result, Encoding.UTF8));
            }
        }

        static Enum2Generoi? GetSemanticTargetForGeneration(GeneratorSyntaxContext kontexti)
        {
            // we know the node is a EnumDeclarationSyntax thanks to IsSyntaxTargetForGeneration
            var enumDeclarationSyntax = (EnumDeclarationSyntax)kontexti.Node;

            // loop through all the attributes on the method
            foreach (AttributeListSyntax attribuuttiListaSyntaksi in enumDeclarationSyntax.AttributeLists)
            {
                foreach (AttributeSyntax attribuuttiSyntaksi in attribuuttiListaSyntaksi.Attributes)
                {
                    if (kontexti.SemanticModel.GetSymbolInfo(attribuuttiSyntaksi).Symbol is not IMethodSymbol attribuuttiSymboli)
                    {
                        // weird, we couldn't get the symbol, ignore it
                        continue;
                    }

                    INamedTypeSymbol attributeContainingTypeSymboli = attribuuttiSymboli.ContainingType;
                    string kokoNimi = attributeContainingTypeSymboli.ToDisplayString();

                    // Is the attribute the [EnumExtensions] attribute?
                    if (kokoNimi == "SourssiKeneroijaUtility.EnumGeneraattorit.EnumLaajennuksetAttribuutti")
                    {
                        // return the enum. Implementation shown in section 7.
                        return HaeEnum2Generoi(kontexti.SemanticModel, enumDeclarationSyntax);
                    }
                }
            }

            // we didn't find the attribute we were looking for
            return null;
        }
        static Enum2Generoi? HaeEnum2Generoi(SemanticModel semanttinenMalli, SyntaxNode enumKuvausSyntaksi)
        {
            // Get the semantic representation of the enum syntax
            if (semanttinenMalli.GetDeclaredSymbol(enumKuvausSyntaksi) is not INamedTypeSymbol enumSymboli)
            {
                // something went wrong
                return null;
            }

            // Get the full type name of the enum e.g. Colour, 
            // or OuterClass<T>.Colour if it was nested in a generic type (for example)
            string enumNimi = enumSymboli.ToString();

            // Get all the members in the enum
            ImmutableArray<ISymbol> enumJasenet = enumSymboli.GetMembers();
            var jasenet = new List<string>(enumJasenet.Length);

            // Get all the fields from the enum, and add their name to the list
            foreach (ISymbol jasen in enumJasenet)
            {
                if (jasen is IFieldSymbol field && field.ConstantValue is not null)
                {
                    jasenet.Add(jasen.Name);
                }
            }

            return new Enum2Generoi(enumNimi, jasenet);
        }
    }
}
