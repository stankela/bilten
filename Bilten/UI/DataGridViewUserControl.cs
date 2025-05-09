using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Iesi.Collections.Generic;
using Bilten.Util;

namespace Bilten.UI
{
    public partial class DataGridViewUserControl : UserControl
    {
        private string[] sortProperties;
        private ListSortDirection[] sortDirections;
        public event EventHandler<GridColumnHeaderMouseClickEventArgs> 
            GridColumnHeaderMouseClick;
   
        public DataGridViewUserControl()
        {
            InitializeComponent();
            initializeGrid();
        }

        public DataGridView DataGridView
        {
            get { return dataGridView1; }
        }

        private bool columnHeaderSorting = true;
        public bool ColumnHeaderSorting
        {
            get { return columnHeaderSorting; }
            set { columnHeaderSorting = value; }
        }

        private void initializeGrid()
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
            dataGridView1.CellFormatting += new DataGridViewCellFormattingEventHandler(dataGridView1_CellFormatting);
            dataGridView1.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(dataGridView1_ColumnHeaderMouseClick);
            dataGridView1.MouseDown += new MouseEventHandler(dataGridView1_MouseDown);
        }

        void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.DataGridView.HitTestInfo info = DataGridView.HitTest(e.X, e.Y);
            if (e.Button == MouseButtons.Left)
            {
                if (info.Type == DataGridViewHitTestType.None)
                {
                    // None znaci da nije kliknut ni Cell, ni ColumnHeader, ni RowHeader, ni HorizontalScrollBar, ni
                    // VerticalScrollBar.
                    clearSelection();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // TODO: Ovaj deo je ranije bio u eventu CellMouseDown zato sto MouseDown event ne daje informacije o
                // RowIndex. Medjutim ova informacija (i jos ponesto) moze da se sazna pozivanjem
                // DataGridView.HitTest(e.X, e.Y). Probaj i na ostalim mestima da CellMouseDown zamenis sa MouseDown.

                if (info.Type == DataGridViewHitTestType.Cell)
                {
                    if (info.RowIndex >= 0 && !DataGridView.Rows[info.RowIndex].Selected)
                    {
                        // selektuj vrstu
                        clearSelection();
                        DataGridView.Rows[info.RowIndex].Selected = true;
                    }
                }
            }
        }

        void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // This method formats only boolean columns. Additional formating can be
            // specified in the AddColumn method.
            DataGridView dgw = (DataGridView)sender;
            if (e.Value == null)
                return;
            if (e.Value.GetType() == typeof(bool))
            {
                if ((bool)e.Value == true)
                    e.Value = "Da";
                else
                    e.Value = "Ne";
                e.FormattingApplied = true;
            }
            else if (e.Value.GetType() == typeof(SudijskaUloga))
            {
                e.Value = SudijskeUloge.toString((SudijskaUloga)e.Value);
                e.FormattingApplied = true;
            }
            else if (e.Value.GetType() == typeof(KvalifikacioniStatus))
            {
                e.Value = KvalifikacioniStatusi.toString((KvalifikacioniStatus)e.Value);
                e.FormattingApplied = true;
            }
            else if (e.Value.GetType() == typeof(TipTakmicenja))
            {
                if ((TipTakmicenja)e.Value == TipTakmicenja.FinaleKupa)
                    e.Value = "Finale kupa";
                else if ((TipTakmicenja)e.Value == TipTakmicenja.ZbirViseKola)
                    e.Value = "Zbir vise kola";
                else
                    e.Value = String.Empty;
                e.FormattingApplied = true;
            }
        }

        private DataGridViewColumn CreateGridColumn(string columnTitle, string boundPropertyName)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = boundPropertyName;
            column.HeaderText = columnTitle;
            column.Name = columnTitle;  // Name je potrebno da bi Columns kolekciju mogao da indeksiram po stringu.
            column.SortMode = DataGridViewColumnSortMode.Programmatic;
            return column;
        }

        public DataGridViewColumn AddColumn(string columnTitle, string boundPropertyName)
        {
            DataGridViewColumn column = CreateGridColumn(columnTitle, boundPropertyName);

            dataGridView1.Columns.Add(column);
            return column;
        }

        public DataGridViewColumn AddColumn(string columnTitle, string boundPropertyName, int width, string formatString)
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

            dataGridView1.CellFormatting += new DataGridViewCellFormattingEventHandler(delegate(object sender, DataGridViewCellFormattingEventArgs e)
            {
                DataGridView dgw = (DataGridView)sender;
                if (dgw.Columns[e.ColumnIndex].DataPropertyName == boundPropertyName)
                {
                    e.Value = string.Format(formatString, e.Value);
                    e.FormattingApplied = true;
                }
            });

            dataGridView1.Columns.Add(column);
            return column;
        }

        public DataGridViewColumn AddColumn(string columnTitle, string boundPropertyName, int width)
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
            dataGridView1.Columns.Add(column);
            return column;
        }

        public void HideColumn(int index)
        {
            dataGridView1.Columns[index].Visible = false;
        }

        public List<T> getItems<T>()
        {
            return DataGridView.DataSource as List<T>;
        }

        public int getItemCount<T>()
        {
            return (DataGridView.DataSource as List<T>).Count;
        }

        public void setItems<T>(List<T> items)
        {
            DataGridView.DataSource = new List<T>(items);
            resort<T>();
        }

        public void setItems<T>(IList<T> items)
        {
            // NOTE: Mora da se kreira nov objekt tipa List zato sto data binding
            // nece da radi za neke kolekcije koje interno koristi NHibernate.
            // Ovo znaci da ne sme da se koristi metod refreshItems (jer ce on
            // uvek osveziti staru kolekciju), vec svaki put kada treba prikazati
            // novo stanje treba koristiti setItems
            DataGridView.DataSource = new List<T>(items);
            resort<T>();
        }

        public void resort<T>()
        {
            if (isSorted())
                sort<T>(sortProperties, sortDirections);
        }

        public void setItems<T>(Iesi.Collections.Generic.ISet<T> items)
        {
            DataGridView.DataSource = new List<T>(items);
            resort<T>();
        }

        public void clearItems()
        {
            DataGridView.DataSource = null;
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // NOTE: Ne mogu direktno da pozovem onColumnHeaderMouseClick<T> zato sto
            // mi za to treba genericki parametar T (tip objekta u gridu). Zato se
            // prvo generise event, a primalac eventa (koji zna odgovarajuci tip T)
            // treba da pozove metod onColumnHeaderMouseClick<T>.
            OnGridColumnHeaderMouseClick(new GridColumnHeaderMouseClickEventArgs(e));
        }

        protected virtual void OnGridColumnHeaderMouseClick(
            GridColumnHeaderMouseClickEventArgs e)
        {
            // Save the delegate field in a temporary field for thread safety
            EventHandler<GridColumnHeaderMouseClickEventArgs> temp = GridColumnHeaderMouseClick;

            if (temp != null)
                temp(this, e);
        }

        public void onColumnHeaderMouseClick<T>(DataGridViewCellMouseEventArgs e)
        {
            if (!columnHeaderSorting)
                return;
            string propertyName = DataGridView.Columns[e.ColumnIndex].DataPropertyName;
            if (propertyName == String.Empty)
                return;
            columnHeaderSort<T>(propertyName);
        }

        private void columnHeaderSort<T>(string propertyName)
        {
            PropertyDescriptor propDesc = TypeDescriptor.GetProperties(typeof(T))[propertyName];
            if (propDesc == null)
                return;

            ListSortDirection direction = ListSortDirection.Ascending;
            if (isSortedByProperty(propertyName))
            {
                // toggleDirection
                direction = (sortDirections[0] == ListSortDirection.Ascending) ?
                    ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            doSort<T>(new PropertyDescriptor[] { propDesc },
                new ListSortDirection[] { direction });
        }

        public bool isSorted()
        {
            return sortProperties != null && sortProperties.Length > 0;
        }

        private bool isSortedByProperty(string propertyName)
        {
            return sortProperties != null 
                && sortProperties.Length == 1 
                && sortProperties[0] == propertyName;
        }

        public void sort<T>(string propertyName, ListSortDirection direction)
        {
            sort<T>(new string[] { propertyName }, 
                new ListSortDirection[] { direction });
        }

        public void sort<T>(string[] propertyNames, ListSortDirection[] directions)
        {
            PropertyDescriptor[] propDescriptors = new PropertyDescriptor[directions.Length];
            for (int i = 0; i < propertyNames.Length; i++)
            {
                PropertyDescriptor propDesc =
                    TypeDescriptor.GetProperties(typeof(T))[propertyNames[i]];
                if (propDesc == null)
                    return;
                propDescriptors[i] = propDesc;
            }

            doSort<T>(propDescriptors, directions);
        }

        private void doSort<T>(PropertyDescriptor[] propDesc, 
            ListSortDirection[] direction)
        {
            List<T> items = (List<T>)DataGridView.DataSource;
            items.Sort(new SortComparer<T>(propDesc, direction));
            refreshItems();

            sortProperties = new string[propDesc.Length];
            for (int i = 0; i < propDesc.Length; i++)
                sortProperties[i] = propDesc[i].Name;

            sortDirections = direction;
        }

        public void refreshItems()
        {
            getCurrencyManager().Refresh();
        }

        private CurrencyManager getCurrencyManager()
        {
            return (CurrencyManager)this.BindingContext[DataGridView.DataSource];
        }

        public int getSelectedItemIndex()
        {
            return getCurrencyManager().Position;
        }

        public void setSelectedItemIndex(int index)
        {
            getCurrencyManager().Position = index;
        }

        public T getSelectedItem<T>()
        {
            if (DataGridView.SelectedRows.Count > 0)
                return (T)DataGridView.SelectedRows[0].DataBoundItem;
            else
                return default(T);  
            // NOTE: Nije moguce return null. Zato se koristi default(T) koji ima 
            // vrednost null ako je T reference tip, tj. 0 ako je T value tip.
            // Alternativno, moguce je koristiti where T : class
        }

        public void setSelectedItem<T>(T item)
        {
            // NOTE: Da bi radilo ispravno, T mora da implementira Equals

            List<T> items = (List<T>)DataGridView.DataSource;
            int index = items.IndexOf(item);
            if (index >= 0)
                getCurrencyManager().Position = index;
        }

        public IList<T> getSelectedItems<T>()
        {
            IList<T> result = new List<T>();
            foreach (DataGridViewRow row in DataGridView.SelectedRows)
                result.Add((T)row.DataBoundItem);
            return result;
        }

        public int getSelectedItemCount()
        {
            return DataGridView.SelectedRows.Count;
        }

        public void clearSelection()
        {
            foreach (DataGridViewRow row in DataGridView.SelectedRows)
                row.Selected = false;
        }

    }
}
