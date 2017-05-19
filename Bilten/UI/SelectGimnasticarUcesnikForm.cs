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
        private TakmicarskaKategorija kategorija;

        private FilterGimnasticarUcesnikUserControl filterGimnasticarUcesnikUserControl1;
        
        public SelectGimnasticarUcesnikForm(int takmicenjeId, Gimnastika gimnastika, TakmicarskaKategorija kategorija)
        {
            InitializeComponent();
            Text = "Izaberi gimnasticara";

            this.takmicenjeId = takmicenjeId;
            this.gimnastika = gimnastika;
            this.kategorija = kategorija;

            filterGimnasticarUcesnikUserControl1 = new FilterGimnasticarUcesnikUserControl();
            this.pnlFilter.SuspendLayout();
            this.pnlFilter.Controls.Add(filterGimnasticarUcesnikUserControl1);
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.Height = filterGimnasticarUcesnikUserControl1.Height + 10;
            filterGimnasticarUcesnikUserControl1.Filter += filterGimnasticarUcesnikUserControl1_Filter;
            filterGimnasticarUcesnikUserControl1.initialize(takmicenjeId, gimnastika, kategorija);

            initializeGridColumns();

            DataGridViewUserControl.GridColumnHeaderMouseClick += new EventHandler<GridColumnHeaderMouseClickEventArgs>(
                DataGridViewUserControl_GridColumnHeaderMouseClick);
            this.ClientSize = new Size(800, 450);

            showAll();
        }

        void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender, GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<GimnasticarUcesnik>(e.DataGridViewCellMouseEventArgs);
        }

        private void initializeGridColumns()
        {
            // TODO: Napravi da budu sinhronizovani ovi podaci sa podacima iz
            // EkipaForm
            dataGridViewUserControl1.AddColumn("Ime", "ImeSrednjeIme", 100);
            dataGridViewUserControl1.AddColumn("Prezime", "Prezime", 100);
            dataGridViewUserControl1.AddColumn("Datum rodjenja", "DatumRodjenja", 100, "{0:d}");
            dataGridViewUserControl1.AddColumn("Gimnastika", "Gimnastika", 70);
            dataGridViewUserControl1.AddColumn("Kategorija", "TakmicarskaKategorija", 100);
            dataGridViewUserControl1.AddColumn("Klub", "KlubUcesnik", 150);
            dataGridViewUserControl1.AddColumn("Drzava", "DrzavaUcesnik", 100);
        }

        private void showAll()
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<GimnasticarUcesnik> gimnasticari
                        = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO().FindByTakmicenjeKat(takmicenjeId, kategorija);
                    setEntities(gimnasticari);
                    DataGridViewUserControl.sort<GimnasticarUcesnik>(
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
                    string failureMsg = "Ne postoje gimnasticari koji zadovoljavaju date kriterijume.";
                    IList<GimnasticarUcesnik> gimnasticari
                        = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO().FindGimnasticariUcesnici(
                            flt.Ime, flt.Prezime, flt.GodRodj, flt.Drzava,
                            flt.Kategorija, flt.Klub, takmicenjeId);
                    setEntities(gimnasticari);
                    if (gimnasticari.Count == 0)
                        MessageDialogs.showMessage(failureMsg, this.Text);
                    dataGridViewUserControl1.clearSelection();
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

        private void SelectGimnasticarUcesnikForm_Load(object sender, EventArgs e)
        {
            dataGridViewUserControl1.clearSelection();
        }
    }
}
