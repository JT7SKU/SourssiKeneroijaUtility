using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourssiKeneroijaUtility
{
    [Generator]
    public class Luokkageneroija : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
             
            // Retrieve syntax receiver
            if (context.SyntaxReceiver is not LuokkaSyntaxReceiver receiver)
                return;

            foreach (var classDeclaration in receiver.CandidateClasses)
            {
                string className = classDeclaration.Identifier.Text;

                // Generate a generic class factory constrained where T : class
                string source = $@"
using System;
using System;

namespace Generated
{{
    public static class {className}Factory
    {{
        public static T CreateInstance<T>() where T : class, new()
        {{
            return new T();
        }}
    }}
}}
";
            context.AddSource($"{className}Factory.g.cs", SourceText.From(source, Encoding.UTF8));
        }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // Optional: Register a syntax receiver if needed
            context.RegisterForSyntaxNotifications(() => new LuokkaSyntaxReceiver());
        }
        // Syntax receiver to collect candidate classes
        private class LuokkaSyntaxReceiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> CandidateClasses { get; } = new();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclaration)
                {
                    CandidateClasses.Add(classDeclaration);
                }
            }
        }
    }
    
}
