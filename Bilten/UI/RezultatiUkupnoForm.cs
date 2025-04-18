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
    public partial class RezultatiUkupnoForm : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private IList<RezultatskoTakmicenje> svaRezTakmicenja;
        private DeoTakmicenjaKod deoTakKod;
        private Takmicenje takmicenje;
        private bool forViewingOnly;

        private List<RezultatUkupno> istiRezultati = new List<RezultatUkupno>();

        private RezultatskoTakmicenje ActiveTakmicenje
        {
            get { return cmbTakmicenje.SelectedItem as RezultatskoTakmicenje; }
            set { cmbTakmicenje.SelectedItem = value; }
        }

        public RezultatiUkupnoForm(int takmicenjeId, DeoTakmicenjaKod deoTakKod, int startRezTakmicenjeId,
            bool forViewingOnly)
        {
            InitializeComponent();
            this.deoTakKod = deoTakKod;
            this.forViewingOnly = forViewingOnly;

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

                    svaRezTakmicenja = loadRezTakmicenja(takmicenjeId);
                    if (svaRezTakmicenja.Count == 0)
                        throw new BusinessException(Strings.NO_KATEGORIJE_I_TAKMICENJA_ERROR_MSG);

                    rezTakmicenja = takmicenje.getRezTakmicenjaViseboj(svaRezTakmicenja, deoTakKod, false);
                    if (rezTakmicenja.Count == 0)
                        throw new BusinessException("Ne postoji takmicenje II ni za jednu kategoriju.");

                    RezultatskoTakmicenje startRezTakmicenje = null;
                    if (startRezTakmicenjeId != -1)
                    {
                        startRezTakmicenje = findRezTakmicenje(startRezTakmicenjeId, rezTakmicenja);
                        if (startRezTakmicenje == null)
                            throw new BusinessException("Ne postoje rezultati viseboj za dato takmicenje.");
                    }

                    initUI(startRezTakmicenje);
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
                    .FindByTakmicenjeFetch_Tak1_PoredakUkupno_Gimnasticari(takmicenjeId);
            else
                result = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                    .FindByTakmicenjeFetchTakmicenje2(takmicenjeId);

            foreach (RezultatskoTakmicenje rt in result)
            {
                // Potrebno u kvalColumnVisible
                NHibernateUtil.Initialize(rt.Propozicije);

                // NOTE: Moram ovako da inicijalizujem, zato sto ako probam
                // fetch u queriju, jako se sporo izvrsava (verovato
                // zato sto se dobavljaju dve kolekcije - Gimnasticari i 
                // Rezultati).
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                    NHibernateUtil.Initialize(rt.Takmicenje1.PoredakUkupno.Rezultati);
                else if (rt.odvojenoTak2())
                    NHibernateUtil.Initialize(rt.Takmicenje2.Poredak.Rezultati);

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

        private void initUI(RezultatskoTakmicenje startRezTakmicenje)
        {
            Text = "Rezultati - " + DeoTakmicenjaKodovi.toString(deoTakKod);
            this.ClientSize = new Size(1100, 540);

            cmbTakmicenje.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTakmicenje.DataSource = rezTakmicenja;
            cmbTakmicenje.DisplayMember = "Naziv";
            cmbTakmicenje.SelectedIndex = 0;
            if (startRezTakmicenje != null)
                ActiveTakmicenje = startRezTakmicenje;
            cmbTakmicenje.SelectedIndexChanged += new EventHandler(cmbTakmicenje_SelectedIndexChanged);

            dataGridViewUserControl1.DataGridView.MouseUp += new MouseEventHandler(DataGridView_MouseUp);
            dataGridViewUserControl1.GridColumnHeaderMouseClick += 
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
      
            if (forViewingOnly)
            {
                btnPrint.Enabled = btnPrint.Visible = false;
                btnIzracunaj.Enabled = btnIzracunaj.Visible = false;
                prikaziKlubToolStripMenuItem.Enabled = false;
                prikaziDrzavuToolStripMenuItem.Enabled = false;
                btnStampajKvalifikante.Enabled = btnStampajKvalifikante.Visible = false;
                btnStampajSaOgranicenjem.Enabled = btnStampajSaOgranicenjem.Visible = false;
            }
        }

        void DataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView grid = dataGridViewUserControl1.DataGridView;
            if (e.Button == MouseButtons.Right && grid.HitTest(e.X, e.Y).Type == DataGridViewHitTestType.Cell)
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
                contextMenuStrip1.Show(grid, new Point(e.X, e.Y));
            }
        }

        private bool kvalColumnVisible()
        {
            if (takmicenje.FinaleKupa)
                // Za finale kupa se kvalifikanti prikazuju u RezultatiUkupnoFinaleKupa
                return false;
            else
                return deoTakKod == DeoTakmicenjaKod.Takmicenje1 && ActiveTakmicenje.odvojenoTak2();
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<RezultatUkupno>(e.DataGridViewCellMouseEventArgs);
        }

        void cmbTakmicenje_SelectedIndexChanged(object sender, EventArgs e)
        {
            onSelectedTakmicenjeChanged();
        }

        private void onSelectedTakmicenjeChanged()
        {
            btnStampajKvalifikante.Enabled = !forViewingOnly && kvalColumnVisible();
            btnStampajSaOgranicenjem.Enabled = !forViewingOnly && deoTakKod == DeoTakmicenjaKod.Takmicenje1
                                               && !takmicenje.FinaleKupa && !ActiveTakmicenje.odvojenoTak2();
            if (dataGridViewUserControl1.DataGridView.Columns.Count == 0)
            {
                GridColumnsInitializer.initRezultatiUkupno(dataGridViewUserControl1,
                    takmicenje, kvalColumnVisible(), true);
                GridColumnsInitializer.maximizeColumnsRezultatiUkupno(dataGridViewUserControl1,
                    deoTakKod, rezTakmicenja);
            }
            else
            {
                // grid je vec inicijalizovan. podesi da velicine kolona budu nepromenjene.
                GridColumnsInitializer.reinitRezultatiUkupnoKeepColumnWidths(dataGridViewUserControl1,
                    takmicenje, kvalColumnVisible(), true);
            }            
            setItems();
        }

        private void setItems()
        {
            dataGridViewUserControl1.setItems<RezultatUkupno>(ActiveTakmicenje.getPoredakUkupno(deoTakKod).getRezultati());
            dataGridViewUserControl1.clearSelection();
        }

        private void cmbTakmicenja_DropDownClosed(object sender, EventArgs e)
        {
            dataGridViewUserControl1.Focus();
        }

        private void RezultatiUkupnoForm_Shown(object sender, EventArgs e)
        {
            onSelectedTakmicenjeChanged();
            dataGridViewUserControl1.Focus();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            stampaj(null, null);
        }

        // TODO5: Treba pitati da li da se stampa bonus, zato sto kod zena nema bonusa

        private void stampaj(PoredakUkupno p, string nazivIzvestaja)
        {
            if (String.IsNullOrEmpty(nazivIzvestaja))
            {
                nazivIzvestaja = ActiveTakmicenje.getNazivIzvestajaViseboj(deoTakKod, takmicenje.FinaleKupa, false);
            }

            HeaderFooterForm form = new HeaderFooterForm(deoTakKod, true, false, false, false, false, false, false, false,
                                                         true, false, false);
            string gym = GimnastikaUtil.getGimnastikaStr(takmicenje.Gimnastika, Opcije.Instance.Jezik);
            if (!Opcije.Instance.HeaderFooterInitialized)
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);

                string mestoDatum = takmicenje.Mesto + "  "
                    + takmicenje.Datum.ToShortDateString();
                form.Header1Text = ActiveTakmicenje.TakmicenjeDescription.Naziv;
                form.Header2Text = mestoDatum;
                form.Header3Text = gym + " - " + nazivIzvestaja;
                form.Header4Text = ActiveTakmicenje.Kategorija.Naziv;
                form.FooterText = mestoDatum;
            }
            else
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);
                form.Header1Text = ActiveTakmicenje.TakmicenjeDescription.Naziv;
                form.Header3Text = gym + " - " + nazivIzvestaja;
                form.Header4Text = ActiveTakmicenje.Kategorija.Naziv;
            }

            if (form.ShowDialog() != DialogResult.OK)
                return;
            FormUtil.initOpcijeFromHeaderFooterForm(form);
            Opcije.Instance.HeaderFooterInitialized = true;

            if (p == null)
                p = ActiveTakmicenje.getPoredakUkupno(deoTakKod);
            List<RezultatUkupnoExtended> rezultatiEx = null;

            bool extended = false;
            bool penalizacijaZaSprave = false;
            if (Opcije.Instance.PrikaziPenalizacijuViseboj)
            {
                extended = true;
                penalizacijaZaSprave = true;
            }
            else if (Opcije.Instance.PrikaziDEOcene)
            {
                extended = true;
                penalizacijaZaSprave = false;
            }
            
            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<Ocena> ocene = null;
                    if (extended)
                    {
                        ocene = DAOFactoryFactory.DAOFactory.GetOcenaDAO()
                            .FindByDeoTakmicenja(takmicenje.Id, deoTakKod);
                    }
                    rezultatiEx = p.getRezultatiExtended(ocene, extended,
                        ActiveTakmicenje.Propozicije.ZaPreskokVisebojRacunajBoljuOcenu);
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

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                string documentName = gym + " - " + nazivIzvestaja + " - " + ActiveTakmicenje.Kategorija.Naziv;
                PreviewDialog form2 = new PreviewDialog();
                form2.setIzvestaj(new UkupnoIzvestaj(rezultatiEx, extended, kvalColumnVisible(), p.hasPenalty(),
                    dataGridViewUserControl1.DataGridView, documentName, false, penalizacijaZaSprave, takmicenje,
                    new Font(form.TekstFont, form.TekstFontSize), form.ResizeByGrid));
                form2.ShowDialog();
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
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
            foreach (RezultatUkupno r in dataGridViewUserControl1.getSelectedItems<RezultatUkupno>())
                gimnasticari.Add(r.Gimnasticar);
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

            RezultatUkupno rez = dataGridViewUserControl1.getSelectedItem<RezultatUkupno>();
            dataGridViewUserControl1.refreshItems();
            dataGridViewUserControl1.setSelectedItem<RezultatUkupno>(rez);
        }

        private void findIstiRezultati()
        {
            istiRezultati.Clear();
            RezultatUkupno rez = dataGridViewUserControl1.getSelectedItem<RezultatUkupno>();
            if (rez == null)
                return;
            if (rez.Total == null)
                return;

            foreach (RezultatUkupno r in ActiveTakmicenje.getPoredakUkupno(deoTakKod).getRezultati())
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
            RezultatUkupno rez = dataGridViewUserControl1.getSelectedItem<RezultatUkupno>();
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

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    rez.KvalStatus = kvalStatus;
                    DAOFactoryFactory.DAOFactory.GetPoredakUkupnoDAO().Update(ActiveTakmicenje.getPoredakUkupno(deoTakKod));

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

            dataGridViewUserControl1.refreshItems();
            dataGridViewUserControl1.setSelectedItem<RezultatUkupno>(rez);
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

                    PoredakUkupno p = ActiveTakmicenje.getPoredakUkupno(deoTakKod);
                    p.create(ActiveTakmicenje, ocene);
                    DAOFactoryFactory.DAOFactory.GetPoredakUkupnoDAO().Update(p);

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
                Close();
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

        private void mnPromeniPoredakZaIsteOcene_Click(object sender, EventArgs e)
        {
            RazresiIsteOceneForm form = new RazresiIsteOceneForm(istiRezultati, takmicenje);
            if (form.ShowDialog() != DialogResult.OK)
                return;

            for (int i = 0; i < istiRezultati.Count; ++i)
            {
                istiRezultati[i].Rank = (short)form.Poredak[i];
            }

            PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                TypeDescriptor.GetProperties(typeof(RezultatUkupno))["Rank"],
                TypeDescriptor.GetProperties(typeof(RezultatUkupno))["PrezimeIme"]
            };
            ListSortDirection[] sortDir = new ListSortDirection[] {
                ListSortDirection.Ascending,
                ListSortDirection.Ascending
            };

            short redBroj = istiRezultati[0].RedBroj;
            istiRezultati.Sort(new SortComparer<RezultatUkupno>(propDesc, sortDir));
            foreach (RezultatUkupno r in istiRezultati)
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
                    DAOFactoryFactory.DAOFactory.GetPoredakUkupnoDAO().Update(ActiveTakmicenje.getPoredakUkupno(deoTakKod));

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

            dataGridViewUserControl1.sort<RezultatUkupno>("RedBroj", ListSortDirection.Ascending);
            //dataGridViewUserControl1.refreshItems();
            dataGridViewUserControl1.setSelectedItem<RezultatUkupno>(istiRezultati[0]);
        }

        private void mnPenalizacija_Click(object sender, EventArgs e)
        {
            IList<RezultatUkupno> rezultati = dataGridViewUserControl1.getSelectedItems<RezultatUkupno>();
            if (rezultati.Count != 1)
                return;
            RezultatUkupno r = rezultati[0];

            PenalizacijaForm form = new PenalizacijaForm(r.Penalty, takmicenje);
            if (form.ShowDialog() != DialogResult.OK)
                return;

            Nullable<float> penalty = null;
            if (form.Penalizacija.Trim() != String.Empty)
                penalty = float.Parse(form.Penalizacija);
            PoredakUkupno p = ActiveTakmicenje.getPoredakUkupno(deoTakKod);
            p.promeniPenalizaciju(r, penalty, ActiveTakmicenje);

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO().Update(r.Gimnasticar);
                    DAOFactoryFactory.DAOFactory.GetPoredakUkupnoDAO().Update(p);

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
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            dataGridViewUserControl1.setItems<RezultatUkupno>(p.getRezultati());
            dataGridViewUserControl1.setSelectedItem<RezultatUkupno>(r);
        }

        private List<RezultatUkupnoExtended> getKvalifikantiIRezerve(List<RezultatUkupnoExtended> rezultatiEx)
        {
            List<RezultatUkupnoExtended> result = new List<RezultatUkupnoExtended>();
            foreach (RezultatUkupnoExtended r in rezultatiEx)
            {
                if (r.KvalStatus == KvalifikacioniStatus.Q)
                    result.Add(r);
            }
            foreach (RezultatUkupnoExtended r in rezultatiEx)
            {
                if (r.KvalStatus == KvalifikacioniStatus.R)
                    result.Add(r);
            }
            return result;
        }

        // TODO5: Dodaj "Bonus" kolonu u gridu

        private void btnStampajKvalifikante_Click(object sender, EventArgs e)
        {
            string nazivIzvestaja = "Finale vi" + Jezik.shMalo + "eboja - kvalifikanti i rezerve";
            string documentName = nazivIzvestaja + " - " + ActiveTakmicenje.Kategorija.Naziv;

            HeaderFooterForm form = new HeaderFooterForm(deoTakKod, true, false, false, false, false, false, false, false,
                                                         false, false, false);
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
            FormUtil.initOpcijeFromHeaderFooterForm(form);
            Opcije.Instance.HeaderFooterInitialized = true;

            PoredakUkupno p = ActiveTakmicenje.getPoredakUkupno(deoTakKod);
            List<RezultatUkupnoExtended> rezultatiEx = null;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<Ocena> ocene = DAOFactoryFactory.DAOFactory.GetOcenaDAO()
                        .FindByDeoTakmicenja(takmicenje.Id, deoTakKod);
                    rezultatiEx = p.getRezultatiExtended(ocene, Opcije.Instance.PrikaziDEOcene,
                        ActiveTakmicenje.Propozicije.ZaPreskokVisebojRacunajBoljuOcenu);
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

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                PreviewDialog form2 = new PreviewDialog();
                form2.setIzvestaj(new UkupnoIzvestaj(getKvalifikantiIRezerve(rezultatiEx), Opcije.Instance.PrikaziDEOcene,
                    false, p.hasPenalty(), dataGridViewUserControl1.DataGridView, documentName, true, false, takmicenje,
                    new Font(form.TekstFont, form.TekstFontSize), form.ResizeByGrid));
                form2.ShowDialog();
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

        private void btnStampajSaOgranicenjem_Click(object sender, EventArgs e)
        {
            MaxGimIzKlubaUFinaluForm form = new MaxGimIzKlubaUFinaluForm();
            if (form.ShowDialog() != DialogResult.OK)
                return;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            PoredakUkupno p = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
                    IList<Ocena> ocene = ocenaDAO.FindByDeoTakmicenja(takmicenje.Id, DeoTakmicenjaKod.Takmicenje1);

                    p = new PoredakUkupno(DeoTakmicenjaKod.Takmicenje1);
                    p.create(ActiveTakmicenje, ocene, true, form.MaxBrojTakmicaraIzKluba, form.MaxBrojTakmicaraVaziZaDrzavu);

                    foreach (Ocena o in ocene)
                        ocenaDAO.Evict(o);
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
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            string nazivIzvestaja = "Finale vi" + Jezik.shMalo + "eboja";
            stampaj(p, nazivIzvestaja);
        }

        private void btnStampajPoKlubovimaIKategorijama_Click(object sender, EventArgs e)
        {
            string nazivIzvestaja = "Vi" + Jezik.shMalo + "eboj po klubovima i kategorijama";

            HeaderFooterForm form = new HeaderFooterForm(deoTakKod, false, false, false, false, false, false, false, false,
                                                         false, false, false);
            string gym = GimnastikaUtil.getGimnastikaStr(takmicenje.Gimnastika, Opcije.Instance.Jezik);
            if (!Opcije.Instance.HeaderFooterInitialized)
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);

                string mestoDatum = takmicenje.Mesto + "  "
                    + takmicenje.Datum.ToShortDateString();
                form.Header1Text = ActiveTakmicenje.TakmicenjeDescription.Naziv;
                form.Header2Text = mestoDatum;
                form.Header3Text = gym + " - " + nazivIzvestaja;
                form.Header4Text = "";
                form.FooterText = mestoDatum;
            }
            else
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);
                form.Header1Text = ActiveTakmicenje.TakmicenjeDescription.Naziv;
                form.Header3Text = gym + " - " + nazivIzvestaja;
                form.Header4Text = "";
            }

            if (form.ShowDialog() != DialogResult.OK)
                return;
            FormUtil.initOpcijeFromHeaderFooterForm(form);
            Opcije.Instance.HeaderFooterInitialized = true;
            
            List<RezultatUkupno> rezultati = new List<RezultatUkupno>();

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO =
                        DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();

                    // TODO5: Stampaj ceo bilten sa jednim klikom
                    // TODO5: Ovo dugme treba da je omoguceno samo za Takmicenje1
                    // TODO5: Dodaj mogucnost da ako je heder 1 predugacak pa ne moze da stane u jedan red, da moze da se
                    //        odredi gde ce biti novi red.
                    // TODO5: U hederu 2, gde stoji datum takmicenja, treba da postoji mogucnost za od-do

                    IDictionary<int, RezultatUkupno> rezultatiMap = new Dictionary<int, RezultatUkupno>();
                    foreach (RezultatskoTakmicenje rt in svaRezTakmicenja)
                    {
                        rezultatskoTakmicenjeDAO.Attach(rt, false);
                        foreach (RezultatUkupno r in rt.getPoredakUkupno(DeoTakmicenjaKod.Takmicenje1).Rezultati)
                        {
                            if (!rezultatiMap.ContainsKey(r.Gimnasticar.Id))
                                rezultatiMap.Add(r.Gimnasticar.Id, r);
                        }
                    }
                    foreach (RezultatUkupno r in rezultatiMap.Values)
                    {
                        rezultati.Add(r);
                    }
                    PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                            TypeDescriptor.GetProperties(typeof(RezultatUkupno))["KlubDrzava"],
                            TypeDescriptor.GetProperties(typeof(RezultatUkupno))["KategorijaRedBroj"],
                            TypeDescriptor.GetProperties(typeof(RezultatUkupno))["Total"],
                            TypeDescriptor.GetProperties(typeof(RezultatUkupno))["PrezimeIme"]
                    };
                    ListSortDirection[] sortDir = new ListSortDirection[] {
                            ListSortDirection.Ascending,
                            ListSortDirection.Ascending,
                            ListSortDirection.Descending,
                            ListSortDirection.Ascending
                    };
                    rezultati.Sort(new SortComparer<RezultatUkupno>(propDesc, sortDir));
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

            short redBroj = 0;
            foreach (RezultatUkupno r in rezultati)
            {
                // Ne smem da koristim Rank ili RedBroj da ne bih poremetio poredak po rezultatima ili poredak u gridu.
                r.RedBrojIzvestaj = ++redBroj;
            }

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                string documentName = gym + " - " + nazivIzvestaja;
                PreviewDialog form2 = new PreviewDialog();
                form2.setIzvestaj(new UkupnoIzvestaj(rezultati, takmicenje.Gimnastika,
                    dataGridViewUserControl1.DataGridView, documentName, takmicenje,
                    new Font(form.TekstFont, form.TekstFontSize), form.ResizeByGrid));
                form2.ShowDialog();
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