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
using Bilten.Dao.NHibernate;

namespace Bilten.UI
{
    public partial class OtvoriTakmicenjeForm : Form
    {
        private Nullable<int> currTakmicenjeId;
        private List<Takmicenje> takmicenja;
        bool selectMode;
        int broj;
        bool gornjaGranica;

        private Takmicenje takmicenje;
        public Takmicenje Takmicenje
        {
            get { return takmicenje; }
        }

        private IList<Takmicenje> selTakmicenja;
        public IList<Takmicenje> SelTakmicenja
        {
            get { return selTakmicenja; }
        }

        public OtvoriTakmicenjeForm(Nullable<int> currTakmicenjeId, bool selectMode, int broj, bool gornjaGranica)
        {
            InitializeComponent();
            this.currTakmicenjeId = currTakmicenjeId;
            this.selectMode = selectMode;
            this.broj = broj;
            this.gornjaGranica = gornjaGranica;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    initUI();
                    takmicenja = new List<Takmicenje>(DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindAll());
                    setTakmicenja(takmicenja);
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

        private void initUI()
        {
            if (!selectMode)
            {
                this.Text = "Otvori takmicenje";
                dataGridViewUserControl1.DataGridView.MultiSelect = false;
            }
            else
            {
                this.Text = "Izaberi takmicenje";
                dataGridViewUserControl1.DataGridView.MultiSelect = true;
                btnDelete.Visible = false;
                btnDelete.Enabled = false;
                btnOpen.Text = "OK";
            }

            dataGridViewUserControl1.DataGridView.CellDoubleClick += new DataGridViewCellEventHandler(DataGridView_CellDoubleClick);
            GridColumnsInitializer.initTakmicenje(dataGridViewUserControl1);
        }

        void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                otvoriTakmicenje();
                DialogResult = DialogResult.OK;
            }
        }

        private void setTakmicenja(List<Takmicenje> takmicenja)
        {
            dataGridViewUserControl1.setItems<Takmicenje>(takmicenja);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            otvoriTakmicenje();
        }

        private void otvoriTakmicenje()
        {
            if (!selectMode)
            {
                Takmicenje selTakmicenje = dataGridViewUserControl1.getSelectedItem<Takmicenje>();
                if (selTakmicenje != null)
                    takmicenje = selTakmicenje;
                else
                    DialogResult = DialogResult.None;
            }
            else
            {
                IList<Takmicenje> selItems = dataGridViewUserControl1.getSelectedItems<Takmicenje>();
                if ((!gornjaGranica && selItems.Count == broj) ||
                    (gornjaGranica && selItems.Count > 0 && selItems.Count <= broj))
                    selTakmicenja = selItems;
                else
                {
                    string msg;
                    if (broj == 1)
                        msg = "Izaberite jedno takmicenje.";
                    else if (!gornjaGranica)
                        msg = String.Format("Izaberite {0} takmicenja.", broj);
                    else
                        msg = String.Format("Izaberite do {0} takmicenja.", broj);
                    MessageDialogs.showMessage(msg, this.Text);
                    DialogResult = DialogResult.None;
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Takmicenje selTakmicenje = dataGridViewUserControl1.getSelectedItem<Takmicenje>();
            if (selTakmicenje == null)
                return;

            string msgFmt = "Da li zelite da izbrisete takmicenje \"{0}\"?";
            if (!MessageDialogs.queryConfirmation(String.Format(
                msgFmt, selTakmicenje), this.Text))
                return;

            if (selTakmicenje.Id == currTakmicenjeId)
            {
                string msg = "Nije dozvoljeno brisanje takmicenja koje je trenutno otvoreno.";
                MessageDialogs.showMessage(msg, this.Text);
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    deleteTakmicenje(selTakmicenje);
                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                Close();
                return;
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            takmicenja.Remove(selTakmicenje);
            setTakmicenja(takmicenja);
            if (dataGridViewUserControl1.isSorted())
                dataGridViewUserControl1.refreshItems();
        }

        private void deleteTakmicenje(Takmicenje takmicenje)
        {
            // brisi ocene
            OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
            IList<Ocena> ocene = ocenaDAO.FindByTakmicenje(takmicenje.Id);
            foreach (Ocena o in ocene)
                ocenaDAO.Delete(o);

            // brisi rasporede nastupa
            RasporedNastupaDAO rasporedNastupaDAO = DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO();
            IList<RasporedNastupa> rasporediNastupa = rasporedNastupaDAO.FindByTakmicenje(takmicenje.Id);
            foreach (RasporedNastupa r in rasporediNastupa)
            {
                rasporedNastupaDAO.Delete(r);
            }

            // brisi rasporede sudija
            RasporedSudijaDAO rasporedSudijaDAO = DAOFactoryFactory.DAOFactory.GetRasporedSudijaDAO();
            IList<RasporedSudija> rasporediSudija = rasporedSudijaDAO.FindByTakmicenje(takmicenje.Id);
            foreach (RasporedSudija r in rasporediSudija)
                rasporedSudijaDAO.Delete(r);

            // brisi sudije ucesnike
            SudijaUcesnikDAO sudijaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetSudijaUcesnikDAO();
            IList<SudijaUcesnik> sudije = sudijaUcesnikDAO.FindByTakmicenje(takmicenje.Id);
            foreach (SudijaUcesnik s in sudije)
                sudijaUcesnikDAO.Delete(s);

            // brisi rezultatska takmicenja i ekipe
            RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();
            IList<RezultatskoTakmicenje> rezTakmicenja = rezultatskoTakmicenjeDAO.FindByTakmicenje(takmicenje.Id);
            foreach (RezultatskoTakmicenje r in rezTakmicenja)
            {
                Takmicenje1 t1 = r.Takmicenje1;
                foreach (Ekipa ek in t1.Ekipe)
                    ekipaDAO.Delete(ek);
                rezultatskoTakmicenjeDAO.Delete(r);
            }

            // brisi gimnasticare ucesnike
            GimnasticarUcesnikDAO gimnasticarUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
            IList<GimnasticarUcesnik> gimnasticari = gimnasticarUcesnikDAO.FindByTakmicenje(takmicenje.Id);
            foreach (GimnasticarUcesnik g in gimnasticari)
                gimnasticarUcesnikDAO.Delete(g);

            // brisi klubove ucesnike
            KlubUcesnikDAO klubUcesnikDAO = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO();
            IList<KlubUcesnik> klubovi = klubUcesnikDAO.FindByTakmicenje(takmicenje.Id);
            foreach (KlubUcesnik k in klubovi)
                klubUcesnikDAO.Delete(k);

            // brisi drzave ucesnike
            DrzavaUcesnikDAO drzavaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO();
            IList<DrzavaUcesnik> drzave = drzavaUcesnikDAO.FindByTakmicenje(takmicenje.Id);
            foreach (DrzavaUcesnik d in drzave)
                drzavaUcesnikDAO.Delete(d);

            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            TakmicarskaKategorijaDAO takmicarskaKategorijaDAO = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO();
            RezultatskoTakmicenjeDescriptionDAO rezTakDescDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDescriptionDAO();

            // brisi kategorije
            takmicenjeDAO.Attach(takmicenje, false);
            foreach (TakmicarskaKategorija k in takmicenje.Kategorije)
                takmicarskaKategorijaDAO.Delete(k);

            // brisi descriptions
            foreach (RezultatskoTakmicenjeDescription d in takmicenje.TakmicenjeDescriptions)
                rezTakDescDAO.Delete(d);

            // brisi takmicenje
            takmicenjeDAO.Delete(takmicenje);
        }

    }
}