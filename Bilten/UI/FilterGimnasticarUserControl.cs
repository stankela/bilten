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
    public partial class FilterGimnasticarUserControl : UserControl
    {
        private Nullable<Gimnastika> gimnastika;

        private readonly string SVE_KATEGORIJE = "SVE KATEGORIJE";
        private readonly string SVI_KLUBOVI = "SVI KLUBOVI";
        private readonly string SVE_DRZAVE = "SVE DRZAVE";
        private readonly string SVI = "Svi";
        private readonly string MSG = "MSG";
        private readonly string ZSG = "ZSG";

        public event EventHandler Filter;
        private bool generateFilterEvent = true;

        public Point btnPonistiLocation
        { 
            get { return btnPonisti.Location; }
        }

        public FilterGimnasticarUserControl()
        {
            InitializeComponent();
        }

        public void initialize(Nullable<Gimnastika> gimnastika)
        {
            this.gimnastika = gimnastika;
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<KategorijaGimnasticara> kategorije = loadKategorije(gimnastika);
                    IList<Klub> klubovi = DAOFactoryFactory.DAOFactory.GetKlubDAO().FindAll();
                    IList<Drzava> drzave = DAOFactoryFactory.DAOFactory.GetDrzavaDAO().FindAll();

                    initUI(kategorije, klubovi, drzave);

                    // Stavljam ovo na kraj da se ne bi dvaput ucitavale kategorije
                    cmbGimnastika.SelectedIndexChanged += cmbGimnastika_SelectedIndexChanged;
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

        private IList<KategorijaGimnasticara> loadKategorije(Nullable<Gimnastika> gimnastika)
        {
            if (gimnastika != null)
                return DAOFactoryFactory.DAOFactory.GetKategorijaGimnasticaraDAO().FindByGimnastika(gimnastika.Value);
            else
                return DAOFactoryFactory.DAOFactory.GetKategorijaGimnasticaraDAO().FindAll();
        }

        private void initUI(IList<KategorijaGimnasticara> kategorije, IList<Klub> klubovi, IList<Drzava> drzave)
        {
            cmbKategorija.DropDownStyle = ComboBoxStyle.DropDown;
            setKategorije(kategorije);
            cmbKategorija.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbKategorija.AutoCompleteSource = AutoCompleteSource.ListItems;

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

            cmbGimnastika.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGimnastika.Items.AddRange(new string[] { SVI, MSG, ZSG });

            generateFilterEvent = false;
            resetFilter();
            generateFilterEvent = true;
        }

        private void setKategorije(IList<KategorijaGimnasticara> kategorije)
        {
            cmbKategorija.Items.Clear();
            cmbKategorija.Items.Add(SVE_KATEGORIJE);
            cmbKategorija.Items.AddRange(new List<KategorijaGimnasticara>(kategorije).ToArray());
        }

        public void resetFilter()
        {
            txtIme.Text = String.Empty;
            txtPrezime.Text = String.Empty;
            txtGodRodj.Text = String.Empty;

            if (gimnastika == null)
                cmbGimnastika.SelectedIndex = cmbGimnastika.Items.IndexOf(SVI);
            else
            {
                cmbGimnastika.Enabled = false;
                if (gimnastika.Value == Gimnastika.MSG)
                    cmbGimnastika.SelectedIndex = cmbGimnastika.Items.IndexOf(MSG);
                else
                    cmbGimnastika.SelectedIndex = cmbGimnastika.Items.IndexOf(ZSG);
            }
            cmbKategorija.SelectedIndex = cmbKategorija.Items.IndexOf(SVE_KATEGORIJE);
            cmbKlub.SelectedIndex = cmbKlub.Items.IndexOf(SVI_KLUBOVI);
            cmbDrzava.SelectedIndex = cmbDrzava.Items.IndexOf(SVE_DRZAVE);
        }

        private bool validateFilter()
        {
            Notification notification = new Notification();
            int dummyInt;

            if (txtGodRodj.Text.Trim() != String.Empty && !int.TryParse(txtGodRodj.Text, out dummyInt))
                notification.RegisterMessage("GodinaRodjenja", "Neispravan format za godinu rodjenja.");
            if (!notification.IsValid())
            {
                NotificationMessage msg = notification.FirstMessage;
                MessageDialogs.showMessage(msg.Message, String.Empty);
                setFocus(msg.FieldName);
                return false;
            }
            else
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

                case "GodinaRodjenja":
                    txtGodRodj.Focus();
                    break;

                case "Gimnastika":
                    cmbGimnastika.Focus();
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

        public GimnasticarFilter getFilter()
        {
            if (!validateFilter())
                return null;

            GimnasticarFilter result = new GimnasticarFilter();
            result.Ime = txtIme.Text.Trim();
            result.Prezime = txtPrezime.Text.Trim();
            if (txtGodRodj.Text.Trim() != String.Empty)
                result.GodRodj = int.Parse(txtGodRodj.Text);
            if (this.gimnastika == null)
            {
                if (cmbGimnastika.SelectedIndex == cmbGimnastika.Items.IndexOf(MSG))
                    result.Gimnastika = Gimnastika.MSG;
                else if (cmbGimnastika.SelectedIndex == cmbGimnastika.Items.IndexOf(ZSG))
                    result.Gimnastika = Gimnastika.ZSG;
            }
            else
                result.Gimnastika = this.gimnastika.Value;

            // NOTE: operator as vraca null ako sa leve strane nije objekt
            // odgovarajuceg tipa, ili je sa leve strane null
            result.Drzava = cmbDrzava.SelectedItem as Drzava;
            result.Kategorija = cmbKategorija.SelectedItem as KategorijaGimnasticara;
            result.Klub = cmbKlub.SelectedItem as Klub;

            return result;
        }

        private void cmbGimnastika_SelectedIndexChanged(object sender, EventArgs e)
        {
            reloadKategorije();
            cmbKategorija.SelectedIndex = cmbKategorija.Items.IndexOf(SVE_KATEGORIJE);
        }

        private void reloadKategorije()
        {
            Nullable<Gimnastika> gim = null;
            if (cmbGimnastika.SelectedIndex == cmbGimnastika.Items.IndexOf(MSG))
                gim = Gimnastika.MSG;
            else if (cmbGimnastika.SelectedIndex == cmbGimnastika.Items.IndexOf(ZSG))
                gim = Gimnastika.ZSG;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    setKategorije(loadKategorije(gim));
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showMessage(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);

                // TODO: Ovde bi nekako trebalo zatvoriti form na kome se nalazi
                // ova user kontrola
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
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

        private void txtGodRodj_KeyDown(object sender, KeyEventArgs e)
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
            resetFilter();
            generateFilterEvent = true;
            OnFilter(EventArgs.Empty);
        }
    }
}
