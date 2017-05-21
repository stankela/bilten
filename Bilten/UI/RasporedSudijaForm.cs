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
using Bilten.Report;
using NHibernate.Context;
using Bilten.Dao;
using Bilten.Util;

namespace Bilten.UI
{
    public partial class RasporedSudijaForm : Form
    {
        private Takmicenje takmicenje;
        private DeoTakmicenjaKod deoTakKod;
        private IList<RasporedSudija> rasporedi;
        private List<bool> tabOpened;
        private int kategorijeCount;

        private int clickedRow;
        private int clickedColumn;
        private Sprava clickedSprava;
        private Point USER_CONTROL_LOCATION = new Point(10, 10);

        public RasporedSudijaForm(int takmicenjeId, DeoTakmicenjaKod deoTakKod)
        {
            InitializeComponent();
            this.ClientSize = new Size(1150, 540);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.deoTakKod = deoTakKod;

            this.Text = "Raspored sudija - " +
                DeoTakmicenjaKodovi.toString(deoTakKod);

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    kategorijeCount = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO()
                        .GetCountForTakmicenje(takmicenjeId);
                    if (kategorijeCount == 0)
                        throw new Exception("Greska u programu.");
                    rasporedi = DAOFactoryFactory.DAOFactory.GetRasporedSudijaDAO()
                        .FindByTakmicenjeDeoTak(takmicenjeId, deoTakKod);

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                    NHibernateUtil.Initialize(takmicenje);

                    // create tabs
                    for (int i = 0; i < rasporedi.Count; i++)
                        createTab(rasporedi[i]);

                    tabOpened = new List<bool>();
                    for (int i = 0; i < rasporedi.Count; i++)
                        tabOpened.Add(false);

                    // show first tab
                    if (rasporedi.Count > 0)
                    {
                        if (tabControl1.SelectedIndex != 0)
                            tabControl1.SelectedIndex = 0;
                        else
                            onSelectedIndexChanged();
                    }
                    else
                        tabControl1.TabPages.Remove(tabPage1);
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
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void createTab(RasporedSudija raspored)
        {
            if (rasporedi.IndexOf(raspored) == 0) // prvi tab
            {
                // init first tab
                if (tabControl1.TabPages.IndexOf(tabPage1) < 0)
                    tabControl1.TabPages.Add(tabPage1);
                spravaGridGroupUserControl1.Location = USER_CONTROL_LOCATION;
                spravaGridGroupUserControl1.SpravaGridRightClick +=
                  new EventHandler<SpravaGridRightClickEventArgs>(spravaGridGroupUserControl1_SpravaGridRightClick);
                spravaGridGroupUserControl1.init(takmicenje.Gimnastika);
                foreach (SpravaGridUserControl c in spravaGridGroupUserControl1.SpravaGridUserControls)
                {
                    SudijskiOdborNaSpravi odbor = raspored.getOdbor(c.Sprava);
                    int odborId = odbor != null ? odbor.Id : 0;
                    GridColumnsInitializer.initRasporedSudija(odborId, c.DataGridViewUserControl);
                    c.DataGridViewUserControl.DataGridView.ColumnWidthChanged += new DataGridViewColumnEventHandler(DataGridView_ColumnWidthChanged);
                }
                tabPage1.AutoScroll = true;
                tabPage1.AutoScrollMinSize = new Size(
                    spravaGridGroupUserControl1.Right, spravaGridGroupUserControl1.Bottom);
                tabPage1.AutoScrollMargin =
                    new Size(spravaGridGroupUserControl1.Location);
                tabPage1.Text = raspored.Naziv;
            }
            else
            {
                // init other tabs
                TabPage newTab = new TabPage();
                tabControl1.Controls.Add(newTab);
                initTab(newTab, raspored);
            }
        }

        void spravaGridGroupUserControl1_SpravaGridRightClick(object sender, SpravaGridRightClickEventArgs e)
        {
            clickedSprava = e.Sprava;
            DataGridView grid = getActiveSpravaGridGroupUserControl()[clickedSprava]
                .DataGridViewUserControl.DataGridView;
            int x = e.MouseEventArgs.X;
            int y = e.MouseEventArgs.Y;
            if (grid.HitTest(x, y).Type == DataGridViewHitTestType.Cell)
            {
                clickedRow = grid.HitTest(x, y).RowIndex;
                clickedColumn = grid.HitTest(x, y).ColumnIndex;
                mnPrikaziKlub.Enabled = mnPrikaziKlub.Visible = true;
                mnPrikaziDrzavu.Enabled = mnPrikaziDrzavu.Visible = true;

            }
            else
            {
                mnPrikaziKlub.Enabled = mnPrikaziKlub.Visible = false;
                mnPrikaziDrzavu.Enabled = mnPrikaziDrzavu.Visible = false;
            }

            contextMenuStrip1.Show(grid, new Point(x, y));
        }

        private void initTab(TabPage tabPage, RasporedSudija raspored)
        {
            SpravaGridGroupUserControl spravaGridGroupUserControl = new SpravaGridGroupUserControl();
            spravaGridGroupUserControl.Location = USER_CONTROL_LOCATION;
            spravaGridGroupUserControl.SpravaGridRightClick +=
              new EventHandler<SpravaGridRightClickEventArgs>(spravaGridGroupUserControl1_SpravaGridRightClick);
            //spravaGridGroupUserControl.Size = this.rasporedSudijaUserControl1.Size;
            spravaGridGroupUserControl.init(takmicenje.Gimnastika); // odredjuje i Size
            foreach (SpravaGridUserControl c in spravaGridGroupUserControl.SpravaGridUserControls)
            {
                SudijskiOdborNaSpravi odbor = raspored.getOdbor(c.Sprava);
                int odborId = odbor != null ? odbor.Id : 0;
                GridColumnsInitializer.initRasporedSudija(odborId, c.DataGridViewUserControl);
                c.DataGridViewUserControl.DataGridView.ColumnWidthChanged += new DataGridViewColumnEventHandler(DataGridView_ColumnWidthChanged);
            }
            spravaGridGroupUserControl.TabIndex = this.spravaGridGroupUserControl1.TabIndex;

            tabPage.SuspendLayout();
            tabPage.Controls.Add(spravaGridGroupUserControl);
            tabPage.BackColor = this.tabPage1.BackColor;
            tabPage.Location = this.tabPage1.Location;
            tabPage.Padding = this.tabPage1.Padding;
            tabPage.Size = this.tabPage1.Size;
            //tabPage.TabIndex = rasporedi.IndexOf(raspored); // This property is not 
            //  meaningful for this control.
            tabPage.AutoScroll = true;
            tabPage.AutoScrollMinSize = new Size(
                spravaGridGroupUserControl.Right, spravaGridGroupUserControl.Bottom);
            tabPage.AutoScrollMargin = new Size(spravaGridGroupUserControl.Location);
            tabPage.Text = raspored.Naziv;
            //tabPage.UseVisualStyleBackColor = this.tabPage1.UseVisualStyleBackColor;
            tabPage.ResumeLayout(false);
        }

        void DataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (ActiveRaspored == null)
                return;

            DataGridView dgw = sender as DataGridView;
            foreach (SpravaGridUserControl c in getActiveSpravaGridGroupUserControl().SpravaGridUserControls)
            {
                if (c.DataGridViewUserControl.DataGridView == dgw)
                {
                    SudijskiOdborNaSpravi odbor = ActiveRaspored.getOdbor(c.Sprava);
                    if (odbor != null)
                    {
                        GridColumnsInitializer.rasporedSudijaColumnWidthChanged(odbor.Id, dgw);
                        return;
                    }
                }
            }
        }

        private void onSelectedIndexChanged()
        {
            if (ActiveRaspored == null)
            {
                // kada je izbrisan poslednji tab
                return;
            }
            
            if (tabOpened[tabControl1.SelectedIndex])
                return;

            tabOpened[tabControl1.SelectedIndex] = true;
            setRaspored(ActiveRaspored);

            // ponisti selekcije za prvo prikazivanje
            getActiveSpravaGridGroupUserControl().clearSelection();
        }

        private RasporedSudija ActiveRaspored
        {
            get
            {
                if (rasporedi.Count == 0)
                    return null;
                else
                    return rasporedi[tabControl1.SelectedIndex];
            }
        }

        private void setRaspored(RasporedSudija raspored)
        {
            SpravaGridGroupUserControl c = getActiveSpravaGridGroupUserControl();
            foreach (SudijskiOdborNaSpravi odbor in raspored.Odbori)
            {
                c[odbor.Sprava].setItems(odbor.Raspored);
            }
        }

        private SpravaGridGroupUserControl getActiveSpravaGridGroupUserControl()
        {
            foreach (Control c in tabControl1.SelectedTab.Controls)
            {
                SpravaGridGroupUserControl c2 = c as SpravaGridGroupUserControl;
                if (c2 != null)
                    return c2;
            }
            return null;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    onSelectedIndexChanged();
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
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void RasporedSudijaForm_Load(object sender, EventArgs e)
        {
            // Ponistavanje selekcija za prvi tab mora da se radi u Load eventu
            // zato sto u konstruktoru nema efekta
            if (rasporedi.Count > 0)
                getActiveSpravaGridGroupUserControl().clearSelection();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (ActiveRaspored == null)
                return;
            SelectSpravaForm form = new SelectSpravaForm(takmicenje.Gimnastika,
                getActiveSpravaGridGroupUserControl().SelectedSprava);
            if (form.ShowDialog() != DialogResult.OK)
                return;

            Sprava sprava = form.Sprava;
            if (sprava == Sprava.Undefined)
                return;

            promeniStartListuCommand(sprava);
        }

        private void promeniStartListuCommand(Sprava sprava)
        {
            try
            {
                RasporedSudijaEditorForm form = new RasporedSudijaEditorForm(
                    ActiveRaspored.Id, sprava, takmicenje.Id);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    rasporedi[tabControl1.SelectedIndex] = form.RasporedSudija;
                    refresh(sprava);

                    // za slucaj da su promenjene sirine kolona
                    GridColumnsInitializer.initRasporedSudija(ActiveRaspored.getOdbor(sprava).Id, 
                        getActiveSpravaGridGroupUserControl()[sprava].DataGridViewUserControl);

                    getActiveSpravaGridGroupUserControl()[sprava].clearSelection();
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }

        private void refresh(Sprava sprava)
        {
            SudijskiOdborNaSpravi odbor = ActiveRaspored.getOdbor(sprava);
            getActiveSpravaGridGroupUserControl()[odbor.Sprava].setItems(odbor.Raspored);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            IList<TakmicarskaKategorija> kategorije;
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    kategorije = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().FindByTakmicenje(takmicenje.Id);
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            IList<string> kategorijeStr = new List<string>();
            foreach (TakmicarskaKategorija k in kategorije)
                kategorijeStr.Add(k.Naziv);

            string msg = "Izaberite kategorije za koje vazi raspored sudija";
            CheckListForm form = new CheckListForm(kategorijeStr, new List<int>(), msg, "Kategorije", true, msg, true);
            if (form.ShowDialog() != DialogResult.OK)
                return;

            IList<TakmicarskaKategorija> selKategorije = new List<TakmicarskaKategorija>();
            foreach (int i in form.CheckedIndices)
                selKategorije.Add(kategorije[i]);

            RasporedSudija newRaspored = null;
            bool added = false;
            session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    newRaspored = new RasporedSudija(selKategorije, deoTakKod, takmicenje.Gimnastika);
                    DAOFactoryFactory.DAOFactory.GetRasporedSudijaDAO().Add(newRaspored);

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                    takmicenje.LastModified = DateTime.Now;
                    session.Transaction.Commit();
                    added = true;
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showMessage(ex.Message, this.Text);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            if (!added)
            {
                Close();
                return;
            }

            rasporedi.Add(newRaspored);
            tabOpened.Add(false);

            createTab(newRaspored);
            if (tabControl1.SelectedIndex != tabControl1.TabPages.Count - 1)
                tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;
            else
                onSelectedIndexChanged();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (ActiveRaspored == null)
                return;

            string msgFmt = "Da li zelite da izbrisete raspored sudija?";
            if (!MessageDialogs.queryConfirmation(String.Format(
                msgFmt, ""), this.Text))
                return;

            bool deleted = false;
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    DAOFactoryFactory.DAOFactory.GetRasporedSudijaDAO().Delete(ActiveRaspored);

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                    takmicenje.LastModified = DateTime.Now;
                    session.Transaction.Commit();
                    deleted = true;
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
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            if (!deleted)
                return;

            rasporedi.Remove(ActiveRaspored);

            tabOpened.RemoveAt(tabControl1.SelectedIndex);
            tabControl1.TabPages.Remove(tabControl1.SelectedTab);
        }

        private void mnPrikaziKlub_Click(object sender, EventArgs e)
        {
            promeniKlubDrzava(true);
        }

        private void mnPrikaziDrzavu_Click(object sender, EventArgs e)
        {
            promeniKlubDrzava(false);
        }

        private void promeniKlubDrzava(bool prikaziKlub)
        {
            DataGridViewUserControl dgw = getActiveSpravaGridGroupUserControl()[clickedSprava]
                .DataGridViewUserControl;
            List<SudijaUcesnik> sudije = new List<SudijaUcesnik>();
            foreach (SudijaNaSpravi s in dgw.getSelectedItems<SudijaNaSpravi>())
            {
                if (s.Sudija != null)
                    sudije.Add(s.Sudija);
            }
            if (sudije.Count == 0)
                return;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    SudijaUcesnikDAO sudijaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetSudijaUcesnikDAO();
                    foreach (SudijaUcesnik s in sudije)
                    {
                        s.NastupaZaDrzavu = !prikaziKlub;
                        sudijaUcesnikDAO.Update(s);
                    }

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                    takmicenje.LastModified = DateTime.Now;
                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showMessage(ex.Message, this.Text);
                Close();
                return;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            SudijaNaSpravi s2 = dgw.getSelectedItem<SudijaNaSpravi>();
            dgw.refreshItems();
            dgw.setSelectedItem<SudijaNaSpravi>(s2);
        }

        private void mnPromeniRasporedSudija_Click(object sender, EventArgs e)
        {
            if (ActiveRaspored == null)
                return;
            if (clickedSprava == Sprava.Undefined)
                return;

            promeniStartListuCommand(clickedSprava);
        }

        private void btnStampaj_Click(object sender, EventArgs e)
        {
            if (ActiveRaspored == null)
                return;

            string nazivIzvestaja;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                nazivIzvestaja = "Raspored sudija - kvalifikacije";
            }
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje2)
            {
                nazivIzvestaja = "Raspored sudija - finale viseboja";
            }
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
            {
                nazivIzvestaja = "Raspored sudija - finale po spravama";
            }
            else
            {
                nazivIzvestaja = "Raspored sudija - finale ekipno";
            }
            string kategorija = ActiveRaspored.Naziv;

            HeaderFooterForm form = new HeaderFooterForm(deoTakKod, false, true, false, false, false, false, false);
            if (!Opcije.Instance.HeaderFooterInitialized)
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);

                string mestoDatum = takmicenje.Mesto + "  "
                    + takmicenje.Datum.ToShortDateString();
                form.Header1Text = takmicenje.Naziv;
                form.Header2Text = mestoDatum;
                form.Header3Text = nazivIzvestaja;
                form.Header4Text = kategorija;
                form.FooterText = mestoDatum;
            }
            else
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);
                form.Header3Text = nazivIzvestaja;
                form.Header4Text = kategorija;
            }

            if (form.ShowDialog() != DialogResult.OK)
                return;
            FormUtil.initHeaderFooterFromForm(form);
            Opcije.Instance.HeaderFooterInitialized = true;

            Sprava sprava = Sprava.Undefined;
            if (!form.StampajSveSprave)
            {
                SelectSpravaForm form2 = new SelectSpravaForm(takmicenje.Gimnastika,
                    getActiveSpravaGridGroupUserControl().SelectedSprava);
                if (form2.ShowDialog() != DialogResult.OK)
                    return;

                sprava = form2.Sprava;
                if (sprava == Sprava.Undefined)
                    return;
            }

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                PreviewDialog p = new PreviewDialog();
                string documentName = nazivIzvestaja + kategorija;

                if (form.StampajSveSprave)
                {
                    List<SudijskiOdborNaSpravi> odbori = new List<SudijskiOdborNaSpravi>();
                    foreach (Sprava s in Sprave.getSprave(takmicenje.Gimnastika))
                    {
                        odbori.Add(ActiveRaspored.getOdbor(s));
                    }
                    p.setIzvestaj(new RasporedSudijaIzvestaj(odbori, takmicenje.Gimnastika, documentName,
                        form.BrojSpravaPoStrani, getActiveSpravaGridGroupUserControl()));
                }
                else
                {
                    SudijskiOdborNaSpravi odbor = ActiveRaspored.getOdbor(sprava);
                    p.setIzvestaj(new RasporedSudijaIzvestaj(odbor, documentName,
                    getActiveSpravaGridGroupUserControl()[sprava].DataGridViewUserControl.DataGridView));
                }

                p.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }
        }
    }
}