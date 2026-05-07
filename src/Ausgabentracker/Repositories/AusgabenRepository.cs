using Ausgabentracker.Database;
using Ausgabentracker.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace Ausgabentracker.Repositories
{
    public class AusgabenRepository
    {
        // CREATE: Daten sicher in die DB schreiben (Modul 106)
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

        // READ: Holt die Ausgaben inkl. Kategorie-Namen für die UI Liste
        public List<Ausgabe> LadeAlleAusgaben()
        {
            var ausgaben = new List<Ausgabe>();
            string connectionString = "Data Source=Ausgaben.db";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                // SQL JOIN (Erfüllt Modul 106)
                command.CommandText = @"
                    SELECT t.Id, t.Betrag, t.Datum, t.Notiz, t.KategorieId, k.Name 
                    FROM Transaktion t 
                    JOIN Kategorie k ON t.KategorieId = k.Id 
                    WHERE t.Typ = 'Ausgabe'
                    ORDER BY t.Datum DESC";

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
                            KategorieName = reader.GetString(5) // Nimmt den Namen für die Liste
                        });
                    }
                }
            }
            return ausgaben;
        }

        // READ: Holt die Kategorien für das Dropdown
        public List<Kategorie> LadeAlleKategorien()
        {
            var kategorien = new List<Kategorie>();
            string connectionString = "Data Source=Ausgaben.db";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name, Beschreibung FROM Kategorie";
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
    }
}