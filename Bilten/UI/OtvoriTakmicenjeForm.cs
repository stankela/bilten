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
using Bilten.Services;

namespace Bilten.UI
{
    public partial class OtvoriTakmicenjeForm : Form
    {
        private Nullable<int> currTakmicenjeId;
        private List<Takmicenje> takmicenja;
        bool selectMode;
        int broj;
        bool gornjaGranica;
        private StatusBar statusBar;


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

        public OtvoriTakmicenjeForm(Nullable<int> currTakmicenjeId, bool selectMode, int broj, bool gornjaGranica,
            Gimnastika gimnastika)
        {
            InitializeComponent();
            ClientSize = new System.Drawing.Size(ClientSize.Width, Screen.PrimaryScreen.WorkingArea.Height - 100);
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
                    if (gimnastika != Gimnastika.Undefined)
                    {
                        takmicenja = new List<Takmicenje>(DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO()
                            .FindByGimnastika(gimnastika));
                    }
                    else
                        takmicenja = new List<Takmicenje>(DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindAll());
                    setTakmicenja(takmicenja);
                    if (!selectMode)
                        updateTakmicenjaCount();
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

                statusBar = new StatusBar();
                statusBar.Parent = this;
                statusBar.ShowPanels = true;
                StatusBarPanel sbPanel1 = new StatusBarPanel();
                statusBar.Panels.Add(sbPanel1);
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
                    (gornjaGranica && selItems.Count > 1 && selItems.Count <= broj))
                    selTakmicenja = selItems;
                else
                {
                    string msg;
                    if (broj == 1)
                        msg = "Izaberite jedno takmicenje.";
                    else if (!gornjaGranica)
                        msg = String.Format("Izaberite {0} takmicenja.", broj);
                    else
                        msg = String.Format("Izaberite do {0} takmicenja (minimalno 2).", broj);
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
                    TakmicenjeService.deleteTakmicenje(selTakmicenje.Id);
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
            updateTakmicenjaCount();
        }

        private void updateTakmicenjaCount()
        {
            statusBar.Panels[0].Text = takmicenja.Count.ToString() + " takmicenja";
        }

        private void OtvoriTakmicenjeForm_Shown(object sender, EventArgs e)
        {
            dataGridViewUserControl1.clearSelection();
        }

    }
}