# Persönlicher Ausgaben-Tracker 

## Ziel der Anwendung
Der "Persönliche Ausgaben-Tracker" ist eine C#-Desktop-Anwendung, die es Benutzern ermöglicht, ihre alltäglichen Finanzen im Blick zu behalten. Das Ziel ist es, Einnahmen und Ausgaben schnell zu erfassen, sie logischen Kategorien zuzuordnen und dauerhaft abzuspeichern. Die Applikation richtet sich an Privatpersonen, die ohne komplexe Buchhaltungssoftware ihre persönlichen Ausgaben kontrollieren möchten. Die Datenpersistenz wird durch eine lokale, ressourcenschonende SQLite-Datenbank gewährleistet.

## Features (Funktionaler Umfang)
Die Anwendung deckt folgende Kernfunktionen ab:

* **Datenbank-Automatisierung:** Beim ersten Start prüft die App, ob die Datenbank existiert, erstellt diese automatisch (Create-Script) und füllt sie bei Bedarf mit Standard-Kategorien (Seed-Script). Auch automatische Updates werden unterstützt.
* **Kategorien verwalten (CRUD):** Benutzer können eigene Ausgabenkategorien (z.B. "Lebensmittel", "Miete", "Freizeit") anlegen, bearbeiten, ansehen und löschen.
* **Ausgaben verwalten (CRUD):** Erfassen von neuen Ausgaben mit Betrag, Datum, Kategorie und einer optionalen Notiz. Einträge können nachträglich mutiert oder gelöscht werden.
* **Übersicht & Filterung:** Anzeige aller getätigten Ausgaben, filterbar nach Monaten oder Kategorien (erfordert Datenbankabfragen).
* **Validierung & Sicherheit:** Fehleingaben (z.B. Buchstaben im Betragsfeld oder leere Pflichtfelder) werden von der Applikation abgefangen (Exception Handling) und dem Benutzer als verständliche Fehlermeldung ausgegeben.

## Technologien & Architektur
* **Sprache:** C# / .NET
* **Datenbank:** SQLite (Anbindung via Singleton-Pattern)
* **Paradigma:** Objektorientierte Programmierung (OOP) inkl. Vererbung, Interfaces und abstrakter Klassen.
* **Qualitätssicherung:** Unit-Tests für Kernlogik und Datenbankzugriffe.

## Projekt-Team
* **Boris:** Softwareentwicklung, OOP-Architektur, Benutzeroberfläche
* **Dmytro:** Datenbankmodellierung, SQL-Scripts, Persistenz
* **Fabio:** Qualitätssicherung (Unit-Tests), Dokumentation, Git-Verwaltung
