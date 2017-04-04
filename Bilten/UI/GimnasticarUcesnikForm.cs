using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Exceptions;
using Bilten.Domain;
using Bilten.Data;
using Bilten.Util;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class GimnasticarUcesnikForm : EntityDetailForm
    {
        private TakmicarskaKategorija kategorija;
        private List<KlubUcesnik> klubovi;
        private List<DrzavaUcesnik> drzave;
        private IList<RezultatskoTakmicenje> rezTakmicenja;

        private KlubUcesnik emptyKlub;
        private DrzavaUcesnik emptyDrzava;
        private readonly string PRAZNO = "<<Prazno>>";

        private KlubUcesnik SelectedKlub
        {
            get { return cmbKlub.SelectedItem as KlubUcesnik; }
            set { cmbKlub.SelectedItem = value; }
        }

        private DrzavaUcesnik SelectedDrzava
        {
            get { return cmbDrzava.SelectedItem as DrzavaUcesnik; }
            set { cmbDrzava.SelectedItem = value; }
        }
        
        // TODO: Dodaj mogucnost promene kategorije (samo bi se promenila kategorija dok bi sve drugo ostalo
        // isto - gimnasticar bi i dalje bio u istim takmicenjima, ekipama, imao bi iste ocene. Prvo proveri da
        // li je ovo uopste moguce, tj. da li nece da dovede do greske u nekom drugom delu programa).

        public GimnasticarUcesnikForm(Nullable<int> gimnasticarUcesnikId,
            TakmicarskaKategorija kategorija)
        {
            if (gimnasticarUcesnikId == null)
                throw new ArgumentException("GimnasticarUcesnikForm only works in edit mode.");
            InitializeComponent();

            emptyKlub = new KlubUcesnik();
            emptyKlub.Naziv = PRAZNO;
            emptyDrzava = new DrzavaUcesnik();
            emptyDrzava.Naziv = PRAZNO;

            this.kategorija = kategorija;
            initialize(gimnasticarUcesnikId, true);
        }

        protected override void loadData()
        {
            loadKlubovi(kategorija.Takmicenje.Id);
            loadDrzave(kategorija.Takmicenje.Id);
            rezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                                            .FindRezTakmicenjaForGimnasticar((GimnasticarUcesnik)entity);
        }

        private void loadKlubovi(int takmicenjeId)
        {
            ISet<KlubUcesnik> kluboviSet = new HashSet<KlubUcesnik>(
                DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO().FindByTakmicenje(takmicenjeId));

            foreach (Klub k in DAOFactoryFactory.DAOFactory.GetKlubDAO().FindAll())
            { 
                KlubUcesnik ku = new KlubUcesnik();
                ku.Naziv = k.Naziv;
                ku.Kod = k.Kod;
                if (!kluboviSet.Contains(ku))
                {
                    kluboviSet.Add(ku);
                }
            }

            klubovi = new List<KlubUcesnik>(kluboviSet);
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(KlubUcesnik))["Naziv"];
            klubovi.Sort(new SortComparer<KlubUcesnik>(propDesc, ListSortDirection.Ascending));

            klubovi.Insert(0, emptyKlub);
        }

        private void loadDrzave(int takmicenjeId)
        {
            ISet<DrzavaUcesnik> drzaveSet = new HashSet<DrzavaUcesnik>(
                DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO().FindByTakmicenje(takmicenjeId));

            foreach (Drzava d in DAOFactoryFactory.DAOFactory.GetDrzavaDAO().FindAll())
            {
                DrzavaUcesnik du = new DrzavaUcesnik();
                du.Naziv = d.Naziv;
                du.Kod = d.Kod;
                if (!drzaveSet.Contains(du))
                {
                    drzaveSet.Add(du);
                }
            }

            drzave = new List<DrzavaUcesnik>(drzaveSet);
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(DrzavaUcesnik))["Naziv"];
            drzave.Sort(new SortComparer<DrzavaUcesnik>(propDesc, ListSortDirection.Ascending));

            drzave.Insert(0, emptyDrzava);
        }

        protected override void initUI()
        {
            base.initUI();
            Text = "Prijavljeni gimnasticar";

            txtIme.Text = String.Empty;
            txtSrednjeIme.Text = String.Empty;
            txtPrezime.Text = String.Empty;
            txtDatumRodj.Text = String.Empty;

            cmbKlub.DropDownStyle = ComboBoxStyle.DropDown;
            cmbKlub.DataSource = klubovi;
            cmbKlub.DisplayMember = "Naziv";
            cmbKlub.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbKlub.AutoCompleteSource = AutoCompleteSource.ListItems;

            cmbDrzava.DropDownStyle = ComboBoxStyle.DropDown;
            cmbDrzava.DataSource = drzave;
            cmbDrzava.DisplayMember = "Naziv";
            cmbDrzava.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbDrzava.AutoCompleteSource = AutoCompleteSource.ListItems;

            txtTakKategorija.Text = kategorija.ToString();

            lstTakmicenja.HorizontalScrollbar = true;
            lstTakmicenja.Items.Clear();

            rbtKlub.Checked = true;
        }

        protected override DomainObject getEntityById(int id)
        {
            return DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO().FindById(id);
        }

        protected override void updateUIFromEntity(DomainObject entity)
        {
            GimnasticarUcesnik gimnasticar = (GimnasticarUcesnik)entity;
            txtIme.Text = gimnasticar.Ime;
            txtSrednjeIme.Text = gimnasticar.SrednjeIme;
            txtPrezime.Text = gimnasticar.Prezime;

            txtDatumRodj.Text = String.Empty;
            if (gimnasticar.DatumRodjenja != null)
                txtDatumRodj.Text = gimnasticar.DatumRodjenja.ToString("d");

            SelectedKlub = gimnasticar.KlubUcesnik;
            SelectedDrzava = gimnasticar.DrzavaUcesnik;
            txtTakKategorija.Text = kategorija.ToString();

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                lstTakmicenja.Items.Add(rt.Naziv);

            rbtDrzava.Checked = gimnasticar.NastupaZaDrzavu;
        }

        protected override void requiredFieldsAndFormatValidation(Notification notification)
        {
            Datum dummyDatum;
            if (txtDatumRodj.Text.Trim() != String.Empty
            && !Datum.TryParse(txtDatumRodj.Text, out dummyDatum))
            {
                notification.RegisterMessage(
                    "DatumRodjenja", "Neispravan format za datum ili godinu rodjenja.");
            }
            if (cmbKlub.Text.Trim() != String.Empty && cmbKlub.Text.Trim() != PRAZNO && SelectedKlub == null)
            {
                notification.RegisterMessage(
                    "Klub", "Uneli ste nepostojeci klub.");
            }
            if (cmbDrzava.Text.Trim() != String.Empty && cmbDrzava.Text.Trim() != PRAZNO && SelectedDrzava == null)
            {
                notification.RegisterMessage(
                    "Drzava", "Uneli ste nepostojecu drzavu.");
            }
        }

        protected override void setFocus(string propertyName)
        {
            switch (propertyName)
            {
                case "DatumRodjenja":
                    txtDatumRodj.Focus();
                    break;

                case "Klub":
                    cmbKlub.Focus();
                    break;

                case "Drzava":
                    cmbDrzava.Focus();
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        protected override void updateEntityFromUI(DomainObject entity)
        {
            GimnasticarUcesnik gimnasticar = (GimnasticarUcesnik)entity;
            if (txtDatumRodj.Text.Trim() == String.Empty)
                gimnasticar.DatumRodjenja = null;
            else
                gimnasticar.DatumRodjenja = Datum.Parse(txtDatumRodj.Text);

            gimnasticar.KlubUcesnik = SelectedKlub;
            if (gimnasticar.KlubUcesnik != null && gimnasticar.KlubUcesnik.Naziv == PRAZNO)
                gimnasticar.KlubUcesnik = null;
            
            gimnasticar.DrzavaUcesnik = SelectedDrzava;
            if (gimnasticar.DrzavaUcesnik != null && gimnasticar.DrzavaUcesnik.Naziv == PRAZNO)
                gimnasticar.DrzavaUcesnik = null;
            
            gimnasticar.NastupaZaDrzavu = rbtDrzava.Checked;
        }

        protected override void updateEntity(DomainObject entity)
        {
            GimnasticarUcesnik g = (GimnasticarUcesnik)entity;
            if (g.KlubUcesnik != null && g.KlubUcesnik.Id == 0)
            {
                g.KlubUcesnik.Takmicenje = kategorija.Takmicenje;
                DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO().Add(g.KlubUcesnik);
            }
            if (g.DrzavaUcesnik != null && g.DrzavaUcesnik.Id == 0)
            {
                g.DrzavaUcesnik.Takmicenje = kategorija.Takmicenje;
                DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO().Add(g.DrzavaUcesnik);
            }
            DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO().Update(g);
        }

    }
}