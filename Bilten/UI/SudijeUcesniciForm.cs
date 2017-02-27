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
using System.Data.SqlServerCe;
using Bilten.Dao;
using NHibernate;
using NHibernate.Context;

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

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);

                    IList<SudijaUcesnik> sudije
                        = DAOFactoryFactory.DAOFactory.GetSudijaUcesnikDAO().FindForTakmicenje(takmicenjeId);
                    SetItems(sudije);
                    dataGridViewUserControl1.sort<SudijaUcesnik>(
                        new string[] { "Prezime", "Ime" },
                        new ListSortDirection[] { ListSortDirection.Ascending, ListSortDirection.Ascending });
                    updateEntityCount();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }
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

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    List<SudijaUcesnik> okSudije = new List<SudijaUcesnik>();
                    List<Sudija> illegalSudije = new List<Sudija>();
                    foreach (Sudija s in form.SelectedEntities)
                    {
                        if (canAddSudija(s, takmicenje))
                        {
                            SudijaUcesnik sudija = createSudijaUcesnik(s, takmicenje);
                            DAOFactoryFactory.DAOFactory.GetSudijaUcesnikDAO().Add(sudija);
                            okSudije.Add(sudija);
                        }
                        else
                        {
                            illegalSudije.Add(s);
                        }

                    }
                    session.Transaction.Commit();

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
                    updateEntityCount();

                    if (illegalSudije.Count > 0)
                    {
                        string msg = "Sledece sudije vec postoje na listi: \n\n";
                        msg += StringUtil.getListString(illegalSudije.ToArray());
                        MessageDialogs.showMessage(msg, this.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private bool canAddSudija(Sudija s, Takmicenje takmicenje)
        {
            // TODO: Add business rules
            return !DAOFactoryFactory.DAOFactory.GetSudijaUcesnikDAO().existsSudijaUcesnik(s, takmicenje);
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
                DrzavaUcesnikDAO drzavaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO();
                DrzavaUcesnik drzavaUcesnik = drzavaUcesnikDAO.FindDrzavaUcesnik(
                    takmicenje.Id, s.Drzava.Naziv);
                if (drzavaUcesnik == null)
                {
                    drzavaUcesnik = new DrzavaUcesnik();
                    drzavaUcesnik.Naziv = s.Drzava.Naziv;
                    drzavaUcesnik.Kod = s.Drzava.Kod;
                    drzavaUcesnik.Takmicenje = takmicenje;
                    drzavaUcesnikDAO.Add(drzavaUcesnik);
                }
                result.DrzavaUcesnik = drzavaUcesnik;
            }
            if (s.Klub == null)
                result.KlubUcesnik = null;
            else
            {
                KlubUcesnikDAO klubUcesnikDAO = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO();
                KlubUcesnik klubUcesnik = klubUcesnikDAO.FindKlubUcesnik(
                    takmicenje.Id, s.Klub.Naziv);
                if (klubUcesnik == null)
                {
                    klubUcesnik = new KlubUcesnik();
                    klubUcesnik.Naziv = s.Klub.Naziv;
                    klubUcesnik.Kod = s.Klub.Kod;
                    klubUcesnik.Takmicenje = takmicenje;
                    klubUcesnikDAO.Add(klubUcesnik);
                }
                result.KlubUcesnik = klubUcesnik;
            }
            return result;
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
                string msg = "Da biste izbrisali sudiju morate najpre da ga izbrisete iz rasporeda sudija na spravi.";
                MessageDialogs.showMessage(msg, this.Text);
                return false;
            }
        }

        protected override void delete(SudijaUcesnik s)
        {
            DAOFactoryFactory.DAOFactory.GetSudijaUcesnikDAO().Delete(s);
        }

        private bool sudiNaSpravi(SudijaUcesnik s)
        {
            // TODO: Probaj da uradis ovo u NHibernate.
            try
            {
                // can throw InfrastructureException
                string findSQL =
                    "SELECT * FROM sudija_na_spravi " +
                    "WHERE sudija_id = @sudija_id";

                SqlCeCommand cmd = new SqlCeCommand(findSQL);
                cmd.Parameters.Add("@sudija_id", SqlDbType.Int).Value = s.Id;

                SqlCeDataReader rdr = Database.executeReader(cmd, Strings.DatabaseAccessExceptionMessage);
                bool result = false;
                if (rdr.Read())
                    result = true;
                rdr.Close();
                return result;
            }
            catch (Exception ex)
            {
                // TODO: Izgleda da se ovaj izuzetak nigde ne hendluje.
                throw new InfrastructureException(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        protected override string deleteErrorMessage()
        {
            return "Neuspesno brisanje sudije.";
        }

        protected override void updateEntityCount()
        {
            int count = dataGridViewUserControl1.getItems<SudijaUcesnik>().Count;
            StatusPanel.Panels[0].Text = count.ToString() + " sudija";
        }
    }
}