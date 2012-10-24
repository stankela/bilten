using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Data.QueryModel;
using Bilten.Exceptions;
using Bilten.Data;

namespace Bilten.UI
{
    public partial class TakmicenjeForm : EntityDetailForm
    {
        private Takmicenje prvoKolo;
        private Takmicenje drugoKolo;
        
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

            prvoKolo = null;
            drugoKolo = null;
            listBox1.Items.Clear();

            cmbGimnastika.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGimnastika.Items.AddRange(new string[] { "MSG", "ZSG" });
            cmbGimnastika.SelectedIndex = -1;

            ckbFinaleKupa.Checked = false;
            listBox1.Enabled = false;
            btnIzaberiPrvaDvaKola.Enabled = false;

            this.ckbFinaleKupa.CheckedChanged += new System.EventHandler(this.ckbFinaleKupa_CheckedChanged);
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

            if (ckbFinaleKupa.Checked && (prvoKolo == null || drugoKolo == null))
            {
                notification.RegisterMessage(
                    "FinaleKupa", "Izaberite I i II kolo kupa.");
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
            takmicenje.FinaleKupa = ckbFinaleKupa.Checked;
            if (takmicenje.FinaleKupa)
            {
                takmicenje.PrvoKolo = prvoKolo;
                takmicenje.DrugoKolo = drugoKolo;
            }
            else
            {
                takmicenje.PrvoKolo = null;
                takmicenje.DrugoKolo = null;
            }

            if (cmbGimnastika.SelectedIndex == 0)
                takmicenje.Gimnastika = Gimnastika.MSG;
            else if (cmbGimnastika.SelectedIndex == 1)
                takmicenje.Gimnastika = Gimnastika.ZSG;
        }

        protected override void checkBusinessRulesOnAdd(DomainObject entity)
        {
            Takmicenje takmicenje = (Takmicenje)entity;
            Notification notification = new Notification();

            if (existsTakmicenje(takmicenje))
            {
                notification.RegisterMessage("Naziv",
                    "Takmicenje sa datim nazivom, gimnastikom i datumom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private bool existsTakmicenje(Takmicenje tak)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Naziv", CriteriaOperator.Equal, tak.Naziv));
            q.Criteria.Add(new Criterion("Gimnastika", CriteriaOperator.Equal, (byte)tak.Gimnastika));
            q.Criteria.Add(new Criterion("Datum", CriteriaOperator.Equal, tak.Datum));
            q.Operator = QueryOperator.And;
            return dataContext.GetCount<Takmicenje>(q) > 0;
        }

        private void ckbFinaleKupa_CheckedChanged(object sender, EventArgs e)
        {
            listBox1.Enabled = ckbFinaleKupa.Checked;
            btnIzaberiPrvaDvaKola.Enabled = ckbFinaleKupa.Checked;
        }

        private void btnIzaberiPrvaDvaKola_Click(object sender, EventArgs e)
        {
            OtvoriTakmicenjeForm form;
            DialogResult result;
            try
            {
                form = new OtvoriTakmicenjeForm(null, true, 2);
                result = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (result != DialogResult.OK)
                return;
            if (ckbFinaleKupa.Checked && form.SelTakmicenja.Count != 2)
                return;

            if (ckbFinaleKupa.Checked)
            {
                prvoKolo = form.SelTakmicenja[0];
                drugoKolo = form.SelTakmicenja[1];
                if (prvoKolo.Datum > drugoKolo.Datum)
                {
                    Takmicenje temp = prvoKolo;
                    prvoKolo = drugoKolo;
                    drugoKolo = temp;
                }

                listBox1.Items.Clear();
                listBox1.Items.Add(prvoKolo.Naziv);
                listBox1.Items.Add(drugoKolo.Naziv);
            }
        }
    }
}