namespace Ausgabentracker.Models
{
    public class Ausgabe : FinanzEintrag
    {
        public bool IstSteuerlichAbsetzbar { get; set; }
        public override string Typ { get { return "Ausgabe"; } }

        public override string ZeigeZusammenfassung()
        {
            string steuer = IstSteuerlichAbsetzbar ? "[Steuerlich absetzbar]" : "";
            return $"AUSGABE: {Notiz} - {Betrag} CHF {steuer}";
        }


        public override string GetBeschreibung()
        {
            string steuer = IstSteuerlichAbsetzbar ? " [Steuerabzug]" : "";
            return $"{Datum.ToShortDateString()}: Ausgabe{steuer} - {Notiz} (-{Betrag} CHF)";
        }

        public override decimal BerechneBetrag()
        {
            return -Betrag;
        }
    }
}
