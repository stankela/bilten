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
    public partial class KluboviForm : SingleEntityListForm<Klub>
    {
        public KluboviForm()
        {
            this.Text = "Klubovi";
            pnlFilter.Visible = false;
            btnRefresh.Visible = false;
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
                    IList<Klub> klubovi = DAOFactoryFactory.DAOFactory.GetKlubDAO().FindAllFetchMesto();
                    SetItems(klubovi);
                    dataGridViewUserControl1.sort<Klub>(
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
                dgwuc.onColumnHeaderMouseClick<Klub>(e.DataGridViewCellMouseEventArgs);
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

        protected override string deleteConfirmationMessage(Klub klub)
        {
            return String.Format("Da li zelite da izbrisete klub \"{0}\"?", klub);
        }

        protected override bool refIntegrityDeleteDlg(Klub klub)
        {
            if (!DAOFactoryFactory.DAOFactory.GetGimnasticarDAO().existsGimnasticar(klub))
                return true;
            else
            {
                string msg = "Postoje gimnasticari koji su clanovi kluba '{0}'. Ako " +
                    "ga izbrisete, ovi gimnasticari nece imati naveden klub. " +
                    "Da li zelite da izbrisete klub?";
                return MessageDialogs.queryConfirmation(String.Format(msg, klub), this.Text);
            }
        }

        protected override void delete(Klub klub)
        {
            GimnasticarDAO gimnasticarDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarDAO();
            IList<Gimnasticar> gimnasticari = gimnasticarDAO.FindGimnasticariByKlub(klub);
            foreach (Gimnasticar g in gimnasticari)
            {
                g.Klub = null;
                gimnasticarDAO.Update(g);
            }
            DAOFactoryFactory.DAOFactory.GetKlubDAO().Delete(klub);
        }

        protected override string deleteErrorMessage()
        {
            return "Neuspesno brisanje kluba.";
        }

        protected override void updateEntityCount()
        {
            int count = dataGridViewUserControl1.getItems<Klub>().Count;
            StatusPanel.Panels[0].Text = count.ToString() + " klub";
        }
    }
}