namespace Ausgabentracker.Models
{
    public class Einnahme : FinanzEintrag
    {
        public string EinnahmeQuelle { get; set; }
        public override string Typ { get { return "Einnahme"; } }

        public override string ZeigeZusammenfassung()
        {
            return $"EINNAHME von {EinnahmeQuelle}: +{Betrag} CHF";
        }

        public override string GetBeschreibung()
        {
            return $"{Datum.ToShortDateString()}: Einnahme aus {EinnahmeQuelle} - {Notiz} (+{Betrag} CHF)";
        }

        public override decimal BerechneBetrag()
        {
            return Betrag;
        }
    }
}
