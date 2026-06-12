using Ausgabentracker.Models;
using Ausgabentracker.Database;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ausgabentracker.Repositories
{
    public class AusgabenRepository
    {
        private string connectionString => DatabaseManager.Instance.ConnectionString;

        public void SpeichereAusgabe(Ausgabe neueAusgabe)
        {
            SpeichereTransaktion(neueAusgabe, "Ausgabe", "False");
        }

        public void SpeichereEinnahme(Einnahme neueEinnahme)
        {
            SpeichereTransaktion(neueEinnahme, "Einnahme", neueEinnahme.EinnahmeQuelle ?? "");
        }

        private void SpeichereTransaktion(FinanzEintrag transaktion, string typ, string spezialFeld)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Transaktion (Betrag, Datum, Notiz, KategorieId, Typ, SpezialFeld)
                    VALUES ($betrag, $datum, $notiz, $kategorieId, $typ, $spezialFeld)";
                command.Parameters.AddWithValue("$betrag", transaktion.Betrag);
                command.Parameters.AddWithValue("$datum", transaktion.Datum.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("$notiz", transaktion.Notiz ?? "");
                command.Parameters.AddWithValue("$kategorieId", transaktion.KategorieId);
                command.Parameters.AddWithValue("$typ", typ);
                command.Parameters.AddWithValue("$spezialFeld", spezialFeld ?? "");
                command.ExecuteNonQuery();
            }
        }

        public void AktualisiereAusgabe(Ausgabe ausgabe)
        {
            AktualisiereTransaktion(ausgabe, "Ausgabe");
        }

        public void AktualisiereTransaktion(FinanzEintrag transaktion, string typ)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE Transaktion
                    SET Betrag = $betrag,
                        Datum = $datum,
                        Notiz = $notiz,
                        KategorieId = $kategorieId,
                        Typ = $typ,
                        SpezialFeld = $spezialFeld
                    WHERE Id = $id";
                command.Parameters.AddWithValue("$betrag", transaktion.Betrag);
                command.Parameters.AddWithValue("$datum", transaktion.Datum.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("$notiz", transaktion.Notiz ?? "");
                command.Parameters.AddWithValue("$kategorieId", transaktion.KategorieId);
                command.Parameters.AddWithValue("$typ", typ);
                command.Parameters.AddWithValue("$spezialFeld", typ == "Einnahme" ? "Einnahme" : "False");
                command.Parameters.AddWithValue("$id", transaktion.Id);
                command.ExecuteNonQuery();
            }
        }

        public List<FinanzEintrag> LadeAlleTransaktionen()
        {
            var transaktionen = new List<FinanzEintrag>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT t.Id, t.Betrag, t.Datum, t.Notiz, t.KategorieId, k.Name, t.Typ, t.SpezialFeld
                    FROM Transaktion t
                    LEFT JOIN Kategorie k ON t.KategorieId = k.Id
                    ORDER BY t.Datum DESC, t.Id DESC";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string typ = reader.IsDBNull(6) ? "Ausgabe" : reader.GetString(6);
                        FinanzEintrag transaktion = typ == "Einnahme"
                            ? (FinanzEintrag)new Einnahme
                            {
                                EinnahmeQuelle = reader.IsDBNull(7) ? "" : reader.GetString(7)
                            }
                            : new Ausgabe();

                        transaktion.Id = reader.GetInt32(0);
                        transaktion.Betrag = reader.GetDecimal(1);
                        transaktion.Datum = reader.GetDateTime(2);
                        transaktion.Notiz = reader.IsDBNull(3) ? "" : reader.GetString(3);
                        transaktion.KategorieId = reader.GetInt32(4);
                        transaktion.KategorieName = reader.IsDBNull(5) ? "" : reader.GetString(5);
                        transaktionen.Add(transaktion);
                    }
                }
            }

            return transaktionen;
        }

        public List<Ausgabe> LadeAlleAusgaben()
        {
            var ausgaben = new List<Ausgabe>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT t.Id, t.Betrag, t.Datum, t.Notiz, t.KategorieId, k.Name
                    FROM Transaktion t
                    LEFT JOIN Kategorie k ON t.KategorieId = k.Id
                    WHERE t.Typ = 'Ausgabe'
                    ORDER BY t.Datum DESC, t.Id DESC";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ausgaben.Add(new Ausgabe
                        {
                            Id = reader.GetInt32(0),
                            Betrag = reader.GetDecimal(1),
                            Datum = reader.GetDateTime(2),
                            Notiz = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            KategorieId = reader.GetInt32(4),
                            KategorieName = reader.IsDBNull(5) ? "" : reader.GetString(5)
                        });
                    }
                }
            }

            return ausgaben;
        }

        public List<Kategorie> LadeAlleKategorien()
        {
            var kategorien = new List<Kategorie>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name, Beschreibung FROM Kategorie ORDER BY Name";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        kategorien.Add(new Kategorie
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Beschreibung = reader.IsDBNull(2) ? "" : reader.GetString(2)
                        });
                    }
                }
            }

            return kategorien;
        }

        public void SpeichereKategorie(Kategorie kategorie)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Kategorie (Name, Beschreibung)
                    VALUES ($name, $beschreibung)";
                command.Parameters.AddWithValue("$name", kategorie.Name);
                command.Parameters.AddWithValue("$beschreibung", kategorie.Beschreibung ?? "");
                command.ExecuteNonQuery();
            }
        }

        public void AktualisiereKategorie(Kategorie kategorie)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE Kategorie
                    SET Name = $name,
                        Beschreibung = $beschreibung
                    WHERE Id = $id";
                command.Parameters.AddWithValue("$name", kategorie.Name);
                command.Parameters.AddWithValue("$beschreibung", kategorie.Beschreibung ?? "");
                command.Parameters.AddWithValue("$id", kategorie.Id);
                command.ExecuteNonQuery();
            }
        }

        public void LoescheKategorie(int id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var pruefen = connection.CreateCommand();
                pruefen.CommandText = "SELECT COUNT(*) FROM Transaktion WHERE KategorieId = $id";
                pruefen.Parameters.AddWithValue("$id", id);
                long verwendet = (long)pruefen.ExecuteScalar();
                if (verwendet > 0)
                {
                    throw new InvalidOperationException("Diese Kategorie wird noch von Transaktionen verwendet und kann nicht gelöscht werden.");
                }

                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Kategorie WHERE Id = $id";
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
            }
        }

        public int FuegeBeispieldatenEin()
        {
            int eingefuegt = 0;
            var kategorien = LadeAlleKategorien();
            int lebensmittelId = KategorieIdErmittelnOderAnlegen(kategorien, "Lebensmittel", "Einkäufe im Supermarkt");
            int wohnenId = KategorieIdErmittelnOderAnlegen(kategorien, "Wohnen", "Miete, Strom, Internet");
            int freizeitId = KategorieIdErmittelnOderAnlegen(kategorien, "Freizeit", "Kino, Ausgang, Hobbys");
            int gehaltId = KategorieIdErmittelnOderAnlegen(kategorien, "Gehalt", "Monatlicher Lohn");

            if (SpeichereBeispielWennNeu(new Einnahme { Betrag = 4200m, Datum = DateTime.Today.AddDays(-10), Notiz = "Beispiel: Monatslohn", KategorieId = gehaltId, EinnahmeQuelle = "Arbeit" })) eingefuegt++;
            if (SpeichereBeispielWennNeu(new Ausgabe { Betrag = 185.40m, Datum = DateTime.Today.AddDays(-8), Notiz = "Beispiel: Wocheneinkauf", KategorieId = lebensmittelId })) eingefuegt++;
            if (SpeichereBeispielWennNeu(new Ausgabe { Betrag = 1450m, Datum = DateTime.Today.AddDays(-6), Notiz = "Beispiel: Miete", KategorieId = wohnenId })) eingefuegt++;
            if (SpeichereBeispielWennNeu(new Ausgabe { Betrag = 64.90m, Datum = DateTime.Today.AddDays(-3), Notiz = "Beispiel: Kino und Essen", KategorieId = freizeitId })) eingefuegt++;

            return eingefuegt;
        }

        public void LoescheAusgabe(int id)
        {
            LoescheTransaktion(id);
        }

        public void LoescheTransaktion(int id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Transaktion WHERE Id = $id";
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
            }
        }

        public decimal LadeGesamtAusgaben()
        {
            return LadeSummeNachTyp("Ausgabe");
        }

        public decimal LadeGesamtEinnahmen()
        {
            return LadeSummeNachTyp("Einnahme");
        }

        public decimal LadeSaldo()
        {
            return LadeGesamtEinnahmen() - LadeGesamtAusgaben();
        }

        public decimal LadeGesamtSumme()
        {
            return LadeGesamtAusgaben();
        }

        private decimal LadeSummeNachTyp(string typ)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT SUM(Betrag) FROM Transaktion WHERE Typ = $typ";
                command.Parameters.AddWithValue("$typ", typ);
                var result = command.ExecuteScalar();
                return (result != null && result != DBNull.Value) ? Convert.ToDecimal(result) : 0;
            }
        }

        private int KategorieIdErmittelnOderAnlegen(List<Kategorie> kategorien, string name, string beschreibung)
        {
            var vorhandeneKategorie = kategorien.FirstOrDefault(k => string.Equals(k.Name, name, StringComparison.OrdinalIgnoreCase));
            if (vorhandeneKategorie != null)
            {
                return vorhandeneKategorie.Id;
            }

            SpeichereKategorie(new Kategorie { Name = name, Beschreibung = beschreibung });
            var neueKategorie = LadeAlleKategorien().First(k => string.Equals(k.Name, name, StringComparison.OrdinalIgnoreCase));
            kategorien.Add(neueKategorie);
            return neueKategorie.Id;
        }

        private bool SpeichereBeispielWennNeu(FinanzEintrag transaktion)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var pruefen = connection.CreateCommand();
                pruefen.CommandText = "SELECT COUNT(*) FROM Transaktion WHERE Notiz = $notiz";
                pruefen.Parameters.AddWithValue("$notiz", transaktion.Notiz);
                long vorhanden = (long)pruefen.ExecuteScalar();
                if (vorhanden > 0)
                {
                    return false;
                }
            }

            var einnahme = transaktion as Einnahme;
            if (einnahme != null)
            {
                SpeichereEinnahme(einnahme);
                return true;
            }

            SpeichereAusgabe((Ausgabe)transaktion);
            return true;
        }

        public List<KategorieSumme> LadeKategorieSummen()
        {
            var list = new List<KategorieSumme>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT KategorieName, TotalBetrag, AnzahlTransaktionen FROM v_KategorieSummen";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new KategorieSumme
                        {
                            KategorieName = reader.GetString(0),
                            TotalBetrag = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1),
                            AnzahlTransaktionen = reader.GetInt32(2)
                        });
                    }
                }
            }
            return list;
        }
    }
}
