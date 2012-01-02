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
    public partial class DrzaveForm : SingleEntityListForm<Drzava>
    {
        public DrzaveForm()
        {
            this.Text = "Drzave";
            dataGridViewUserControl1.GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            InitializeGridColumns();

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                IList<Drzava> drzave = loadAll();
                SetItems(drzave);
                dataGridViewUserControl1.sort<Drzava>(
                    new string[] { "Naziv" },
                    new ListSortDirection[] { ListSortDirection.Ascending });

                // NOTE: Iako pozivam Commit a ne pozivam Clear, ne generise se
                // UPDATE za svaku drzavu (isto vazi i za form koji prikazuje
                // klubove i kategorija). Izgleda da se UPDATE generise samo kada
                // entiteti koji se nalaze u sessionu imaju asocijacije prema drugim
                // entitetima (proveriti ovo; takodje proveriti da li ima nekakve veza
                // da li je asicijacija lazy ili ne)
        //        dataContext.Commit();
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

        private IList<Drzava> loadAll()
        {
            string query = @"from Drzava";
            IList<Drzava> result = dataContext.
                ExecuteQuery<Drzava>(QueryLanguageType.HQL, query,
                        new string[] { }, new object[] { });
            return result;
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<Drzava>(e.DataGridViewCellMouseEventArgs);
        }

        private void InitializeGridColumns()
        {
            AddColumn("Naziv drzave", "Naziv", 100);
            AddColumn("Skraceni kod", "Kod", 100);
        }

        protected override EntityDetailForm createEntityDetailForm(Nullable<int> entityId)
        {
            return new DrzavaForm(entityId);
        }

        protected override string deleteConfirmationMessage(Drzava drzava)
        {
            return String.Format("Da li zelite da izbrisete drzavu \"{0}\"?", drzava);
        }

        protected override bool refIntegrityDeleteDlg(Drzava drzava)
        {
            bool existsGimnasticari = existsGimnasticar(drzava);
            bool existsSudije = existsSudija(drzava);
            if (!existsGimnasticari && !existsSudije)
                return true;
            else
            {
                String s = String.Empty;
                if (existsGimnasticari)
                    s = "gimnasticari";
                if (existsSudije)
                {
                    if (existsGimnasticari)
                        s += " i ";
                    s += "sudije";
                }
                string msg = "Postoje {1} iz drzave '{0}'. Ako " +
                    "je izbrisete, ovi {1} nece imati navedenu drzavu. " +
                    "Da li zelite da izbrisete drzavu?";
                return MessageDialogs.queryConfirmation(String.Format(msg, drzava, s), this.Text);
            }
        }

        private bool existsGimnasticar(Drzava drzava)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Drzava", CriteriaOperator.Equal, drzava));
            return dataContext.GetCount<Gimnasticar>(q) > 0;
        }

        private bool existsSudija(Drzava drzava)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Drzava", CriteriaOperator.Equal, drzava));
            return dataContext.GetCount<Sudija>(q) > 0;
        }

        protected override void delete(Drzava drzava)
        {
            IList<Gimnasticar> gimnasticari = getGimnasticariByDrzava(drzava);
            IList<Sudija> sudije = getSudijeByDrzava(drzava);
            foreach (Gimnasticar g in gimnasticari)
            {
                g.Drzava = null;
                dataContext.Save(g);
            }
            foreach (Sudija s in sudije)
            {
                s.Drzava = null;
                dataContext.Save(s);
            }
            dataContext.Delete(drzava);
        }

        private IList<Gimnasticar> getGimnasticariByDrzava(Drzava drzava)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Drzava", CriteriaOperator.Equal, drzava));
            return dataContext.GetByCriteria<Gimnasticar>(q);
        }

        private IList<Sudija> getSudijeByDrzava(Drzava drzava)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Drzava", CriteriaOperator.Equal, drzava));
            return dataContext.GetByCriteria<Sudija>(q);
        }

        protected override string deleteErrorMessage()
        {
            return "Neuspesno brisanje drzave.";
        }

    }
}