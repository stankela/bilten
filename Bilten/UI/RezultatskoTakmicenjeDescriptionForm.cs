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
        private int takmicenjeId;
        private string oldNaziv;
        public List<TakmicarskaKategorija> SelKategorije = new List<TakmicarskaKategorija>();

        public RezultatskoTakmicenjeDescriptionForm(Nullable<int> descId, int takmicenjeId)
        {
            InitializeComponent();
            this.takmicenjeId = takmicenjeId;
            initialize(descId, true);
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Takmicenje";

            txtNaziv.Text = String.Empty;
            if (!editMode)
            {
                Takmicenje takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                if (takmicenje.TakmicenjeDescriptions.Count == 0)
                {
                    // za prvo takmicenje, ponudi naziv kao glavno takmicenje
                    txtNaziv.Text = takmicenje.Naziv;
                }
            }

            checkedListBoxKategorije.CheckOnClick = true;

            IList<TakmicarskaKategorija> sveKategorije
                = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().FindByTakmicenje(takmicenjeId);
            IList<TakmicarskaKategorija> kategorije = null;
            if (editMode)
            {
                IList<RezultatskoTakmicenje> rezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                    .FindByTakmicenje(takmicenjeId);
                kategorije = Takmicenje.getKategorije(rezTakmicenja, (RezultatskoTakmicenjeDescription)entity);
            }

            checkedListBoxKategorije.Items.Clear();
            foreach (TakmicarskaKategorija k in sveKategorije)
            {
                checkedListBoxKategorije.Items.Add(k);
                if (!editMode || kategorije.Contains(k))
                    checkedListBoxKategorije.SetItemChecked(checkedListBoxKategorije.Items.Count - 1, true);
            }
        }

        protected override DomainObject getEntityById(int id)
        {
            return DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDescriptionDAO().FindById(id);
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
            // kategorije su checkirane u initUI
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
            SelKategorije.Clear();
            foreach (object item in checkedListBoxKategorije.CheckedItems)
            {
                SelKategorije.Add(item as TakmicarskaKategorija);
            }
        }

        protected override void updateEntity(DomainObject entity)
        {
            RezultatskoTakmicenjeDescription desc = (RezultatskoTakmicenjeDescription)entity;
            DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDescriptionDAO().Update(desc);

            // TODO4: Obradi promenjene kategorije.            
        }

        protected override void addEntity(DomainObject entity)
        {
            RezultatskoTakmicenjeDescription desc = (RezultatskoTakmicenjeDescription)entity;
            Takmicenje takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
            takmicenje.addTakmicenjeDescription(desc);
            DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().Update(takmicenje);

            RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            int redBroj = rezTakDAO.FindMaxRedBroj(takmicenjeId) + 1;
            foreach (TakmicarskaKategorija k in SelKategorije)
                rezTakDAO.Add(createRezultatskoTakmicenje(takmicenje, k, desc, redBroj++));
        }

        private RezultatskoTakmicenje createRezultatskoTakmicenje(Takmicenje takmicenje, TakmicarskaKategorija k,
            RezultatskoTakmicenjeDescription d, int redBroj)
        {
            RezultatskoTakmicenje result = new RezultatskoTakmicenje(takmicenje, k, d, new Propozicije());
            result.RedBroj = (byte)redBroj;
            return result;
        }

        protected override void checkBusinessRulesOnAdd(DomainObject entity)
        {
            RezultatskoTakmicenjeDescription d = (RezultatskoTakmicenjeDescription)entity;
            Notification notification = new Notification();

            if (DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDescriptionDAO()
                .existsDescriptionNaziv(d.Naziv, takmicenjeId))
            {
                notification.RegisterMessage("Naziv", "Takmicenje sa datim nazivom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        protected override void checkBusinessRulesOnUpdate(DomainObject entity)
        {
            RezultatskoTakmicenjeDescription d = (RezultatskoTakmicenjeDescription)entity;
            Notification notification = new Notification();

            bool nazivChanged = (d.Naziv.ToUpper() != oldNaziv.ToUpper()) ? true : false;
            if (nazivChanged && DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDescriptionDAO()
                .existsDescriptionNaziv(d.Naziv, takmicenjeId))
            {
                notification.RegisterMessage("Naziv", "Takmicenje sa datim nazivom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private void RezultatskoTakmicenjeDescriptionForm_Shown(object sender, EventArgs e)
        {
            if (!editMode)
                txtNaziv.Focus();
            else
                btnCancel.Focus();
        }
    }
}