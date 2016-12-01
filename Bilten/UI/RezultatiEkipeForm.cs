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
using Iesi.Collections.Generic;
using Bilten.Report;

namespace Bilten.UI
{
    public partial class RezultatiEkipeForm : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private IDataContext dataContext;
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
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                takmicenje = dataContext.GetById<Takmicenje>(takmicenjeId);
                NHibernateUtil.Initialize(takmicenje);

                IList<RezultatskoTakmicenje> svaRezTakmicenja = loadRezTakmicenja(takmicenjeId);
                if (svaRezTakmicenja.Count == 0)
                    throw new BusinessException("Morate najpre da unesete takmicarske kategorije.");

                rezTakmicenja = takmicenje.getRezTakmicenjaEkipe(svaRezTakmicenja, deoTakKod, false);
                if (rezTakmicenja.Count == 0)
                    throw new BusinessException("Ne postoji takmicenje IV ni za jednu kategoriju.");
                
                ocene = loadOcene(takmicenjeId, deoTakKod);
                sviRezultatiUkupno = takmicenje.createRezultatiUkupnoZaSveEkipe(rezTakmicenja, ocene, deoTakKod);
               
                initUI();
                takmicenjeOpened = new bool[rezTakmicenja.Count];
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
                    left join fetch t.PoredakEkipno
                    left join fetch t.Ekipe e
                    left join fetch e.Gimnasticari g
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    where r.Takmicenje.Id = :takmicenjeId
                    order by r.RedBroj";
            else
                query = @"select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje4 t
                    left join fetch t.Poredak
                    left join fetch t.Ucesnici u
                    left join fetch u.Ekipa e
                    left join fetch e.Gimnasticari g
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
            cmdSelectedIndexChanged();
        }

        void cmdSelectedIndexChanged()
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                if (onSelectedTakmicenjeChanged())
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
        }

        private bool kvalColumnVisible()
        {
            if (takmicenje.FinaleKupa)
                // Za finale kupa se kvalifikanti prikazuju u RezultatiEkipeFinaleKupa
                return false;
            else
                return ActiveTakmicenje.postojeKvalifikacijeEkipno(deoTakKod);
        }

        private bool onSelectedTakmicenjeChanged()
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

            
            bool save = false;
            if (!takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)])
            {
                takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)] = true;
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                {
                    ActiveTakmicenje.Takmicenje1.PoredakEkipno.create(ActiveTakmicenje, ocene);
                    dataContext.Save(ActiveTakmicenje.Takmicenje1.PoredakEkipno);
                }
                else
                {
                    ActiveTakmicenje.Takmicenje4.Poredak.create(ActiveTakmicenje, ocene);
                    dataContext.Save(ActiveTakmicenje.Takmicenje4.Poredak);
                }
                save = true;
            }

            dataGridViewUserControl1.setItems<RezultatEkipno>(
                ActiveTakmicenje.getPoredakEkipno(deoTakKod).getRezultati());
            onEkipeCellMouseClick();

            return save;
        }

        private void RezultatiEkipeForm_Shown(object sender, EventArgs e)
        {
            dataGridViewUserControl1.Focus();
            cmdSelectedIndexChanged();
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
                // TODO: Ispis za heder3 treba da bude drugaciji ako je jedno ekipno takmicenje za sve kategorije. Isto vazi
                // i za documentName
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

                bool kvalColumn = deoTakKod == DeoTakmicenjaKod.Takmicenje1
                && ActiveTakmicenje.Propozicije.PostojiTak4
                && ActiveTakmicenje.Propozicije.OdvojenoTak4;

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
            rezultat.addPenalty(0.1f);
        }

    }
}