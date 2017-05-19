using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class SelectSudijaUcesnikForm : SelectEntityForm
    {
        public SelectSudijaUcesnikForm(int takmicenjeId)
        {
            InitializeComponent();
            Text = "Izaberi sudiju";
            this.ClientSize = new System.Drawing.Size(800, 540);
            initializeGridColumns();

            DataGridViewUserControl.GridColumnHeaderMouseClick += new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<SudijaUcesnik> sudije
                        = DAOFactoryFactory.DAOFactory.GetSudijaUcesnikDAO().FindByTakmicenjeFetchKlubDrzava(takmicenjeId);
                    setEntities(sudije);
                    DataGridViewUserControl.sort<SudijaUcesnik>(
                        new string[] { "Prezime", "Ime" },
                        new ListSortDirection[] { ListSortDirection.Ascending, ListSortDirection.Ascending });
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

        void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender, GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<SudijaUcesnik>(e.DataGridViewCellMouseEventArgs);
        }

        private void initializeGridColumns()
        {
            // TODO: Sinhronizuj sa SudijeForm
            dataGridViewUserControl1.AddColumn("Ime", "Ime", 100);
            dataGridViewUserControl1.AddColumn("Prezime", "Prezime", 100);
            dataGridViewUserControl1.AddColumn("Pol", "Pol", 100);
            dataGridViewUserControl1.AddColumn("Klub", "KlubUcesnik", 150);
            dataGridViewUserControl1.AddColumn("Drzava", "DrzavaUcesnik", 100);
        }
    }
}