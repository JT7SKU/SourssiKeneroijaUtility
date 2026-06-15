using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace SourssiKeneroijaUtility.EnumGeneraattorit
{
    public static class SourssiGeneroijaApulainen
    {
        public const string Attribuutti = @"
        namespace SourssiKeneroijaUtility.EnumGeneraattorit
{
    [System.AttributeUsage(System.AttributeTargets.Enum)]
    public class EnumExtensionsAttribute : System.Attribute
    {
    }
    }";

        

        public static string GeneroiLaajennusLuokka(List<Enum2Generoi> enums2Generoi)
        {
            var sb = new StringBuilder();
            sb.Append(@"
namespace SourssiKeneroijaUtility.EnumGeneraattorit
{");
            foreach (var enum2Generoi in enums2Generoi)
            {
                sb.Append(@"public static partial class ").Append(enum2Generoi.LaajennusNimi).Append(@"
            public static string ToStringFast(this ").Append(enum2Generoi.Nimi).Append(@" arvo)
                => arvo switch
                {");
                foreach (var jasen in enum2Generoi.Arvot)
                {
                    sb.Append(@"
            ").Append(enum2Generoi.Nimi).Append('.').Append(jasen)
                        .Append(" => nameof(")
                        .Append(enum2Generoi.Nimi).Append('.').Append(jasen).Append("),");
                }

                sb.Append(@"
                _ => value.ToString(),
            };
");
            }
            sb.Append(@"
    }
}");

            return sb.ToString();
        }
    }
}
