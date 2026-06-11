using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace SourssiKeneroijaUtility.EnumGeneraattori
{
    public static class SourssiGeneroijaApulainen
    {
        public const string Attribuutti = @"
        namespace NetEscapades.EnumGenerators
{
    [System.AttributeUsage(System.AttributeTargets.Enum)]
    public class EnumExtensionsAttribute : System.Attribute
    {
    }
    }";

        

        public static string GeneroiLaajennusLuokka(Enum2Generoi enum2Generoi)
        {
            var sb = new StringBuilder();
            sb.Append(@"
namespace NetEscapades.EnumGenerators
{
    public static partial class EnumExtensions
    {");
            sb.Append(@"
            public static string ToStringFast(this ").Append(enum2Generoi.Nimi).Append(@" arvo)
                => arvo switch
                {");
            foreach (var member in enum2Generoi.Arvot)
            {
                sb.Append(@"
            ").Append(enum2Generoi.Nimi).Append('.').Append(member)
                    .Append(" => nameof(")
                    .Append(enum2Generoi.Nimi).Append('.').Append(member).Append("),");
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
