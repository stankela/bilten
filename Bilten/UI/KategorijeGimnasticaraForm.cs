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
    public partial class KategorijeGimnasticaraForm : SingleEntityListForm<KategorijaGimnasticara>
    {
        public KategorijeGimnasticaraForm()
        {
            this.Text = "Kategorije gimnasticara";
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
            AddColumn("Naziv kategorije", "Naziv", 100);
            AddColumn("Gimnastika", "Gimnastika", 70);
        }

        protected override EntityDetailForm createEntityDetailForm(Nullable<int> entityId)
        {
            return new KategorijaGimnasticaraForm(entityId);
        }

        protected override int getEntityId(KategorijaGimnasticara entity)
        {
            return entity.Id;
        }

        protected override string DefaultSortingPropertyName
        {
            get { return "Naziv"; }
        }

        protected override string deleteConfirmationMessage(KategorijaGimnasticara kat)
        {
            return String.Format("Da li zelite da izbrisete kategoriju \"{0}\"?", kat);
        }

        protected override bool refIntegrityDeleteDlg(KategorijaGimnasticara kategorija)
        {
            if (!existsGimnasticar(kategorija))
                return true;
            else
            {
                string msg = "Postoje gimnasticari kategorije '{0}'. Ako " +
                    "je izbrisete, ovi gimnasticari nece imati navedenu kategoriju. " +
                    "Da li zelite da izbrisete kategoriju?";
                return MessageDialogs.queryConfirmation(String.Format(msg, kategorija), this.Text);
            }
        }

        private bool existsGimnasticar(KategorijaGimnasticara kategorija)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Kategorija", CriteriaOperator.Equal, kategorija));
            return dataContext.GetCount<Gimnasticar>(q) > 0;
        }

        protected override void delete(KategorijaGimnasticara kategorija)
        {
            IList<Gimnasticar> gimnasticari = getGimnasticariByKategorija(kategorija);
            foreach (Gimnasticar g in gimnasticari)
            {
                g.Kategorija = null;
                dataContext.Save(g);
            }
            dataContext.Delete(kategorija);
        }

        private IList<Gimnasticar> getGimnasticariByKategorija(KategorijaGimnasticara kategorija)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Kategorija", CriteriaOperator.Equal, kategorija));
            return dataContext.GetByCriteria<Gimnasticar>(q);
        }

        protected override string deleteErrorMessage()
        {
            return "Neuspesno brisanje kategorije.";
        }

    }
}