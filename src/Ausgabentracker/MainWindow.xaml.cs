using System;
using System.Windows;
using Ausgabentracker.Database;
using Ausgabentracker.Repositories;
using Ausgabentracker.Models;

namespace Ausgabentracker
{
    public partial class MainWindow : Window
    {
        private AusgabenRepository _repo = new AusgabenRepository();

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

                lstAusgaben.ItemsSource = _repo.LadeAlleAusgaben();
                lblGesamtSumme.Text = _repo.LadeGesamtSumme().ToString("N2") + " CHF";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(txtBetrag.Text, out decimal betrag))
            {
                MessageBox.Show("Bitte gültigen Betrag eingeben!"); return;
            }
            if (cmbKategorie.SelectedValue == null) return;

            var neueAusgabe = new Ausgabe
            {
                Betrag = betrag,
                Datum = dpDatum.SelectedDate ?? DateTime.Now,
                Notiz = txtNotiz.Text,
                KategorieId = (int)cmbKategorie.SelectedValue
            };

            _repo.SpeichereAusgabe(neueAusgabe);
            txtBetrag.Text = ""; txtNotiz.Text = "";
            RefreshAll();
        }

        private void BtnLoeschen_Click(object sender, RoutedEventArgs e)
        {
            var selected = lstAusgaben.SelectedItem as Ausgabe;
            if (selected == null) { MessageBox.Show("Bitte Eintrag auswählen!"); return; }

            if (MessageBox.Show("Wirklich löschen?", "Bestätigen", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _repo.LoescheAusgabe(selected.Id);
                RefreshAll();
            }
        }
    }
}