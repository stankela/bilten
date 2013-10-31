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
    public partial class RezultatiSpravaFinaleKupaForm : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private IDataContext dataContext;
        private Takmicenje takmicenje;

        // kljuc je rezTakmicenja.IndexOf(takmicenje) * (Sprava.Max + 1) + sprava
        private ISet<int> rezultatiOpened;

        private RezultatskoTakmicenje ActiveTakmicenje
        {
            get { return cmbTakmicenje.SelectedItem as RezultatskoTakmicenje; }
            set { cmbTakmicenje.SelectedItem = value; }
        }

        private Sprava ActiveSprava
        {
            get { return Sprave.parse(cmbSprava.SelectedItem.ToString()); }
            set { cmbSprava.SelectedItem = Sprave.toString(value); }
        }

        public RezultatiSpravaFinaleKupaForm(int takmicenjeId)
        {
            InitializeComponent();
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                takmicenje = loadTakmicenje(takmicenjeId);

                IList<RezultatskoTakmicenje> svaRezTakmicenja = loadRezTakmicenja(takmicenjeId);
                if (svaRezTakmicenja.Count == 0)
                    throw new BusinessException("Morate najpre da unesete takmicarske kategorije.");

                rezTakmicenja = new List<RezultatskoTakmicenje>();
                foreach (RezultatskoTakmicenje rt in svaRezTakmicenja)
                {
                    if (rt.Propozicije.PostojiTak3)
                        rezTakmicenja.Add(rt);
                }
                if (rezTakmicenja.Count == 0)
                    throw new BusinessException("Ne postoji takmicenje III ni za jednu kategoriju.");

                initUI();
                rezultatiOpened = new HashedSet<int>();

                cmbTakmicenje.SelectedIndex = 0;
                cmbSprava.SelectedIndex = 0;

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

        private Takmicenje loadTakmicenje(int takmicenjeId)
        {
            string query = @"from Takmicenje t
                    where t.Id = :takmicenjeId";
            IList<Takmicenje> result = dataContext.
                ExecuteQuery<Takmicenje>(QueryLanguageType.HQL, query,
                        new string[] { "takmicenjeId" },
                        new object[] { takmicenjeId });
            if (result.Count == 0)
                return null;
            else
                return result[0];
        }

        private IList<RezultatskoTakmicenje> loadRezTakmicenja(int takmicenjeId)
        {
            IList<RezultatskoTakmicenje> rezTakmicenjaPrvoKolo = loadRezTakmicenjaPrethKolo(takmicenje.PrvoKolo.Id);
            IList<RezultatskoTakmicenje> rezTakmicenjaDrugoKolo = loadRezTakmicenjaPrethKolo(takmicenje.DrugoKolo.Id);

            string query = @"select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.Gimnasticari g
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    where r.Takmicenje.Id = :takmicenjeId
                    order by r.RedBroj";

            IList<RezultatskoTakmicenje> result = dataContext.
                ExecuteQuery<RezultatskoTakmicenje>(QueryLanguageType.HQL, query,
                        new string[] { "takmicenjeId" },
                        new object[] { takmicenjeId });
            foreach (RezultatskoTakmicenje rezTak in result)
            {
                // potrebno u Poredak.create
                NHibernateUtil.Initialize(rezTak.Propozicije);

                RezultatskoTakmicenje rezTakPrvoKolo = findRezTakmicenje(rezTakmicenjaPrvoKolo, rezTak.Kategorija);
                RezultatskoTakmicenje rezTakDrugoKolo = findRezTakmicenje(rezTakmicenjaDrugoKolo, rezTak.Kategorija);

                rezTak.Takmicenje1.initPoredakSpravaFinaleKupa(takmicenje.Gimnastika);
                foreach (Sprava s in Sprave.getSprave(takmicenje.Gimnastika))
                {
                    if (s != Sprava.Preskok)
                    {
                        PoredakSprava poredakPrvoKolo = null;
                        PoredakSprava poredakDrugoKolo = null;
                        if (rezTakPrvoKolo != null)
                            poredakPrvoKolo = rezTakPrvoKolo.Takmicenje1.getPoredakSprava(s);
                        if (rezTakDrugoKolo != null)
                            poredakDrugoKolo = rezTakDrugoKolo.Takmicenje1.getPoredakSprava(s);
                        rezTak.Takmicenje1.getPoredakSpravaFinaleKupa(s).create(rezTak,
                            poredakPrvoKolo, poredakDrugoKolo);
                    }
                    else
                    {
                        PoredakPreskok poredakPrvoKolo = null;
                        PoredakPreskok poredakDrugoKolo = null;
                        if (rezTakPrvoKolo != null)
                            poredakPrvoKolo = rezTakPrvoKolo.Takmicenje1.PoredakPreskok;
                        if (rezTakDrugoKolo != null)
                            poredakDrugoKolo = rezTakDrugoKolo.Takmicenje1.PoredakPreskok;

                        bool poredakNaOsnovuObaPreskokaPrvoKolo = false;
                        bool poredakNaOsnovuObaPreskokaDrugoKolo = false;
                        if (rezTakPrvoKolo != null)
                            poredakNaOsnovuObaPreskokaPrvoKolo =
                                rezTakPrvoKolo.Propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka;
                        if (rezTakDrugoKolo != null)
                            poredakNaOsnovuObaPreskokaDrugoKolo =
                                rezTakDrugoKolo.Propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka;

                        rezTak.Takmicenje1.getPoredakSpravaFinaleKupa(s).create(rezTak,
                            poredakPrvoKolo, poredakDrugoKolo,
                            poredakNaOsnovuObaPreskokaPrvoKolo, poredakNaOsnovuObaPreskokaDrugoKolo);
                    }
                }
            }
            return result;
        }

        private IList<RezultatskoTakmicenje> loadRezTakmicenjaPrethKolo(int takmicenjeId)
        {
            string query = @"select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.PoredakSprava
                    left join fetch t.PoredakPreskok
                    left join fetch t.Gimnasticari g
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    where r.Takmicenje.Id = :takmicenjeId";

            IList<RezultatskoTakmicenje> result = dataContext.
                ExecuteQuery<RezultatskoTakmicenje>(QueryLanguageType.HQL, query,
                        new string[] { "takmicenjeId" },
                        new object[] { takmicenjeId });
            return result;
        }

        private RezultatskoTakmicenje findRezTakmicenje(IList<RezultatskoTakmicenje> rezTakmicenja,
            TakmicarskaKategorija kat)
        {
            foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
            {
                if (rezTak.Kategorija.Equals(kat))
                    return rezTak;
            }
            return null;
        }

        private void initUI()
        {
            Text = "I i II Kolo - rezultati sprave";
            this.ClientSize = new Size(ClientSize.Width, 450);

            cmbTakmicenje.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTakmicenje.DataSource = rezTakmicenja;
            cmbTakmicenje.DisplayMember = "Naziv";

            cmbSprava.DropDownStyle = ComboBoxStyle.DropDownList;
            Pol pol = rezTakmicenja[0].Pol;
            List<string> sprave = new List<string>(Sprave.getSpraveNazivi(pol));
            cmbSprava.Items.AddRange(sprave.ToArray());
            
            spravaGridUserControl1.DataGridViewUserControl.GridColumnHeaderMouseClick += 
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            spravaGridUserControl1.DataGridViewUserControl.DataGridView.MultiSelect = true;

            btnOk.Enabled = false;
            btnOk.Visible = false;
            btnCancel.Enabled = false;
            btnCancel.Visible = false;
            btnClose.Enabled = true;
            btnClose.Visible = true;
            btnClose.Location = new Point(btnOk.Location.X + 10, btnClose.Location.Y);
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (ActiveSprava != Sprava.Preskok)
                dgwuc.onColumnHeaderMouseClick<RezultatSpravaFinaleKupa>(e.DataGridViewCellMouseEventArgs);
            else
                dgwuc.onColumnHeaderMouseClick<RezultatSpravaFinaleKupa>(e.DataGridViewCellMouseEventArgs);
        }

        void selectedRezultatiChanged(object sender, EventArgs e)
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

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
            initSpravaGridUserControl(ActiveSprava);

            int rezultatiKey = getRezultatiKey(ActiveTakmicenje, ActiveSprava);
            if (!rezultatiOpened.Contains(rezultatiKey))
            {
                rezultatiOpened.Add(rezultatiKey);
            }

            spravaGridUserControl1.DataGridViewUserControl
                .setItems<RezultatSpravaFinaleKupa>(getRezultatiSprava(ActiveTakmicenje, ActiveSprava));
            spravaGridUserControl1.DataGridViewUserControl
                .sort<RezultatSpravaFinaleKupa>("RedBroj", ListSortDirection.Ascending);

        }

        private void initSpravaGridUserControl(Sprava sprava)
        {
            spravaGridUserControl1.init(sprava);

            GridColumnsInitializer.initRezultatiSpravaFinaleKupa(
                spravaGridUserControl1.DataGridViewUserControl,
                takmicenje, kvalColumnVisible());
        }

        private bool kvalColumnVisible()
        {
            bool result = ActiveTakmicenje.Propozicije.OdvojenoTak3;
            return result;
        }

        private int getRezultatiKey(RezultatskoTakmicenje tak, Sprava sprava)
        {
            int result = rezTakmicenja.IndexOf(tak) * ((int)Sprava.Max + 1) + (int)sprava;
            return result;
        }

        private IList<RezultatSpravaFinaleKupa> getRezultatiSprava(RezultatskoTakmicenje rezTakmicenje,
            Sprava sprava)
        {
            return rezTakmicenje.Takmicenje1.getPoredakSpravaFinaleKupa(sprava).Rezultati;
        }

        private void cmbTakmicenje_DropDownClosed(object sender, EventArgs e)
        {
            spravaGridUserControl1.DataGridViewUserControl.Focus();
        }

        private void cmbSprava_DropDownClosed(object sender, EventArgs e)
        {
            spravaGridUserControl1.DataGridViewUserControl.Focus();
        }

        private void RezultatiSpravaFinaleKupaForm_Shown(object sender, EventArgs e)
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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            /*string nazivIzvestaja;
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
            }*/
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
