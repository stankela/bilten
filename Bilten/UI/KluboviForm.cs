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
    public partial class KluboviForm : SingleEntityListForm<Klub>
    {
        public KluboviForm()
        {
            this.Text = "Klubovi";
            dataGridViewUserControl1.GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            InitializeGridColumns();

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                IList<Klub> klubovi = loadAll();
                SetItems(klubovi);
                dataGridViewUserControl1.sort<Klub>(
                    new string[] { "Naziv" },
                    new ListSortDirection[] { ListSortDirection.Ascending });
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

        private IList<Klub> loadAll()
        {
            string query = @"from Klub k
                left join fetch k.Mesto";
            IList<Klub> result = dataContext.
                ExecuteQuery<Klub>(QueryLanguageType.HQL, query,
                        new string[] { }, new object[] { });
            return result;
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<Klub>(e.DataGridViewCellMouseEventArgs);
        }

        private void InitializeGridColumns()
        {
            AddColumn("Naziv kluba", "Naziv", 200);
            AddColumn("Skraceni kod", "Kod", 100);
            AddColumn("Mesto", "Mesto", 100);
        }

        protected override EntityDetailForm createEntityDetailForm(Nullable<int> entityId)
        {
            return new KlubForm(entityId);
        }

        protected override string deleteConfirmationMessage(Klub klub)
        {
            return String.Format("Da li zelite da izbrisete klub \"{0}\"?", klub);
        }

        protected override bool refIntegrityDeleteDlg(Klub klub)
        {
            if (!existsGimnasticar(klub))
                return true;
            else
            {
                string msg = "Postoje gimnasticari koji su clanovi kluba '{0}'. Ako " +
                    "ga izbrisete, ovi gimnasticari nece imati naveden klub. " +
                    "Da li zelite da izbrisete klub?";
                return MessageDialogs.queryConfirmation(String.Format(msg, klub), this.Text);
            }
        }

        private bool existsGimnasticar(Klub klub)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Klub", CriteriaOperator.Equal, klub));
            return dataContext.GetCount<Gimnasticar>(q) > 0;
        }

        protected override void delete(Klub klub)
        {
            IList<Gimnasticar> gimnasticari = getGimnasticariByKlub(klub);
            foreach (Gimnasticar g in gimnasticari)
            {
                g.Klub = null;
                dataContext.Save(g);
            }
            dataContext.Delete(klub);
        }

        private IList<Gimnasticar> getGimnasticariByKlub(Klub klub)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Klub", CriteriaOperator.Equal, klub));
            return dataContext.GetByCriteria<Gimnasticar>(q);
        }

        protected override string deleteErrorMessage()
        {
            return "Neuspesno brisanje kluba.";
        }

    }
}