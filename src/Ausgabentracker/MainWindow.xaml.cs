using Ausgabentracker.Database;
using Ausgabentracker.Models;
using Ausgabentracker.Repositories;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ausgabentracker
{
    public partial class MainWindow : Window
    {
        private AusgabenRepository _repo = new AusgabenRepository();
        private FinanzEintrag _transaktionInBearbeitung;

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
                cmbKategorie.ItemsSource = _repo.LadeAlleKategorien();
                if (cmbKategorie.Items.Count > 0 && cmbKategorie.SelectedIndex == -1) cmbKategorie.SelectedIndex = 0;

                ICollectionView transaktionen = CollectionViewSource.GetDefaultView(_repo.LadeAlleTransaktionen());
                transaktionen.GroupDescriptions.Clear();
                transaktionen.GroupDescriptions.Add(new PropertyGroupDescription("MonatGruppe"));
                lstAusgaben.ItemsSource = transaktionen;

                lblGesamtEinnahmen.Text = _repo.LadeGesamtEinnahmen().ToString("N2") + " CHF";
                lblGesamtAusgaben.Text = _repo.LadeGesamtAusgaben().ToString("N2") + " CHF";
                lblSaldo.Text = _repo.LadeSaldo().ToString("N2") + " CHF";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
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
}
