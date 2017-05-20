using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;
using Bilten.Util;
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class FilterSudijaUserControl : UserControl
    {
        private readonly string SVI_KLUBOVI = "SVI KLUBOVI";
        private readonly string SVE_DRZAVE = "SVE DRZAVE";
        private readonly string SVI = "Svi";
        private readonly string MUSKI = "MUSKI";
        private readonly string ZENSKI = "ZENSKI";

        public event EventHandler Filter;
        private bool generateFilterEvent = true;

        public FilterSudijaUserControl()
        {
            InitializeComponent();
        }

        public void initialize()
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<Klub> klubovi = DAOFactoryFactory.DAOFactory.GetKlubDAO().FindAll();
                    IList<Drzava> drzave = DAOFactoryFactory.DAOFactory.GetDrzavaDAO().FindAll();
                    initUI(klubovi, drzave);
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

        private void initUI(IList<Klub> klubovi, IList<Drzava> drzave)
        {
            cmbKlub.DropDownStyle = ComboBoxStyle.DropDown;
            cmbKlub.Items.Add(SVI_KLUBOVI);
            cmbKlub.Items.AddRange(new List<Klub>(klubovi).ToArray());
            cmbKlub.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbKlub.AutoCompleteSource = AutoCompleteSource.ListItems;

            cmbDrzava.DropDownStyle = ComboBoxStyle.DropDown;
            cmbDrzava.Items.Add(SVE_DRZAVE);
            cmbDrzava.Items.AddRange(new List<Drzava>(drzave).ToArray());
            cmbDrzava.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbDrzava.AutoCompleteSource = AutoCompleteSource.ListItems;

            cmbPol.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPol.Items.AddRange(new string[] { SVI, MUSKI, ZENSKI });

            generateFilterEvent = false;
            resetFilter();
            generateFilterEvent = true;
        }

        public void resetFilter()
        {
            txtIme.Text = String.Empty;
            txtPrezime.Text = String.Empty;
            cmbPol.SelectedIndex = cmbPol.Items.IndexOf(SVI);
            cmbKlub.SelectedIndex = cmbKlub.Items.IndexOf(SVI_KLUBOVI);
            cmbDrzava.SelectedIndex = cmbDrzava.Items.IndexOf(SVE_DRZAVE);
        }

        private bool validateFilter()
        {
            return true;
        }

        private void setFocus(string propertyName)
        {
            switch (propertyName)
            {
                case "Ime":
                    txtIme.Focus();
                    break;

                case "Prezime":
                    txtPrezime.Focus();
                    break;

                case "Pol":
                    cmbPol.Focus();
                    break;

                case "Klub":
                    cmbKlub.Focus();
                    break;

                case "Drzava":
                    cmbDrzava.Focus();
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        public SudijaFilter getFilter()
        {
            if (!validateFilter())
                return null;

            SudijaFilter result = new SudijaFilter();
            result.Ime = txtIme.Text.Trim();
            result.Prezime = txtPrezime.Text.Trim();

            if (cmbPol.SelectedIndex == cmbPol.Items.IndexOf(MUSKI))
                result.Pol = Pol.Muski;
            else if (cmbPol.SelectedIndex == cmbPol.Items.IndexOf(ZENSKI))
                result.Pol = Pol.Zenski;

            result.Drzava = cmbDrzava.SelectedItem as Drzava;
            result.Klub = cmbKlub.SelectedItem as Klub;
            return result;
        }

        protected virtual void OnFilter(EventArgs e)
        {
            if (!generateFilterEvent)
                return;
            EventHandler tmp = Filter;
            if (tmp != null)
                Filter(this, e);
        }

        private void txtIme_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                OnFilter(EventArgs.Empty);
        }

        private void txtPrezime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                OnFilter(EventArgs.Empty);
        }

        private void cmbKlub_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnFilter(EventArgs.Empty);
        }

        private void cmbDrzava_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnFilter(EventArgs.Empty);
        }

        private void cmbPol_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnFilter(EventArgs.Empty);
        }

        private void btnPonisti_Click(object sender, EventArgs e)
        {
            generateFilterEvent = false;
            resetFilter();
            generateFilterEvent = true;
            OnFilter(EventArgs.Empty);
        }
    }
}
