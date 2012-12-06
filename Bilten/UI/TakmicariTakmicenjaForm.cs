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
using System.Collections;

namespace Bilten.UI
{
    public partial class TakmicariTakmicenjaForm : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private IDataContext dataContext;
        private bool[] tabOpened;
        private StatusBar statusBar;
        
        public TakmicariTakmicenjaForm(int takmicenjeId)
        {
            InitializeComponent();
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                rezTakmicenja = loadRezTakmicenja(takmicenjeId);
                if (rezTakmicenja.Count == 0)
                    throw new BusinessException("Morate najpre da unesete takmicarske kategorije.");

                initUI();
                tabOpened = new bool[rezTakmicenja.Count];
                onSelectedIndexChanged();

                //dataContext.Commit();
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
            }
        }

        private IList<RezultatskoTakmicenje> loadRezTakmicenja(int takmicenjeId)
        {
            return dataContext.ExecuteNamedQuery<RezultatskoTakmicenje>(
                "FindRezTakmicenja",
                new string[] { "takmicenjeId" },
                new object[] { takmicenjeId });
        }

        private void initUI()
        {
            Text = "Takmicari - takmicenja";
            this.ClientSize = new Size(ClientSize.Width, 550);
            
            statusBar = new StatusBar();
            statusBar.Parent = this;
            statusBar.ShowPanels = true;
            StatusBarPanel sbPanel1 = new StatusBarPanel();
            statusBar.Panels.Add(sbPanel1);

            initTabs();
        }

        private void initTabs()
        {
            // init first tab
            tabPage1.Text = rezTakmicenja[0].Naziv;
            GridColumnsInitializer.initGimnasticarUcesnik(dataGridViewUserControl1);
            dataGridViewUserControl1.GridColumnHeaderMouseClick += 
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(dataGridViewUserControl_GridColumnHeaderMouseClick);

            // init other tabs
            for (int i = 1; i < rezTakmicenja.Count; i++)
            {
                TabPage newTab = new TabPage();
                tabControl1.Controls.Add(newTab);
                initTab(i, newTab, rezTakmicenja[i]);
            }
        }

        void dataGridViewUserControl_GridColumnHeaderMouseClick(object sender, 
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<GimnasticarUcesnik>(e.DataGridViewCellMouseEventArgs);
        }

        private void initTab(int i, TabPage tabPage, RezultatskoTakmicenje rezTakmicenje)
        {
            // TODO: Kod u ovom metodu je prekopiran iz Designer.cs fajla. Proveri
            // da li je u Designer.cs fajlu nesto menjano, i ako jeste promeni ovde.
            DataGridViewUserControl dataGridViewUserControl =
                new DataGridViewUserControl();
            dataGridViewUserControl.Anchor = this.dataGridViewUserControl1.Anchor;
            dataGridViewUserControl.Location = this.dataGridViewUserControl1.Location;
            dataGridViewUserControl.Size = this.dataGridViewUserControl1.Size;
            dataGridViewUserControl.TabIndex = this.dataGridViewUserControl1.TabIndex;
            GridColumnsInitializer.initGimnasticarUcesnik(dataGridViewUserControl);
            dataGridViewUserControl.GridColumnHeaderMouseClick += new EventHandler<GridColumnHeaderMouseClickEventArgs>(dataGridViewUserControl_GridColumnHeaderMouseClick);

            tabPage.SuspendLayout();    // NOTE: ovo je obavezno, jer bez toga naredba
            // tabPage.Controls.Add(dataGridViewUserControl) pozicionira
            // user controlu unutar taba pre nego sto je zavrsena inicijalizacija 
            // taba, i kao rezultat se dobija pogresna pozicija
            tabPage.Controls.Add(dataGridViewUserControl);
            tabPage.Location = this.tabPage1.Location;
            tabPage.Padding = this.tabPage1.Padding;
            tabPage.Size = this.tabPage1.Size;
            tabPage.TabIndex = i;
            tabPage.Text = rezTakmicenje.Naziv;
            tabPage.UseVisualStyleBackColor = this.tabPage1.UseVisualStyleBackColor;
            tabPage.ResumeLayout(false);
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

        private void onSelectedIndexChanged()
        {
            if (tabOpened[tabControl1.SelectedIndex])
            {
                updateGimnasticariCount();
                return;
            }

            tabOpened[tabControl1.SelectedIndex] = true;
            setGimnasticari(ActiveRezTakmicenje.Takmicenje1.Gimnasticari);
            getActiveDataGridViewUserControl().sort<GimnasticarUcesnik>(
                new string[] { "Prezime", "Ime" },
                new ListSortDirection[] { ListSortDirection.Ascending, ListSortDirection.Ascending });
            updateGimnasticariCount();
        }

        private void updateGimnasticariCount()
        {
            int count = getActiveDataGridViewUserControl().getItems<GimnasticarUcesnik>().Count;
            if (count == 1)
                statusBar.Panels[0].Text = count.ToString() + " gimnasticar";
            else
                statusBar.Panels[0].Text = count.ToString() + " gimnasticara";
        }

        private void setGimnasticari(ISet<GimnasticarUcesnik> gimnasticari)
        {
            getActiveDataGridViewUserControl().setItems<GimnasticarUcesnik>(gimnasticari);
        }

        private DataGridViewUserControl getActiveDataGridViewUserControl()
        {
            foreach (Control c in tabControl1.SelectedTab.Controls)
            {
                DataGridViewUserControl c2 = c as DataGridViewUserControl;
                if (c2 != null)
                    return c2;
            }
            return null;
        }

        private RezultatskoTakmicenje ActiveRezTakmicenje
        {
            get
            {
                return rezTakmicenja[tabControl1.SelectedIndex];
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult = DialogResult.None;
            SelectGimnasticarUcesnikForm form = null;
            try
            {
                form = new SelectGimnasticarUcesnikForm(
                    ActiveRezTakmicenje.Takmicenje.Id, ActiveRezTakmicenje.Pol,
                    ActiveRezTakmicenje.Kategorija);
                dlgResult = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (dlgResult != DialogResult.OK || form.SelectedEntities.Count == 0)
                return;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            bool added = false;
            List<GimnasticarUcesnik> illegalGimnasticari = new List<GimnasticarUcesnik>();
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                dataContext.Attach(ActiveRezTakmicenje, false);
                foreach (GimnasticarUcesnik g in ActiveRezTakmicenje.Takmicenje1.Gimnasticari)
                    dataContext.Attach(g, false);

                foreach (GimnasticarUcesnik g in form.SelectedEntities)
                {
                    if (canAddGimnasticar(ActiveRezTakmicenje, g))
                    {
                        ActiveRezTakmicenje.Takmicenje1.addGimnasticar(g);

                        IList<Ocena> ocene = loadOceneTak1(g);
                        ActiveRezTakmicenje.Takmicenje1
                            .gimnasticarAdded(g, ocene, ActiveRezTakmicenje);
                        added = true;
                    }
                    else
                    {
                        illegalGimnasticari.Add(g);
                    }
                }
                if (added)
                {
                    dataContext.Save(ActiveRezTakmicenje.Takmicenje1);
                    foreach (GimnasticarUcesnik g in ActiveRezTakmicenje.Takmicenje1.Gimnasticari)
                        dataContext.Evict(g);
                    dataContext.Commit();
                }
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
                MessageDialogs.showMessage(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                Close();
                return;
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;

                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }

            if (added)
            {
                setGimnasticari(ActiveRezTakmicenje.Takmicenje1.Gimnasticari);
                updateGimnasticariCount();
            }

            if (illegalGimnasticari.Count > 0)
            {
                string msg = "Sledeci gimnasticari nisu dodati: \n\n";
                msg += StringUtil.getListString(illegalGimnasticari.ToArray());
                //       MessageDialogs.showMessage(msg, this.Text);
            }
        }

        private bool canAddGimnasticar(RezultatskoTakmicenje rezTakmicenje, 
            GimnasticarUcesnik gimnasticar)
        {
            // TODO: Verovatno bi trebalo proveriti i kategoriju
            foreach (GimnasticarUcesnik g in rezTakmicenje.Takmicenje1.Gimnasticari)
            {
                if (g.Equals(gimnasticar))
                    return false;
            }
            return true;
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            IList<GimnasticarUcesnik> selItems = getActiveDataGridViewUserControl()
                .getSelectedItems<GimnasticarUcesnik>();
            if (selItems == null || selItems.Count == 0)
                return;

            bool delete;
            if (selItems.Count == 1)
            {
                delete = MessageDialogs.queryConfirmation(
                    deleteConfirmationMessage(selItems[0]), this.Text);
            }
            else
            {
                delete = MessageDialogs.queryConfirmation(
                    deleteConfirmationMessage(), this.Text);
            }
            if (!delete)
                return;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                dataContext.Attach(ActiveRezTakmicenje, false);
                foreach (GimnasticarUcesnik g in ActiveRezTakmicenje.Takmicenje1.Gimnasticari)
                    dataContext.Attach(g, false);
                
                foreach (GimnasticarUcesnik g in selItems)
                {
                    ActiveRezTakmicenje.Takmicenje1.removeGimnasticar(g);

                    // najpre ucitavam sprave na kojima je gimnasticar vezbao, da bih
                    // azurirao samo te poretke. Inace bi se u metodu 
                    // Takmicenje1.gimnasticarDeleted ucitavali svi poretci (da bi se
                    // proverilo u kojima se gimnasticar nalazi) i zatim bi se svi 
                    // ponovo snimali u bazu.
                    IList sprave = loadVezbaneSpraveTak1(g);
                    ActiveRezTakmicenje.Takmicenje1.gimnasticarDeleted(g, sprave,
                        ActiveRezTakmicenje);
                }

                dataContext.Save(ActiveRezTakmicenje.Takmicenje1);
                foreach (GimnasticarUcesnik g in ActiveRezTakmicenje.Takmicenje1.Gimnasticari)
                    dataContext.Evict(g);
                
                dataContext.Commit();
            
                setGimnasticari(ActiveRezTakmicenje.Takmicenje1.Gimnasticari);
                updateGimnasticariCount();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(
                    String.Format("{0} \n\n{1}", deleteErrorMessage(), ex.Message),
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

        private IList loadVezbaneSpraveTak1(GimnasticarUcesnik g)
        {
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                return dataContext.ExecuteNamedQuery(
                    "FindVezbaneSpraveForGimnasticar",
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

        private string deleteConfirmationMessage(GimnasticarUcesnik gimnasticar)
        {
            return String.Format("Da li zelite da izbrisete gimnasticara \"{0}\"?", gimnasticar);
        }

        private string deleteConfirmationMessage()
        {
            return String.Format("Da li zelite da izbrisete selektovane gimnasticare?");
        }

        private string deleteErrorMessage()
        {
            return "Neuspesno brisanje gimnasticara.";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}