using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Data;
using Bilten.Exceptions;
using Bilten.Util;
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class FilterGimnasticarUcesnikUserControl : UserControl
    {
        private int takmicenjeId;
        private Gimnastika gimnastika;

        private readonly string SVE_KATEGORIJE = "SVE KATEGORIJE";
        private readonly string SVI_KLUBOVI = "SVI KLUBOVI";
        private readonly string SVE_DRZAVE = "SVE DRZAVE";

        public event EventHandler Filter;
        private bool generateFilterEvent = true;

        public Point btnPonistiLocation
        {
            get { return btnPonisti.Location; }
        }
        
        public FilterGimnasticarUcesnikUserControl()
        {
            InitializeComponent();
        }

        public void initialize(int takmicenjeId, Gimnastika gimnastika, TakmicarskaKategorija startKategorija)
        {
            this.takmicenjeId = takmicenjeId;
            this.gimnastika = gimnastika;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<TakmicarskaKategorija> kategorije
                        = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().FindByTakmicenje(takmicenjeId);
                    IList<KlubUcesnik> klubovi
                        = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO().FindByTakmicenje(takmicenjeId);
                    IList<DrzavaUcesnik> drzave
                        = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO().FindByTakmicenje(takmicenjeId);

                    initUI(kategorije, klubovi, drzave, startKategorija);
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

        private void initUI(IList<TakmicarskaKategorija> kategorije, IList<KlubUcesnik> klubovi,
            IList<DrzavaUcesnik> drzave, TakmicarskaKategorija startKategorija)
        {
            cmbKategorija.DropDownStyle = ComboBoxStyle.DropDown;
            cmbKategorija.Items.Add(SVE_KATEGORIJE);
            cmbKategorija.Items.AddRange(new List<TakmicarskaKategorija>(kategorije).ToArray());
            cmbKategorija.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbKategorija.AutoCompleteSource = AutoCompleteSource.ListItems;

            cmbKlub.DropDownStyle = ComboBoxStyle.DropDown;
            cmbKlub.Items.Add(SVI_KLUBOVI);
            cmbKlub.Items.AddRange(new List<KlubUcesnik>(klubovi).ToArray());
            cmbKlub.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbKlub.AutoCompleteSource = AutoCompleteSource.ListItems;

            cmbDrzava.DropDownStyle = ComboBoxStyle.DropDown;
            cmbDrzava.Items.Add(SVE_DRZAVE);
            cmbDrzava.Items.AddRange(new List<DrzavaUcesnik>(drzave).ToArray());
            cmbDrzava.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbDrzava.AutoCompleteSource = AutoCompleteSource.ListItems;

            generateFilterEvent = false;
            resetFilter(startKategorija);
            generateFilterEvent = true;
        }

        public void resetFilter(TakmicarskaKategorija startKategorija)
        {
            txtIme.Text = String.Empty;
            txtPrezime.Text = String.Empty;
            if (startKategorija == null)
                cmbKategorija.SelectedIndex = cmbKategorija.Items.IndexOf(SVE_KATEGORIJE);
            else
                cmbKategorija.SelectedItem = startKategorija;
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

                case "Kategorija":
                    cmbKategorija.Focus();
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

        public GimnasticarUcesnikFilter getFilter()
        {
            if (!validateFilter())
                return null;

            GimnasticarUcesnikFilter result = new GimnasticarUcesnikFilter();
            result.Ime = txtIme.Text.Trim();
            result.Prezime = txtPrezime.Text.Trim();
            result.Kategorija = cmbKategorija.SelectedItem as TakmicarskaKategorija;
            result.Klub = cmbKlub.SelectedItem as KlubUcesnik;
            result.Drzava = cmbDrzava.SelectedItem as DrzavaUcesnik;
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

        private void cmbKategorija_SelectedIndexChanged(object sender, EventArgs e)
        {
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

        private void btnPonisti_Click(object sender, EventArgs e)
        {
            generateFilterEvent = false;
            resetFilter(null);
            generateFilterEvent = true;
            OnFilter(EventArgs.Empty);
        }
    }
}
