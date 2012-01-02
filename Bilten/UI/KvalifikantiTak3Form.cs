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
using System.Collections;

namespace Bilten.UI
{
    public partial class KvalifikantiTak3Form : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private IDataContext dataContext;
        private Takmicenje takmicenje;

        // kljuc je rezTakmicenja.IndexOf(takmicenje) * (Sprava.Max + 1) + sprava
        private ISet<int> rezultatiOpened;

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

        public KvalifikantiTak3Form(int takmicenjeId)
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
                    if (rt.Propozicije.PostojiTak3 && rt.Propozicije.OdvojenoTak3)
                        rezTakmicenja.Add(rt);
                }
                if (rezTakmicenja.Count == 0)
                    throw new BusinessException("Ne postoji odvojeno takmicenje III ni za jednu kategoriju.");

                takmicenje = dataContext.GetById<Takmicenje>(takmicenjeId);
                NHibernateUtil.Initialize(takmicenje);

                initUI();
                rezultatiOpened = new HashedSet<int>();
                cmbTakmicenje.SelectedIndex = 0;
                cmbSprava.SelectedIndex = 0;

                cmbTakmicenje.SelectedIndexChanged += new EventHandler(selectedRezultatiChanged);
                cmbSprava.SelectedIndexChanged += new EventHandler(selectedRezultatiChanged);

                //onSelectedRezultatiChanged();
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
                NHibernateUtil.Initialize(tak.Propozicije);
                if (tak.Propozicije.PostojiTak3)
                {
                    foreach (PoredakSprava p in tak.Takmicenje3.Poredak)
                        NHibernateUtil.Initialize(p.Rezultati);
                    NHibernateUtil.Initialize(tak.Takmicenje3.PoredakPreskok.Rezultati);
                }
            }
            return result;
        }

        private void initUI()
        {
            Text = "Kvalifikanti - " 
                + DeoTakmicenjaKodovi.toString(DeoTakmicenjaKod.Takmicenje3);

            cmbTakmicenje.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTakmicenje.DataSource = rezTakmicenja;
            cmbTakmicenje.DisplayMember = "Naziv";

            cmbSprava.DropDownStyle = ComboBoxStyle.DropDownList;
            List<string> sprave = new List<string>(Sprave.getSpraveNazivi(takmicenje.Gimnastika));
            cmbSprava.Items.AddRange(sprave.ToArray());

            spravaGridUserControl1.DataGridViewUserControl.GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            spravaGridUserControl1.DataGridViewUserControl.DataGridView.MultiSelect = true;

            dataGridViewUserControl1.GridColumnHeaderMouseClick += new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            GridColumnsInitializer.initKvalifikantiTak3(dataGridViewUserControl1,
                takmicenje);
            dataGridViewUserControl1.DataGridView.MultiSelect = true;

            this.ClientSize = new Size(ClientSize.Width, 550);
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
           GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            dgwuc.onColumnHeaderMouseClick<UcesnikTakmicenja3>(e.DataGridViewCellMouseEventArgs);
        }

        void selectedRezultatiChanged(object sender, EventArgs e)
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
            initSpravaGridUserControl(ActiveSprava);

            int rezultatiKey = getRezultatiKey(ActiveTakmicenje, ActiveSprava);
            if (!rezultatiOpened.Contains(rezultatiKey))
            {
                rezultatiOpened.Add(rezultatiKey);
            }

            refreshKvalifikanti();
    
            dataGridViewUserControl1.setItems<UcesnikTakmicenja3>(
                ActiveTakmicenje.Takmicenje3.getUcesniciRezerve(ActiveSprava));
            dataGridViewUserControl1.sort<UcesnikTakmicenja3>("QualOrder", ListSortDirection.Ascending);
    }

        private void refreshKvalifikanti()
        {
            spravaGridUserControl1.DataGridViewUserControl
                .setItems<UcesnikTakmicenja3>(ActiveTakmicenje.Takmicenje3.getUcesniciKvalifikanti(ActiveSprava));
            spravaGridUserControl1.DataGridViewUserControl
                .sort<UcesnikTakmicenja3>("QualOrder", ListSortDirection.Ascending);
        }

        private void initSpravaGridUserControl(Sprava sprava)
        {
            spravaGridUserControl1.init(sprava);

            GridColumnsInitializer.initKvalifikantiTak3(
                spravaGridUserControl1.DataGridViewUserControl, takmicenje);
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

        private void KvalifikantiTak3Form_Shown(object sender, EventArgs e)
        {
            spravaGridUserControl1.DataGridViewUserControl.Focus();
            selectedRezultatiChanged(null, EventArgs.Empty);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult;
            KvalifikantiTak3EditorForm form;
            try
            {
                form = new KvalifikantiTak3EditorForm(takmicenje.Id, ActiveTakmicenje.Id, ActiveSprava);
                dlgResult = form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (dlgResult != DialogResult.OK)
                return;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                // ponovo ucitaj takmicenje
                RezultatskoTakmicenje rt = loadRezTakmicenje(ActiveTakmicenje.Id);
                int index;
                for (index = 0; index < rezTakmicenja.Count; index++)
                {
                    if (rezTakmicenja[index].Id == rt.Id)
                        break;
                }
                rezTakmicenja[index] = rt;
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                string msg = Strings.getFullDatabaseAccessExceptionMessage(ex);
                MessageDialogs.showMessage(msg, this.Text);
                Close();
                return;
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }

            CurrencyManager currencyManager =
                (CurrencyManager)this.BindingContext[cmbTakmicenje.DataSource];
            currencyManager.Refresh();
            refreshKvalifikanti();
        }

        private RezultatskoTakmicenje loadRezTakmicenje(int rezTakmicenjeId)
        {
            string query = @"select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje3 t
                    left join fetch t.Poredak
                    left join fetch t.Ucesnici u
                    left join fetch u.Gimnasticar g
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    where r.Id = :rezTakmicenjeId";

            IList<RezultatskoTakmicenje> result = dataContext.
                ExecuteQuery<RezultatskoTakmicenje>(QueryLanguageType.HQL, query,
                        new string[] { "rezTakmicenjeId" },
                        new object[] { rezTakmicenjeId });
            foreach (RezultatskoTakmicenje tak in result)
            {
                NHibernateUtil.Initialize(tak.Propozicije);
                foreach (PoredakSprava p in tak.Takmicenje3.Poredak)
                    NHibernateUtil.Initialize(p.Rezultati);
                NHibernateUtil.Initialize(tak.Takmicenje3.PoredakPreskok.Rezultati);
            }
            if (result.Count > 0)
                return result[0];
            else
                return null;
        }

    }
}
