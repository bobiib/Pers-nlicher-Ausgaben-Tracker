using Ausgabentracker.Database; 
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Ausgabentracker.Tests
{
    [TestClass]
    public class DatenbankTests
    {
        [TestMethod]
        public void Teste_Ob_Datenbank_Erstellt_Wird()
        {
            // 1. Arrange (Vorbereitung)
            // Wir prüfen, wo die Datenbank liegen sollte
            string dbPfad = "Ausgaben.db";

            // Wenn es eine alte Test-Datenbank gibt, löschen wir sie vorher,
            // damit der Test wirklich beweist, dass sie NEU erstellt wird.
            if (File.Exists(dbPfad))
            {
                File.Delete(dbPfad);
            }

            // 2. Act (Ausführung)
            // Wir rufen eure Singleton-Verbindung auf (Das ist Fabios Code!)
            DatabaseManager.Instance.InitializeDatabase();

            // 3. Assert (Überprüfung)
            // Wir behaupten: "Die Datei MUSS jetzt existieren!"
            bool existiert = File.Exists(dbPfad);

            Assert.IsTrue(existiert, "FEHLER: Die Datenbankdatei wurde nicht erstellt! Überprüfe die SQL-Scripte.");
        }

        [TestMethod]
        public void Teste_Singleton_Instanz()
        {
            // Testet die Anforderung aus Modul 320: Singleton Pattern
            var instanz1 = DatabaseManager.Instance;
            var instanz2 = DatabaseManager.Instance;

            // Beide Instanzen müssen im Arbeitsspeicher absolut identisch sein
            Assert.AreSame(instanz1, instanz2, "FEHLER: Das Singleton Pattern funktioniert nicht richtig!");
        }
    }
}