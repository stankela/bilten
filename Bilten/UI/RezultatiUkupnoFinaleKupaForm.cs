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
    public partial class RezultatiUkupnoFinaleKupaForm : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private IDataContext dataContext;
        private bool[] takmicenjeOpened;
        private Takmicenje takmicenje;

        private RezultatskoTakmicenje ActiveTakmicenje
        {
            get { return cmbTakmicenje.SelectedItem as RezultatskoTakmicenje; }
            set { cmbTakmicenje.SelectedItem = value; }
        }

        public RezultatiUkupnoFinaleKupaForm(int takmicenjeId)
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
                    if (rt.Propozicije.PostojiTak2)
                        rezTakmicenja.Add(rt);
                }
                if (rezTakmicenja.Count == 0)
                    throw new BusinessException("Ne postoji takmicenje II ni za jednu kategoriju.");

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
            foreach (RezultatskoTakmicenje tak in result)
            {
                // potrebno u Poredak.create
                NHibernateUtil.Initialize(tak.Propozicije);

                // TODO3: Ovo ce raditi samo ako su prvo i drugo kolo imali samo jedno takmicenje. (takodje i kod
                // poretka ekipa i sprava)
                PoredakUkupno poredak1 = loadPoredakUkupnoTak1(takmicenje.PrvoKolo.Id, tak.Kategorija);
                PoredakUkupno poredak2 = loadPoredakUkupnoTak1(takmicenje.DrugoKolo.Id, tak.Kategorija);
                tak.Takmicenje1.PoredakUkupnoFinaleKupa.create(tak, poredak1, poredak2);

            }
            return result;
        }

        private PoredakUkupno loadPoredakUkupnoTak1(int takmicenjeId, TakmicarskaKategorija kat)
        {
            string query = @"select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.Gimnasticari g
                    where r.Takmicenje.Id = :takmicenjeId
                    and kat.Naziv = :nazivKat";

            IList<RezultatskoTakmicenje> result = dataContext.
                ExecuteQuery<RezultatskoTakmicenje>(QueryLanguageType.HQL, query,
                        new string[] { "takmicenjeId", "nazivKat" },
                        new object[] { takmicenjeId, kat.Naziv });
            if (result.Count == 0)
                return null;

            RezultatskoTakmicenje rezTak = result[0];

            // NOTE: Moram ovako da inicijalizujem, zato sto ako probam
            // fetch u queriju, jako se sporo izvrsava (verovato
            // zato sto se dobavljaju dve kolekcije - Gimnasticari i 
            // Rezultati).
            NHibernateUtil.Initialize(rezTak.Takmicenje1.PoredakUkupno.Rezultati);
            return rezTak.Takmicenje1.PoredakUkupno;
        }

        private void initUI()
        {
            Text = "I i II Kolo - rezultati viseboj";

            cmbTakmicenje.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTakmicenje.DataSource = rezTakmicenja;
            cmbTakmicenje.DisplayMember = "Naziv";

            dataGridViewUserControl1.GridColumnHeaderMouseClick += 
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);

            this.ClientSize = new Size(ClientSize.Width, 450);
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<RezultatUkupnoFinaleKupa>(e.DataGridViewCellMouseEventArgs);
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
            bool kvalColumn = ActiveTakmicenje.Propozicije.OdvojenoTak2;
            GridColumnsInitializer.initRezultatiUkupnoFinaleKupa(dataGridViewUserControl1,
                takmicenje, kvalColumn);
            
            if (!takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)])
            {
                takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)] = true;
            }

            dataGridViewUserControl1.setItems<RezultatUkupnoFinaleKupa>(getRezultati(ActiveTakmicenje));
            dataGridViewUserControl1.sort<RezultatUkupnoFinaleKupa>("RedBroj", ListSortDirection.Ascending);
        }

        private IList<RezultatUkupnoFinaleKupa> getRezultati(RezultatskoTakmicenje rezTakmicenje)
        {
            return rezTakmicenje.Takmicenje1.PoredakUkupnoFinaleKupa.Rezultati;
        }

        private void cmbTakmicenja_DropDownClosed(object sender, EventArgs e)
        {
            dataGridViewUserControl1.Focus();
        }

        private void RezultatiUkupnoFinaleKupaForm_Shown(object sender, EventArgs e)
        {
            dataGridViewUserControl1.Focus();
            cmbTakmicenje_SelectedIndexChanged(null, EventArgs.Empty);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //char shVeliko = '\u0160';
            /*char shMalo = '\u0161';
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

                bool kvalColumn = deoTakKod == DeoTakmicenjaKod.Takmicenje1
                && ActiveTakmicenje.Propozicije.PostojiTak2
                && ActiveTakmicenje.Propozicije.OdvojenoTak2;

                p.setIzvestaj(new UkupnoIzvestaj(rezultati, 
                    ActiveTakmicenje.Gimnastika, extended, kvalColumn));
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
            }*/
        }

        /*private List<RezultatUkupnoExtended> getRezultatiExtended(
            RezultatskoTakmicenje rezTakmicenje)
        {
            if (rezultatiExtended == null)
            {
                IList<RezultatUkupno> rezultati = getRezultati(ActiveTakmicenje);

                IList<Ocena> ocene;
                if (deoTakKod == DeoTakmicenjaKod.Takmicenje1
                || rezTakmicenje.Propozicije.Tak2NaOsnovuTak1)
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
                rezultatiExtended = new List<RezultatUkupnoExtended>(rezultatiMap.Values);
            }
            return rezultatiExtended;
        }*/

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}