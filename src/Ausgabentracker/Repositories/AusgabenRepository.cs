using Ausgabentracker.Database;
using Ausgabentracker.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace Ausgabentracker.Repositories
{
    public class AusgabenRepository
    {
        private string connectionString = "Data Source=Ausgaben.db";

        // CREATE
        public void SpeichereAusgabe(Ausgabe neueAusgabe)
        {
            string sql = $@"
                INSERT INTO Transaktion (Betrag, Datum, Notiz, KategorieId, Typ, SpezialFeld) 
                VALUES ({neueAusgabe.Betrag.ToString(System.Globalization.CultureInfo.InvariantCulture)}, 
                        '{neueAusgabe.Datum:yyyy-MM-dd HH:mm:ss}', 
                        '{neueAusgabe.Notiz}', 
                        {neueAusgabe.KategorieId}, 
                        'Ausgabe', 
                        'False')";
            DatabaseManager.Instance.ExecuteNonQuery(sql);
        }

        // READ (Alle Ausgaben für die Liste)
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
                    JOIN Kategorie k ON t.KategorieId = k.Id 
                    WHERE t.Typ = 'Ausgabe' ORDER BY t.Datum DESC";

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
                            KategorieName = reader.GetString(5)
                        });
                    }
                }
            }
            return ausgaben;
        }

        // READ (Kategorien für Dropdown)
        public List<Kategorie> LadeAlleKategorien()
        {
            var kategorien = new List<Kategorie>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name, Beschreibung FROM Kategorie";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        kategorien.Add(new Kategorie { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                    }
                }
            }
            return kategorien;
        }

        // DELETE (Das fehlende 'D' in CRUD - Modul 106)
        public void LoescheAusgabe(int id)
        {
            string sql = $"DELETE FROM Transaktion WHERE Id = {id}";
            DatabaseManager.Instance.ExecuteNonQuery(sql);
        }

        // LOGIK IN DB (Nutzt die View für Modul 106)
        public decimal LadeGesamtSumme()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                // Wir fragen die View v_KategorieSummen ab, die Dmytro in SQL erstellt hat!
                command.CommandText = "SELECT SUM(TotalBetrag) FROM v_KategorieSummen";
                var result = command.ExecuteScalar();
                return (result != null && result != DBNull.Value) ? Convert.ToDecimal(result) : 0;
            }
        }
    }
}