using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourssiKeneroijaUtility.LuokkaGeneraattorit
{
    [Generator]
    public class Luokkageneroija : IIncrementalGenerator
    {

        public void Initialize(IncrementalGeneratorInitializationContext kontentti)
        {
            // Register a syntax provider that filters class declarations
            var luokkaKuvaukset = kontentti.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => s is ClassDeclarationSyntax, // Only consider class nodes
                    transform: static (ktx, _) => (ClassDeclarationSyntax)ktx.Node
                )
                .Collect(); // Collect all found class declarations

            // Register source output
            kontentti.RegisterSourceOutput(luokkaKuvaukset, (spc, luokat) =>
            {
                foreach (var luokkaKuvaus in luokat)
                {
                    string luokkaNimi = luokkaKuvaus.Identifier.Text;

                    // Generate a simple factory with generic constraint where T : class
                    string lahde = $@"
using System;

namespace SourssiKeneroijaUtility.LuokkaGeneraattorit
{{
    public static class {luokkaNimi}
    {{
        public static T LuoInstanssi<T>() where T : class, new()
        {{
            return new T();
        }}
    }}
}}";
                    spc.AddSource($"{luokkaNimi}.g.cs", SourceText.From(lahde, Encoding.UTF8));
                }
            });
        }
        static void Execute(Luokka2Generoi? luokka2Generoi, SourceProductionContext konteksti)
        {
            if (luokka2Generoi is { } arvo)
            {
                // generate the source code and add it to the output
                string result = SourssiGeneroijaApulainen.GeneroiLaajennusLuokka(arvo);
                // Create a separate partial class file for each enum
                konteksti.AddSource($"LuokkaLaajennukset.{arvo.Nimi}.g.cs", SourceText.From(result, Encoding.UTF8));
            }
        }



    }
    
}
