# Projektplan Persönlicher Ausgaben-Tracker

## Ausgangslage

Wir entwickeln im Team eine Anwendung in C#, mit der persönliche Einnahmen und Ausgaben erfasst und verwaltet werden können. Die Daten werden dauerhaft in einer SQLite-Datenbank gespeichert. Das Projekt setzt objektorientierte Programmierung um und erfüllt Anforderungen aus den Modulen 162, 164, 319, 320 und 106.

Wir arbeiten während der vorgesehenen Donnerstagvormittage am Projekt und ergänzen die Arbeit bei Bedarf individuell ausserhalb des Unterrichts.

## Ziel der Anwendung

Die Anwendung ermöglicht es, Einnahmen, Ausgaben und Kategorien zu erfassen, zu bearbeiten, zu löschen und auszuwerten. Die Daten werden persistent gespeichert und können jederzeit wieder abgerufen werden.

Die Software soll:

- benutzerfreundlich sein
- Eingaben validieren
- Fehler verständlich behandeln
- CRUD-Operationen unterstützen
- objektorientiert aufgebaut sein
- SQLite verwenden
- Unit-Tests für zentrale Funktionen enthalten

## Teamorganisation

**Boris**

- Softwareentwicklung und OOP
- Klassenstruktur und Geschäftslogik
- UML-Klassendiagramm
- Benutzeroberfläche

**Dmytro**

- Datenbank und Persistenz
- ER-Modell und relationales Modell
- SQLite-Datenbankstruktur
- SQL-Scripts
- Datenbankzugriff und CRUD-Operationen
- Singleton-Datenbankverbindung

**Fabio**

- Qualitätssicherung
- Unit-Tests und Datenbanktests
- Validierung und Fehlerbehandlung
- Markdown-Dokumentation
- Git-Verwaltung
- Video und Präsentation

## Zeitplanung & Detaillierter Semesterplan (17 Lerneinheiten)

Die Zusammenarbeit erfolgt an insgesamt 17 Donnerstagvormittagen zu je 2 Lektionen (insgesamt 34 Lektionen), ergänzt durch individuelle Arbeiten ausserhalb des Unterrichts.

| Datum | Lektionen (Donnerstag) | Geplante Aktivitäten / Meilensteine | Relevante Modul-Zuordnungen & Prüfungen |
| :--- | :--- | :--- | :--- |
| **29.01.2026** | Lektion 1 & 2 | Semesterstart, Kick-Off, Teambildung, Git-Repository & Projekt-Setup | Allgemeine Bestimmungen |
| **05.02.2026** | Lektion 3 & 4 | Definition der funktionalen Anforderungen, Features und Anwendungsziele | Modul 319 (Applikationen entwerfen) |
| **12.02.2026** | Lektion 5 & 6 | Modellierung der Datenbank: Konzeptionelles ER-Modell | Modul 162 (Daten modellieren) |
| **19.02.2026** | Lektion 7 & 8 | Ableitung des relationalen Datenbankmodells aus dem ER-Modell | Modul 162 (Daten modellieren) |
| **26.02.2026** | Lektion 9 & 10 | Abschluss Datenmodellierung & **Refresher Test Modul 162** | **Refresher Test Modul 162** |
| **05.03.2026** | Lektion 11 & 12 | Softwaredesign: Erstellung des UML-Klassendiagramms (OOP-Struktur) | Modul 320 (Objektorientiert Programmieren) |
| **12.03.2026** | Lektion 13 & 14 | Entwurf des Programmablaufplans (PAP) mittels PAPDesigner | Modul 319 (Applikationen implementieren) |
| **19.03.2026** | Lektion 15 & 16 | Erstellung von SQL-Create-/Seed-Skripten & **Refresher Test Modul 164** | **Refresher Test Modul 164** / Modul 164 |
| **26.03.2026** | Lektion 17 & 18 | Implementierung der Singleton-Datenbankverbindung und des Schema-Updates | Modul 164 / Modul 320 |
| **02.04.2026** | Lektion 19 & 20 | Repository-Entwurf und C#-Verbindungsaufbau zu SQLite | Modul 164 / Modul 106 |
| **09.04.2026** | Lektion 21 & 22 | OOP-Modellierung: Implementierung abstrakter Klassen und Vererbung | Modul 320 (Objektorientiert Programmieren) |
| **16.04.2026** | Lektion 23 & 24 | Polymorphie: Interface-Methoden und deren runtime-spezifische Overrides | Modul 320 (Objektorientiert Programmieren) |
| **23.04.2026** | Lektion 25 & 26 | Start WPF-UI & **Refresher Test Modul 319** | **Refresher Test Modul 319** |
| **30.04.2026** | Lektion 27 & 28 | UI-Entwicklung: Datenbindung (Binding) und CRUD-Schnittstellen | Modul 319 / Modul 106 |
| **07.05.2026** | Lektion 29 & 30 | Validierung von Benutzereingaben, Fehlerbehandlung und Excel-Export | Modul 106 / Allgemein |
| **14.05.2026** | Lektion 31 & 32 | Qualitätssicherung: Schreiben von Unit-Tests für CRUD & Geschäftslogik | Modul 106 / Modul 320 (Tests) |
| **21.05.2026** | Lektion 33 & 34 | Finalisierung der C#-Anwendung & **Refresher Test Modul 320** | **Refresher Test Modul 320** |

## Semester-Abschlusstermine

- **Freitag, 12.06.2026 23:59 Uhr:** Abgabetermin (Quellcode & Markdown-Dokumentation vollständig im Git-Repository)
- **Samstag & Sonntag, 13. + 14. Juni 2026:** Projektkorrektur durch die Lehrkraft
- **Donnerstag, 18.06.2026:** Projektbesprechung
- **Donnerstag, 25.06.2026 & 02.07.2026:** Projektpräsentationen (Vorführung des 7-minütigen Videos und Fragerunde)
- **Donnerstag, 02.07.2026:** Modulende


## Arbeitsorganisation

Die Hauptarbeitszeit ist der Donnerstagvormittag im Unterricht. Zusätzliche Arbeiten erfolgen individuell ausserhalb des Unterrichts.

Zur Koordination werden:

- Aufgaben wöchentlich besprochen
- Fortschritte im Git Repository dokumentiert
- Arbeiten nach Verfügbarkeit verteilt
- Pufferzeiten vor der Abgabe eingeplant
