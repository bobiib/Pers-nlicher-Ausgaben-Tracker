namespace Ausgabentracker.Models
{
    public interface ITransaktion
    {
        string GetBeschreibung();
        decimal BerechneBetrag();
    }
}