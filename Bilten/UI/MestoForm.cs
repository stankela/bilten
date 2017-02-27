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
using Bilten.Util;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class MestoForm : EntityDetailForm
    {
        private string oldNaziv;

        public MestoForm(Nullable<int> mestoId)
        {
            InitializeComponent();
            initialize(mestoId, true);
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Mesto";

            txtNaziv.Text = String.Empty;
        }

        protected override DomainObject createNewEntity()
        {
            return new Mesto();
        }

        protected override DomainObject getEntityById(int id)
        {
            return DAOFactoryFactory.DAOFactory.GetMestoDAO().FindById(id);
        }

        protected override void saveOriginalData(DomainObject entity)
        {
            Mesto m = (Mesto)entity;
            oldNaziv = m.Naziv;
        }

        protected override void updateUIFromEntity(DomainObject entity)
        {
            Mesto m = (Mesto)entity;
            txtNaziv.Text = m.Naziv;
        }

        protected override void requiredFieldsAndFormatValidation(Notification notification)
        {
            if (txtNaziv.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv mesta je obavezan.");
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
            Mesto m = (Mesto)entity;
            m.Naziv = txtNaziv.Text.Trim();
        }

        protected override void updateEntity(DomainObject entity)
        {
            DAOFactoryFactory.DAOFactory.GetMestoDAO().Update((Mesto)entity);
        }

        protected override void addEntity(DomainObject entity)
        {
            DAOFactoryFactory.DAOFactory.GetMestoDAO().Add((Mesto)entity);
        }

        protected override void checkBusinessRulesOnAdd(DomainObject entity)
        {
            Mesto m = (Mesto)entity;
            Notification notification = new Notification();

            if (DAOFactoryFactory.DAOFactory.GetMestoDAO().existsMestoNaziv(m.Naziv))
            {
                notification.RegisterMessage("Naziv", "Mesto sa datim nazivom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        protected override void checkBusinessRulesOnUpdate(DomainObject entity)
        {
            Mesto m = (Mesto)entity;
            Notification notification = new Notification();

            bool nazivChanged = (m.Naziv.ToUpper() != oldNaziv.ToUpper()) ? true : false;
            if (nazivChanged && DAOFactoryFactory.DAOFactory.GetMestoDAO().existsMestoNaziv(m.Naziv))
            {
                notification.RegisterMessage("Naziv", "Mesto sa datim nazivom vec postoji.");
                throw new BusinessException(notification);
            }
        }
    }
}