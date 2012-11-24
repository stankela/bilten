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

namespace Bilten.UI
{
    public partial class SingleEntityListForm<T> : BaseEntityListForm where T : DomainObject, new()
    {
        protected IDataContext dataContext;
        protected FilterForm filterForm;

        private IList<AssociationFetch> fetchModes = new List<AssociationFetch>();
        protected IList<AssociationFetch> FetchModes
        {
            get { return fetchModes; }
        }

        public SingleEntityListForm()
        {
            InitializeComponent();
            pnlPager.Visible = false;
            btnPrintItem.Visible = false;
            btnPrintPreview.Visible = false;
            btnDuplicate.Visible = false;
            btnShowHelp.Visible = false;
            btnApplySort.Visible = false;
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

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                if (refIntegrityDeleteDlg(SelectedItem))
                {
                    delete(SelectedItem);
                    dataContext.Commit();

                    List<T> items = dataGridViewUserControl1.getItems<T>();
                    items.Remove(SelectedItem);
                    CurrencyManager currencyManager =
                        (CurrencyManager)this.BindingContext[dataGridViewUserControl1.DataGridView.DataSource];
                    currencyManager.Refresh();
                }
                else
                {
                    dataContext.Rollback();
                }
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
            }
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
            dataContext.Delete(item);
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
