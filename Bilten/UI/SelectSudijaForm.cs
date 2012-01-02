using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Data.QueryModel;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;

namespace Bilten.UI
{
    public partial class SelectSudijaForm : SelectEntityForm
    {

        public SelectSudijaForm()
        {
            InitializeComponent();
            Text = "Izaberi sudiju";
            btnFilter.Enabled = false;
            initializeGridColumns();

            DataGridViewUserControl.GridColumnHeaderMouseClick += new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);

            FetchModes.Add(new AssociationFetch(
                "Drzava", AssociationFetchMode.Eager));

            showAll();
        }

        private void showAll()
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();

                IList<Sudija> sudije = loadSudije();
                setEntities(sudije);
                DataGridViewUserControl.sort<Sudija>(
                    new string[] { "Prezime", "Ime" },
                    new ListSortDirection[] { ListSortDirection.Ascending, ListSortDirection.Ascending });
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

        void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender, GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<Sudija>(e.DataGridViewCellMouseEventArgs);
        }

        private void initializeGridColumns()
        {
            // TODO: Sinhronizuj sa SudijeForm
            dataGridViewUserControl1.AddColumn("Ime", "Ime", 100);
            dataGridViewUserControl1.AddColumn("Prezime", "Prezime", 100);
            dataGridViewUserControl1.AddColumn("Pol", "Pol", 100);
            dataGridViewUserControl1.AddColumn("Drzava", "Drzava", 100);
        }

        private IList<Sudija> loadSudije()
        {
            Query q = new Query();
            foreach (AssociationFetch f in this.FetchModes)
                q.FetchModes.Add(f);
            q.OrderClauses.Add(new OrderClause("Prezime", OrderClause.OrderClauseCriteria.Ascending));
            q.OrderClauses.Add(new OrderClause("Ime", OrderClause.OrderClauseCriteria.Ascending));
            return dataContext.GetByCriteria<Sudija>(q);
        }

    }
}