using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Bilten.Data;
using Bilten.Data.QueryModel;
using Bilten.Domain;
using Bilten.Exceptions;

namespace Bilten.UI
{
    public partial class FilterGimnasticarUserControl : UserControl
    {
        private IDataContext dataContext;
        private List<KategorijaGimnasticara> kategorije;
        private List<Klub> klubovi;
        private List<Drzava> drzave;
        private Nullable<Gimnastika> gimnastika;
        private bool initializing;

        private readonly string SVE_KATEGORIJE = "Sve kategorije";
        private readonly string SVI_KLUBOVI = "Svi klubovi";
        private readonly string SVE_DRZAVE = "Sve drzave";
        private readonly string SVI = "Svi";
        private readonly string MSG = "MSG";
        private readonly string ZSG = "ZSG";
        
        public FilterGimnasticarUserControl()
        {
            InitializeComponent();
        }

        public void initialize(Nullable<Gimnastika> gimnastika)
        {
            initializing = true;
            this.gimnastika = gimnastika;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();
                loadData();
                initUI();

                //dataContext.Commit();

                initializing = false;
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
            kategorije = loadKategorije(gimnastika);

            Query q = new Query();
            string sortingPropertyName = "Naziv";
            q.OrderClauses.Add(new OrderClause(sortingPropertyName, OrderClause.OrderClauseCriteria.Ascending));
            klubovi = new List<Klub>(dataContext.GetByCriteria<Klub>(q));

            q = new Query();
            sortingPropertyName = "Naziv";
            q.OrderClauses.Add(new OrderClause(sortingPropertyName, OrderClause.OrderClauseCriteria.Ascending));
            drzave = new List<Drzava>(dataContext.GetByCriteria<Drzava>(q));
        }

        private List<KategorijaGimnasticara> loadKategorije(Nullable<Gimnastika> gimnastika)
        {
            Query q = new Query();
            if (gimnastika != null)
                q.Criteria.Add(new Criterion("Gimnastika", CriteriaOperator.Equal, (byte)gimnastika));
            q.OrderClauses.Add(new OrderClause("Naziv", OrderClause.OrderClauseCriteria.Ascending));
            return new List<KategorijaGimnasticara>(
                dataContext.GetByCriteria<KategorijaGimnasticara>(q));
        }

        private void initUI()
        {
            cmbGimnastika.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGimnastika.Items.AddRange(new string[] { SVI, MSG, ZSG });

            cmbDrzava.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDrzava.Items.Add(SVE_DRZAVE);
            cmbDrzava.Items.AddRange(drzave.ToArray());

            cmbKategorija.DropDownStyle = ComboBoxStyle.DropDownList;
            setKategorije();

            cmbKlub.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKlub.Items.Add(SVI_KLUBOVI);
            cmbKlub.Items.AddRange(klubovi.ToArray());

            resetFilter();
        }

        private void setKategorije()
        {
            cmbKategorija.Items.Clear();
            cmbKategorija.Items.Add(SVE_KATEGORIJE);
            cmbKategorija.Items.AddRange(kategorije.ToArray());
        }

        public void resetFilter()
        {
            txtRegBroj.Text = String.Empty;
            txtIme.Text = String.Empty;
            txtPrezime.Text = String.Empty;
            txtGodRodj.Text = String.Empty;

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
            cmbKategorija.SelectedIndex = cmbKategorija.Items.IndexOf(SVE_KATEGORIJE);
            cmbKlub.SelectedIndex = cmbKlub.Items.IndexOf(SVI_KLUBOVI);
            cmbDrzava.SelectedIndex = cmbDrzava.Items.IndexOf(SVE_DRZAVE);
        }

        private bool validateFilter()
        {
            Notification notification = new Notification();
            int dummyInt;
            RegistarskiBroj dummyRegBroj;

            if (txtRegBroj.Text.Trim() != String.Empty
            && !RegistarskiBroj.TryParse(txtRegBroj.Text, out dummyRegBroj))
            {
                notification.RegisterMessage(
                    "RegistarskiBroj", "Neispravan format za registarski broj.");
            }

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
                case "RegistarskiBroj":
                    txtRegBroj.Focus();
                    break;

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
            if (txtRegBroj.Text.Trim() != String.Empty)
                result.RegBroj = RegistarskiBroj.Parse(txtRegBroj.Text);
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
            if (initializing)
                return;
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
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                kategorije = loadKategorije(gim);
                setKategorije();

                //     dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showMessage(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);

                // TODO: Ovde bi nekako trebalo zatvoriti form na kome se nalazi
                // ova user kontrola
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }
    }
}
