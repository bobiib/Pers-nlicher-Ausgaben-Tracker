using Ausgabentracker.Database;
using Ausgabentracker.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace Ausgabentracker.Repositories
{
    public class AusgabenRepository
    {
        // 1. CREATE: Eine neue Ausgabe speichern
        public void SpeichereAusgabe(Ausgabe neueAusgabe)
        {
            string sql = $@"
                INSERT INTO Transaktion (Betrag, Datum, Notiz, KategorieId, Typ, SpezialFeld) 
                VALUES ({neueAusgabe.Betrag.ToString(System.Globalization.CultureInfo.InvariantCulture)}, 
                        '{neueAusgabe.Datum:yyyy-MM-dd HH:mm:ss}', 
                        '{neueAusgabe.Notiz}', 
                        {neueAusgabe.KategorieId}, 
                        'Ausgabe', 
                        '{neueAusgabe.IstSteuerlichAbsetzbar}')";

            DatabaseManager.Instance.ExecuteNonQuery(sql);
        }

        // 2. READ: Alle Kategorien aus der DB laden (für ein Dropdown in der UI)
        public List<Kategorie> LadeAlleKategorien()
        {
            var kategorien = new List<Kategorie>();
            string connectionString = "Data Source=Ausgaben.db"; // Pfad zur DB

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