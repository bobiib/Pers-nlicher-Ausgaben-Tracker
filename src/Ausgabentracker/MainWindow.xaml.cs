using Ausgabentracker.Database;
using Ausgabentracker.Models;
using Ausgabentracker.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using Microsoft.Win32;

namespace Ausgabentracker
{
    public partial class MainWindow : Window
    {
        private const string SpreadsheetNamespace = "urn:schemas-microsoft-com:office:spreadsheet";
        private AusgabenRepository _repo = new AusgabenRepository();
        private FinanzEintrag _transaktionInBearbeitung;
        private Kategorie _kategorieInBearbeitung;
        private List<FinanzEintrag> _alleTransaktionen = new List<FinanzEintrag>();
        private List<Kategorie> _alleKategorien = new List<Kategorie>();
        private bool _filterWerdenAktualisiert;

        public MainWindow()
        {
            InitializeComponent();
            DatabaseManager.Instance.InitializeDatabase();
            RefreshAll();
        }

        private void RefreshAll()
        {
            try
            {
                int? ausgewaehlteKategorieId = cmbKategorie.SelectedValue as int?;
                int? filterKategorieId = cmbFilterKategorie.SelectedValue as int?;
                string filterMonat = cmbFilterMonat.SelectedItem as string;

                _alleKategorien = _repo.LadeAlleKategorien();
                _alleTransaktionen = _repo.LadeAlleTransaktionen();

                cmbKategorie.ItemsSource = _alleKategorien;
                lstKategorien.ItemsSource = _alleKategorien;
                WaehleKategorieAus(ausgewaehlteKategorieId);
                AktualisiereFilter(filterKategorieId, filterMonat);
                WendeFilterAn();

                lblGesamtEinnahmen.Text = _repo.LadeGesamtEinnahmen().ToString("N2") + " CHF";
                lblGesamtAusgaben.Text = _repo.LadeGesamtAusgaben().ToString("N2") + " CHF";
                lblSaldo.Text = _repo.LadeSaldo().ToString("N2") + " CHF";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void WaehleKategorieAus(int? kategorieId)
        {
            if (_alleKategorien.Count == 0) return;

            if (kategorieId.HasValue && _alleKategorien.Any(k => k.Id == kategorieId.Value))
            {
                cmbKategorie.SelectedValue = kategorieId.Value;
                return;
            }

            if (cmbKategorie.SelectedIndex == -1)
            {
                cmbKategorie.SelectedIndex = 0;
            }
        }

        private void AktualisiereFilter(int? filterKategorieId, string filterMonat)
        {
            _filterWerdenAktualisiert = true;

            var filterKategorien = new List<Kategorie> { new Kategorie { Id = 0, Name = "Alle Kategorien" } };
            filterKategorien.AddRange(_alleKategorien);
            cmbFilterKategorie.ItemsSource = filterKategorien;
            cmbFilterKategorie.SelectedValue = filterKategorieId.HasValue && filterKategorien.Any(k => k.Id == filterKategorieId.Value)
                ? filterKategorieId.Value
                : 0;

            var monate = new List<string> { "Alle Monate" };
            monate.AddRange(_alleTransaktionen.Select(t => t.MonatGruppe).Distinct());
            cmbFilterMonat.ItemsSource = monate;
            cmbFilterMonat.SelectedItem = !string.IsNullOrEmpty(filterMonat) && monate.Contains(filterMonat)
                ? filterMonat
                : "Alle Monate";

            _filterWerdenAktualisiert = false;
        }

        private void WendeFilterAn()
        {
            IEnumerable<FinanzEintrag> gefilterteTransaktionen = _alleTransaktionen;

            int kategorieId = cmbFilterKategorie.SelectedValue is int ? (int)cmbFilterKategorie.SelectedValue : 0;
            if (kategorieId > 0)
            {
                gefilterteTransaktionen = gefilterteTransaktionen.Where(t => t.KategorieId == kategorieId);
            }

            string monat = cmbFilterMonat.SelectedItem as string;
            if (!string.IsNullOrEmpty(monat) && monat != "Alle Monate")
            {
                gefilterteTransaktionen = gefilterteTransaktionen.Where(t => t.MonatGruppe == monat);
            }

            ICollectionView transaktionen = CollectionViewSource.GetDefaultView(gefilterteTransaktionen.ToList());
            transaktionen.GroupDescriptions.Clear();
            transaktionen.GroupDescriptions.Add(new PropertyGroupDescription("MonatGruppe"));
            lstAusgaben.ItemsSource = transaktionen;
        }

        private void KategorieFormularZuruecksetzen()
        {
            _kategorieInBearbeitung = null;
            txtKategorieName.Text = "";
            txtKategorieBeschreibung.Text = "";
            lstKategorien.SelectedItem = null;
            btnKategorieSpeichern.Content = "Kategorie speichern";
        }

        private void BtnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(txtBetrag.Text, out decimal betrag))
            {
                MessageBox.Show("Bitte gültigen Betrag eingeben!");
                return;
            }
            if (cmbKategorie.SelectedValue == null) return;

            bool istEinnahme = rbEinnahme.IsChecked == true;
            FinanzEintrag transaktion = istEinnahme ? (FinanzEintrag)new Einnahme() : new Ausgabe();
            transaktion.Id = _transaktionInBearbeitung?.Id ?? 0;
            transaktion.Betrag = Math.Abs(betrag);
            transaktion.Datum = dpDatum.SelectedDate ?? DateTime.Now;
            transaktion.Notiz = txtNotiz.Text;
            transaktion.KategorieId = (int)cmbKategorie.SelectedValue;

            if (_transaktionInBearbeitung == null)
            {
                if (istEinnahme)
                {
                    _repo.SpeichereEinnahme((Einnahme)transaktion);
                }
                else
                {
                    _repo.SpeichereAusgabe((Ausgabe)transaktion);
                }
            }
            else
            {
                _repo.AktualisiereTransaktion(transaktion, istEinnahme ? "Einnahme" : "Ausgabe");
            }

            FormularZuruecksetzen();
            RefreshAll();
        }

        private void BtnLoeschen_Click(object sender, RoutedEventArgs e)
        {
            var selected = lstAusgaben.SelectedItem as FinanzEintrag;
            if (selected == null)
            {
                MessageBox.Show("Bitte Eintrag auswählen!");
                return;
            }

            if (MessageBox.Show("Wirklich löschen?", "Bestätigen", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _repo.LoescheTransaktion(selected.Id);
                FormularZuruecksetzen();
                RefreshAll();
            }
        }

        private void BtnBeispieldaten_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int eingefuegt = _repo.FuegeBeispieldatenEin();
                RefreshAll();
                MessageBox.Show(
                    eingefuegt > 0
                        ? $"{eingefuegt} Beispieltransaktionen wurden eingefügt."
                        : "Die Beispieldaten sind bereits vorhanden.",
                    "Beispieldaten",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Beispieldaten konnten nicht eingefügt werden: " + ex.Message, "Beispieldaten", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_filterWerdenAktualisiert) return;
            WendeFilterAn();
        }

        private void BtnFilterZuruecksetzen_Click(object sender, RoutedEventArgs e)
        {
            cmbFilterKategorie.SelectedValue = 0;
            cmbFilterMonat.SelectedItem = "Alle Monate";
            WendeFilterAn();
        }

        private void BtnKategorieSpeichern_Click(object sender, RoutedEventArgs e)
        {
            string name = txtKategorieName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Bitte einen Kategorienamen eingeben.");
                return;
            }

            try
            {
                if (_kategorieInBearbeitung == null)
                {
                    _repo.SpeichereKategorie(new Kategorie
                    {
                        Name = name,
                        Beschreibung = txtKategorieBeschreibung.Text.Trim()
                    });
                }
                else
                {
                    _kategorieInBearbeitung.Name = name;
                    _kategorieInBearbeitung.Beschreibung = txtKategorieBeschreibung.Text.Trim();
                    _repo.AktualisiereKategorie(_kategorieInBearbeitung);
                }

                KategorieFormularZuruecksetzen();
                RefreshAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kategorie konnte nicht gespeichert werden: " + ex.Message);
            }
        }

        private void BtnKategorieNeu_Click(object sender, RoutedEventArgs e)
        {
            KategorieFormularZuruecksetzen();
        }

        private void BtnKategorieLoeschen_Click(object sender, RoutedEventArgs e)
        {
            var selected = lstKategorien.SelectedItem as Kategorie;
            if (selected == null)
            {
                MessageBox.Show("Bitte eine Kategorie auswählen.");
                return;
            }

            if (MessageBox.Show("Kategorie wirklich löschen?", "Bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                _repo.LoescheKategorie(selected.Id);
                KategorieFormularZuruecksetzen();
                RefreshAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Kategorie löschen", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LstKategorien_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = lstKategorien.SelectedItem as Kategorie;
            if (selected == null) return;

            _kategorieInBearbeitung = selected;
            txtKategorieName.Text = selected.Name;
            txtKategorieBeschreibung.Text = selected.Beschreibung;
            btnKategorieSpeichern.Content = "Kategorie ändern";
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var transaktionen = _repo.LadeAlleTransaktionen();
                var dialog = new SaveFileDialog
                {
                    Title = "Übersicht als Excel-Datei exportieren",
                    Filter = "Excel-Datei (*.xls)|*.xls",
                    FileName = "Transaktionen_Uebersicht_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xls"
                };

                if (dialog.ShowDialog() != true) return;

                ExportiereUebersichtNachExcel(dialog.FileName, transaktionen);
                MessageBox.Show("Die Übersicht wurde erfolgreich exportiert.", "Excel-Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export fehlgeschlagen: " + ex.Message, "Excel-Export", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportiereUebersichtNachExcel(string filePath, List<FinanzEintrag> transaktionen)
        {
            decimal einnahmen = transaktionen.Where(t => t is Einnahme).Sum(t => t.Betrag);
            decimal ausgaben = transaktionen.Where(t => t is Ausgabe).Sum(t => t.Betrag);
            decimal saldo = einnahmen - ausgaben;
            var settings = new XmlWriterSettings { Indent = true, Encoding = System.Text.Encoding.UTF8 };

            using (var writer = XmlWriter.Create(filePath, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Workbook", SpreadsheetNamespace);
                writer.WriteAttributeString("xmlns", "o", null, "urn:schemas-microsoft-com:office:office");
                writer.WriteAttributeString("xmlns", "x", null, "urn:schemas-microsoft-com:office:excel");
                writer.WriteAttributeString("xmlns", "ss", null, SpreadsheetNamespace);

                SchreibeExcelStyles(writer);

                writer.WriteStartElement("Worksheet", SpreadsheetNamespace);
                writer.WriteAttributeString("ss", "Name", SpreadsheetNamespace, "Transaktionen");
                writer.WriteStartElement("Table", SpreadsheetNamespace);

                SchreibeSpalten(writer);
                SchreibeTitel(writer);
                SchreibeKopfzeile(writer);

                foreach (var transaktion in transaktionen)
                {
                    writer.WriteStartElement("Row", SpreadsheetNamespace);
                    SchreibeZelle(writer, transaktion.Typ, "String", "Text");
                    SchreibeZelle(writer, transaktion.Datum.ToString("yyyy-MM-ddTHH:mm:ss.000", CultureInfo.InvariantCulture), "DateTime", "Date");
                    SchreibeZelle(writer, transaktion.KategorieName, "String", "Text");
                    SchreibeZelle(writer, transaktion.Notiz, "String", "Text");
                    SchreibeZelle(writer, transaktion.BerechneBetrag().ToString(CultureInfo.InvariantCulture), "Number", transaktion is Einnahme ? "Income" : "Expense");
                    writer.WriteEndElement();
                }

                SchreibeLeerzeile(writer, 5);
                SchreibeZusammenfassung(writer, "Einnahmen", einnahmen, "IncomeTotal");
                SchreibeZusammenfassung(writer, "Ausgaben", -ausgaben, "ExpenseTotal");
                SchreibeZusammenfassung(writer, "Saldo", saldo, "BalanceTotal");

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void SchreibeExcelStyles(XmlWriter writer)
        {
            writer.WriteStartElement("Styles", SpreadsheetNamespace);

            SchreibeStyle(writer, "Title", "#176B87", "#FFFFFF", true, 16, null, true);
            SchreibeStyle(writer, "Subtitle", "#E8EEF3", "#24313F", false, 10, null, false);
            SchreibeStyle(writer, "Header", "#24313F", "#FFFFFF", true, 11, null, true);
            SchreibeStyle(writer, "Text", null, "#24313F", false, 10, null, false);
            SchreibeStyle(writer, "Date", null, "#24313F", false, 10, "dd.mm.yyyy", false);
            SchreibeStyle(writer, "Income", null, "#15803D", false, 10, "+#,##0.00 \"CHF\";-#,##0.00 \"CHF\";0.00 \"CHF\"", false);
            SchreibeStyle(writer, "Expense", null, "#B42318", false, 10, "+#,##0.00 \"CHF\";-#,##0.00 \"CHF\";0.00 \"CHF\"", false);
            SchreibeStyle(writer, "IncomeTotal", "#EAF8EF", "#15803D", true, 11, "+#,##0.00 \"CHF\";-#,##0.00 \"CHF\";0.00 \"CHF\"", true);
            SchreibeStyle(writer, "ExpenseTotal", "#FFF1F0", "#B42318", true, 11, "+#,##0.00 \"CHF\";-#,##0.00 \"CHF\";0.00 \"CHF\"", true);
            SchreibeStyle(writer, "BalanceTotal", "#EEF4FF", "#1D4ED8", true, 11, "+#,##0.00 \"CHF\";-#,##0.00 \"CHF\";0.00 \"CHF\"", true);
            SchreibeStyle(writer, "TotalLabel", "#E8EEF3", "#24313F", true, 11, null, true);

            writer.WriteEndElement();
        }

        private void SchreibeStyle(XmlWriter writer, string id, string backgroundColor, string fontColor, bool bold, int size, string numberFormat, bool center)
        {
            writer.WriteStartElement("Style", SpreadsheetNamespace);
            writer.WriteAttributeString("ss", "ID", SpreadsheetNamespace, id);

            if (!string.IsNullOrEmpty(backgroundColor))
            {
                writer.WriteStartElement("Interior", SpreadsheetNamespace);
                writer.WriteAttributeString("ss", "Color", SpreadsheetNamespace, backgroundColor);
                writer.WriteAttributeString("ss", "Pattern", SpreadsheetNamespace, "Solid");
                writer.WriteEndElement();
            }

            writer.WriteStartElement("Font", SpreadsheetNamespace);
            writer.WriteAttributeString("ss", "Color", SpreadsheetNamespace, fontColor);
            writer.WriteAttributeString("ss", "Size", SpreadsheetNamespace, size.ToString(CultureInfo.InvariantCulture));
            if (bold) writer.WriteAttributeString("ss", "Bold", SpreadsheetNamespace, "1");
            writer.WriteEndElement();

            writer.WriteStartElement("Alignment", SpreadsheetNamespace);
            writer.WriteAttributeString("ss", "Vertical", SpreadsheetNamespace, "Center");
            writer.WriteAttributeString("ss", "WrapText", SpreadsheetNamespace, "1");
            if (center) writer.WriteAttributeString("ss", "Horizontal", SpreadsheetNamespace, "Center");
            writer.WriteEndElement();

            writer.WriteStartElement("Borders", SpreadsheetNamespace);
            string[] positions = { "Bottom", "Left", "Right", "Top" };
            foreach (string position in positions)
            {
                writer.WriteStartElement("Border", SpreadsheetNamespace);
                writer.WriteAttributeString("ss", "Position", SpreadsheetNamespace, position);
                writer.WriteAttributeString("ss", "LineStyle", SpreadsheetNamespace, "Continuous");
                writer.WriteAttributeString("ss", "Weight", SpreadsheetNamespace, "1");
                writer.WriteAttributeString("ss", "Color", SpreadsheetNamespace, "#D8E0E7");
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            if (!string.IsNullOrEmpty(numberFormat))
            {
                writer.WriteStartElement("NumberFormat", SpreadsheetNamespace);
                writer.WriteAttributeString("ss", "Format", SpreadsheetNamespace, numberFormat);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private void SchreibeSpalten(XmlWriter writer)
        {
            SchreibeSpalte(writer, 90);
            SchreibeSpalte(writer, 95);
            SchreibeSpalte(writer, 140);
            SchreibeSpalte(writer, 260);
            SchreibeSpalte(writer, 120);
        }

        private void SchreibeSpalte(XmlWriter writer, int width)
        {
            writer.WriteStartElement("Column", SpreadsheetNamespace);
            writer.WriteAttributeString("ss", "Width", SpreadsheetNamespace, width.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();
        }

        private void SchreibeTitel(XmlWriter writer)
        {
            writer.WriteStartElement("Row", SpreadsheetNamespace);
            writer.WriteAttributeString("ss", "Height", SpreadsheetNamespace, "28");
            writer.WriteStartElement("Cell", SpreadsheetNamespace);
            writer.WriteAttributeString("ss", "MergeAcross", SpreadsheetNamespace, "4");
            writer.WriteAttributeString("ss", "StyleID", SpreadsheetNamespace, "Title");
            writer.WriteStartElement("Data", SpreadsheetNamespace);
            writer.WriteAttributeString("ss", "Type", SpreadsheetNamespace, "String");
            writer.WriteString("Transaktionen Übersicht");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("Row", SpreadsheetNamespace);
            writer.WriteStartElement("Cell", SpreadsheetNamespace);
            writer.WriteAttributeString("ss", "MergeAcross", SpreadsheetNamespace, "4");
            writer.WriteAttributeString("ss", "StyleID", SpreadsheetNamespace, "Subtitle");
            writer.WriteStartElement("Data", SpreadsheetNamespace);
            writer.WriteAttributeString("ss", "Type", SpreadsheetNamespace, "String");
            writer.WriteString("Exportiert am " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void SchreibeKopfzeile(XmlWriter writer)
        {
            writer.WriteStartElement("Row", SpreadsheetNamespace);
            SchreibeZelle(writer, "Typ", "String", "Header");
            SchreibeZelle(writer, "Datum", "String", "Header");
            SchreibeZelle(writer, "Kategorie", "String", "Header");
            SchreibeZelle(writer, "Notiz", "String", "Header");
            SchreibeZelle(writer, "Betrag", "String", "Header");
            writer.WriteEndElement();
        }

        private void SchreibeZusammenfassung(XmlWriter writer, string label, decimal betrag, string styleId)
        {
            writer.WriteStartElement("Row", SpreadsheetNamespace);
            SchreibeZelle(writer, "", "String", "Text");
            SchreibeZelle(writer, "", "String", "Text");
            SchreibeZelle(writer, "", "String", "Text");
            SchreibeZelle(writer, label, "String", "TotalLabel");
            SchreibeZelle(writer, betrag.ToString(CultureInfo.InvariantCulture), "Number", styleId);
            writer.WriteEndElement();
        }

        private void SchreibeLeerzeile(XmlWriter writer, int spalten)
        {
            writer.WriteStartElement("Row", SpreadsheetNamespace);
            for (int i = 0; i < spalten; i++)
            {
                SchreibeZelle(writer, "", "String", "Text");
            }
            writer.WriteEndElement();
        }

        private void SchreibeZelle(XmlWriter writer, string value, string dataType, string styleId)
        {
            writer.WriteStartElement("Cell", SpreadsheetNamespace);
            writer.WriteAttributeString("ss", "StyleID", SpreadsheetNamespace, styleId);
            writer.WriteStartElement("Data", SpreadsheetNamespace);
            writer.WriteAttributeString("ss", "Type", SpreadsheetNamespace, dataType);
            writer.WriteString(value ?? "");
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void LstAusgaben_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = lstAusgaben.SelectedItem as FinanzEintrag;
            if (selected == null) return;

            _transaktionInBearbeitung = selected;
            txtBetrag.Text = selected.Betrag.ToString("N2");
            dpDatum.SelectedDate = selected.Datum;
            txtNotiz.Text = selected.Notiz;
            cmbKategorie.SelectedValue = selected.KategorieId;
            rbEinnahme.IsChecked = selected is Einnahme;
            rbAusgabe.IsChecked = selected is Ausgabe;
            txtFormularTitel.Text = "Transaktion bearbeiten";
            btnSpeichern.Content = "Änderungen speichern";
            btnAbbrechen.Visibility = Visibility.Visible;
        }

        private void BtnAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            FormularZuruecksetzen();
        }

        private void FormularZuruecksetzen()
        {
            _transaktionInBearbeitung = null;
            txtBetrag.Text = "";
            txtNotiz.Text = "";
            dpDatum.SelectedDate = DateTime.Now;
            rbAusgabe.IsChecked = true;
            if (cmbKategorie.Items.Count > 0) cmbKategorie.SelectedIndex = 0;
            lstAusgaben.SelectedItem = null;
            txtFormularTitel.Text = "Neue Transaktion";
            btnSpeichern.Content = "Speichern";
            btnAbbrechen.Visibility = Visibility.Collapsed;
        }
    }

    public class MonthGroupSummaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var group = value as CollectionViewGroup;
            if (group == null) return "";

            var transaktionen = GetTransaktionen(group).ToList();
            decimal einnahmen = transaktionen.OfType<Einnahme>().Sum(t => t.Betrag);
            decimal ausgaben = transaktionen.OfType<Ausgabe>().Sum(t => t.Betrag);
            decimal saldo = einnahmen - ausgaben;

            return string.Format(
                culture,
                "Einnahmen {0:N2} CHF   Ausgaben {1:N2} CHF   Saldo {2:+0.00;-0.00;0.00} CHF",
                einnahmen,
                ausgaben,
                saldo);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static IEnumerable<FinanzEintrag> GetTransaktionen(CollectionViewGroup group)
        {
            foreach (object item in group.Items)
            {
                var subgroup = item as CollectionViewGroup;
                if (subgroup != null)
                {
                    foreach (var transaktion in GetTransaktionen(subgroup))
                    {
                        yield return transaktion;
                    }

                    continue;
                }

                var transaktionItem = item as FinanzEintrag;
                if (transaktionItem != null)
                {
                    yield return transaktionItem;
                }
            }
        }
    }
}
