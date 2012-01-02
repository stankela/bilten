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
    public partial class StartListeForm : Form
    {
        private int takmicenjeId;
        private DeoTakmicenjaKod deoTakKod;
        private IList<RasporedNastupa> rasporedi;
        private List<bool> tabOpened;
        private IDataContext dataContext;
        private int kategorijeCount;
    
        // grupe i rotacije combo boxeva, indeksirane po tabovima
        private List<int> grupa;
        private List<int> rot;

        private int clickedRow;
        private int clickedColumn;
        private Sprava clickedSprava;
        private Point USER_CONTROL_LOCATION = new Point(10, 10);

        public StartListeForm(int takmicenjeId, DeoTakmicenjaKod deoTakKod)
        {
            InitializeComponent();
            this.takmicenjeId = takmicenjeId;
            this.deoTakKod = deoTakKod;

            Text = "Start liste - " +
                DeoTakmicenjaKodovi.toString(deoTakKod);

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
                grupa = new List<int>();
                rot = new List<int>();
                for (int i = 0; i < rasporedi.Count; i++)
                {
                    tabOpened.Add(false);
                    grupa.Add(0);
                    rot.Add(0);
                }

                // show first tab
                if (rasporedi.Count > 0)
                {
                    if (tabControl1.SelectedIndex != 0)
                        tabControl1.SelectedIndex = 0;
                    else
                        onSelectedTabIndexChanged();
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
            }
        }

        private IList<RasporedNastupa> loadRasporedi(int takmicenjeId,
            DeoTakmicenjaKod kod)
        {
            return dataContext.ExecuteNamedQuery<RasporedNastupa>(
                "FindRaspNastByTakDeoTakmFetch",
                new string[] { "takmicenje", "deoTak" },
                new object[] { takmicenjeId, (byte)kod });
        }

        private int getKategorijeCount(int takmicenjeId)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje.Id", CriteriaOperator.Equal, takmicenjeId));
            return dataContext.GetCount<TakmicarskaKategorija>(q);
        }

        private void createTab(RasporedNastupa raspored)
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
                    GridColumnsInitializer.initStartListaRotacija(c.DataGridViewUserControl);
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
                contextMenuStrip1.Show(grid, new Point(x, y));
            }
        }

        private string getTabText(RasporedNastupa rasporedNastupa)
        {
            // TODO: Obradi slucaj kada raspored vazi za vise kategorija
            List<TakmicarskaKategorija> kategorije =
                new List<TakmicarskaKategorija>(rasporedNastupa.Kategorije);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(TakmicarskaKategorija))["RedBroj"];
            kategorije.Sort(new SortComparer<TakmicarskaKategorija>(
                propDesc, ListSortDirection.Ascending));

            if (kategorije.Count == 0)
                return String.Empty;
            else
                return kategorije[0].ToString();
        }

        private void initTab(TabPage tabPage, RasporedNastupa raspored)
        {
            // TODO: Kod u ovom metodu je prekopiran iz Designer.cs fajla (plus ono
            // sto sam dodao u createTab metodu). Proveri da li je u Designer.cs fajlu
            // nesto menjano, i ako jeste promeni ovde.
            SpravaGridGroupUserControl spravaGridGroupUserControl = new SpravaGridGroupUserControl();
            spravaGridGroupUserControl.Location = USER_CONTROL_LOCATION;
            spravaGridGroupUserControl.SpravaGridRightClick +=
                new EventHandler<SpravaGridRightClickEventArgs>(spravaGridGroupUserControl1_SpravaGridRightClick);
            //spravaGridGroupUserControl.Size = this.rasporedSudijaUserControl1.Size;
            spravaGridGroupUserControl.init(raspored.Pol); // odredjuje i Size
            foreach (SpravaGridUserControl c in spravaGridGroupUserControl.SpravaGridUserControls)
            {
                GridColumnsInitializer.initStartListaRotacija(c.DataGridViewUserControl);
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
            tabPage.Text = getTabText(raspored);
            //tabPage.UseVisualStyleBackColor = this.tabPage1.UseVisualStyleBackColor;
            tabPage.ResumeLayout(false);
        }

        void cmbRotacija_SelectedIndexChanged(object sender, EventArgs e)
        {
            onRotacijaChanged();
        }

        void cmbGrupa_SelectedIndexChanged(object sender, EventArgs e)
        {
            onRotacijaChanged();
        }

        private void initCombos(int brojGrupa, int brojRotacija)
        {
            disableComboHandlers();
            
            cmbGrupa.Items.Clear();
            for (int i = 1; i <= brojGrupa; i++)
                cmbGrupa.Items.Add(i);
            cmbGrupa.SelectedIndex = 0;

            cmbRotacija.Items.Clear();
            for (int i = 1; i <= brojRotacija; i++)
                cmbRotacija.Items.Add(i);
            cmbRotacija.SelectedIndex = 0;

            enableComboHandlers();
        }

        private void onSelectedTabIndexChanged()
        {
            if (ActiveRaspored == null)
            {
                // kada je izbrisan poslednji tab
                return;
            }

            int brojGrupa = ActiveRaspored.getBrojGrupa();
            if (brojGrupa == 0)
                brojGrupa = 1;
            int brojRotacija = 4;
            if (ActiveRaspored.Pol == Pol.Muski)
                brojRotacija = 6;
            initCombos(brojGrupa, brojRotacija);

            if (tabOpened[tabControl1.SelectedIndex])
            {
                // tab je vec otvaran.
                updateCombos(ActiveGrupa, ActiveRotacija);
                return;
            }

            tabOpened[tabControl1.SelectedIndex] = true;

            int g = 1;
            int r = 1;
            grupa[tabControl1.SelectedIndex] = g;
            rot[tabControl1.SelectedIndex] = r;
            updateCombos(g, r);

            setStartListe(ActiveRaspored, g, r);

            // ponisti selekcije za prvo prikazivanje
            getActiveSpravaGridGroupUserControl().clearSelection();
        }

        private RasporedNastupa ActiveRaspored
        {
            get
            {
                if (rasporedi.Count == 0)
                    return null;
                else
                    return rasporedi[tabControl1.SelectedIndex];
            }
        }

        private void updateCombos(int g, int r)
        {
            disableComboHandlers();

            cmbGrupa.SelectedItem = g;
            cmbRotacija.SelectedItem = r;

            enableComboHandlers();
        }

        private void disableComboHandlers()
        {
            cmbGrupa.SelectedIndexChanged -= cmbGrupa_SelectedIndexChanged;
            cmbRotacija.SelectedIndexChanged -= cmbRotacija_SelectedIndexChanged;
        }

        private void enableComboHandlers()
        {
            cmbGrupa.SelectedIndexChanged += cmbGrupa_SelectedIndexChanged;
            cmbRotacija.SelectedIndexChanged += cmbRotacija_SelectedIndexChanged;
        }

        private int ActiveGrupa
        {
            get { return grupa[tabControl1.SelectedIndex];}
        }

        private int ActiveRotacija
        {
            get { return rot[tabControl1.SelectedIndex]; }
        }

        private void setStartListe(RasporedNastupa raspored, int g, int r)
        {
            SpravaGridGroupUserControl c = getActiveSpravaGridGroupUserControl();
            foreach (SpravaGridUserControl c2 in c.SpravaGridUserControls)
                c2.clearItems();
            foreach (StartListaNaSpravi s in raspored.getStartListe(g, r))
                c[s.Sprava].setItems(s.Nastupi);
        }

        private SpravaGridGroupUserControl getActiveSpravaGridGroupUserControl()
        {
            if (ActiveRaspored == null)
                return null;
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
                onSelectedTabIndexChanged();

        //        dataContext.Commit();
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

        private void onRotacijaChanged()
        {
            if (ActiveRaspored == null)
                return;

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                handleRotacijaChanged();
                
                dataContext.Clear();
                dataContext.Commit();
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

        private void handleRotacijaChanged()
        {
            int g = (int)cmbGrupa.SelectedItem;
            int r = (int)cmbRotacija.SelectedItem;
            grupa[tabControl1.SelectedIndex] = g;
            rot[tabControl1.SelectedIndex] = r;

            setStartListe(ActiveRaspored, g, r);

            // ponisti selekcije za prvo prikazivanje
            getActiveSpravaGridGroupUserControl().clearSelection();
        }

        private void StartListeForm_Load(object sender, EventArgs e)
        {
            // Ponistavanje selekcija za prvi tab mora da se radi u Load eventu
            // zato sto u konstruktoru nema efekta
            if (ActiveRaspored != null)
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

            try
            {
                StartListaRotEditorForm form2 = new StartListaRotEditorForm(
                    ActiveRaspored.Id, sprava, ActiveGrupa, ActiveRotacija, takmicenjeId);
                if (form2.ShowDialog() == DialogResult.OK)
                {
                    rasporedi[tabControl1.SelectedIndex] = form2.RasporedNastupa;
                    refresh(sprava);
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }

        private void refresh(Sprava sprava)
        {
            StartListaNaSpravi s = ActiveRaspored.getStartLista(sprava, ActiveGrupa, ActiveRotacija);
            getActiveSpravaGridGroupUserControl()[s.Sprava].setItems(s.Nastupi);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNewGroup_Click(object sender, EventArgs e)
        {
            // TODO: Dodaj brisanje grupa (razmisli da li samo poslednje ili bilo koje)
            // Takodje, kada se promeni grupa, proveri da li je grupa ostala prazna i
            // pitaj da li treba da se izbrise

            if (ActiveRaspored == null)
                return;
            string msg = "Da li zelite da dodate novu grupu?";
            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;
            if (!ActiveRaspored.canAddNewGrupa())
            {
                string fmt = "Nije moguce dodati novu grupu zato sto je poslednja " +
                    "grupa (grupa {0}) prazna.";
                MessageDialogs.showMessage(
                    String.Format(fmt, ActiveRaspored.getBrojGrupa()), this.Text);
                return;
            }

            bool added = false;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                ActiveRaspored.addNewGrupa();
                dataContext.Save(ActiveRaspored);

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
            
            int brojGrupa = ActiveRaspored.getBrojGrupa();
            cmbGrupa.Items.Add(brojGrupa);

            updateCombos(brojGrupa, 1);
            onRotacijaChanged();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            IList<TakmicarskaKategorija> dodeljeneKategorije = getDodeljeneKategorije();
            if (dodeljeneKategorije.Count == kategorijeCount)
            {
                MessageDialogs.showMessage(
                    "Vec su odredjene start liste za sve kategorije.", this.Text);
                return;
            }

            string msg = "Izaberite kategorije za koje vazi start lista";
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

            RasporedNastupa newRaspored = null;
            bool added = false;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                newRaspored = new RasporedNastupa(form.SelektovaneKategorije, deoTakKod);
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
            grupa.Add(0);
            rot.Add(0);

            createTab(newRaspored);
            if (tabControl1.SelectedIndex != tabControl1.TabPages.Count - 1)
                tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;
            else
                onSelectedTabIndexChanged();
        }

        private IList<TakmicarskaKategorija> getDodeljeneKategorije()
        {
            List<TakmicarskaKategorija> result = new List<TakmicarskaKategorija>();
            foreach (RasporedNastupa r in rasporedi)
                result.AddRange(r.Kategorije);
            return result;
        }

        private void btnSablon_Click(object sender, EventArgs e)
        {
            if (ActiveRaspored == null)
                return;

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                SablonRasporedaNastupaTakm1 sablon = findSablon(
                    ActiveRaspored, (byte)ActiveGrupa);
                Nullable<int> sablonId = null;
                if (sablon != null)
                    sablonId = sablon.Id;
                SablonRasporedaNastupaTakm1Form f = new SablonRasporedaNastupaTakm1Form(
                    sablonId, ActiveRaspored, (byte)ActiveGrupa);
                if (f.ShowDialog() != DialogResult.OK)
                    return;

                dataContext.Commit();
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

        private SablonRasporedaNastupaTakm1 findSablon(RasporedNastupa raspored,
            byte grupa)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("RasporedNastupa", CriteriaOperator.Equal, raspored));
            q.Criteria.Add(new Criterion("Grupa", CriteriaOperator.Equal, grupa));
            q.Operator = QueryOperator.And;
            IList<SablonRasporedaNastupaTakm1> result = 
                dataContext.GetByCriteria<SablonRasporedaNastupaTakm1>(q);
            if (result.Count == 0)
                return null;
            else
                return result[0];
        }

        private IList<SablonRasporedaNastupaTakm1> findSabloni(RasporedNastupa raspored)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("RasporedNastupa", CriteriaOperator.Equal, raspored));
            return dataContext.GetByCriteria<SablonRasporedaNastupaTakm1>(q);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (ActiveRaspored == null)
                return;

            string msgFmt = "Da li zelite da izbrisete start listu?";
            if (!MessageDialogs.queryConfirmation(String.Format(
                msgFmt, ""), this.Text))
                return;

            bool deleted = false;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                IList<SablonRasporedaNastupaTakm1> sabloni = findSabloni(ActiveRaspored);
                foreach (SablonRasporedaNastupaTakm1 s in sabloni)
                {
                    dataContext.Delete(s);
                }
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
            grupa.RemoveAt(tabControl1.SelectedIndex);
            rot.RemoveAt(tabControl1.SelectedIndex);

            tabControl1.TabPages.Remove(tabControl1.SelectedTab);

        }

        private void StartListeForm_Shown(object sender, EventArgs e)
        {
            if (ActiveRaspored != null)
                getActiveSpravaGridGroupUserControl().Focus();
        }

        private void cmbGrupa_DropDownClosed(object sender, EventArgs e)
        {
            if (ActiveRaspored != null)
                getActiveSpravaGridGroupUserControl().Focus();
        }

        private void cmbRotacija_DropDownClosed(object sender, EventArgs e)
        {
            if (ActiveRaspored != null)
                getActiveSpravaGridGroupUserControl().Focus();
        }

        private void mnUnesiOcenu_Click(object sender, EventArgs e)
        {
            SpravaGridUserControl c = 
                getActiveSpravaGridGroupUserControl()[clickedSprava];
            NastupNaSpravi nastup = c.getSelectedItem<NastupNaSpravi>();
            if (nastup == null)
                return;

            Ocena ocena = null;
            GimnasticarUcesnik g = null;
            bool ok = false;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                g = nastup.Gimnasticar;
                ocena = findOcena(g, deoTakKod, clickedSprava);
                
                ok = true;
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

            if (!ok)
            {
                Close();
                return;
            }

            Nullable<int> ocenaId = null;
            if (ocena != null)
                ocenaId = ocena.Id;
            OcenaForm f = new OcenaForm(ocenaId, g, clickedSprava, deoTakKod, 
                takmicenjeId);
            f.ShowDialog();
        }

        private Ocena findOcena(GimnasticarUcesnik g, DeoTakmicenjaKod deoTakKod, 
            Sprava sprava)
        {
            IList<Ocena> result = dataContext.
                ExecuteNamedQuery<Ocena>("FindOcena",
                new string[] { "gimnasticarId", "deoTakKod", "sprava" },
                new object[] { g.Id, deoTakKod, sprava });
            if (result.Count > 0)
                return result[0];
            else
                return null;
        }
    }
}