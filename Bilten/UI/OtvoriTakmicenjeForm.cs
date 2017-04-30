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
        private bool selectMode;
        private int donjaGranica;
        private int gornjaGranica;
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

        public OtvoriTakmicenjeForm(Nullable<int> currTakmicenjeId)
        {
            InitializeComponent();
            this.currTakmicenjeId = currTakmicenjeId;
            this.selectMode = false;
            this.donjaGranica = -1;
            this.gornjaGranica = -1;
            init(Gimnastika.Undefined);
        }

        public OtvoriTakmicenjeForm(int broj, Gimnastika gimnastika)
        {
            InitializeComponent();
            this.currTakmicenjeId = null;
            this.selectMode = true;
            this.donjaGranica = broj;
            this.gornjaGranica = broj;
            init(gimnastika);
        }

        public OtvoriTakmicenjeForm(int donjaGranica, int gornjaGranica, Gimnastika gimnastika)
        {
            InitializeComponent();
            this.currTakmicenjeId = null;
            this.selectMode = true;
            this.donjaGranica = donjaGranica;
            this.gornjaGranica = gornjaGranica;
            init(gimnastika);
        }

        private void init(Gimnastika gimnastika)
        {
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
            ClientSize = new Size(ClientSize.Width, Screen.PrimaryScreen.WorkingArea.Height - 100);
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
                handleOkClick();
        }

        private void setTakmicenja(List<Takmicenje> takmicenja)
        {
            dataGridViewUserControl1.setItems<Takmicenje>(takmicenja);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            handleOkClick();
        }

        private void handleOkClick()
        {
            if (!selectMode)
            {
                Takmicenje selTakmicenje = dataGridViewUserControl1.getSelectedItem<Takmicenje>();
                if (selTakmicenje != null)
                {
                    takmicenje = selTakmicenje;
                    DialogResult = DialogResult.OK;
                }
                else
                    DialogResult = DialogResult.None;
            }
            else
            {
                IList<Takmicenje> selItems = dataGridViewUserControl1.getSelectedItems<Takmicenje>();
                if (selItems.Count >= donjaGranica && selItems.Count <= gornjaGranica)
                {
                    selTakmicenja = selItems;
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    string msg;
                    if (donjaGranica == gornjaGranica && donjaGranica == 1)
                        msg = "Izaberite jedno takmicenje.";
                    else if (donjaGranica == gornjaGranica && donjaGranica > 1)
                        msg = String.Format("Izaberite {0} takmicenja.", donjaGranica);
                    else
                        msg = String.Format("Izaberite do {0} takmicenja (minimalno {1}).", gornjaGranica, donjaGranica);
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