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
            InitializeGrid();
            InitializeGridColumns();
            FetchModes.Add(new AssociationFetch(
                "Drzava", AssociationFetchMode.Eager));

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();
                ShowFirstPage();

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

        private void InitializeGridColumns()
        {
            AddColumn("Ime", "Ime", 100);
            AddColumn("Prezime", "Prezime", 100);
            AddColumn("Pol", "Pol", 100);
            AddColumn("Drzava", "Drzava", 100);
        }

        protected override EntityDetailForm createEntityDetailForm(Nullable<int> entityId)
        {
            return new SudijaForm(entityId);
        }

        protected override int getEntityId(Sudija entity)
        {
            return entity.Id;
        }

        protected override string DefaultSortingPropertyName
        {
            get { return "Prezime"; }
        }

        protected override string deleteConfirmationMessage(Sudija sudija)
        {
            return String.Format("Da li zelite da izbrisete sudiju \"{0}\"?", sudija);
        }

        protected override string deleteErrorMessage()
        {
            return "Neuspesno brisanje sudije.";
        }

    }
}