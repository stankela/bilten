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
        private TakmicarskaKategorija startKategorija;

        private List<KlubUcesnik> klubovi;
        private List<DrzavaUcesnik> drzave;
        private List<TakmicarskaKategorija> kategorije;

        private readonly string SVE_KATEGORIJE = "Sve kategorije";
        private readonly string SVI_KLUBOVI = "Svi klubovi";
        private readonly string SVE_DRZAVE = "Sve drzave";
        private readonly string MSG = "MSG";
        private readonly string ZSG = "ZSG";

        public FilterGimnasticarUcesnikUserControl()
        {
            InitializeComponent();
        }

        public void initialize(int takmicenjeId, Gimnastika gimnastika, TakmicarskaKategorija startKategorija)
        {
            this.takmicenjeId = takmicenjeId;
            this.gimnastika = gimnastika;
            this.startKategorija = startKategorija;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    loadData();
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

        private void loadData()
        {
            kategorije = new List<TakmicarskaKategorija>(
                DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().FindByTakmicenje(takmicenjeId));
            klubovi = new List<KlubUcesnik>(
                DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO().FindByTakmicenje(takmicenjeId));
            drzave = new List<DrzavaUcesnik>(
                DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO().FindByTakmicenje(takmicenjeId));
        }

        private void initUI()
        {
            cmbGimnastika.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGimnastika.Items.AddRange(new string[] { MSG, ZSG });
            if (gimnastika == Gimnastika.MSG)
                cmbGimnastika.SelectedIndex = cmbGimnastika.Items.IndexOf(MSG);
            else
                cmbGimnastika.SelectedIndex = cmbGimnastika.Items.IndexOf(ZSG);
            cmbGimnastika.Enabled = false;

            cmbKategorija.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKategorija.Items.Add(SVE_KATEGORIJE);
            cmbKategorija.Items.AddRange(kategorije.ToArray());

            cmbKlub.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKlub.Items.Add(SVI_KLUBOVI);
            cmbKlub.Items.AddRange(klubovi.ToArray());

            cmbDrzava.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDrzava.Items.Add(SVE_DRZAVE);
            cmbDrzava.Items.AddRange(drzave.ToArray());

            resetFilter();
        }

        public void resetFilter()
        {
            txtIme.Text = String.Empty;
            txtPrezime.Text = String.Empty;
            txtGodRodj.Text = String.Empty;

            if (startKategorija == null)
                cmbKategorija.SelectedIndex = cmbKategorija.Items.IndexOf(SVE_KATEGORIJE);
            else
                cmbKategorija.SelectedItem = startKategorija;

            cmbKlub.SelectedIndex = cmbKlub.Items.IndexOf(SVI_KLUBOVI);
            cmbDrzava.SelectedIndex = cmbDrzava.Items.IndexOf(SVE_DRZAVE);
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
            result.Gimnastika = this.gimnastika;

            result.Kategorija = cmbKategorija.SelectedItem as TakmicarskaKategorija;
            result.Klub = cmbKlub.SelectedItem as KlubUcesnik;
            result.Drzava = cmbDrzava.SelectedItem as DrzavaUcesnik;

            return result;
        }
    }
}
