using System;
using System.Collections.Generic;
using System.Text;

namespace SourssiKeneroijaUtility.JSONRivit
{
    public static class SourssiGeneroijaApulainen
    {
        public const string Attribuutti = @"
        namespace SourssiKeneroijaUtility.EnumGeneraattorit
{
    [System.AttributeUsage(System.AttributeTargets.Enum)]
    public class JsonLinesExtensionAttribute : System.Attribute
    {
    }
    }";

        public static string GeneroiLaajennusLuokka(JsonRivit2Generoi jsonRivit2Generoi)
        {
            var sb = new StringBuilder();
            sb.Append(@"
namespace SourssiKeneroijaUtility.EnumGeneraattorit
{
    public static partial class JsonRivitLaajennokset
    {");
            sb.Append(@"
            public static string ToLangalleNopeasti(this ").Append(jsonRivit2Generoi.Nimi).Append(@" arvo)
                => arvo switch
                {");
            foreach (var jasen in jsonRivit2Generoi.Arvot)
            {
                sb.Append(@"
            ").Append(jsonRivit2Generoi.Nimi).Append('.').Append(jasen)
                    .Append(" => nameof(")
                    .Append(jsonRivit2Generoi.Nimi).Append('.').Append(jasen).Append("),");
            }

            sb.Append(@"
                _ => value.ToString(),
            };
");

            sb.Append(@"
    }
}");

            return sb.ToString();
        }
    }
}
