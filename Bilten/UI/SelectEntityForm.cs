using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Data;
using Bilten.Data.QueryModel;
using Bilten.Exceptions;

namespace Bilten.UI
{
    public partial class SelectEntityForm : Form
    {
        protected FilterForm filterForm;

        protected IList<object> selectedEntities = new List<object>();
        public IList<object> SelectedEntities
        {
            get { return selectedEntities; }
        }

        public bool MultiSelect
        {
            set { dataGridViewUserControl1.DataGridView.MultiSelect = value; }
        }

        public DataGridViewUserControl DataGridViewUserControl
        {
            get { return dataGridViewUserControl1; }
        }

        public SelectEntityForm()
        {
            InitializeComponent();
        }

        protected void setEntities<T>(IList<T> entities)
        {
            dataGridViewUserControl1.setItems<T>(entities);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (dataGridViewUserControl1.DataGridView.SelectedRows.Count == 0)
            {
                DialogResult = DialogResult.None;
                return;
            }
            List<DataGridViewRow> selectedRows = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dataGridViewUserControl1.DataGridView.SelectedRows)
            {
                selectedRows.Add(row);
            }
            // NOTE: DataGridView.SelectedRows sadrzi selektovane vrste po obrnutom
            // vremenskom redosledu selekcije (zadnja vremenski selektovana vrsta
            // nalazi se na prvom mestu). Potrebno ih je sortirati po broju vrste.
            selectedRows.Sort(
                delegate(DataGridViewRow r1, DataGridViewRow r2)
                {
                    return r1.Index.CompareTo(r2.Index);
                }
            );

            foreach (DataGridViewRow row in selectedRows)
                selectedEntities.Add(row.DataBoundItem);
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (filterForm == null || filterForm.IsDisposed)
            {
                // NOTE: IsDisposed je true kada se form zatvori (bilo pritiskom na X
                // ili pozivom Close)

                try
                {
                    filterForm = createFilterForm(); // can throw
                    filterForm.Filter += new EventHandler(filterForm_Filter);
                    filterForm.Show();
                }
                catch (InfrastructureException ex)
                {
                    MessageDialogs.showError(ex.Message, this.Text);
                }
            }
            else
            {
                filterForm.Activate();
            }
        }

        protected virtual FilterForm createFilterForm()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private void filterForm_Filter(object sender, EventArgs e)
        {
            object filterObject = filterForm.FilterObject;
            if (filterObject != null)
                filter(filterObject);
        }

        protected virtual void filter(object filterObject)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private void SelectEntityForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (filterForm != null)
                filterForm.Close();
        }
    }
}