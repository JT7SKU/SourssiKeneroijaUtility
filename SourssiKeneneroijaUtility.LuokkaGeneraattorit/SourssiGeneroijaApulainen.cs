using System;
using System.Collections.Generic;
using System.Text;

namespace SourssiKeneroijaUtility.LuokkaGeneraattorit
{
    public static class SourssiGeneroijaApulainen
    {
        public const string Attribuutti ="" ;
        public  static string GeneroiLaajennusLuokka(Luokka2Generoi luokka2Generoi)
        {
            var sb = new StringBuilder();
            sb.Append($@"
using System;

namespace {luokka2Generoi.LaajennusNimi}
{{
    public static class {luokka2Generoi.Nimi}
    {{
        public static T LuoInstanssi<T>() where T : class, new()
        {{
            return new T();
        }}
    }}
}}");


            return sb.ToString();
        }

    }
}
