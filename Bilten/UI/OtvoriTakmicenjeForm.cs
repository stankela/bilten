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
    public partial class OtvoriTakmicenjeForm : Form
    {
        private Nullable<int> currTakmicenjeId;
        private List<Takmicenje> takmicenja;
        private IDataContext dataContext;
        bool selectMode;
        int broj;

        private Takmicenje takmicenje;
        public Takmicenje Takmicenje
        {
            get { return takmicenje; }
        }

        private IList<Takmicenje> selTakmicenja;
        public IList<Takmicenje> SelTakmicenja
        {
            get { return selTakmicenja; }
        }

        public OtvoriTakmicenjeForm(Nullable<int> currTakmicenjeId, bool selectMode, int broj)
        {
            InitializeComponent();
            this.currTakmicenjeId = currTakmicenjeId;
            this.selectMode = selectMode;
            this.broj = broj;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                initUI();
                takmicenja = loadTakmicenja();
                setTakmicenja(takmicenja);

                //          dataContext.Commit();
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

        private void initUI()
        {
            if (!selectMode)
            {
                this.Text = "Otvori takmicenje";
                dataGridViewUserControl1.DataGridView.MultiSelect = false;
            }
            else
            {
                this.Text = "Izaberi takmicenje";
                dataGridViewUserControl1.DataGridView.MultiSelect = true;
                btnDelete.Visible = false;
                btnDelete.Enabled = false;
                btnOpen.Text = "OK";
            }

            dataGridViewUserControl1.DataGridView.CellDoubleClick += new DataGridViewCellEventHandler(DataGridView_CellDoubleClick);
            GridColumnsInitializer.initTakmicenje(dataGridViewUserControl1);
        }

        void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                otvoriTakmicenje();
                DialogResult = DialogResult.OK;
            }
        }

        private List<Takmicenje> loadTakmicenja()
        {
            Query q = new Query();
            q.OrderClauses.Add(new OrderClause("Datum", OrderClause.OrderClauseCriteria.Descending));
            return new List<Takmicenje>(
                dataContext.GetByCriteria<Takmicenje>(q));
        }

        private void setTakmicenja(List<Takmicenje> takmicenja)
        {
            dataGridViewUserControl1.setItems<Takmicenje>(takmicenja);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            otvoriTakmicenje();
        }

        private void otvoriTakmicenje()
        {
            if (!selectMode)
            {
                Takmicenje selTakmicenje = dataGridViewUserControl1.getSelectedItem<Takmicenje>();
                if (selTakmicenje != null)
                    takmicenje = selTakmicenje;
                else
                    DialogResult = DialogResult.None;
            }
            else
            {
                IList<Takmicenje> selItems = dataGridViewUserControl1.getSelectedItems<Takmicenje>();
                if (selItems.Count == broj)
                    selTakmicenja = selItems;
                else
                {
                    string msg;
                    if (broj == 1)
                        msg = "Izaberite jedno takmicenje.";
                    else
                        msg = String.Format("Izaberite {0} takmicenja.", broj);
                    MessageDialogs.showMessage(msg, this.Text);
                    DialogResult = DialogResult.None;
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Takmicenje selTakmicenje = dataGridViewUserControl1.getSelectedItem<Takmicenje>();
            if (selTakmicenje == null)
                return;

            string msgFmt = "Da li zelite da izbrisete takmicenje \"{0}\"?";
            if (!MessageDialogs.queryConfirmation(String.Format(
                msgFmt, selTakmicenje), this.Text))
                return;

            if (selTakmicenje.Id == currTakmicenjeId)
            {
                string msg = "Nije dozvoljeno brisanje takmicenja koje je trenutno otvoreno.";
                MessageDialogs.showMessage(msg, this.Text);
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                deleteTakmicenje(selTakmicenje);
    
                dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                Close();
                return;
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }

            takmicenja.Remove(selTakmicenje);
            setTakmicenja(takmicenja);
            if (dataGridViewUserControl1.isSorted())
                dataGridViewUserControl1.refreshItems();
        }

        private void deleteTakmicenje(Takmicenje takmicenje)
        {
            // brisi ocene
            string query = @"from Ocena o
	                       where o.Gimnasticar.Takmicenje.Id = :id";
            IList<Ocena> ocene = dataContext.ExecuteQuery<Ocena>(QueryLanguageType.HQL, query,
                    new string[] { "id" }, new object[] { takmicenje.Id });
            foreach (Ocena o in ocene)
                dataContext.Delete(o);

            // brisi rasporede nastupa
            query = @"select distinct r
                        from RasporedNastupa r
                        join r.Kategorije k
                        where k.Takmicenje.Id = :id";
            IList<RasporedNastupa> rasporediNastupa =
                dataContext.ExecuteQuery<RasporedNastupa>(QueryLanguageType.HQL, query,
                    new string[] { "id" }, new object[] { takmicenje.Id });
            foreach (RasporedNastupa r in rasporediNastupa)
            {
                dataContext.Delete(r);
            }

            // brisi rasporede sudija
            query = @"select distinct r
                        from RasporedSudija r
                        join r.Kategorije k
                        where k.Takmicenje.Id = :id";
            IList<RasporedSudija> rasporediSudija =
                dataContext.ExecuteQuery<RasporedSudija>(QueryLanguageType.HQL, query,
                    new string[] { "id" }, new object[] { takmicenje.Id });
            foreach (RasporedSudija r in rasporediSudija)
                dataContext.Delete(r);

            // brisi sudije ucesnike
            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, takmicenje));
            IList<SudijaUcesnik> sudije = dataContext.GetByCriteria<SudijaUcesnik>(q);
            foreach (SudijaUcesnik s in sudije)
                dataContext.Delete(s);

            // brisi rezultatska takmicenja i ekipe
            q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, takmicenje));
            IList<RezultatskoTakmicenje> rezTakmicenja =
                dataContext.GetByCriteria<RezultatskoTakmicenje>(q);
            foreach (RezultatskoTakmicenje r in rezTakmicenja)
            {
                Takmicenje1 t1 = r.Takmicenje1;
                foreach (Ekipa ek in t1.Ekipe)
                    dataContext.Delete(ek);
                dataContext.Delete(r);
            }

            // brisi gimnasticare ucesnike
            q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, takmicenje));
            IList<GimnasticarUcesnik> gimnasticari =
                dataContext.GetByCriteria<GimnasticarUcesnik>(q);
            foreach (GimnasticarUcesnik g in gimnasticari)
                dataContext.Delete(g);

            // brisi klubove ucesnike
            q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, takmicenje));
            IList<KlubUcesnik> klubovi =
                dataContext.GetByCriteria<KlubUcesnik>(q);
            foreach (KlubUcesnik k in klubovi)
                dataContext.Delete(k);

            // brisi drzave ucesnike
            q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, takmicenje));
            IList<DrzavaUcesnik> drzave =
                dataContext.GetByCriteria<DrzavaUcesnik>(q);
            foreach (DrzavaUcesnik d in drzave)
                dataContext.Delete(d);

            // brisi kategorije
            dataContext.Attach(takmicenje, false);
            foreach (TakmicarskaKategorija k in takmicenje.Kategorije)
                dataContext.Delete(k);

            // brisi descriptions
            foreach (RezultatskoTakmicenjeDescription d in takmicenje.TakmicenjeDescriptions)
                dataContext.Delete(d);

            // brisi takmicenje
            dataContext.Delete(takmicenje);
        }

    }
}