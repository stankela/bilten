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
    public partial class TakmicenjeForm : EntityDetailForm
    {
        private string oldNaziv;
        private DateTime oldDatum;
        private Gimnastika oldGimnastika;
        
        public TakmicenjeForm(Nullable<int> takmicenjeId)
        {
            InitializeComponent();
            initialize(takmicenjeId, true);
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Takmicenje";

            txtNaziv.Text = String.Empty;
            txtDatum.Text = String.Empty;
            txtMesto.Text = String.Empty;

            cmbGimnastika.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGimnastika.Items.AddRange(new string[] { "MSG", "ZSG" });
            cmbGimnastika.SelectedIndex = -1;
        }

        protected override DomainObject getEntityById(int id)
        {
            return dataContext.GetById<Takmicenje>(id);
        }

        protected override void saveOriginalData(DomainObject entity)
        {
            Takmicenje takmicenje = (Takmicenje)entity;
            oldNaziv = takmicenje.Naziv;
            oldDatum = takmicenje.Datum;
            oldGimnastika = takmicenje.Gimnastika;
        }

        protected override void updateUIFromEntity(DomainObject entity)
        {
            Takmicenje takmicenje = (Takmicenje)entity;
            txtNaziv.Text = takmicenje.Naziv;
            txtDatum.Text = takmicenje.Datum.ToString("d");
            txtMesto.Text = takmicenje.Mesto;

            if (takmicenje.Gimnastika == Gimnastika.MSG)
                cmbGimnastika.SelectedIndex = 0;
            else if (takmicenje.Gimnastika == Gimnastika.ZSG)
                cmbGimnastika.SelectedIndex = 1;
            else
                cmbGimnastika.SelectedIndex = -1;
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

        protected override void checkBusinessRulesOnUpdate(DomainObject entity)
        {
            Takmicenje takmicenje = (Takmicenje)entity;
            Notification notification = new Notification();

            bool changed = (takmicenje.Naziv != oldNaziv
                || takmicenje.Gimnastika != oldGimnastika
                || takmicenje.Datum != oldDatum) ? true : false;
            if (changed && existsTakmicenje(takmicenje))
            {
                notification.RegisterMessage("Naziv",
                    "Takmicenje sa datim nazivom, gimnastikom i datumom vec postoji.");
                throw new BusinessException(notification);
            }
        }
    }
}