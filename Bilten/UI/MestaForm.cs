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
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class MestaForm : SingleEntityListForm<Mesto>
    {
        public MestaForm()
        {
            this.Text = "Mesta";
            dataGridViewUserControl1.GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            InitializeGridColumns();

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<Mesto> mesta = DAOFactoryFactory.DAOFactory.GetMestoDAO().FindAll();
                    SetItems(mesta);
                    dataGridViewUserControl1.sort<Mesto>(
                        new string[] { "Naziv" },
                        new ListSortDirection[] { ListSortDirection.Ascending });
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
            }
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<Mesto>(e.DataGridViewCellMouseEventArgs);
        }

        private void InitializeGridColumns()
        {
            AddColumn("Naziv mesta", "Naziv", 100);
        }

        protected override EntityDetailForm createEntityDetailForm(Nullable<int> entityId)
        {
            return new MestoForm(entityId);
        }

        protected override string deleteConfirmationMessage(Mesto m)
        {
            return String.Format("Da li zelite da izbrisete mesto \"{0}\"?", m);
        }

        protected override bool refIntegrityDeleteDlg(Mesto m)
        {
            if (!DAOFactoryFactory.DAOFactory.GetKlubDAO().existsKlub(m))
                return true;
            else
            {
                string msg = "Mesto '{0}' nije moguce izbrisati zato sto postoje " +
                    "klubovi iz datog mesta. Ako zelite da izbrisete mesto, morate " +
                    "najpre da izbrisete sve klubove iz datog mesta. ";
                MessageDialogs.showMessage(String.Format(msg, m), this.Text);
                return false;
            }
        }

        protected override void delete(Mesto m)
        {
            DAOFactoryFactory.DAOFactory.GetMestoDAO().Delete(m);
        }

        protected override string deleteErrorMessage()
        {
            return "Neuspesno brisanje mesta.";
        }

        protected override void updateEntityCount()
        {
            int count = dataGridViewUserControl1.getItems<Mesto>().Count;
            StatusPanel.Panels[0].Text = count.ToString() + " mesta";
        }
    }
}