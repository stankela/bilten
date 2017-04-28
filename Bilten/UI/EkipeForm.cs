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
using Iesi.Collections.Generic;
using NHibernate;
using Bilten.Dao;
using Bilten.Dao.NHibernate;
using NHibernate.Context;
using Bilten.Misc;
using Bilten.Services;

namespace Bilten.UI
{
    public partial class EkipeForm : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private bool[] tabOpened;
        private bool[] clanoviSorted;
        private bool viseKola;
        private int takmicenjeId;

        public EkipeForm(int takmicenjeId)
        {
            InitializeComponent();
            this.takmicenjeId = takmicenjeId;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<RezultatskoTakmicenje> rezTak = loadRezTakmicenja(takmicenjeId);
                    if (rezTak.Count == 0)
                        throw new BusinessException(Strings.NO_KATEGORIJE_I_TAKMICENJA_ERROR_MSG);

                    rezTakmicenja = new List<RezultatskoTakmicenje>();
                    foreach (RezultatskoTakmicenje tak in rezTak)
                    {
                        if (tak.Propozicije.PostojiTak4 && tak.ImaEkipnoTakmicenje)
                            rezTakmicenja.Add(tak);
                    }
                    if (rezTakmicenja.Count == 0)
                        throw new BusinessException("Ne postoje ekipna takmicenja ni za jednu kategoriju.");

                    Takmicenje t = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                    viseKola = t.FinaleKupa || t.ZbirViseKola;

                    initUI();
                    tabOpened = new bool[rezTakmicenja.Count];
                    clanoviSorted = new bool[rezTakmicenja.Count];
                    onSelectedIndexChanged();
                }
            }
            catch (BusinessException)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw;
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
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
            IList<RezultatskoTakmicenje> result = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                .FindByTakmicenjeFetch_Tak1_Gimnasticari_PoredakEkipno(takmicenjeId);

            foreach (RezultatskoTakmicenje tak in result)
            {
                NHibernateUtil.Initialize(tak.Propozicije);
                NHibernateUtil.Initialize(tak.Takmicenje1.PoredakEkipno.Rezultati);
            }
            return result;
        }

        private RezultatskoTakmicenje loadRezTakmicenje(int id)
        {
            RezultatskoTakmicenje result = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                .FindByIdFetch_Tak1_Gimnasticari_PoredakEkipno(id);
            if (result != null)
            {
                NHibernateUtil.Initialize(result.Propozicije);
                NHibernateUtil.Initialize(result.Takmicenje1.PoredakEkipno.Rezultati);
            }
            return result;
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
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    onSelectedIndexChanged();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                // TODO: Da li ce ovaj izuzetak biti uhvacen?
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void btnAddEkipa_Click(object sender, EventArgs e)
        {
            addEkipaCmd();
        }

        private void addEkipaCmd()
        {
            EkipaForm form;
            try
            {
                form = new EkipaForm(null, ActiveRezTakmicenje.Id, !viseKola);
                if (form.ShowDialog() != DialogResult.OK)
                    return;
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    // ponovo ucitaj takmicenje zato sto je dodata ekipa
                    rezTakmicenja[tabControl1.SelectedIndex] = loadRezTakmicenje(ActiveRezTakmicenje.Id);
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showMessage(ex.Message, this.Text);
                return;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            setEkipe(ActiveRezTakmicenje.Takmicenje1.Ekipe);
            setSelectedEkipa((Ekipa)form.Entity);
            onEkipeCellMouseClick();
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
            EkipaForm form;
            try
            {
                form = new EkipaForm(selEkipa.Id, ActiveRezTakmicenje.Id, !viseKola);
                if (form.ShowDialog() != DialogResult.OK)
                    return;
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    // ponovo ucitaj takmicenje zato sto je promenjena ekipa
                    rezTakmicenja[tabControl1.SelectedIndex] = loadRezTakmicenje(ActiveRezTakmicenje.Id);
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            setEkipe(ActiveRezTakmicenje.Takmicenje1.Ekipe);
            setSelectedEkipa((Ekipa)form.Entity);
            onEkipeCellMouseClick();
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

            int index = getSelectedEkipaIndex();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    RezultatskoTakmicenjeService.deleteEkipaFromRezTak(selEkipa, ActiveRezTakmicenje);

                    Takmicenje t = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                    t.LastModified = DateTime.Now;
                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                Close();
                return;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        
            setEkipe(ActiveRezTakmicenje.Takmicenje1.Ekipe);
            if (index < ActiveRezTakmicenje.Takmicenje1.Ekipe.Count)
                setSelectedEkipaIndex(index);
            else if (ActiveRezTakmicenje.Takmicenje1.Ekipe.Count > 0)
                setSelectedEkipaIndex(ActiveRezTakmicenje.Takmicenje1.Ekipe.Count - 1);
            else
                getActiveClanoviDataGridViewUserControl().clearItems();
            onEkipeCellMouseClick();
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