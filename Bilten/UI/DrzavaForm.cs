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
            return DAOFactoryFactory.DAOFactory.GetDrzavaDAO().FindById(id);
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

        protected override void updateEntity(DomainObject entity)
        {
            DAOFactoryFactory.DAOFactory.GetDrzavaDAO().Update((Drzava)entity);
        }

        protected override void addEntity(DomainObject entity)
        {
            DAOFactoryFactory.DAOFactory.GetDrzavaDAO().Add((Drzava)entity);
        }

        protected override void checkBusinessRulesOnAdd(DomainObject entity)
        {
            Drzava drzava = (Drzava)entity;
            Notification notification = new Notification();
            DrzavaDAO drzavaDAO = DAOFactoryFactory.DAOFactory.GetDrzavaDAO();

            if (drzavaDAO.existsDrzavaNaziv(drzava.Naziv))
            {
                notification.RegisterMessage("Naziv", "Drzava sa datim nazivom vec postoji.");
                throw new BusinessException(notification);
            }

            if (drzavaDAO.existsDrzavaKod(drzava.Kod))
            {
                notification.RegisterMessage("Kod", "Drzava sa datim kodom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        protected override void checkBusinessRulesOnUpdate(DomainObject entity)
        {
            Drzava drzava = (Drzava)entity;
            Notification notification = new Notification();
            DrzavaDAO drzavaDAO = DAOFactoryFactory.DAOFactory.GetDrzavaDAO();

            bool nazivChanged = (drzava.Naziv.ToUpper() != oldNaziv.ToUpper()) ? true : false;
            if (nazivChanged && drzavaDAO.existsDrzavaNaziv(drzava.Naziv))
            {
                notification.RegisterMessage("Naziv", "Drzava sa datim nazivom vec postoji.");
                throw new BusinessException(notification);
            }

            bool kodChanged = (drzava.Kod.ToUpper() != oldKod.ToUpper()) ? true : false;
            if (kodChanged && drzavaDAO.existsDrzavaKod(drzava.Kod))
            {
                notification.RegisterMessage("Kod", "Drzava sa datim kodom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private void DrzavaForm_Shown(object sender, EventArgs e)
        {
            if (!editMode)
            {
                txtNaziv.Focus();
            }
        }
    }
}