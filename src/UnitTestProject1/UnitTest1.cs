using Ausgabentracker.Database; 
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Ausgabentracker.Tests
{
    [TestClass]
    [DoNotParallelize]
    public class DatenbankTests
    {
        private const string DbPfad = "Ausgaben.db";

        [TestInitialize]
        public void TestInitialisieren()
        {
            DatenbankNeuErstellen();
        }

        private static void DatenbankNeuErstellen()
        {
            SqliteConnection.ClearAllPools();

            if (File.Exists(DbPfad))
            {
                File.Delete(DbPfad);
            }

            DatabaseManager.Instance.InitializeDatabase();
        }

        [TestMethod]
        public void Teste_Ob_Datenbank_Erstellt_Wird()
        {
            bool existiert = File.Exists(DbPfad);

            Assert.IsTrue(existiert, "FEHLER: Die Datenbankdatei wurde nicht erstellt! Überprüfe die SQL-Scripte.");
        }

        [TestMethod]
        public void Teste_Singleton_Instanz()
        {
            var instanz1 = DatabaseManager.Instance;
            var instanz2 = DatabaseManager.Instance;

            Assert.AreSame(instanz1, instanz2, "FEHLER: Das Singleton Pattern funktioniert nicht richtig!");
        }

        [TestMethod]
        public void Teste_Ob_Ausgabe_Gespeichert_Wird()
        {
            var repo = new Ausgabentracker.Repositories.AusgabenRepository();
            var neueAusgabe = new Ausgabentracker.Models.Ausgabe
            {
                Betrag = 99.95m,
                Datum = System.DateTime.Now,
                Notiz = "Unit Test Ausgabe",
                KategorieId = 1
            };

            repo.SpeichereAusgabe(neueAusgabe);
            var alleAusgaben = repo.LadeAlleAusgaben();

            bool gefunden = false;
            foreach (var a in alleAusgaben)
            {
                if (a.Notiz == "Unit Test Ausgabe" && a.Betrag == 99.95m)
                {
                    gefunden = true;
                    break;
                }
            }

            Assert.IsTrue(gefunden, "FEHLER: Die gespeicherte Ausgabe wurde nicht in der Datenbank gefunden!");
        }

        [TestMethod]
        public void Teste_Ob_Loeschen_Funktioniert()
        {
            var repo = new Ausgabentracker.Repositories.AusgabenRepository();
            var a = new Ausgabentracker.Models.Ausgabe { Betrag = 5m, Datum = DateTime.Now, Notiz = "DeleteTest", KategorieId = 1 };
            repo.SpeichereAusgabe(a);

            var liste = repo.LadeAlleAusgaben();
            var zuLoeschen = liste.Find(x => x.Notiz == "DeleteTest");

            repo.LoescheAusgabe(zuLoeschen.Id);

            var listeDanach = repo.LadeAlleAusgaben();
            bool existiertNoch = listeDanach.Exists(x => x.Id == zuLoeschen.Id);

            Assert.IsFalse(existiertNoch, "Fehler: Eintrag wurde nicht gelöscht!");
        }
    }
}
