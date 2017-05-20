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
    public partial class SelectGimnasticarForm : SelectEntityForm
    {
        private Gimnastika gimnastika;
        private FilterGimnasticarUserControl filterGimnasticarUserControl1;

        public SelectGimnasticarForm(Gimnastika gimnastika)
        {
            InitializeComponent();
            Text = "Izaberi gimnasticara";
            this.gimnastika = gimnastika;

            filterGimnasticarUserControl1 = new FilterGimnasticarUserControl();
            this.pnlFilter.SuspendLayout();
            this.pnlFilter.Controls.Add(filterGimnasticarUserControl1);
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.Height = filterGimnasticarUserControl1.Height + 10;
            filterGimnasticarUserControl1.initialize(gimnastika);
            filterGimnasticarUserControl1.Filter += filterGimnasticarUserControl1_Filter;

            this.ClientSize = new Size(filterGimnasticarUserControl1.Width + 20, 540);
            initializeGridColumns();

            DataGridViewUserControl.GridColumnHeaderMouseClick += new EventHandler<GridColumnHeaderMouseClickEventArgs>(
                DataGridViewUserControl_GridColumnHeaderMouseClick);            
            showAll();
        }

        void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender, GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<Gimnasticar>(e.DataGridViewCellMouseEventArgs);
        }

        private void initializeGridColumns()
        {
            // TODO: Napravi da budu sinhronizovani ovi podaci sa podacima iz
            // GimnasticariForm
            dataGridViewUserControl1.AddColumn("Ime", "ImeSrednjeIme", 100);
            dataGridViewUserControl1.AddColumn("Prezime", "Prezime", 100);
            dataGridViewUserControl1.AddColumn("Datum rodjenja", "DatumRodjenja", 100, "{0:d}");
            dataGridViewUserControl1.AddColumn("Gimnastika", "Gimnastika", 70);
            dataGridViewUserControl1.AddColumn("Klub", "Klub", 150);
            dataGridViewUserControl1.AddColumn("Kategorija", "Kategorija", 100);
            dataGridViewUserControl1.AddColumn("Drzava", "Drzava", 100);
            dataGridViewUserControl1.AddColumn("Registarski broj", "RegistarskiBroj", 70);
        }

        private void showAll()
        {
            filterGimnasticarUserControl1.Filter -= filterGimnasticarUserControl1_Filter;
            filterGimnasticarUserControl1.resetFilter();
            filterGimnasticarUserControl1.Filter += filterGimnasticarUserControl1_Filter;
            
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<Gimnasticar> gimnasticari
                        = DAOFactoryFactory.DAOFactory.GetGimnasticarDAO().FindByGimnastika(gimnastika);
                    setEntities(gimnasticari);
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

        private void filterGimnasticarUserControl1_Filter(object sender, EventArgs e)
        {
            GimnasticarFilter flt = filterGimnasticarUserControl1.getFilter();
            if (flt != null)
                filter(flt);
        }

        private void filter(GimnasticarFilter flt)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    IList<Gimnasticar> gimnasticari;
                    if (flt.isEmpty(false))
                        gimnasticari = DAOFactoryFactory.DAOFactory.GetGimnasticarDAO().FindByGimnastika(gimnastika);
                    else
                    {
                        gimnasticari = DAOFactoryFactory.DAOFactory.GetGimnasticarDAO().FindGimnasticari(
                            flt.Ime, flt.Prezime, flt.GodRodj, flt.Gimnastika, flt.Drzava, flt.Kategorija, flt.Klub);
                    }
                    setEntities(gimnasticari);
                    if (gimnasticari.Count == 0)
                        MessageDialogs.showMessage("Ne postoje gimnasticari koji zadovoljavaju date kriterijume.", "");
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
