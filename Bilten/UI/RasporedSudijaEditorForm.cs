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
using Iesi.Collections;
using NHibernate;

namespace Bilten.UI
{
    public partial class RasporedSudijaEditorForm : Form
    {
        private SudijskiOdborNaSpravi sudijskiOdbor;
        private int takmicenjeId;
        private IDataContext dataContext;

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
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                // TODO: Trebalo bi i NHibernateUtil.Initialize smestiti u 
                // IDataContext klasu

                raspored = loadRaspored(rasporedId);
                sudijskiOdbor = raspored.getOdbor(sprava);
                
                initUI();
                spravaGridUserControl1.setItems(sudijskiOdbor.Raspored);

              //  dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw new InfrastructureException(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
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
            GridColumnsInitializer.updateRasporedSudija(sudijskiOdbor.Id, sender as DataGridView);
        }

        private RasporedSudija loadRaspored(int rasporedId)
        {
            IList<RasporedSudija> result = dataContext.
                ExecuteNamedQuery<RasporedSudija>("FindRaspSudById",
                new string[] { "id" },
                new object[] { rasporedId });
            if (result.Count > 0)
                return result[0];
            else
                return null;
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
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                // TODO: Prvo proveri da li je nesto menjano
                dataContext.Save(sudijskiOdbor);

                dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showMessage(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                this.DialogResult = DialogResult.Cancel;
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
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