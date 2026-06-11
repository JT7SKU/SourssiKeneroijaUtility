using System;
using System.Collections.Generic;
using System.Text;

namespace SourssiKeneroijaUtility.EnumGeneraattorit
{
    public readonly record struct Enum2Generoi
    {
        public readonly string Nimi;
        public readonly EquatableArray<string> Arvot;

        public Enum2Generoi(string nimi, List<string> arvot)
        {
            Nimi = nimi;
            Arvot = new(arvot.ToArray());
        }
    }
}
