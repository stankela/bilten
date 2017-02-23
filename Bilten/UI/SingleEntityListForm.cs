using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Bilten.Data;
using Bilten.Data.QueryModel;
using Bilten.Exceptions;
using Bilten.Domain;
using NHibernate;
using NHibernate.Context;

namespace Bilten.UI
{
    public partial class SingleEntityListForm<T> : BaseEntityListForm where T : DomainObject, new()
    {
        private ISession session;
        protected IDataContext dataContext;
        protected FilterForm filterForm;

        private IList<AssociationFetch> fetchModes = new List<AssociationFetch>();
        protected IList<AssociationFetch> FetchModes
        {
            get { return fetchModes; }
        }

        private StatusBar statusBar;
        public StatusBar StatusPanel
        {
            get { return statusBar; }
        }

        public SingleEntityListForm()
        {
            InitializeComponent();

            statusBar = new StatusBar();
            statusBar.Parent = this;
            statusBar.ShowPanels = true;
            StatusBarPanel sbPanel1 = new StatusBarPanel();
            statusBar.Panels.Add(sbPanel1);

            pnlPager.Visible = false;
            btnPrintItem.Visible = false;
            btnPrintPreview.Visible = false;
            btnDuplicate.Visible = false;
            btnShowHelp.Visible = false;
            btnApplySort.Visible = false;
            panel2.Height = 0;
            pnlPager.Height = 0; // nema efekta

            this.Load += new EventHandler(SingleEntityListForm_Load);
        }

        void SingleEntityListForm_Load(object sender, EventArgs e)
        {
            // TODO3: Ponisti selekciju i u ostalim prozorima gde je to potrebno (npr, takmicari  kategorije,
            // takmicari takmicenja, ekipe i sl.)
            dataGridViewUserControl1.clearSelection();
        }

        public T SelectedItem
        {
            get
            {
                return dataGridViewUserControl1.getSelectedItem<T>();
            }
            set
            {
                dataGridViewUserControl1.setSelectedItem<T>(value);
            }
        }

        protected void AddColumn(string columnTitle, string boundPropertyName)
        {
            dataGridViewUserControl1.AddColumn(columnTitle, boundPropertyName);
        }

        protected void AddColumn(string columnTitle, string boundPropertyName, int width, string formatString)
        {
            dataGridViewUserControl1.AddColumn(columnTitle, boundPropertyName, width, formatString);
        }

        protected void AddColumn(string columnTitle, string boundPropertyName, int width)
        {
            dataGridViewUserControl1.AddColumn(columnTitle, boundPropertyName, width);
        }

        private void btnEditItem_Click(object sender, EventArgs e)
        {
            Edit();
        }

        public virtual void Edit()
        {
            if (SelectedItem == null)
                return;
            int index = dataGridViewUserControl1.getSelectedItemIndex();

            try
            {
                EntityDetailForm form = createEntityDetailForm(getEntityId(SelectedItem));
                if (form.ShowDialog() == DialogResult.OK)
                {
                    T entity = (T)form.Entity;
                    List<T> items = dataGridViewUserControl1.getItems<T>();
                    items[index] = entity;
                    dataGridViewUserControl1.setItems<T>(items);  // ovo ponovo sortira items
                    dataGridViewUserControl1.setSelectedItem<T>(entity);
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }

        private int getEntityId(T entity)
        {
            return entity.Id;
        }

        private void btnNewItem_Click(object sender, EventArgs e)
        {
            AddNew();
        }

        protected virtual void AddNew()
        {
            try
            {
                EntityDetailForm form = createEntityDetailForm(null);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    T newEntity = (T)form.Entity;
                    List<T> items = dataGridViewUserControl1.getItems<T>();
                    items.Add(newEntity);
                    dataGridViewUserControl1.setItems<T>(items);
                    dataGridViewUserControl1.setSelectedItem<T>(newEntity);
                    updateEntityCount();
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }

        protected virtual EntityDetailForm createEntityDetailForm(Nullable<int> entityId)
        {
            throw new Exception("Derived classes should override this method.");
        }

        private void btnDeleteItem_Click(object sender, EventArgs e)
        {
            deleteCommand();
        }

        private void deleteCommand()
        {
            if (SelectedItem == null)
                return;
            if (!MessageDialogs.queryConfirmation(deleteConfirmationMessage(SelectedItem), this.Text))
                return;

            session = null;
            bool ok = false;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    if (refIntegrityDeleteDlg(SelectedItem))
                    {
                        delete(SelectedItem);
                        session.Transaction.Commit();
                        ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(
                    String.Format("{0} \n\n{1}", deleteErrorMessage(), ex.Message),
                    this.Text);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            if (ok)
            {
                List<T> items = dataGridViewUserControl1.getItems<T>();
                items.Remove(SelectedItem);
                CurrencyManager currencyManager =
                    (CurrencyManager)this.BindingContext[dataGridViewUserControl1.DataGridView.DataSource];
                currencyManager.Refresh();
                updateEntityCount();
            }
        }

        protected virtual void updateEntityCount()
        {

        }

        protected virtual string deleteConfirmationMessage(T item)
        {
            throw new NotImplementedException("Derived classes should override this method.");
        }

        // Metod koji proverava na koje referencijalne integritete utice brisanje
        // datog itema. Metod treba ili da spreci brisanje (povratna vrednost false),
        // ili da navede koje ce akcije biti preduzete da bi se odrzao referencijalni
        // integritet, i korisniku da priliku da odustane od brisanja.
        // NOTE: Inace, klase za koje treba analizirati referential constraints
        // prilikom brisanja su: klase sa dolaznom asocijacijom (usmerenom ka klasi)
        // cija je multiplikativnost 1 (jedino u tom slucaju postoji
        // foreign key od druge klase prema datoj klasi). Ovo istovremeno odredjuje
        // i forme koje treba da redefinisu ovaj metod.
        protected virtual bool refIntegrityDeleteDlg(T item)
        {
            return true;
        }

        protected virtual void delete(T item)
        {
            throw new Exception("Derived classes should override this method.");
        }

        protected virtual string deleteErrorMessage()
        {
            throw new NotImplementedException("Derived classes should override this method.");
        }

        public void SetItems(IList<T> list)
        {
            dataGridViewUserControl1.setItems<T>(list);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            onApplyFilter();
        }

        protected virtual void onApplyFilter()
        {

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            prikaziSve();
        }

        protected virtual void prikaziSve()
        {

        }
    }
}
