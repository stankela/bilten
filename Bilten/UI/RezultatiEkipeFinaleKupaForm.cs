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
    public partial class RezultatiEkipeFinaleKupaForm : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private IDataContext dataContext;
        private bool[] takmicenjeOpened;
        private DeoTakmicenjaKod deoTakKod;
        private Takmicenje takmicenje;
        private IList<RezultatUkupno> rezultatiUkupno;
        private IList<Ocena> ocene;

        private RezultatskoTakmicenje ActiveTakmicenje
        {
            get { return cmbTakmicenje.SelectedItem as RezultatskoTakmicenje; }
            set { cmbTakmicenje.SelectedItem = value; }
        }

        public RezultatiEkipeFinaleKupaForm(int takmicenjeId, DeoTakmicenjaKod deoTakKod)
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

                rezTakmicenja = new List<RezultatskoTakmicenje>();
                foreach (RezultatskoTakmicenje rt in svaRezTakmicenja)
                {
                    if (rt.Propozicije.PostojiTak4 && rt.ImaEkipnoTakmicenje)
                        rezTakmicenja.Add(rt);
                }
                if (rezTakmicenja.Count == 0)
                    throw new BusinessException("Ne postoji takmicenje IV ni za jednu kategoriju.");

                ocene = loadOcene(takmicenjeId, deoTakKod);
                rezultatiUkupno = createRezultatiUkupno(rezTakmicenja, ocene);

                takmicenje = dataContext.GetById<Takmicenje>(takmicenjeId);
                NHibernateUtil.Initialize(takmicenje);

                initUI();
                takmicenjeOpened = new bool[rezTakmicenja.Count];
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

        private IList<RezultatUkupno> createRezultatiUkupno(
            IList<RezultatskoTakmicenje> rezTak, IList<Ocena> ocene)
        {
            List<GimnasticarUcesnik> gimnasticari = new List<GimnasticarUcesnik>();
            foreach (RezultatskoTakmicenje rt in rezTak)
            {
                IList<Ekipa> ekipe;
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                    ekipe = new List<Ekipa>(rt.Takmicenje1.Ekipe);
                else
                    ekipe = new List<Ekipa>(rt.Takmicenje4.getUcesnici());

                foreach (Ekipa e in ekipe)
                {
                    foreach (GimnasticarUcesnik g in e.Gimnasticari)
                    {
                        if (!gimnasticari.Contains(g))
                            gimnasticari.Add(g);
                    }
                }
            }
            
            IDictionary<int, RezultatUkupno> rezultatiMap = new Dictionary<int, RezultatUkupno>();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                RezultatUkupno rezultat = new RezultatUkupno();
                rezultat.Gimnasticar = g;
                rezultatiMap.Add(g.Id, rezultat);
            }

            foreach (Ocena o in ocene)
            {
                if (rezultatiMap.ContainsKey(o.Gimnasticar.Id))
                    rezultatiMap[o.Gimnasticar.Id].addOcena(o);
            }

            List<RezultatUkupno> result = new List<RezultatUkupno>(rezultatiMap.Values);
            return result;
        }

        private void initUI()
        {
            Text = "Rezultati ekipno - " + DeoTakmicenjaKodovi.toString(deoTakKod);

            cmbTakmicenje.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTakmicenje.DataSource = rezTakmicenja;
            cmbTakmicenje.DisplayMember = "NazivEkipnog";

            dataGridViewUserControl1.DataGridView.CellMouseClick += new DataGridViewCellMouseEventHandler(DataGridViewEkipe_CellMouseClick);
            dataGridViewUserControl1.GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            GridColumnsInitializer.initRezultatiUkupnoZaEkipe(dataGridViewUserControl2,
                takmicenje);
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
            IList<RezultatUkupno> rezultati = getRezultatiUkupno(e);
            dataGridViewUserControl2.setItems<RezultatUkupno>(rezultati);
            dataGridViewUserControl2.sort<RezultatUkupno>("PrezimeIme", ListSortDirection.Ascending);
        }

        private IList<RezultatUkupno> getRezultatiUkupno(Ekipa e)
        {
            IList<RezultatUkupno> result = new List<RezultatUkupno>();
            foreach (GimnasticarUcesnik g in e.Gimnasticari)
            {
                foreach (RezultatUkupno rez in rezultatiUkupno)
                {
                    if (g.Equals(rez.Gimnasticar))
                        result.Add(rez);
                }
            }
            return result;
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

        private bool onSelectedTakmicenjeChanged()
        {
            bool kvalColumn = deoTakKod == DeoTakmicenjaKod.Takmicenje1
                && ActiveTakmicenje.Propozicije.PostojiTak4
                && ActiveTakmicenje.Propozicije.OdvojenoTak4;
            GridColumnsInitializer.initRezultatiEkipno(dataGridViewUserControl1,
                takmicenje, kvalColumn);
            
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

            dataGridViewUserControl1.setItems<RezultatEkipno>(getRezultatiEkipno(ActiveTakmicenje));
            dataGridViewUserControl1.sort<RezultatEkipno>("RedBroj", ListSortDirection.Ascending);
            onEkipeCellMouseClick();

            return save;
        }

        private IList<RezultatEkipno> getRezultatiEkipno(RezultatskoTakmicenje rezTakmicenje)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                return rezTakmicenje.Takmicenje1.PoredakEkipno.Rezultati;
            else
                return rezTakmicenje.Takmicenje4.Poredak.Rezultati;
        }

        private void RezultatiEkipeFinaleKupaForm_Shown(object sender, EventArgs e)
        {
            dataGridViewUserControl1.Focus();
            cmbTakmicenje_SelectedIndexChanged(null, EventArgs.Empty);
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

            HeaderFooterForm form = new HeaderFooterForm(deoTakKod, false, false, false, false, false);
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

                List<RezultatEkipno> rezultatiEkipno =
                    new List<RezultatEkipno>(getRezultatiEkipno(ActiveTakmicenje));

                PropertyDescriptor propDesc =
                    TypeDescriptor.GetProperties(typeof(RezultatEkipno))["RedBroj"];
                rezultatiEkipno.Sort(new SortComparer<RezultatEkipno>(propDesc,
                    ListSortDirection.Ascending));

                bool kvalColumn = deoTakKod == DeoTakmicenjaKod.Takmicenje1
                && ActiveTakmicenje.Propozicije.PostojiTak4
                && ActiveTakmicenje.Propozicije.OdvojenoTak4;

                p.setIzvestaj(new EkipeIzvestaj(rezultatiEkipno, rezultatiUkupno,
                    ActiveTakmicenje.Gimnastika, kvalColumn));
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

    }
}