using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Exceptions;
using Bilten.Data;
using Iesi.Collections.Generic;
using System.Collections;
using Bilten.Util;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class EkipaForm : EntityDetailForm
    {
        private int rezTakmicenjeId;
        private RezultatskoTakmicenje rezTakmicenje;
        private List<KlubUcesnik> klubovi;
        private List<DrzavaUcesnik> drzave;
        private string oldNaziv;
        private string oldKod;
        private bool clanoviSorted;

        public EkipaForm(Nullable<int> ekipaId, int rezTakmicenjeId)
        {
            InitializeComponent();
            this.rezTakmicenjeId = rezTakmicenjeId;
            initialize(ekipaId, true);
        }

        protected override void initAddMode()
        {
            base.initAddMode();
            Ekipa ekipa = (Ekipa)entity;
            setClanovi(ekipa.Gimnasticari);
        }

        protected override void initUpdateMode(int entityId)
        {
            // NOTE: Ovde je promenjen redosled inicijalizacije za update rezim rada
            // tako da se prvo ucitavaju podaci (loadData), pa tek onda objekt. Razlog
            // je sledeci. loadData dobavlja klubove i drzave ucesnike pomocu
            // join operacije sa gimnasticarima ucesnicima, a u getEntityById se pored
            // ekipe takodje dobavljaju i gimnasticari ucesnici (eager fetch). Kada se 
            // getEntityId poziva pre loadData, tada se dobavljeni gimnasticari 
            // ucesnici iz metoda getEntityId najpre azuriraju u bazi (cak i ako nisu
            // menjani) da bi loadData imao najnoviju verziju gimnasticara ucesnika. 
            // Promenom redosleda pozivanja ova dva metoda azuriranje se izbegava.

            loadData();
            entity = getEntityById(entityId);
            saveOriginalData(entity);
            initUI();
            updateUIFromEntity(entity);
        }

        protected override void loadData()
        {
            // NOTE: Iako se prvo ucitava ekipa (tj. entity) pa tek onda rezTakmicenje,
            // objekat ekipe se nalazi unutar kolekcije Takmicenje1.Ekipe objekta 
            // rezTakmicenje jer se sve obavlja unutar iste sesije. To znaci da u edit 
            // modu rezTakmicenje "vidi" sve promene na ekipi.
            rezTakmicenje = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO().loadRezTakmicenje(rezTakmicenjeId);
            klubovi = new List<KlubUcesnik>(
                DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO().FindByTakmicenje(rezTakmicenje.Takmicenje.Id));
            drzave = new List<DrzavaUcesnik>(
                DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO().FindByTakmicenje(rezTakmicenje.Takmicenje.Id));
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Ekipa";

            txtRezTakmicenje.Text = rezTakmicenje.Naziv;
            txtNaziv.Text = String.Empty;
            txtKod.Text = String.Empty;

            cmbKlub.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKlub.Items.Add("<<Prazno>>");
            cmbKlub.Items.AddRange(klubovi.ToArray());
            cmbKlub.SelectedIndex = -1;

            cmbDrzava.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDrzava.Items.Add("<<Prazno>>");
            cmbDrzava.Items.AddRange(drzave.ToArray());
            cmbDrzava.SelectedIndex = -1;

            dgwUserControlClanovi.DataGridView.MultiSelect = false;
            addClanoviColumns();
        }

        private void addClanoviColumns()
        {
            // TODO: Ovo je delom prekopirano iz EkipeForm. Pokusaj da sve objedinis na
            // jednom mestu
            dgwUserControlClanovi.AddColumn("Ime", "Ime", 100);
            dgwUserControlClanovi.AddColumn("Prezime", "Prezime", 100);
            dgwUserControlClanovi.AddColumn("Datum rodjenja", "DatumRodjenja", 100, "{0:d}");
            dgwUserControlClanovi.AddColumn("Takmicarski broj", "TakmicarskiBroj", 100);
            dgwUserControlClanovi.AddColumn("Klub", "KlubUcesnik", 150);
            dgwUserControlClanovi.AddColumn("Drzava", "DrzavaUcesnik", 100);
        }

        protected override DomainObject getEntityById(int id)
        {
            return DAOFactoryFactory.DAOFactory.GetEkipaDAO().FindEkipaById(id);
        }

        protected override void saveOriginalData(DomainObject entity)
        {
            Ekipa ekipa = (Ekipa)entity;
            oldNaziv = ekipa.Naziv;
            oldKod = ekipa.Kod;
        }

        protected override void updateUIFromEntity(DomainObject entity)
        {
            Ekipa ekipa = (Ekipa)entity;

            cmbKlub.SelectedIndex = -1;
            for (int i = 0; i < cmbKlub.Items.Count; ++i)
            {
                KlubUcesnik k = cmbKlub.Items[i] as KlubUcesnik;
                if (k != null && k.Naziv == ekipa.Naziv)
                {
                    rbtKlub.Checked = true;
                    //cmbKlub.SelectedItem = k;
                    cmbKlub.SelectedIndex = i;
                    break;
                }
            }

            cmbDrzava.SelectedIndex = -1;
            for (int i = 0; i < cmbDrzava.Items.Count; ++i)
            {
                DrzavaUcesnik d = cmbDrzava.Items[i] as DrzavaUcesnik;
                if (d != null && d.Naziv == ekipa.Naziv)
                {
                    rbtDrzava.Checked = true;
                    cmbDrzava.SelectedIndex = i;
                    break;
                }
            }

            // bitno je da ove dve naredbe idu posle naredbi kojima se podesava
            // drzava ili klub, za slucaj da su naziv i kod uneti rucno (da bi se
            // prebrisale automatske vrednosti koje se zadaju kada se izabere drzava
            // ili klub)
            txtNaziv.Text = ekipa.Naziv;
            txtKod.Text = ekipa.Kod;

            setClanovi(ekipa.Gimnasticari);
        }

        private void setClanovi(Iesi.Collections.Generic.ISet<GimnasticarUcesnik> gimnasticari)
        {
            dgwUserControlClanovi.setItems<GimnasticarUcesnik>(gimnasticari);
            if (!clanoviSorted)
            {
                dgwUserControlClanovi.sort<GimnasticarUcesnik>(
                    new string[] { "Prezime", "Ime" },
                    new ListSortDirection[] { ListSortDirection.Ascending, ListSortDirection.Ascending });
                clanoviSorted = true;
            }
        }

        protected override void requiredFieldsAndFormatValidation(Notification notification)
        {
            if (txtNaziv.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv ekipe je obavezan.");
            }

            if (txtKod.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Kod", "Kod ekipe je obavezan.");
            }
        }

        protected override void setFocus(string propertyName)
        {
            switch (propertyName)
            {
                case "RezultatskoTakmicenje":
                    txtRezTakmicenje.Focus();
                    break;

                case "Naziv":
                    txtNaziv.Focus();
                    break;

                case "Kod":
                    txtKod.Focus();
                    break;

                case "Gimnasticari":
                    dgwUserControlClanovi.DataGridView.Focus();
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        protected override DomainObject createNewEntity()
        {
            return new Ekipa();
        }

        protected override void updateEntityFromUI(DomainObject entity)
        {
            Ekipa ekipa = (Ekipa)entity;
            ekipa.Naziv = txtNaziv.Text.Trim();
            ekipa.Kod = txtKod.Text.Trim().ToUpper();
        }

        protected override void checkBusinessRulesOnAdd(DomainObject entity)
        {
            Ekipa ekipa = (Ekipa)entity;
            Notification notification = new Notification();
            EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();

            if (ekipaDAO.existsEkipaNaziv(rezTakmicenje.Id, ekipa.Naziv))
            {
                notification.RegisterMessage("Naziv", "Ekipa sa datim nazivom vec postoji.");
                throw new BusinessException(notification);
            }

            if (ekipaDAO.existsEkipaKod(rezTakmicenje.Id, ekipa.Kod))
            {
                notification.RegisterMessage("Kod", "Ekipa sa datim kodom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        protected override void addEntity(DomainObject entity)
        {
            Ekipa ekipa = (Ekipa)entity;
            rezTakmicenje.Takmicenje1.addEkipa(ekipa);
            DAOFactoryFactory.DAOFactory.GetEkipaDAO().Add(ekipa);
            DAOFactoryFactory.DAOFactory.GetTakmicenje1DAO().Update(rezTakmicenje.Takmicenje1);
        }

        protected override void updateEntity(DomainObject entity)
        {
            DAOFactoryFactory.DAOFactory.GetEkipaDAO().Update((Ekipa)entity);
        }

        protected override void checkBusinessRulesOnUpdate(DomainObject entity)
        {
            Ekipa ekipa = (Ekipa)entity;
            Notification notification = new Notification();
            EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();

            bool nazivChanged = (ekipa.Naziv.ToUpper() != oldNaziv.ToUpper()) ? true : false;
            if (nazivChanged && ekipaDAO.existsEkipaNaziv(rezTakmicenje.Id, ekipa.Naziv))
            {
                notification.RegisterMessage("Naziv", "Ekipa sa datim nazivom vec postoji.");
                throw new BusinessException(notification);
            }

            bool kodChanged = (ekipa.Kod.ToUpper() != oldKod.ToUpper()) ? true : false;
            if (kodChanged && ekipaDAO.existsEkipaKod(rezTakmicenje.Id, ekipa.Kod))
            {
                notification.RegisterMessage("Kod", "Ekipa sa datim kodom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult = DialogResult.None;
            SelectGimnasticarUcesnikForm form = null;
            try
            {
                form = new SelectGimnasticarUcesnikForm(rezTakmicenje.Takmicenje.Id, 
                    rezTakmicenje.Gimnastika, null);
                dlgResult = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (dlgResult != DialogResult.OK || form.SelectedEntities.Count == 0)
                return;

            Ekipa ekipa = (Ekipa)entity;
            List<GimnasticarUcesnik> okGimnasticari = new List<GimnasticarUcesnik>();
            List<GimnasticarUcesnik> illegalGimnasticari = new List<GimnasticarUcesnik>();
            foreach (GimnasticarUcesnik g in form.SelectedEntities)
            {
                if (canAddGimnasticar(ekipa, g))
                {
                    ekipa.addGimnasticar(g);
                    okGimnasticari.Add(g);
                }
                else
                {
                    illegalGimnasticari.Add(g);
                }

            }
            if (okGimnasticari.Count > 0)
            {
                setClanovi(ekipa.Gimnasticari);
                dgwUserControlClanovi.setSelectedItem<GimnasticarUcesnik>
                    (okGimnasticari[okGimnasticari.Count - 1]);
            }

            if (illegalGimnasticari.Count > 0)
            {
                string msg = "Sledeci gimnasticari nisu dodati: \n\n";
                msg += StringUtil.getListString(illegalGimnasticari.ToArray());
                //       MessageDialogs.showMessage(msg, this.Text);
            }
        }

        private bool canAddGimnasticar(Ekipa ekipa, GimnasticarUcesnik gimnasticar)
        {
            // TODO: Najpre proveri da li ekipa vec ima max broj clanova. Takodje, 
            // razmisli da li clan ekipe mora da bude i ucesnik rez. takmicenja
            // u kome se ekipa takmici.

            return !ekipa.Gimnasticari.Contains(gimnasticar);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            GimnasticarUcesnik selClan = 
                dgwUserControlClanovi.getSelectedItem<GimnasticarUcesnik>();
            if (selClan == null)
                return;
            string msgFmt = "Da li zelite da izbrisete clana ekipe \"{0}\"?";
            if (!MessageDialogs.queryConfirmation(String.Format(msgFmt, selClan), this.Text))
                return;
            
            Ekipa ekipa = (Ekipa)entity;
            // TODO: Check business rules
            ekipa.removeGimnasticar(selClan);

            setClanovi(ekipa.Gimnasticari);
            dgwUserControlClanovi.clearSelection();
        }

        private void rbtKlub_CheckedChanged(object sender, EventArgs e)
        {
            onKlubDrzavaCheckedChanged();
        }

        private void rbtDrzava_CheckedChanged(object sender, EventArgs e)
        {
            onKlubDrzavaCheckedChanged();
        }

        private void onKlubDrzavaCheckedChanged()
        {
            updateKlubDrzavaEnabledStatus();
            updateNazivKod();
        }

        private void updateKlubDrzavaEnabledStatus()
        {
            if (rbtKlub.Checked)
            {
                cmbKlub.Enabled = true;
                chbSkola.Enabled = true;

                cmbDrzava.Enabled = false;
            }
            else if (rbtDrzava.Checked)
            {
                cmbKlub.Enabled = false;
                chbSkola.Enabled = false;

                cmbDrzava.Enabled = true;
            }
        }

        private void updateNazivKod()
        {
            txtNaziv.Text = String.Empty;
            txtKod.Text = String.Empty;
            if (rbtKlub.Checked)
            {
                KlubUcesnik klub = cmbKlub.SelectedItem as KlubUcesnik;
                if (klub != null)
                {
                    txtNaziv.Text = klub.Naziv;
                    txtKod.Text = klub.Kod;
                }
            }
            else if (rbtDrzava.Checked)
            {
                DrzavaUcesnik drzava = cmbDrzava.SelectedItem as DrzavaUcesnik;
                if (drzava != null)
                {
                    txtNaziv.Text = drzava.Naziv;
                    txtKod.Text = drzava.Kod;
                }
            }
        }

        private void cmbKlub_SelectedIndexChanged(object sender, EventArgs e)
        {
            rbtKlub.Checked = true;
            updateNazivKod();
        }

        private void cmbDrzava_SelectedIndexChanged(object sender, EventArgs e)
        {
            rbtDrzava.Checked = true;
            updateNazivKod();
        }
    }
}