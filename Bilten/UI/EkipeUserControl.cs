using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Bilten.UI
{
    public partial class EkipeUserControl : UserControl
    {
        public EkipeUserControl()
        {
            InitializeComponent();

            dgwUserControlEkipe.AddColumn("Naziv ekipe", "Naziv", 200);
            dgwUserControlEkipe.AddColumn("Skraceni kod", "Kod", 100);

            dgwUserControlClanovi.AddColumn("Ime", "Ime", 100);
            dgwUserControlClanovi.AddColumn("Prezime", "Prezime", 100);
            dgwUserControlClanovi.AddColumn("Datum rodjenja", "DatumRodjenja", 100, "{0:d}");
            dgwUserControlClanovi.AddColumn("Takmicarski broj", "TakmicarskiBroj", 100);
            dgwUserControlClanovi.AddColumn("Klub", "KlubUcesnik", 150);
            dgwUserControlClanovi.AddColumn("Drzava", "DrzavaUcesnik", 100);
        }

        public DataGridViewUserControl EkipeDataGridViewUserControl
        {
            get { return dgwUserControlEkipe; }
        }

        public DataGridViewUserControl ClanoviDataGridViewUserControl
        {
            get { return dgwUserControlClanovi; }
        }
    }
}
