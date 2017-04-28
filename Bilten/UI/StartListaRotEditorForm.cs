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
                form = new SelectGimnasticarUcesnikForm(takmicenjeId, raspored.Gimnastika, null);
                dlgResult = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (dlgResult != DialogResult.OK || form.SelectedEntities.Count == 0)
                return;

            List<GimnasticarUcesnik> okGimnasticari = new List<GimnasticarUcesnik>();
            List<GimnasticarUcesnik> illegalGimnasticari = new List<GimnasticarUcesnik>();
            foreach (GimnasticarUcesnik g in form.SelectedEntities)
            {
                if (startLista.canAddGimnasticar(g))
                    okGimnasticari.Add(g);
                else
                    illegalGimnasticari.Add(g);
            }

            /*for (int i = okGimnasticari.Count - 1; i >= 0; i--)
            {
                GimnasticarUcesnik g = okGimnasticari[i];
                if (!raspored.Kategorije.Contains(g.TakmicarskaKategorija))
                {
                    okGimnasticari.RemoveAt(i);
                    illegalGimnasticari.Add(g);
                }
            }*/

            foreach (GimnasticarUcesnik g in okGimnasticari)
            {
                startLista.addGimnasticar(g);
            }

            if (okGimnasticari.Count > 0)
            {
                spravaGridUserControl1.setItems(startLista.Nastupi);
                spravaGridUserControl1.clearSelection();
            }

            if (illegalGimnasticari.Count > 0)
            {
                string msg = "Sledeci gimnasticari nisu dodati, zato sto ili vec " +
                    "postoje na start listi, ili im kategorija nije odgovarajuca: \n\n";
                msg += StringUtil.getListString(illegalGimnasticari.ToArray());
                //       MessageDialogs.showMessage(msg, this.Text);
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
                spravaGridUserControl1.setItems(startLista.Nastupi);
                spravaGridUserControl1.setSelectedItem<NastupNaSpravi>(nastup);
            }
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

                    // Proveri da li se sve ekipe sastoje od uzastopnih gimnsticara. Ako ne, sve gimnasticare koji se nalaze
                    // izmedju dva clana neke ekipe proglasi za clanove te iste ekipe.
                    if (rotacija == 1)
                    {
                        byte ekipa = findFragmentedEkipa(startLista);
                        while (ekipa > 0)
                        {
                            kompaktujEkipu(ekipa, startLista);
                            ekipa = findFragmentedEkipa(startLista);
                        }
                    }

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

        private byte findFragmentedEkipa(StartListaNaSpravi startLista)
        {
            List<byte> ekipe = new List<byte>();
            byte prevEkipa = 0;
            for (int i = 0; i < startLista.Nastupi.Count; ++i)
            {
                NastupNaSpravi n = startLista.Nastupi[i];
                if (n.Ekipa != prevEkipa)
                {
                    // Nasli smo novu ekipu (ili pojedinca) n.Ekipa, sto znaci da se time zavrsava prevEkipa.
                    if (prevEkipa > 0)
                        ekipe.Add(prevEkipa);

                    // Proveri da li je nova ekipa n.Ekipa ranije vec pronadjena.
                    if (n.Ekipa > 0 && ekipe.IndexOf(n.Ekipa) != -1)
                    {
                        // Clanovi ekipe n.Ekipa nisu uzastopni.
                        return n.Ekipa;
                    }

                    prevEkipa = n.Ekipa;
                }
            }
            return 0;
        }

        private void kompaktujEkipu(byte ekipa, StartListaNaSpravi startLista)
        {
            int start = -1;
            int end = -1;
            for (int i = 0; i < startLista.Nastupi.Count; ++i)
            {
                NastupNaSpravi n = startLista.Nastupi[i];
                if (n.Ekipa == ekipa)
                {
                    if (start == -1)
                        start = i;
                    end = i;
                }
            }

            for (int i = start; i <= end; ++i)
            {
                NastupNaSpravi n = startLista.Nastupi[i];
                n.Ekipa = ekipa;
            }
        }

    }
}