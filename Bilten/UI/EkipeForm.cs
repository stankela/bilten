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
using Bilten.Data.QueryModel;
using Iesi.Collections.Generic;
using NHibernate;

namespace Bilten.UI
{
    public partial class EkipeForm : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private IDataContext dataContext;
        private bool[] tabOpened;
        private bool[] clanoviSorted;

        public EkipeForm(int takmicenjeId)
        {
            InitializeComponent();

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                IList<RezultatskoTakmicenje> rezTak = loadRezTakmicenja(takmicenjeId);
                if (rezTak.Count == 0)
                    throw new BusinessException("Morate najpre da unesete takmicarske kategorije.");

                rezTakmicenja = new List<RezultatskoTakmicenje>();
                foreach (RezultatskoTakmicenje tak in rezTak)
                {
                    if (tak.Propozicije.PostojiTak4 && tak.ImaEkipnoTakmicenje)
                        rezTakmicenja.Add(tak);
                }
                if (rezTakmicenja.Count == 0)
                    throw new BusinessException("Ne postoje ekipna takmicenja ni za jednu kategoriju.");

                initUI();
                tabOpened = new bool[rezTakmicenja.Count];
                clanoviSorted = new bool[rezTakmicenja.Count];
                onSelectedIndexChanged();

            }
            catch (BusinessException)
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

        private void initUI()
        {
            Text = "Ekipe";
            this.ClientSize = new Size(900, 540);
            StartPosition = FormStartPosition.CenterScreen;
            initTabs();
        }

        private IList<RezultatskoTakmicenje> loadRezTakmicenja(int takmicenjeId)
        {
            string query = @"select distinct r
                from RezultatskoTakmicenje r
                left join fetch r.Kategorija kat
                left join fetch r.TakmicenjeDescription d
                left join fetch r.Takmicenje1 t
                left join fetch t.PoredakEkipno
                left join fetch t.Ekipe e
                left join fetch e.Gimnasticari g
                left join fetch g.DrzavaUcesnik dr
                left join fetch g.KlubUcesnik kl
                where r.Takmicenje.Id = :id
                order by r.RedBroj";
            IList<RezultatskoTakmicenje> result = dataContext.ExecuteQuery<RezultatskoTakmicenje>(QueryLanguageType.HQL, query,
                        new string[] { "id" }, new object[] { takmicenjeId });

            foreach (RezultatskoTakmicenje tak in result)
            {
                NHibernateUtil.Initialize(tak.Propozicije);
                NHibernateUtil.Initialize(tak.Takmicenje1.PoredakEkipno.Rezultati);
            }
            return result;
        }

        private RezultatskoTakmicenje loadRezTakmicenje(int id)
        {
            string query = @"select distinct r
                from RezultatskoTakmicenje r
                left join fetch r.Kategorija kat
                left join fetch r.TakmicenjeDescription d
                left join fetch r.Takmicenje1 t
                left join fetch t.PoredakEkipno
                left join fetch t.Ekipe e
                left join fetch e.Gimnasticari g
                left join fetch g.DrzavaUcesnik dr
                left join fetch g.KlubUcesnik kl
                where r.Id = :id";
            IList<RezultatskoTakmicenje> result = dataContext.
                ExecuteQuery<RezultatskoTakmicenje>(QueryLanguageType.HQL, query,
                        new string[] { "id" }, new object[] { id });
            foreach (RezultatskoTakmicenje tak in result)
            {
                NHibernateUtil.Initialize(tak.Propozicije);
                NHibernateUtil.Initialize(tak.Takmicenje1.PoredakEkipno.Rezultati);
            }
            if (result.Count == 0)
                return null;
            else
                return result[0];
        }

        private void initTabs()
        {
            //       this.tabControl1.SuspendLayout();

            // init first tab
            tabPage1.Text = rezTakmicenja[0].NazivEkipnog;
            ekipeUserControl1.EkipeDataGridViewUserControl.DataGridView.CellMouseClick +=
                new DataGridViewCellMouseEventHandler(DataGridView_CellMouseClick);
            ekipeUserControl1.EkipeDataGridViewUserControl.DataGridView.MultiSelect = false;
            ekipeUserControl1.ClanoviDataGridViewUserControl.DataGridView.MultiSelect = false;

            // init other tabs
            for (int i = 1; i < rezTakmicenja.Count; i++)
            {
                TabPage newTab = new TabPage();
                tabControl1.Controls.Add(newTab);
                initTab(i, newTab, rezTakmicenja[i]);
            }
            //     this.tabControl1.ResumeLayout(false);
        }

        private void initTab(int i, TabPage tabPage, RezultatskoTakmicenje rezTakmicenje)
        {
            // TODO: Kod u ovom metodu je prekopiran iz Designer.cs fajla. Proveri
            // da li je u Designer.cs fajlu nesto menjano, i ako jeste promeni ovde.
            EkipeUserControl ekipeUserControl = new EkipeUserControl();
            ekipeUserControl.Anchor = this.ekipeUserControl1.Anchor;
            ekipeUserControl.Location = this.ekipeUserControl1.Location;
            ekipeUserControl.Size = this.ekipeUserControl1.Size;
            ekipeUserControl.TabIndex = this.ekipeUserControl1.TabIndex;
            ekipeUserControl.EkipeDataGridViewUserControl.DataGridView.CellMouseClick +=
                new DataGridViewCellMouseEventHandler(DataGridView_CellMouseClick);
            ekipeUserControl.EkipeDataGridViewUserControl.DataGridView.MultiSelect = false;
            ekipeUserControl.ClanoviDataGridViewUserControl.DataGridView.MultiSelect = false;

            tabPage.SuspendLayout();
            tabPage.Controls.Add(ekipeUserControl);
            tabPage.Location = this.tabPage1.Location;
            tabPage.Padding = this.tabPage1.Padding;
            tabPage.Size = this.tabPage1.Size;
            tabPage.TabIndex = i;
            tabPage.Text = rezTakmicenje.NazivEkipnog;
            tabPage.UseVisualStyleBackColor = this.tabPage1.UseVisualStyleBackColor;
            tabPage.ResumeLayout(false);
        }

        void DataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            onEkipeCellMouseClick();
        }

        private void onEkipeCellMouseClick()
        {
            Ekipa selEkipa = getSelectedEkipa();
            if (selEkipa != null)
                setClanovi(selEkipa.Gimnasticari);
        }

        private void setClanovi(Iesi.Collections.Generic.ISet<GimnasticarUcesnik> gimnasticari)
        {
            getActiveClanoviDataGridViewUserControl()
                .setItems<GimnasticarUcesnik>(gimnasticari);
            if (!clanoviSorted[tabControl1.SelectedIndex])
            {
                getActiveClanoviDataGridViewUserControl().sort<GimnasticarUcesnik>(
                    new string[] { "Prezime", "Ime" },
                    new ListSortDirection[] { ListSortDirection.Ascending, ListSortDirection.Ascending });
                clanoviSorted[tabControl1.SelectedIndex] = true;
            }
        }

        private void onSelectedIndexChanged()
        {
            if (tabOpened[tabControl1.SelectedIndex])
                return;

            tabOpened[tabControl1.SelectedIndex] = true;
            setEkipe(ActiveRezTakmicenje.Takmicenje1.Ekipe);
            getActiveEkipeDataGridViewUserControl().sort<Ekipa>("Naziv", ListSortDirection.Ascending);
            onEkipeCellMouseClick();
        }

        private RezultatskoTakmicenje ActiveRezTakmicenje
        {
            get
            {
                return rezTakmicenja[tabControl1.SelectedIndex];
            }
        }

        private void setEkipe(Iesi.Collections.Generic.ISet<Ekipa> ekipe)
        {
            getActiveEkipeDataGridViewUserControl().setItems<Ekipa>(ekipe);
        }

        private DataGridViewUserControl getActiveEkipeDataGridViewUserControl()
        {
            foreach (Control c in tabControl1.SelectedTab.Controls)
            {
                EkipeUserControl c2 = c as EkipeUserControl;
                if (c2 != null)
                    return c2.EkipeDataGridViewUserControl;
            }
            return null;
        }

        private DataGridViewUserControl getActiveClanoviDataGridViewUserControl()
        {
            foreach (Control c in tabControl1.SelectedTab.Controls)
            {
                EkipeUserControl c2 = c as EkipeUserControl;
                if (c2 != null)
                    return c2.ClanoviDataGridViewUserControl;
            }
            return null;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();
                onSelectedIndexChanged();

                //dataContext.Commit();
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

        private void btnAddEkipa_Click(object sender, EventArgs e)
        {
            addEkipaCmd();
        }

        private void addEkipaCmd()
        {
            DialogResult dlgResult;
            EkipaForm form;
            try
            {
                form = new EkipaForm(null, ActiveRezTakmicenje.Id);
                dlgResult = form.ShowDialog();
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

                // ponovo ucitaj takmicenje zato sto je dodata ekipa
                rezTakmicenja[tabControl1.SelectedIndex] =
                    loadRezTakmicenje(ActiveRezTakmicenje.Id);

                Ekipa e = (Ekipa)form.Entity;
                List<Ocena> ocene = new List<Ocena>();
                foreach (GimnasticarUcesnik g in e.Gimnasticari)
                {
                    ocene.AddRange(loadOceneTak1(g));
                }
                ActiveRezTakmicenje.Takmicenje1.ekipaAdded(e, ocene, ActiveRezTakmicenje);

                dataContext.Save(ActiveRezTakmicenje.Takmicenje1.PoredakEkipno);
                foreach (GimnasticarUcesnik g in e.Gimnasticari)
                    dataContext.Evict(g);
                dataContext.Commit();
            }
            catch (InfrastructureException ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                Close();
                return;
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

            setEkipe(ActiveRezTakmicenje.Takmicenje1.Ekipe);
            setSelectedEkipa((Ekipa)form.Entity);
            onEkipeCellMouseClick();
        }

        private IList<Ocena> loadOceneTak1(GimnasticarUcesnik g)
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
                    new object[] { g, DeoTakmicenjaKod.Takmicenje1 });
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

        private void btnEditEkipa_Click(object sender, EventArgs e)
        {
            editEkipaCmd();
        }

        private void editEkipaCmd()
        {
            Ekipa selEkipa = getSelectedEkipa();
            if (selEkipa == null)
                return;
            DialogResult dlgResult;
            EkipaForm form;
            try
            {
                form = new EkipaForm(selEkipa.Id, ActiveRezTakmicenje.Id);
                dlgResult = form.ShowDialog();
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

                // ponovo ucitaj takmicenje zato sto je promenjena ekipa
                rezTakmicenja[tabControl1.SelectedIndex] =
                    loadRezTakmicenje(ActiveRezTakmicenje.Id);

                List<GimnasticarUcesnik> orig = new List<GimnasticarUcesnik>(
                    selEkipa.Gimnasticari);
                
                List<Ekipa> ekipe = new List<Ekipa>(ActiveRezTakmicenje.Takmicenje1.Ekipe);
                Ekipa ekipa = null;
                foreach (Ekipa e in ekipe)
                {
                    if (e.Id == selEkipa.Id)
                    {
                        ekipa = e;
                        break;
                    }
                }
                if (ekipa == null)
                {
                    throw new Exception("Greska u programu.");
                }

                List<GimnasticarUcesnik> curr = new List<GimnasticarUcesnik>(
                    ekipa.Gimnasticari);
                
                List<GimnasticarUcesnik> added = new List<GimnasticarUcesnik>();
                List<GimnasticarUcesnik> updated = new List<GimnasticarUcesnik>();
                List<GimnasticarUcesnik> deleted = new List<GimnasticarUcesnik>();
                diff(curr, orig, added, updated, deleted);

                foreach (GimnasticarUcesnik g in deleted)
                {
                    ActiveRezTakmicenje.Takmicenje1.gimnasticarDeletedFromEkipa(
                        g, ekipa, loadOceneTak1(g), ActiveRezTakmicenje);
                }
                foreach (GimnasticarUcesnik g in added)
                {
                    ActiveRezTakmicenje.Takmicenje1.gimnasticarAddedToEkipa(
                        g, ekipa, loadOceneTak1(g), ActiveRezTakmicenje);
                }

                dataContext.Save(ActiveRezTakmicenje.Takmicenje1.PoredakEkipno);
                foreach (GimnasticarUcesnik g in ekipa.Gimnasticari)
                    dataContext.Evict(g);
                dataContext.Commit();
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

            setEkipe(ActiveRezTakmicenje.Takmicenje1.Ekipe);
            setSelectedEkipa((Ekipa)form.Entity);
            onEkipeCellMouseClick();
        }

        private void diff<T>(IList<T> current, IList<T> original, IList<T> added,
            IList<T> updated, IList<T> deleted)
        {
            foreach (T t in current)
            {
                if (!contains(original, t))
                    added.Add(t);
                else
                    updated.Add(t);
            }
            foreach (T t in original)
            {
                if (!contains(current, t))
                    deleted.Add(t);
            }
        }

        private bool contains<T>(IList<T> list, T t)
        {
            foreach (T t2 in list)
            {
                if (t2.Equals(t))
                    return true;
            }
            return false;
        }
        
        Ekipa getSelectedEkipa()
        {
            return getActiveEkipeDataGridViewUserControl().getSelectedItem<Ekipa>();
        }

        public void setSelectedEkipa(Ekipa e)
        {
            getActiveEkipeDataGridViewUserControl().setSelectedItem<Ekipa>(e);
        }

        int getSelectedEkipaIndex()
        {
            return getActiveEkipeDataGridViewUserControl().getSelectedItemIndex();
        }

        void setSelectedEkipaIndex(int index)
        {
            getActiveEkipeDataGridViewUserControl().setSelectedItemIndex(index);
        }

        private void btnDeleteEkipa_Click(object sender, EventArgs e)
        {
            deleteEkipaCmd();
        }

        private void deleteEkipaCmd()
        {
            Ekipa selEkipa = getSelectedEkipa();
            if (selEkipa == null)
                return;
            string msgFmt = "Da li zelite da izbrisete ekipu '{0}'?";
            if (!MessageDialogs.queryConfirmation(String.Format(msgFmt, selEkipa), this.Text))
                return;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                int index = getSelectedEkipaIndex();

                delete(selEkipa);
                dataContext.Commit();

                setEkipe(ActiveRezTakmicenje.Takmicenje1.Ekipe);

                if (index < ActiveRezTakmicenje.Takmicenje1.Ekipe.Count)
                    setSelectedEkipaIndex(index);
                else if (ActiveRezTakmicenje.Takmicenje1.Ekipe.Count > 0)
                    setSelectedEkipaIndex(ActiveRezTakmicenje.Takmicenje1.Ekipe.Count - 1);
                else
                    getActiveClanoviDataGridViewUserControl().clearItems();
                onEkipeCellMouseClick();
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
        }

        private void delete(Ekipa ekipa)
        {
            ActiveRezTakmicenje.Takmicenje1.removeEkipa(ekipa);
            ActiveRezTakmicenje.Takmicenje1.ekipaDeleted(ekipa, ActiveRezTakmicenje);
            dataContext.Delete(ekipa);
            dataContext.Save(ActiveRezTakmicenje.Takmicenje1);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void EkipeForm_Load(object sender, EventArgs e)
        {
            onEkipeCellMouseClick();
        }

    }
}