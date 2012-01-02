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

            this.ClientSize = new Size(ClientSize.Width, 450);
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

            spravaGridUserControl1.DataGridViewUserControl
                .setItems<UcesnikTakmicenja3>(ActiveTakmicenje.Takmicenje3.getUcesniciKvalifikanti(ActiveSprava));
            spravaGridUserControl1.DataGridViewUserControl
                .sort<UcesnikTakmicenja3>("QualOrder", ListSortDirection.Ascending);
    
            dataGridViewUserControl1.setItems<UcesnikTakmicenja3>(
                ActiveTakmicenje.Takmicenje3.getUcesniciRezerve(ActiveSprava));
            dataGridViewUserControl1.sort<UcesnikTakmicenja3>("QualOrder", ListSortDirection.Ascending);
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            RezultatiSpravaForm form = null;
            try
            {
                form = new RezultatiSpravaForm(takmicenje.Id, 
                    DeoTakmicenjaKod.Takmicenje1, true, ActiveTakmicenje, ActiveSprava);
                if (form.ShowDialog() != DialogResult.OK)
                    return;
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, this.Text);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }

            // moram da koristim Naziv zato sto nije implementiran Equals u klasi
            // RezultatskoTakmicenje
            if (form.SelectedTakmicenje.Naziv != ActiveTakmicenje.Naziv)
            {
                string msg = "Morate da izaberete kvalifikanta iz istog takmicenja " +
                    "kao ono koje je trenutno selektovano.";
                MessageDialogs.showMessage(msg, this.Text);
                return;
            }
            if (form.SelectedSprava != ActiveSprava)
            {
                string msg = "Morate da izaberete kvalifikanta za istu spravu " +
                    "kao ona koja je trenutno selektovana.";
                MessageDialogs.showMessage(msg, this.Text);
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                dataContext.Attach(ActiveTakmicenje, false);
                foreach (UcesnikTakmicenja3 u in ActiveTakmicenje.Takmicenje3.Ucesnici)
                    dataContext.Attach(u, false);

                if (ActiveSprava != Sprava.Preskok)
                {
                    RezultatSprava selResult = form.SelectedResult;
                    ActiveTakmicenje.Takmicenje3.addUcesnik(
                        new UcesnikTakmicenja3(selResult.Gimnasticar, ActiveSprava,
                            null, selResult.Total, selResult.Rank, KvalifikacioniStatus.Q));

                    Ocena o = loadOcena(selResult.Gimnasticar, DeoTakmicenjaKod.Takmicenje3,
                        ActiveSprava);
                    ActiveTakmicenje.Takmicenje3.getPoredak(ActiveSprava)
                        .addGimnasticar(selResult.Gimnasticar, o, ActiveTakmicenje);
                }
                else
                {
                    RezultatPreskok selResult = (RezultatPreskok)form.SelectedResult;
                    bool obaPreskoka = ActiveTakmicenje.Propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka;
                    Nullable<float> qualScore = obaPreskoka ? selResult.TotalObeOcene : selResult.Total;
                    Nullable<short> qualRank = obaPreskoka ? selResult.Rank2 : selResult.Rank;

                    ActiveTakmicenje.Takmicenje3.addUcesnik(
                        new UcesnikTakmicenja3(selResult.Gimnasticar, ActiveSprava,
                            null, qualScore, qualRank, KvalifikacioniStatus.Q));

                    Ocena o = loadOcena(selResult.Gimnasticar, DeoTakmicenjaKod.Takmicenje3,
                        ActiveSprava);
                    ActiveTakmicenje.Takmicenje3.PoredakPreskok
                        .addGimnasticar(selResult.Gimnasticar, o, ActiveTakmicenje);
                }

                dataContext.Save(ActiveTakmicenje.Takmicenje3);
                //foreach (UcesnikTakmicenja3 u in ActiveTakmicenje.Takmicenje3.Ucesnici)
                //  dataContext.Evict(u.Gimnasticar);

                dataContext.Commit();

                spravaGridUserControl1.DataGridViewUserControl
                    .setItems<UcesnikTakmicenja3>(ActiveTakmicenje.Takmicenje3.getUcesniciKvalifikanti(ActiveSprava));
            }
            catch (InfrastructureException ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
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

        private Ocena loadOcena(GimnasticarUcesnik g, DeoTakmicenjaKod deoTakKod,
            Sprava spava)
        {
            IList<Ocena> ocene = loadOcene(g, deoTakKod);
            foreach (Ocena o in ocene)
            {
                if (o.Sprava == spava)
                    return o;
            }
            return null;
        }

        private IList<Ocena> loadOcene(GimnasticarUcesnik g, DeoTakmicenjaKod deoTakKod)
        {
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                return dataContext.ExecuteNamedQuery<Ocena>(
                    "FindOceneForGimnasticar",
                    new string[] { "gim", "deoTakKod" },
                    new object[] { g, deoTakKod });
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            IList<UcesnikTakmicenja3> selItems = spravaGridUserControl1.DataGridViewUserControl
                .getSelectedItems<UcesnikTakmicenja3>();
            if (selItems == null || selItems.Count != 1)
                return;

            UcesnikTakmicenja3 selItem = selItems[0];
            string msg = String.Format("Da li zelite da izbrisete kvalifikanta \"{0}\"?", selItem.Gimnasticar);

            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                dataContext.Attach(ActiveTakmicenje, false);
                foreach (UcesnikTakmicenja3 u in ActiveTakmicenje.Takmicenje3.Ucesnici)
                    dataContext.Attach(u, false);

                ActiveTakmicenje.Takmicenje3.removeUcesnik(selItem);
                if (selItem.Sprava == Sprava.Preskok)
                    ActiveTakmicenje.Takmicenje3.PoredakPreskok.deleteGimnasticar(selItem.Gimnasticar, ActiveTakmicenje);
                else
                    ActiveTakmicenje.Takmicenje3.getPoredak(selItem.Sprava).deleteGimnasticar(selItem.Gimnasticar, ActiveTakmicenje);

                dataContext.Save(ActiveTakmicenje.Takmicenje3);
                //foreach (UcesnikTakmicenja3 u in ActiveTakmicenje.Takmicenje3.Ucesnici)
                  //  dataContext.Evict(u.Gimnasticar);

                dataContext.Commit();

                spravaGridUserControl1.DataGridViewUserControl
                    .setItems<UcesnikTakmicenja3>(ActiveTakmicenje.Takmicenje3.getUcesniciKvalifikanti(ActiveSprava));
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                string msg2 = "Neuspesno brisanje gimnasticara";
                MessageDialogs.showError(String.Format("{0} \n\n{1}", msg2, ex.Message),
                    this.Text);
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}