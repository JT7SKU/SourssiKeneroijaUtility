using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourssiKeneroijaUtility
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

namespace Generated
{{
    public static class {luokkaNimi}
    {{
        public static T CreateInstance<T>() where T : class, new()
        {{
            return new T();
        }}
    }}
}}";
                    spc.AddSource($"{luokkaNimi}.g.cs", SourceText.From(lahde, Encoding.UTF8));
                }
            });
        }



    }
    
}
