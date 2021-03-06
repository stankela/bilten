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
    public partial class SelectGimnasticarUcesnikForm : SelectEntityForm
    {
        private int takmicenjeId;
        private Gimnastika gimnastika;

        private FilterGimnasticarUcesnikUserControl filterGimnasticarUcesnikUserControl1;
        
        public SelectGimnasticarUcesnikForm(int takmicenjeId, Gimnastika gimnastika, TakmicarskaKategorija kategorija)
        {
            InitializeComponent();
            Text = "Izaberi gimnasticara";

            this.takmicenjeId = takmicenjeId;
            this.gimnastika = gimnastika;

            filterGimnasticarUcesnikUserControl1 = new FilterGimnasticarUcesnikUserControl();
            filterGimnasticarUcesnikUserControl1.initialize(takmicenjeId, gimnastika, kategorija);
            this.pnlFilter.SuspendLayout();
            this.pnlFilter.Controls.Add(filterGimnasticarUcesnikUserControl1);
            this.pnlFilter.Width = filterGimnasticarUcesnikUserControl1.Width;
            this.pnlFilter.ResumeLayout(false);
            filterGimnasticarUcesnikUserControl1.Filter += filterGimnasticarUcesnikUserControl1_Filter;

            int x = filterGimnasticarUcesnikUserControl1.Location.X + filterGimnasticarUcesnikUserControl1.Width + 30;
            int y = filterGimnasticarUcesnikUserControl1.Location.Y + filterGimnasticarUcesnikUserControl1.btnPonistiLocation.Y;
            btnOk.Location = new Point(x, y);
            btnCancel.Location = new Point(btnOk.Location.X + 85, btnOk.Location.Y);

            this.ClientSize = new Size(btnCancel.Location.X + btnCancel.Size.Width + 20, 540);
            initializeGridColumns();
            DataGridViewUserControl.GridColumnHeaderMouseClick += new EventHandler<GridColumnHeaderMouseClickEventArgs>(
                DataGridViewUserControl_GridColumnHeaderMouseClick);

            showAll(kategorija);
        }

        void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender, GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<GimnasticarUcesnik>(e.DataGridViewCellMouseEventArgs);
        }

        private void initializeGridColumns()
        {
            dataGridViewUserControl1.AddColumn("Ime", "ImeSrednjeIme", 100);
            dataGridViewUserControl1.AddColumn("Prezime", "Prezime", 100);
            dataGridViewUserControl1.AddColumn("Datum rodjenja", "DatumRodjenja", 100, "{0:d}");
            dataGridViewUserControl1.AddColumn("Gimnastika", "Gimnastika", 70);
            dataGridViewUserControl1.AddColumn("Kategorija", "TakmicarskaKategorija", 100);
            dataGridViewUserControl1.AddColumn("Klub", "KlubUcesnik", 150);
            dataGridViewUserControl1.AddColumn("Drzava", "DrzavaUcesnik", 100);
        }

        private void showAll(TakmicarskaKategorija kategorija)
        {
            filterGimnasticarUcesnikUserControl1.Filter -= filterGimnasticarUcesnikUserControl1_Filter;
            filterGimnasticarUcesnikUserControl1.resetFilter(kategorija);
            filterGimnasticarUcesnikUserControl1.Filter += filterGimnasticarUcesnikUserControl1_Filter;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<GimnasticarUcesnik> gimnasticari = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO()
                        .FindByTakmicenjeKat(takmicenjeId, kategorija);
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

        private void filterGimnasticarUcesnikUserControl1_Filter(object sender, EventArgs e)
        {
            GimnasticarUcesnikFilter flt = filterGimnasticarUcesnikUserControl1.getFilter();
            if (flt != null)
                filter(flt);
        }

        private void filter(GimnasticarUcesnikFilter flt)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<GimnasticarUcesnik> gimnasticari;
                    if (flt.isEmpty())
                    {
                        gimnasticari = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO().FindByTakmicenjeKat(
                            takmicenjeId, null);
                    }
                    else
                    {
                        gimnasticari = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO().FindGimnasticariUcesnici(
                                flt.Ime, flt.Prezime, flt.Drzava, flt.Kategorija, flt.Klub, takmicenjeId);
                    }
                    setEntities(gimnasticari);
                    dataGridViewUserControl1.Focus();
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
