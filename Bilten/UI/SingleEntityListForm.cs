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
        List<string> sortPropertyNames = new List<string>();
        int pageIndex;
        int pageCount;
        protected IDataContext dataContext;

        private IList<AssociationFetch> fetchModes = new List<AssociationFetch>();
        protected IList<AssociationFetch> FetchModes
        {
            get { return fetchModes; }
        }

        private IList<Criterion> criteria = new List<Criterion>();
        protected IList<Criterion> Criteria
        {
            get { return criteria; }
        }

        /// <summary>
        /// Creates a new instance of type T .
        /// </summary>
        /// <returns>The new instance</returns>
        protected /*abstract*/ virtual T CreateNewItem()
        {
            throw new NotImplementedException("Derived classes should override this method.");
        }

        /// <summary>
        /// Gets the property name used as the default sorting criteria
        /// </summary>
        protected /*abstract*/ virtual string DefaultSortingPropertyName
        {
            get { throw new NotImplementedException("Derived classes should override this method."); }
        }

        public SingleEntityListForm()
        {
            InitializeComponent();
            pnlPager.Visible = false;
            btnRefreshList.Visible = false;
            btnPrintItem.Visible = false;
            btnPrintPreview.Visible = false;
            btnDuplicate.Visible = false;
            btnShowHelp.Visible = false;
            btnApplyFilter.Visible = false;
            btnApplySort.Visible = false;
            /*if (pnlListPlaceholder.Controls.IndexOf(dgwItemList) < 0)
            {
                this.pnlListPlaceholder.Controls.Add(this.dgwItemList);
            }*/
            dgwItemList.CellFormatting += new DataGridViewCellFormattingEventHandler(dgwItemList_CellFormatting);
        }

        void dgwItemList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // This method formats only boolean columns. Additional formating can be
            // specified in the AddColumn method.
            DataGridView dgw = (DataGridView)sender;
            if (e.Value != null && e.Value.GetType() == typeof(bool))
            {
                if ((bool)e.Value == true)
                    e.Value = "Da";
                else
                    e.Value = "Ne";
                e.FormattingApplied = true;
            }
        }

        public override void PrepareItemSelection()
        {
            dgwItemList.DoubleClick += new EventHandler(dgwItemList_DoubleClick);
        }

        void dgwItemList_DoubleClick(object sender, EventArgs e)
        {
            //DialogResult = DialogResult.OK;
            //Close();
        }

        public T SelectedItem
        {
            get
            {
                if (dgwItemList.SelectedRows.Count > 0)
                    return (T)dgwItemList.SelectedRows[0].DataBoundItem;
                else
                    return null;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual void RefreshPageCounter()
        {
            lblPageCounter.Text = string.Format("Page {0} of {1}", this.PageIndex, this.PageCount);
        }

        public List<string> SortPropertyNames
        {
            get { return sortPropertyNames; }
        }

        public int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = value; }
        }

        public int PageCount
        {
            get { return pageCount; }
            set { pageCount = value; }
        }

        private static DataGridViewColumn CreateGridColumn(string columnTitle, string boundPropertyName)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = boundPropertyName;
            column.HeaderText = columnTitle;
            column.SortMode = DataGridViewColumnSortMode.Programmatic;
            return column;
        }

        protected void InitializeGrid()
        {
            dgwItemList.AllowUserToAddRows = false;
            dgwItemList.AllowUserToDeleteRows = false;
            dgwItemList.AutoGenerateColumns = false;
            dgwItemList.ReadOnly = true;
            dgwItemList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        /// <summary>
        /// Adds a column to the displayed grid.
        /// </summary>
        /// <param name="columnTitle">The column title.</param>
        /// <param name="boundPropertyName">The name of the bound property</param>
        protected void AddColumn(string columnTitle, string boundPropertyName)
        {
            DataGridViewColumn column = CreateGridColumn(columnTitle, boundPropertyName);

            dgwItemList.Columns.Add(column);
        }

        /// <summary>
        /// Adds a column to the displayed grid
        /// </summary>
        /// <param name="columnTitle">The column title.</param>
        /// <param name="boundPropertyName">The name of the bound property</param>
        /// <param name="width">The column width.</param>
        /// <param name="formatString">The column format string.</param>
        protected void AddColumn(string columnTitle, string boundPropertyName, int width, string formatString)
        {
            DataGridViewColumn column = CreateGridColumn(columnTitle, boundPropertyName);
            if (width == int.MaxValue)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                column.Width = width;
            }

            dgwItemList.CellFormatting += new DataGridViewCellFormattingEventHandler(delegate(object sender, DataGridViewCellFormattingEventArgs e)
            {
                DataGridView dgw = (DataGridView)sender;
                if (dgw.Columns[e.ColumnIndex].DataPropertyName == boundPropertyName)
                {
                    e.Value = string.Format(formatString, e.Value);
                    e.FormattingApplied = true;
                }
            });

            dgwItemList.Columns.Add(column);
        }

        /// <summary>
        /// Adds a column to the displayed grid
        /// </summary>
        /// <param name="columnTitle">The column title.</param>
        /// <param name="boundPropertyName">The name of the bound property</param>
        /// <param name="width">The column width.</param>
        protected void AddColumn(string columnTitle, string boundPropertyName, int width)
        {
            DataGridViewColumn column = CreateGridColumn(columnTitle, boundPropertyName);
            if (width == int.MaxValue)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                column.Width = width;
            }
            dgwItemList.Columns.Add(column);
        }

        public void SetBindingList(BindingList<T> list)
        {
            this.dgwItemList.DataSource = list;
        }

        private void btnEditItem_Click(object sender, EventArgs e)
        {
            Edit();
        }

        /// <summary>
        /// Edits the selected item
        /// </summary>
        public virtual void Edit()
        {
            if (SelectedItem == null)
                return;
            try
            {
                EntityDetailForm form = createEntityDetailForm(getEntityId(SelectedItem));
                if (form.ShowDialog() == DialogResult.OK)
                {
                    CurrencyManager currencyManager =
                        (CurrencyManager)this.BindingContext[dgwItemList.DataSource];
                    BindingList<T> items = dgwItemList.DataSource as BindingList<T>;
                    items[currencyManager.Position] = (T)form.Entity;
                    //currencyManager.Refresh();
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }

        // TODO: Ukloni ovaj metod kada prebacis Id u DomainObject
        protected virtual int getEntityId(T entity)
        {
            throw new Exception("Derived classes should override this method.");
        }

        private void btnNewItem_Click(object sender, EventArgs e)
        {
            AddNew();
        }

        /// <summary>
        /// Display the view used to create a new item
        /// </summary>
        protected virtual void AddNew()
        {
            try
            {
                EntityDetailForm form = createEntityDetailForm(null);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    BindingList<T> items = dgwItemList.DataSource as BindingList<T>;
                    items.Add((T)form.Entity);

                    CurrencyManager currencyManager =
                        (CurrencyManager)this.BindingContext[dgwItemList.DataSource];
                    currencyManager.Position = items.Count - 1;
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

        /// <summary>
        /// Deletes the selected item
        /// </summary>
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

                    BindingList<T> items = dgwItemList.DataSource as BindingList<T>;
                    items.Remove(SelectedItem);
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

        private void dgwItemList_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();
                this.SortPropertyNames.Clear();
                this.SortPropertyNames.Add(dgwItemList.Columns[e.ColumnIndex].DataPropertyName);
                beforeSort();
                ShowCurrentPage();
                dataContext.Clear();
                dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        protected virtual void beforeSort()
        {
            // Empty
        }

        /// <summary>
        /// Display the first page
        /// </summary>
        public void ShowFirstPage()
        {
            int pageCount = GetPageCount();
            PageCount = pageCount;
            PageIndex = 1;
            ShowCurrentPage();
        }

        /// <summary>
        /// Evaluates the page count
        /// </summary>
        /// <returns>The page count</returns>
        private int GetPageCount()
        {
            int itemCount = dataContext.GetCount<T>();
            int pageCount = itemCount / ConfigurationParameters.ItemsPerPage;
            if (itemCount % ConfigurationParameters.ItemsPerPage != 0)
            {
                pageCount++;
            }
            return pageCount;
        }

        private void btnShowFirstPage_Click(object sender, EventArgs e)
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();
                ShowFirstPage();    // dva prisutpa bazi
                dataContext.Clear();
                dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);

                // NOTE: Ovde nema efekta izbacivanje izuzetka jer se izuzetci unutar
                // forma ne propagiraju do koda koji je prikazao form. Eventualno bi
                // moglo da se pozove Close().

                // throw new InfrastructureException(message, ex);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        private void btnShowPreviousPage_Click(object sender, EventArgs e)
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();
                ShowPreviousPage();
                dataContext.Clear();
                dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        /// <summary>
        /// Display the previous page
        /// </summary>
        public void ShowPreviousPage()
        {
            int pageCount = GetPageCount();
            PageCount = pageCount;
            if (PageIndex > 1)
            {
                PageIndex--;
            }
            ShowCurrentPage();
        }

        private void btnShowNextPage_Click(object sender, EventArgs e)
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();
                ShowNextPage();
                dataContext.Clear();
                dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        /// <summary>
        /// Display the next page
        /// </summary>
        public void ShowNextPage()
        {
            int pageCount = GetPageCount();
            PageCount = pageCount;
            if (PageIndex < pageCount)
            {
                PageIndex++;
            }
            ShowCurrentPage();
        }

        private void btnShowLastPage_Click(object sender, EventArgs e)
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();
                ShowLastPage();
                dataContext.Clear();
                dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        /// <summary>
        /// Display the last page
        /// </summary>
        public void ShowLastPage()
        {
            int pageCount = GetPageCount();
            PageIndex = pageCount;
            PageCount = pageCount;
            ShowCurrentPage();
        }

        private void btnRefreshList_Click(object sender, EventArgs e)
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                ShowCurrentPage();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        /// <summary>
        /// Shows the current page 
        /// </summary>
        public virtual void ShowCurrentPage()
        {
            Query q = new Query();
            foreach (Criterion c in this.Criteria)
            {
                q.Criteria.Add(c);
            }
            foreach (string s in SortPropertyNames)
            {
                q.OrderClauses.Add(new OrderClause(s, OrderClause.OrderClauseCriteria.Ascending));
            }
            if (q.OrderClauses.Count == 0)
                q.OrderClauses.Add(new OrderClause(DefaultSortingPropertyName, OrderClause.OrderClauseCriteria.Ascending));

            foreach (AssociationFetch f in this.FetchModes)
            {
                q.FetchModes.Add(f);
            }

            // TODO: Ovde bi trebalo GetByCriteriaDistinct, za slucaj da entitet ima
            // asocijacije one-to-many i many-to-many, ali je problem sto ne postoji
            // metod koji prima 3 argumenta
            IList<T> items = dataContext.GetByCriteria<T>(q, PageIndex - 1, ConfigurationParameters.ItemsPerPage);
            RefreshPageCounter();
            SetItems(items);
        }

        public void SetItems(IList<T> list)
        {
            BindingList<T> items = new BindingList<T>(list);
            SetBindingList(items);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
