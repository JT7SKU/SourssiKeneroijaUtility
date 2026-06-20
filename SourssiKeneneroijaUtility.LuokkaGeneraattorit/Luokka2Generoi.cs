namespace SourssiKeneroijaUtility.LuokkaGeneraattorit
{
    public class Luokka2Generoi
    {
        public readonly string LaajennusNimi;
        public readonly string Nimi;
        public LuokkaTaulukko<string> Arvot;

        public Luokka2Generoi(string nimi, List<string> arvot , string laajennusNimi)
        {
            this.Nimi = nimi;
            Arvot = new(arvot.ToArray());
            LaajennusNimi = laajennusNimi;
        }
    }
}
