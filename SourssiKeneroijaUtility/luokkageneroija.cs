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

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Register a syntax provider that filters class declarations
            var classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => s is ClassDeclarationSyntax, // Only consider class nodes
                    transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node
                )
                .Collect(); // Collect all found class declarations

            // Register source output
            context.RegisterSourceOutput(classDeclarations, (spc, luokat) =>
            {
                foreach (var classDeclaration in luokat)
                {
                    string luokkaNimi = classDeclaration.Identifier.Text;

                    // Generate a simple factory with generic constraint where T : class
                    string source = $@"
using System;

namespace Generated
{{
    public static class {luokkaNimi}Factory
    {{
        public static T CreateInstance<T>() where T : class, new()
        {{
            return new T();
        }}
    }}
}}";
                    spc.AddSource($"{luokkaNimi}Factory.g.cs", SourceText.From(source, Encoding.UTF8));
                }
            });
        }



    }
    
}
