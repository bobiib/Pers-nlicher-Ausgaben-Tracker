-- Fügt der Kategorie-Tabelle eine neue Spalte hinzu, falls wir später Icons in der UI wollen
-- (Wenn die Spalte schon existiert, fängt unser C#-Code den Fehler ab)
ALTER TABLE Kategorie ADD COLUMN Icon TEXT;
