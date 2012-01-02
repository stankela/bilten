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
            // NOTE: Kada form nasledjuje drugi form koji ima genericki parametar,
            // dizajner nece da ga prikaze. Zato sam izbacio fajl
            // GimnasticariForm.Designer.cs (jer je nepotreban) i poziv
            // InitializeComponent(). Ukoliko form treba da dodaje neke kontrole
            // (osim onih koje je nasledio), to treba da se radi programski.

            this.Text = "Kategorije gimnasticara";
            dataGridViewUserControl1.GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            InitializeGridColumns();

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                IList<KategorijaGimnasticara> kategorije = loadAll();
                SetItems(kategorije);
                dataGridViewUserControl1.sort<KategorijaGimnasticara>(
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

        private IList<KategorijaGimnasticara> loadAll()
        {
            string query = @"from KategorijaGimnasticara";
            IList<KategorijaGimnasticara> result = dataContext.
                ExecuteQuery<KategorijaGimnasticara>(QueryLanguageType.HQL, query,
                        new string[] { }, new object[] { });
            return result;
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<KategorijaGimnasticara>(e.DataGridViewCellMouseEventArgs);
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