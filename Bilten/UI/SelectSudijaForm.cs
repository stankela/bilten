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
    public partial class SelectSudijaForm : SelectEntityForm
    {
        private FilterSudijaUserControl filterSudijaUserControl1;

        public SelectSudijaForm()
        {
            InitializeComponent();
            Text = "Izaberi sudiju";

            filterSudijaUserControl1 = new FilterSudijaUserControl();
            this.pnlFilter.SuspendLayout();
            this.pnlFilter.Controls.Add(filterSudijaUserControl1);
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.Height = filterSudijaUserControl1.Height + 10;
            filterSudijaUserControl1.initialize();
            filterSudijaUserControl1.Filter += filterSudijaUserControl1_Filter;

            this.ClientSize = new Size(filterSudijaUserControl1.Width + 20, 540);
            initializeGridColumns();
            DataGridViewUserControl.GridColumnHeaderMouseClick += new EventHandler<GridColumnHeaderMouseClickEventArgs>(
                DataGridViewUserControl_GridColumnHeaderMouseClick);

            showAll();
        }

        private void showAll()
        {
            filterSudijaUserControl1.Filter -= filterSudijaUserControl1_Filter;
            filterSudijaUserControl1.resetFilter();
            filterSudijaUserControl1.Filter += filterSudijaUserControl1_Filter;
            
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<Sudija> sudije = DAOFactoryFactory.DAOFactory.GetSudijaDAO().FindAll();
                    setEntities(sudije);
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
                dgwuc.onColumnHeaderMouseClick<Sudija>(e.DataGridViewCellMouseEventArgs);
        }

        private void initializeGridColumns()
        {
            // TODO: Sinhronizuj sa SudijeForm
            dataGridViewUserControl1.AddColumn("Ime", "Ime", 100);
            dataGridViewUserControl1.AddColumn("Prezime", "Prezime", 100);
            dataGridViewUserControl1.AddColumn("Pol", "Pol", 100);
            dataGridViewUserControl1.AddColumn("Klub", "Klub", 150);
            dataGridViewUserControl1.AddColumn("Drzava", "Drzava", 100);
        }

        private void filterSudijaUserControl1_Filter(object sender, EventArgs e)
        {
            SudijaFilter flt = filterSudijaUserControl1.getFilter();
            if (flt != null)
                filter(flt);
        }

        private void filter(SudijaFilter flt)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    IList<Sudija> sudije;
                    if (flt.isEmpty())
                        sudije = DAOFactoryFactory.DAOFactory.GetSudijaDAO().FindAll();
                    else
                    {
                        sudije = DAOFactoryFactory.DAOFactory.GetSudijaDAO().FindSudije(
                            flt.Ime, flt.Prezime, flt.Pol, flt.Drzava, flt.Klub);
                    }
                    setEntities(sudije);
                    dataGridViewUserControl1.Focus();
                    if (sudije.Count == 0)
                        MessageDialogs.showMessage("Ne postoje sudije koje zadovoljavaju date kriterijume.", "");
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }
    }
}