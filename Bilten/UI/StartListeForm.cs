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
using Bilten.Report;
using NHibernate;

namespace Bilten.UI
{
    public partial class StartListeForm : Form
    {
        private Takmicenje takmicenje;
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
            this.deoTakKod = deoTakKod;

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                kategorijeCount = getKategorijeCount(takmicenjeId);
                if (kategorijeCount == 0)
                    throw new BusinessException("Morate najpre da unesete takmicarske kategorije.");

                rasporedi = loadRasporedi(takmicenjeId, deoTakKod);

                takmicenje = dataContext.GetById<Takmicenje>(takmicenjeId);
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

                //    dataContext.Commit();
            }
            catch (BusinessException)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw;
            }
            catch (InfrastructureException)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw;
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

        private void initUI()
        {
            btnSablon.Visible = false;
            btnSablon.Enabled = false;

            Text = "Start liste - " +
                DeoTakmicenjaKodovi.toString(deoTakKod);
            this.ClientSize = new System.Drawing.Size(this.ClientSize.Width, 540);
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
            {
                cmbRotacija.Enabled = false;
                btnOstaleRotacije.Text = "Kreiraj na osnovu kvalifikanata";
            }

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

        // TODO: Trenutno se meni otvara samo kada se klikne unutar sprave. Promeni da se meni otvara kada se klike bilo gde.
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
                mnUnesiOcenu.Enabled = true;
            }
            else
                mnUnesiOcenu.Enabled = false;

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
                    ActiveRaspored.Id, sprava, ActiveGrupa, ActiveRotacija, takmicenje.Id);
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
                form = new SelectKategorijaForm(takmicenje.Id, dodeljeneKategorije, 
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
                takmicenje.Id);
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

            HeaderFooterForm form = new HeaderFooterForm(deoTakKod, false, true, false, true, false);
            if (!Opcije.Instance.HeaderFooterInitialized)
            {
                Opcije.Instance.initHeaderFooterFormFromOpcije(form);

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
                Opcije.Instance.initHeaderFooterFormFromOpcije(form);
                form.Header3Text = nazivIzvestaja;
                form.Header4Text = kategorijaRotacija;
            }

            if (form.ShowDialog() != DialogResult.OK)
                return;
            Opcije.Instance.initHeaderFooterFromForm(form);
            Opcije.Instance.HeaderFooterInitialized = true;

            Sprava sprava = Sprava.Undefined;
            if (!form.StampajSveSprave)
            {
                SelectSpravaForm form2 = new SelectSpravaForm(ActiveRaspored.Pol,
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

                    Sprava[] sprave = Sprave.getSprave(takmicenje.Gimnastika);
                    foreach (Sprava s in sprave)
                    {
                        startListe.Add(ActiveRaspored.getStartLista(s, ActiveGrupa, ActiveRotacija));
                    }
                    p.setIzvestaj(new StartListaIzvestaj(startListe, takmicenje.Gimnastika, documentName,
                        form.BrojSpravaPoStrani, form.StampajRedniBrojNaStartListi));
                }
                else
                {
                    StartListaNaSpravi startLista = ActiveRaspored.getStartLista(sprava, ActiveGrupa, ActiveRotacija);
                    p.setIzvestaj(new StartListaIzvestaj(startLista, documentName, form.StampajRedniBrojNaStartListi));
                }

                p.ShowDialog();

                // TODO2: Dodaj kategoriju i godiste u start liste.

            }
            catch (InfrastructureException ex)
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
                kreirajPreostaleRotacije();
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
                kreirajNaOsnovuKvalifikanata();
        }

        private void kreirajPreostaleRotacije()
        {
            if (ActiveRaspored == null)
                return;

            int finalRot = (takmicenje.Gimnastika == Gimnastika.ZSG) ? 4 : 6;
            string preostaleRot = "2-" + finalRot.ToString();
            string msgFmt = "Da li zelite da kreirate rotacije {0}? Prethodni raspored koji je postojao na rotacijama {0} " +
                "bice izbrisan.";
            if (!MessageDialogs.queryConfirmation(String.Format(msgFmt, preostaleRot), this.Text))
                return;

            Sprava[] sprave = Sprave.getSprave(takmicenje.Gimnastika);
            for (int i = 2; i <= finalRot; i++)
            {
                for (int j = 0; j < sprave.Length; j++)
                {
                    StartListaNaSpravi startLista = ActiveRaspored.getStartLista(sprave[j], ActiveGrupa, i);
                    Sprava prethSprava = (j == 0) ? sprave[sprave.Length - 1] : sprave[j - 1];
                    StartListaNaSpravi startLista2 = ActiveRaspored.getStartLista(prethSprava, ActiveGrupa, i - 1);

                    startLista.clear();
                    foreach (NastupNaSpravi n in startLista2.Nastupi)
                    {
                        startLista.addNastup(new NastupNaSpravi(false, n.Gimnasticar));
                    }
                    if (startLista.Nastupi.Count > 1)
                    {
                        NastupNaSpravi n2 = startLista.Nastupi[0];
                        startLista.removeNastup(n2);
                        startLista.addNastup(n2);
                    }
                }
            }
     
            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                for (int i = 2; i <= finalRot; i++)
                {
                    for (int j = 0; j < sprave.Length; j++)
                    {
                        StartListaNaSpravi startLista = ActiveRaspored.getStartLista(sprave[j], ActiveGrupa, i);
                        dataContext.Save(startLista);
                    }
                }
                dataContext.Commit();

                if (ActiveRotacija != 1)
                    setStartListe(ActiveRaspored, ActiveGrupa, ActiveRotacija);
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showMessage(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                Close();
                return;
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

        // TODO: Ceo ovaj deo gde se kreira na osnovu kvalifikanata je radjen na brzinu, gde je jedino bilo bitno da moze da
        // se primeni na Memorijal. Trebalo bi ga temeljno proveriti i uciniti robustnijim.
        private void kreirajNaOsnovuKvalifikanata()
        {
            if (ActiveRaspored == null)
                return;

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
                StartListaNaSpravi startLista = ActiveRaspored.getStartLista(sprave[j], ActiveGrupa, ActiveRotacija);
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
                        startLista.addNastup(new NastupNaSpravi(false, kvalifikanti[zreb[k] - 1].Gimnasticar));
                    k++;
                }
                k = startLista.Nastupi.Count;
                while (k < kvalifikanti.Count)
                {
                    startLista.addNastup(new NastupNaSpravi(false, kvalifikanti[k].Gimnasticar));
                    k++;
                }
            }

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                for (int j = 0; j < sprave.Length; j++)
                {
                    StartListaNaSpravi startLista = ActiveRaspored.getStartLista(sprave[j], ActiveGrupa, ActiveRotacija);
                    foreach (NastupNaSpravi n in startLista.Nastupi)
                    {
                        //  potrebno za slucaj kada se u start listi nalaze i gimnasticari iz kategorija razlicitih od kategorija
                        // za koje start lista vazi.
                        NHibernateUtil.Initialize(n.Gimnasticar.TakmicarskaKategorija);
                    }
                    dataContext.Save(startLista);
                }
                dataContext.Commit();

                setStartListe(ActiveRaspored, ActiveGrupa, ActiveRotacija);
                getActiveSpravaGridGroupUserControl().clearSelection();
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showMessage(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                Close();
                return;
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

        private RezultatskoTakmicenje loadRezTakmicenje(int takmicenjeId, TakmicarskaKategorija kat)
        {
            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                return doLoadRezTakmicenje(takmicenjeId, kat);
            }
            catch (InfrastructureException ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                return null;
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                return null;
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

        private RezultatskoTakmicenje doLoadRezTakmicenje(int takmicenjeId, TakmicarskaKategorija kat)
        {
            string query = @"select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje3 t
                    left join fetch t.Poredak
                    left join fetch t.Ucesnici u
                    left join fetch u.Gimnasticar g
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    where r.Takmicenje.Id = :takmicenjeId
                    and r.Kategorija = :kategorija
                    order by r.RedBroj";

            IList<RezultatskoTakmicenje> svaRezTakmicenja = dataContext.
                ExecuteQuery<RezultatskoTakmicenje>(QueryLanguageType.HQL, query,
                        new string[] { "takmicenjeId", "kategorija" },
                        new object[] { takmicenjeId, kat });
            foreach (RezultatskoTakmicenje tak in svaRezTakmicenja)
            {
                NHibernateUtil.Initialize(tak.Propozicije);
                if (tak.Propozicije.PostojiTak3)
                {
                    foreach (PoredakSprava p in tak.Takmicenje3.Poredak)
                        NHibernateUtil.Initialize(p.Rezultati);
                    NHibernateUtil.Initialize(tak.Takmicenje3.PoredakPreskok.Rezultati);
                }
            }
            List<RezultatskoTakmicenje> result = new List<RezultatskoTakmicenje>();
            foreach (RezultatskoTakmicenje rt in svaRezTakmicenja)
            {
                if (rt.Propozicije.PostojiTak3 && rt.Propozicije.OdvojenoTak3)
                    result.Add(rt);
            }
            return result[0];
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
                RezultatiUkupnoForm form = new RezultatiUkupnoForm(takmicenje.Id, deoTakKod);
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

        private void mnRezultatiSprave_Click(object sender, EventArgs e)
        {
            try
            {
                RezultatiSpravaForm form = new RezultatiSpravaForm(takmicenje.Id, deoTakKod, false, null, Sprava.Undefined);
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

            try
            {
                StartListaRotEditorForm form = new StartListaRotEditorForm(
                    ActiveRaspored.Id, clickedSprava, ActiveGrupa, ActiveRotacija, takmicenje.Id);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    rasporedi[tabControl1.SelectedIndex] = form.RasporedNastupa;
                    refresh(clickedSprava);
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
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

            HeaderFooterForm form = new HeaderFooterForm(deoTakKod, false, true, false, true, true);
            if (!Opcije.Instance.HeaderFooterInitialized)
            {
                Opcije.Instance.initHeaderFooterFormFromOpcije(form);

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
                Opcije.Instance.initHeaderFooterFormFromOpcije(form);
                form.Header3Text = nazivIzvestaja;
                form.Header4Text = kategorijaRotacija;
            }

            if (form.ShowDialog() != DialogResult.OK)
                return;
            Opcije.Instance.initHeaderFooterFromForm(form);
            Opcije.Instance.HeaderFooterInitialized = true;

            Sprava sprava = Sprava.Undefined;
            if (!form.StampajSveSprave)
            {
                SelectSpravaForm form2 = new SelectSpravaForm(ActiveRaspored.Pol,
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

                //bool stampajEOcene = true;
                //bool stampajEOcene = form.StampajEOcene;
                //int brojEOcena = 0;
                //if (stampajEOcene)
                    //brojEOcena = takmicenje.BrojESudija;
                //    brojEOcena = 0;
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
                        brojEOcena, form.BrojSpravaPoStrani, form.StampajRedniBrojNaStartListi));
                }
                else
                {
                    StartListaNaSpravi startLista = ActiveRaspored.getStartLista(sprava, ActiveGrupa, ActiveRotacija);
                    p.setIzvestaj(new SudijskiFormularIzvestaj(startLista, documentName, brojEOcena, form.StampajRedniBrojNaStartListi));
                }

                p.ShowDialog();

            }
            catch (InfrastructureException ex)
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
