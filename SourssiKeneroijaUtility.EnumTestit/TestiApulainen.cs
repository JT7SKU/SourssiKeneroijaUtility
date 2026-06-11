using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SourssiKeneroijaUtility.EnumGeneraattorit;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourssiKeneroijaUtility.EnumTestit
{
    public static class TestiApulainen
    {
        public static Task Verify(string sourssi)
        {
            // Parse the provided string into a C# syntax tree
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourssi);

            IEnumerable<PortableExecutableReference> references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };

            // Create a Roslyn compilation for the syntax tree.
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "Testit",
                syntaxTrees: new[] { syntaxTree });


            // Create an instance of our EnumGenerator incremental source generator
            var generaattori = new EnumGeneraattori();

            // The GeneratorDriver is used to run our generator against a compilation
            GeneratorDriver ajuri = CSharpGeneratorDriver.Create(generaattori);

            // Run the source generator!
            ajuri = ajuri.RunGenerators(compilation);

            // Use verify to snapshot test the source generator output!
            return Verifier.Verify(ajuri).UseDirectory("Snapshots");
        }
    }
}
