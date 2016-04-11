using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Data.QueryModel;
using Bilten.Exceptions;
using Bilten.Data;
using System.Collections.Specialized;
using Bilten.Util;

namespace Bilten.UI
{
    public partial class GimnasticarForm : EntityDetailForm
    {
        private List<KategorijaGimnasticara> kategorije;
        private List<Klub> klubovi;
        private List<Drzava> drzave;
        private string oldIme;
        private string oldPrezime;
        private string oldSrednjeIme;
        private Datum oldDatumRodjenja;
        private RegistarskiBroj oldRegBroj;
        private readonly string PRAZNO_ITEM = "<<Prazno>>";
        private List<Gimnasticar> gimnasticari;

        private Gimnasticar gimnasticarToEdit;
        public Gimnasticar GimnasticarToEdit
        {
            get { return gimnasticarToEdit; }
        }

        public GimnasticarForm(Nullable<int> gimnasticarId)
        {
            InitializeComponent();
            initialize(gimnasticarId, true);
        }

        protected override void loadData()
        {
            if (editMode)
                kategorije = loadKategorije(((Gimnasticar)entity).Gimnastika);
            else
                kategorije = new List<KategorijaGimnasticara>();

            Query q = new Query();
            q.OrderClauses.Add(new OrderClause("Naziv", OrderClause.OrderClauseCriteria.Ascending));
            klubovi = new List<Klub>(dataContext.GetByCriteria<Klub>(q));

            q = new Query();
            q.OrderClauses.Add(new OrderClause("Naziv", OrderClause.OrderClauseCriteria.Ascending));
            drzave = new List<Drzava>(dataContext.GetByCriteria<Drzava>(q));

            q = new Query();
            q.OrderClauses.Add(new OrderClause("Prezime", OrderClause.OrderClauseCriteria.Ascending));
            gimnasticari = new List<Gimnasticar>(dataContext.GetByCriteria<Gimnasticar>(q));
        }

        private List<KategorijaGimnasticara> loadKategorije(Gimnastika gimnastika)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Gimnastika", CriteriaOperator.Equal, (byte)gimnastika));
            q.OrderClauses.Add(new OrderClause("Naziv", OrderClause.OrderClauseCriteria.Ascending));
            return new List<KategorijaGimnasticara>(
                dataContext.GetByCriteria<KategorijaGimnasticara>(q));
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Gimnasticar";

            txtIme.Text = String.Empty;
            txtSrednjeIme.Text = String.Empty;

            txtPrezime.Text = String.Empty;
            txtPrezime.AutoCompleteMode = AutoCompleteMode.Suggest;
            txtPrezime.AutoCompleteSource = AutoCompleteSource.CustomSource;

            txtDatRodj.Text = String.Empty;
            txtRegBroj.Text = String.Empty;
            txtDatumPoslReg.Text = String.Empty;

            cmbGimnastika.DropDownStyle = ComboBoxStyle.DropDownList;
            setGimnastike();
            SelectedGimnastika = Gimnastika.Undefined;

            cmbKategorija.DropDownStyle = ComboBoxStyle.DropDown;
            setKategorije(kategorije);
            SelectedKategorija = null;
            cmbKategorija.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbKategorija.AutoCompleteSource = AutoCompleteSource.ListItems;

            cmbKlub.DropDownStyle = ComboBoxStyle.DropDown;
            setKlubovi(klubovi);
            SelectedKlub = null;
            cmbKlub.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbKlub.AutoCompleteSource = AutoCompleteSource.ListItems;

            cmbDrzava.DropDownStyle = ComboBoxStyle.DropDown;
            setDrzave(drzave);
            SelectedDrzava = getSrbija();
            cmbDrzava.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbDrzava.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private Drzava getSrbija()
        {
            foreach (Drzava d in drzave)
            {
                if (d.Naziv.ToUpper() == "SRBIJA")
                    return d;
            }
            return null;
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

        private void setDrzave(List<Drzava> drzave)
        {
            cmbDrzava.DisplayMember = "Naziv";
            cmbDrzava.DataSource = drzave;

            CurrencyManager currencyManager =
                (CurrencyManager)this.BindingContext[drzave];
            currencyManager.Refresh();
        }

        private Drzava SelectedDrzava
        {
            get { return cmbDrzava.SelectedItem as Drzava; }
            set { cmbDrzava.SelectedItem = value; }
        }

        private void setKategorije(List<KategorijaGimnasticara> kategorije)
        {
            List<object> items = new List<object>();
            items.Add(PRAZNO_ITEM);
            items.AddRange(kategorije.ToArray());
            cmbKategorija.DisplayMember = "Naziv";
            cmbKategorija.DataSource = items;
            // NOTE: If the specified DisplayMember property does not exist on the
            // object (as is the case for PRAZNO_ITEM) or the value of DisplayMember
            // is an empty string (""), the results of the object's ToString method
            // are displayed instead.
        }

        private KategorijaGimnasticara SelectedKategorija
        {
            get { return cmbKategorija.SelectedItem as KategorijaGimnasticara; }
            set { cmbKategorija.SelectedItem = value; }
        }

        private void setKlubovi(List<Klub> klubovi)
        {
            List<object> items = new List<object>();
            items.Add(PRAZNO_ITEM);
            items.AddRange(klubovi.ToArray());
            cmbKlub.DisplayMember = "Naziv";
            cmbKlub.DataSource = items;
        }

        private Klub SelectedKlub
        {
            get { return cmbKlub.SelectedItem as Klub; }
            set { cmbKlub.SelectedItem = value; }
        }

        protected override DomainObject getEntityById(int id)
        {
            return dataContext.GetById<Gimnasticar>(id);
        }

        protected override void saveOriginalData(DomainObject entity)
        {
            Gimnasticar gimnasticar = (Gimnasticar)entity;
            oldIme = gimnasticar.Ime;
            oldSrednjeIme = gimnasticar.SrednjeIme;
            oldPrezime = gimnasticar.Prezime;
            oldDatumRodjenja = gimnasticar.DatumRodjenja;
            oldRegBroj = gimnasticar.RegistarskiBroj;
        }

        protected override void updateUIFromEntity(DomainObject entity)
        {
            Gimnasticar gimnasticar = (Gimnasticar)entity;
            txtIme.Text = gimnasticar.Ime;

            txtSrednjeIme.Text = String.Empty;
            if (!string.IsNullOrEmpty(gimnasticar.SrednjeIme))
                txtSrednjeIme.Text = gimnasticar.SrednjeIme;

            txtPrezime.Text = gimnasticar.Prezime;

            txtDatRodj.Text = String.Empty;
            if (gimnasticar.DatumRodjenja != null)
                txtDatRodj.Text = gimnasticar.DatumRodjenja.ToString("d");

            txtRegBroj.Text = String.Empty;
            if (gimnasticar.RegistarskiBroj != null)
                txtRegBroj.Text = gimnasticar.RegistarskiBroj.ToString();

            txtDatumPoslReg.Text = String.Empty;
            if (gimnasticar.DatumPoslednjeRegistracije != null)
                txtDatumPoslReg.Text = gimnasticar.DatumPoslednjeRegistracije.ToString("d");

            SelectedGimnastika = gimnasticar.Gimnastika;

            SelectedKategorija = gimnasticar.Kategorija;
            SelectedKlub = gimnasticar.Klub;
            SelectedDrzava = gimnasticar.Drzava;
        }

        protected override void requiredFieldsAndFormatValidation(Notification notification)
        {
            Datum dummyDatum;
            RegistarskiBroj dummyRegBroj;
            if (txtIme.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Ime", "Ime gimnasticara je obavezno.");
            }
            if (txtPrezime.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Prezime", "Prezime gimnasticara je obavezno.");
            }
            if (SelectedGimnastika == Gimnastika.Undefined)
            {
                notification.RegisterMessage(
                    "Gimnastika", "Gimnastika je obavezna.");
            }
            if (txtDatRodj.Text.Trim() != String.Empty
            && !Datum.TryParse(txtDatRodj.Text, out dummyDatum))
            {
                notification.RegisterMessage(
                    "DatumRodjenja", "Neispravan format za datum ili godinu rodjenja.");
            }

            if (txtRegBroj.Text.Trim() != String.Empty
            && !RegistarskiBroj.TryParse(txtRegBroj.Text, out dummyRegBroj))
            {
                notification.RegisterMessage(
                    "RegistarskiBroj", "Neispravan format za registarski broj.");
            }

            if (txtDatumPoslReg.Text.Trim() != String.Empty
            && !Datum.TryParse(txtDatumPoslReg.Text, out dummyDatum))
            {
                notification.RegisterMessage(
                    "DatumPoslednjeRegistracije", "Neispravan format za datum ili godinu poslednje registracije.");
            }

            if (SelectedDrzava == null)
            {
                if (cmbDrzava.Text.Trim() != String.Empty)
                {
                    notification.RegisterMessage(
                        "Drzava", "Uneli ste nepostojecu drzavu.");
                }
                else
                {
                    notification.RegisterMessage(
                        "Drzava", "Drzava je obavezna.");
                }
            }

            if (cmbKlub.Text.Trim() != String.Empty && cmbKlub.Text.Trim() != PRAZNO_ITEM && SelectedKlub == null)
            {
                notification.RegisterMessage(
                    "Klub", "Uneli ste nepostojeci klub.");
            }

            if (cmbKategorija.Text.Trim() != String.Empty && cmbKategorija.Text.Trim() != PRAZNO_ITEM
                && SelectedKategorija == null)
            {
                notification.RegisterMessage(
                    "Kategorija", "Uneli ste nepostojecu kategoriju.");
            }
        }

        protected override void setFocus(string propertyName)
        {
            switch (propertyName)
            {
                case "Ime":
                    txtIme.Focus();
                    break;

                case "SrednjeIme":
                    txtSrednjeIme.Focus();
                    break;

                case "Prezime":
                    txtPrezime.Focus();
                    break;

                case "Gimnastika":
                    cmbGimnastika.Focus();
                    break;

                case "DatumRodjenja":
                    txtDatRodj.Focus();
                    break;

                case "RegistarskiBroj":
                    txtRegBroj.Focus();
                    break;

                case "DatumPoslednjeRegistracije":
                    txtDatumPoslReg.Focus();
                    break;

                case "Kategorija":
                    cmbKategorija.Focus();
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

        protected override DomainObject createNewEntity()
        {
            return new Gimnasticar();
        }

        protected override void updateEntityFromUI(DomainObject entity)
        {
            Gimnasticar gimnasticar = (Gimnasticar)entity;
            
            gimnasticar.Ime = txtIme.Text.Trim();
            
            if (txtSrednjeIme.Text.Trim() == String.Empty)
                gimnasticar.SrednjeIme = null;
            else
                gimnasticar.SrednjeIme = txtSrednjeIme.Text.Trim();
            
            gimnasticar.Prezime = txtPrezime.Text.Trim();

            if (txtDatRodj.Text.Trim() == String.Empty)
                gimnasticar.DatumRodjenja = null;
            else
                gimnasticar.DatumRodjenja = Datum.Parse(txtDatRodj.Text);

            gimnasticar.Gimnastika = SelectedGimnastika;
            
            if (txtRegBroj.Text.Trim() == String.Empty)
                gimnasticar.RegistarskiBroj = null;
            else
                gimnasticar.RegistarskiBroj = RegistarskiBroj.Parse(txtRegBroj.Text);

            if (txtDatumPoslReg.Text.Trim() == String.Empty)
                gimnasticar.DatumPoslednjeRegistracije = null;
            else
                gimnasticar.DatumPoslednjeRegistracije = Datum.Parse(txtDatumPoslReg.Text);

            gimnasticar.Drzava = SelectedDrzava;
            gimnasticar.Klub = SelectedKlub;
            gimnasticar.Kategorija = SelectedKategorija;
        }

        protected override void updateEntity(DomainObject entity)
        {
            // NOTE: Desava mi se greska kada menjam datum rodjenja:
            // Error: a different object with the same identifier value was already associated with the session.

            // Moguce je da greska ima veze sa time sto je DatumRodjenja (koji se koristi u Equals za poredjenje)
            // u .hbm fajlu mapiran kao component. Kada menjam ime ili prezime (koji se takodje koriste u Equals)
            // greska se ne pojavljuje. Mada, interesantno je da u GimnasticarUcesnikForm ne postoji taj problem.
                        
            // Ovo je objasnjenje koje sam nasao za workaround koji resava problem:
            // This error is raised from nHibernate when you are updating an instance of an Entity that is saved on the
            // Cache. Basically nHibernate stores your objects on the cache once you loaded it, so next calls would get
            // it from the cache. If you update an instance that is present on the cache nHibernate throws this error
            // otherwise it could cause dirty reads and conflicts regarding loading the old copy of the object. To get
            // around this, you need to remove the object from the cache using the Evict method.

            try
            {
                // TODO3: Evict sam okruzio sa try/catch zato sto kada promenim srednje ime dobijam izuzetak "The given
                // key was not present in the dictionary". Proveri u NHibernate in Action zasto se ovo desava.
                dataContext.Evict(entity);
            }
            catch (Exception)
            { }

            dataContext.Save(entity);
        }

        protected override void checkBusinessRulesOnAdd(DomainObject entity)
        {
            Gimnasticar gimnasticar = (Gimnasticar)entity;
            Notification notification = new Notification();

            if (existsGimnasticarImeSrednjeImePrezimeDatumRodjenja(gimnasticar))
            {
                notification.RegisterMessage("Ime", 
                    "Gimnasticar sa datim imenom, prezimenom i datumom rodjenja vec postoji.");
                throw new BusinessException(notification);
            }

            if (gimnasticar.RegistarskiBroj != null
            && existsGimnasticarRegBroj(gimnasticar.RegistarskiBroj))
            {
                notification.RegisterMessage("RegistarskiBroj", 
                    "Gimnasticar sa datim registarskim brojem vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private bool existsGimnasticarImeSrednjeImePrezimeDatumRodjenja(Gimnasticar g)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Ime", CriteriaOperator.Equal, g.Ime));
            q.Criteria.Add(new Criterion("Prezime", CriteriaOperator.Equal, g.Prezime));
            if (string.IsNullOrEmpty(g.SrednjeIme))
                q.Criteria.Add(new Criterion("SrednjeIme", CriteriaOperator.IsNull, null));
            else
                q.Criteria.Add(new Criterion("SrednjeIme", CriteriaOperator.Equal, g.SrednjeIme));
            q.Operator = QueryOperator.And;
            IList<Gimnasticar> result = dataContext.GetByCriteria<Gimnasticar>(q);
            foreach (Gimnasticar g2 in result)
            {
                // Equals dodatno proverava datum rodjenja
                if (g2.Equals(g))
                    return true;
            }
            return false;
        }

        private bool existsGimnasticarRegBroj(RegistarskiBroj regBroj)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("RegistarskiBroj.Broj", CriteriaOperator.Equal, regBroj.Broj));
            return dataContext.GetCount<Gimnasticar>(q) > 0;
        }

        protected override void checkBusinessRulesOnUpdate(DomainObject entity)
        {
            Gimnasticar gimnasticar = (Gimnasticar)entity;
            Notification notification = new Notification();

            if (hasImeSrednjeImePrezimeDatumRodjenjaChanged(gimnasticar)
            && existsGimnasticarImeSrednjeImePrezimeDatumRodjenja(gimnasticar))
            {
                notification.RegisterMessage("Ime",
                    "Gimnasticar sa datim imenom, prezimenom i datumom rodjenja vec postoji.");
                throw new BusinessException(notification);
            }

            bool regBrojChanged = (gimnasticar.RegistarskiBroj != oldRegBroj) ? true : false;            
            if (regBrojChanged && gimnasticar.RegistarskiBroj != null
            && existsGimnasticarRegBroj(gimnasticar.RegistarskiBroj))
            {
                notification.RegisterMessage("RegistarskiBroj",
                    "Gimnasticar sa datim registarskim brojem vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private bool hasImeSrednjeImePrezimeDatumRodjenjaChanged(Gimnasticar g)
        {
            bool equals = g.Ime.ToUpper() == oldIme.ToUpper()
                && g.Prezime.ToUpper() == oldPrezime.ToUpper();
            if (equals)
            {
                equals = string.IsNullOrEmpty(g.SrednjeIme)
                    && string.IsNullOrEmpty(oldSrednjeIme)
                || (!string.IsNullOrEmpty(g.SrednjeIme)
                    && !string.IsNullOrEmpty(oldSrednjeIme)
                    && g.SrednjeIme.ToUpper() == oldSrednjeIme.ToUpper());
            }
            if (equals)
            {
                equals = g.DatumRodjenja == oldDatumRodjenja;
            }
            return !equals;
        }

        private void btnAddDrzava_Click(object sender, EventArgs e)
        {
            try
            {
                DrzavaForm form = new DrzavaForm(null);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Drzava d = (Drzava)form.Entity;
                    drzave.Add(d);
                    drzave.Sort();
                    setDrzave(drzave);
                    SelectedDrzava = d;
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }

        private void btnAddKlub_Click(object sender, EventArgs e)
        {
            try
            {
                KlubForm form = new KlubForm(null);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Klub k = (Klub)form.Entity;
                    klubovi.Add(k);                    
                    klubovi.Sort();
                    setKlubovi(klubovi);
                    SelectedKlub = k;
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }

        private void btnAddKategorija_Click(object sender, EventArgs e)
        {
            try
            {
                KategorijaGimnasticaraForm form = new KategorijaGimnasticaraForm(null);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    reloadKategorije(SelectedGimnastika);

                    KategorijaGimnasticara newKat = (KategorijaGimnasticara)form.Entity;
                    if (kategorije.IndexOf(newKat) != -1)
                        SelectedKategorija = newKat;
                    else
                        SelectedKategorija = null;
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }

        protected override void discardChanges()
        {
            // TODO: Razmotri da li bi trebalo da se brisu entiteti dodati
            // pritiskom na dugme (...) ako korisnik zatvori dijalog sa Cancel ili X.
            // (ovo vazi i za ostale dijaloge). Ovde je problem kako pratiti cascading
            // changes - npr. ako korisnik doda nov klub, a u dijalogu za klub doda
            // novo mesto itd.

            // NOTE: Ako implementiras discardChanges metod, kod okruzi sa try catch
            // blokom, jer u suprotnom ako se desi unhandled exception dijalog nece
            // biti zatvoren. (Ako se u bilo kom handleru dijaloga dogodi unhandled
            // exception, kontrola ostaje unutar dijaloga, a metod discardChanged se
            // poziva iz btnCancel_Click i EntityDetailForm_FormClosed handlera. )
        }

        private void cmbKategorija_DropDown(object sender, EventArgs e)
        {
            if (SelectedGimnastika == Gimnastika.Undefined)
                MessageDialogs.showMessage("Najpre unesite gimnastiku.", this.Text);
        }

        private void cmbGimnastika_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (initializing)
                return;
            reloadKategorije(SelectedGimnastika);
            SelectedKategorija = null;
        }

        private void reloadKategorije(Gimnastika gimnastika)
        {
            if (gimnastika != Gimnastika.Undefined)
            {
                try
                {
                    DataAccessProviderFactory factory = new DataAccessProviderFactory();
                    dataContext = factory.GetDataContext();
                    dataContext.BeginTransaction();

                    kategorije = loadKategorije(gimnastika);

                    //     dataContext.Commit();
                }
                catch (Exception ex)
                {
                    if (dataContext != null && dataContext.IsInTransaction)
                        dataContext.Rollback();
                    MessageDialogs.showMessage(
                        Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                    Close();
                    return;
                }
                finally
                {
                    if (dataContext != null)
                        dataContext.Dispose();
                    dataContext = null;
                }
            }
            else
            {
                kategorije = new List<KategorijaGimnasticara>();
            }

            setKategorije(kategorije);
        }

        private void GimnasticarForm_Shown(object sender, EventArgs e)
        {
            if (!editMode)
            {
                txtIme.Focus();
            }
        }

        private void txtPrezime_Enter(object sender, EventArgs e)
        {
            AutoCompleteStringCollection col = new AutoCompleteStringCollection();
            
            string ime = txtIme.Text.Trim();
            foreach (Gimnasticar g in gimnasticari)
            {
                if (g.Ime.ToUpper() == ime.ToUpper())
                {
                    string s = g.Prezime;
                    if (g.DatumRodjenja != null)
                    {
                        s += ", " + g.DatumRodjenja.ToString("dd.MM.yyyy");
                    }
                    if (g.SrednjeIme == null || g.SrednjeIme == string.Empty)
                    {
                        col.Add(s);
                    }
                    else
                        col.Add(s + "   (" + g.ImeSrednjeImePrezimeDatumRodjenja + ")");
                }
            }

            txtPrezime.AutoCompleteCustomSource = col;
        }

        private void txtPrezime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            Gimnasticar g;
            if (txtPrezime.Text.IndexOf('(') != -1 && txtPrezime.Text.IndexOf(')') != -1)
            {
                string imeSrednjeImePrezimeDatumRodjenja = txtPrezime.Text.Substring(txtPrezime.Text.IndexOf('(')).Trim();

                // Ukloni zagrade
                imeSrednjeImePrezimeDatumRodjenja = 
                    imeSrednjeImePrezimeDatumRodjenja.Substring(1, imeSrednjeImePrezimeDatumRodjenja.Length - 2).Trim();

                g = findGimnasticar(imeSrednjeImePrezimeDatumRodjenja.Trim());
            }
            else
            {
                string prezime = String.Empty;
                Datum datumRodjenja = null;
                int index = txtPrezime.Text.IndexOf(',');
                if (index != -1)
                {
                    prezime = txtPrezime.Text.Substring(0, index).Trim();
                    datumRodjenja = Datum.Parse(txtPrezime.Text.Substring(index + 1).Trim());
                }
                else
                {
                    prezime = txtPrezime.Text.Trim();
                }
                g = findGimnasticar(txtIme.Text.Trim(), String.Empty, prezime, datumRodjenja);
            }

            if (g != null)
            {
                gimnasticarToEdit = g;
                closedByOK = true;
                DialogResult = DialogResult.OK;
                //Close();
                return;
            }
        }

        private Gimnasticar findGimnasticar(string imeSrednjeImePrezimeDatumRodjenja)
        {
            foreach (Gimnasticar g in gimnasticari)
            {
                if (g.ImeSrednjeImePrezimeDatumRodjenja.ToUpper() == imeSrednjeImePrezimeDatumRodjenja.ToUpper())
                    return g;
            }
            return null;
        }

        private Gimnasticar findGimnasticar(string ime, string srednjeIme, string prezime, Datum datumRodjenja)
        {
            foreach (Gimnasticar g in gimnasticari)
            {
                string srednjeImeGim = g.SrednjeIme == null ? String.Empty : g.SrednjeIme.ToUpper();
                bool datumRodjenjaEquals = (datumRodjenja == null && g.DatumRodjenja == null)
                    || (datumRodjenja != null && g.DatumRodjenja != null && datumRodjenja.Equals(g.DatumRodjenja));
                if (g.Ime.ToUpper() == ime.Trim().ToUpper() && g.Prezime.ToUpper() == prezime.Trim().ToUpper()
                        && srednjeImeGim == srednjeIme.Trim().ToUpper() && datumRodjenjaEquals)
                    return g;
            }
            return null;
        }
    }
}