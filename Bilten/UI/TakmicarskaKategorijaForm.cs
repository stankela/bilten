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
    public partial class TakmicarskaKategorijaForm : EntityDetailForm
    {
        private Takmicenje takmicenje;
        private string oldNaziv;

        public TakmicarskaKategorijaForm(TakmicarskaKategorija kat, Takmicenje takmicenje)
        {
            InitializeComponent();
            this.takmicenje = takmicenje;
            initialize2(kat, false);
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Kategorija";

            txtNaziv.Text = String.Empty;
        }

        protected override void saveOriginalData(DomainObject entity)
        {
            TakmicarskaKategorija kat = (TakmicarskaKategorija)entity;
            oldNaziv = kat.Naziv;
        }

        protected override void updateUIFromEntity(DomainObject entity)
        {
            TakmicarskaKategorija kat = (TakmicarskaKategorija)entity;
            txtNaziv.Text = kat.Naziv;
        }

        protected override void requiredFieldsAndFormatValidation(Notification notification)
        {
            if (txtNaziv.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv kategorije je obavezan.");
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
            TakmicarskaKategorija kat = (TakmicarskaKategorija)entity;
            kat.Naziv = txtNaziv.Text.Trim();
        }

        protected override void checkBusinessRulesOnAdd(DomainObject entity)
        {
            TakmicarskaKategorija kat = (TakmicarskaKategorija)entity;
            Notification notification = new Notification();

            if (existsKategorijaNaziv(kat))
            {
                notification.RegisterMessage("Naziv", "Kategorija sa datim nazivom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private bool existsKategorijaNaziv(TakmicarskaKategorija k)
        {
            foreach (TakmicarskaKategorija k2 in takmicenje.Kategorije)
            {
                if (!object.ReferenceEquals(k2, k) && k2.Naziv.ToUpper() == k.Naziv.ToUpper())
                    return true;
            }
            return false;
        }

        protected override void checkBusinessRulesOnUpdate(DomainObject entity)
        {
            TakmicarskaKategorija kat = (TakmicarskaKategorija)entity;
            Notification notification = new Notification();

            bool nazivChanged = (kat.Naziv.ToUpper() != oldNaziv.ToUpper()) ? true : false;
            if (nazivChanged && existsKategorijaNaziv(kat))
            {
                notification.RegisterMessage("Naziv", "Kategorija sa datim nazivom vec postoji.");
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
                TakmicarskaKategorija kat = (TakmicarskaKategorija)entity;
                kat.Naziv = oldNaziv;
            }
        }

    }
}