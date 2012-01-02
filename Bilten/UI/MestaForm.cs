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
    public partial class MestaForm : SingleEntityListForm<Mesto>
    {
        public MestaForm()
        {
            this.Text = "Mesta";
            InitializeGrid();
            InitializeGridColumns();

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
            AddColumn("Naziv mesta", "Naziv", 100);
        }

        protected override EntityDetailForm createEntityDetailForm(Nullable<int> entityId)
        {
            return new MestoForm(entityId);
        }

        protected override int getEntityId(Mesto entity)
        {
            return entity.Id;
        }

        protected override string DefaultSortingPropertyName
        {
            get { return "Naziv"; }
        }

        protected override string deleteConfirmationMessage(Mesto m)
        {
            return String.Format("Da li zelite da izbrisete mesto \"{0}\"?", m);
        }

        protected override bool refIntegrityDeleteDlg(Mesto m)
        {
            if (!existsKlub(m))
                return true;
            else
            {
                string msg = "Mesto '{0}' nije moguce izbrisati zato sto postoje " +
                    "klubovi iz datog mesta. Ako zelite da izbrisete mesto, morate " +
                    "najpre da izbrisete sve klubove iz datog mesta. ";
                MessageDialogs.showMessage(String.Format(msg, m), this.Text);
                return false;
            }
        }

        private bool existsKlub(Mesto m)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Mesto", CriteriaOperator.Equal, m));
            return dataContext.GetCount<Klub>(q) > 0;
        }

        protected override string deleteErrorMessage()
        {
            return "Neuspesno brisanje mesta.";
        }
    }
}