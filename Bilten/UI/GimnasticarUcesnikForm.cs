using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Exceptions;
using Bilten.Domain;
using Bilten.Data.QueryModel;
using Bilten.Data;
using Iesi.Collections.Generic;
using Bilten.Util;

namespace Bilten.UI
{
    public partial class GimnasticarUcesnikForm : EntityDetailForm
    {
        private Nullable<int> oldTakBroj;
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
            rezTakmicenja = loadRezTakmicenja((GimnasticarUcesnik)entity);
        }

        private void loadKlubovi(int takmicenjeId)
        {
            string query = @"from KlubUcesnik k
                    where k.Takmicenje.Id = :takmicenjeId
                    order by k.Naziv";
            IList<KlubUcesnik> result = dataContext.
                ExecuteQuery<KlubUcesnik>(QueryLanguageType.HQL, query,
                        new string[] { "takmicenjeId" },
                        new object[] { takmicenjeId });

            ISet<KlubUcesnik> kluboviSet = new HashedSet<KlubUcesnik>(result);

            string query2 = @"from Klub k
                    order by k.Naziv";
            IList<Klub> result2 = dataContext.
                ExecuteQuery<Klub>(QueryLanguageType.HQL, query2,
                        new string[] { },
                        new object[] { });

            foreach (Klub k in result2)
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
            string query = @"from DrzavaUcesnik d
                    where d.Takmicenje.Id = :takmicenjeId
                    order by d.Naziv";
            IList<DrzavaUcesnik> result = dataContext.
                ExecuteQuery<DrzavaUcesnik>(QueryLanguageType.HQL, query,
                        new string[] { "takmicenjeId" },
                        new object[] { takmicenjeId });
            
            ISet<DrzavaUcesnik> drzaveSet = new HashedSet<DrzavaUcesnik>(result);

            string query2 = @"from Drzava d
                    order by d.Naziv";
            IList<Drzava> result2 = dataContext.
                ExecuteQuery<Drzava>(QueryLanguageType.HQL, query2,
                        new string[] { },
                        new object[] { });

            foreach (Drzava d in result2)
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

        private IList<RezultatskoTakmicenje> loadRezTakmicenja(GimnasticarUcesnik g)
        {
            return dataContext.ExecuteNamedQuery<RezultatskoTakmicenje>(
                "FindRezTakmicenjaForGimnasticar",
                new string[] { "gimnasticar" },
                new object[] { g });
        }

        protected override void initUI()
        {
            base.initUI();
            Text = "Prijavljeni gimnasticar";

            txtIme.Text = String.Empty;
            txtSrednjeIme.Text = String.Empty;
            txtPrezime.Text = String.Empty;
            txtGimnastika.Text = String.Empty;
            txtDatumRodj.Text = String.Empty;
            txtRegBroj.Text = String.Empty;

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
            txtTakBroj.Text = String.Empty;

            lstTakmicenja.HorizontalScrollbar = true;
            lstTakmicenja.Items.Clear();

            rbtKlub.Checked = true;
        }

        protected override DomainObject getEntityById(int id)
        {
            return dataContext.GetById<GimnasticarUcesnik>(id);
        }

        protected override void saveOriginalData(DomainObject entity)
        {
            GimnasticarUcesnik gimnasticar = (GimnasticarUcesnik)entity;
            oldTakBroj = gimnasticar.TakmicarskiBroj;
        }

        protected override void updateUIFromEntity(DomainObject entity)
        {
            GimnasticarUcesnik gimnasticar = (GimnasticarUcesnik)entity;
            txtIme.Text = gimnasticar.Ime;
            txtSrednjeIme.Text = gimnasticar.SrednjeIme;
            txtPrezime.Text = gimnasticar.Prezime;
            txtGimnastika.Text = gimnasticar.Gimnastika.ToString();

            txtDatumRodj.Text = String.Empty;
            if (gimnasticar.DatumRodjenja != null)
                txtDatumRodj.Text = gimnasticar.DatumRodjenja.ToString("d");

            txtRegBroj.Text = String.Empty;
            if (gimnasticar.RegistarskiBroj != null)
                txtRegBroj.Text = gimnasticar.RegistarskiBroj.ToString();

            SelectedKlub = gimnasticar.KlubUcesnik;
            SelectedDrzava = gimnasticar.DrzavaUcesnik;
            txtTakKategorija.Text = kategorija.ToString();

            txtTakBroj.Text = String.Empty;
            if (gimnasticar.TakmicarskiBroj != null)
                txtTakBroj.Text = gimnasticar.TakmicarskiBroj.ToString();

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                lstTakmicenja.Items.Add(rt.Naziv);

            rbtDrzava.Checked = gimnasticar.NastupaZaDrzavu;
        }

        protected override void requiredFieldsAndFormatValidation(Notification notification)
        {
            Datum dummyDatum;
            int dummyInt;
            if (txtDatumRodj.Text.Trim() != String.Empty
            && !Datum.TryParse(txtDatumRodj.Text, out dummyDatum))
            {
                notification.RegisterMessage(
                    "DatumRodjenja", "Neispravan format za datum ili godinu rodjenja.");
            }
            if (txtTakBroj.Text.Trim() != String.Empty &&
                !int.TryParse(txtTakBroj.Text, out dummyInt))
            {
                notification.RegisterMessage(
                    "TakmicarskiBroj", "Neispravan format za takmicarski broj.");
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

                case "TakmicarskiBroj":
                    txtTakBroj.Focus();
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

            if (txtTakBroj.Text.Trim() == String.Empty)
                gimnasticar.TakmicarskiBroj = null;
            else
                gimnasticar.TakmicarskiBroj = int.Parse(txtTakBroj.Text);

            gimnasticar.KlubUcesnik = SelectedKlub;
            if (gimnasticar.KlubUcesnik != null && gimnasticar.KlubUcesnik.Naziv == PRAZNO)
                gimnasticar.KlubUcesnik = null;
            
            gimnasticar.DrzavaUcesnik = SelectedDrzava;
            if (gimnasticar.DrzavaUcesnik != null && gimnasticar.DrzavaUcesnik.Naziv == PRAZNO)
                gimnasticar.DrzavaUcesnik = null;
            
            gimnasticar.NastupaZaDrzavu = rbtDrzava.Checked;
        }

        protected override void checkBusinessRulesOnUpdate(DomainObject entity)
        {
            GimnasticarUcesnik gimnasticar = (GimnasticarUcesnik)entity;
            Notification notification = new Notification();

            bool takBrojChanged =
                (gimnasticar.TakmicarskiBroj != oldTakBroj) ? true : false;
            if (takBrojChanged && gimnasticar.TakmicarskiBroj != null
            && existsGimnasticarTakBroj(gimnasticar.TakmicarskiBroj.Value))
            {
                notification.RegisterMessage("TakmicarskiBroj",
                    "Gimnasticar sa datim takmicarskim brojem vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private bool existsGimnasticarTakBroj(int takBroj)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("TakmicarskiBroj", CriteriaOperator.Equal, takBroj));
            q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, kategorija.Takmicenje));
            q.Operator = QueryOperator.And;
            return dataContext.GetCount<GimnasticarUcesnik>(q) > 0;
        }

        protected override void updateEntity(DomainObject entity)
        {
            GimnasticarUcesnik g = (GimnasticarUcesnik)entity;
            if (g.KlubUcesnik != null && g.KlubUcesnik.Id == 0)
            {
                g.KlubUcesnik.Takmicenje = kategorija.Takmicenje;
                dataContext.Add(g.KlubUcesnik);
            }
            if (g.DrzavaUcesnik != null && g.DrzavaUcesnik.Id == 0)
            {
                g.DrzavaUcesnik.Takmicenje = kategorija.Takmicenje;
                dataContext.Add(g.DrzavaUcesnik);
            }
            dataContext.Save(g);
        }

    }
}