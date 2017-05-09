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
using Bilten.Util;

namespace Bilten.UI
{
    public partial class StartListaRotEditorForm : Form
    {
        private StartListaNaSpravi startLista;
        private int takmicenjeId;
        private int rotacija;
        private Color[] bojeZaEkipe;
        private bool dirty = false;
        private Gimnastika gimnastika;

        private RasporedNastupa raspored;
        public RasporedNastupa RasporedNastupa
        {
            get { return raspored; }
        }

        public StartListaRotEditorForm(int rasporedId, Sprava sprava,
            int grupa, int rotacija, int takmicenjeId, Color[] bojeZaEkipe)
        {
            InitializeComponent();
            this.takmicenjeId = takmicenjeId;
            this.rotacija = rotacija;
            this.bojeZaEkipe = bojeZaEkipe;

            spravaGridUserControl1.init(sprava);
            spravaGridUserControl1.DataGridViewUserControl.DataGridView.CellFormatting += new DataGridViewCellFormattingEventHandler(DataGridView_CellFormatting);

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    gimnastika = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId).Gimnastika;
                    raspored = DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO().FindByIdFetch(rasporedId);
                    startLista = raspored.getStartLista(sprava, grupa, rotacija);
                    foreach (NastupNaSpravi n in startLista.Nastupi)
                    {
                        //  potrebno za slucaj kada se u start listi nalaze i gimnasticari iz kategorija razlicitih od kategorija
                        // za koje start lista vazi.
                        NHibernateUtil.Initialize(n.Gimnasticar.TakmicarskaKategorija);
                    }

                    initUI();
                    spravaGridUserControl1.setItems(startLista.Nastupi);
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

        void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (rotacija != 1)
                return;

            if (e.ColumnIndex != 0)
                return;

            NastupNaSpravi n = (sender as DataGridView).Rows[e.RowIndex].DataBoundItem as NastupNaSpravi;
            if (n == null)
                return;

            List<byte> ekipe = getEkipe(sender as DataGridView, false);
            if (n.Ekipa > 0)
                e.CellStyle.BackColor = bojeZaEkipe[ekipe.IndexOf(n.Ekipa)];
            else
                e.CellStyle.BackColor = Color.White;
        }

        List<byte> getEkipe(DataGridView dgw, bool samoSelektovane)
        {
            List<byte> result = new List<byte>();
            foreach (DataGridViewRow row in dgw.Rows)
            {
                if (!row.Selected && samoSelektovane)
                    continue;
                NastupNaSpravi n = row.DataBoundItem as NastupNaSpravi;
                if (n.Ekipa > 0 && result.IndexOf(n.Ekipa) == -1)
                {
                    result.Add(n.Ekipa);
                }
            }
            return result;
        }

        private void initUI()
        {
            Text = "Start lista - " +
                DeoTakmicenjaKodovi.toString(raspored.DeoTakmicenjaKod);
            GridColumnsInitializer.initStartLista(startLista.Id, spravaGridUserControl1.DataGridViewUserControl);
            spravaGridUserControl1.DataGridViewUserControl.DataGridView.ColumnWidthChanged += new DataGridViewColumnEventHandler(DataGridView_ColumnWidthChanged);
        }

        void DataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            GridColumnsInitializer.startListaColumnWidthChanged(startLista.Id, sender as DataGridView);
        }

        private void StartListaRotEditorForm_Load(object sender, EventArgs e)
        {
            spravaGridUserControl1.clearSelection();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult = DialogResult.None;
            SelectGimnasticarUcesnikForm form = null;
            try
            {
                form = new SelectGimnasticarUcesnikForm(takmicenjeId, gimnastika, null);
                dlgResult = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (dlgResult != DialogResult.OK || form.SelectedEntities.Count == 0)
                return;

            bool added = false;
            foreach (GimnasticarUcesnik g in form.SelectedEntities)
            {
                if (startLista.addGimnasticar(g))
                    added = true;
            }

            if (added)
            {
                dirty = true;
                spravaGridUserControl1.setItems(startLista.Nastupi);
                spravaGridUserControl1.clearSelection();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            NastupNaSpravi nastup =
                spravaGridUserControl1.getSelectedItem<NastupNaSpravi>();
            if (nastup == null)
                return;

            string msgFmt = "Da li zelite da izbrisete gimnasticara '{0}'?";
            if (!MessageDialogs.queryConfirmation(
                String.Format(msgFmt, nastup.Gimnasticar.PrezimeIme), this.Text))
                return;

            startLista.removeNastup(nastup);
            dirty = true;
            spravaGridUserControl1.setItems(startLista.Nastupi);
            spravaGridUserControl1.clearSelection();
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if (startLista.empty())
                return;

            string msg = "Da li zelite da izbrisete sve gimnasticare?";
            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;

            startLista.clear();
            dirty = true;
            spravaGridUserControl1.setItems(startLista.Nastupi);
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            NastupNaSpravi nastup =
                spravaGridUserControl1.getSelectedItem<NastupNaSpravi>();
            if (nastup == null)
                return;

            if (startLista.moveNastupUp(nastup))
            {
                dirty = true;
                spravaGridUserControl1.setItems(startLista.Nastupi);
                spravaGridUserControl1.setSelectedItem<NastupNaSpravi>(nastup);
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            NastupNaSpravi nastup =
                spravaGridUserControl1.getSelectedItem<NastupNaSpravi>();
            if (nastup == null)
                return;

            if (startLista.moveNastupDown(nastup))
            {
                dirty = true;
                spravaGridUserControl1.setItems(startLista.Nastupi);
                spravaGridUserControl1.setSelectedItem<NastupNaSpravi>(nastup);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!dirty)
                return;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    DAOFactoryFactory.DAOFactory.GetStartListaNaSpraviDAO().Update(startLista);

                    Takmicenje t = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                    t.LastModified = DateTime.Now;
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
    }
}