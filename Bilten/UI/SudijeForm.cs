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
    public partial class SudijeForm : SingleEntityListForm<Sudija>
    {
        private FilterSudijaUserControl filterSudijaUserControl1;
        
        public SudijeForm()
        {
            this.Text = "Sudije";

            filterSudijaUserControl1 = new FilterSudijaUserControl();
            this.pnlFilter.SuspendLayout();
            this.pnlFilter.Controls.Add(filterSudijaUserControl1);
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.Height = filterSudijaUserControl1.Height + 10;
            filterSudijaUserControl1.initialize();
            filterSudijaUserControl1.Filter += filterSudijaUserControl1_Filter;

            this.ClientSize = new Size(filterSudijaUserControl1.Width + panel1.Width + 20, 540);
            dataGridViewUserControl1.GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            InitializeGridColumns();

            prikaziSve();            
        }

        protected override void prikaziSve()
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
                    SetItems(sudije);
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

        // TODO: Dodaj i druge stvari koje postoje u GimnasticariForm, kao npr. provera prilikom unosa da li
        // sudija vec postoji i ako postoji otvaranje dijaloga za edit.

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<Sudija>(e.DataGridViewCellMouseEventArgs);
        }

        private void InitializeGridColumns()
        {
            AddColumn("Ime", "Ime", 100);
            AddColumn("Prezime", "Prezime", 100);
            AddColumn("Pol", "Pol", 100);
            AddColumn("Klub", "Klub", 150);
            AddColumn("Drzava", "Drzava", 100);
        }

        protected override EntityDetailForm createEntityDetailForm(Nullable<int> entityId)
        {
            return new SudijaForm(entityId);
        }

        protected override string deleteConfirmationMessage(Sudija sudija)
        {
            return String.Format("Da li zelite da izbrisete sudiju \"{0}\"?", sudija);
        }

        protected override string deleteErrorMessage()
        {
            return "Neuspesno brisanje sudije.";
        }

        protected override void delete(Sudija s)
        {
            DAOFactoryFactory.DAOFactory.GetSudijaDAO().Delete(s);
        }

        protected override void updateEntityCount()
        {
            int count = dataGridViewUserControl1.getItems<Sudija>().Count;
            StatusPanel.Panels[0].Text = count.ToString() + " sudija";
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
                    SetItems(sudije);
                    dataGridViewUserControl1.Focus();
                    updateEntityCount();
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