using System;
using System.Collections.Generic;
using System.Text;

namespace SourssiKeneroijaUtility.EnumGeneraattorit
{
    public readonly record struct Enum2Generoi
    {
        public readonly string LaajennusNimi;
        public readonly string Nimi;
        public readonly List<string> Arvot;

        public Enum2Generoi(string laajennusNimi ,string nimi, List<string> arvot)
        {
            Nimi = nimi;
            Arvot = arvot;
            LaajennusNimi = laajennusNimi;
        }
    }
}
