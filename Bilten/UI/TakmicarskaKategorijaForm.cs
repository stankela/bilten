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
using NHibernate;
using Bilten.Data;
using NHibernate.Context;
using Bilten.Dao;
using Bilten.Services;

namespace Bilten.UI
{
    public partial class TakmicarskaKategorijaForm : EntityDetailForm
    {
        private int takmicenjeId;
        List<string> kategorije;
        private string oldNaziv;
        private Gimnastika gimnastika;

        public TakmicarskaKategorijaForm(Nullable<int> kategorijaId, int takmicenjeId)
        {
            InitializeComponent();
            this.updateLastModified = true;
            this.takmicenjeId = takmicenjeId;
            initialize(kategorijaId, true);
        }

        protected override void loadData()
        {
            Takmicenje t = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
            gimnastika = t.Gimnastika;
            ISet<string> kategorijeSet = new HashSet<string>();
            foreach (KategorijaGimnasticara k in DAOFactoryFactory.DAOFactory.GetKategorijaGimnasticaraDAO()
                .FindByGimnastika(gimnastika))
            {
                kategorijeSet.Add(k.Naziv);
            }
            foreach (TakmicarskaKategorija k in t.Kategorije)
                kategorijeSet.Add(k.Naziv);
            kategorije = new List<string>(kategorijeSet);
            kategorije.Sort();
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Izaberite kategoriju";

            cmbKategorija.DropDownStyle = ComboBoxStyle.DropDown;
            setKategorije(kategorije);
            SelectedKategorija = null;
            cmbKategorija.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbKategorija.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void setKategorije(IList<string> kategorije)
        {
            cmbKategorija.DataSource = kategorije;
        }

        private string SelectedKategorija
        {
            get { return cmbKategorija.SelectedItem.ToString(); }
            set { cmbKategorija.SelectedItem = value; }
        }

        protected override DomainObject getEntityById(int id)
        {
            return DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().FindById(id);
        }

        protected override void saveOriginalData(DomainObject entity)
        {
            TakmicarskaKategorija kat = (TakmicarskaKategorija)entity;
            oldNaziv = kat.Naziv;
        }

        protected override void updateUIFromEntity(DomainObject entity)
        {
            TakmicarskaKategorija kat = (TakmicarskaKategorija)entity;
            SelectedKategorija = kat.Naziv;
        }

        protected override void requiredFieldsAndFormatValidation(Notification notification)
        {
            if (cmbKategorija.Text.Trim() == String.Empty)
                notification.RegisterMessage("Naziv", "Naziv kategorije je obavezan.");
        }

        protected override void setFocus(string propertyName)
        {
            switch (propertyName)
            {
                case "Naziv":
                    cmbKategorija.Focus();
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        protected override DomainObject createNewEntity()
        {
            return new TakmicarskaKategorija();
        }

        protected override void updateEntityFromUI(DomainObject entity)
        {
            TakmicarskaKategorija kat = (TakmicarskaKategorija)entity;
            kat.Naziv = cmbKategorija.Text.Trim();
            kat.Gimnastika = gimnastika;
        }

        protected override void updateEntity(DomainObject entity)
        {
            DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().Update((TakmicarskaKategorija)entity);
        }

        protected override void addEntity(DomainObject entity)
        {
            TakmicenjeService.addTakmicarskaKategorija((TakmicarskaKategorija)entity, takmicenjeId);
        }

        protected override void checkBusinessRulesOnUpdate(DomainObject entity)
        {
            TakmicarskaKategorija kat = (TakmicarskaKategorija)entity;
            Notification notification = new Notification();

            bool nazivChanged = (kat.Naziv.ToUpper() != oldNaziv.ToUpper()) ? true : false;
            if (nazivChanged
            && DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().existsKategorijaNaziv(kat.Naziv, takmicenjeId))
            {
                notification.RegisterMessage("Naziv", "Kategorija sa datim nazivom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private void TakmicarskaKategorijaForm_Shown(object sender, EventArgs e)
        {
            if (!editMode)
                cmbKategorija.Focus();
            else
                btnCancel.Focus();
        }
    }
}