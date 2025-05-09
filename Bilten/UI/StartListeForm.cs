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

                    // TODO4: Treba uvesti redni broj i turnus, i sortirati rasporede po njima nakon sto se ucitaju iz baze.
                    // TODO4: Kada promenis da svaki RasporedNastupa odgovara jednom turnusu, promeni da kod brisanja
                    // start liste brise sve odgovarajuce turnuse.
                    rasporedi = DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO()
                        .FindByTakmicenjeDeoTak(takmicenjeId, deoTakKod);

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                    NHibernateUtil.Initialize(takmicenje);

                    initUI();
                    createTabs(rasporedi);
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
                cmbGrupa.Enabled = false;
                cmbRotacija.Enabled = false;
                btnNewGroup.Enabled = false;
                btnOstaleRotacije.Text = "Kreiraj na osnovu kvalifikanata";
            }
            mnRotirajEkipeRotirajGim.Checked = true;
        }

        private void updateMnPrebaciNa(Sprava[] sprave)
        {
            // TODO4: Proveri da li su itemi koji se dodaju isti kao i oni koji se brisu, i u tom slucaju ne radi nista.
            mnPrebaciNa.DropDownItems.Clear();
            for (int i = 0; i < sprave.Length; ++i)
            {
                Sprava s = sprave[i];
                ToolStripMenuItem item = new ToolStripMenuItem();
                //item.Size = new System.Drawing.Size(152, 22);
                item.Text = Sprave.toString(s);
                item.Tag = s;
                item.Click += mnPrebaciNa_Click;
                mnPrebaciNa.DropDownItems.Add(item);
            }
        }

        void mnPrebaciNa_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mn = (ToolStripMenuItem)sender;
            prebaciGimnasticare((Sprava)mn.Tag);
        }

        private void createTabs(IList<RasporedNastupa> rasporedi)
        {
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
                spravaGridGroupUserControl1.init(Sprave.getSpraveIPauze(raspored.PauzeMask, takmicenje.Gimnastika));
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
            foreach (SpravaGridUserControl c in getActiveSpravaGridGroupUserControl().SpravaGridUserControls)
            {
                if (c.Sprava != clickedSprava)
                    c.clearSelection();
            }
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
                mnPrebaciNa.Enabled = selCount > 0;
            }
            else
            {
                getActiveSpravaGridGroupUserControl()[clickedSprava].clearSelection();
                mnUnesiOcenu.Enabled = false;
                mnPrikaziKlub.Enabled = mnPrikaziKlub.Visible = false;
                mnPrikaziDrzavu.Enabled = mnPrikaziDrzavu.Visible = false;
                mnPrebaciNa.Enabled = false;
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
            if (mnPrebaciNa.Enabled)
            {
                foreach (ToolStripMenuItem mn in mnPrebaciNa.DropDownItems)
                {
                    mn.Enabled = (Sprava)mn.Tag != clickedSprava;
                }
            }
            contextMenuStrip1.Show(grid, new Point(x, y));
        }

        private void initTab(TabPage tabPage, RasporedNastupa raspored)
        {
            // TODO: Kod u ovom metodu je prekopiran iz Designer.cs fajla (plus ono
            // sto sam dodao u createTab metodu). Proveri da li je u Designer.cs fajlu
            // nesto menjano, i ako jeste promeni ovde (isto i u EkipeForm, RasporedSudijaForm, TakmicariKategorijeForm,
            // TakmicariTakmicenjaForm).
            SpravaGridGroupUserControl spravaGridGroupUserControl = new SpravaGridGroupUserControl();
            spravaGridGroupUserControl.Location = USER_CONTROL_LOCATION;
            spravaGridGroupUserControl.SpravaGridRightClick +=
                new EventHandler<SpravaGridRightClickEventArgs>(spravaGridGroupUserControl1_SpravaGridRightClick);
            //spravaGridGroupUserControl.Size = this.rasporedSudijaUserControl1.Size;
            spravaGridGroupUserControl.init(Sprave.getSpraveIPauze(raspored.PauzeMask, takmicenje.Gimnastika)); // odredjuje i Size
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
            tabPage.Text = raspored.Naziv;
            //tabPage.UseVisualStyleBackColor = this.tabPage1.UseVisualStyleBackColor;
            tabPage.ResumeLayout(false);
        }

        // TODO4: Naziv rasporeda bi trebalo menjati kada se gimnasticari dodaju i brisu iz neke od start lista, da sadrzi
        // samo one kategorije koje postoje u start listama. Ako su sve kategorije u start listama, mozda bi trebalo
        // da naziv bude "Sve kategorije".

        void cmbRotacija_SelectedIndexChanged(object sender, EventArgs e)
        {
            onRotacijaChanged(false);
        }

        void cmbGrupa_SelectedIndexChanged(object sender, EventArgs e)
        {
            onRotacijaChanged(true);
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
            Sprava[] sprave = Sprave.getSpraveIPauze(ActiveRaspored.PauzeMask, takmicenje.Gimnastika);
            int brojRotacija = sprave.Length;
            initCombos(brojGrupa, brojRotacija);

            updateMnPrebaciNa(sprave);

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

        private void onRotacijaChanged(bool updateMnPrebaciNa)
        {
            if (ActiveRaspored == null)
                return;

            handleRotacijaChanged(updateMnPrebaciNa);
        }

        private void handleRotacijaChanged(bool UpdateMnPrebaciNa)
        {
            int g = (int)cmbGrupa.SelectedItem;
            int r = (int)cmbRotacija.SelectedItem;
            grupa[tabControl1.SelectedIndex] = g;
            rot[tabControl1.SelectedIndex] = r;

            setStartListe(ActiveRaspored, g, r);
            if (UpdateMnPrebaciNa)
            {
                updateMnPrebaciNa(Sprave.getSpraveIPauze(ActiveRaspored.PauzeMask, takmicenje.Gimnastika));
            }

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
                GridColumnsInitializer.setColumnWidthsStartLista(c.DataGridViewUserControl, startLista);
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
            SelectSpravaForm form = new SelectSpravaForm(Sprave.getSpraveIPauze(ActiveRaspored.PauzeMask, takmicenje.Gimnastika),
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
                    ActiveRaspored.addNewGrupa(takmicenje.Gimnastika);
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
            onRotacijaChanged(true);
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

            string msg = "Izaberite kategorije za koje vazi start lista";
            CheckListForm form = new CheckListForm(kategorijeStr, new List<int>(), msg, "Kategorije", true, msg, true);
            if (form.ShowDialog() != DialogResult.OK)
                return;

            IList<TakmicarskaKategorija> selKategorije = new List<TakmicarskaKategorija>();
            foreach (int i in form.CheckedIndices)
                selKategorije.Add(kategorije[i]);

            int pauzeMask = 0;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                PauzeForm form2 = new PauzeForm(takmicenje.Gimnastika);
                form2.init();
                if (form2.ShowDialog() != DialogResult.OK)
                    return;
                pauzeMask = form2.PauzeMask;
            }

            RasporedNastupa newRaspored = null;
            bool added = false;
            session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    newRaspored = new RasporedNastupa(selKategorije, deoTakKod, takmicenje.Gimnastika, pauzeMask);
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
            if (Sprave.isPraznaSprava(c.Sprava))
            {
                //MessageDialogs.showMessage("Nije dozvoljen unos ocena za pauze u rotaciji.", this.Text);
                return;
            }

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
                nazivIzvestaja = Opcije.Instance.KvalStartListe;
            }
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje2)
            {
                nazivIzvestaja = "Start liste - finale viseboja";
            }
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
            {
                nazivIzvestaja = Opcije.Instance.FinaleStartListe;
            }
            else
            {
                nazivIzvestaja = "Start liste - finale ekipno";
            }
            // TODO5: Verovatno bi trebalo ukljuciti i turnus, ukoliko ima vise turnusa. Dodaj turnus i subdivision
            // u jezike
            // Primer: II TURNUS - Juniors - Rotacija 1
            //         SUBDIVISION II - Juniors - Rotation 1
            // TODO5: Dodaj kolonu za takmicarski broj u gridu
            // TODO5: Dodaj skaliranje za ceo program (UI)
            // TODO5: Proveri zaokruzivanje E ocene, i kako da ga program pravilno obradjuje
            // TODO5: Kod stampanja kvalifikanata, pored Score treba ponuditi mogucnost da se stampaju i ostale ocene (D,
            //        E, Bonus, Pen)
            // TODO5: Dodaj bonus u grid za rezultate sprave
            // TODO5: Dodaj OCR za ocitavanje ocena sa sudijskih formulara (npr pogledaj Googlov Tesseract OCR)

            string kategorijaRotacija = ActiveRaspored.Naziv;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                kategorijaRotacija += " - " + Opcije.Instance.Rotacija + " " + ActiveRotacija.ToString();
            }

            HeaderFooterForm form = new HeaderFooterForm(deoTakKod, false, true, false, true, true, true, false, false,
                                                         false, false, false);
 
            string gym = GimnastikaUtil.getGimnastikaStr(takmicenje.Gimnastika, Opcije.Instance.Jezik);
            if (!Opcije.Instance.HeaderFooterInitialized)
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);

                string mestoDatum = takmicenje.Mesto + "  "
                    + takmicenje.Datum.ToShortDateString();
                form.Header1Text = takmicenje.Naziv;
                form.Header2Text = mestoDatum;
                form.Header3Text = gym + " - " + nazivIzvestaja;
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
                form.Header3Text = gym + " - " + nazivIzvestaja;
                form.Header4Text = kategorijaRotacija;
            }

            if (form.ShowDialog() != DialogResult.OK)
                return;
            FormUtil.initOpcijeFromHeaderFooterForm(form);
            Opcije.Instance.HeaderFooterInitialized = true;

            Sprava sprava = Sprava.Undefined;
            if (!form.StampajSveSprave)
            {
                SelectSpravaForm form2 = new SelectSpravaForm(Sprave.getSpraveIPauze(ActiveRaspored.PauzeMask, takmicenje.Gimnastika),
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
                string documentName = gym + " - " + nazivIzvestaja + " - " + kategorijaRotacija;

                DataGridView formGrid = getActiveSpravaGridGroupUserControl()[Sprave.getSprava(1, takmicenje.Gimnastika)]
                    .DataGridViewUserControl.DataGridView;
                if (form.StampajSveSprave)
                {
                    List<StartListaNaSpravi> startListe = new List<StartListaNaSpravi>();
                    foreach (Sprava s in Sprave.getSpraveIPauze(ActiveRaspored.PauzeMask, takmicenje.Gimnastika))
                    {
                        startListe.Add(ActiveRaspored.getStartLista(s, ActiveGrupa, ActiveRotacija));
                    }
                    p.setIzvestaj(new StartListaIzvestaj(startListe, documentName, form.BrojSpravaPoStrani,
                        form.StampajRedniBrojNaStartListi, form.StampajKlub, form.StampajKategoriju, formGrid, takmicenje,
                        new Font(form.TekstFont, form.TekstFontSize),
                        form.ResizeByGrid));
                }
                else
                {
                    StartListaNaSpravi startLista = ActiveRaspored.getStartLista(sprava, ActiveGrupa, ActiveRotacija);
                    p.setIzvestaj(new StartListaIzvestaj(startLista, documentName, form.StampajRedniBrojNaStartListi,
                        form.StampajKlub, form.StampajKategoriju, formGrid, takmicenje,
                        new Font(form.TekstFont, form.TekstFontSize), form.ResizeByGrid));
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
                    kreirajNaOsnovuKvalifikanata(true);
                else
                    kreirajPreostaleRotacije();
            }
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
                kreirajNaOsnovuKvalifikanata(false);
        }

        private void kreirajPreostaleRotacije()
        {
            if (ActiveRaspored == null)
                return;

            List<List<Sprava>> aktivneSprave = null;
            int finalRot = Sprave.getSpraveIPauze(ActiveRaspored.PauzeMask, takmicenje.Gimnastika).Length;
            if (!ActiveRaspored.hasPauze())
            {
                // Nadji aktivne sprave za rotaciju 1.
                List<Sprava> aktivneSpraveRot1 = new List<Sprava>();
                foreach (Sprava s in Sprave.getSprave(takmicenje.Gimnastika))
                {
                    if (ActiveRaspored.getStartLista(s, ActiveGrupa, 1).Nastupi.Count != 0)
                        aktivneSpraveRot1.Add(s);
                }
                try
                {
                    SpraveNaRotacijiForm form = new SpraveNaRotacijiForm(takmicenje.Gimnastika, aktivneSpraveRot1);
                    if (form.ShowDialog() != DialogResult.OK)
                        return;
                    aktivneSprave = form.AktivneSprave;
                }
                catch (Exception ex)
                {
                    MessageDialogs.showError(ex.Message, this.Text);
                    return;
                }
            }
            else
            {
                aktivneSprave = new List<List<Sprava>>();
                for (int i = 1; i <= finalRot; ++i)
                {
                    aktivneSprave.Add(new List<Sprava>(Sprave.getSpraveIPauze(ActiveRaspored.PauzeMask, takmicenje.Gimnastika)));
                }
            }

            for (int rot = 2; rot <= finalRot; rot++)
                ActiveRaspored.kreirajRotaciju(ActiveGrupa, rot, aktivneSprave);
     
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

        private void kreirajNaOsnovuKvalifikanata(bool finaleKupa)
        {
            string msg = "Da li zelite da kreirate start listu na osnovu kvalifikanata?";
            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;

            List<List<int>> zreb;
            if (takmicenje.ZrebZaFinalePoSpravama.Trim() == String.Empty)
            {
                msg = "Nije unesen zreb za start liste. Da li zelite da kreirate start listu bez zreba?";
                if (!MessageDialogs.queryConfirmation(msg, this.Text))
                    return;
                zreb = new List<List<int>>();
            }
            else
            {
                zreb = Zreb.parseZreb(takmicenje.ZrebZaFinalePoSpravama, takmicenje.Gimnastika);
            }

            ISession session = null;
            if (rasporedi.Count > 0)
            {
                // Posto cu brisati jedan po jedan tab pocevsi od poslednjeg, selektujem prvi tab da se ne bi pozivalo
                // onSelectedTabIndexChanged prilikom svakog brisanja.
                tabControl1.SelectedIndex = 0;

                // TODO4: Postojece rasporede brisem u posebnoj sesiji, zato sto ako ih brisem u istoj sesiji u
                // kojoj i kreiram nove rasporede, sesija se odjednom izgubi (u metodu loadRezTakmicenjaOdvojenoTak3).
                // Proveri zasto se ovo desava. Ako ne uspes da pronadjes razlog (i ostane ovakvo resenje), verovatno bi
                // trebalo klonirati sve rasporede koji se brisu, i zatim ih restorovati ako druga sesija generise izuzetak.
                Cursor.Current = Cursors.WaitCursor;
                Cursor.Show();
                try
                {
                    using (session = NHibernateHelper.Instance.OpenSession())
                    using (session.BeginTransaction())
                    {
                        CurrentSessionContext.Bind(session);
                        RasporedNastupaDAO rasporedNastupaDAO = DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO();
                        while (rasporedi.Count > 0)
                        {
                            rasporedNastupaDAO.Delete(rasporedi[0]);
                            rasporedi.RemoveAt(0);
                        }
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
                    Cursor.Hide();
                    Cursor.Current = Cursors.Arrow;
                    CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
                }

                // Izbrisi tabove.
                while (tabControl1.TabPages.Count > 0)
                {
                    int index = tabControl1.TabPages.Count - 1;
                    tabControl1.TabPages.RemoveAt(index);
                    tabOpened.RemoveAt(index);
                    grupa.RemoveAt(index);
                    rot.RemoveAt(index);
                }
            }

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    // TODO4: Da li ce ovo raditi ako postoji vise takmicenja (kao npr. Memorijal i DKMT kup)?

                    CurrentSessionContext.Bind(session);

                    TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
                    takmicenjeDAO.Attach(takmicenje, false);

                    RasporedNastupaDAO rasporedNastupaDAO = DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO();

                    IList<RezultatskoTakmicenje> rezTakmicenja;
                    if (finaleKupa)
                        rezTakmicenja = loadRezTakmicenjaFinaleKupaOdvojenoTak3(takmicenje.Id);
                    else
                        rezTakmicenja = loadRezTakmicenjaOdvojenoTak3(takmicenje.Id);
                    if (rezTakmicenja.Count == 0)
                    {
                        MessageDialogs.showMessage("Ne postoji posebno takmicenje III ni za jednu kategoriju.", this.Text);
                        return;
                    }

                    foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                    {
                        RasporedNastupa newRaspored = new RasporedNastupa(rt.Kategorija, deoTakKod, takmicenje.Gimnastika, 0);
                        foreach (Sprava sprava in Sprave.getSprave(takmicenje.Gimnastika))
                        {
                            IList<GimnasticarUcesnik> kvalifikanti;
                            if (finaleKupa)
                            {
                                if (sprava != Sprava.Preskok)
                                    kvalifikanti = rt.Takmicenje1.getPoredakSpravaFinaleKupa(sprava).getKvalifikanti();
                                else
                                    kvalifikanti = rt.Takmicenje1.PoredakPreskokFinaleKupa.getKvalifikanti();
                            }
                            else
                                kvalifikanti = rt.Takmicenje3.getGimnasticariKvalifikanti(sprava);
                            StartListaNaSpravi startLista = newRaspored.getStartLista(sprava, 1, 1);
                            List<int> zrb;
                            if (zreb.Count == 0)
                                zrb = new List<int>();
                            else
                                zrb = zreb[Sprave.indexOf(sprava, takmicenje.Gimnastika)];
                            startLista.kreirajNaOsnovuKvalifikanata(kvalifikanti, zrb);
                        }
                        rasporedNastupaDAO.Add(newRaspored);
                        rasporedi.Add(newRaspored);
                    }

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
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        
            createTabs(rasporedi);
            getActiveSpravaGridGroupUserControl().clearSelection();
        }

        private IList<RezultatskoTakmicenje> loadRezTakmicenjaOdvojenoTak3(int takmicenjeId)
        {
            IList<RezultatskoTakmicenje> result = new List<RezultatskoTakmicenje>();

            IList<RezultatskoTakmicenje> rezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                .FindByTakmicenjeFetch_Tak3_Poredak(takmicenjeId);
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                if (rt.odvojenoTak3())
                    result.Add(rt);
            }
            return result;
        }

        private IList<RezultatskoTakmicenje> loadRezTakmicenjaFinaleKupaOdvojenoTak3(int takmicenjeId)
        {
            IList<RezultatskoTakmicenje> result = new List<RezultatskoTakmicenje>();
            IList<RezultatskoTakmicenje> rezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                .FindByTakmicenjeFetch_Tak1_PoredakSpravaFinaleKupa(takmicenje.Id);
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                if (rt.odvojenoTak3())
                    result.Add(rt);
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
                nazivIzvestaja = Opcije.Instance.KvalStartListe;
            }
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje2)
            {
                nazivIzvestaja = "Start liste - finale viseboja";
            }
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
            {
                nazivIzvestaja = Opcije.Instance.FinaleStartListe;
            }
            else
            {
                nazivIzvestaja = "Start liste - finale ekipno";
            }

            string kategorijaRotacija = ActiveRaspored.Naziv;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                kategorijaRotacija += " - " + Opcije.Instance.Rotacija + " " + ActiveRotacija.ToString();
            }

            HeaderFooterForm form = new HeaderFooterForm(deoTakKod, false, true, false, true, true, true, true, false,
                false, false, true);
            form.restoreCkbPrikaziBonusLocation();  // hack, da ne moram da dodajem novi parametar u konstruktoru za ovo

            string gym = GimnastikaUtil.getGimnastikaStr(takmicenje.Gimnastika, Opcije.Instance.Jezik);
            if (!Opcije.Instance.HeaderFooterInitialized)
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);

                string mestoDatum = takmicenje.Mesto + "  "
                    + takmicenje.Datum.ToShortDateString();
                form.Header1Text = takmicenje.Naziv;
                form.Header2Text = mestoDatum;
                form.Header3Text = gym + " - " + nazivIzvestaja;
                form.Header4Text = kategorijaRotacija;
                form.FooterText = mestoDatum;
            }
            else
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);
                form.Header3Text = gym + " - " + nazivIzvestaja;
                form.Header4Text = kategorijaRotacija;
            }

            // Sudijski formular ima poseban font size za tekst
            form.TekstFontSize = Opcije.Instance.SudijskiFormularFontSize;
            
            if (form.ShowDialog() != DialogResult.OK)
                return;

            // Azuriraj Opcije. TekstFontSize treba da ostane nepromenjen, a SudijskiFormularFontSize treba da dobije
            // novu vrednost
            int oldTekstFontSize = Opcije.Instance.TekstFontSize;
            FormUtil.initOpcijeFromHeaderFooterForm(form);
            Opcije.Instance.TekstFontSize = oldTekstFontSize;
            Opcije.Instance.SudijskiFormularFontSize = form.TekstFontSize;

            Opcije.Instance.HeaderFooterInitialized = true;

            Sprava sprava = Sprava.Undefined;
            if (!form.StampajSveSprave)
            {
                SelectSpravaForm form2 = new SelectSpravaForm(Sprave.getSprave(takmicenje.Gimnastika),
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
                string documentName = gym + " - " + nazivIzvestaja + " - " + kategorijaRotacija;
                int brojEOcena = form.BrojEOcenaFormular;

                if (form.StampajSveSprave)
                {
                    List<StartListaNaSpravi> startListe = new List<StartListaNaSpravi>();

                    Sprava[] sprave = Sprave.getSprave(takmicenje.Gimnastika);
                    foreach (Sprava s in sprave)
                    {
                        startListe.Add(ActiveRaspored.getStartLista(s, ActiveGrupa, ActiveRotacija));
                    }
                    p.setIzvestaj(new SudijskiFormularIzvestaj(startListe, documentName, brojEOcena, form.BrojSpravaPoStrani,
                        form.StampajRedniBrojNaStartListi, form.StampajKategoriju, form.StampajKlub,
                        getActiveSpravaGridGroupUserControl(), takmicenje, form.PrikaziBonus,
                        new Font(form.TekstFont, form.TekstFontSize), form.ResizeByGrid));
                }
                else
                {
                    StartListaNaSpravi startLista = ActiveRaspored.getStartLista(sprava, ActiveGrupa, ActiveRotacija);
                    p.setIzvestaj(new SudijskiFormularIzvestaj(startLista, documentName, brojEOcena,
                        form.StampajRedniBrojNaStartListi, form.StampajKategoriju, form.StampajKlub,
                        getActiveSpravaGridGroupUserControl()[sprava].DataGridViewUserControl.DataGridView, takmicenje,
                        form.PrikaziBonus, new Font(form.TekstFont, form.TekstFontSize), form.ResizeByGrid));
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

            int ekipaPre = (dgw.Rows[firstIndex - 1].DataBoundItem as NastupNaSpravi).Ekipa;
            int ekipaPosle = (dgw.Rows[lastIndex + 1].DataBoundItem as NastupNaSpravi).Ekipa;

            if (ekipaPre != 0 && ekipaPosle != 0 && ekipaPre == ekipaPosle)
                return true;
            return false;
        }

        private void prebaciGimnasticare(Sprava sprava)
        {
            StartListaNaSpravi startLista = ActiveRaspored.getStartLista(clickedSprava, ActiveGrupa, ActiveRotacija);

            IList<NastupNaSpravi> selNastupi = getActiveSpravaGridGroupUserControl()[clickedSprava]
                .DataGridViewUserControl.getSelectedItems<NastupNaSpravi>();
            if (selNastupi.Count == 0)
                return;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    StartListaNaSpravi startLista2 = ActiveRaspored.getStartLista(sprava, ActiveGrupa, ActiveRotacija);
                    ActiveRaspored.prebaciGimnasticare(selNastupi, startLista, startLista2);

                    StartListaNaSpraviDAO startListaDAO = DAOFactoryFactory.DAOFactory.GetStartListaNaSpraviDAO();
                    startListaDAO.Update(startLista);
                    startListaDAO.Update(startLista2);

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
                return;
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        
            setStartListe(ActiveRaspored, ActiveGrupa, ActiveRotacija);
            getActiveSpravaGridGroupUserControl().clearSelection();
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

                // Ako je selektovan pravi podskup ekipe, samo jedna ekipa moze da bude selektovana.
                int selEkipa = getEkipe(getActiveSpravaGridGroupUserControl()[clickedSprava]
                    .DataGridViewUserControl.DataGridView, true)[0];

                IList<NastupNaSpravi> sviNastupi = getActiveSpravaGridGroupUserControl()[clickedSprava]
                    .DataGridViewUserControl.getItems<NastupNaSpravi>();
                foreach (NastupNaSpravi n in sviNastupi)
                {
                    if (n.Ekipa == selEkipa)
                        n.Ekipa = 0;
                }
            }
            else
            {
                // Ponisti ekipe za selektovane gimnasticare.
                foreach (NastupNaSpravi n in selNastupi)
                    n.Ekipa = 0;
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
                    result.Add(n.Ekipa);
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
