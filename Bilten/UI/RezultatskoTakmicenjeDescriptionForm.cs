using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Exceptions;
using Bilten.Util;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class RezultatskoTakmicenjeDescriptionForm : EntityDetailForm
    {
        private Takmicenje takmicenje;
        private string oldNaziv;
        public List<TakmicarskaKategorija> Kategorije = new List<TakmicarskaKategorija>();

        public RezultatskoTakmicenjeDescriptionForm(Takmicenje takmicenje)
        {
            InitializeComponent();
            this.takmicenje = takmicenje;
            initialize(null, false);
        }

        public RezultatskoTakmicenjeDescriptionForm(
            RezultatskoTakmicenjeDescription desc, IList<TakmicarskaKategorija> kategorije, Takmicenje takmicenje)
        {
            InitializeComponent();
            this.takmicenje = takmicenje;
            Kategorije.AddRange(kategorije);
            initialize2(desc, false);
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Takmicenje";

            txtNaziv.Text = String.Empty;
            if (!editMode && takmicenje.TakmicenjeDescriptions.Count == 0)
            {
                // za prvo takmicenje, ponudi naziv kao glavno takmicenje
                txtNaziv.Text = takmicenje.Naziv;
            }

            checkedListBoxKategorije.CheckOnClick = true;

            checkedListBoxKategorije.Items.Clear();
            foreach (TakmicarskaKategorija k in takmicenje.Kategorije)
            {
                checkedListBoxKategorije.Items.Add(k);
                if (editMode && Kategorije.Contains(k))
                    checkedListBoxKategorije.SetItemChecked(checkedListBoxKategorije.Items.Count - 1, true);
            }
        }

        protected override DomainObject createNewEntity()
        {
            RezultatskoTakmicenjeDescription result = new RezultatskoTakmicenjeDescription();
            result.Propozicije = new Propozicije();
            return result;
        }

        protected override void saveOriginalData(DomainObject entity)
        {
            RezultatskoTakmicenjeDescription d = (RezultatskoTakmicenjeDescription)entity;
            oldNaziv = d.Naziv;
        }

        protected override void updateUIFromEntity(DomainObject entity)
        {
            RezultatskoTakmicenjeDescription d = (RezultatskoTakmicenjeDescription)entity;
            txtNaziv.Text = d.Naziv;
        }

        protected override void requiredFieldsAndFormatValidation(Notification notification)
        {
            if (txtNaziv.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv takmicenja je obavezan.");
            }
            if (checkedListBoxKategorije.Items.Count > 0 && checkedListBoxKategorije.CheckedItems.Count == 0)
            {
                notification.RegisterMessage(
                    "Kategorije", "Izaberite kategorije za takmicenje.");
            }
        }

        protected override void setFocus(string propertyName)
        {
            switch (propertyName)
            {
                case "Naziv":
                    txtNaziv.Focus();
                    break;

                case "Kategorije":
                    checkedListBoxKategorije.Focus();
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        protected override void updateEntityFromUI(DomainObject entity)
        {
            RezultatskoTakmicenjeDescription d = (RezultatskoTakmicenjeDescription)entity;
            d.Naziv = txtNaziv.Text.Trim();
            Kategorije.Clear();
            foreach (object item in checkedListBoxKategorije.CheckedItems)
            {
                Kategorije.Add(item as TakmicarskaKategorija);
            }
        }

        protected override void checkBusinessRulesOnAdd(DomainObject entity)
        {
            RezultatskoTakmicenjeDescription d = (RezultatskoTakmicenjeDescription)entity;
            Notification notification = new Notification();

            if (existsTakmicenjeNaziv(d))
            {
                notification.RegisterMessage("Naziv", "Takmicenje sa datim nazivom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private bool existsTakmicenjeNaziv(RezultatskoTakmicenjeDescription d)
        {
            foreach (RezultatskoTakmicenjeDescription d2 in takmicenje.TakmicenjeDescriptions)
            {
                if (!object.ReferenceEquals(d2, d) && d2.Naziv.ToUpper() == d.Naziv.ToUpper())
                    return true;
            }
            return false;
        }

        protected override void checkBusinessRulesOnUpdate(DomainObject entity)
        {
            RezultatskoTakmicenjeDescription d = (RezultatskoTakmicenjeDescription)entity;
            Notification notification = new Notification();

            bool nazivChanged = (d.Naziv.ToUpper() != oldNaziv.ToUpper()) ? true : false;
            if (nazivChanged && existsTakmicenjeNaziv(d))
            {
                notification.RegisterMessage("Naziv", "Takmicenje sa datim nazivom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        protected override void discardChanges()
        {
            if (editMode)
            {
                // TODO: Za sva nova svojstva koja dodam u klasu RezultatskoTakmicenjeDescription, 
                // moraju biti vracene stare vrednosti
                RezultatskoTakmicenjeDescription d = (RezultatskoTakmicenjeDescription)entity;
                d.Naziv = oldNaziv;
            }
        }

        private void RezultatskoTakmicenjeDescriptionForm_Shown(object sender, EventArgs e)
        {
            if (!editMode)
                txtNaziv.Focus();
        }

    }
}