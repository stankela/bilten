using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Data;
using Bilten.Exceptions;
using NHibernate;
using Bilten.Report;
using NHibernate.Context;
using Bilten.Dao;
using Bilten.Dao.NHibernate;
using Bilten.Util;

namespace Bilten.UI
{
    public partial class RezultatiSpravaForm : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private DeoTakmicenjaKod deoTakKod;
        private Takmicenje takmicenje;

        // kljuc je rezTakmicenja.IndexOf(takmicenje) * (Sprava.Max + 1) + sprava
        private ISet<int> rezultatiOpened;
        private bool forViewingOnly;
        private bool select;

        private List<RezultatSprava> istiRezultati = new List<RezultatSprava>();

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

        public RezultatiSpravaForm(int takmicenjeId, DeoTakmicenjaKod deoTakKod, int startRezTakmicenjeId,
            Sprava startSprava, bool forViewingOnly, bool select)
        {
            InitializeComponent();
            this.deoTakKod = deoTakKod;
            this.forViewingOnly = forViewingOnly;
            this.select = select;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                    NHibernateUtil.Initialize(takmicenje);

                    IList<RezultatskoTakmicenje> svaRezTakmicenja = loadRezTakmicenja(takmicenjeId);
                    if (svaRezTakmicenja.Count == 0)
                        throw new BusinessException(Strings.NO_KATEGORIJE_I_TAKMICENJA_ERROR_MSG);

                    rezTakmicenja = takmicenje.getRezTakmicenjaSprava(svaRezTakmicenja, deoTakKod, false);
                    if (rezTakmicenja.Count == 0)
                        throw new BusinessException("Ne postoji takmicenje III ni za jednu kategoriju.");

                    RezultatskoTakmicenje startRezTakmicenje = null;
                    if (startRezTakmicenjeId != -1)
                    {
                        startRezTakmicenje = findRezTakmicenje(startRezTakmicenjeId, rezTakmicenja);
                        if (startRezTakmicenje == null)
                            throw new BusinessException("Ne postoje rezultati sprave za dato takmicenje.");
                    }
                    
                    initUI(startRezTakmicenje, startSprava);
                    rezultatiOpened = new HashSet<int>();
                }
            }
            catch (BusinessException)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw;
            }
            catch (InfrastructureException)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw;
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private IList<RezultatskoTakmicenje> loadRezTakmicenja(int takmicenjeId)
        {

            IList<RezultatskoTakmicenje> result;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                result = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                    .FindByTakmicenjeFetch_Tak1_PoredakSprava(takmicenjeId);
            else
                result = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                    .FindByTakmicenjeFetch_Tak3_Poredak(takmicenjeId);

            foreach (RezultatskoTakmicenje rt in result)
            {
                // potrebno u Poredak.create
                NHibernateUtil.Initialize(rt.Propozicije);

                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                {
                    foreach (PoredakSprava p in rt.Takmicenje1.PoredakSprava)
                        NHibernateUtil.Initialize(p.Rezultati);
                    NHibernateUtil.Initialize(rt.Takmicenje1.PoredakPreskok.Rezultati);
                }
                else
                {
                    if (rt.odvojenoTak3())
                    {
                        foreach (PoredakSprava p in rt.Takmicenje3.Poredak)
                            NHibernateUtil.Initialize(p.Rezultati);
                        NHibernateUtil.Initialize(rt.Takmicenje3.PoredakPreskok.Rezultati);
                    }
                }
            }
            return result;
        }

        private RezultatskoTakmicenje findRezTakmicenje(int rezTakmicenjeId, IList<RezultatskoTakmicenje> rezTakmicenja)
        {
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                if (rt.Id == rezTakmicenjeId)
                    return rt;
            }
            return null;
        }

        private void initUI(RezultatskoTakmicenje startRezTakmicenje, Sprava startSprava)
        {
            Text = "Rezultati - " + DeoTakmicenjaKodovi.toString(deoTakKod);
            this.ClientSize = new Size(ClientSize.Width, 540);

            cmbTakmicenje.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTakmicenje.DataSource = rezTakmicenja;
            cmbTakmicenje.DisplayMember = "Naziv";
            cmbTakmicenje.SelectedIndex = 0;
            if (startRezTakmicenje != null)
                ActiveTakmicenje = startRezTakmicenje;
            cmbTakmicenje.SelectedIndexChanged += new EventHandler(cmbTakmicenje_SelectedIndexChanged);

            cmbSprava.DropDownStyle = ComboBoxStyle.DropDownList;
            List<string> sprave = new List<string>(Sprave.getSpraveNazivi(rezTakmicenja[0].Gimnastika));
            cmbSprava.Items.AddRange(sprave.ToArray());
            cmbSprava.SelectedIndex = 0;
            if (startSprava != Sprava.Undefined)
                ActiveSprava = startSprava;
            cmbSprava.SelectedIndexChanged += new EventHandler(cmbSprava_SelectedIndexChanged);
            
            spravaGridUserControl1.DataGridViewUserControl.GridColumnHeaderMouseClick += 
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            if (forViewingOnly)
                spravaGridUserControl1.DataGridViewUserControl.DataGridView.MultiSelect = false;
            else
                spravaGridUserControl1.DataGridViewUserControl.DataGridView.MultiSelect = true;

            spravaGridUserControl1.SpravaGridMouseUp += new EventHandler<SpravaGridMouseUpEventArgs>(spravaGridUserControl1_SpravaGridMouseUp);

            if (forViewingOnly)
            {
                btnOk.Enabled = select;
                btnOk.Visible = select;
                btnCancel.Enabled = select;
                btnCancel.Visible = select;
                btnClose.Enabled = !select;
                btnClose.Visible = !select;
                btnPrint.Enabled = btnPrint.Visible = false;
                btnIzracunaj.Enabled = btnIzracunaj.Visible = false;
                btnStampajKvalifikante.Enabled = btnStampajKvalifikante.Visible = false;
                if (!select)
                    btnClose.Location = new Point(btnCancel.Location.X, btnCancel.Location.Y);
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
            onSelectedRezultatiChanged();
        }

        void cmbSprava_SelectedIndexChanged(object sender, EventArgs e)
        {
            onSelectedRezultatiChanged();
        }

        private void onSelectedRezultatiChanged()
        {
            btnStampajKvalifikante.Enabled = !forViewingOnly && kvalColumnVisible();            
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
                spravaGridUserControl1.DataGridViewUserControl
                    .setItems<RezultatPreskok>(ActiveTakmicenje.getPoredakPreskok(deoTakKod).getRezultati());
            }
            spravaGridUserControl1.DataGridViewUserControl.clearSelection();
        }

        private void initSpravaGridUserControl(Sprava sprava)
        {
            spravaGridUserControl1.init(sprava);

            DataGridViewUserControl dgw = spravaGridUserControl1.DataGridViewUserControl;
            // TODO: Indexi kolona bi trebali da budu konstante

            bool obaPreskoka = ActiveTakmicenje.Propozicije.racunajObaPreskoka(deoTakKod, takmicenje.FinaleKupa);
            if (dgw.DataGridView.Columns.Count == 0)
            {
                GridColumnsInitializer.initRezultatiSprava(dgw, takmicenje, kvalColumnVisible(), sprava, obaPreskoka);
                GridColumnsInitializer.maximizeColumnsRezultatiSprava(dgw, deoTakKod, rezTakmicenja, takmicenje.FinaleKupa);
            }
            else
            {
                // grid je vec inicijalizovan. podesi da velicine kolona budu nepromenjene.
                GridColumnsInitializer.reinitRezultatiSpravaKeepColumnWidths(dgw, takmicenje, kvalColumnVisible(), sprava,
                    obaPreskoka);
            }
        }

        private bool kvalColumnVisible()
        {
            if (takmicenje.FinaleKupa)
                // Za finale kupa se kvalifikanti prikazuju u RezultatiSpravaFinaleKupa
                return false;
            else
                return deoTakKod == DeoTakmicenjaKod.Takmicenje1 && ActiveTakmicenje.odvojenoTak3();
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
            onSelectedRezultatiChanged();
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
                FormUtil.initHeaderFooterFormFromOpcije(form);

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
                FormUtil.initHeaderFooterFormFromOpcije(form);
                form.Header1Text = ActiveTakmicenje.TakmicenjeDescription.Naziv;
                form.Header3Text = ActiveTakmicenje.Kategorija.Naziv;
                form.Header4Text = nazivIzvestaja;
            }

            if (form.ShowDialog() != DialogResult.OK)
                return;
            FormUtil.initHeaderFooterFromForm(form);
            Opcije.Instance.HeaderFooterInitialized = true;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
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
                bool obaPreskoka = ActiveTakmicenje.Propozicije.racunajObaPreskoka(deoTakKod, takmicenje.FinaleKupa);

                PreviewDialog p = new PreviewDialog();
                if (form.StampajSveSprave)
                {
                    List<List<RezultatSprava>> rezultatiSprave = new List<List<RezultatSprava>>();
                    List<RezultatPreskok> rezultatiPreskok = null;

                    foreach (Sprava s in Sprave.getSprave(ActiveTakmicenje.Gimnastika))
                    {
                        if (s != Sprava.Preskok)
                            rezultatiSprave.Add(ActiveTakmicenje.getPoredakSprava(deoTakKod, s).getRezultati());
                        else
                            rezultatiPreskok = ActiveTakmicenje.getPoredakPreskok(deoTakKod).getRezultati();
                    }
                    p.setIzvestaj(new SpravaIzvestaj(rezultatiSprave, rezultatiPreskok,
                        obaPreskoka, ActiveTakmicenje.Gimnastika, kvalColumnVisible(), documentName, form.BrojSpravaPoStrani,
                        form.PrikaziPenalSprave, spravaGridUserControl1.DataGridViewUserControl.DataGridView,
                        /*markFirstRows*/false, /*numRowsToMark*/0));
                }
                else
                {
                    if (ActiveSprava != Sprava.Preskok)
                    {
                        // TODO3: Implementiraj oznacavanje osvajaca medalja i za ostale izvestaje (gde treba).
                        // Takodje, uvedi odgovarajucu opciju u dijalogu za stampanje.
                        List<RezultatSprava> rezultati =
                            ActiveTakmicenje.getPoredakSprava(deoTakKod, ActiveSprava).getRezultati();
                        p.setIzvestaj(new SpravaIzvestaj(ActiveSprava, rezultati,
                            kvalColumnVisible(), documentName, form.PrikaziPenalSprave,
                            spravaGridUserControl1.DataGridViewUserControl.DataGridView,
                            /*markFirstRows*/!kvalColumnVisible(), /*numRowsToMark*/getNumMedalists(rezultati)));
                    }
                    else
                    {
                        List<RezultatPreskok> rezultati =
                            ActiveTakmicenje.getPoredakPreskok(deoTakKod).getRezultati();
                        p.setIzvestaj(new SpravaIzvestaj(obaPreskoka, rezultati,
                            kvalColumnVisible(), documentName, form.PrikaziPenalSprave,
                            spravaGridUserControl1.DataGridViewUserControl.DataGridView,
                            /*markFirstRows*/!kvalColumnVisible(),
                            /*numRowsToMark*/getNumMedalists(rezultati)));
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

                // TODO2: U izvestajima uvedi opciju da grupa koja ne moze da stane
                // cela na jednu stranu pocinje na vrhu sledece strane
            }
            catch (Exception ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }
        }

        private int getNumMedalists(List<RezultatSprava> rezultati)
        {
            int result = 0;
            foreach (RezultatSprava r in rezultati)
            {
                if (r.Rank >= 1 && r.Rank <= 3)
                    ++result;
            }
            return result;
        }

        private int getNumMedalists(List<RezultatPreskok> rezultati)
        {
            int result = 0;
            foreach (RezultatPreskok r in rezultati)
            {
                if (r.Rank >= 1 && r.Rank <= 3)
                    ++result;
            }
            return result;
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
                mnQ.Enabled = /*mnQ.Visible =*/ !forViewingOnly && kvalColumnVisible();
                mnR.Enabled = /*mnR.Visible =*/ !forViewingOnly && kvalColumnVisible();
                mnPrazno.Enabled = /*mnPrazno.Visible =*/ !forViewingOnly && kvalColumnVisible();
                mnPromeniPoredakZaIsteOcene.Enabled = !forViewingOnly;
                if (!forViewingOnly)
                {
                    findIstiRezultati();
                    mnPromeniPoredakZaIsteOcene.Enabled = istiRezultati.Count > 1;
                }
                contextMenuStrip1.Show(grid, new Point(x, y));
            }
        }

        private void findIstiRezultati()
        {
            istiRezultati.Clear();
            RezultatSprava rez;
            if (ActiveSprava != Sprava.Preskok)
            {
                rez = spravaGridUserControl1.DataGridViewUserControl
                    .getSelectedItem<RezultatSprava>();
            }
            else
            {
                rez = spravaGridUserControl1.DataGridViewUserControl
                    .getSelectedItem<RezultatPreskok>();
            }
            if (rez == null)
                return;

            if (ActiveSprava != Sprava.Preskok)
            {
                if (rez.Total == null)
                    return;
                foreach (RezultatSprava r in ActiveTakmicenje.getPoredakSprava(deoTakKod, ActiveSprava).getRezultati())
                {
                    if (r.Total == rez.Total)
                        istiRezultati.Add(r);
                    else if (istiRezultati.Count == 0)
                        continue;
                    else
                    {
                        if (istiRezultati.Count == 1)
                            istiRezultati.Clear();
                        else
                            istiRezultati.Add(r); // dodaj i prvog sledeceg sa razlicitom ocenom
                        break;
                    }
                }
            }
            else
            {
                RezultatPreskok rp = (RezultatPreskok)rez;
                bool obaPreskoka = ActiveTakmicenje.Propozicije.racunajObaPreskoka(deoTakKod, takmicenje.FinaleKupa);
                if (obaPreskoka && (rp.TotalObeOcene == null) || !obaPreskoka && (rp.Total == null))
                    return;
                foreach (RezultatPreskok r in ActiveTakmicenje.getPoredakPreskok(deoTakKod).getRezultati())
                {
                    if (obaPreskoka && (r.TotalObeOcene == rp.TotalObeOcene) || !obaPreskoka && (r.Total == rp.Total))
                        istiRezultati.Add(r);
                    else if (istiRezultati.Count == 0)
                        continue;
                    else
                    {
                        if (istiRezultati.Count == 1)
                            istiRezultati.Clear();
                        else
                            istiRezultati.Add(r); // dodaj i prvog sledeceg sa razlicitom ocenom
                        break;
                    }
                }
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

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    rez.KvalStatus = kvalStatus;
                    if (ActiveSprava != Sprava.Preskok)
                        DAOFactoryFactory.DAOFactory.GetPoredakSpravaDAO()
                            .Update(ActiveTakmicenje.getPoredakSprava(deoTakKod, ActiveSprava));
                    else
                        DAOFactoryFactory.DAOFactory.GetPoredakPreskokDAO()
                            .Update(ActiveTakmicenje.getPoredakPreskok(deoTakKod));

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                    takmicenje.LastModified = DateTime.Now;
                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                Close();
                return;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            spravaGridUserControl1.DataGridViewUserControl.refreshItems();
            if (ActiveSprava != Sprava.Preskok)
                spravaGridUserControl1.DataGridViewUserControl.setSelectedItem<RezultatSprava>(rez);
            else
                spravaGridUserControl1.DataGridViewUserControl.setSelectedItem<RezultatPreskok>(rez as RezultatPreskok);
        }

        private void mnPromeniPoredakZaIsteOcene_Click(object sender, EventArgs e)
        {
            if (ActiveSprava != Sprava.Preskok)
                promeniPoredakSprava();
            else
                promeniPoredakPreskok();
        }

        // TODO4: Trebalo bi uvesti neko persistent svojstvo koje oznacava da je poredak rucno promenjen
        private void promeniPoredakSprava()
        {
            RazresiIsteOceneForm form = new RazresiIsteOceneForm(istiRezultati, takmicenje, ActiveSprava, false);
            if (form.ShowDialog() != DialogResult.OK)
                return;

            for (int i = 0; i < istiRezultati.Count; ++i)
            {
                istiRezultati[i].Rank = (short)form.Poredak[i];
            }

            PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                TypeDescriptor.GetProperties(typeof(RezultatSprava))["Rank"],
                TypeDescriptor.GetProperties(typeof(RezultatSprava))["PrezimeIme"]
            };
            ListSortDirection[] sortDir = new ListSortDirection[] {
                ListSortDirection.Ascending,
                ListSortDirection.Ascending
            };

            short redBroj = istiRezultati[0].RedBroj;
            istiRezultati.Sort(new SortComparer<RezultatSprava>(propDesc, sortDir));
            foreach (RezultatSprava r in istiRezultati)
            {
                r.RedBroj = redBroj++;
            }

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    DAOFactoryFactory.DAOFactory.GetPoredakSpravaDAO()
                        .Update(ActiveTakmicenje.getPoredakSprava(deoTakKod, ActiveSprava));

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                    takmicenje.LastModified = DateTime.Now;
                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                Close();
                return;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            spravaGridUserControl1.DataGridViewUserControl
                .sort<RezultatSprava>("RedBroj", ListSortDirection.Ascending);
            //spravaGridUserControl1.DataGridViewUserControl.refreshItems();
            spravaGridUserControl1.DataGridViewUserControl.setSelectedItem<RezultatSprava>(istiRezultati[0]);
        }

        private void promeniPoredakPreskok()
        {
            RazresiIsteOceneForm form = new RazresiIsteOceneForm(istiRezultati, takmicenje, Sprava.Preskok,
                ActiveTakmicenje.Propozicije.racunajObaPreskoka(deoTakKod, takmicenje.FinaleKupa));
            if (form.ShowDialog() != DialogResult.OK)
                return;

            for (int i = 0; i < istiRezultati.Count; ++i)
                istiRezultati[i].Rank = (short)form.Poredak[i];

            PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                TypeDescriptor.GetProperties(typeof(RezultatPreskok))["Rank"],
                TypeDescriptor.GetProperties(typeof(RezultatPreskok))["PrezimeIme"]
            };
            ListSortDirection[] sortDir = new ListSortDirection[] {
                ListSortDirection.Ascending,
                ListSortDirection.Ascending
            };

            short redBroj = istiRezultati[0].RedBroj;
            istiRezultati.Sort(new SortComparer<RezultatSprava>(propDesc, sortDir));
            foreach (RezultatPreskok r in istiRezultati)
                r.RedBroj = redBroj++;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    DAOFactoryFactory.DAOFactory.GetPoredakPreskokDAO()
                        .Update(ActiveTakmicenje.getPoredakPreskok(deoTakKod));

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                    takmicenje.LastModified = DateTime.Now;
                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                Close();
                return;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            spravaGridUserControl1.DataGridViewUserControl
                .sort<RezultatPreskok>("RedBroj", ListSortDirection.Ascending);
            //spravaGridUserControl1.DataGridViewUserControl.refreshItems();
            spravaGridUserControl1.DataGridViewUserControl.setSelectedItem<RezultatPreskok>(
                istiRezultati[0] as RezultatPreskok);
        }

        // TODO4: Ispitaj i ispravi gresku koja je nastala na takmicenju "ZSG - I KOLO PGL SRBIJE ZSG, NOVI SAD, 18.5.2013".
        // Nije unesena nijedna ocena u takmicenju 3, ali poredak postoji i to sa ocenama iz takmicenja 1. Poredak
        // za preskok je ispravljen sa apdejtom koji je ponovo racunao poredak preskok za sva takmicenja. Trebalo bi
        // ispraviti poretke i za ostale sprave. Takodje bi trebalo proveriti da li se ista stvar desila i na
        // nekom drugom takmicenju (na bilo kojoj spravi). Pretpostavljam da treba gledati samo takmicenja koja imaju
        // odvojeno takmicenje 3, tj gde je ZavrsenoTak1 == true. Za proveru kakvo je bilo stanje ranije koristi
        // "Bilten - Copy 15.03.2017"
        // Slicna greska u "Bilten - Copy 15.03.2017" postoji i u "MEMORIJAL 2015" gde u takmicenju 3 u V kategoriji za
        // Octavian Tomescu postoji rezultat za preskok (sa ocenom koja nije kao u takmicenju 1), a u takmicenju 3 nije
        // uneta nijedana ocena za Octavian Tomescu. Proveri kako je ovo moglo da se desi.

        private void btnIzracunaj_Click(object sender, EventArgs e)
        {
            string msg;
            if (kvalColumnVisible())
                msg = "Da li zelite da izracunate poredak, kvalifikante i rezerve?";
            else
                msg = "Da li zelite da izracunate poredak?";
            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
                    IList<Ocena> ocene = ocenaDAO.FindByDeoTakmicenja(takmicenje.Id, deoTakKod);
                    if (ActiveSprava != Sprava.Preskok)
                    {
                        PoredakSprava p = ActiveTakmicenje.getPoredakSprava(deoTakKod, ActiveSprava);
                        p.create(ActiveTakmicenje, ocene);
                        DAOFactoryFactory.DAOFactory.GetPoredakSpravaDAO().Update(p);
                    }
                    else
                    {
                        PoredakPreskok p = ActiveTakmicenje.getPoredakPreskok(deoTakKod);
                        p.create(ActiveTakmicenje, ocene);
                        DAOFactoryFactory.DAOFactory.GetPoredakPreskokDAO().Update(p);
                    }
                    foreach (Ocena o in ocene)
                        ocenaDAO.Evict(o);

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                    takmicenje.LastModified = DateTime.Now;
                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            setItems();
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

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    GimnasticarUcesnikDAO gimUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
                    foreach (GimnasticarUcesnik g in gimnasticari)
                    {
                        g.NastupaZaDrzavu = !prikaziKlub;
                        gimUcesnikDAO.Update(g);
                    }

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                    takmicenje.LastModified = DateTime.Now;
                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showMessage(ex.Message, this.Text);
                Close();
                return;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
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
                FormUtil.initHeaderFooterFormFromOpcije(form);

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
                FormUtil.initHeaderFooterFormFromOpcije(form);
                form.Header1Text = ActiveTakmicenje.TakmicenjeDescription.Naziv;
                form.Header3Text = ActiveTakmicenje.Kategorija.Naziv;
                form.Header4Text = nazivIzvestaja;
            }

            if (form.ShowDialog() != DialogResult.OK)
                return;
            FormUtil.initHeaderFooterFromForm(form);
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
            catch (Exception ex)
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
