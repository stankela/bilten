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
            get { return Sprave.parse(cmbSprava.SelectedItem.ToString()); }
            set { cmbSprava.SelectedItem = Sprave.toString(value); }
        }

        public RezultatiSpravaForm(int takmicenjeId, DeoTakmicenjaKod deoTakKod, bool selectMode,
            RezultatskoTakmicenje startTakmicenje, Sprava startSprava)
        {
            InitializeComponent();
            this.deoTakKod = deoTakKod;
            this.selectMode = selectMode;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                IList<RezultatskoTakmicenje> svaRezTakmicenja = loadRezTakmicenja(takmicenjeId);
                if (svaRezTakmicenja.Count == 0)
                    throw new BusinessException("Morate najpre da unesete takmicarske kategorije.");

                takmicenje = dataContext.GetById<Takmicenje>(takmicenjeId);
                NHibernateUtil.Initialize(takmicenje);

                rezTakmicenja = takmicenje.getRezTakmicenjaSprava(svaRezTakmicenja, deoTakKod, false);
                if (rezTakmicenja.Count == 0)
                    throw new BusinessException("Ne postoji takmicenje III ni za jednu kategoriju.");

                initUI(startTakmicenje, startSprava);
                rezultatiOpened = new HashedSet<int>();
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

        private void initUI(RezultatskoTakmicenje startTakmicenje, Sprava startSprava)
        {
            Text = "Rezultati - " + DeoTakmicenjaKodovi.toString(deoTakKod);
            this.ClientSize = new Size(ClientSize.Width, 540);

            cmbTakmicenje.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTakmicenje.DataSource = rezTakmicenja;
            cmbTakmicenje.DisplayMember = "Naziv";
            cmbTakmicenje.SelectedIndex = 0;
            if (selectMode)
                ActiveTakmicenje = startTakmicenje;
            cmbTakmicenje.SelectedIndexChanged += new EventHandler(cmbTakmicenje_SelectedIndexChanged);

            cmbSprava.DropDownStyle = ComboBoxStyle.DropDownList;
            Pol pol = rezTakmicenja[0].Pol;
            List<string> sprave = new List<string>(Sprave.getSpraveNazivi(pol));
            cmbSprava.Items.AddRange(sprave.ToArray());
            cmbSprava.SelectedIndex = 0;
            if (selectMode)
                ActiveSprava = startSprava;
            cmbSprava.SelectedIndexChanged += new EventHandler(cmbSprava_SelectedIndexChanged);
            
            spravaGridUserControl1.DataGridViewUserControl.GridColumnHeaderMouseClick += 
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            if (selectMode)
                spravaGridUserControl1.DataGridViewUserControl.DataGridView.MultiSelect = false;
            else
                spravaGridUserControl1.DataGridViewUserControl.DataGridView.MultiSelect = true;

            spravaGridUserControl1.SpravaGridMouseUp += new EventHandler<SpravaGridMouseUpEventArgs>(spravaGridUserControl1_SpravaGridMouseUp);

            if (selectMode)
            {
                btnOk.Enabled = true;
                btnOk.Visible = true;
                btnCancel.Enabled = true;
                btnCancel.Visible = true;
                btnPrint.Enabled = btnPrint.Visible = false;
                btnClose.Enabled = false;
                btnClose.Visible = false;
                btnIzracunaj.Enabled = btnIzracunaj.Visible = false;
                btnStampajKvalifikante.Enabled = btnStampajKvalifikante.Visible = false;
            }
            else
            {
                btnOk.Enabled = false;
                btnOk.Visible = false;
                btnCancel.Enabled = false;
                btnCancel.Visible = false;

                btnStampajKvalifikante.Location = new Point(550, btnClose.Location.Y);

                btnIzracunaj.Enabled = btnIzracunaj.Visible = true;                                                             
                btnIzracunaj.Location = new Point(btnStampajKvalifikante.Location.X + btnStampajKvalifikante.Size.Width + 20,
                    btnCancel.Location.Y);
                btnClose.Enabled = true;
                btnClose.Visible = true;
                btnClose.Location = new Point(btnIzracunaj.Location.X + btnIzracunaj.Size.Width + 20, btnCancel.Location.Y);
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

        void cmbTakmicenje_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmdSelectedRezultatiChanged();
        }

        void cmbSprava_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmdSelectedRezultatiChanged();
        }

        void cmdSelectedRezultatiChanged()
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
            btnStampajKvalifikante.Enabled = kvalColumnVisible();
            
            // TODO: Kada se promeni sprava trebalo bi da kolone zadrze postojecu sirinu.
            initSpravaGridUserControl(ActiveSprava);

            int rezultatiKey = getRezultatiKey(ActiveTakmicenje, ActiveSprava);
            if (!rezultatiOpened.Contains(rezultatiKey))
                rezultatiOpened.Add(rezultatiKey);

            setItems();
        }

        private void setItems()
        {
            if (ActiveSprava != Sprava.Preskok)
            {
                spravaGridUserControl1.DataGridViewUserControl
                    .setItems<RezultatSprava>(ActiveTakmicenje.getPoredakSprava(deoTakKod, ActiveSprava).getRezultati());
            }
            else
            {
                bool obaPreskoka = ActiveTakmicenje.Propozicije.racunajObaPreskoka(deoTakKod);
                spravaGridUserControl1.DataGridViewUserControl
                    .setItems<RezultatPreskok>(ActiveTakmicenje.getPoredakPreskok(deoTakKod).getRezultati(obaPreskoka));
            }
            spravaGridUserControl1.DataGridViewUserControl.clearSelection();
        }

        private void initSpravaGridUserControl(Sprava sprava)
        {
            spravaGridUserControl1.init(sprava);

            DataGridViewUserControl dgw = spravaGridUserControl1.DataGridViewUserControl;
            // TODO: Indexi kolona bi trebali da budu konstante

            bool obaPreskoka = sprava == Sprava.Preskok
                && ActiveTakmicenje.Propozicije.racunajObaPreskoka(deoTakKod);

            if (dgw.DataGridView.Columns.Count == 0)
            {
                GridColumnsInitializer.initRezultatiSprava(dgw, takmicenje, kvalColumnVisible(), obaPreskoka);
                GridColumnsInitializer.maximizeColumnsRezultatiSprava(dgw, deoTakKod, rezTakmicenja);
            }
            else
            {
                // grid je vec inicijalizovan. podesi da velicine kolona budu nepromenjene.
                GridColumnsInitializer.reinitRezultatiSpravaKeepColumnWidths(dgw,
                    takmicenje, kvalColumnVisible(), obaPreskoka);
            }
        }

        private bool kvalColumnVisible()
        {
            if (takmicenje.FinaleKupa)
                // Za finale kupa se kvalifikanti prikazuju u RezultatiSpravaFinaleKupa
                return false;
            else
                return ActiveTakmicenje.postojeKvalifikacijeSprava(deoTakKod);            
        }

        private int getRezultatiKey(RezultatskoTakmicenje tak, Sprava sprava)
        {
            int result = rezTakmicenja.IndexOf(tak) * ((int)Sprava.Max + 1) + (int)sprava;
            return result;
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
            cmdSelectedRezultatiChanged();
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
                SelectedResult = spravaGridUserControl1.DataGridViewUserControl.getSelectedItem<RezultatSprava>();
            else
                SelectedResult = spravaGridUserControl1.DataGridViewUserControl.getSelectedItem<RezultatPreskok>();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string nazivIzvestaja = ActiveTakmicenje.getNazivIzvestajaSprava(deoTakKod, takmicenje.FinaleKupa, false);

            HeaderFooterForm form = new HeaderFooterForm(deoTakKod, false, true, true, false, false, false, false);
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
                bool obaPreskoka = ActiveTakmicenje.Propozicije.racunajObaPreskoka(deoTakKod);

                if (form.StampajSveSprave)
                {
                    List<List<RezultatSprava>> rezultatiSprave = new List<List<RezultatSprava>>();
                    List<RezultatPreskok> rezultatiPreskok = null;

                    foreach (Sprava s in Sprave.getSprave(ActiveTakmicenje.Gimnastika))
                    {
                        if (s != Sprava.Preskok)
                            rezultatiSprave.Add(ActiveTakmicenje.getPoredakSprava(deoTakKod, s).getRezultati());
                        else
                            rezultatiPreskok = ActiveTakmicenje.getPoredakPreskok(deoTakKod).getRezultati(obaPreskoka);
                    }
                    p.setIzvestaj(new SpravaIzvestaj(rezultatiSprave, rezultatiPreskok,
                        obaPreskoka, ActiveTakmicenje.Gimnastika, kvalColumnVisible(), documentName, form.BrojSpravaPoStrani,
                        form.PrikaziPenalSprave, spravaGridUserControl1.DataGridViewUserControl.DataGridView));
                }
                else
                {
                    if (ActiveSprava != Sprava.Preskok)
                    {
                        List<RezultatSprava> rezultati =
                            ActiveTakmicenje.getPoredakSprava(deoTakKod, ActiveSprava).getRezultati();
                        p.setIzvestaj(new SpravaIzvestaj(ActiveSprava, rezultati,
                            kvalColumnVisible(), documentName, form.PrikaziPenalSprave,
                            spravaGridUserControl1.DataGridViewUserControl.DataGridView));
                    }
                    else
                    {
                        List<RezultatPreskok> rezultati =
                            ActiveTakmicenje.getPoredakPreskok(deoTakKod).getRezultati(obaPreskoka);
                        p.setIzvestaj(new SpravaIzvestaj(obaPreskoka, rezultati,
                            kvalColumnVisible(), documentName, form.PrikaziPenalSprave,
                            spravaGridUserControl1.DataGridViewUserControl.DataGridView));
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
                contextMenuStrip1.Show(grid, new Point(x, y));
            }
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
            int selCount = 0;
            RezultatSprava rez;
            if (ActiveSprava != Sprava.Preskok)
            {
                selCount = spravaGridUserControl1.DataGridViewUserControl.getSelectedItems<RezultatSprava>().Count;
                rez = spravaGridUserControl1.DataGridViewUserControl
                    .getSelectedItem<RezultatSprava>();
            }
            else
            {
                selCount = spravaGridUserControl1.DataGridViewUserControl.getSelectedItems<RezultatPreskok>().Count;
                rez = spravaGridUserControl1.DataGridViewUserControl
                    .getSelectedItem<RezultatPreskok>();
            }

            if (selCount != 1 || rez.KvalStatus == kvalStatus)
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

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                rez.KvalStatus = kvalStatus;
                if (ActiveSprava != Sprava.Preskok)
                    dataContext.Save(ActiveTakmicenje.getPoredakSprava(deoTakKod, ActiveSprava));
                else
                    dataContext.Save(ActiveTakmicenje.getPoredakPreskok(deoTakKod));
                dataContext.Commit();
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
            if (ActiveSprava != Sprava.Preskok)
                spravaGridUserControl1.DataGridViewUserControl.setSelectedItem<RezultatSprava>(rez);
            else
                spravaGridUserControl1.DataGridViewUserControl.setSelectedItem<RezultatPreskok>(rez as RezultatPreskok);
        }

        private void btnIzracunaj_Click(object sender, EventArgs e)
        {
            string msg;
            if (kvalColumnVisible())
                msg = "Da li zelite da izracunate poredak, kvalifikante i rezerve?";
            else
                msg = "Da li zelite da izracunate poredak?";
            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                Cursor.Current = Cursors.WaitCursor;
                Cursor.Show();

                IList<Ocena> ocene = loadOcene(takmicenje.Id, deoTakKod);
                if (ActiveSprava != Sprava.Preskok)
                {
                    PoredakSprava p = ActiveTakmicenje.getPoredakSprava(deoTakKod, ActiveSprava);
                    p.create(ActiveTakmicenje, ocene);
                    dataContext.Save(p);
                }
                else
                {
                    PoredakPreskok p = ActiveTakmicenje.getPoredakPreskok(deoTakKod);
                    p.create(ActiveTakmicenje, ocene);
                    dataContext.Save(p);
                }
                dataContext.Commit();
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

                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }

            setItems();
        }

        private IList<Ocena> loadOcene(int takmicenjeId, DeoTakmicenjaKod deoTakKod)
        {
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                return dataContext.ExecuteNamedQuery<Ocena>(
                    "FindOceneByDeoTakmicenja",
                    new string[] { "takId", "deoTakKod" },
                    new object[] { takmicenjeId, deoTakKod });
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

        private void prikaziKlubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            promeniKlubDrzava(true);
        }

        private void prikaziDrzavuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            promeniKlubDrzava(false);
        }

        private void promeniKlubDrzava(bool prikaziKlub)
        {
            List<GimnasticarUcesnik> gimnasticari = new List<GimnasticarUcesnik>();
            if (ActiveSprava != Sprava.Preskok)
            {
                foreach (RezultatSprava r in spravaGridUserControl1.DataGridViewUserControl.getSelectedItems<RezultatSprava>())
                    gimnasticari.Add(r.Gimnasticar);
            }
            else
            {
                foreach (RezultatPreskok r in spravaGridUserControl1.DataGridViewUserControl.getSelectedItems<RezultatPreskok>())
                    gimnasticari.Add(r.Gimnasticar);
            }
            if (gimnasticari.Count == 0)
                return;

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                foreach (GimnasticarUcesnik g in gimnasticari)
                {
                    g.NastupaZaDrzavu = !prikaziKlub;
                    dataContext.Save(g);
                }
                dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showMessage(Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                Close();
                return;
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }

            RezultatSprava rez;
            if (ActiveSprava != Sprava.Preskok)
                rez = spravaGridUserControl1.DataGridViewUserControl.getSelectedItem<RezultatSprava>();
            else
                rez = spravaGridUserControl1.DataGridViewUserControl.getSelectedItem<RezultatPreskok>();
            spravaGridUserControl1.DataGridViewUserControl.refreshItems();
            if (ActiveSprava != Sprava.Preskok)
                spravaGridUserControl1.DataGridViewUserControl.setSelectedItem<RezultatSprava>(rez);
            else
                spravaGridUserControl1.DataGridViewUserControl.setSelectedItem<RezultatPreskok>(rez as RezultatPreskok);
        }

        private void btnStampajKvalifikante_Click(object sender, EventArgs e)
        {
            string nazivIzvestaja = "Finale po spravama - kvalifikanti i rezerve";

            HeaderFooterForm form = new HeaderFooterForm(deoTakKod, false, true, false, false, false, false, false);
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
                if (takmicenje.Gimnastika == Gimnastika.ZSG)
                    form.BrojSpravaPoStrani = 4;
                else
                    form.BrojSpravaPoStrani = 6;
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
                bool obaPresk = ActiveTakmicenje.Propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka;

                if (form.StampajSveSprave)
                {
                    List<List<RezultatSprava>> rezultatiSprave = new List<List<RezultatSprava>>();
                    List<RezultatPreskok> rezultatiPreskok = null;

                    foreach (Sprava s in Sprave.getSprave(ActiveTakmicenje.Gimnastika))
                    {
                        if (s != Sprava.Preskok)
                            rezultatiSprave.Add(ActiveTakmicenje.getPoredakSprava(deoTakKod, s).getKvalifikantiIRezerve());
                        else
                            rezultatiPreskok = ActiveTakmicenje.getPoredakPreskok(deoTakKod).getKvalifikantiIRezerve(obaPresk);
                    }
                    p.setIzvestaj(new KvalifikantiTak3Izvestaj(rezultatiSprave, rezultatiPreskok, obaPresk, 
                        takmicenje.Gimnastika, documentName, form.BrojSpravaPoStrani,
                        spravaGridUserControl1.DataGridViewUserControl.DataGridView));
                }
                else
                {
                    if (ActiveSprava != Sprava.Preskok)
                    {
                        List<RezultatSprava> rezultati =
                            ActiveTakmicenje.getPoredakSprava(deoTakKod, ActiveSprava).getKvalifikantiIRezerve();
                        p.setIzvestaj(new KvalifikantiTak3Izvestaj(rezultati, ActiveSprava, documentName,
                            spravaGridUserControl1.DataGridViewUserControl.DataGridView));
                    }
                    else
                    {
                        List<RezultatPreskok> rezultati =
                            ActiveTakmicenje.getPoredakPreskok(deoTakKod).getKvalifikantiIRezerve(obaPresk);
                        p.setIzvestaj(new KvalifikantiTak3Izvestaj(rezultati, obaPresk, documentName,
                            spravaGridUserControl1.DataGridViewUserControl.DataGridView));
                    }
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
    }
}
