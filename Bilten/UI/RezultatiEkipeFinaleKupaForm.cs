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
        private Takmicenje takmicenje;

        private RezultatskoTakmicenje ActiveTakmicenje
        {
            get { return cmbTakmicenje.SelectedItem as RezultatskoTakmicenje; }
            set { cmbTakmicenje.SelectedItem = value; }
        }

        public RezultatiEkipeFinaleKupaForm(int takmicenjeId)
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
                    if (rt.Propozicije.PostojiTak4 && rt.ImaEkipnoTakmicenje)
                        rezTakmicenja.Add(rt);
                }
                if (rezTakmicenja.Count == 0)
                    throw new BusinessException("Ne postoji takmicenje IV ni za jednu kategoriju.");

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
                    left join fetch t.Ekipe e
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

                PoredakEkipno poredak1 = loadPoredakEkipnoTak1(takmicenje.PrvoKolo.Id, tak.Kategorija);
                PoredakEkipno poredak2 = loadPoredakEkipnoTak1(takmicenje.DrugoKolo.Id, tak.Kategorija);
                tak.Takmicenje1.PoredakEkipnoFinaleKupa.create(tak, poredak1, poredak2);
            }
            return result;
        }

        private PoredakEkipno loadPoredakEkipnoTak1(int takmicenjeId, TakmicarskaKategorija kat)
        {
            string query = @"select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Kategorija kat
                    left join fetch r.Takmicenje1 t
                    left join fetch t.Ekipe e
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
            NHibernateUtil.Initialize(rezTak.Takmicenje1.PoredakEkipno.Rezultati);
            return rezTak.Takmicenje1.PoredakEkipno;
        }

        private void initUI()
        {
            Text = "I i II Kolo - rezultati ekipno";

            cmbTakmicenje.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTakmicenje.DataSource = rezTakmicenja;
            cmbTakmicenje.DisplayMember = "NazivEkipnog";

            dataGridViewUserControl1.GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<RezultatEkipnoFinaleKupa>(e.DataGridViewCellMouseEventArgs);
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
            bool kvalColumn = !ActiveTakmicenje.Propozicije.Tak4NaOsnovuPrethodnihKola;
            GridColumnsInitializer.initRezultatiEkipnoFinaleKupa(dataGridViewUserControl1,
                takmicenje, kvalColumn);
            
            bool save = false;
            if (!takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)])
            {
                takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)] = true;
                //ActiveTakmicenje.Takmicenje1.PoredakEkipno.create(ActiveTakmicenje, ocene);
                //dataContext.Save(ActiveTakmicenje.Takmicenje1.PoredakEkipno);
                //save = true;
            }

            dataGridViewUserControl1.setItems<RezultatEkipnoFinaleKupa>(getRezultatiEkipno(ActiveTakmicenje));
            dataGridViewUserControl1.sort<RezultatEkipnoFinaleKupa>("RedBroj", ListSortDirection.Ascending);

            return save;
        }

        private IList<RezultatEkipnoFinaleKupa> getRezultatiEkipno(RezultatskoTakmicenje rezTakmicenje)
        {
            return rezTakmicenje.Takmicenje1.PoredakEkipnoFinaleKupa.Rezultati;
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
            /*string nazivIzvestaja;
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
            }*/
        }

        private void btnZatvori_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}