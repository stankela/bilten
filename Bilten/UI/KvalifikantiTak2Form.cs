using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;
using NHibernate;

namespace Bilten.UI
{
    public partial class KvalifikantiTak2Form : Form
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

        public KvalifikantiTak2Form(int takmicenjeId)
        {
            InitializeComponent();
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
                    if (rt.Propozicije.PostojiTak2 && rt.Propozicije.OdvojenoTak2)
                        rezTakmicenja.Add(rt);
                }
                if (rezTakmicenja.Count == 0)
                    throw new BusinessException("Ne postoji odvojeno takmicenje II ni za jednu kategoriju.");

                takmicenje = dataContext.GetById<Takmicenje>(takmicenjeId);

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
            string query = @"select distinct r
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
                NHibernateUtil.Initialize(tak.Propozicije);
                if (tak.Propozicije.PostojiTak2)
                    NHibernateUtil.Initialize(tak.Takmicenje2.Poredak.Rezultati);
            }
            return result;
        }

        private void initUI()
        {
            Text = "Kvalifikanti - " 
                + DeoTakmicenjaKodovi.toString(DeoTakmicenjaKod.Takmicenje2);

            cmbTakmicenje.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTakmicenje.DataSource = rezTakmicenja;
            cmbTakmicenje.DisplayMember = "Naziv";

            dataGridViewUserControl1.GridColumnHeaderMouseClick += new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            GridColumnsInitializer.initKvalifikantiTak2(dataGridViewUserControl1,
                takmicenje);
            dataGridViewUserControl1.DataGridView.MultiSelect = true;

            dataGridViewUserControl2.GridColumnHeaderMouseClick += new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            GridColumnsInitializer.initKvalifikantiTak2(dataGridViewUserControl2,
                takmicenje);
            dataGridViewUserControl2.DataGridView.MultiSelect = true;
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
          GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<UcesnikTakmicenja2>(e.DataGridViewCellMouseEventArgs);
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
            if (!takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)])
            {
                takmicenjeOpened[rezTakmicenja.IndexOf(ActiveTakmicenje)] = true;
            }

            dataGridViewUserControl1.setItems<UcesnikTakmicenja2>(
                ActiveTakmicenje.Takmicenje2.getUcesniciKvalifikanti());
            dataGridViewUserControl1.sort<UcesnikTakmicenja2>("QualOrder", ListSortDirection.Ascending);

            dataGridViewUserControl2.setItems<UcesnikTakmicenja2>(
                ActiveTakmicenje.Takmicenje2.getUcesniciRezerve());
            dataGridViewUserControl2.sort<UcesnikTakmicenja2>("QualOrder", ListSortDirection.Ascending);
        }

        private void cmbTakmicenje_DropDownClosed(object sender, EventArgs e)
        {
            dataGridViewUserControl1.Focus();
        }

        private void KvalifikantiTak2Form_Shown(object sender, EventArgs e)
        {
            dataGridViewUserControl1.Focus();
            cmbTakmicenje_SelectedIndexChanged(null, EventArgs.Empty);
        }

    }
}