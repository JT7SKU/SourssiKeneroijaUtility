using Microsoft.CodeAnalysis;
using System.Text.Json;

namespace SourssiKeneroijaUtility
{
    [Generator]
    public class Jsonrivitgeneroija : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initKonteksti)
        {
            // define the execution pipeline here via a series of transformations:

            // find all additional files that end with .txt
            IncrementalValuesProvider<AdditionalText> textFiles = initKonteksti.AdditionalTextsProvider.Where(static file => file.Path.EndsWith(".txt"));

            // read their contents and save their name
            IncrementalValuesProvider<(string name, string content)> namesAndContents = textFiles.Select((text, cancellationToken) => (name: Path.GetFileNameWithoutExtension(text.Path), content: text.GetText(cancellationToken)!.ToString()));

            // generate a class that contains their values as const strings
            initKonteksti.RegisterSourceOutput(namesAndContents, (spc, nameAndContent) =>
            {
                spc.AddSource($"ConstStrings.{nameAndContent.name}", $@"
    public static partial class ConstStrings
    {{
        public const string {nameAndContent.name} = ""{nameAndContent.content}"";
    }}");
            });
        }

       static ValueTask<List<T>> LueJsonRivit(string osoite)
        {
            var tulos = new List<T>();
            try
            {
                using var lukija = new StreamReader(osoite);
                string? rivi;
                while ((rivi = lukija.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(rivi)) continue;
                    try
                    {
                        var olio = new JsonSerializer.Deserialize<T>(rivi);
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
