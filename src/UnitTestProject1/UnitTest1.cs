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

        [TestMethod]
        public void Teste_Ob_Ausgabe_Bearbeitet_Wird()
        {
            var repo = new Ausgabentracker.Repositories.AusgabenRepository();
            var ausgabe = new Ausgabentracker.Models.Ausgabe
            {
                Betrag = 12m,
                Datum = DateTime.Now,
                Notiz = "Vor Bearbeitung",
                KategorieId = 1
            };

            repo.SpeichereAusgabe(ausgabe);
            var gespeichert = repo.LadeAlleAusgaben().Find(x => x.Notiz == "Vor Bearbeitung");

            gespeichert.Betrag = 42.50m;
            gespeichert.Notiz = "Nach Bearbeitung";
            gespeichert.KategorieId = 2;

            repo.AktualisiereAusgabe(gespeichert);

            var bearbeitet = repo.LadeAlleAusgaben().Find(x => x.Id == gespeichert.Id);

            Assert.AreEqual(42.50m, bearbeitet.Betrag, "FEHLER: Der Betrag wurde nicht aktualisiert!");
            Assert.AreEqual("Nach Bearbeitung", bearbeitet.Notiz, "FEHLER: Die Notiz wurde nicht aktualisiert!");
            Assert.AreEqual(2, bearbeitet.KategorieId, "FEHLER: Die Kategorie wurde nicht aktualisiert!");
        }

        [TestMethod]
        public void Teste_Ob_Einnahme_Gespeichert_Wird()
        {
            var repo = new Ausgabentracker.Repositories.AusgabenRepository();
            var einnahme = new Ausgabentracker.Models.Einnahme
            {
                Betrag = 2500m,
                Datum = DateTime.Now,
                Notiz = "Lohn",
                KategorieId = 4,
                EinnahmeQuelle = "Arbeit"
            };

            repo.SpeichereEinnahme(einnahme);

            var transaktionen = repo.LadeAlleTransaktionen();
            var gespeichert = transaktionen.Find(x => x.Notiz == "Lohn");

            Assert.IsNotNull(gespeichert, "FEHLER: Die Einnahme wurde nicht gespeichert!");
            Assert.AreEqual("Einnahme", gespeichert.Typ, "FEHLER: Die Transaktion wurde nicht als Einnahme gespeichert!");
            Assert.AreEqual(2500m, gespeichert.BerechneBetrag(), "FEHLER: Einnahmen müssen positiv gerechnet werden!");
        }

        [TestMethod]
        public void Teste_Ob_Saldo_Berechnet_Wird()
        {
            var repo = new Ausgabentracker.Repositories.AusgabenRepository();
            repo.SpeichereEinnahme(new Ausgabentracker.Models.Einnahme { Betrag = 100m, Datum = DateTime.Now, Notiz = "Einnahme", KategorieId = 4 });
            repo.SpeichereAusgabe(new Ausgabentracker.Models.Ausgabe { Betrag = 35m, Datum = DateTime.Now, Notiz = "Ausgabe", KategorieId = 1 });

            Assert.AreEqual(100m, repo.LadeGesamtEinnahmen(), "FEHLER: Einnahmen-Summe stimmt nicht!");
            Assert.AreEqual(35m, repo.LadeGesamtAusgaben(), "FEHLER: Ausgaben-Summe stimmt nicht!");
            Assert.AreEqual(65m, repo.LadeSaldo(), "FEHLER: Saldo stimmt nicht!");
        }

        [TestMethod]
        public void Teste_Ob_Kategorie_CRUD_Funktioniert()
        {
            var repo = new Ausgabentracker.Repositories.AusgabenRepository();
            var kategorie = new Ausgabentracker.Models.Kategorie
            {
                Name = "TestKategorie",
                Beschreibung = "Vor Bearbeitung"
            };

            repo.SpeichereKategorie(kategorie);
            var gespeichert = repo.LadeAlleKategorien().Find(k => k.Name == "TestKategorie");

            Assert.IsNotNull(gespeichert, "FEHLER: Die Kategorie wurde nicht gespeichert!");

            gespeichert.Name = "TestKategorieBearbeitet";
            gespeichert.Beschreibung = "Nach Bearbeitung";
            repo.AktualisiereKategorie(gespeichert);

            var bearbeitet = repo.LadeAlleKategorien().Find(k => k.Id == gespeichert.Id);
            Assert.AreEqual("TestKategorieBearbeitet", bearbeitet.Name, "FEHLER: Der Kategoriename wurde nicht aktualisiert!");
            Assert.AreEqual("Nach Bearbeitung", bearbeitet.Beschreibung, "FEHLER: Die Beschreibung wurde nicht aktualisiert!");

            repo.LoescheKategorie(bearbeitet.Id);
            var geloescht = repo.LadeAlleKategorien().Find(k => k.Id == bearbeitet.Id);

            Assert.IsNull(geloescht, "FEHLER: Die Kategorie wurde nicht gelöscht!");
        }

        [TestMethod]
        public void Teste_Ob_Beispieldaten_Eingefuegt_Werden()
        {
            var repo = new Ausgabentracker.Repositories.AusgabenRepository();

            int eingefuegt = repo.FuegeBeispieldatenEin();
            int erneutEingefuegt = repo.FuegeBeispieldatenEin();
            var transaktionen = repo.LadeAlleTransaktionen();

            Assert.AreEqual(4, eingefuegt, "FEHLER: Es sollten vier Beispieltransaktionen eingefügt werden!");
            Assert.AreEqual(0, erneutEingefuegt, "FEHLER: Beispieldaten dürfen nicht doppelt eingefügt werden!");
            Assert.IsTrue(transaktionen.Exists(t => t.Notiz == "Beispiel: Monatslohn"), "FEHLER: Beispiel-Einnahme fehlt!");
            Assert.IsTrue(transaktionen.Exists(t => t.Notiz == "Beispiel: Miete"), "FEHLER: Beispiel-Ausgabe fehlt!");
        }
    }
}
