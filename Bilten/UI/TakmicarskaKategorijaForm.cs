using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Exceptions;
using Bilten.Util;
using NHibernate;
using Bilten.Data;
using NHibernate.Context;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class TakmicarskaKategorijaForm : Form
    {
        private Takmicenje takmicenje;
        public string NazivKategorije;

        public TakmicarskaKategorijaForm(Gimnastika gimnastika)
        {
            InitializeComponent();
            this.Text = "Izaberite kategoriju";

            IList<KategorijaGimnasticara> kategorije;
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    kategorije = DAOFactoryFactory.DAOFactory.GetKategorijaGimnasticaraDAO().FindByGimnastika(gimnastika);
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showMessage(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                Close();
                return;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            cmbKategorija.DropDownStyle = ComboBoxStyle.DropDown;
            setKategorije(kategorije);
            SelectedKategorija = null;
            cmbKategorija.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbKategorija.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void setKategorije(IList<KategorijaGimnasticara> kategorije)
        {
            cmbKategorija.DisplayMember = "Naziv";
            cmbKategorija.DataSource = kategorije;
        }

        private KategorijaGimnasticara SelectedKategorija
        {
            get { return cmbKategorija.SelectedItem as KategorijaGimnasticara; }
            set { cmbKategorija.SelectedItem = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cmbKategorija.Text == String.Empty)
            {
                MessageDialogs.showMessage("Izaberite ili unesite kategoriju", this.Text);
                DialogResult = DialogResult.None;
                return;
            }
            NazivKategorije = cmbKategorija.Text;
        }
    }
}