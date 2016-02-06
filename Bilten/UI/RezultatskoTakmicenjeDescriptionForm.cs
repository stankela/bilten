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

namespace Bilten.UI
{
    public partial class RezultatskoTakmicenjeDescriptionForm : EntityDetailForm
    {
        private Takmicenje takmicenje;
        private string oldNaziv;

        public RezultatskoTakmicenjeDescriptionForm(Takmicenje takmicenje)
        {
            InitializeComponent();
            this.takmicenje = takmicenje;
            initialize(null, false);
        }

        public RezultatskoTakmicenjeDescriptionForm(
            RezultatskoTakmicenjeDescription desc, Takmicenje takmicenje)
        {
            InitializeComponent();
            this.takmicenje = takmicenje;
            initialize2(desc, false);
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Takmicenje";

            txtNaziv.Text = String.Empty;
        }

        protected override DomainObject createNewEntity()
        {
            RezultatskoTakmicenjeDescription result = 
                new RezultatskoTakmicenjeDescription();
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
        }

        protected override void setFocus(string propertyName)
        {
            switch (propertyName)
            {
                case "Naziv":
                    txtNaziv.Focus();
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        protected override void updateEntityFromUI(DomainObject entity)
        {
            RezultatskoTakmicenjeDescription d = (RezultatskoTakmicenjeDescription)entity;
            d.Naziv = txtNaziv.Text.Trim();
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
                // TODO: Za sva nova svojstva koja dodam u klasu 
                // RezultatskoTakmicenjeDescription, 
                // moraju biti vracene stare vrednosti
                RezultatskoTakmicenjeDescription d = (RezultatskoTakmicenjeDescription)entity;
                d.Naziv = oldNaziv;
            }
        }

    }
}