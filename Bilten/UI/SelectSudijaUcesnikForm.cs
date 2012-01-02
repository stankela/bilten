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
    public partial class SelectSudijaUcesnikForm : SelectEntityForm
    {
        public SelectSudijaUcesnikForm(int takmicenjeId)
        {
            InitializeComponent();
            Text = "Izaberi sudiju";
            btnFilter.Enabled = false;
            initializeGridColumns();

            DataGridViewUserControl.GridColumnHeaderMouseClick += new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);

            FetchModes.Add(new AssociationFetch(
                "Drzava", AssociationFetchMode.Eager));

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();

                IList<SudijaUcesnik> sudije = loadSudije(takmicenjeId);
                setEntities(sudije);
                DataGridViewUserControl.sort<SudijaUcesnik>(
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
                dgwuc.onColumnHeaderMouseClick<SudijaUcesnik>(e.DataGridViewCellMouseEventArgs);
        }

        private void initializeGridColumns()
        {
            // TODO: Sinhronizuj sa SudijeForm
            dataGridViewUserControl1.AddColumn("Ime", "Ime", 100);
            dataGridViewUserControl1.AddColumn("Prezime", "Prezime", 100);
            dataGridViewUserControl1.AddColumn("Pol", "Pol", 100);
            dataGridViewUserControl1.AddColumn("Drzava", "Drzava", 100);
        }

        private IList<SudijaUcesnik> loadSudije(int takmicenjeId)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje.Id", CriteriaOperator.Equal, takmicenjeId));
            q.OrderClauses.Add(new OrderClause("Prezime", OrderClause.OrderClauseCriteria.Ascending));
            q.OrderClauses.Add(new OrderClause("Ime", OrderClause.OrderClauseCriteria.Ascending));
            foreach (AssociationFetch f in this.FetchModes)
                q.FetchModes.Add(f);
            return dataContext.GetByCriteria<SudijaUcesnik>(q);
        }

    }
}