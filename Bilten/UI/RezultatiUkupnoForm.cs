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

namespace Bilten.UI
{
    public partial class RezultatiUkupnoForm : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private IDataContext dataContext;
        private bool[] takmicenjeOpened;
        private DeoTakmicenjaKod deoTakKod;
        private Takmicenje takmicenje;

        List<RezultatUkupnoExtended>[] rezultatiExtended;

        private List<RezultatUkupno> istiRezultati = new List<RezultatUkupno>();

        private RezultatskoTakmicenje ActiveTakmicenje
        {
            get { return cmbTakmicenje.SelectedItem as RezultatskoTakmicenje; }
            set { cmbTakmicenje.SelectedItem = value; }
        }

        public RezultatiUkupnoForm(int takmicenjeId, DeoTakmicenjaKod deoTakKod)
        {
            InitializeComponent();
            this.deoTakKod = deoTakKod;
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
                        if (rt.Propozicije.PostojiTak2)
                            rezTakmicenja.Add(rt);
                    }
                    if (rezTakmicenja.Count == 0)
                        throw new BusinessException("Ne postoji takmicenje II ni za jednu kategoriju.");
                }

                takmicenje = dataContext.GetById<Takmicenje>(takmicenjeId);
                NHibernateUtil.Initialize(takmicenje);

                if (takmicenje.FinaleKupa)
                {
                    List<RezultatskoTakmicenje> rezTakmicenja2 = new List<RezultatskoTakmicenje>(rezTakmicenja);
                    rezTakmicenja.Clear();
                    foreach (RezultatskoTakmicenje rt in rezTakmicenja2)
                    {
                        if (rt.Propozicije.OdvojenoTak2)
                            rezTakmicenja.Add(rt);
                    }
                    if (rezTakmicenja.Count == 0)
                        throw new BusinessException("Ne postoji posebno takmicenje II ni za jednu kategoriju.");
                }
                
                initUI();
                takmicenjeOpened = new bool[rezTakmicenja.Count];
                rezultatiExtended = new List<RezultatUkupnoExtended>[rezTakmicenja.Count];
                cmbTakmicenje.SelectedIndex = 0;

                cmbTakmicenje.SelectedIndexChanged += new EventHandler(cmbTakmicenje_SelectedIndexChanged);
     
                //onSelectedTakmicenjeChanged();
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

        private IList<RezultatskoTakmicenje> loadRezTakmicenja(int takmicenjeId)
        {
            string query;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                query = @"select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.PoredakUkupno
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
                    left join fetch r.Takmicenje2 t
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

                // NOTE: Moram ovako da inicijalizujem, zato sto ako probam
                // fetch u queriju, jako se sporo izvrsava (verovato
                // zato sto se dobavljaju dve kolekcije - Gimnasticari i 
                // Rezultati).
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                    NHibernateUtil.Initialize(tak.Takmicenje1.PoredakUkupno.Rezultati);
                else
                {
                    if (tak.Propozicije.PostojiTak2)
                        NHibernateUtil.Initialize(tak.Takmicenje2.Poredak.Rezultati);
                }

            }
            return result;
        }

        private void initUI()
        {
            Text = "Rezultati - " + DeoTakmicenjaKodovi.toString(deoTakKod);

            cmbTakmicenje.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTakmicenje.DataSource = rezTakmicenja;
            cmbTakmicenje.DisplayMember = "Naziv";

            dataGridViewUserControl1.DataGridView.CellMouseDown += new DataGridViewCellMouseEventHandler(DataGridView_CellMouseDown);
            dataGridViewUserControl1.DataGridView.MouseUp += new MouseEventHandler(DataGridView_MouseUp);
            dataGridViewUserControl1.GridColumnHeaderMouseClick += 
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);

            this.ClientSize = new Size(ClientSize.Width, 450);
        }

        void DataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0 && !dataGridViewUserControl1.DataGridView.Rows[e.RowIndex].Selected)
                {
                    // selektuj vrstu
                    dataGridViewUserControl1.clearSelection();
                    dataGridViewUserControl1.DataGridView.Rows[e.RowIndex].Selected = true;
                }
            }
        }

        void DataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView grid = dataGridViewUserControl1.DataGridView;
            int x = e.X;
            int y = e.Y;
            if (e.Button == MouseButtons.Right && grid.HitTest(x, y).Type == DataGridViewHitTestType.Cell)
            {
                mnQ.Enabled = /*mnQ.Visible =*/ kvalColumnVisible();
                mnR.Enabled = /*mnR.Visible =*/ kvalColumnVisible();
                mnPrazno.Enabled = /*mnPrazno.Visible =*/ kvalColumnVisible();
                findIstiRezultati();
                mnPromeniPoredakZaIsteOcene.Enabled = istiRezultati.Count > 1;
                contextMenuStrip1.Show(grid, new Point(x, y));
            }
        }

        private bool kvalColumnVisible()
        {
            return deoTakKod == DeoTakmicenjaKod.Takmicenje1
                && ActiveTakmicenje.Propozicije.PostojiTak2
                && ActiveTakmicenje.Propozicije.OdvojenoTak2;
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
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                onSelectedTakmicenjeChanged();
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

        private void onSelectedTakmicenjeChanged()
        {
            bool kvalColumn = kvalColumnVisible();
            if (takmicenje.FinaleKupa)
                kvalColumn = false;
            GridColumnsInitializer.initRezultatiUkupno(dataGridViewUserControl1,
                takmicenje, kvalColumn);
            
            if (!takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)])
            {
                takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)] = true;
            }

            dataGridViewUserControl1.setItems<RezultatUkupno>(getRezultati(ActiveTakmicenje));
            dataGridViewUserControl1.sort<RezultatUkupno>("RedBroj", ListSortDirection.Ascending);
        }

        private IList<RezultatUkupno> getRezultati(RezultatskoTakmicenje rezTakmicenje)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                return rezTakmicenje.Takmicenje1.PoredakUkupno.Rezultati;
            else
                return rezTakmicenje.Takmicenje2.Poredak.Rezultati;
        }

        private void cmbTakmicenja_DropDownClosed(object sender, EventArgs e)
        {
            dataGridViewUserControl1.Focus();
        }

        private void RezultatiUkupnoForm_Shown(object sender, EventArgs e)
        {
            dataGridViewUserControl1.Focus();
            cmbTakmicenje_SelectedIndexChanged(null, EventArgs.Empty);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //char shVeliko = '\u0160';
            char shMalo = '\u0161';
            string nazivIzvestaja;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                if (ActiveTakmicenje.Propozicije.OdvojenoTak2)
                    nazivIzvestaja = "Kvalifikacije za finale vi" + shMalo + "eboja";
                else
                    nazivIzvestaja = "Vi" + shMalo + "eboj";
            }
            else
            {
                if (ActiveTakmicenje.Propozicije.OdvojenoTak2)
                    nazivIzvestaja = "Finale vi" + shMalo + "eboja";
                else
                    nazivIzvestaja = "Vi" + shMalo + "eboj";
            }

            HeaderFooterForm form = new HeaderFooterForm(deoTakKod, true, false, false, false, false);
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

                List<RezultatUkupnoExtended> rezultati;
                bool extended = Opcije.Instance.PrikaziDEOcene;
                if (extended)
                {
                    rezultati = getRezultatiExtended(ActiveTakmicenje);
                }
                else
                {
                    rezultati = new List<RezultatUkupnoExtended>();

                    List<RezultatUkupno> rez =
                        new List<RezultatUkupno>(getRezultati(ActiveTakmicenje));
                    foreach (RezultatUkupno r in rez)
                    {
                        rezultati.Add(new RezultatUkupnoExtended(r));
                    }
                }
                
                PropertyDescriptor propDesc =
                    TypeDescriptor.GetProperties(typeof(RezultatUkupnoExtended))["RedBroj"];
                rezultati.Sort(new SortComparer<RezultatUkupnoExtended>(propDesc,
                    ListSortDirection.Ascending));

                p.setIzvestaj(new UkupnoIzvestaj(rezultati,
                    ActiveTakmicenje.Gimnastika, extended, kvalColumnVisible(), dataGridViewUserControl1.DataGridView));
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

        private List<RezultatUkupnoExtended> getRezultatiExtended(
            RezultatskoTakmicenje rezTakmicenje)
        {
            if (rezultatiExtended[rezTakmicenja.IndexOf(rezTakmicenje)] == null)
            {
                IList<RezultatUkupno> rezultati = getRezultati(ActiveTakmicenje);

                IList<Ocena> ocene;
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1
                || !rezTakmicenje.Propozicije.OdvojenoTak2)
                    ocene = loadOcene(takmicenje.Id, DeoTakmicenjaKod.Takmicenje1);
                else
                    ocene = loadOcene(takmicenje.Id, DeoTakmicenjaKod.Takmicenje2);

                IDictionary<int, RezultatUkupnoExtended> rezultatiMap = new Dictionary<int, RezultatUkupnoExtended>();
                foreach (RezultatUkupno rez in rezultati)
                {
                    RezultatUkupnoExtended rezEx = new RezultatUkupnoExtended(rez);
                    rezultatiMap.Add(rezEx.Gimnasticar.Id, rezEx);
                }

                foreach (Ocena o in ocene)
                {
                    if (rezultatiMap.ContainsKey(o.Gimnasticar.Id))
                    {
                        rezultatiMap[o.Gimnasticar.Id].setDOcena(o.Sprava, o.D);
                        rezultatiMap[o.Gimnasticar.Id].setEOcena(o.Sprava, o.E);
                    }
                }
                rezultatiExtended[rezTakmicenja.IndexOf(rezTakmicenje)] = new List<RezultatUkupnoExtended>(rezultatiMap.Values);
            }
            return rezultatiExtended[rezTakmicenja.IndexOf(rezTakmicenje)];
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

            dataGridViewUserControl1.refreshItems();
        }

        private void findIstiRezultati()
        {
            istiRezultati.Clear();
            RezultatUkupno rez = dataGridViewUserControl1.getSelectedItem<RezultatUkupno>();
            if (rez == null)
                return;
            if (rez.Total == null)
                return;

            List<RezultatUkupno> rezultati = getRezultatiSorted(ActiveTakmicenje, "RedBroj");
            foreach (RezultatUkupno r in rezultati)
            {
                if (r.Total == rez.Total)
                    istiRezultati.Add(r);
            }
        }

        private List<RezultatUkupno> getRezultatiSorted(RezultatskoTakmicenje rezTakmicenje,
            string sortColumn)
        {
            List<RezultatUkupno> result = new List<RezultatUkupno>(getRezultati(rezTakmicenje));
            PropertyDescriptor propDesc = TypeDescriptor.GetProperties(typeof(RezultatUkupno))[sortColumn];
            result.Sort(new SortComparer<RezultatUkupno>(propDesc, ListSortDirection.Ascending));
            return result;
        }

    }
}