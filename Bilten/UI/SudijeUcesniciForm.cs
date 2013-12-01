using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Data.QueryModel;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;

namespace Bilten.UI
{
    public partial class SudijeUcesniciForm : SingleEntityListForm<SudijaUcesnik>
    {
        private Takmicenje takmicenje;

        public SudijeUcesniciForm(int takmicenjeId)
        {
            this.Text = "Sudije na takmicenju";
            this.ClientSize = new System.Drawing.Size(800, 540);
            btnEditItem.Enabled = false;
            dataGridViewUserControl1.GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            InitializeGridColumns();

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                takmicenje = dataContext.GetById<Takmicenje>(takmicenjeId);

                IList<SudijaUcesnik> sudije = loadAll(takmicenjeId);
                SetItems(sudije);
                dataGridViewUserControl1.sort<SudijaUcesnik>(
                    new string[] { "Prezime", "Ime" },
                    new ListSortDirection[] { ListSortDirection.Ascending, ListSortDirection.Ascending });
                updateSudijeCount();
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

        private IList<SudijaUcesnik> loadAll(int takmicenjeId)
        {
            string query = @"from SudijaUcesnik s
                left join fetch s.DrzavaUcesnik
                left join fetch s.KlubUcesnik
                where s.Takmicenje.Id = :takmicenjeId";
            IList<SudijaUcesnik> result = dataContext.
                ExecuteQuery<SudijaUcesnik>(QueryLanguageType.HQL, query,
                        new string[] { "takmicenjeId" }, new object[] { takmicenjeId });
            return result;
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<SudijaUcesnik>(e.DataGridViewCellMouseEventArgs);
        }

        private void InitializeGridColumns()
        {
            AddColumn("Ime", "Ime", 100);
            AddColumn("Prezime", "Prezime", 100);
            AddColumn("Pol", "Pol", 100);
            AddColumn("Klub", "KlubUcesnik", 150);
            AddColumn("Drzava", "DrzavaUcesnik", 100);
        }

        protected override void AddNew()
        {
            DialogResult dlgResult = DialogResult.None;
            SelectSudijaForm form = null;
            try
            {
                form = new SelectSudijaForm();
                dlgResult = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (dlgResult != DialogResult.OK || form.SelectedEntities.Count == 0)
                return;

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                List<SudijaUcesnik> okSudije = new List<SudijaUcesnik>();
                List<Sudija> illegalSudije = new List<Sudija>();
                foreach (Sudija s in form.SelectedEntities)
                {
                    if (canAddSudija(s, takmicenje))
                    {
                        SudijaUcesnik sudija = createSudijaUcesnik(s, takmicenje);
                        dataContext.Add(sudija);
                        okSudije.Add(sudija);
                    }
                    else
                    {
                        illegalSudije.Add(s);
                    }

                }
                dataContext.Commit();

                if (okSudije.Count > 0)
                {
                    IList<SudijaUcesnik> sudije =
                         dataGridViewUserControl1.DataGridView.DataSource as IList<SudijaUcesnik>;
                    foreach (SudijaUcesnik s in okSudije)
                    {
                        sudije.Add(s);
                    }

                    CurrencyManager currencyManager =
                        (CurrencyManager)this.BindingContext[dataGridViewUserControl1.DataGridView.DataSource];
                    currencyManager.Position = sudije.Count - 1;
                    currencyManager.Refresh();
                }
                updateSudijeCount();

                if (illegalSudije.Count > 0)
                {
                    string msg = "Sledece sudije vec postoje na listi: \n\n";
                    msg += StringUtil.getListString(illegalSudije.ToArray());
                    MessageDialogs.showMessage(msg, this.Text);
                }
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

        private bool canAddSudija(Sudija s, Takmicenje takmicenje)
        {
            // TODO: Add business rules
            return !existsSudijaUcesnik(s, takmicenje);
        }

        private bool existsSudijaUcesnik(Sudija s, Takmicenje takmicenje)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Ime", CriteriaOperator.Equal, s.Ime));
            q.Criteria.Add(new Criterion("Prezime", CriteriaOperator.Equal, s.Prezime));
            q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, takmicenje));
            return dataContext.GetCount<SudijaUcesnik>(q) > 0;
        }

        private SudijaUcesnik createSudijaUcesnik(Sudija s, Takmicenje takmicenje)
        {
            SudijaUcesnik result = new SudijaUcesnik();
            result.Ime = s.Ime;
            result.Prezime = s.Prezime;
            result.Pol = s.Pol;
            result.UlogaUGlavnomSudijskomOdboru = SudijskaUloga.Undefined;
            result.Takmicenje = takmicenje;
            if (s.Drzava == null)
                result.DrzavaUcesnik = null;
            else
            {
                DrzavaUcesnik drzavaUcesnik = findDrzavaUcesnik(
                    takmicenje.Id, s.Drzava.Naziv);
                if (drzavaUcesnik == null)
                {
                    drzavaUcesnik = new DrzavaUcesnik();
                    drzavaUcesnik.Naziv = s.Drzava.Naziv;
                    drzavaUcesnik.Kod = s.Drzava.Kod;
                    drzavaUcesnik.Takmicenje = takmicenje;
                    dataContext.Add(drzavaUcesnik);
                }
                result.DrzavaUcesnik = drzavaUcesnik;
            }
            if (s.Klub == null)
                result.KlubUcesnik = null;
            else
            {
                KlubUcesnik klubUcesnik = findKlubUcesnik(
                    takmicenje.Id, s.Klub.Naziv);
                if (klubUcesnik == null)
                {
                    klubUcesnik = new KlubUcesnik();
                    klubUcesnik.Naziv = s.Klub.Naziv;
                    klubUcesnik.Kod = s.Klub.Kod;
                    klubUcesnik.Takmicenje = takmicenje;
                    dataContext.Add(klubUcesnik);
                }
                result.KlubUcesnik = klubUcesnik;
            }
            return result;
        }

        private DrzavaUcesnik findDrzavaUcesnik(int takmicenjeId, string naziv)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje.Id", CriteriaOperator.Equal, takmicenjeId));
            q.Criteria.Add(new Criterion("Naziv", CriteriaOperator.Equal, naziv));
            q.Operator = QueryOperator.And;
            IList<DrzavaUcesnik> result = dataContext.GetByCriteria<DrzavaUcesnik>(q);
            if (result.Count == 0)
                return null;
            else
                return result[0];
        }

        private KlubUcesnik findKlubUcesnik(int takmicenjeId, string naziv)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje.Id", CriteriaOperator.Equal, takmicenjeId));
            q.Criteria.Add(new Criterion("Naziv", CriteriaOperator.Equal, naziv));
            q.Operator = QueryOperator.And;
            IList<KlubUcesnik> result = dataContext.GetByCriteria<KlubUcesnik>(q);
            if (result.Count == 0)
                return null;
            else
                return result[0];
        }

        protected override string deleteConfirmationMessage(SudijaUcesnik sudija)
        {
            return String.Format("Da li zelite da izbrisete sudiju \"{0}\"?", sudija);
        }

        protected override bool refIntegrityDeleteDlg(SudijaUcesnik s)
        {
            if (!sudiNaSpravi(s))
                return true;
            else
            {
                // TODO: Eventualno navedi na kojim spravama, delovima takmicenja
                // i kategorijama sudi dati sudija
                string msg = "Sudiju '{0}' nije moguce izbrisati zato sto je " +
                    "odredjen da sudi na spravi. Ako zelite da izbrisete sudiju, morate " +
                    "najpre da ga uklonite iz sudijskog odbora sprave. ";
                MessageDialogs.showMessage(String.Format(msg, s), this.Text);
                return false;
            }
        }

        private bool sudiNaSpravi(SudijaUcesnik s)
        {
            // TODO
            return false;    
        }

        protected override string deleteErrorMessage()
        {
            return "Neuspesno brisanje sudije.";
        }

        protected override void updateEntityCount()
        {
            updateSudijeCount();
        }

        private void updateSudijeCount()
        {
            int count = dataGridViewUserControl1.getItems<SudijaUcesnik>().Count;
            StatusPanel.Panels[0].Text = count.ToString() + " sudija";
        }

    }
}