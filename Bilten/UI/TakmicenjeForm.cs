using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Exceptions;
using Bilten.Data;
using Bilten.Util;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class TakmicenjeForm : EntityDetailForm
    {
        private static readonly string STANDARDNO_TAKMICENJE = "Standardno takmicenje";
        private static readonly string FINALE_KUPA = "Finale kupa";
        private static readonly string ZBIR_VISE_KOLA = "Zbir vise kola, viseboj i ekipno";
        private static readonly string IZABERI_PRVO_I_DRUGO_KOLO = "Izaberi I kolo i II kolo";
        private static readonly string IZABERI_PRETHODNA_KOLA = "Izaberi prethodna kola";
        private static readonly int MAX_KOLA = 4;

        List<Takmicenje> prethodnaKola = new List<Takmicenje>();

        public TakmicenjeForm()
        {
            InitializeComponent();
            initialize(null, true);
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Takmicenje";

            txtNaziv.Text = String.Empty;
            txtDatum.Text = String.Empty;
            txtMesto.Text = String.Empty;

            prethodnaKola.Clear();

            cmbGimnastika.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGimnastika.Items.AddRange(new string[] { "MSG", "ZSG" });
            cmbGimnastika.SelectedIndex = -1;

            cmbTipTakmicenja.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipTakmicenja.Items.AddRange(new string[] { STANDARDNO_TAKMICENJE, FINALE_KUPA, ZBIR_VISE_KOLA });
            cmbTipTakmicenja.SelectedIndex = 0;

            listBox1.Enabled = false;
            listBox1.Items.Clear();
            btnIzaberiPrvaDvaKola.Enabled = false;
            btnIzaberiPrvaDvaKola.Text = IZABERI_PRVO_I_DRUGO_KOLO;

            cmbTipTakmicenja.SelectedIndexChanged += new EventHandler(cmbTipTakmicenja_SelectedIndexChanged);
        }

        private bool finaleKupa()
        {
            return cmbTipTakmicenja.SelectedIndex == cmbTipTakmicenja.Items.IndexOf(FINALE_KUPA);
        }

        private bool zbirViseKola()
        {
            return cmbTipTakmicenja.SelectedIndex == cmbTipTakmicenja.Items.IndexOf(ZBIR_VISE_KOLA);
        }

        private void cmbTipTakmicenja_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Enabled = finaleKupa() || zbirViseKola();
            btnIzaberiPrvaDvaKola.Enabled = finaleKupa() || zbirViseKola();
            if (finaleKupa())
                btnIzaberiPrvaDvaKola.Text = IZABERI_PRVO_I_DRUGO_KOLO;
            else if (zbirViseKola())
                btnIzaberiPrvaDvaKola.Text = IZABERI_PRETHODNA_KOLA;
            else
            {
                prethodnaKola.Clear();
                listBox1.Items.Clear();
            }
        }

        protected override void requiredFieldsAndFormatValidation(Notification notification)
        {
            if (txtNaziv.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv takmicenja je obavezan.");
            }
            if (cmbGimnastika.SelectedIndex == -1)
            {
                notification.RegisterMessage(
                    "Gimnastika", "Gimnastika je obavezna.");
            }
            if (txtDatum.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Datum", "Datum takmicenja je obavezan.");
            }
            else if (!tryParseDateTime(txtDatum.Text))
            {
                notification.RegisterMessage(
                    "Datum", "Neispravan format za datum takmicenja.");
            }
            else if (Datum.Parse(txtDatum.Text).ToDateTime().Year < 1753)
            {
                // NOTE: C# DateTime dozvoljava datume od 1.1.0001 dok SQL Serverov
                // tip datetime dozvoljava datume od 1.1.1753
                notification.RegisterMessage(
                    "Datum", "Neispravna vrednost za datum takmicenja.");
            }
            if (txtMesto.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Mesto", "Mesto odrzavanja je obavezno.");
            }

            if (finaleKupa() && (prethodnaKola.Count != 2))
            {
                notification.RegisterMessage(
                    "FinaleKupa", "Izaberite I i II kolo kupa.");
            }
            if (zbirViseKola())
            {
                if (prethodnaKola.Count == 0)
                {
                    notification.RegisterMessage("FinaleKupa", "Izaberite prethodna kola.");
                }
                else if (prethodnaKola.Count > MAX_KOLA)
                {
                    string msg = String.Format("Maksimalno dozvoljen broj kola je {0}.", MAX_KOLA);
                    notification.RegisterMessage("FinaleKupa", msg);
                }
            }
        }

        private bool tryParseDateTime(string s)
        {
            // koristi se klasa Datum zato sto DateTime ne dozvoljava tacku na kraju
            // datuma
            Datum datum;
            bool result = Datum.TryParse(s, out datum);
            if (result)
                result = datum.hasFullDatum();
            return result;
        }

        protected override void setFocus(string propertyName)
        {
            switch (propertyName)
            {
                case "Naziv":
                    txtNaziv.Focus();
                    break;

                case "Gimnastika":
                    cmbGimnastika.Focus();
                    break;

                case "Datum":
                    txtDatum.Focus();
                    break;

                case "Mesto":
                    txtMesto.Focus();
                    break;

                case "FinaleKupa":
                    listBox1.Focus();
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        protected override DomainObject createNewEntity()
        {
            return new Takmicenje();
        }

        protected override void updateEntityFromUI(DomainObject entity)
        {
            Takmicenje takmicenje = (Takmicenje)entity;
            takmicenje.Naziv = txtNaziv.Text.Trim();
            takmicenje.Datum = Datum.Parse(txtDatum.Text).ToDateTime();
            takmicenje.Mesto = txtMesto.Text.Trim();

            takmicenje.PrvoKolo = null;
            takmicenje.DrugoKolo = null;
            takmicenje.TreceKolo = null;
            takmicenje.CetvrtoKolo = null;
            if (finaleKupa())
            {
                takmicenje.TipTakmicenja = TipTakmicenja.FinaleKupa;
                takmicenje.PrvoKolo = prethodnaKola[0];
                takmicenje.DrugoKolo = prethodnaKola[1];
            }
            else if (zbirViseKola())
            {
                takmicenje.TipTakmicenja = TipTakmicenja.ZbirViseKola;
                if (prethodnaKola.Count > 0)
                    takmicenje.PrvoKolo = prethodnaKola[0];
                if (prethodnaKola.Count > 1)
                    takmicenje.DrugoKolo = prethodnaKola[1];
                if (prethodnaKola.Count > 2)
                    takmicenje.TreceKolo = prethodnaKola[2];
                if (prethodnaKola.Count > 3)
                    takmicenje.CetvrtoKolo = prethodnaKola[3];
            }
            else
            {
                takmicenje.TipTakmicenja = TipTakmicenja.StandardnoTakmicenje;
            }

            if (cmbGimnastika.SelectedIndex == 0)
                takmicenje.Gimnastika = Gimnastika.MSG;
            else if (cmbGimnastika.SelectedIndex == 1)
                takmicenje.Gimnastika = Gimnastika.ZSG;
        }

        protected override void addEntity(DomainObject entity)
        {
            DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().Add((Takmicenje)entity);
        }

        protected override void checkBusinessRulesOnAdd(DomainObject entity)
        {
            Takmicenje takmicenje = (Takmicenje)entity;
            Notification notification = new Notification();

            if (DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().existsTakmicenje(
                takmicenje.Naziv, takmicenje.Gimnastika, takmicenje.Datum))
            {
                notification.RegisterMessage("Naziv",
                    "Takmicenje sa datim nazivom, gimnastikom i datumom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private void btnIzaberiPrvaDvaKola_Click(object sender, EventArgs e)
        {
            OtvoriTakmicenjeForm form = null;
            DialogResult result;
            try
            {
                if (finaleKupa())
                    form = new OtvoriTakmicenjeForm(null, true, 2, false);
                else if (zbirViseKola())
                    form = new OtvoriTakmicenjeForm(null, true, MAX_KOLA, true);
                result = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (result != DialogResult.OK)
                return;

            if (finaleKupa() || zbirViseKola())
            {
                prethodnaKola.Clear();
                for (int i = 0; i < form.SelTakmicenja.Count; ++i)
                    prethodnaKola.Add(form.SelTakmicenja[i]);

                PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                    TypeDescriptor.GetProperties(typeof(Takmicenje))["Datum"]
                };
                ListSortDirection[] sortDir = new ListSortDirection[] {
                    ListSortDirection.Ascending
                };
                prethodnaKola.Sort(new SortComparer<Takmicenje>(propDesc, sortDir));

                listBox1.Items.Clear();
                for (int i = 0; i < prethodnaKola.Count; ++i)
                    listBox1.Items.Add(prethodnaKola[i].Naziv);
            }
        }

        private void TakmicenjeForm_Shown(object sender, EventArgs e)
        {
            txtNaziv.Focus();
        }

    }
}