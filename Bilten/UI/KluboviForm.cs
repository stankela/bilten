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
            InitializeGrid();
            InitializeGridColumns();
            FetchModes.Add(new AssociationFetch("Mesto", AssociationFetchMode.Eager));

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
            AddColumn("Naziv kluba", "Naziv", 200);
            AddColumn("Skraceni kod", "Kod", 100);
            AddColumn("Mesto", "Mesto", 100);
        }

        protected override EntityDetailForm createEntityDetailForm(Nullable<int> entityId)
        {
            return new KlubForm(entityId);
        }

        protected override int getEntityId(Klub entity)
        {
            return entity.Id;
        }

        protected override string DefaultSortingPropertyName
        {
            get { return "Naziv"; }
        }

        protected override void beforeSort()
        {
            for (int i = SortPropertyNames.Count - 1; i >= 0; i--)
            {
                if (SortPropertyNames[i] == "Mesto")
                {
                    // TODO: Obraditi situaciju (i ovde i na ostalim mestima) kada se
                    // sortira po koloni tipa entiteta. Tada nije moguce koristiti
                    //      SortPropertyNames[i] = "Mesto.Naziv";
                    // jer se dobija greska.
                    // Pokusaj da grid u tom slucaju sortiras rucno ili koristi HQL
                    // queries.
                }
            }
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