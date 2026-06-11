using System;
using System.Collections.Generic;
using System.Text;

namespace SourssiKeneroijaUtility.JSONRivit
{
    public readonly record struct JsonRivit2Generoi
    {
        public readonly string Nimi;
        public readonly JsonRiviTaulukko<string> Arvot;

        public JsonRivit2Generoi(string nimi, List<string> arvot)
        {
            this.Nimi = nimi;
            this.Arvot = new(arvot.ToArray());
        }
    }
}
