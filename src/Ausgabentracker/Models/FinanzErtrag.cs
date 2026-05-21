using System;

namespace Ausgabentracker.Models
{
    public abstract class FinanzEintrag : ITransaktion
    {
        public int Id { get; set; }
        public decimal Betrag { get; set; }
        public DateTime Datum { get; set; }
        public string Notiz { get; set; }
        public int KategorieId { get; set; }
        public string KategorieName { get; set; }
        public abstract string ZeigeZusammenfassung();
        public virtual string GetBeschreibung()
        {
            return $"{Datum.ToShortDateString()}: {Notiz} ({Betrag} CHF)";
        }
        public abstract decimal BerechneBetrag();
    }
}