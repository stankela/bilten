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

namespace Bilten.UI
{
    public partial class SudijeForm : SingleEntityListForm<Sudija>
    {
        public SudijeForm()
        {
            this.Text = "Sudije";
            this.ClientSize = new System.Drawing.Size(800, 540);
            dataGridViewUserControl1.GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            InitializeGridColumns();

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                IList<Sudija> sudije = loadAll();
                SetItems(sudije);
                dataGridViewUserControl1.sort<Sudija>(
                    new string[] { "Prezime", "Ime" },
                    new ListSortDirection[] { ListSortDirection.Ascending, ListSortDirection.Ascending });
                updateSudijeCount();
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

        // TODO: Dodaj i druge stvari koje postoje u GimnasticariForm, kao npr. filtriranje, provera prilikom unosa da li
        // sudija vec postoji i ako postoji otvaranje dijaloga za edit.

        private void updateSudijeCount()
        {
            int count = dataGridViewUserControl1.getItems<Sudija>().Count;
            StatusPanel.Panels[0].Text = count.ToString() + " sudija";
        }

        private IList<Sudija> loadAll()
        {
            string query = @"from Sudija s
                left join fetch s.Drzava
                left join fetch s.Klub";
            IList<Sudija> result = dataContext.
                ExecuteQuery<Sudija>(QueryLanguageType.HQL, query,
                        new string[] { }, new object[] { });
            return result;
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<Sudija>(e.DataGridViewCellMouseEventArgs);
        }

        private void InitializeGridColumns()
        {
            AddColumn("Ime", "Ime", 100);
            AddColumn("Prezime", "Prezime", 100);
            AddColumn("Pol", "Pol", 100);
            AddColumn("Klub", "Klub", 150);
            AddColumn("Drzava", "Drzava", 100);
        }

        protected override EntityDetailForm createEntityDetailForm(Nullable<int> entityId)
        {
            return new SudijaForm(entityId);
        }

        protected override string deleteConfirmationMessage(Sudija sudija)
        {
            return String.Format("Da li zelite da izbrisete sudiju \"{0}\"?", sudija);
        }

        protected override string deleteErrorMessage()
        {
            return "Neuspesno brisanje sudije.";
        }

        protected override void updateEntityCount()
        {
            updateSudijeCount();
        }
    }
}