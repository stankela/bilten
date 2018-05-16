using Bilten.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Bilten.UI
{
    public partial class JezikForm : Form
    {
        public JezikForm()
        {
            InitializeComponent();
            Text = "Jezik";

            txtRedBroj.Text = Opcije.Instance.RedBrojString;
            txtRank.Text = Opcije.Instance.RankString;
            txtIme.Text = Opcije.Instance.ImeString;
            txtKlub.Text = Opcije.Instance.KlubDrzavaString;
            txtKategorija.Text = Opcije.Instance.KategorijaString;
            txtTotal.Text = Opcije.Instance.TotalString;
            txtOcena.Text = Opcije.Instance.OcenaString;
            txtRezerve.Text = Opcije.Instance.RezerveString;
        }

        private void JezikForm_Shown(object sender, EventArgs e)
        {
            lblRedBroj.Focus();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Validate

            string msg = String.Empty;
            TextBox txtBox = null;
            if (txtRedBroj.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za redni broj.";
                txtBox = txtRedBroj;
            }
            else if (txtRank.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za rank.";
                txtBox = txtRank;
            }
            else if (txtIme.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za ime.";
                txtBox = txtIme;
            }
            else if (txtKlub.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za klub.";
                txtBox = txtKlub;
            }
            else if (txtKategorija.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za kategoriju.";
                txtBox = txtKategorija;
            }
            else if (txtTotal.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za ukupno.";
                txtBox = txtTotal;
            }
            else if (txtOcena.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za ocenu.";
                txtBox = txtOcena;
            }
            else if (txtRezerve.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za rezerve.";
                txtBox = txtRezerve;
            }

            if (msg != String.Empty)
            {
                MessageDialogs.showMessage(msg, this.Text);
                txtBox.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            // Update options

            Opcije.Instance.RedBrojString = txtRedBroj.Text.Trim();
            Opcije.Instance.RankString = txtRank.Text.Trim();
            Opcije.Instance.ImeString = txtIme.Text.Trim();
            Opcije.Instance.KlubDrzavaString = txtKlub.Text.Trim();
            Opcije.Instance.KategorijaString = txtKategorija.Text.Trim();
            Opcije.Instance.TotalString = txtTotal.Text.Trim();
            Opcije.Instance.OcenaString = txtOcena.Text.Trim();
            Opcije.Instance.RezerveString = txtRezerve.Text.Trim();
        }
    }
}
