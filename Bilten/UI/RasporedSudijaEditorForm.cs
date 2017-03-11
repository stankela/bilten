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
using Iesi.Collections;
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class RasporedSudijaEditorForm : Form
    {
        private SudijskiOdborNaSpravi sudijskiOdbor;
        private int takmicenjeId;

        private RasporedSudija raspored;
        public RasporedSudija RasporedSudija
        {
            get { return raspored; }
        }

        public RasporedSudijaEditorForm(int rasporedId, Sprava sprava,
            int takmicenjeId)
        {
            InitializeComponent();
            this.takmicenjeId = takmicenjeId;
            spravaGridUserControl1.init(sprava);

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    // TODO: Trebalo bi i NHibernateUtil.Initialize smestiti u 
                    // DAO klasu

                    raspored = DAOFactoryFactory.DAOFactory.GetRasporedSudijaDAO().FindByIdFetch(rasporedId);
                    sudijskiOdbor = raspored.getOdbor(sprava);

                    initUI();
                    spravaGridUserControl1.setItems(sudijskiOdbor.Raspored);
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
            Text = "Raspored sudija - " +
                DeoTakmicenjaKodovi.toString(raspored.DeoTakmicenjaKod);
            GridColumnsInitializer.initRasporedSudija(sudijskiOdbor.Id, spravaGridUserControl1.DataGridViewUserControl);
            spravaGridUserControl1.DataGridViewUserControl.DataGridView.ColumnWidthChanged += new DataGridViewColumnEventHandler(DataGridView_ColumnWidthChanged);
        }

        void DataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            GridColumnsInitializer.rasporedSudijaColumnWidthChanged(sudijskiOdbor.Id, sender as DataGridView);
        }

        private void RasporedSudijaEditorForm_Load(object sender, EventArgs e)
        {
            spravaGridUserControl1.clearSelection();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (sudijskiOdbor.isComplete())
                return;

            DialogResult dlgResult = DialogResult.None;
            SelectSudijaUcesnikForm form = null;
            try
            {
                form = new SelectSudijaUcesnikForm(takmicenjeId);
                dlgResult = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (dlgResult != DialogResult.OK || form.SelectedEntities.Count == 0)
                return;

            List<SudijaUcesnik> okSudije = new List<SudijaUcesnik>();
            List<SudijaUcesnik> illegalSudije = new List<SudijaUcesnik>();
            foreach (SudijaUcesnik s in form.SelectedEntities)
            {
                if (sudijskiOdbor.canAddSudija(s))
                {
                    sudijskiOdbor.addSudija(s);
                    okSudije.Add(s);
                }
                else
                {
                    illegalSudije.Add(s);
                }

            }
            if (okSudije.Count > 0)
            {
                spravaGridUserControl1.setItems(sudijskiOdbor.Raspored);
            }

            if (illegalSudije.Count > 0)
            {
                string msg = "Sledece sudije nisu dodate: \n\n";
                msg += StringUtil.getListString(illegalSudije.ToArray());
         //       MessageDialogs.showMessage(msg, this.Text);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            SudijaNaSpravi sudija = 
                spravaGridUserControl1.getSelectedItem<SudijaNaSpravi>();
            if (sudija == null || sudija.Sudija == null)
                return;

            string msgFmt = "Da li zelite da izbrisete sudiju '{0}'?";
            if (!MessageDialogs.queryConfirmation(String.Format(msgFmt, sudija.PrezimeIme), this.Text))
                return;

            sudijskiOdbor.removeSudija(sudija.Uloga);
            spravaGridUserControl1.setItems(sudijskiOdbor.Raspored);
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if (sudijskiOdbor.isEmpty())
                return;

            string msg = "Da li zelite da izbrisete sve sudije?";
            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;

            sudijskiOdbor.clearSudije();
            spravaGridUserControl1.setItems(sudijskiOdbor.Raspored);
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            SudijaNaSpravi sudija = 
                spravaGridUserControl1.getSelectedItem<SudijaNaSpravi>();
            if (sudija == null)
                return;

            int index = sudijskiOdbor.Raspored.IndexOf(sudija);
            if (sudijskiOdbor.moveSudijaUp(index))
                spravaGridUserControl1.setSelectedItem<SudijaNaSpravi>(
                    sudijskiOdbor.Raspored[index - 1]);
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            SudijaNaSpravi sudija =
                spravaGridUserControl1.getSelectedItem<SudijaNaSpravi>();
            if (sudija == null)
                return;

            int index = sudijskiOdbor.Raspored.IndexOf(sudija);
            if (sudijskiOdbor.moveSudijaDown(index))
                spravaGridUserControl1.setSelectedItem<SudijaNaSpravi>(
                    sudijskiOdbor.Raspored[index + 1]);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    // TODO: Prvo proveri da li je nesto menjano
                    DAOFactoryFactory.DAOFactory.GetSudijskiOdborNaSpraviDAO().Update(sudijskiOdbor);
                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showMessage(ex.Message, this.Text);
                this.DialogResult = DialogResult.Cancel;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void btnFunkcije_Click(object sender, EventArgs e)
        {
            SudijskeUlogeEditorForm form = new SudijskeUlogeEditorForm(sudijskiOdbor);
            if (form.ShowDialog() == DialogResult.OK)
                spravaGridUserControl1.setItems(sudijskiOdbor.Raspored);
        }
    }
}