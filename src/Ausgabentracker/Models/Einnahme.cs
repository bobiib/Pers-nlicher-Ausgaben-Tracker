namespace AusgabenTracker.Models
{
    public class Einnahme : FinanzEintrag
    {
        public string EinnahmeQuelle { get; set; }

        public override string ZeigeZusammenfassung()
        {
            return $"EINNAHME von {EinnahmeQuelle}: +{Betrag} CHF";
        }

        public override decimal BerechneBetrag()
        {
            return Betrag;
        }
    }
}