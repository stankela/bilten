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

        public SelectGimnasticarUcesnikForm(int takmicenjeId, Gimnastika gimnastika,
            TakmicarskaKategorija kategorija)
        {
            InitializeComponent();
            Text = "Izaberi gimnasticara";

            this.takmicenjeId = takmicenjeId;
            this.gimnastika = gimnastika;
            this.kategorija = kategorija;
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
            dataGridViewUserControl1.AddColumn("Ime", "Ime", 100);
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

        protected override void filter(object filterObject)
        {
            GimnasticarUcesnikFilter flt = filterObject as GimnasticarUcesnikFilter;
            if (flt == null)
                return;

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

        protected override FilterForm createFilterForm()
        {
            // TODO: Ovaj poziv je uredu za prvo prikazivanje FilterForma, zato sto
            // ga inicijalizuje na osnovu stanja u gridu. Medjutim, za sledeca
            // prikazivanja ne mora da bude u redu - npr. ako se nakon prvog 
            // prikazivanja FilterForma u gridu budu neki opstiji podaci, tada
            // ce za drugo prikazivanje FilterForm biti inicijalizovan razlicito
            // od stanja u gridu. Razmisli o varijanti da se FilterForm uvek 
            // inicijalizuje na osnovu stanja u gridu
            return new FilterGimnasticarUcesnikForm(takmicenjeId, gimnastika, kategorija);
        }

        private void SelectGimnasticarUcesnikForm_Load(object sender, EventArgs e)
        {
            dataGridViewUserControl1.clearSelection();
        }
    }
}
