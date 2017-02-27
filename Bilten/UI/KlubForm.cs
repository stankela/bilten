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
    public partial class KlubForm : EntityDetailForm
    {
        private List<Mesto> mesta;
        private string oldNaziv;
        private string oldKod;
        
        public KlubForm(Nullable<int> klubId)
        {
            InitializeComponent();
            initialize(klubId, true);
        }

        protected override void loadData()
        {
            mesta = new List<Mesto>(DAOFactoryFactory.DAOFactory.GetMestoDAO().FindAll(true));
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Klub";
            txtNaziv.Text = String.Empty;
            txtKod.Text = String.Empty;

            cmbMesto.DropDownStyle = ComboBoxStyle.DropDownList;
            setMesta(mesta);
            SelectedMesto = null;
        }

        private void setMesta(List<Mesto> mesta)
        {
            cmbMesto.DisplayMember = "Naziv";
            cmbMesto.DataSource = mesta;
            
            // NOTE: Ako se referenca na listu 'mesta' vec nalazi u DataSource
            // svojstvu (tj. ako DataSource svojstvo ostaje nepromenjeno) combo
            // nece biti osvezen. Zato je potrebno osveziti ga rucno.
            CurrencyManager currencyManager =
                (CurrencyManager)this.BindingContext[mesta];
            currencyManager.Refresh();
        }

        private Mesto SelectedMesto
        {
            get { return cmbMesto.SelectedItem as Mesto; }
            set { cmbMesto.SelectedItem = value; }
        }

        protected override DomainObject getEntityById(int id)
        {
            return DAOFactoryFactory.DAOFactory.GetKlubDAO().FindById(id);
        }

        protected override void saveOriginalData(DomainObject entity)
        {
            Klub klub = (Klub)entity;
            oldNaziv = klub.Naziv;
            oldKod = klub.Kod;
        }

        protected override void updateUIFromEntity(DomainObject entity)
        {
            Klub klub = (Klub)entity;
            txtNaziv.Text = klub.Naziv;
            txtKod.Text = klub.Kod;

            SelectedMesto = klub.Mesto;
        }

        protected override void requiredFieldsAndFormatValidation(Notification notification)
        {
            if (txtNaziv.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv kluba je obavezan.");
            }

            if (txtKod.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Kod", "Kod kluba je obavezan.");
            }

            if (cmbMesto.SelectedIndex == -1)
            {
                notification.RegisterMessage(
                    "Mesto", "Mesto kluba je obavezno.");
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

                case "Mesto":
                    cmbMesto.Focus();
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        protected override DomainObject createNewEntity()
        {
            return new Klub();
        }

        protected override void updateEntityFromUI(DomainObject entity)
        {
            Klub klub = (Klub)entity;
            klub.Naziv = txtNaziv.Text.Trim();
            klub.Kod = txtKod.Text.Trim().ToUpper();
            klub.Mesto = SelectedMesto;
        }

        protected override void updateEntity(DomainObject entity)
        {
            DAOFactoryFactory.DAOFactory.GetKlubDAO().Update((Klub)entity);
        }

        protected override void addEntity(DomainObject entity)
        {
            DAOFactoryFactory.DAOFactory.GetKlubDAO().Add((Klub)entity);
        }

        protected override void checkBusinessRulesOnAdd(DomainObject entity)
        {
            Klub klub = (Klub)entity;
            Notification notification = new Notification();

            KlubDAO klubDAO = DAOFactoryFactory.DAOFactory.GetKlubDAO();
            if (klubDAO.existsKlubNaziv(klub.Naziv))
            {
                notification.RegisterMessage("Naziv", "Klub sa datim nazivom vec postoji.");
                throw new BusinessException(notification);
            }

            if (klubDAO.existsKlubKod(klub.Kod))
            {
                notification.RegisterMessage("Kod", "Klub sa datim kodom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        protected override void checkBusinessRulesOnUpdate(DomainObject entity)
        {
            Klub klub = (Klub)entity;
            Notification notification = new Notification();
            KlubDAO klubDAO = DAOFactoryFactory.DAOFactory.GetKlubDAO();

            bool nazivChanged = (klub.Naziv.ToUpper() != oldNaziv.ToUpper()) ? true : false;
            if (nazivChanged && klubDAO.existsKlubNaziv(klub.Naziv))
            {
                notification.RegisterMessage("Naziv", "Klub sa datim nazivom vec postoji.");
                throw new BusinessException(notification);
            }

            bool kodChanged = (klub.Kod.ToUpper() != oldKod.ToUpper()) ? true : false;
            if (kodChanged && klubDAO.existsKlubKod(klub.Kod))
            {
                notification.RegisterMessage("Kod", "Klub sa datim kodom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private void btnAddMesto_Click(object sender, EventArgs e)
        {
            try
            {
                MestoForm form = new MestoForm(null);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Mesto m = (Mesto)form.Entity;
                    mesta.Add(m);

                    // NOTE: Nije potrebno zamrzavati combo (pomocu
                    // currencyManager.SuspendBinding) za vreme sortiranja, zato sto 
                    // se kao binding kolekcija koristi List (a ona ne reflektuje
                    // automatski promene na UI kontrolu)              
                    mesta.Sort();

                    setMesta(mesta);
                    SelectedMesto = m;

                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }
    }
}

