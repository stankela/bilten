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
using Bilten.Data.QueryModel;
using Bilten.Util;

namespace Bilten.UI
{
    public partial class FilterGimnasticarUcesnikUserControl : UserControl
    {
        private int takmicenjeId;
        private Nullable<Gimnastika> gimnastika;
        private TakmicarskaKategorija startKategorija;

        private List<KlubUcesnik> klubovi;
        private List<DrzavaUcesnik> drzave;
        private IDataContext dataContext;
        private IList<TakmicarskaKategorija> sveKategorije;
        private List<TakmicarskaKategorija> mKategorije = new List<TakmicarskaKategorija>();
        private List<TakmicarskaKategorija> zKategorije = new List<TakmicarskaKategorija>();

        private readonly string SVE_KATEGORIJE = "Sve kategorije";
        private readonly string SVI_KLUBOVI = "Svi klubovi";
        private readonly string SVE_DRZAVE = "Sve drzave";
        private readonly string SVI = "Svi";
        private readonly string MSG = "MSG";
        private readonly string ZSG = "ZSG";

        public FilterGimnasticarUcesnikUserControl()
        {
            InitializeComponent();
        }

        public void initialize(int takmicenjeId, Nullable<Gimnastika> gimnastika, 
            TakmicarskaKategorija startKategorija)
        {
            this.takmicenjeId = takmicenjeId;
            this.gimnastika = gimnastika;
            this.startKategorija = startKategorija;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                loadData();
                initUI();

                //dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw new InfrastructureException(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        private void loadData()
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje.Id", CriteriaOperator.Equal, takmicenjeId));
            sveKategorije = dataContext.GetByCriteria<TakmicarskaKategorija>(q);

            foreach (TakmicarskaKategorija k in sveKategorije)
            {
                if (k.Gimnastika == Gimnastika.MSG)
                    mKategorije.Add(k);
                else
                    zKategorije.Add(k);
            }

            IList<KlubUcesnik> kluboviList = findKluboviUcesnici(takmicenjeId);
            klubovi = new List<KlubUcesnik>(kluboviList);

            IList<DrzavaUcesnik> drzaveList = findDrzaveUcesnici(takmicenjeId);
            drzave = new List<DrzavaUcesnik>(drzaveList);
        }

        private IList<DrzavaUcesnik> findDrzaveUcesnici(int takmicenjeId)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje.Id", CriteriaOperator.Equal, takmicenjeId));
            q.OrderClauses.Add(new OrderClause("Naziv", OrderClause.OrderClauseCriteria.Ascending));
            return dataContext.GetByCriteria<DrzavaUcesnik>(q);
        }

        private IList<KlubUcesnik> findKluboviUcesnici(int takmicenjeId)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje.Id", CriteriaOperator.Equal, takmicenjeId));
            q.OrderClauses.Add(new OrderClause("Naziv", OrderClause.OrderClauseCriteria.Ascending));
            return dataContext.GetByCriteria<KlubUcesnik>(q);
        }

        private void initUI()
        {
            cmbGimnastika.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGimnastika.Items.AddRange(new string[] { SVI, MSG, ZSG });

            cmbKategorija.DropDownStyle = ComboBoxStyle.DropDownList;
            fillKategorijeCombo(gimnastika);

            cmbKlub.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKlub.Items.Add(SVI_KLUBOVI);
            cmbKlub.Items.AddRange(klubovi.ToArray());

            cmbDrzava.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDrzava.Items.Add(SVE_DRZAVE);
            cmbDrzava.Items.AddRange(drzave.ToArray());

            resetFilter();

            cmbGimnastika.SelectedIndexChanged += new EventHandler(cmbGimnastika_SelectedIndexChanged);
        }

        private void fillKategorijeCombo(Nullable<Gimnastika> gimnastika)
        {
            cmbKategorija.Items.Clear();
            IList<TakmicarskaKategorija> kategorije;
            if (gimnastika == null)
            {
                kategorije = sveKategorije;
                cmbKategorija.Items.Add(SVE_KATEGORIJE);
            }
            else if (gimnastika.Value == Gimnastika.MSG)
                kategorije = mKategorije;
            else
                kategorije = zKategorije;
            foreach (TakmicarskaKategorija k in kategorije)
                cmbKategorija.Items.Add(k);
        }

        private void cmbGimnastika_SelectedIndexChanged(object sender, EventArgs e)
        {
            Nullable<Gimnastika> gimnastika = getSelectedGimnastika();
            object selKategorijaItem = cmbKategorija.SelectedItem;
            fillKategorijeCombo(gimnastika);
            cmbKategorija.SelectedItem = selKategorijaItem;
            if (cmbKategorija.SelectedIndex == -1 && cmbKategorija.Items.Count > 0)
                cmbKategorija.SelectedIndex = 0;
        }

        private Nullable<Gimnastika> getSelectedGimnastika()
        {
            Nullable<Gimnastika> result = null;
            if (cmbGimnastika.SelectedIndex == cmbGimnastika.Items.IndexOf(MSG))
                result = Gimnastika.MSG;
            else if (cmbGimnastika.SelectedIndex == cmbGimnastika.Items.IndexOf(ZSG))
                result = Gimnastika.ZSG;
            return result;
        }

        public void resetFilter()
        {
            txtIme.Text = String.Empty;
            txtPrezime.Text = String.Empty;
            txtGodRodj.Text = String.Empty;

            cmbGimnastika.SelectedIndexChanged -= cmbGimnastika_SelectedIndexChanged;
            if (gimnastika == null)
            {
                cmbGimnastika.SelectedIndex = cmbGimnastika.Items.IndexOf(SVI);
            }
            else
            {
                cmbGimnastika.Enabled = false;
                if (gimnastika.Value == Gimnastika.MSG)
                    cmbGimnastika.SelectedIndex = cmbGimnastika.Items.IndexOf(MSG);
                else
                    cmbGimnastika.SelectedIndex = cmbGimnastika.Items.IndexOf(ZSG);
            }
            cmbGimnastika.SelectedIndexChanged += cmbGimnastika_SelectedIndexChanged;

            fillKategorijeCombo(gimnastika);
            if (startKategorija == null)
                cmbKategorija.SelectedIndex = cmbKategorija.Items.IndexOf(SVE_KATEGORIJE);
            else
                selectKategorija(startKategorija);
            if (cmbKategorija.SelectedIndex == -1 && cmbKategorija.Items.Count > 0)
                cmbKategorija.SelectedIndex = 0;

            cmbKlub.SelectedIndex = cmbKlub.Items.IndexOf(SVI_KLUBOVI);
            cmbDrzava.SelectedIndex = cmbDrzava.Items.IndexOf(SVE_DRZAVE);
        }

        private void selectKategorija(TakmicarskaKategorija kategorija)
        {
            foreach (object item in cmbKategorija.Items)
            {
                TakmicarskaKategorija kat = item as TakmicarskaKategorija;
                if (kat != null && kat.Equals(kategorija))
                    cmbKategorija.SelectedItem = item;
            }
        }

        private bool validateFilter()
        {
            Notification notification = new Notification();
            int dummyInt;
            if (txtGodRodj.Text.Trim() != String.Empty &&
            !int.TryParse(txtGodRodj.Text, out dummyInt))
            {
                notification.RegisterMessage(
                  "GodinaRodjenja", "Neispravan format za godinu rodjenja.");
            }
            if (!notification.IsValid())
            {
                NotificationMessage msg = notification.FirstMessage;
                // TODO: this.Text nije inicijalizovan
                MessageDialogs.showMessage(msg.Message, this.Text);
                setFocus(msg.FieldName);
                return false;
            }
            else
            {
                return true;
            }
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

        public GimnasticarUcesnikFilter getFilter()
        {
            if (!validateFilter())
                return null;

            GimnasticarUcesnikFilter result = new GimnasticarUcesnikFilter();
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

            result.Kategorija = cmbKategorija.SelectedItem as TakmicarskaKategorija;
            result.Klub = cmbKlub.SelectedItem as KlubUcesnik;
            result.Drzava = cmbDrzava.SelectedItem as DrzavaUcesnik;

            return result;
        }
    }
}
