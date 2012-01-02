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

namespace Bilten.UI
{
    public partial class SudijaForm : EntityDetailForm
    {
        private List<Drzava> drzave;
        private string oldIme;
        private string oldPrezime;

        public SudijaForm(Nullable<int> sudijaId)
        {
            InitializeComponent();
            initialize(sudijaId, true);
        }

        protected override void loadData()
        {
            Query q = new Query();
            string sortingPropertyName = "Naziv";
            q.OrderClauses.Add(new OrderClause(sortingPropertyName, OrderClause.OrderClauseCriteria.Ascending));
            drzave = new List<Drzava>(dataContext.GetByCriteria<Drzava>(q));
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Sudija";

            txtIme.Text = String.Empty;
            txtPrezime.Text = String.Empty;

            cmbPol.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPol.Items.AddRange(new string[] { "Muski", "Zenski" });
            cmbPol.SelectedIndex = -1;

            cmbDrzava.DropDownStyle = ComboBoxStyle.DropDownList;
            setDrzave(drzave);
            SelectedDrzava = null;
        }

        private void setDrzave(List<Drzava> drzave)
        {
            cmbDrzava.DisplayMember = "Naziv";
            cmbDrzava.DataSource = drzave;

            CurrencyManager currencyManager =
                (CurrencyManager)this.BindingContext[drzave];
            currencyManager.Refresh();
        }

        private Drzava SelectedDrzava
        {
            get { return cmbDrzava.SelectedItem as Drzava; }
            set { cmbDrzava.SelectedItem = value; }
        }

        protected override DomainObject getEntityById(int id)
        {
            return dataContext.GetById<Sudija>(id);
        }

        protected override void saveOriginalData(DomainObject entity)
        {
            Sudija sudija = (Sudija)entity;
            oldIme = sudija.Ime;
            oldPrezime = sudija.Prezime;
        }

        protected override void updateUIFromEntity(DomainObject entity)
        {
            Sudija sudija = (Sudija)entity;
            txtIme.Text = sudija.Ime;
            txtPrezime.Text = sudija.Prezime;

            cmbPol.SelectedIndex = -1;
            if (sudija.Pol == Pol.Muski)
                cmbPol.SelectedIndex = 0;
            else if (sudija.Pol == Pol.Zenski)
                cmbPol.SelectedIndex = 1;

            SelectedDrzava = sudija.Drzava;
        }

        protected override void requiredFieldsAndFormatValidation(Notification notification)
        {
            if (txtIme.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Ime", "Ime sudije je obavezno.");
            }
            if (txtPrezime.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Prezime", "Prezime sudije je obavezno.");
            }
            if (cmbPol.SelectedIndex == -1)
            {
                notification.RegisterMessage(
                    "Pol", "Pol sudije je obavezan.");
            }
            if (cmbDrzava.SelectedIndex == -1)
            {
                notification.RegisterMessage(
                    "Drzava", "Drzava je obavezna.");
            }
        }

        protected override void setFocus(string propertyName)
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

                case "Drzava":
                    cmbDrzava.Focus();
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        protected override DomainObject createNewEntity()
        {
            return new Sudija();
        }

        protected override void updateEntityFromUI(DomainObject entity)
        {
            Sudija sudija = (Sudija)entity;
            sudija.Ime = txtIme.Text.Trim();
            sudija.Prezime = txtPrezime.Text.Trim();

            if (cmbPol.SelectedIndex == 0)
                sudija.Pol = Pol.Muski;
            else
                sudija.Pol = Pol.Zenski;

            sudija.Drzava = SelectedDrzava;
        }

        protected override void checkBusinessRulesOnAdd(DomainObject entity)
        {
            Sudija sudija = (Sudija)entity;
            Notification notification = new Notification();

            if (existsSudijaImePrezime(sudija.Ime, sudija.Prezime))
            {
                notification.RegisterMessage("Ime",
                    "Sudija sa datim imenom i prezimenom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private bool existsSudijaImePrezime(string ime, string prezime)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Ime", CriteriaOperator.Equal, ime));
            q.Criteria.Add(new Criterion("Prezime", CriteriaOperator.Equal, prezime));
            q.Operator = QueryOperator.And;
            return dataContext.GetCount<Sudija>(q) > 0;
        }

        protected override void checkBusinessRulesOnUpdate(DomainObject entity)
        {
            Sudija sudija = (Sudija)entity;
            Notification notification = new Notification();

            bool imePrezimeChanged = (sudija.Ime.ToUpper() != oldIme.ToUpper()
                || sudija.Prezime.ToUpper() != oldPrezime.ToUpper()) ? true : false;
            if (imePrezimeChanged
            && existsSudijaImePrezime(sudija.Ime, sudija.Prezime))
            {
                notification.RegisterMessage("Ime",
                    "Sudija sa datim imenom i prezimenom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private void btnAddDrzava_Click(object sender, EventArgs e)
        {
            try
            {
                DrzavaForm form = new DrzavaForm(null);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Drzava d = (Drzava)form.Entity;
                    drzave.Add(d);
                    drzave.Sort();
                    setDrzave(drzave);
                    SelectedDrzava = d;
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }
    }
}