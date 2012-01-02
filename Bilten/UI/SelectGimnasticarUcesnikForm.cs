using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Data.QueryModel;
using Bilten.Data;
using Bilten.Exceptions;

namespace Bilten.UI
{
    public partial class SelectGimnasticarUcesnikForm : SelectEntityForm
    {
        private int takmicenjeId;
        private Nullable<Gimnastika> gimnastika;
        private TakmicarskaKategorija kategorija;

        public SelectGimnasticarUcesnikForm(int takmicenjeId, Nullable<Pol> pol,
            TakmicarskaKategorija kategorija)
        {
            InitializeComponent();
            Text = "Izaberi gimnasticara";

            this.takmicenjeId = takmicenjeId;
            this.gimnastika = null;
            if (pol != null)
            {
                if (pol == Pol.Muski)
                    gimnastika = Gimnastika.MSG;
                else if (pol == Pol.Zenski)
                    gimnastika = Gimnastika.ZSG;
            }
            this.kategorija = kategorija;
            initializeGridColumns();

            DataGridViewUserControl.GridColumnHeaderMouseClick += new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);

            FetchModes.Add(new AssociationFetch(
                "Takmicenje", AssociationFetchMode.Eager));
            FetchModes.Add(new AssociationFetch(
                "TakmicarskaKategorija", AssociationFetchMode.Eager));
            FetchModes.Add(new AssociationFetch(
                "KlubUcesnik", AssociationFetchMode.Eager));
            FetchModes.Add(new AssociationFetch(
                "DrzavaUcesnik", AssociationFetchMode.Eager));

            this.ClientSize = new Size(800, 450);

            showAll();
        }

        void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender, GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<GimnasticarUcesnik>(e.DataGridViewCellMouseEventArgs);
        }

        private void initializeGridColumns()
        {
            // TODO: Napravi da budu sinhronizovani ovi podaci sa podacima iz
            // EkipaForm
            dataGridViewUserControl1.AddColumn("Ime", "Ime", 100);
            dataGridViewUserControl1.AddColumn("Prezime", "Prezime", 100);
            dataGridViewUserControl1.AddColumn("Datum rodjenja", "DatumRodjenja", 100, "{0:d}");
            dataGridViewUserControl1.AddColumn("Gimnastika", "Gimnastika", 70);
            dataGridViewUserControl1.AddColumn("Takmicarski broj", "TakmicarskiBroj", 70);
            dataGridViewUserControl1.AddColumn("Kategorija", "TakmicarskaKategorija", 100);
            dataGridViewUserControl1.AddColumn("Klub", "KlubUcesnik", 130);
            dataGridViewUserControl1.AddColumn("Drzava", "DrzavaUcesnik", 100);
        }

        private void showAll()
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();

                IList<GimnasticarUcesnik> gimnasticari = loadAll();
                setEntities(gimnasticari);
                DataGridViewUserControl.sort<GimnasticarUcesnik>(
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

        private IList<GimnasticarUcesnik> loadAll()
        {
            Query q = new Query();
            if (gimnastika != null)
                q.Criteria.Add(new Criterion("Gimnastika", CriteriaOperator.Equal, (byte)gimnastika.Value));
            if (kategorija != null)
                q.Criteria.Add(new Criterion("TakmicarskaKategorija", CriteriaOperator.Equal, kategorija));
            q.Criteria.Add(new Criterion("Takmicenje.Id", CriteriaOperator.Equal, takmicenjeId));
            q.Operator = QueryOperator.And;
            q.OrderClauses.Add(new OrderClause("Prezime", OrderClause.OrderClauseCriteria.Ascending));
            q.OrderClauses.Add(new OrderClause("Ime", OrderClause.OrderClauseCriteria.Ascending));
            foreach (AssociationFetch f in this.FetchModes)
                q.FetchModes.Add(f);
            return dataContext.GetByCriteria<GimnasticarUcesnik>(q);
        }

        protected override void filter(object filterObject)
        {
            GimnasticarUcesnikFilter flt = filterObject as GimnasticarUcesnikFilter;
            if (flt == null)
                return;

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();

                string failureMsg = "Ne postoje gimnasticari koji zadovoljavaju date kriterijume.";
                IList<GimnasticarUcesnik> gimnasticari = findGimnasticariUcesnici(
                    flt.Ime, flt.Prezime, flt.GodRodj, flt.Gimnastika, flt.Drzava,
                    flt.Kategorija, flt.Klub, takmicenjeId);
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

        private IList<GimnasticarUcesnik> findGimnasticariUcesnici(string ime,
            string prezime, Nullable<int> godRodj, Nullable<Gimnastika> gimnastika,
            DrzavaUcesnik drzava, TakmicarskaKategorija kategorija, KlubUcesnik klub,
            int takmicenjeId)
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
                q.Criteria.Add(new Criterion("DrzavaUcesnik", CriteriaOperator.Equal, drzava));
            if (kategorija != null)
                q.Criteria.Add(new Criterion("TakmicarskaKategorija", CriteriaOperator.Equal, kategorija));
            if (klub != null)
                q.Criteria.Add(new Criterion("KlubUcesnik", CriteriaOperator.Equal, klub));
            q.Criteria.Add(new Criterion("Takmicenje.Id", CriteriaOperator.Equal, takmicenjeId));

            q.Operator = QueryOperator.And;
            q.OrderClauses.Add(new OrderClause("Prezime", OrderClause.OrderClauseCriteria.Ascending));
            q.OrderClauses.Add(new OrderClause("Ime", OrderClause.OrderClauseCriteria.Ascending));
            foreach (AssociationFetch f in this.FetchModes)
            {
                q.FetchModes.Add(f);
            }
            return dataContext.GetByCriteria<GimnasticarUcesnik>(q);
        }

        protected override FilterForm createFilterForm()
        {
            // TODO: Ovaj poziv je uredu za prvo prikazivanje FilterForma, zato sto
            // ga inicijalizuje na osnovu stanja u gridu. Medjutim, za sledeca
            // prikazivanja ne mora da bude u redu - npr. ako se nakon prvog 
            // prikazivanja FilterForma u gridu budu neki opstiji podaci, tada
            // ce za drugo prikazivanje FilterForm biti inicijalizovan razlicito
            // od stanja u gridu. Razmisli o varijanti da se FilterForm uvek 
            // inicijalizuje na osnovu stanja u gridu
            return new FilterGimnasticarUcesnikForm(takmicenjeId, gimnastika, kategorija);
        }

        private void SelectGimnasticarUcesnikForm_Load(object sender, EventArgs e)
        {
            dataGridViewUserControl1.clearSelection();
        }
    }
}
