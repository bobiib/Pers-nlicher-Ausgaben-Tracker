# Persönlicher Ausgaben-Tracker

## Ziel der Anwendung

Der Persönliche Ausgaben-Tracker ist eine C#-Desktop-Anwendung, mit der Benutzer ihre alltäglichen Finanzen verwalten können. Einnahmen und Ausgaben werden erfasst, Kategorien zugeordnet und dauerhaft in einer lokalen SQLite-Datenbank gespeichert.

Die Anwendung richtet sich an Privatpersonen, die ohne komplexe Buchhaltungssoftware einen klaren Überblick über ihre Einnahmen, Ausgaben und ihren Saldo behalten möchten.

## Features

Die Anwendung deckt folgende Kernfunktionen ab:

- **Datenbank-Automatisierung:** Beim ersten Start prüft die App, ob die Datenbank existiert. Falls nicht, wird sie automatisch über das Create-Script erstellt und mit Standard-Kategorien befüllt.
- **Datenbank-Update:** Beim Start wird ein vorhandenes Update-Script ausgeführt, damit spätere Datenbankänderungen übernommen werden können.
- **Beispieldaten:** Über eine Schaltfläche können Beispieltransaktionen eingefügt werden. Bereits vorhandene Beispieldaten werden nicht doppelt erstellt.
- **Transaktionen verwalten (CRUD):** Einnahmen und Ausgaben können erstellt, angezeigt, bearbeitet und gelöscht werden.
- **Kategorien verwalten (CRUD):** Kategorien können erstellt, angezeigt, bearbeitet und gelöscht werden. Kategorien, die noch von Transaktionen verwendet werden, werden geschützt.
- **Übersicht und Filterung:** Transaktionen werden gruppiert nach Monat angezeigt. Zusätzlich können sie nach Monat und Kategorie gefiltert werden.
- **Monatliche Zusammenfassung:** Neben jedem Monatsnamen werden Einnahmen, Ausgaben und Saldo dieses Monats angezeigt.
- **Excel-Export:** Die Übersicht kann als Excel-kompatible Datei exportiert werden.
- **Validierung und Fehlerbehandlung:** Ungültige Eingaben und Datenbankfehler werden abgefangen und als verständliche Meldungen angezeigt.

## Technologien und Architektur

- **Sprache:** C# / .NET Framework 4.8
- **Oberfläche:** WPF
- **Datenbank:** SQLite
- **Datenbankzugriff:** Repository-Klasse mit SQL-Befehlen
- **Datenbankverbindung:** Singleton Pattern über `DatabaseManager`
- **OOP:** Klassen, Objekte, abstrakte Klasse, Interface, Vererbung und Polymorphismus
- **Qualitätssicherung:** MSTest-Unit-Tests für Datenbankerstellung, Singleton, CRUD, Einnahmen/Ausgaben, Kategorien und Beispieldaten

## Projektstruktur

- `src/Ausgabentracker`: Hauptanwendung
- `src/UnitTestProject1`: Unit-Tests
- `SQL_Scripts`: Create-, Update- und Seed-Scripts
- `Datenbankstruktur`: ER-Modell und relationales Modell
- `Softwaredesign`: UML-Klassendiagramm und Programmablaufplan
- `Planung`: Projektplan

## Projekt-Team

- **Boris:** Softwareentwicklung, OOP-Architektur, Benutzeroberfläche
- **Dmytro:** Datenbankmodellierung, SQL-Scripts, Persistenz
- **Fabio:** Qualitätssicherung, Unit-Tests, Dokumentation, Git-Verwaltung
