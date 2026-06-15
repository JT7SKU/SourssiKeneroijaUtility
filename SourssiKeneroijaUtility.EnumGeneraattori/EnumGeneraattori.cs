using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace SourssiKeneroijaUtility.EnumGeneraattorit
{
    [Generator]
    public class EnumGeneraattori : IIncrementalGenerator
    {
        private const string EnumLaajennusAttribuutti = "SourssiKeneroijaUtility.EnumGeneraattorit.EnumLaajennusAttribuutti";
        public void Initialize(IncrementalGeneratorInitializationContext konteksti)
        {

            // Add the marker attribute to the compilation
            konteksti.RegisterPostInitializationOutput(ktx => ktx.AddSource(
                "EnumExtensionsAttribute.g.cs",
                SourceText.From(SourssiGeneroijaApulainen.Attribuutti, Encoding.UTF8)));

            // Do a simple filter for enums
            IncrementalValuesProvider<EnumDeclarationSyntax?> enumKuvaukset = konteksti.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => OnkoSyntaksiKohdeValmisGenerointiin(s), // select enums with attributes
                    transform: static (ktx, _) => HaeSemanttinenKohdeValmisGenerointiin(ktx)) // select enums with the [EnumExtensions] attribute and extract details
                .Where(static m => m is not null); // Filter out errors that we don't care about

            //If you're targeting the .NET 7 SDK, use this version instead:
            //IncrementalValuesProvider<Enum2Generoi?> enumsToGenerate = konteksti.SyntaxProvider
            //    .ForAttributeWithMetadataName(
            //        "SourssiKeneroijaUtility.EnumGeneraattori.EnumAttribuutti",
            //        predicate: static (s, _) => true,
            //        transform: static (ctx, _) => HaeEnum2Generoi(ctx.SemanticModel, ctx.TargetNode))
            //    .Where(static m => m is not null);

            IncrementalValueProvider<(Compilation, ImmutableArray<EnumDeclarationSyntax>)> kompilationJaEnums
            = konteksti.CompilationProvider.Combine(enumKuvaukset.Collect());


            // Generate source code for each enum found
            konteksti.RegisterSourceOutput(kompilationJaEnums,
                static (spc, sourssi) => Execute(sourssi.Item1,sourssi.Item2, spc));
        }
        static bool OnkoSyntaksiKohdeValmisGenerointiin(SyntaxNode solmu)
    => solmu is EnumDeclarationSyntax m && m.AttributeLists.Count > 0;

        static EnumDeclarationSyntax? HaeSemanttinenKohdeValmisGenerointiin(GeneratorSyntaxContext kontexti)
        {
            var enumKuvausSyntaksi = (EnumDeclarationSyntax)kontexti.Node;
            foreach (AttributeListSyntax attribuuttiListaSyntaksi in enumKuvausSyntaksi.AttributeLists)
            {
                foreach (AttributeSyntax attribuuttiSyntaksi in attribuuttiListaSyntaksi.Attributes)
                {
                    if (kontexti.SemanticModel.GetSymbolInfo(attribuuttiSyntaksi).Symbol is not IMethodSymbol attribuuttiSymboli)
                    {
                        continue;
                    }
                    INamedTypeSymbol attribuuttikonteiningTyyppiSympoli = attribuuttiSymboli.ContainingType;
                    string kokoNimi = attribuuttiSymboli.ToDisplayString();
                    if (kokoNimi == EnumLaajennusAttribuutti)
                    {
                        return enumKuvausSyntaksi;
                    }
                }
            }
            return null;
        }
        static void Execute(Compilation? kompilation,ImmutableArray<EnumDeclarationSyntax> enums, SourceProductionContext konteksti)
        {
            if (enums.IsDefaultOrEmpty)
            {
                return;
            }
            // I'm not sure if this is actually necessary, but `[LoggerMessage]` does it, so seems like a good idea!
            IEnumerable<EnumDeclarationSyntax> kaukaisetEnumit = enums.Distinct();
            List<Enum2Generoi> enums2Generoi = HaeTyypit2Generoi(kompilation, kaukaisetEnumit, konteksti.CancellationToken);

            if (enums2Generoi.Count >0)
            {
                string tulos = SourssiGeneroijaApulainen.GeneroiLaajennusLuokka(enums2Generoi);
                konteksti.AddSource("enumlaajennokset.g.cs", SourceText.From(tulos, Encoding.UTF8));
            }

        }

        static List<Enum2Generoi>? HaeTyypit2Generoi(Compilation? kompilation, IEnumerable<EnumDeclarationSyntax> enums, CancellationToken kt)
        {
            var enums2keneroi = new List<Enum2Generoi>();
            INamedTypeSymbol? enumAttribuutti = kompilation.GetTypeByMetadataName(EnumLaajennusAttribuutti);
            if (enumAttribuutti == null)
            {
                return enums2keneroi;
            }

            // loop through all the attributes on the method
            foreach (var enumKuvausSyntaksi in enums)
            {
                kt.ThrowIfCancellationRequested();

                SemanticModel semanttinenMalli = kompilation.GetSemanticModel(enumKuvausSyntaksi.SyntaxTree);
                if (semanttinenMalli.GetDeclaredSymbol(enumKuvausSyntaksi) is not INamedTypeSymbol enumSymboli )
                {
                    continue;
                }

                string enumNimi = enumSymboli.ToString();
                string enumLaajennusNimi = "EnumLaajennuket";
                foreach (AttributeData attribuuttiData in enumSymboli.GetAttributes())
                {
                    if (!enumAttribuutti.Equals(attribuuttiData.AttributeClass, SymbolEqualityComparer.Default))
                    {
                        // weird, we couldn't get the symbol, ignore it
                        continue;
                    }

                    foreach (KeyValuePair<string,TypedConstant> nimettyArgumentti in attribuuttiData.NamedArguments)
                    {
                        if (nimettyArgumentti.Key == "ExtensionClassName" && nimettyArgumentti.Value.Value?.ToString() is { } n)
                        {
                            enumLaajennusNimi = n;
                        }
                    }
                    break;                  
                }

                ImmutableArray<ISymbol> enumJasenet = enumSymboli.GetMembers();
                var jasenet = new List<string>(enumJasenet.Length);

                foreach (ISymbol jasen in enumJasenet)
                {
                    if (jasen is IFieldSymbol kentta && kentta.ConstantValue is not null)
                    {
                        jasenet.Add(jasen.Name);
                    }
                }
                enums2keneroi.Add(new Enum2Generoi(enumLaajennusNimi, enumNimi, jasenet));
            }

            // we didn't find the attribute we were looking for
            return enums2keneroi;
        }
        

    }
}
