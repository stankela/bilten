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
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class DrzaveForm : SingleEntityListForm<Drzava>
    {
        public DrzaveForm()
        {
            this.Text = "Drzave";
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
                    IList<Drzava> drzave = DAOFactoryFactory.DAOFactory.GetDrzavaDAO().FindAll();
                    SetItems(drzave);
                    dataGridViewUserControl1.sort<Drzava>(
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
                dgwuc.onColumnHeaderMouseClick<Drzava>(e.DataGridViewCellMouseEventArgs);
        }

        private void InitializeGridColumns()
        {
            AddColumn("Naziv drzave", "Naziv", 100);
            AddColumn("Skraceni kod", "Kod", 100);
        }

        protected override EntityDetailForm createEntityDetailForm(Nullable<int> entityId)
        {
            return new DrzavaForm(entityId);
        }

        protected override string deleteConfirmationMessage(Drzava drzava)
        {
            return String.Format("Da li zelite da izbrisete drzavu \"{0}\"?", drzava);
        }

        protected override bool refIntegrityDeleteDlg(Drzava drzava)
        {
            bool existsGimnasticari = DAOFactoryFactory.DAOFactory.GetGimnasticarDAO().existsGimnasticar(drzava);
            bool existsSudije = DAOFactoryFactory.DAOFactory.GetSudijaDAO().existsSudija(drzava);
            if (!existsGimnasticari && !existsSudije)
                return true;
            else
            {
                // Posto je drzava obavezna i za gimnasticare i za sudije, nije moguce brisanje drzave za koju postoje
                // gimnasticari i/ili sudije.
                String s = String.Empty;
                if (existsGimnasticari)
                    s = "gimnasticari";
                if (existsSudije)
                {
                    if (existsGimnasticari)
                        s += " i ";
                    s += "sudije";
                }
                string msg = "Nije moguce izbrisati drzavu zato sto postoje {1} iz drzave '{0}'. Ako " +
                    "zelite da izbrisete drzavu, morate najpre da izbrisete sve gimnasticare i sudije iz te drzave.";
                MessageDialogs.showMessage(String.Format(msg, drzava, s), this.Text);
                return false;
            }
        }

        protected override void delete(Drzava drzava)
        {
            GimnasticarDAO gimnasticarDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarDAO();
            IList<Gimnasticar> gimnasticari = gimnasticarDAO.FindGimnasticariByDrzava(drzava);
            SudijaDAO sudijaDAO = DAOFactoryFactory.DAOFactory.GetSudijaDAO();
            IList<Sudija> sudije = sudijaDAO.FindSudijeByDrzava(drzava);
            foreach (Gimnasticar g in gimnasticari)
            {
                g.Drzava = null;
                gimnasticarDAO.MakePersistent(g);
            }
            foreach (Sudija s in sudije)
            {
                s.Drzava = null;
                sudijaDAO.MakePersistent(s);
            }
            DAOFactoryFactory.DAOFactory.GetDrzavaDAO().MakeTransient(drzava);
        }

        protected override string deleteErrorMessage()
        {
            return "Neuspesno brisanje drzave.";
        }

        protected override void updateEntityCount()
        {
            int count = dataGridViewUserControl1.getItems<Drzava>().Count;
            StatusPanel.Panels[0].Text = count.ToString() + " drzava";
        }
    }
}