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

namespace Bilten.UI
{
    public partial class RezultatiEkipeForm : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private bool[] takmicenjeOpened;
        private DeoTakmicenjaKod deoTakKod;
        private Takmicenje takmicenje;
        private IList<RezultatUkupno> sviRezultatiUkupno;
        private IList<Ocena> ocene;

        private RezultatskoTakmicenje ActiveTakmicenje
        {
            get { return cmbTakmicenje.SelectedItem as RezultatskoTakmicenje; }
            set { cmbTakmicenje.SelectedItem = value; }
        }

        public RezultatiEkipeForm(int takmicenjeId, DeoTakmicenjaKod deoTakKod)
        {
            InitializeComponent();
            this.deoTakKod = deoTakKod;

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

                    rezTakmicenja = takmicenje.getRezTakmicenjaEkipe(svaRezTakmicenja, deoTakKod, false);
                    if (rezTakmicenja.Count == 0)
                        throw new BusinessException("Ne postoji takmicenje IV ni za jednu kategoriju.");

                    ocene = loadOcene(takmicenjeId, deoTakKod);
                    sviRezultatiUkupno = takmicenje.createRezultatiUkupnoZaSveEkipe(rezTakmicenja, ocene, deoTakKod);

                    initUI();
                    takmicenjeOpened = new bool[rezTakmicenja.Count];
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
                    .FindByTakmicenjeFetch_Tak1_Gimnasticari_PoredakEkipno(takmicenjeId);
            else
                result = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                    .FindByTakmicenjeFetch_Tak4_Gimnasticari_Poredak(takmicenjeId);

            foreach (RezultatskoTakmicenje tak in result)
            {
                // potrebno u Poredak.create
                NHibernateUtil.Initialize(tak.Propozicije);

                // NOTE: Moram ovako da inicijalizujem, zato sto ako probam
                // fetch u queriju, jako se sporo izvrsava (verovato
                // zato sto se dobavljaju dve kolekcije - Gimnasticari i 
                // Rezultati).
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                    NHibernateUtil.Initialize(tak.Takmicenje1.PoredakEkipno.Rezultati);
                else
                {
                    if (tak.Propozicije.PostojiTak4)
                        NHibernateUtil.Initialize(tak.Takmicenje4.Poredak.Rezultati);
                }

            }
            return result;
        }

        private IList<Ocena> loadOcene(int takmicenjeId, DeoTakmicenjaKod deoTakKod)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
                    ocenaDAO.Session = session;
                    return ocenaDAO.FindByDeoTakmicenja(takmicenjeId, deoTakKod);
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {

            }                        
        }

        private void initUI()
        {
            Text = "Rezultati ekipno - " + DeoTakmicenjaKodovi.toString(deoTakKod);
            this.ClientSize = new Size(800, 540);

            cmbTakmicenje.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTakmicenje.DataSource = rezTakmicenja;
            cmbTakmicenje.DisplayMember = "NazivEkipnog";
            cmbTakmicenje.SelectedIndex = 0;
            cmbTakmicenje.SelectedIndexChanged += new EventHandler(cmbTakmicenje_SelectedIndexChanged);

            dataGridViewUserControl1.DataGridView.CellMouseClick += new DataGridViewCellMouseEventHandler(DataGridViewEkipe_CellMouseClick);
            dataGridViewUserControl1.GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);

            dataGridViewUserControl1.DataGridView.MouseUp += new MouseEventHandler(DataGridViewEkipe_MouseUp);
            dataGridViewUserControl2.DataGridView.MouseUp += new MouseEventHandler(DataGridView_MouseUp);
        }

        void DataGridViewEkipe_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView grid = dataGridViewUserControl1.DataGridView;
            if (e.Button == MouseButtons.Right && grid.HitTest(e.X, e.Y).Type == DataGridViewHitTestType.Cell)
            {
                contextMenuStrip2.Show(grid, new Point(e.X, e.Y));
            }
        }

        void DataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView grid = dataGridViewUserControl2.DataGridView;
            if (e.Button == MouseButtons.Right && grid.HitTest(e.X, e.Y).Type == DataGridViewHitTestType.Cell)
            {
                contextMenuStrip1.Show(grid, new Point(e.X, e.Y));
            }
        }

        void DataGridViewEkipe_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            onEkipeCellMouseClick();
        }

        private void onEkipeCellMouseClick()
        {
            RezultatEkipno rez = dataGridViewUserControl1.getSelectedItem<RezultatEkipno>();
            if (rez != null)
                setRezultatiUkupno(rez.Ekipa);
        }

        private void setRezultatiUkupno(Ekipa e)
        {
            dataGridViewUserControl2.setItems<RezultatUkupno>(e.getRezultatiUkupno(sviRezultatiUkupno));
            dataGridViewUserControl2.sort<RezultatUkupno>("PrezimeIme", ListSortDirection.Ascending);
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<RezultatEkipno>(e.DataGridViewCellMouseEventArgs);
        }

        void cmbTakmicenje_SelectedIndexChanged(object sender, EventArgs e)
        {
            onSelectedTakmicenjeChanged();
        }

        private bool kvalColumnVisible()
        {
            if (takmicenje.FinaleKupa)
                // Za finale kupa se kvalifikanti prikazuju u RezultatiEkipeFinaleKupa
                return false;
            else
                return deoTakKod == DeoTakmicenjaKod.Takmicenje1 && ActiveTakmicenje.odvojenoTak4();
        }

        private void onSelectedTakmicenjeChanged()
        {
            GridColumnsInitializer.initRezultatiEkipno(dataGridViewUserControl1, takmicenje, kvalColumnVisible(), true);
            GridColumnsInitializer.initRezultatiUkupnoZaEkipe(dataGridViewUserControl2, takmicenje);
            List<string> imena = new List<string>();
            List<string> klubovi = new List<string>();
            foreach (RezultatUkupno r in sviRezultatiUkupno)
            {
                imena.Add(r.Gimnasticar.PrezimeIme);
                klubovi.Add(r.Gimnasticar.KlubDrzava);
            }
            // TODO: Indexi kolona bi trebali da budu konstante
            if (imena.Count > 0)
            {
                dataGridViewUserControl2.DataGridView.Columns[0].Width =
                    GridColumnsInitializer.getMaxWidth(imena, dataGridViewUserControl2.DataGridView);
            }
            if (klubovi.Count > 0)
            {
                dataGridViewUserControl2.DataGridView.Columns[1].Width =
                    GridColumnsInitializer.getMaxWidth(klubovi, dataGridViewUserControl2.DataGridView);
            }

            if (!takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)])
                takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)] = true;

            dataGridViewUserControl1.setItems<RezultatEkipno>(
                ActiveTakmicenje.getPoredakEkipno(deoTakKod).getRezultati());
            onEkipeCellMouseClick();
        }

        private void RezultatiEkipeForm_Shown(object sender, EventArgs e)
        {
            dataGridViewUserControl1.Focus();
            onSelectedTakmicenjeChanged();
        }

        private void cmbTakmicenje_DropDownClosed(object sender, EventArgs e)
        {
            dataGridViewUserControl1.Focus();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string nazivIzvestaja;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                if (ActiveTakmicenje.Propozicije.OdvojenoTak4)
                    nazivIzvestaja = "Rezultati ekipno - kvalifikacije";
                else
                    nazivIzvestaja = "Rezultati ekipno";
            }
            else
            {
                if (ActiveTakmicenje.Propozicije.OdvojenoTak4)
                    nazivIzvestaja = "Finale ekipno";
                else
                    nazivIzvestaja = "Rezultati ekipno";
            }

            // TODO3: Dodaj opciju da li da se prikaze kolona za klub.
            // Razmisli da li u opcijama treba uvesti vise polja StampajKlub,
            // za svaki izvestaj po jedno. Sada postoji samo jedno polje koje
            // vazi za sve izvestaje. Razmisli da li to treba uvesti i za
            // ostala polja.
            HeaderFooterForm form = new HeaderFooterForm(deoTakKod,
                false, false, false, false, false, false, false);
            if (!Opcije.Instance.HeaderFooterInitialized)
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);

                string mestoDatum = takmicenje.Mesto + "  "
                    + takmicenje.Datum.ToShortDateString();
                form.Header1Text = ActiveTakmicenje.TakmicenjeDescription.Naziv;
                form.Header2Text = mestoDatum;
                // TODO: Ispis za heder3 treba da bude drugaciji ako je jedno ekipno takmicenje za sve kategorije.
                // Isto vazi i za documentName. Koristi svojstvo NazivEkipnog klase RezultatskoTakmicenje.
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
                PreviewDialog p = new PreviewDialog();

                string documentName = nazivIzvestaja + " - " + ActiveTakmicenje.Kategorija.Naziv;

                List<RezultatEkipno> rezultatiEkipno =
                    ActiveTakmicenje.getPoredakEkipno(deoTakKod).getRezultati();

                bool kvalColumn = deoTakKod == DeoTakmicenjaKod.Takmicenje1 && ActiveTakmicenje.odvojenoTak4();

                p.setIzvestaj(new EkipeIzvestaj(rezultatiEkipno, sviRezultatiUkupno,
                    ActiveTakmicenje.Gimnastika, kvalColumn, dataGridViewUserControl2.DataGridView, documentName));
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

        private void btnZatvori_Click(object sender, EventArgs e)
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
            foreach (RezultatUkupno r in dataGridViewUserControl2.getSelectedItems<RezultatUkupno>())
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
                    GimnasticarUcesnikDAO gimnasticarUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
                    foreach (GimnasticarUcesnik g in gimnasticari)
                    {
                        g.NastupaZaDrzavu = !prikaziKlub;
                        gimnasticarUcesnikDAO.Update(g);
                    }
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

            RezultatUkupno rez = dataGridViewUserControl2.getSelectedItem<RezultatUkupno>();
            dataGridViewUserControl2.refreshItems();
            dataGridViewUserControl2.setSelectedItem<RezultatUkupno>(rez);
        }

        private void dodajPenalizacijuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IList<RezultatEkipno> rezultatiEkipe = dataGridViewUserControl1.getSelectedItems<RezultatEkipno>();
            if (rezultatiEkipe.Count != 1)
                return;
            RezultatEkipno rezultat = rezultatiEkipe[0];

            PenalizacijaForm form = new PenalizacijaForm(rezultat, takmicenje);
            if (form.ShowDialog() != DialogResult.OK)
                return;

            Nullable<float> penalty = null;
            if (form.Penalizacija.Trim() != String.Empty)
                penalty = float.Parse(form.Penalizacija);
            ActiveTakmicenje.getPoredakEkipno(deoTakKod).dodajEkipnuPenalizaciju(rezultat.Ekipa, penalty, ActiveTakmicenje);

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    DAOFactoryFactory.DAOFactory.GetEkipaDAO().Update(rezultat.Ekipa);
                    DAOFactoryFactory.DAOFactory.GetPoredakEkipnoDAO().Update(ActiveTakmicenje.getPoredakEkipno(deoTakKod));
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

            dataGridViewUserControl1.setItems<RezultatEkipno>(
                ActiveTakmicenje.getPoredakEkipno(deoTakKod).getRezultati());
            dataGridViewUserControl1.setSelectedItem<RezultatEkipno>(rezultat);
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
                    PoredakEkipno p = ActiveTakmicenje.getPoredakEkipno(deoTakKod);
                    p.create(ActiveTakmicenje, loadOcene(takmicenje.Id, deoTakKod));
                    DAOFactoryFactory.DAOFactory.GetPoredakEkipnoDAO().Update(p);
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

            dataGridViewUserControl1.setItems<RezultatEkipno>(
                ActiveTakmicenje.getPoredakEkipno(deoTakKod).getRezultati());
            onEkipeCellMouseClick();
        }

    }
}