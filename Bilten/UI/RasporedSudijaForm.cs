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

namespace Bilten.UI
{
    public partial class RasporedSudijaForm : Form
    {
        private int takmicenjeId;
        private DeoTakmicenjaKod deoTakKod;
        private IList<RasporedSudija> rasporedi;
        private List<bool> tabOpened;
        private IDataContext dataContext;
        private int kategorijeCount;

        private int clickedRow;
        private int clickedColumn;
        private Sprava clickedSprava;
        private Point USER_CONTROL_LOCATION = new Point(10, 10);

        public RasporedSudijaForm(int takmicenjeId, DeoTakmicenjaKod deoTakKod)
        {
            InitializeComponent();
            this.ClientSize = new System.Drawing.Size(1150, 540);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.takmicenjeId = takmicenjeId;
            this.deoTakKod = deoTakKod;

            this.Text = "Raspored sudija - " +
                DeoTakmicenjaKodovi.toString(deoTakKod);

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                kategorijeCount = getKategorijeCount(takmicenjeId);
                if (kategorijeCount == 0)
                    throw new Exception("Greska u programu.");
                rasporedi = loadRasporedi(takmicenjeId, deoTakKod);

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

            //    dataContext.Commit();
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

                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }
        }

        private IList<RasporedSudija> loadRasporedi(int takmicenjeId,
            DeoTakmicenjaKod kod)
        {
            return dataContext.ExecuteNamedQuery<RasporedSudija>(
                "FindRaspSudByTakDeoTakmFetch",
                new string[] { "takmicenje", "deoTak" },
                new object[] { takmicenjeId, (byte)kod });
        }

        private int getKategorijeCount(int takmicenjeId)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje.Id", CriteriaOperator.Equal, takmicenjeId));
            return dataContext.GetCount<TakmicarskaKategorija>(q);
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
                spravaGridGroupUserControl1.init(rasporedi[0].Pol);
                foreach (SpravaGridUserControl c in spravaGridGroupUserControl1.SpravaGridUserControls)
                {
                    GridColumnsInitializer.initRasporedSudija(c.DataGridViewUserControl);
                }
                tabPage1.AutoScroll = true;
                tabPage1.AutoScrollMinSize = new Size(
                    spravaGridGroupUserControl1.Right, spravaGridGroupUserControl1.Bottom);
                tabPage1.AutoScrollMargin =
                    new Size(spravaGridGroupUserControl1.Location);
                tabPage1.Text = getTabText(raspored);
            }
            else
            {
                // init other tabs
                TabPage newTab = new TabPage();
                tabControl1.Controls.Add(newTab);
                initTab(rasporedi.Count - 1, newTab, raspored);
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

        private string getTabText(RasporedSudija rasporedSudija)
        {
            // TODO: Obradi slucaj kada raspored vazi za vise kategorija
            List<TakmicarskaKategorija> kategorije =
                new List<TakmicarskaKategorija>(rasporedSudija.Kategorije);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(TakmicarskaKategorija))["RedBroj"];
            kategorije.Sort(new SortComparer<TakmicarskaKategorija>(
                propDesc, ListSortDirection.Ascending));

            if (kategorije.Count == 0)
                return String.Empty;
            else
                return kategorije[0].ToString();
        }

        private void initTab(int tabIndex, TabPage tabPage, RasporedSudija raspored)
        {
            // TODO: Kod u ovom metodu je prekopiran iz Designer.cs fajla (plus ono
            // sto sam dodao u initTabs metodu). Proveri da li je u Designer.cs fajlu
            // nesto menjano, i ako jeste promeni ovde.
            SpravaGridGroupUserControl spravaGridGroupUserControl = new SpravaGridGroupUserControl();
            spravaGridGroupUserControl.Location = USER_CONTROL_LOCATION;
            spravaGridGroupUserControl.SpravaGridRightClick +=
              new EventHandler<SpravaGridRightClickEventArgs>(spravaGridGroupUserControl1_SpravaGridRightClick);
            //spravaGridGroupUserControl.Size = this.rasporedSudijaUserControl1.Size;
            spravaGridGroupUserControl.init(raspored.Pol); // odredjuje i Size
            foreach (SpravaGridUserControl c in spravaGridGroupUserControl.SpravaGridUserControls)
            {
                GridColumnsInitializer.initRasporedSudija(c.DataGridViewUserControl);
            }
            spravaGridGroupUserControl.TabIndex = this.spravaGridGroupUserControl1.TabIndex;

            tabPage.SuspendLayout();
            tabPage.Controls.Add(spravaGridGroupUserControl);
            tabPage.BackColor = this.tabPage1.BackColor;
            tabPage.Location = this.tabPage1.Location;
            tabPage.Padding = this.tabPage1.Padding;
            tabPage.Size = this.tabPage1.Size;
            tabPage.TabIndex = tabIndex;
            tabPage.AutoScroll = true;
            tabPage.AutoScrollMinSize = new Size(
                spravaGridGroupUserControl.Right, spravaGridGroupUserControl.Bottom);
            tabPage.AutoScrollMargin = new Size(spravaGridGroupUserControl.Location);
            tabPage.Text = getTabText(raspored);
            //tabPage.UseVisualStyleBackColor = this.tabPage1.UseVisualStyleBackColor;
            tabPage.ResumeLayout(false);
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
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();
                onSelectedIndexChanged();

                //dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                Close();
                return;
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
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
            SelectSpravaForm form = new SelectSpravaForm(ActiveRaspored.Pol,
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
                    ActiveRaspored.Id, sprava, takmicenjeId);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    rasporedi[tabControl1.SelectedIndex] = form.RasporedSudija;
                    refresh(sprava);
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
            IList<TakmicarskaKategorija> dodeljeneKategorije = getDodeljeneKategorije();
            if (dodeljeneKategorije.Count == kategorijeCount)
            {
                MessageDialogs.showMessage(
                    "Vec su odredjeni rasporedi sudija za sve kategorije.", this.Text);
                return;
            }

            string msg = "Izaberite kategorije za koje vazi raspored sudija";
            DialogResult dlgResult = DialogResult.None;
            SelectKategorijaForm form = null;
            try
            {
                form = new SelectKategorijaForm(takmicenjeId, dodeljeneKategorije, 
                    false, msg);
                dlgResult = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }
            
            if (dlgResult != DialogResult.OK || form.SelektovaneKategorije.Count == 0)
                return;

            RasporedSudija newRaspored = null;
            bool added = false;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                Takmicenje takmicenje = dataContext.GetById<Takmicenje>(takmicenjeId);
                newRaspored = new RasporedSudija(form.SelektovaneKategorije, deoTakKod,
                    takmicenje);
                dataContext.Add(newRaspored);

                dataContext.Commit();
                added = true;
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showMessage(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
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

        private IList<TakmicarskaKategorija> getDodeljeneKategorije()
        {
            List<TakmicarskaKategorija> result = new List<TakmicarskaKategorija>();
            foreach (RasporedSudija r in rasporedi)
                result.AddRange(r.Kategorije);
            return result;
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
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                dataContext.Delete(ActiveRaspored);

                dataContext.Commit();
                deleted = true;
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                Close();
                return;
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
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

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                foreach (SudijaUcesnik s in sudije)
                {
                    s.NastupaZaDrzavu = !prikaziKlub;
                    dataContext.Save(s);
                }
                dataContext.Commit();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showMessage(Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                Close();
                return;
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
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

    }
}