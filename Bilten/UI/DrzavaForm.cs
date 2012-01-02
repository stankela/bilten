using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Exceptions;
using Bilten.Data.QueryModel;

namespace Bilten.UI
{
    public partial class DrzavaForm : EntityDetailForm
    {
        private string oldNaziv;
        private string oldKod;
        
        public DrzavaForm(Nullable<int> drzavaId)
        {
            InitializeComponent();
            initialize(drzavaId, true);
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Drzava";

            txtNaziv.Text = String.Empty;
            txtKod.Text = String.Empty;
        }

        protected override DomainObject getEntityById(int id)
        {
            return dataContext.GetById<Drzava>(id);
        }

        protected override void saveOriginalData(DomainObject entity)
        {
            Drzava drzava = (Drzava)entity;
            oldNaziv = drzava.Naziv;
            oldKod = drzava.Kod;
        }

        protected override void updateUIFromEntity(DomainObject entity)
        {
            Drzava drzava = (Drzava)entity;
            txtNaziv.Text = drzava.Naziv;
            txtKod.Text = drzava.Kod;
        }

        protected override void requiredFieldsAndFormatValidation(Notification notification)
        {
            if (txtNaziv.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv drzave je obavezan.");
            }
            if (txtKod.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Kod", "Kod drzave je obavezan.");
            }
        }

        protected override void setFocus(string propertyName)
        {
            switch (propertyName)
            {
                case "Naziv":
                    txtNaziv.Focus();
                    break;

                case "Kod":
                    txtKod.Focus();
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        protected override DomainObject createNewEntity()
        {
            return new Drzava();
        }

        protected override void updateEntityFromUI(DomainObject entity)
        {
            Drzava drzava = (Drzava)entity;
            drzava.Naziv = txtNaziv.Text.Trim();
            drzava.Kod = txtKod.Text.Trim().ToUpper();
        }

        protected override void checkBusinessRulesOnAdd(DomainObject entity)
        {
            Drzava drzava = (Drzava)entity;
            Notification notification = new Notification();

            if (existsDrzavaNaziv(drzava.Naziv))
            {
                notification.RegisterMessage("Naziv", "Drzava sa datim nazivom vec postoji.");
                throw new BusinessException(notification);
            }

            if (existsDrzavaKod(drzava.Kod))
            {
                notification.RegisterMessage("Kod", "Drzava sa datim kodom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private bool existsDrzavaNaziv(string naziv)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Naziv", CriteriaOperator.Equal, naziv));
            return dataContext.GetCount<Drzava>(q) > 0;
        }

        private bool existsDrzavaKod(string kod)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Kod", CriteriaOperator.Equal, kod));
            return dataContext.GetCount<Drzava>(q) > 0;
        }

        protected override void checkBusinessRulesOnUpdate(DomainObject entity)
        {
            Drzava drzava = (Drzava)entity;
            Notification notification = new Notification();

            bool nazivChanged = (drzava.Naziv.ToUpper() != oldNaziv.ToUpper()) ? true : false;
            if (nazivChanged && existsDrzavaNaziv(drzava.Naziv))
            {
                notification.RegisterMessage("Naziv", "Drzava sa datim nazivom vec postoji.");
                throw new BusinessException(notification);
            }

            bool kodChanged = (drzava.Kod.ToUpper() != oldKod.ToUpper()) ? true : false;
            if (kodChanged && existsDrzavaKod(drzava.Kod))
            {
                notification.RegisterMessage("Kod", "Drzava sa datim kodom vec postoji.");
                throw new BusinessException(notification);
            }
        }
    }
}