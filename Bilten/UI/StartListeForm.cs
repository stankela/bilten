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
using Bilten.Report;
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;
using Bilten.Util;

namespace Bilten.UI
{
    public partial class StartListeForm : Form
    {
        private Takmicenje takmicenje;
        private DeoTakmicenjaKod deoTakKod;
        private IList<RasporedNastupa> rasporedi;
        private List<bool> tabOpened;
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
            this.deoTakKod = deoTakKod;

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
                        throw new BusinessException(Strings.NO_KATEGORIJE_I_TAKMICENJA_ERROR_MSG);

                    rasporedi = DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO()
                        .FindByTakmicenjeDeoTak(takmicenjeId, deoTakKod);

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                    NHibernateUtil.Initialize(takmicenje);

                    initUI();

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
                }
            }
            catch (BusinessException)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw;
            }
            catch (InfrastructureException)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw;
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

        private void initUI()
        {
            Text = "Start liste - " +
                DeoTakmicenjaKodovi.toString(deoTakKod);
            this.ClientSize = new Size(this.ClientSize.Width, 540);
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje3 || takmicenje.FinaleKupa)
            {
                cmbRotacija.Enabled = false;
                btnOstaleRotacije.Text = "Kreiraj na osnovu kvalifikanata";
            }
            mnRotirajEkipeRotirajGim.Checked = true;
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
                spravaGridGroupUserControl1.init(rasporedi[0].Gimnastika);
                foreach (SpravaGridUserControl c in spravaGridGroupUserControl1.SpravaGridUserControls)
                {
                    c.DataGridViewUserControl.DataGridView.CellFormatting +=
                        new DataGridViewCellFormattingEventHandler(DataGridView_CellFormatting);
                    c.DataGridViewUserControl.DataGridView.ColumnWidthChanged +=
                        new DataGridViewColumnEventHandler(DataGridView_ColumnWidthChanged);
                    c.DataGridViewUserControl.DataGridView.KeyDown += DataGridView_KeyDown;
                    c.DataGridViewUserControl.DataGridView.MouseDoubleClick += DataGridView_MouseDoubleClick;
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

        void DataGridView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DataGridView dgw = sender as DataGridView;
            foreach (SpravaGridUserControl c in getActiveSpravaGridGroupUserControl().SpravaGridUserControls)
            {
                if (c.DataGridViewUserControl.DataGridView == dgw)
                {
                    unesiOcenu(c, false);
                    return;
                }
            }
        }

        void DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            DataGridView dgw = sender as DataGridView;
            foreach (SpravaGridUserControl c in getActiveSpravaGridGroupUserControl().SpravaGridUserControls)
            {
                if (c.DataGridViewUserControl.DataGridView == dgw)
                {
                    unesiOcenu(c, true);
                    return;
                }
            }
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
                    StartListaNaSpravi startLista = ActiveRaspored.getStartLista(c.Sprava, ActiveGrupa, ActiveRotacija);
                    if (startLista != null)
                    {
                        GridColumnsInitializer.startListaColumnWidthChanged(startLista.Id, dgw);
                        return;
                    }
                }
            }
        }

        // TODO: Trenutno se meni otvara samo kada se klikne unutar sprave. Promeni da se meni otvara kada se klikne bilo gde.
        // (isto i u rasporedu sudija)
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
                int selCount = getActiveSpravaGridGroupUserControl()[clickedSprava]
                    .DataGridViewUserControl.getSelectedItems<NastupNaSpravi>().Count;
                mnUnesiOcenu.Enabled = selCount == 1;
                mnPrikaziKlub.Enabled = mnPrikaziKlub.Visible = true;
                mnPrikaziDrzavu.Enabled = mnPrikaziDrzavu.Visible = true;
                getActiveSpravaGridGroupUserControl()[clickedSprava]
                    .DataGridViewUserControl.setSelectedItemIndex(clickedRow);
            }
            else
            {
                mnUnesiOcenu.Enabled = false;
                mnPrikaziKlub.Enabled = mnPrikaziKlub.Visible = false;
                mnPrikaziDrzavu.Enabled = mnPrikaziDrzavu.Visible = false;
            }

            bool enableNacinRotacije = deoTakKod == DeoTakmicenjaKod.Takmicenje1
                                       && ActiveRotacija == 1
                                       && takmicenje.TipTakmicenja == TipTakmicenja.StandardnoTakmicenje;
            mnOznaciKaoEkipu.Enabled = enableNacinRotacije;
            mnOznaciKaoPojedinca.Enabled = enableNacinRotacije;
            mnNacinRotacije.Enabled = enableNacinRotacije;

            if (enableNacinRotacije)
            {
                StartListaNaSpravi startLista = ActiveRaspored.getStartLista(clickedSprava, ActiveGrupa, ActiveRotacija);

                mnRotirajEkipeRotirajGim.Checked = startLista.NacinRotacije == NacinRotacije.RotirajEkipeRotirajGimnasticare;
                mnNeRotirajEkipeRotirajGim.Checked = startLista.NacinRotacije == NacinRotacije.NeRotirajEkipeRotirajGimnasticare;
                mnRotirajSve.Checked = startLista.NacinRotacije == NacinRotacije.RotirajSve;
                mnNeRotirajNista.Checked = startLista.NacinRotacije == NacinRotacije.NeRotirajNista;
            }

            if (deoTakKod == DeoTakmicenjaKod.Takmicenje2)
            {
                mnRezultatiSprave.Enabled = false;
                mnRezultatiSprave.Visible = false;
                mnRezultatiEkipno.Enabled = false;
                mnRezultatiEkipno.Visible = false;
            }
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
            {
                mnRezultatiViseboj.Enabled = false;
                mnRezultatiViseboj.Visible = false;
                mnRezultatiEkipno.Enabled = false;
                mnRezultatiEkipno.Visible = false;
            }
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje4)
            {
                mnRezultatiViseboj.Enabled = false;
                mnRezultatiViseboj.Visible = false;
                mnRezultatiSprave.Enabled = false;
                mnRezultatiSprave.Visible = false;
            }
            contextMenuStrip1.Show(grid, new Point(x, y));
        }

        private string getTabText(RasporedNastupa rasporedNastupa)
        {
            List<TakmicarskaKategorija> kategorije =
                new List<TakmicarskaKategorija>(rasporedNastupa.Kategorije);

            if (kategorije.Count == 0)
                return String.Empty;

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(TakmicarskaKategorija))["RedBroj"];
            kategorije.Sort(new SortComparer<TakmicarskaKategorija>(
                propDesc, ListSortDirection.Ascending));

            string retValue = kategorije[0].ToString();
            for (int i = 1; i < kategorije.Count; i++)
                retValue = retValue + ", " + kategorije[i].ToString();
            return retValue;
        }

        private string getFirstKategorijaText(RasporedNastupa rasporedNastupa)
        {
            List<TakmicarskaKategorija> kategorije =
                new List<TakmicarskaKategorija>(rasporedNastupa.Kategorije);

            if (kategorije.Count == 0)
                return String.Empty;

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(TakmicarskaKategorija))["RedBroj"];
            kategorije.Sort(new SortComparer<TakmicarskaKategorija>(
                propDesc, ListSortDirection.Ascending));

            string retValue = kategorije[0].ToString();
            //for (int i = 1; i < kategorije.Count; i++)
              //  retValue = retValue + ", " + kategorije[i].ToString();
            return retValue;
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
            spravaGridGroupUserControl.init(raspored.Gimnastika); // odredjuje i Size
            foreach (SpravaGridUserControl c in spravaGridGroupUserControl.SpravaGridUserControls)
            {
                c.DataGridViewUserControl.DataGridView.CellFormatting +=
                    new DataGridViewCellFormattingEventHandler(DataGridView_CellFormatting);
                c.DataGridViewUserControl.DataGridView.ColumnWidthChanged +=
                    new DataGridViewColumnEventHandler(DataGridView_ColumnWidthChanged);
                c.DataGridViewUserControl.DataGridView.KeyDown += DataGridView_KeyDown;
                c.DataGridViewUserControl.DataGridView.MouseDoubleClick += DataGridView_MouseDoubleClick;
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
            if (ActiveRaspored.Gimnastika == Gimnastika.MSG)
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

            updateGridColumnWidths();

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
            get
            {
                if (grupa.Count != 0)
                    return grupa[tabControl1.SelectedIndex];
                return 0;
            }
        }

        private int ActiveRotacija
        {
            get
            {
                if (rot.Count != 0)
                    return rot[tabControl1.SelectedIndex];
                return 0;
            }
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
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    onSelectedTabIndexChanged();
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

        private void onRotacijaChanged()
        {
            if (ActiveRaspored == null)
                return;

            handleRotacijaChanged();
        }

        private void handleRotacijaChanged()
        {
            int g = (int)cmbGrupa.SelectedItem;
            int r = (int)cmbRotacija.SelectedItem;
            grupa[tabControl1.SelectedIndex] = g;
            rot[tabControl1.SelectedIndex] = r;

            setStartListe(ActiveRaspored, g, r);

            updateGridColumnWidths();

            // ponisti selekcije za prvo prikazivanje
            getActiveSpravaGridGroupUserControl().clearSelection();
        }

        private void updateGridColumnWidths()
        {
            foreach (SpravaGridUserControl c in getActiveSpravaGridGroupUserControl().SpravaGridUserControls)
            {
                StartListaNaSpravi startLista = ActiveRaspored.getStartLista(c.Sprava, ActiveGrupa, ActiveRotacija);
                int startListaId = startLista != null ? startLista.Id : 0;
                GridColumnsInitializer.initStartLista(startListaId, c.DataGridViewUserControl);
            }
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
            SelectSpravaForm form = new SelectSpravaForm(ActiveRaspored.Gimnastika,
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
                StartListaRotEditorForm form = new StartListaRotEditorForm(
                    ActiveRaspored.Id, sprava, ActiveGrupa, ActiveRotacija, takmicenje.Id, bojeZaEkipe);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    rasporedi[tabControl1.SelectedIndex] = form.RasporedNastupa;
                    refresh(sprava);

                    // za slucaj da su promenjene sirine kolona
                    GridColumnsInitializer.initStartLista(
                        ActiveRaspored.getStartLista(sprava, ActiveGrupa, ActiveRotacija).Id,
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
            string msg = "Da li zelite da dodate nov turnus?";
            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;
            if (!ActiveRaspored.canAddNewGrupa())
            {
                string fmt = "Nije moguce dodati nov turnus zato sto je poslednji " +
                    "turnus (turnus {0}) prazan.";
                MessageDialogs.showMessage(
                    String.Format(fmt, ActiveRaspored.getBrojGrupa()), this.Text);
                return;
            }

            bool added = false;
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    ActiveRaspored.addNewGrupa();
                    DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO().Update(ActiveRaspored);

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
                // TODO4: Izbrisi SelectKategorijaForm i zameni ga sa CheckListForm
                form = new SelectKategorijaForm(takmicenje.Id, takmicenje.Gimnastika, dodeljeneKategorije, msg);
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
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    newRaspored = new RasporedNastupa(form.SelektovaneKategorije, deoTakKod);
                    DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO().Add(newRaspored);

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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (ActiveRaspored == null)
                return;

            string msgFmt = "Da li zelite da izbrisete start listu?";
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
                    DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO().Delete(ActiveRaspored);

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
            unesiOcenu(getActiveSpravaGridGroupUserControl()[clickedSprava], false);
        }

        private void unesiOcenu(SpravaGridUserControl c, bool openedWithEnter)
        {
            DataGridViewSelectedRowCollection selRows = c.DataGridViewUserControl.DataGridView.SelectedRows;
            if (selRows.Count != 1)
                return;

            NastupNaSpravi nastup = selRows[0].DataBoundItem as NastupNaSpravi;
            Ocena ocena = null;
            GimnasticarUcesnik g = null;
            bool ok = false;
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    g = nastup.Gimnasticar;
                    ocena = DAOFactoryFactory.DAOFactory.GetOcenaDAO().FindOcena(g, deoTakKod, c.Sprava);

                    ok = true;
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            if (!ok)
            {
                Close();
                return;
            }

            Nullable<int> ocenaId = null;
            if (ocena != null)
                ocenaId = ocena.Id;
            OcenaForm f = new OcenaForm(ocenaId, g, c.Sprava, deoTakKod,
                takmicenje.Id);
            if (f.ShowDialog() == DialogResult.OK)
            {
                int index = c.DataGridViewUserControl.getSelectedItemIndex();
                if (openedWithEnter)
                {
                    // Enter ce automatski selektovati novu vrstu. Potrebno je samo proveriti da li je ovo poslednja vrsta.
                    if (index == c.DataGridViewUserControl.DataGridView.Rows.Count - 1)
                    {
                        // uneta je zadnja ocena
                        c.clearSelection();
                    }
                }
                else
                {
                    ++index;
                    if (index < c.DataGridViewUserControl.DataGridView.Rows.Count)
                    {
                        // selektuj sledecu ocenu
                        c.DataGridViewUserControl.setSelectedItemIndex(index);
                    }
                    else
                    {
                        // uneta je zadnja ocena
                        c.clearSelection();
                    }
                }
            }
        }

        // TODO3: Dodaj natpis ispod start lista koji prikazuje trenutno selektovan nacin rotacije za start listu.

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (ActiveRaspored == null)
                return;

            string nazivIzvestaja;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                nazivIzvestaja = "Start liste - kvalifikacije";
            }
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje2)
            {
                nazivIzvestaja = "Start liste - finale viseboja";
            }
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
            {
                nazivIzvestaja = "Start liste - finale po spravama";
            }
            else
            {
                nazivIzvestaja = "Start liste - finale ekipno";
            }
            // TODO: Verovatno bi trebalo ukljuciti i turnus, ukoliko ima vise turnusa
            string kategorijaRotacija = getFirstKategorijaText(ActiveRaspored) + ", Rotacija " + ActiveRotacija.ToString();

            HeaderFooterForm form = new HeaderFooterForm(deoTakKod, false, true, false, true, true, true, false);
            if (!Opcije.Instance.HeaderFooterInitialized)
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);

                string mestoDatum = takmicenje.Mesto + "  "
                    + takmicenje.Datum.ToShortDateString();
                form.Header1Text = takmicenje.Naziv;
                form.Header2Text = mestoDatum;
                form.Header3Text = nazivIzvestaja;
                form.Header4Text = kategorijaRotacija;
                form.FooterText = mestoDatum;
                if (takmicenje.Gimnastika == Gimnastika.ZSG)
                    form.BrojSpravaPoStrani = 4;
                else
                    form.BrojSpravaPoStrani = 6;
                form.StampajKlub = true;
                form.StampajKategoriju = true;
            }
            else
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);
                form.Header3Text = nazivIzvestaja;
                form.Header4Text = kategorijaRotacija;
            }

            if (form.ShowDialog() != DialogResult.OK)
                return;
            FormUtil.initHeaderFooterFromForm(form);
            Opcije.Instance.HeaderFooterInitialized = true;

            Sprava sprava = Sprava.Undefined;
            if (!form.StampajSveSprave)
            {
                SelectSpravaForm form2 = new SelectSpravaForm(ActiveRaspored.Gimnastika,
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
                string documentName = nazivIzvestaja + kategorijaRotacija;

                if (form.StampajSveSprave)
                {
                    List<StartListaNaSpravi> startListe = new List<StartListaNaSpravi>();
                    foreach (Sprava s in Sprave.getSprave(takmicenje.Gimnastika))
                    {
                        startListe.Add(ActiveRaspored.getStartLista(s, ActiveGrupa, ActiveRotacija));
                    }
                    p.setIzvestaj(new StartListaIzvestaj(startListe, takmicenje.Gimnastika, documentName,
                        form.BrojSpravaPoStrani, form.StampajRedniBrojNaStartListi, form.StampajKlub, form.StampajKategoriju,
                        getActiveSpravaGridGroupUserControl()));
                }
                else
                {
                    StartListaNaSpravi startLista = ActiveRaspored.getStartLista(sprava, ActiveGrupa, ActiveRotacija);
                    p.setIzvestaj(new StartListaIzvestaj(startLista, documentName, form.StampajRedniBrojNaStartListi,
                        form.StampajKlub, form.StampajKategoriju,
                        getActiveSpravaGridGroupUserControl()[sprava].DataGridViewUserControl.DataGridView));
                }

                p.ShowDialog();

                // TODO2: Dodaj godiste u start liste.

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

        private void btnOstaleRotacije_Click(object sender, EventArgs e)
        {
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                if (takmicenje.FinaleKupa)
                    kreirajNaOsnovuKvalifikanataFinaleKupa();
                else
                    kreirajPreostaleRotacije();
            }
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
                kreirajNaOsnovuKvalifikanata();
        }

        private void kreirajPreostaleRotacije()
        {
            if (ActiveRaspored == null)
                return;

            /*int finalRot = (takmicenje.Gimnastika == Gimnastika.ZSG) ? 4 : 6;
            string preostaleRot = "2-" + finalRot.ToString();
            string msgFmt = "Da li zelite da kreirate rotacije {0}? Prethodni raspored koji je postojao na rotacijama {0} " +
                "bice izbrisan.";
            if (!MessageDialogs.queryConfirmation(String.Format(msgFmt, preostaleRot), this.Text))
                return;*/

            // Nadji aktivne sprave za rotaciju 1.
            List<Sprava> aktivneSpraveRot1 = new List<Sprava>();
            foreach (Sprava s in Sprave.getSprave(takmicenje.Gimnastika))
            {
                if (ActiveRaspored.getStartLista(s, ActiveGrupa, 1).Nastupi.Count != 0)
                {
                    aktivneSpraveRot1.Add(s);
                }
            }

            SpraveNaRotacijiForm form = null;
            try
            {
                form = new SpraveNaRotacijiForm(takmicenje.Gimnastika, aktivneSpraveRot1);
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            int finalRot = (takmicenje.Gimnastika == Gimnastika.ZSG) ? 4 : 6;
            for (int rot = 2; rot <= finalRot; rot++)
            {
                kreirajRotaciju(rot, form.AktivneSprave);
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
                    Sprava[] sprave = Sprave.getSprave(takmicenje.Gimnastika);
                    for (int i = 2; i <= finalRot; i++)
                    {
                        for (int j = 0; j < sprave.Length; j++)
                        {
                            StartListaNaSpravi startLista = ActiveRaspored.getStartLista(sprave[j], ActiveGrupa, i);
                            DAOFactoryFactory.DAOFactory.GetStartListaNaSpraviDAO().Update(startLista);
                        }
                    }

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                    takmicenje.LastModified = DateTime.Now;
                    session.Transaction.Commit();

                    if (ActiveRotacija != 1)
                        setStartListe(ActiveRaspored, ActiveGrupa, ActiveRotacija);
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
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void kreirajRotaciju(int rot, List<List<Sprava>> aktivneSprave)
        {
            foreach (Sprava s in Sprave.getSprave(takmicenje.Gimnastika))
            {
                StartListaNaSpravi startLista = ActiveRaspored.getStartLista(s, ActiveGrupa, rot);
                startLista.clear();

                StartListaNaSpravi startListaPrethRot = getStartListaPrethRot(ActiveRaspored, ActiveGrupa, rot, s,
                    aktivneSprave);
                if (startListaPrethRot == null)
                    continue;

                // Nadji start listu na prvoj rotaciji
                StartListaNaSpravi currentStartLista = startListaPrethRot;
                int currentRot = rot - 1;
                while (currentRot != 1)
                {
                    currentStartLista = getStartListaPrethRot(ActiveRaspored, ActiveGrupa, currentRot--,
                        currentStartLista.Sprava, aktivneSprave);
                }

                NacinRotacije nacinRotacije = currentStartLista.NacinRotacije;
                if (startListaPrethRot.Nastupi.Count > 0)
                {
                    if (nacinRotacije == NacinRotacije.RotirajSve || nacinRotacije == NacinRotacije.NeRotirajNista)
                    {
                        foreach (NastupNaSpravi n in startListaPrethRot.Nastupi)
                        {
                            startLista.addNastup(new NastupNaSpravi(n.Gimnasticar, n.Ekipa));
                        }

                        if (nacinRotacije == NacinRotacije.RotirajSve)
                        {
                            NastupNaSpravi n2 = startLista.Nastupi[0];
                            startLista.removeNastup(n2);
                            startLista.addNastup(n2);
                        }
                    }
                    else if (nacinRotacije == NacinRotacije.RotirajEkipeRotirajGimnasticare
                             || nacinRotacije == NacinRotacije.NeRotirajEkipeRotirajGimnasticare)
                    {
                        // Najpre pronadji ekipe
                        List<List<NastupNaSpravi>> listaEkipa = new List<List<NastupNaSpravi>>();
                        int m = 0;
                        while (m < startListaPrethRot.Nastupi.Count)
                        {
                            NastupNaSpravi prethNastup = startListaPrethRot.Nastupi[m];
                            if (prethNastup.Ekipa == 0)
                            {
                                List<NastupNaSpravi> pojedinac = new List<NastupNaSpravi>();
                                pojedinac.Add(new NastupNaSpravi(prethNastup.Gimnasticar, 0));
                                listaEkipa.Add(pojedinac);
                                ++m;
                                continue;
                            }

                            List<NastupNaSpravi> novaEkipa = new List<NastupNaSpravi>();
                            int ekipaId = prethNastup.Ekipa;
                            while (m < startListaPrethRot.Nastupi.Count
                                   && prethNastup.Ekipa == ekipaId)
                            {
                                novaEkipa.Add(new NastupNaSpravi(prethNastup.Gimnasticar, prethNastup.Ekipa));
                                ++m;
                                if (m < startListaPrethRot.Nastupi.Count)
                                    prethNastup = startListaPrethRot.Nastupi[m];
                            }
                            listaEkipa.Add(novaEkipa);
                        }

                        if (nacinRotacije == NacinRotacije.RotirajEkipeRotirajGimnasticare)
                        {
                            // Rotiraj ekipe
                            List<NastupNaSpravi> prvaEkipa = listaEkipa[0];
                            listaEkipa.RemoveAt(0);
                            listaEkipa.Add(prvaEkipa);
                        }

                        foreach (List<NastupNaSpravi> ekipa in listaEkipa)
                        {
                            // Rotiraj clanove ekipe
                            NastupNaSpravi nastup = ekipa[0];
                            ekipa.RemoveAt(0);
                            ekipa.Add(nastup);

                            foreach (NastupNaSpravi n in ekipa)
                            {
                                startLista.addNastup(new NastupNaSpravi(n.Gimnasticar, n.Ekipa));
                            }
                        }
                    }
                }
            }
        }

        private StartListaNaSpravi getStartListaPrethRot(RasporedNastupa ActiveRaspored, int ActiveGrupa, int rot,
            Sprava sprava, List<List<Sprava>> aktivneSprave)
        {
            List<Sprava> sprave = aktivneSprave[rot - 1];
            int i = sprave.IndexOf(sprava);
            if (i == -1)
                // Sprava nije aktivna u rotaciji.
                return null;

            List<Sprava> prethSprave = aktivneSprave[rot - 2];

            Sprava prethSprava = (i == 0) ? prethSprave[prethSprave.Count - 1] : prethSprave[i - 1];
            StartListaNaSpravi result = ActiveRaspored.getStartLista(prethSprava, ActiveGrupa, rot - 1);
            return result;
        }

        // TODO: Ceo ovaj deo gde se kreira na osnovu kvalifikanata je radjen na brzinu, gde je jedino bilo bitno da moze da
        // se primeni na Memorijal. Trebalo bi ga temeljno proveriti i uciniti robustnijim.
        private void kreirajNaOsnovuKvalifikanata()
        {
            if (ActiveRaspored == null)
            {
                string msg2 = "Morate najpre da kreirate praznu start listu (dugme \"Nova start lista\").";
                MessageDialogs.showMessage(msg2, this.Text);
                return;
            }

            string msg = "Da li zelite da kreirate start listu na osnovu kvalifikanata?";
            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;

            List<int> zreb = parseZreb();
            if (zreb == null)
            {
                msg = "Nepravilno unesen zreb za finale.";
                MessageDialogs.showMessage(msg, this.Text);
                return;
            }
            else if (zreb.Count == 0)
            {
                msg = "Nije unesen zreb za start liste. Da li zelite da kreirate start listu bez zreba?";
                if (!MessageDialogs.queryConfirmation(msg, this.Text))
                    return;
            }

            List<TakmicarskaKategorija> kategorije = new List<TakmicarskaKategorija>(ActiveRaspored.Kategorije);
            RezultatskoTakmicenje rezTakmicenje = loadRezTakmicenje(takmicenje.Id, kategorije[0]);
            if (rezTakmicenje == null)
                return;

            Sprava[] sprave = Sprave.getSprave(takmicenje.Gimnastika);
            for (int j = 0; j < sprave.Length; j++)
            {
                StartListaNaSpravi startLista = ActiveRaspored.getStartLista(sprave[j], ActiveGrupa, 1 /*ActiveRotacija*/);
                startLista.clear();

                List<UcesnikTakmicenja3> kvalifikanti = new List<UcesnikTakmicenja3>(
                    rezTakmicenje.Takmicenje3.getUcesniciKvalifikanti(sprave[j]));
                PropertyDescriptor propDesc =
                    TypeDescriptor.GetProperties(typeof(UcesnikTakmicenja3))["QualOrder"];
                kvalifikanti.Sort(new SortComparer<UcesnikTakmicenja3>(propDesc, ListSortDirection.Ascending));

                int k = 0;
                while (k < zreb.Count)
                {
                    if (zreb[k] <= kvalifikanti.Count)
                        startLista.addNastup(new NastupNaSpravi(kvalifikanti[zreb[k] - 1].Gimnasticar, 0));
                    k++;
                }
                k = startLista.Nastupi.Count;
                while (k < kvalifikanti.Count)
                {
                    startLista.addNastup(new NastupNaSpravi(kvalifikanti[k].Gimnasticar, 0));
                    k++;
                }
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
                    for (int j = 0; j < sprave.Length; j++)
                    {
                        StartListaNaSpravi startLista = ActiveRaspored.getStartLista(sprave[j], ActiveGrupa, 1 /*ActiveRotacija*/);
                        foreach (NastupNaSpravi n in startLista.Nastupi)
                        {
                            //  potrebno za slucaj kada se u start listi nalaze i gimnasticari iz kategorija razlicitih od kategorija
                            // za koje start lista vazi.

                            // TODO3: Proveri da li ovo (tj. nedostatak ove naredbe na drugim mestima) ima veze sa time sto mi
                            // za start liste ponekad daje gresku, i zbog cega sam morao da u klasi NastupNaSpravi kesiram
                            // Kategoriju.
                            NHibernateUtil.Initialize(n.Gimnasticar.TakmicarskaKategorija);
                        }
                        DAOFactoryFactory.DAOFactory.GetStartListaNaSpraviDAO().Update(startLista);
                    }

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                    takmicenje.LastModified = DateTime.Now;
                    session.Transaction.Commit();

                    setStartListe(ActiveRaspored, ActiveGrupa, 1 /*ActiveRotacija*/);
                    getActiveSpravaGridGroupUserControl().clearSelection();
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
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private List<int> parseZreb()
        {
            string zreb = String.Empty;
            if (takmicenje.ZrebZaFinalePoSpravama != null)
                zreb = takmicenje.ZrebZaFinalePoSpravama.Trim();
            if (zreb == String.Empty)
                return new List<int>();

            List<string> parts = new List<string>();
            char delimiter = ' ';
            int index = zreb.IndexOf(delimiter);
            while (index != -1 && zreb != String.Empty)
            {
                parts.Add(zreb.Substring(0, index));
                zreb = zreb.Substring(index).Trim();
                index = zreb.IndexOf(delimiter);
            }
            if (zreb.Trim() != String.Empty)
                parts.Add(zreb.Trim());

            List<int> result = new List<int>();
            int dummyInt;
            for (int i = 0; i < parts.Count; i++)
            {
                if (!int.TryParse(parts[i], out dummyInt))
                    return null;
                result.Add(int.Parse(parts[i]));
            }

            int[] occurences = new int[result.Count];
            for (int i = 0; i < result.Count; i++)
            {
                occurences[i] = 0;
            }
            for (int i = 0; i < result.Count; i++)
            {
                int number = result[i];
                if (number < 1 || number > result.Count)
                    return null;
                if (occurences[number - 1] == 0)
                    occurences[number - 1] = 1;
                else
                    return null;
            }
            return result;
        }

        private void kreirajNaOsnovuKvalifikanataFinaleKupa()
        {
            if (ActiveRaspored == null)
            {
                string msg2 = "Morate najpre da kreirate praznu start listu (dugme \"Nova start lista\").";
                MessageDialogs.showMessage(msg2, this.Text);
                return;
            }

            string msg = "Da li zelite da kreirate start listu na osnovu kvalifikanata?";
            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;

            List<int> zreb = parseZreb();
            if (zreb == null)
            {
                msg = "Nepravilno unesen zreb za finale.";
                MessageDialogs.showMessage(msg, this.Text);
                return;
            }
            else if (zreb.Count == 0)
            {
                msg = "Nije unesen zreb za start liste. Da li zelite da kreirate start listu bez zreba?";
                if (!MessageDialogs.queryConfirmation(msg, this.Text))
                    return;
            }

            Sprava[] sprave = Sprave.getSprave(takmicenje.Gimnastika);
            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<RezultatskoTakmicenje> svaRezTakmicenja = loadRezTakmicenjaFinaleKupa(takmicenje.Id);

                    IList<RezultatskoTakmicenje> svaRezTakmicenja2 = new List<RezultatskoTakmicenje>();
                    foreach (RezultatskoTakmicenje rt in svaRezTakmicenja)
                    {
                        if (rt.odvojenoTak3())
                            svaRezTakmicenja2.Add(rt);
                    }
                    if (svaRezTakmicenja2.Count == 0)
                    {
                        MessageDialogs.showMessage("Ne postoji posebno takmicenje III ni za jednu kategoriju.", this.Text);
                        return;
                    }

                    IList<RezultatskoTakmicenje> rezTakmicenja = new List<RezultatskoTakmicenje>();
                    List<TakmicarskaKategorija> kategorije = new List<TakmicarskaKategorija>(ActiveRaspored.Kategorije);
                    foreach (TakmicarskaKategorija k in kategorije)
                    {
                        foreach (RezultatskoTakmicenje rt in svaRezTakmicenja2)
                        {
                            if (rt.Kategorija.Equals(k))
                            {
                                rezTakmicenja.Add(rt);
                            }
                        }
                    }
                    if (rezTakmicenja.Count == 0)
                    {
                        return;
                    }

                    for (int j = 0; j < sprave.Length; j++)
                    {
                        StartListaNaSpravi startLista = ActiveRaspored.getStartLista(sprave[j], ActiveGrupa, 1 /*ActiveRotacija*/);
                        startLista.clear();

                        List<RezultatSpravaFinaleKupa> rezultati = new List<RezultatSpravaFinaleKupa>();
                        foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
                        {
                            PoredakSpravaFinaleKupa poredak = rezTak.Takmicenje1.getPoredakSpravaFinaleKupa(sprave[j]);
                            foreach (RezultatSpravaFinaleKupa rez in poredak.getKvalifikanti())
                            {
                                rezultati.Add(rez);
                            }
                        }

                        int k = 0;
                        while (k < zreb.Count)
                        {
                            if (zreb[k] <= rezultati.Count)
                                startLista.addNastup(new NastupNaSpravi(rezultati[zreb[k] - 1].Gimnasticar, 0));
                            k++;
                        }
                        k = startLista.Nastupi.Count;
                        while (k < rezultati.Count)
                        {
                            startLista.addNastup(new NastupNaSpravi(rezultati[k].Gimnasticar, 0));
                            k++;
                        }
                    }
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
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    for (int j = 0; j < sprave.Length; j++)
                    {
                        StartListaNaSpravi startLista = ActiveRaspored.getStartLista(sprave[j], ActiveGrupa, 1 /*ActiveRotacija*/);
                        foreach (NastupNaSpravi n in startLista.Nastupi)
                        {
                            //  potrebno za slucaj kada se u start listi nalaze i gimnasticari iz kategorija razlicitih od kategorija
                            // za koje start lista vazi.
                            NHibernateUtil.Initialize(n.Gimnasticar.TakmicarskaKategorija);
                        }
                        DAOFactoryFactory.DAOFactory.GetStartListaNaSpraviDAO().Update(startLista);
                    }

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                    takmicenje.LastModified = DateTime.Now;
                    session.Transaction.Commit();

                    setStartListe(ActiveRaspored, ActiveGrupa, 1 /*ActiveRotacija*/);
                    getActiveSpravaGridGroupUserControl().clearSelection();
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
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private RezultatskoTakmicenje loadRezTakmicenje(int takmicenjeId, TakmicarskaKategorija kat)
        {
            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    return doLoadRezTakmicenje(takmicenjeId, kat);
                }
            }
            catch (InfrastructureException ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                return null;
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                return null;
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private RezultatskoTakmicenje doLoadRezTakmicenje(int takmicenjeId, TakmicarskaKategorija kat)
        {
            List<RezultatskoTakmicenje> result = new List<RezultatskoTakmicenje>();

            IList<RezultatskoTakmicenje> svaRezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                .FindByTakmicenjeKatFetch_Tak3_Poredak(takmicenjeId, kat);

            foreach (RezultatskoTakmicenje rt in svaRezTakmicenja)
            {
                NHibernateUtil.Initialize(rt.Propozicije);
                if (rt.odvojenoTak3())
                {
                    foreach (PoredakSprava p in rt.Takmicenje3.Poredak)
                        NHibernateUtil.Initialize(p.Rezultati);
                    NHibernateUtil.Initialize(rt.Takmicenje3.PoredakPreskok.Rezultati);
                    result.Add(rt);
                }
            }
            return result[0];
        }

        private IList<RezultatskoTakmicenje> loadRezTakmicenjaFinaleKupa(int takmicenjeId)
        {
            IList<RezultatskoTakmicenje> result = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                .FindByTakmicenjeFetch_Tak1_PoredakSpravaFinaleKupa(takmicenje.Id);
            foreach (RezultatskoTakmicenje rt in result)
            {
                foreach (PoredakSpravaFinaleKupa p in rt.Takmicenje1.PoredakSpravaFinaleKupa)
                    NHibernateUtil.Initialize(p.Rezultati);
            }
            return result;
        }

        private void mnOcene_Click(object sender, EventArgs e)
        {
            try
            {
                OceneForm form = new OceneForm(takmicenje.Id, deoTakKod);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, "Greska");
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, "Greska");
            }
        }

        private void mnRezultatiViseboj_Click(object sender, EventArgs e)
        {
            try
            {
                RezultatiUkupnoForm form = new RezultatiUkupnoForm(takmicenje.Id, deoTakKod,
                    findRezTakmicenjeForRezultati(), false);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageDialogs.showError(ex.Message, "Greska");
            }
        }

        // TODO4: Ispravi gresku kod oznacavanja ekipe - trenutno kada se selektuje vise gimnasticara, jedino radi ako
        // je desni klik bio na zadnjem selektovanom gimnasticaru.
        // TODO4: Razmisli da li treba izbaciti rotiranje gimnasticara unutar ekipe (posto to uvek treneri zadaju na
        // takmicenju.)
        // TODO4: Kod konacnih plasmana, uvedi izbor da li da se prikazuju svi rezultati, ili samo recimo prva 3 mesta.
        // TODO4: Dodaj mogucnost stampanja poruke da se neki gimnasticar u prethodnim kolima takmicio u razlicitim
        // kategorijama.
        // TODO4: Kod konacnih plasmana, prikazi i ocenu.

        int findRezTakmicenjeForRezultati()
        {
            // DOC: Prikazi one rezultate u kojima se nalazi selektovani gimnasticar. Ako nijedan gimnasticar nije
            // selektovan, prikazi rezultate u kojima se nalazi prvi gimnasticar iz selektovane liste. Ako je
            // selektovana lista prazna, prikazi rezultate za prvu kategoriju.
            GimnasticarUcesnik g = null;
            DataGridViewUserControl c = getActiveSpravaGridGroupUserControl()[clickedSprava].DataGridViewUserControl;
            if (c.getSelectedItemCount() > 0)
                g = c.getSelectedItems<NastupNaSpravi>()[0].Gimnasticar;
            else if (c.getItemCount<NastupNaSpravi>() > 0)
                g = c.getItems<NastupNaSpravi>()[0].Gimnasticar;
            if (g == null)
                return -1;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
                    rezTakDAO.Session = session;
                    IList<RezultatskoTakmicenje> rezTakmicenja = rezTakDAO.FindByGimnasticar(g);
                    if (rezTakmicenja.Count > 0)
                        return rezTakmicenja[0].Id;
                    return -1;
                }
            }
            catch (Exception)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw;
            }
            finally
            {

            }
        }

        private void mnRezultatiSprave_Click(object sender, EventArgs e)
        {
            try
            {
                RezultatiSpravaForm form = new RezultatiSpravaForm(takmicenje.Id, deoTakKod,
                    findRezTakmicenjeForRezultati(), clickedSprava, false, false);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageDialogs.showError(ex.Message, "Greska");
            }
        }

        private void mnRezultatiEkipno_Click(object sender, EventArgs e)
        {
            try
            {
                RezultatiEkipeForm form = new RezultatiEkipeForm(takmicenje.Id, deoTakKod);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, "Greska");
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, "Greska");
            }
        }

        private void mnPromeniStartListu_Click(object sender, EventArgs e)
        {
            if (ActiveRaspored == null)
                return;
            if (clickedSprava == Sprava.Undefined)
                return;

            promeniStartListuCommand(clickedSprava);
        }

        private void btnPrintUnosOcena_Click(object sender, EventArgs e)
        {
            if (ActiveRaspored == null)
                return;

            string nazivIzvestaja;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                nazivIzvestaja = "Start liste - kvalifikacije";
            }
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje2)
            {
                nazivIzvestaja = "Start liste - finale viseboja";
            }
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
            {
                nazivIzvestaja = "Start liste - finale po spravama";
            }
            else
            {
                nazivIzvestaja = "Start liste - finale ekipno";
            }
            string kategorijaRotacija = getFirstKategorijaText(ActiveRaspored) + ", Rotacija " + ActiveRotacija.ToString();

            HeaderFooterForm form = new HeaderFooterForm(deoTakKod, false, true, false, true, true, true, true);
            if (!Opcije.Instance.HeaderFooterInitialized)
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);

                string mestoDatum = takmicenje.Mesto + "  "
                    + takmicenje.Datum.ToShortDateString();
                form.Header1Text = takmicenje.Naziv;
                form.Header2Text = mestoDatum;
                form.Header3Text = nazivIzvestaja;
                form.Header4Text = kategorijaRotacija;
                form.FooterText = mestoDatum;
            }
            else
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);
                form.Header3Text = nazivIzvestaja;
                form.Header4Text = kategorijaRotacija;
            }

            if (form.ShowDialog() != DialogResult.OK)
                return;
            FormUtil.initHeaderFooterFromForm(form);
            Opcije.Instance.HeaderFooterInitialized = true;

            Sprava sprava = Sprava.Undefined;
            if (!form.StampajSveSprave)
            {
                SelectSpravaForm form2 = new SelectSpravaForm(ActiveRaspored.Gimnastika,
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
                string documentName = nazivIzvestaja + kategorijaRotacija;
                int brojEOcena = form.BrojEOcenaFormular;

                if (form.StampajSveSprave)
                {
                    List<StartListaNaSpravi> startListe = new List<StartListaNaSpravi>();

                    Sprava[] sprave = Sprave.getSprave(takmicenje.Gimnastika);
                    foreach (Sprava s in sprave)
                    {
                        startListe.Add(ActiveRaspored.getStartLista(s, ActiveGrupa, ActiveRotacija));
                    }
                    p.setIzvestaj(new SudijskiFormularIzvestaj(startListe, takmicenje.Gimnastika, documentName,
                        brojEOcena, form.BrojSpravaPoStrani, form.StampajRedniBrojNaStartListi,
                        form.StampajKategoriju, form.StampajKlub,
                        getActiveSpravaGridGroupUserControl()));
                }
                else
                {
                    StartListaNaSpravi startLista = ActiveRaspored.getStartLista(sprava, ActiveGrupa, ActiveRotacija);
                    p.setIzvestaj(new SudijskiFormularIzvestaj(startLista, documentName, brojEOcena,
                        form.StampajRedniBrojNaStartListi, form.StampajKategoriju, form.StampajKlub,
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
            List<GimnasticarUcesnik> gimnasticari = new List<GimnasticarUcesnik>();
            foreach (NastupNaSpravi n in dgw.getSelectedItems<NastupNaSpravi>())
                gimnasticari.Add(n.Gimnasticar);
            if (gimnasticari.Count == 0)
                return;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    GimnasticarUcesnikDAO gimUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
                    foreach (GimnasticarUcesnik g in gimnasticari)
                    {
                        g.NastupaZaDrzavu = !prikaziKlub;
                        gimUcesnikDAO.Update(g);
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

            NastupNaSpravi n2 = dgw.getSelectedItem<NastupNaSpravi>();
            dgw.refreshItems();
            dgw.setSelectedItem<NastupNaSpravi>(n2);
        }

        private Color[] bojeZaEkipe = new Color[] { Color.Aqua, Color.Wheat,
                                                    Color.Thistle, Color.LightBlue, Color.Yellow,
                                                     Color.LightPink, Color.LightGray, Color.LightGreen };

        private bool areConsecutiveRowsSelected(DataGridView dgw)
        {
            List<DataGridViewRow> selRows = new List<DataGridViewRow>();
            foreach (DataGridViewRow r in dgw.Rows)
            {
                if (r.Selected)
                    selRows.Add(r);
            }

            if (selRows.Count < 2)
                return true;

            int prevIndex = selRows[0].Index;
            for (int i = 1; i < selRows.Count; ++i)
            {
                int index = selRows[i].Index;
                if (index != prevIndex + 1)
                    return false;
                prevIndex = index;
            }
            return true;
        }

        private bool areSelectedRowsTrueSubsetOfEkipa(DataGridView dgw)
        {
            List<DataGridViewRow> selRows = new List<DataGridViewRow>();
            foreach (DataGridViewRow r in dgw.Rows)
            {
                if (r.Selected)
                    selRows.Add(r);
            }
            if (selRows.Count == 0)
                return false;

            int firstIndex = selRows[0].Index;
            int lastIndex = selRows[selRows.Count - 1].Index;

            if (firstIndex == 0 || lastIndex == dgw.Rows.Count - 1)
                // Nije selektovan pravi podskup svih vrsta, pa sledi da ne moze biti selektovan ni pravi podskup ekipa.
                return false;

            int firstEkipa = (dgw.Rows[firstIndex - 1].DataBoundItem as NastupNaSpravi).Ekipa;
            int lastEkipa = (dgw.Rows[lastIndex + 1].DataBoundItem as NastupNaSpravi).Ekipa;

            if (firstEkipa == 0 || lastEkipa == 0 || firstEkipa != lastEkipa)
                // Prva ekipa pre selektovanih i prva ekipa nakon selektovanih nisu iste.
                return false;

            for (int i = firstIndex; i <= lastIndex; i++)
            {
                if ((dgw.Rows[i].DataBoundItem as NastupNaSpravi).Ekipa != firstEkipa)
                    return false;
            }
            return true;
        }

        private void oznaci(bool oznaciKaoPojedinca)
        {
            StartListaNaSpravi startLista = ActiveRaspored.getStartLista(clickedSprava, ActiveGrupa, ActiveRotacija);

            IList<NastupNaSpravi> selNastupi = getActiveSpravaGridGroupUserControl()[clickedSprava]
                .DataGridViewUserControl.getSelectedItems<NastupNaSpravi>();
            if (selNastupi.Count == 0)
                return;

            if (!areConsecutiveRowsSelected(getActiveSpravaGridGroupUserControl()[clickedSprava]
                                              .DataGridViewUserControl.DataGridView))
            {
                string msg;
                if (oznaciKaoPojedinca)
                    msg = "Samo uzastopne gimnasticare je moguce oznaciti kao pojedinca.";
                else
                    msg = "Samo uzastopne gimnasticare je moguce oznaciti kao ekipu.";
                MessageDialogs.showMessage(msg, this.Text);
                return;
            }

            if (areSelectedRowsTrueSubsetOfEkipa(getActiveSpravaGridGroupUserControl()[clickedSprava]
                                              .DataGridViewUserControl.DataGridView))
            {
                // Selektovan je pravi podskup neke ekipe. Ponisti ekipe za sve gimnasticare iz ekipe.
                // Time se sprecava da imamo ekipu ciji clanovi nisu uzastopni u start listi.

                List<byte> selEkipe = getEkipe(getActiveSpravaGridGroupUserControl()[clickedSprava]
                    .DataGridViewUserControl.DataGridView, true);
                IList<NastupNaSpravi> sviNastupi = getActiveSpravaGridGroupUserControl()[clickedSprava]
                    .DataGridViewUserControl.getItems<NastupNaSpravi>();
                foreach (NastupNaSpravi n in sviNastupi)
                {
                    if (selEkipe.IndexOf(n.Ekipa) != -1)
                        n.Ekipa = 0;
                }
            }
            else
            {
                // Ponisti ekipe za selektovane gimnasticare.
                foreach (NastupNaSpravi n in selNastupi)
                {
                    n.Ekipa = 0;
                }
            }

            if (!oznaciKaoPojedinca)
            {
                byte maxEkipa = getMaxEkipa(getActiveSpravaGridGroupUserControl()[clickedSprava]
                    .DataGridViewUserControl.DataGridView);
                foreach (NastupNaSpravi n in selNastupi)
                    n.Ekipa = (byte)(maxEkipa + 1);
            }

            bool close = false;
            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    DAOFactoryFactory.DAOFactory.GetStartListaNaSpraviDAO().Update(startLista);

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
                close = true;
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
            if (close)
            {
                Close();
                return;
            }

            getActiveSpravaGridGroupUserControl()[clickedSprava].clearSelection();
            getActiveSpravaGridGroupUserControl()[clickedSprava].DataGridViewUserControl.DataGridView.Refresh();
        }

        private void mnOznaciKaoEkipu_Click(object sender, EventArgs e)
        {
            oznaci(false);
        }

        void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (ActiveRotacija != 1)
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

        byte getMaxEkipa(DataGridView dgw)
        {
            List<byte> ekipe = getEkipe(dgw, false);
            byte result = 0;
            foreach (byte i in ekipe)
            {
                if (i > result)
                    result = i;
            }
            return result;
        }

        private void mnOznaciKaoPojedinca_Click(object sender, EventArgs e)
        {
            oznaci(true);
        }

        private void mnRotirajEkipeRotirajGim_Click(object sender, EventArgs e)
        {
            promeniNacinRotacije(NacinRotacije.RotirajEkipeRotirajGimnasticare, clickedSprava);
        }

        private void promeniNacinRotacije(NacinRotacije nacinRotacije, Sprava sprava)
        {
            if (ActiveRotacija != 1)
                return;
            StartListaNaSpravi startLista = ActiveRaspored.getStartLista(sprava, ActiveGrupa, ActiveRotacija);
            bool prvaSprava = takmicenje.Gimnastika == Gimnastika.MSG && startLista.Sprava == Sprava.Parter
                || takmicenje.Gimnastika == Gimnastika.ZSG && startLista.Sprava == Sprava.Preskok;
            bool sveSprave = prvaSprava && MessageDialogs.queryConfirmation(
                "Da li zelite ovakav nacin rotacije i na ostalim spravama?","Nacin rotacije");
            List<StartListaNaSpravi> startListe = new List<StartListaNaSpravi>();
            if (sveSprave)
                startListe.AddRange(ActiveRaspored.getStartListe(ActiveGrupa, ActiveRotacija));
            else
                startListe.Add(startLista);

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    bool modified = false;
                    foreach (StartListaNaSpravi s in startListe)
                    {
                        if (s.NacinRotacije != nacinRotacije)
                        {
                            s.NacinRotacije = nacinRotacije;
                            DAOFactoryFactory.DAOFactory.GetStartListaNaSpraviDAO().Update(s);
                            modified = true;
                        }
                    }
                    if (modified)
                    {
                        takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                        takmicenje.LastModified = DateTime.Now;
                        session.Transaction.Commit();
                    }
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
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void mnNeRotirajEkipeRotirajGim_Click(object sender, EventArgs e)
        {
            promeniNacinRotacije(NacinRotacije.NeRotirajEkipeRotirajGimnasticare, clickedSprava);
        }

        private void mnRotirajSve_Click(object sender, EventArgs e)
        {
            promeniNacinRotacije(NacinRotacije.RotirajSve, clickedSprava);
        }

        private void mnNeRotirajNista_Click(object sender, EventArgs e)
        {
            promeniNacinRotacije(NacinRotacije.NeRotirajNista, clickedSprava);
        }

    }
}
