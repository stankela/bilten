﻿using Bilten.Dao;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;
using NHibernate;
using NHibernate.Context;
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
        private List<Jezik> jezici;
        private int currentIndex;
        
        public JezikForm()
        {
            InitializeComponent();
            Text = "Jezik";

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    jezici = new List<Jezik>(DAOFactoryFactory.DAOFactory.GetJezikDAO().FindAll());
                    initUI();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void initUI()
        {
            cmbJezik.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbJezik.DisplayMember = "Naziv";
            cmbJezik.Items.AddRange(jezici.ToArray());
            currentIndex = -1;
            cmbJezik.SelectedIndex = GetSelectedIndex(Opcije.Instance.Jezik);
        }

        private int GetSelectedIndex(string jezik)
        {
            for (int i = 0; i < jezici.Count; ++i)
            {
                if (jezici[i].Naziv == jezik)
                    return i;
            }
            return -1;
        }

        private void cmbJezik_SelectedIndexChanged(object sender, EventArgs e)
        {
            // TODO4: Najpre uradi validaciju (da li je neki textbox ostao prazan), i prijavi gresku (i vrati kombo na
            // prethodnu vrednost).
            if (currentIndex != -1)
            {
                updateJezikFromUI(jezici[currentIndex]);
            }
            currentIndex = cmbJezik.SelectedIndex;
            if (currentIndex != -1)
            {
                updateUIFromJezik(jezici[currentIndex]);
            }
        }

        private void updateUIFromJezik(Jezik jezik)
        {
            txtRedBroj.Text = jezik.RedBroj;
            txtRank.Text = jezik.Rank;
            txtIme.Text = jezik.Ime;
            txtKlubDrzava.Text = jezik.KlubDrzava;
            txtKategorija.Text = jezik.Kategorija;
            txtTotal.Text = jezik.Ukupno;
            txtOcena.Text = jezik.Ocena;
            txtRezerve.Text = jezik.Rezerve;
            txtBroj.Text = jezik.Broj;
            txtPrezime.Text = jezik.Prezime;
            txtDatumRodjenja.Text = jezik.DatumRodjenja;
            txtKlub.Text = jezik.Klub;
            txtDrzava.Text = jezik.Drzava;
            txtEkipa.Text = jezik.Ekipa;
            txtStrana.Text = jezik.Strana;
            txtVrhovniSudijaMuski.Text = jezik.VrhovniSudijaMuski;
            txtVrhovniSudijaZenski.Text = jezik.VrhovniSudijaZenski;
            txtViseboj.Text = jezik.Viseboj;
            txtKvalSprave.Text = jezik.KvalSprave;
            txtFinalistiSprave.Text = jezik.SpraveFinalisti;
            txtFinaleSprave.Text = jezik.FinaleSprave;
            txtEkipeRezultati.Text = jezik.EkipeRezultati;
            txtKvalStartListe.Text = jezik.KvalStartListe;
            txtFinaleStartListe.Text = jezik.FinaleStartListe;
            txtRasporedSudija.Text = jezik.RasporedSudija;
            txtRotacija.Text = jezik.Rotacija;
        }

        private void updateJezikFromUI(Jezik jezik)
        {
            jezik.RedBroj = txtRedBroj.Text;
            jezik.Rank = txtRank.Text;
            jezik.Ime = txtIme.Text;
            jezik.KlubDrzava = txtKlubDrzava.Text;
            jezik.Kategorija = txtKategorija.Text;
            jezik.Ukupno = txtTotal.Text;
            jezik.Ocena = txtOcena.Text;
            jezik.Rezerve = txtRezerve.Text;
            jezik.Broj = txtBroj.Text;
            jezik.Prezime = txtPrezime.Text;
            jezik.DatumRodjenja = txtDatumRodjenja.Text;
            jezik.Klub = txtKlub.Text;
            jezik.Drzava = txtDrzava.Text;
            jezik.Ekipa = txtEkipa.Text;
            jezik.Strana = txtStrana.Text;
            jezik.VrhovniSudijaMuski = txtVrhovniSudijaMuski.Text;
            jezik.VrhovniSudijaZenski = txtVrhovniSudijaZenski.Text;
            jezik.Viseboj = txtViseboj.Text;
            jezik.KvalSprave = txtKvalSprave.Text;
            jezik.SpraveFinalisti = txtFinalistiSprave.Text;
            jezik.FinaleSprave = txtFinaleSprave.Text;
            jezik.EkipeRezultati = txtEkipeRezultati.Text;
            jezik.KvalStartListe = txtKvalStartListe.Text;
            jezik.FinaleStartListe = txtFinaleStartListe.Text;
            jezik.RasporedSudija = txtRasporedSudija.Text;
            jezik.Rotacija = txtRotacija.Text;
        }

        private void JezikForm_Shown(object sender, EventArgs e)
        {
            lblRedBroj.Focus();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Validate

            if (currentIndex == -1)
            {
                MessageDialogs.showMessage("Izaberite jezik", this.Text);
                cmbJezik.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

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
            else if (txtKlubDrzava.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za klub.";
                txtBox = txtKlubDrzava;
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
            else if (txtBroj.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za broj.";
                txtBox = txtBroj;
            }
            else if (txtPrezime.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za prezime.";
                txtBox = txtPrezime;
            }
            else if (txtDatumRodjenja.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za datum rodjenja.";
                txtBox = txtDatumRodjenja;
            }
            else if (txtKlub.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za klub.";
                txtBox = txtKlub;
            }
            else if (txtDrzava.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za drzavu.";
                txtBox = txtDrzava;
            }
            else if (txtEkipa.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za ekipu.";
                txtBox = txtEkipa;
            }
            else if (txtStrana.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za stranu.";
                txtBox = txtStrana;
            }
            else if (txtVrhovniSudijaMuski.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za vrhovnog sudiju.";
                txtBox = txtVrhovniSudijaMuski;
            }
            else if (txtVrhovniSudijaZenski.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za vrhovnu sutkinju.";
                txtBox = txtVrhovniSudijaZenski;
            }
            else if (txtViseboj.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za viseboj.";
                txtBox = txtViseboj;
            }
            else if (txtKvalSprave.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za kvalifikacije za finale po spravama.";
                txtBox = txtKvalSprave;
            }
            else if (txtFinalistiSprave.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za finaliste po spravama.";
                txtBox = txtFinalistiSprave;
            }
            else if (txtFinaleSprave.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za finale po spravama.";
                txtBox = txtFinaleSprave;
            }
            else if (txtEkipeRezultati.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za rezultate ekipno.";
                txtBox = txtEkipeRezultati;
            }
            else if (txtKvalStartListe.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za start liste za kvalifikacije.";
                txtBox = txtKvalStartListe;
            }
            else if (txtFinaleStartListe.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za start liste za finale.";
                txtBox = txtFinaleStartListe;
            }
            else if (txtRasporedSudija.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za raspored sudija.";
                txtBox = txtRasporedSudija;
            }
            else if (txtRotacija.Text.Trim() == String.Empty)
            {
                msg = "Unesite tekst za rotaciju.";
                txtBox = txtRotacija;
            }

            if (msg != String.Empty)
            {
                MessageDialogs.showMessage(msg, this.Text);
                txtBox.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            foreach (Jezik j in jezici)
            {
                j.Default = false;
            }
            updateJezikFromUI(jezici[currentIndex]);
            jezici[currentIndex].Default = true;

            // TODO4: Proveri jos jednom ceo ovaj JezikForm, jer je sve radjeno na brzinu.

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    foreach (Jezik j in jezici)
                    {
                        DAOFactoryFactory.DAOFactory.GetJezikDAO().Update(j);
                    }
                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showMessage(ex.Message, this.Text);
                this.DialogResult = DialogResult.None;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            // Update options
            Opcije.Instance.UpdateJezik(jezici[currentIndex]);
        }
    }
}
