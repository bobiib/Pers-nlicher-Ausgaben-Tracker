-- Tabelle für die Kategorien
CREATE TABLE IF NOT EXISTS Kategorie (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Beschreibung TEXT
);

-- Tabelle für die eigentlichen Einträge (Ausgaben & Einnahmen)
CREATE TABLE IF NOT EXISTS Transaktion (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Betrag DECIMAL(10,2) NOT NULL,
    Datum DATETIME NOT NULL,
    Notiz TEXT,
    KategorieId INTEGER,
    Typ TEXT NOT NULL, -- 'Ausgabe' oder 'Einnahme'
    SpezialFeld TEXT,  -- Für 'IstSteuerlichAbsetzbar' oder 'EinnahmeQuelle'
    FOREIGN KEY(KategorieId) REFERENCES Kategorie(Id)
);
