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
    public partial class KategorijaGimnasticaraForm : EntityDetailForm
    {
        private string oldNaziv;
        private Gimnastika oldGimnastika;
        
        public KategorijaGimnasticaraForm(Nullable<int> kategorijaId)
        {
            InitializeComponent();
            initialize(kategorijaId, true);
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Kategorija gimnasticara";

            txtNaziv.Text = String.Empty;
            
            cmbGimnastika.DropDownStyle = ComboBoxStyle.DropDownList;
            setGimnastike();
            SelectedGimnastika = Gimnastika.Undefined;
        }

        private void setGimnastike()
        {
            cmbGimnastika.Items.AddRange(new string[] { "MSG", "ZSG" });
        }

        private Gimnastika SelectedGimnastika
        {
            get
            {
                if (cmbGimnastika.SelectedIndex == 0)
                    return Gimnastika.MSG;
                else if (cmbGimnastika.SelectedIndex == 1)
                    return Gimnastika.ZSG;
                else
                    return Gimnastika.Undefined;
            }
            set
            {
                if (value == Gimnastika.MSG)
                    cmbGimnastika.SelectedIndex = 0;
                else if (value == Gimnastika.ZSG)
                    cmbGimnastika.SelectedIndex = 1;
                else
                    cmbGimnastika.SelectedIndex = -1;
            }
        }

        protected override DomainObject getEntityById(int id)
        {
            return DAOFactoryFactory.DAOFactory.GetKategorijaGimnasticaraDAO().FindById(id);
        }

        protected override void saveOriginalData(DomainObject entity)
        {
            KategorijaGimnasticara kat = (KategorijaGimnasticara)entity;
            oldNaziv = kat.Naziv;
            oldGimnastika = kat.Gimnastika;
        }

        protected override void updateUIFromEntity(DomainObject entity)
        {
            KategorijaGimnasticara kat = (KategorijaGimnasticara)entity;
            txtNaziv.Text = kat.Naziv;

            SelectedGimnastika = kat.Gimnastika;
        }

        protected override void requiredFieldsAndFormatValidation(Notification notification)
        {
            if (txtNaziv.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv kategorije je obavezan.");
            }
            if (SelectedGimnastika == Gimnastika.Undefined)
            {
                notification.RegisterMessage(
                    "Gimnastika", "Gimnastika je obavezna.");
            }
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

                default:
                    throw new ArgumentException();
            }
        }

        protected override DomainObject createNewEntity()
        {
            return new KategorijaGimnasticara();
        }

        protected override void updateEntityFromUI(DomainObject entity)
        {
            KategorijaGimnasticara kat = (KategorijaGimnasticara)entity;
            kat.Naziv = txtNaziv.Text.Trim();

            kat.Gimnastika = SelectedGimnastika;
        }

        protected override void updateEntity(DomainObject entity)
        {
            DAOFactoryFactory.DAOFactory.GetKategorijaGimnasticaraDAO().Update((KategorijaGimnasticara)entity);
        }

        protected override void addEntity(DomainObject entity)
        {
            DAOFactoryFactory.DAOFactory.GetKategorijaGimnasticaraDAO().Add((KategorijaGimnasticara)entity);
        }

        protected override void checkBusinessRulesOnAdd(DomainObject entity)
        {
            KategorijaGimnasticara kat = (KategorijaGimnasticara)entity;
            Notification notification = new Notification();

            if (DAOFactoryFactory.DAOFactory.GetKategorijaGimnasticaraDAO().existsKategorijaGimnasticara(
                kat.Naziv, kat.Gimnastika))
            {
                notification.RegisterMessage("Naziv", 
                    "Kategorija sa datim nazivom i gimnastikom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        protected override void checkBusinessRulesOnUpdate(DomainObject entity)
        {
            KategorijaGimnasticara kat = (KategorijaGimnasticara)entity;
            Notification notification = new Notification();

            bool changed = (kat.Naziv.ToUpper() != oldNaziv.ToUpper() 
                || kat.Gimnastika != oldGimnastika) ? true : false;
            if (changed && DAOFactoryFactory.DAOFactory.GetKategorijaGimnasticaraDAO().existsKategorijaGimnasticara(
                kat.Naziv, kat.Gimnastika))
            {
                notification.RegisterMessage("Naziv",
                    "Kategorija sa datim nazivom i gimnastikom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private void KategorijaGimnasticaraForm_Shown(object sender, EventArgs e)
        {
            if (!editMode)
                txtNaziv.Focus();
            else
                btnCancel.Focus();
        }
    }
}