using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Exceptions;
using Bilten.Data;
using Bilten.Data.QueryModel;

namespace Bilten.UI
{
    public partial class GimnasticariForm : SingleEntityListForm<Gimnasticar>
    {
        private StatusBar statusBar;
        
        public GimnasticariForm()
        {
            this.Text = "Gimnasticari";
            
            statusBar = new StatusBar();
            statusBar.Parent = this;
            statusBar.ShowPanels = true;
            StatusBarPanel sbPanel1 = new StatusBarPanel();
            statusBar.Panels.Add(sbPanel1);

            dataGridViewUserControl1.GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            InitializeGridColumns();

            // potrebno za filtriranje
            FetchModes.Add(new AssociationFetch(
               "Kategorija", AssociationFetchMode.Eager));
            FetchModes.Add(new AssociationFetch(
                "Klub", AssociationFetchMode.Eager));
            FetchModes.Add(new AssociationFetch(
                "Drzava", AssociationFetchMode.Eager)); 
            
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                IList<Gimnasticar> gimnasticari = loadAll();
                SetItems(gimnasticari);
                dataGridViewUserControl1.sort<Gimnasticar>(
                    new string[] { "Prezime", "Ime" },
                    new ListSortDirection[] { ListSortDirection.Ascending, ListSortDirection.Ascending });
                updateGimnasticariCount();
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

        private void updateGimnasticariCount()
        {
            int count = dataGridViewUserControl1.getItems<Gimnasticar>().Count;
            if (count == 1)
                statusBar.Panels[0].Text = count.ToString() + " gimnasticar";
            else
                statusBar.Panels[0].Text = count.ToString() + " gimnasticara";
        }

        protected override void prikaziSve()
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                IList<Gimnasticar> gimnasticari = loadAll();
                SetItems(gimnasticari);
                dataGridViewUserControl1.sort<Gimnasticar>(
                    new string[] { "Prezime", "Ime" },
                    new ListSortDirection[] { ListSortDirection.Ascending, ListSortDirection.Ascending });
                updateGimnasticariCount();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        private IList<Gimnasticar> loadAll()
        {
            string query = @"from Gimnasticar g
                left join fetch g.Kategorija
                left join fetch g.Klub
                left join fetch g.Drzava";
            IList<Gimnasticar> result = dataContext.
                ExecuteQuery<Gimnasticar>(QueryLanguageType.HQL, query,
                        new string[] { }, new object[] { });
            return result;
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<Gimnasticar>(e.DataGridViewCellMouseEventArgs);
        }

        private void InitializeGridColumns()
        {
            AddColumn("Ime", "ImeSrednjeIme", 100);
            AddColumn("Prezime", "Prezime", 100);
            AddColumn("Datum rodjenja", "DatumRodjenja", 100, "{0:d}");
            AddColumn("Gimnastika", "Gimnastika", 70);
            AddColumn("Drzava", "Drzava", 100);
            AddColumn("Registarski broj", "RegistarskiBroj", 100);
            AddColumn("Poslednja registr.", "DatumPoslednjeRegistracije", 100, "{0:d}");
            AddColumn("Kategorija", "Kategorija", 100);
            AddColumn("Klub", "Klub", 100);
        }

        protected override EntityDetailForm createEntityDetailForm(Nullable<int> entityId)
        {
            return new GimnasticarForm(entityId);
        }

        protected override string deleteConfirmationMessage(Gimnasticar gimnasticar)
        {
            return String.Format("Da li zelite da izbrisete gimnasticara \"{0}\"?", gimnasticar);
        }

        protected override string deleteErrorMessage()
        {
            return "Neuspesno brisanje gimnasticara.";
        }

        protected override void onApplyFilter()
        {
            if (filterForm == null || filterForm.IsDisposed)
            {
                // NOTE: IsDisposed je true kada se form zatvori (bilo pritiskom na X
                // ili pozivom Close)

                try
                {
                    filterForm = new FilterGimnasticarForm(null); // can throw
                    filterForm.Filter += new EventHandler(filterForm_Filter);
                    filterForm.Show();
                }
                catch (InfrastructureException ex)
                {
                    MessageDialogs.showError(ex.Message, this.Text);
                }
            }
            else
            {
                filterForm.Activate();
            }
        }

        private void filterForm_Filter(object sender, EventArgs e)
        {
            object filterObject = filterForm.FilterObject;
            if (filterObject != null)
                filter(filterObject);
        }

        private void filter(object filterObject)
        {
            GimnasticarFilter flt = filterObject as GimnasticarFilter;
            if (flt == null)
                return;

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();

                // biranje gimnasticara sa prethodnog takmicenja
                //Takmicenje takmicenje = dataContext.GetById<Takmicenje>(5);
                //gimnasticari = dataContext.ExecuteNamedQuery<Gimnasticar>(
                //    "FindGimnasticariByTakmicenje",
                //    new string[] { "takmicenje" }, new object[] { takmicenje });

                IList<Gimnasticar> gimnasticari;
                string failureMsg = "";
                if (flt.RegBroj != null)
                {
                    gimnasticari = findGimnasticari(flt.RegBroj);
                    if (gimnasticari.Count == 0)
                        failureMsg = "Ne postoji gimnasticar sa datim registarskim brojem.";
                }
                else
                {
                    gimnasticari = findGimnasticari(flt.Ime, flt.Prezime,
                        flt.GodRodj, flt.Gimnastika, flt.Drzava, flt.Kategorija,
                        flt.Klub);
                    if (gimnasticari.Count == 0)
                        failureMsg = "Ne postoje gimnasticari koji zadovoljavaju date kriterijume.";
                }
                SetItems(gimnasticari);
                updateGimnasticariCount();
                if (gimnasticari.Count == 0)
                    MessageDialogs.showMessage(failureMsg, this.Text);
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        private IList<Gimnasticar> findGimnasticari(RegistarskiBroj regBroj)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("RegistarskiBroj", CriteriaOperator.Equal, regBroj));
            foreach (AssociationFetch f in this.FetchModes)
            {
                q.FetchModes.Add(f);
            }
            return dataContext.GetByCriteria<Gimnasticar>(q);
        }

        private IList<Gimnasticar> findGimnasticari(string ime, string prezime,
            Nullable<int> godRodj, Nullable<Gimnastika> gimnastika, Drzava drzava,
            KategorijaGimnasticara kategorija, Klub klub)
        {
            Query q = new Query();
            if (!String.IsNullOrEmpty(ime))
                q.Criteria.Add(new Criterion("Ime", CriteriaOperator.Like, ime, StringMatchMode.Start, true));
            if (!String.IsNullOrEmpty(prezime))
                q.Criteria.Add(new Criterion("Prezime", CriteriaOperator.Like, prezime, StringMatchMode.Start, true));
            if (godRodj != null)
            {
                q.Criteria.Add(new Criterion("DatumRodjenja.Godina",
                    CriteriaOperator.Equal, (short)godRodj.Value));
            }
            if (gimnastika != null)
                q.Criteria.Add(new Criterion("Gimnastika", CriteriaOperator.Equal, (byte)gimnastika.Value));
            if (drzava != null)
                q.Criteria.Add(new Criterion("Drzava", CriteriaOperator.Equal, drzava));
            if (kategorija != null)
                q.Criteria.Add(new Criterion("Kategorija", CriteriaOperator.Equal, kategorija));
            if (klub != null)
                q.Criteria.Add(new Criterion("Klub", CriteriaOperator.Equal, klub));

            q.Operator = QueryOperator.And;
            q.OrderClauses.Add(new OrderClause("Prezime", OrderClause.OrderClauseCriteria.Ascending));
            q.OrderClauses.Add(new OrderClause("Ime", OrderClause.OrderClauseCriteria.Ascending));
            foreach (AssociationFetch f in this.FetchModes)
            {
                q.FetchModes.Add(f);
            }
            return dataContext.GetByCriteria<Gimnasticar>(q);
        }

        protected override void AddNew()
        {
            try
            {
                GimnasticarForm form = (GimnasticarForm)createEntityDetailForm(null);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.GimnasticarToEdit == null)
                    {
                        Gimnasticar newEntity = (Gimnasticar)form.Entity;
                        List<Gimnasticar> items = dataGridViewUserControl1.getItems<Gimnasticar>();
                        items.Add(newEntity);
                        dataGridViewUserControl1.setItems<Gimnasticar>(items);
                        dataGridViewUserControl1.setSelectedItem<Gimnasticar>(newEntity);
                        updateGimnasticariCount();
                    }
                    else
                    {
                        dataGridViewUserControl1.setSelectedItem<Gimnasticar>(form.GimnasticarToEdit);
                        Edit();
                    }
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }

        protected override void updateEntityCount()
        {
            updateGimnasticariCount();
        }

    }
}
