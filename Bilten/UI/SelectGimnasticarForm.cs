using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Data;
using Bilten.Data.QueryModel;
using Bilten.Exceptions;

namespace Bilten.UI
{
    public partial class SelectGimnasticarForm : SelectEntityForm
    {
        private Nullable<Gimnastika> gimnastika;

        public SelectGimnasticarForm(Nullable<Gimnastika> gimnastika)
        {
            InitializeComponent();
            Text = "Izaberi gimnasticara";
            this.ClientSize = new Size(ClientSize.Width, 500);
            this.gimnastika = gimnastika;
            initializeGridColumns();

            DataGridViewUserControl.GridColumnHeaderMouseClick += new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            
            FetchModes.Add(new AssociationFetch(
                "Kategorija", AssociationFetchMode.Eager));
            FetchModes.Add(new AssociationFetch(
                "Klub", AssociationFetchMode.Eager));
            FetchModes.Add(new AssociationFetch(
                "Drzava", AssociationFetchMode.Eager));

            showAll();
        }

        void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender, GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<Gimnasticar>(e.DataGridViewCellMouseEventArgs);
        }

        private void initializeGridColumns()
        {
            // TODO: Napravi da budu sinhronizovani ovi podaci sa podacima iz
            // GimnasticariForm
            dataGridViewUserControl1.AddColumn("Ime", "Ime", 100);
            dataGridViewUserControl1.AddColumn("Prezime", "Prezime", 100);
            dataGridViewUserControl1.AddColumn("Datum rodjenja", "DatumRodjenja", 100, "{0:d}");
            dataGridViewUserControl1.AddColumn("Gimnastika", "Gimnastika", 70);
            dataGridViewUserControl1.AddColumn("Drzava", "Drzava", 100);
            dataGridViewUserControl1.AddColumn("Registarski broj", "RegistarskiBroj", 70);
            dataGridViewUserControl1.AddColumn("Kategorija", "Kategorija", 100);
            dataGridViewUserControl1.AddColumn("Klub", "Klub", 100);
        }

        private void showAll()
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();

                IList<Gimnasticar> gimnasticari = loadAll();
                setEntities(gimnasticari);
                DataGridViewUserControl.sort<Gimnasticar>(
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

        private IList<Gimnasticar> loadAll()
        {
            Query q = new Query();
            if (gimnastika != null)
                q.Criteria.Add(new Criterion("Gimnastika", CriteriaOperator.Equal, (byte)gimnastika.Value));
            q.OrderClauses.Add(new OrderClause("Prezime", OrderClause.OrderClauseCriteria.Ascending));
            q.OrderClauses.Add(new OrderClause("Ime", OrderClause.OrderClauseCriteria.Ascending));
            foreach (AssociationFetch f in this.FetchModes)
                q.FetchModes.Add(f);
            return dataContext.GetByCriteria<Gimnasticar>(q);
        }

        protected override void filter(object filterObject)
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
                setEntities(gimnasticari);
                if (gimnasticari.Count == 0)
                    MessageDialogs.showMessage(failureMsg, this.Text);
                dataGridViewUserControl1.clearSelection();
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

        protected override FilterForm createFilterForm()
        {
            return new FilterGimnasticarForm(gimnastika);
        }

        private void SelectGimnasticarForm_Load(object sender, EventArgs e)
        {
            dataGridViewUserControl1.clearSelection();
        }
    }
}
