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

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
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

                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
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

            RezultatSpravaFinaleKupaDAO dao = new RezultatSpravaFinaleKupaDAO();

            foreach (RezultatskoTakmicenje rezTak in result)
            {
                // potrebno u Poredak.create
                NHibernateUtil.Initialize(rezTak.Propozicije);

                RezultatskoTakmicenje rezTakPrvoKolo = findRezTakmicenje(rezTakmicenjaPrvoKolo, rezTak.Kategorija);
                RezultatskoTakmicenje rezTakDrugoKolo = findRezTakmicenje(rezTakmicenjaDrugoKolo, rezTak.Kategorija);

                rezTak.Takmicenje1.initPoredakSpravaFinaleKupa(takmicenje.Gimnastika);
                List<RezultatSpravaFinaleKupaUpdate> rezultatiUpdate = dao.findByRezTak(rezTak);

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
                            poredakPrvoKolo, poredakDrugoKolo, rezultatiUpdate);
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
                            poredakNaOsnovuObaPreskokaPrvoKolo, poredakNaOsnovuObaPreskokaDrugoKolo, rezultatiUpdate);
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

            spravaGridUserControl1.SpravaGridMouseUp +=
                new EventHandler<SpravaGridMouseUpEventArgs>(spravaGridUserControl1_SpravaGridMouseUp);
            
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

        private List<RezultatSpravaFinaleKupa> getRezultatiSpravaSorted(RezultatskoTakmicenje rezTakmicenje, Sprava sprava,
            string sortColumn)
        {
            List<RezultatSpravaFinaleKupa> result =
                new List<RezultatSpravaFinaleKupa>(getRezultatiSprava(rezTakmicenje, sprava));
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatSpravaFinaleKupa))[sortColumn];
            result.Sort(new SortComparer<RezultatSpravaFinaleKupa>(propDesc,
                ListSortDirection.Ascending));
            return result;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string nazivIzvestaja = "I i II kolo - Rezultati po spravama";

            HeaderFooterForm form = new HeaderFooterForm(DeoTakmicenjaKod.Takmicenje1, false, true, false, false, false);
            if (!Opcije.Instance.HeaderFooterInitialized)
            {
                Opcije.Instance.initHeaderFooterFormFromOpcije(form);

                string mestoDatum = takmicenje.Mesto + "  "
                    + takmicenje.Datum.ToShortDateString();
                form.Header1Text = ActiveTakmicenje.TakmicenjeDescription.Naziv;
                form.Header2Text = mestoDatum;
                form.Header3Text = ActiveTakmicenje.Kategorija.Naziv;
                form.Header4Text = nazivIzvestaja;
                form.FooterText = mestoDatum;
            }
            else
            {
                Opcije.Instance.initHeaderFooterFormFromOpcije(form);
                form.Header1Text = ActiveTakmicenje.TakmicenjeDescription.Naziv;
                form.Header3Text = ActiveTakmicenje.Kategorija.Naziv;
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

                string documentName;
                if (form.StampajSveSprave)
                {
                    documentName = nazivIzvestaja + " - " + ActiveTakmicenje.Kategorija.Naziv;
                }
                else
                {
                    documentName = nazivIzvestaja + " - " + Sprave.toString(ActiveSprava) + " - "
                        + ActiveTakmicenje.Kategorija.Naziv;
                }
                if (form.StampajSveSprave)
                {
                    kvalColumn = kvalColumnVisible();

                    List<List<RezultatSpravaFinaleKupa>> rezultatiSprave = new List<List<RezultatSpravaFinaleKupa>>();

                    Sprava[] sprave = Sprave.getSprave(ActiveTakmicenje.Gimnastika);
                    foreach (Sprava s in sprave)
                    {
                        rezultatiSprave.Add(getRezultatiSpravaSorted(ActiveTakmicenje, s, "RedBroj"));
                    }
                    p.setIzvestaj(new SpravaFinaleKupaIzvestaj(rezultatiSprave, ActiveTakmicenje.Gimnastika, kvalColumn,
                        documentName, form.BrojSpravaPoStrani,
                        spravaGridUserControl1.DataGridViewUserControl.DataGridView));
                }
                else
                {
                    kvalColumn = kvalColumnVisible();
                    List<RezultatSpravaFinaleKupa> rezultati =
                        new List<RezultatSpravaFinaleKupa>(getRezultatiSpravaSorted(ActiveTakmicenje, ActiveSprava, "RedBroj"));

                    p.setIzvestaj(new SpravaFinaleKupaIzvestaj(ActiveSprava, rezultati,
                        kvalColumn, documentName, spravaGridUserControl1.DataGridViewUserControl.DataGridView));
                }

                p.ShowDialog();
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

        void spravaGridUserControl1_SpravaGridMouseUp(object sender, SpravaGridMouseUpEventArgs e)
        {
            DataGridView grid = spravaGridUserControl1.DataGridViewUserControl.DataGridView;
            int x = e.MouseEventArgs.X;
            int y = e.MouseEventArgs.Y;
            if (e.MouseEventArgs.Button == MouseButtons.Right && grid.HitTest(x, y).Type == DataGridViewHitTestType.Cell)
            {
                mnQ.Enabled = /*mnQ.Visible =*/ kvalColumnVisible();
                mnR.Enabled = /*mnR.Visible =*/ kvalColumnVisible();
                mnPrazno.Enabled = /*mnPrazno.Visible =*/ kvalColumnVisible();
                //findIstiRezultati();
                //mnPromeniPoredakZaIsteOcene.Enabled = istiRezultati.Count > 1;
                contextMenuStrip1.Show(grid, new Point(x, y));
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnQ_Click(object sender, EventArgs e)
        {
            promeniKvalStatus(KvalifikacioniStatus.Q);
        }

        private void mnR_Click(object sender, EventArgs e)
        {
            promeniKvalStatus(KvalifikacioniStatus.R);
        }

        private void mnPrazno_Click(object sender, EventArgs e)
        {
            promeniKvalStatus(KvalifikacioniStatus.None);
        }

        private void promeniKvalStatus(KvalifikacioniStatus kvalStatus)
        {
            RezultatSpravaFinaleKupa rez
                = spravaGridUserControl1.DataGridViewUserControl.getSelectedItem<RezultatSpravaFinaleKupa>();
            if (rez == null || rez.KvalStatus == kvalStatus)
                return;

            string msg = String.Empty;
            if (kvalStatus != KvalifikacioniStatus.None)
            {
                string msgFmt = "Da li zelite da oznacite sa \"{1}\" gimnasticara \"{0}\"?";
                msg = String.Format(msgFmt, rez.Gimnasticar, kvalStatus);
            }
            else
            {
                string msgFmt = "Da li zelite da ponistite oznaku \"{1}\" za gimnasticara \"{0}\"?";
                msg = String.Format(msgFmt, rez.Gimnasticar, rez.KvalStatus);
            }
            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;

            // NOTE: Promena kval. statusa kod finala kupa namerno je implementirana da se cuva u posebnoj tabeli kao
            // update na postojece rezultate. Ako bih implementirao drugacije, tj. da se cuva u istoj tabeli gde su i rezultati,
            // tada bi se poredak snimio u bazu prilikom prvog otvaranja prozora, i ne bi se ponovo izracunavao prilikom
            // svakog sledeceg otvaranja prozora vec bi se ucitavao iz baze. U tom slucaju ne bih mogao da postignem da se
            // npr. promeni neki rezultat iz prvog i drugog kola i da ta promena automatski bude vidljiva kada se ponovo
            // otvori prozor za finale kupa.
            
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                insertRezultatSpravaFinaleKupaUpdate(rez.Gimnasticar, ActiveTakmicenje, ActiveSprava, kvalStatus);
                rez.KvalStatus = kvalStatus;
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

            spravaGridUserControl1.DataGridViewUserControl.refreshItems();
            spravaGridUserControl1.DataGridViewUserControl.setSelectedItem<RezultatSpravaFinaleKupa>(rez);
        }

        private void insertRezultatSpravaFinaleKupaUpdate(GimnasticarUcesnik gimnasticar, RezultatskoTakmicenje rezTak,
            Sprava sprava, KvalifikacioniStatus newKvalStatus)
        {
            RezultatSpravaFinaleKupaDAO dao = new RezultatSpravaFinaleKupaDAO();
            if (!dao.postojiRezultatSpravaFinaleKupaUpdate(gimnasticar, rezTak, sprava))
                dao.insert(gimnasticar, rezTak, sprava, newKvalStatus);
            else
                dao.update(gimnasticar, rezTak, sprava, newKvalStatus);
        }

    }
}
