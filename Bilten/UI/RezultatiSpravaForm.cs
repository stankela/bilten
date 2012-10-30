using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Data;
using Iesi.Collections.Generic;
using Bilten.Exceptions;
using NHibernate;
using Bilten.Report;

namespace Bilten.UI
{
    public partial class RezultatiSpravaForm : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private IDataContext dataContext;
        private DeoTakmicenjaKod deoTakKod;
        private Takmicenje takmicenje;

        // kljuc je rezTakmicenja.IndexOf(takmicenje) * (Sprava.Max + 1) + sprava
        private ISet<int> rezultatiOpened;
        private bool obaPreskoka;
        private const string OBA_PRESKOKA = "Preskok (oba)";

        private bool selectMode = false;

        private RezultatSprava selResult;
        public RezultatSprava SelectedResult
        {
            get { return selResult; }
            private set { selResult = value; }
        }

        public RezultatskoTakmicenje SelectedTakmicenje;
        public Sprava SelectedSprava;

        private RezultatskoTakmicenje ActiveTakmicenje
        {
            get { return cmbTakmicenje.SelectedItem as RezultatskoTakmicenje; }
            set { cmbTakmicenje.SelectedItem = value; }
        }

        private Sprava ActiveSprava
        {
            get 
            {
                if (cmbSprava.SelectedItem.ToString() == OBA_PRESKOKA)
                    return Sprava.Preskok;
                else
                    return Sprave.parse(cmbSprava.SelectedItem.ToString()); 
            }
            set 
            {
                cmbSprava.SelectedItem = Sprave.toString(value); 
            }
        }

        public RezultatiSpravaForm(int takmicenjeId, DeoTakmicenjaKod deoTakKod, bool selectMode,
            RezultatskoTakmicenje startTakmicenje, Sprava startSprava)
        {
            InitializeComponent();
            this.deoTakKod = deoTakKod;
            this.selectMode = selectMode;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                IList<RezultatskoTakmicenje> svaRezTakmicenja = loadRezTakmicenja(takmicenjeId);
                if (svaRezTakmicenja.Count == 0)
                    throw new BusinessException("Morate najpre da unesete takmicarske kategorije.");

                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                    rezTakmicenja = svaRezTakmicenja;
                else
                {
                    rezTakmicenja = new List<RezultatskoTakmicenje>();
                    foreach (RezultatskoTakmicenje rt in svaRezTakmicenja)
                    {
                        if (rt.Propozicije.PostojiTak3)
                            rezTakmicenja.Add(rt);
                    }
                    if (rezTakmicenja.Count == 0)
                        throw new BusinessException("Ne postoji takmicenje III ni za jednu kategoriju.");
                }

                takmicenje = dataContext.GetById<Takmicenje>(takmicenjeId);
                NHibernateUtil.Initialize(takmicenje);

                if (takmicenje.FinaleKupa)
                {
                    List<RezultatskoTakmicenje> rezTakmicenja2 = new List<RezultatskoTakmicenje>(rezTakmicenja);
                    rezTakmicenja.Clear();
                    foreach (RezultatskoTakmicenje rt in rezTakmicenja2)
                    {
                        if (rt.Propozicije.OdvojenoTak3)
                            rezTakmicenja.Add(rt);
                    }
                    if (rezTakmicenja.Count == 0)
                        throw new BusinessException("Ne postoji posebno takmicenje III ni za jednu kategoriju.");
                }
                
                initUI();
                rezultatiOpened = new HashedSet<int>();

                if (!selectMode)
                {
                    cmbTakmicenje.SelectedIndex = 0;
                    cmbSprava.SelectedIndex = 0;
                }
                else
                {
                    cmbTakmicenje.SelectedIndex = findRezTakIndex(startTakmicenje);
                    cmbSprava.SelectedIndex = findSpravaIndex(startSprava, 
                        startTakmicenje.Propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka);
                }

                cmbTakmicenje.SelectedIndexChanged += new EventHandler(selectedRezultatiChanged);
                cmbSprava.SelectedIndexChanged += new EventHandler(selectedRezultatiChanged);

                //onSelectedRezultatiChanged();
            }
            catch (BusinessException)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw;
            }
            catch (InfrastructureException)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw;
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw new InfrastructureException(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        private int findSpravaIndex(Sprava sprava, bool obaPreskoka)
        {
            if (sprava != Sprava.Preskok)
                return findSpravaIndex(Sprave.toString(sprava));
            else if (!obaPreskoka)
                return findSpravaIndex(Sprave.toString(Sprava.Preskok));
            else
                return findSpravaIndex(OBA_PRESKOKA);
        }

        private int findSpravaIndex(string sprava)
        {
            for (int i = 0; i < cmbSprava.Items.Count; i++)
            {
                if (cmbSprava.Items[i].Equals(sprava))
                    return i;
            }
            return -1;
        }

        private int findRezTakIndex(RezultatskoTakmicenje rezTak)
        {
            if (rezTak == null)
                return -1;

            for (int i = 0; i < rezTakmicenja.Count; i++)
            {
                if (rezTakmicenja[i].Naziv == rezTak.Naziv)
                    return i;
            }
            return -1;
        }

        private IList<RezultatskoTakmicenje> loadRezTakmicenja(int takmicenjeId)
        {

            string query;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                query = @"select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.PoredakSprava
                    left join fetch t.Gimnasticari g
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    where r.Takmicenje.Id = :takmicenjeId
                    order by r.RedBroj";
            else
                query = @"select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje3 t
                    left join fetch t.Poredak
                    left join fetch t.Ucesnici u
                    left join fetch u.Gimnasticar g
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    where r.Takmicenje.Id = :takmicenjeId
                    order by r.RedBroj";

            IList<RezultatskoTakmicenje> result = dataContext.
                ExecuteQuery<RezultatskoTakmicenje>(QueryLanguageType.HQL, query,
                        new string[] { "takmicenjeId" },
                        new object[] { takmicenjeId });
            foreach (RezultatskoTakmicenje tak in result)
            {
                // potrebno u Poredak.create
                NHibernateUtil.Initialize(tak.Propozicije);

                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                {
                    foreach (PoredakSprava p in tak.Takmicenje1.PoredakSprava)
                        NHibernateUtil.Initialize(p.Rezultati);
                    NHibernateUtil.Initialize(tak.Takmicenje1.PoredakPreskok.Rezultati);
                }
                else
                {
                    if (tak.Propozicije.PostojiTak3)
                    {
                        foreach (PoredakSprava p in tak.Takmicenje3.Poredak)
                            NHibernateUtil.Initialize(p.Rezultati);
                        NHibernateUtil.Initialize(tak.Takmicenje3.PoredakPreskok.Rezultati);
                    }
                }
            }
            return result;
        }

        private void initUI()
        {
            Text = "Rezultati - " + DeoTakmicenjaKodovi.toString(deoTakKod);
            this.ClientSize = new Size(ClientSize.Width, 450);

            cmbTakmicenje.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTakmicenje.DataSource = rezTakmicenja;
            cmbTakmicenje.DisplayMember = "Naziv";

            cmbSprava.DropDownStyle = ComboBoxStyle.DropDownList;
            Pol pol = rezTakmicenja[0].Pol;
            List<string> sprave = new List<string>(Sprave.getSpraveNazivi(pol));
            sprave.Insert(Sprave.indexOf(Sprava.Preskok, pol) + 1, OBA_PRESKOKA);
            cmbSprava.Items.AddRange(sprave.ToArray());
            
            spravaGridUserControl1.DataGridViewUserControl.GridColumnHeaderMouseClick += 
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            if (selectMode)
                spravaGridUserControl1.DataGridViewUserControl.DataGridView.MultiSelect = false;
            else
                spravaGridUserControl1.DataGridViewUserControl.DataGridView.MultiSelect = true;

            if (selectMode)
            {
                btnOk.Enabled = true;
                btnOk.Visible = true;
                btnCancel.Enabled = true;
                btnCancel.Visible = true;
                btnClose.Enabled = false;
                btnClose.Visible = false;
            }
            else
            {
                btnOk.Enabled = false;
                btnOk.Visible = false;
                btnCancel.Enabled = false;
                btnCancel.Visible = false;
                btnClose.Enabled = true;
                btnClose.Visible = true;
                btnClose.Location = new Point(btnOk.Location.X + 10, btnClose.Location.Y);
            }
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (ActiveSprava != Sprava.Preskok)
                dgwuc.onColumnHeaderMouseClick<RezultatSprava>(e.DataGridViewCellMouseEventArgs);
            else
                dgwuc.onColumnHeaderMouseClick<RezultatPreskok>(e.DataGridViewCellMouseEventArgs);
        }

        void selectedRezultatiChanged(object sender, EventArgs e)
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                obaPreskoka = ActiveSprava == Sprava.Preskok 
                    && cmbSprava.SelectedItem.ToString() == OBA_PRESKOKA;
                onSelectedRezultatiChanged();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
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

        private void onSelectedRezultatiChanged()
        {
            initSpravaGridUserControl(ActiveSprava, obaPreskoka);

            int rezultatiKey = getRezultatiKey(ActiveTakmicenje, ActiveSprava);
            if (!rezultatiOpened.Contains(rezultatiKey))
            {
                rezultatiOpened.Add(rezultatiKey);
            }

            if (ActiveSprava != Sprava.Preskok)
            {
                spravaGridUserControl1.DataGridViewUserControl
                    .setItems<RezultatSprava>(getRezultatiSprava(ActiveTakmicenje, ActiveSprava));
                spravaGridUserControl1.DataGridViewUserControl
                    .sort<RezultatSprava>("RedBroj", ListSortDirection.Ascending);
            }
            else if (!obaPreskoka)
            {
                spravaGridUserControl1.DataGridViewUserControl
                    .setItems<RezultatPreskok>(getRezultatiPreskok1(ActiveTakmicenje));
                spravaGridUserControl1.DataGridViewUserControl
                    .sort<RezultatPreskok>("RedBroj", ListSortDirection.Ascending);
            }
            else
            {
                spravaGridUserControl1.DataGridViewUserControl
                    .setItems<RezultatPreskok>(getRezultatiPreskok2(ActiveTakmicenje));
                spravaGridUserControl1.DataGridViewUserControl
                    .sort<RezultatPreskok>("RedBroj2", ListSortDirection.Ascending);
            }
        }

        private void initSpravaGridUserControl(Sprava sprava, bool obaPreskoka)
        {
            spravaGridUserControl1.init(sprava);

            GridColumnsInitializer.initRezultatiSprava(
                spravaGridUserControl1.DataGridViewUserControl,
                takmicenje, kvalColumnVisible(), obaPreskoka);
        }

        private bool kvalColumnVisible()
        {
            if (takmicenje.FinaleKupa)
                return false;

            bool result = deoTakKod == DeoTakmicenjaKod.Takmicenje1
                && ActiveTakmicenje.Propozicije.PostojiTak3
                && ActiveTakmicenje.Propozicije.OdvojenoTak3;
            if (ActiveSprava == Sprava.Preskok)
            {
                if (!obaPreskoka)
                    result = result
                        && !ActiveTakmicenje.Propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka;
                else
                    result = result
                        && ActiveTakmicenje.Propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka;
            }
            return result;
        }

        private int getRezultatiKey(RezultatskoTakmicenje tak, Sprava sprava)
        {
            int result = rezTakmicenja.IndexOf(tak) * ((int)Sprava.Max + 1) + (int)sprava;
            return result;
        }

        private IList<RezultatSprava> getRezultatiSprava(RezultatskoTakmicenje rezTakmicenje,
            Sprava sprava)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                return rezTakmicenje.Takmicenje1.getPoredakSprava(sprava).Rezultati;
            else
                return rezTakmicenje.Takmicenje3.getPoredak(sprava).Rezultati;
        }

        private IList<RezultatPreskok> getRezultatiPreskok1(RezultatskoTakmicenje rezTakmicenje)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                return rezTakmicenje.Takmicenje1.PoredakPreskok.Rezultati;
            else
                return rezTakmicenje.Takmicenje3.PoredakPreskok.Rezultati;
        }

        private List<RezultatPreskok> getRezultatiPreskok2(RezultatskoTakmicenje rezTakmicenje)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                return rezTakmicenje.Takmicenje1.PoredakPreskok.getRezultatiDvaPreskoka();
            else
                return new List<RezultatPreskok>(rezTakmicenje.Takmicenje3.PoredakPreskok.Rezultati);
        }

        private void cmbTakmicenje_DropDownClosed(object sender, EventArgs e)
        {
            spravaGridUserControl1.DataGridViewUserControl.Focus();
        }

        private void cmbSprava_DropDownClosed(object sender, EventArgs e)
        {
            spravaGridUserControl1.DataGridViewUserControl.Focus();
        }

        private void RezultatiSpravaForm_Shown(object sender, EventArgs e)
        {
            spravaGridUserControl1.DataGridViewUserControl.Focus();
            selectedRezultatiChanged(null, EventArgs.Empty);
        }

        private void cmbSprava_DropDown(object sender, EventArgs e)
        {
            // TODO2: Ako je u pitanju Takmicenje3, treba proveriti za aktivno 
            // takmicenje svojstvo PoredakTak3PreskokNaOsnovuObaPreskoka, i na osnovu 
            // toga u combu sprava prikazati ili 'preskok' ili 'preskok(oba)'
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            int selItemsCount;
            if (ActiveSprava != Sprava.Preskok)
                selItemsCount = spravaGridUserControl1.DataGridViewUserControl.getSelectedItems<RezultatSprava>().Count;
            else
                selItemsCount = spravaGridUserControl1.DataGridViewUserControl.getSelectedItems<RezultatPreskok>().Count;

            if (selItemsCount != 1)
            {
                DialogResult = DialogResult.None;
                return;
            }

            SelectedTakmicenje = ActiveTakmicenje;
            SelectedSprava = ActiveSprava;
            if (ActiveSprava != Sprava.Preskok)
            {
                SelectedResult = spravaGridUserControl1.DataGridViewUserControl
                    .getSelectedItem<RezultatSprava>();
            }
            else
            {
                SelectedResult = spravaGridUserControl1.DataGridViewUserControl
                    .getSelectedItem<RezultatPreskok>();
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string nazivIzvestaja;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                if (ActiveTakmicenje.Propozicije.OdvojenoTak3)
                    nazivIzvestaja = "Kvalifikacije za finale po spravama";
                else
                    nazivIzvestaja = "Finale po spravama";
            }
            else
            {
                nazivIzvestaja = "Finale po spravama";
            }

            HeaderFooterForm form = new HeaderFooterForm(deoTakKod, false, true, true, false, false);
            if (!Opcije.Instance.HeaderFooterInitialized)
            {
                Opcije.Instance.initHeaderFooterFormFromOpcije(form);

                string mestoDatum = takmicenje.Mesto + "  "
                    + takmicenje.Datum.ToShortDateString();
                form.Header1Text = takmicenje.Naziv;
                form.Header2Text = mestoDatum;
                form.Header3Text = ActiveTakmicenje.Naziv;
                form.Header4Text = nazivIzvestaja;
                form.FooterText = mestoDatum;
            }
            else
            {
                Opcije.Instance.initHeaderFooterFormFromOpcije(form);
                form.Header3Text = ActiveTakmicenje.Naziv;
                form.Header4Text = nazivIzvestaja;
            }

            if (form.ShowDialog() != DialogResult.OK)
                return;
            Opcije.Instance.initHeaderFooterFromForm(form);
            Opcije.Instance.HeaderFooterInitialized = true;
    
            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                PreviewDialog p = new PreviewDialog();

                bool kvalColumn;

                string documentName = nazivIzvestaja + " - " + Sprave.toString(ActiveSprava);
                if (form.StampajSveSprave)
                {
                    kvalColumn = deoTakKod == DeoTakmicenjaKod.Takmicenje1
                      && ActiveTakmicenje.Propozicije.PostojiTak3
                      && ActiveTakmicenje.Propozicije.OdvojenoTak3;

                    bool obaPresk = ActiveTakmicenje.Propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka;

                    List<List<RezultatSprava>> rezultatiSprave = new List<List<RezultatSprava>>();
                    List<RezultatPreskok> rezultatiPreskok = null;

                    Sprava[] sprave = Sprave.getSprave(ActiveTakmicenje.Gimnastika);
                    foreach (Sprava s in sprave)
                    {
                        if (s != Sprava.Preskok)
                        {
                            List<RezultatSprava> rezultati =
                                new List<RezultatSprava>(getRezultatiSprava(ActiveTakmicenje, s));

                            PropertyDescriptor propDesc =
                                TypeDescriptor.GetProperties(typeof(RezultatSprava))["RedBroj"];
                            rezultati.Sort(new SortComparer<RezultatSprava>(propDesc,
                                ListSortDirection.Ascending));

                            rezultatiSprave.Add(rezultati);
                        }
                        else if (!obaPresk)
                        {
                            rezultatiPreskok =
                                new List<RezultatPreskok>(getRezultatiPreskok1(ActiveTakmicenje));
                            PropertyDescriptor propDesc =
                                TypeDescriptor.GetProperties(typeof(RezultatPreskok))["RedBroj"];
                            rezultatiPreskok.Sort(new SortComparer<RezultatPreskok>(propDesc,
                                ListSortDirection.Ascending));
                        }
                        else
                        {
                            rezultatiPreskok =
                                new List<RezultatPreskok>(getRezultatiPreskok2(ActiveTakmicenje));
                            PropertyDescriptor propDesc =
                                TypeDescriptor.GetProperties(typeof(RezultatPreskok))["RedBroj2"];
                            rezultatiPreskok.Sort(new SortComparer<RezultatPreskok>(propDesc,
                                ListSortDirection.Ascending));
                        }
                    }
                    p.setIzvestaj(new SpravaIzvestaj(rezultatiSprave, rezultatiPreskok,
                        obaPresk, ActiveTakmicenje.Gimnastika, kvalColumn, documentName, form.BrojSpravaPoStrani,
                        form.PrikaziPenalSprave));
                }
                else
                {
                    kvalColumn = kvalColumnVisible();
                    if (ActiveSprava != Sprava.Preskok)
                    {
                        List<RezultatSprava> rezultati =
                            new List<RezultatSprava>(getRezultatiSprava(ActiveTakmicenje, ActiveSprava));

                        PropertyDescriptor propDesc =
                            TypeDescriptor.GetProperties(typeof(RezultatSprava))["RedBroj"];
                        rezultati.Sort(new SortComparer<RezultatSprava>(propDesc,
                            ListSortDirection.Ascending));

                        p.setIzvestaj(new SpravaIzvestaj(ActiveSprava, rezultati,
                            kvalColumn, documentName, form.PrikaziPenalSprave));

                    }
                    else if (!obaPreskoka)
                    {
                        List<RezultatPreskok> rezultati =
                            new List<RezultatPreskok>(getRezultatiPreskok1(ActiveTakmicenje));
                        PropertyDescriptor propDesc =
                            TypeDescriptor.GetProperties(typeof(RezultatPreskok))["RedBroj"];
                        rezultati.Sort(new SortComparer<RezultatPreskok>(propDesc,
                            ListSortDirection.Ascending));

                        p.setIzvestaj(new SpravaIzvestaj(false, rezultati,
                            kvalColumn, documentName, form.PrikaziPenalSprave));
                    }
                    else
                    {
                        List<RezultatPreskok> rezultati = getRezultatiPreskok2(ActiveTakmicenje);
                        PropertyDescriptor propDesc =
                            TypeDescriptor.GetProperties(typeof(RezultatPreskok))["RedBroj2"];
                        rezultati.Sort(new SortComparer<RezultatPreskok>(propDesc,
                            ListSortDirection.Ascending));

                        p.setIzvestaj(new SpravaIzvestaj(true, rezultati,
                            kvalColumn, documentName, form.PrikaziPenalSprave));
                    }
                }

                p.ShowDialog();

                // TODO2: U izvestajima za spravu treba da bude i penalizacija, a
                // slika sprave treba da bude iznad izvestaja. Naziv kolone total
                // treba da bude "Total" (ili "Ukupno").

                // TODO2: U izvestajima treba da postoji i linija za organizatora
                // takmicenja (recimo Gimnasticki savez srbije), i treba da bude
                // prva (u vrhu papira)

                // TODO2: Uvedi opciju da li se zeli stampanje izvestaja sa ili bez
                // linija

                // TODO2: Proveri zasto u PropozicijeForm ne prikazuje takmicenja
                // po onom redosledu kojim su zadata.

                // TODO2: U izvestajima uvedi opciju da grupa koja ne moze da stane
                // cela na jednu stranu pocinje na vrhu sledece strane

            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
