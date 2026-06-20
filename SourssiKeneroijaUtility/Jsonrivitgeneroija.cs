using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SourssiKeneroijaUtility.JSONRivit;
using System.Text;
using System.Text.Json;

namespace SourssiKeneroijaUtility
{
    [Generator]
    public class Jsonrivitgeneroija : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initKonteksti)
        {
            // define the execution pipeline here via a series of transformations:
            initKonteksti.RegisterPostInitializationOutput(ktx => ktx.AddSource(
                ".g.cs",SourceText.From(SourssiGeneroijaApulainen.Attribuutti, Encoding.UTF8)));

            // find all additional files that end with .txt
            IncrementalValuesProvider<AdditionalText> tekstiTiedostot = initKonteksti.AdditionalTextsProvider.Where(static tiedosto => tiedosto.Path.EndsWith(".txt"));

            // read their contents and save their name
            IncrementalValuesProvider<(string nimi, string kontentti)> nimetJaSisallot = tekstiTiedostot.Select((text, cancellationToken) => (name: Path.GetFileNameWithoutExtension(text.Path), content: text.GetText(cancellationToken)!.ToString()));

            // generate a class that contains their values as const strings
            initKonteksti.RegisterSourceOutput(nimetJaSisallot, (spc, nimiJaSisalto) =>
            {
                spc.AddSource($"JsonRivit.{nimiJaSisalto.nimi}.g.cs", $@"
    public static partial class {nimiJaSisalto.nimi}
    {{
        public const string {nimiJaSisalto.nimi} = ""{nimiJaSisalto.kontentti}"";
    }}");
            });
        }

        static void Execute(JsonRivit2Generoi? jsonRivit2Generate, SourceProductionContext konteksti)
        {
            if (jsonRivit2Generate is { } arvo)
            {
                // generate the source code and add it to the output
                string result = SourssiGeneroijaApulainen.GeneroiLaajennusLuokka(arvo);
                // Create a separate partial class file for each enum
                konteksti.AddSource($"JsonRivitLaajennukset.{arvo.Nimi}.g.cs", SourceText.From(result, Encoding.UTF8));
            }
        }
        static List<JsonRivit2Generoi> LueJsonRivit(string osoite) 
        {
            var tulos = new List<JsonRivit2Generoi>();
            try
            {
                using var lukija = new StreamReader(osoite);
                string? rivi;
                while ((rivi = lukija.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(rivi)) continue;
                    try
                    {
                        var olio = new JsonSerializer.Deserialize(rivi);
                        if (olio != null)
                        {
                            tulos.Add(olio);
                        }
                    }
                    catch (JsonException jsonEx)
                    {

                        Console.Error.WriteLine($"invalid json line skipped {jsonEx.Message}");
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine("File not found.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading JSONL: {ex.Message}");
            }

            return tulos;

        }
    }
}
