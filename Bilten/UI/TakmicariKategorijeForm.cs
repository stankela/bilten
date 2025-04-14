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
using System.Collections;
using NHibernate;
using Bilten.Report;
using NHibernate.Context;
using Bilten.Dao;
using Bilten.Dao.NHibernate;
using Bilten.Util;
using Bilten.Services;

namespace Bilten.UI
{
    public partial class TakmicariKategorijeForm : Form
    {
        private IList<TakmicarskaKategorija> takmicarskeKategorije;
        private List<GimnasticarUcesnik>[] gimnasticari;
        private bool[] tabOpened;
        private StatusBar statusBar;
        private Takmicenje takmicenje;
        private IDictionary<short, GimnasticarUcesnik> brojeviMap;

        private TakmicarskaKategorija ActiveKategorija
        {
            get
            {
                return takmicarskeKategorije[tabControl1.SelectedIndex];
            }
        }

        public TakmicariKategorijeForm(int takmicenjeId)
        {
            InitializeComponent();
            brojeviMap = new Dictionary<short, GimnasticarUcesnik>();

            // TODO3: Probaj da nadjes nacin da obavestis korisnika da postoje gimnasticari koji su uneti u kategorije, a
            // nisu uneti u takmicenja (pa nece biti prikazani u rezultatima)

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    takmicarskeKategorije = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO()
                        .FindByTakmicenje(takmicenjeId);
                    if (takmicarskeKategorije.Count == 0)
                        throw new BusinessException(Strings.NO_KATEGORIJE_I_TAKMICENJA_ERROR_MSG);

                    loadGimnasticari();

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                    NHibernateUtil.Initialize(takmicenje);

                    initUI();
                    tabOpened = new bool[takmicarskeKategorije.Count];
                    onSelectedIndexChanged();
                }
            }
            catch (BusinessException)
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

        private void loadGimnasticari()
        {
            gimnasticari = new List<GimnasticarUcesnik>[takmicarskeKategorije.Count];
            for (int i = 0; i < takmicarskeKategorije.Count; i++)
            {
                // TODO4: Na svim mestima gde se u petlji iznova kreira DAO (kao u
                // DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO()), kreiraj DAO izvan petlje, da se ne bi
                // bezveze gomilali objekti na heapu.
                List<GimnasticarUcesnik> gimList = new List<GimnasticarUcesnik>(
                    DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO().FindByKategorija(takmicarskeKategorije[i]));
                gimnasticari[i] = gimList;

                foreach (GimnasticarUcesnik g in gimList)
                {
                    if (g.TakmicarskiBroj.HasValue)
                    {
                        brojeviMap[g.TakmicarskiBroj.Value] = g;
                    }
                }
            }
        }

        private void initUI()
        {
            Text = "Takmicari - kategorije";
            this.ClientSize = new Size(ClientSize.Width, 550);

            statusBar = new StatusBar();
            statusBar.Parent = this;
            statusBar.ShowPanels = true;
            StatusBarPanel sbPanel1 = new StatusBarPanel();
            statusBar.Panels.Add(sbPanel1);

            initTabs();
        }

        private void initTabs()
        {
            // init first tab
            tabPage1.Text = takmicarskeKategorije[0].ToString();
            takmicariKategorijeUserControl1.DataGridViewUserControl
                .GridColumnHeaderMouseClick += 
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(dataGridViewUserControl_GridColumnHeaderMouseClick);
            takmicariKategorijeUserControl1.DataGridViewUserControl.DataGridView.MouseUp += DataGridView_MouseUp;

            // init other tabs
            for (int i = 1; i < takmicarskeKategorije.Count; i++)
            {
                TabPage newTab = new TabPage();
                tabControl1.Controls.Add(newTab);
                initTab(i, newTab, takmicarskeKategorije[i]);
            }
        }

        void DataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView dgw = (sender as DataGridView);
            if (e.Button == MouseButtons.Right && dgw.HitTest(e.X, e.Y).Type == DataGridViewHitTestType.Cell)
            {
                mnPromeniBrojeve.Enabled = takmicenje.TakBrojevi;
                mnPonistiBrojeve.Enabled = takmicenje.TakBrojevi;
                contextMenuStrip1.Show(dgw, new Point(e.X, e.Y));
            }
        }

        void dataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<GimnasticarUcesnik>(e.DataGridViewCellMouseEventArgs);
        }

        private void initTab(int i, TabPage tabPage, TakmicarskaKategorija takmicarskaKategorija)
        {
            TakmicariKategorijeUserControl takmicariKategorijeUserControl = 
                new TakmicariKategorijeUserControl();

            takmicariKategorijeUserControl.Anchor = this.takmicariKategorijeUserControl1.Anchor;
            takmicariKategorijeUserControl.Location = this.takmicariKategorijeUserControl1.Location;
            takmicariKategorijeUserControl.Size = this.takmicariKategorijeUserControl1.Size;
            takmicariKategorijeUserControl.TabIndex = this.takmicariKategorijeUserControl1.TabIndex;
            takmicariKategorijeUserControl.DataGridViewUserControl
                .GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(dataGridViewUserControl_GridColumnHeaderMouseClick);
            takmicariKategorijeUserControl.DataGridViewUserControl.DataGridView.MouseUp += DataGridView_MouseUp;
            if (!takmicenje.TakBrojevi)
                takmicariKategorijeUserControl.DataGridViewUserControl.HideColumn(0);

            tabPage.SuspendLayout();    // NOTE: ovo je obavezno, jer bez toga naredba
                // tabPage.Controls.Add(takmicariKategorijeUserControl) pozicionira
                // user controlu unutar taba pre nego sto je zavrsena inicijalizacija 
                // taba, i kao rezultat se dobija pogresna pozicija
            tabPage.Controls.Add(takmicariKategorijeUserControl);
            tabPage.Location = this.tabPage1.Location;
            tabPage.Padding = this.tabPage1.Padding;
            tabPage.Size = this.tabPage1.Size;
            tabPage.TabIndex = i;
            tabPage.Text = takmicarskaKategorija.ToString();
            tabPage.UseVisualStyleBackColor = this.tabPage1.UseVisualStyleBackColor;
            tabPage.ResumeLayout(false);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            onSelectedIndexChanged();
        }

        private void onSelectedIndexChanged()
        {
            if (tabOpened[tabControl1.SelectedIndex])
            {
                updateGimnasticariCount();
                return;
            }

            tabOpened[tabControl1.SelectedIndex] = true;
            setGimnasticari(gimnasticari[tabControl1.SelectedIndex]);

            List<GimnasticarUcesnik> gimList = getActiveDataGridViewUserControl().getItems<GimnasticarUcesnik>();
            if (takmicenje.TakBrojevi && hasAnyTakBroj(gimList))
            {
                getActiveDataGridViewUserControl().sort<GimnasticarUcesnik>(
                  new string[] { "TakmicarskiBroj" },
                  new ListSortDirection[] { ListSortDirection.Ascending });
            }
            else
            {
                getActiveDataGridViewUserControl().sort<GimnasticarUcesnik>(
                    new string[] { "Prezime", "Ime" },
                    new ListSortDirection[] { ListSortDirection.Ascending, ListSortDirection.Ascending });
            }
            updateGimnasticariCount();
        }

        private void updateGimnasticariCount()
        {
            int count = getActiveDataGridViewUserControl().getItems<GimnasticarUcesnik>().Count;
            if (count == 1)
                statusBar.Panels[0].Text = count.ToString() + " gimnasticar";
            else
                statusBar.Panels[0].Text = count.ToString() + " gimnasticara";
        }

        private void setGimnasticari(List<GimnasticarUcesnik> gimnasticari)
        {
            getActiveDataGridViewUserControl().setItems<GimnasticarUcesnik>(gimnasticari);
        }

        private DataGridViewUserControl getActiveDataGridViewUserControl()
        {
            foreach (Control c in tabControl1.SelectedTab.Controls)
            {
                TakmicariKategorijeUserControl c2 = c as TakmicariKategorijeUserControl;
                if (c2 != null)
                    return c2.DataGridViewUserControl;
            }
            return null;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            addCmd();
        }

        private void addCmd()
        {
            NacinIzboraGimnasticaraForm form2 = new NacinIzboraGimnasticaraForm();
            if (form2.ShowDialog() != DialogResult.OK)
                return;

            List<GimnasticarUcesnik> selGimnasticari = new List<GimnasticarUcesnik>();

            DialogResult dlgResult = DialogResult.None;
            SelectGimnasticariPrethTakmForm form3 = null;
            SelectGimnasticarForm form = null;
            if (form2.IzPrethodnogTakmicenja)
            {
                try
                {
                    form3 = new SelectGimnasticariPrethTakmForm(takmicenje.Gimnastika, false);
                    dlgResult = form3.ShowDialog();
                }
                catch (InfrastructureException ex)
                {
                    MessageDialogs.showError(ex.Message, this.Text);
                }
                if (dlgResult != DialogResult.OK || form3.SelectedGimnasticari.Count == 0)
                    return;
            }
            else
            {
                try
                {
                    form = new SelectGimnasticarForm(takmicenje.Gimnastika);
                    dlgResult = form.ShowDialog();
                }
                catch (InfrastructureException ex)
                {
                    MessageDialogs.showError(ex.Message, this.Text);
                }

                if (dlgResult != DialogResult.OK || form.SelectedEntities.Count == 0)
                    return;
            }

            bool added = false;
            List<GimnasticarUcesnik> okGimnasticari = new List<GimnasticarUcesnik>();
            List<GimnasticarUcesnik> illegalGimnasticari = new List<GimnasticarUcesnik>();
            
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    if (form2.IzPrethodnogTakmicenja)
                    {
                        foreach (GimnasticarUcesnik g in form3.SelectedGimnasticari)
                            selGimnasticari.Add(GimnasticarUcesnikService.createGimnasticarUcesnik(g, ActiveKategorija));
                    }
                    else
                    {
                        foreach (Gimnasticar g in form.SelectedEntities)
                            selGimnasticari.Add(GimnasticarUcesnikService.createGimnasticarUcesnik(g, ActiveKategorija));
                    }

                    foreach (GimnasticarUcesnik g in selGimnasticari)
                    {
                        if (canAddGimnasticar(g, ActiveKategorija))
                            okGimnasticari.Add(g);
                        else
                            illegalGimnasticari.Add(g);
                    }

                    GimnasticarUcesnikDAO gimUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
                    foreach (GimnasticarUcesnik g in okGimnasticari)
                        gimUcesnikDAO.Add(g);

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
            
            if (okGimnasticari.Count > 0)
            {
                List<GimnasticarUcesnik> activeGimnasticari = gimnasticari[tabControl1.SelectedIndex];
                foreach (GimnasticarUcesnik g in okGimnasticari)
                    activeGimnasticari.Add(g);

                setGimnasticari(activeGimnasticari);

                // NOTE: Ako je DataGridViewUserControl sortiran, tada metod setItems
                // (koga poziva metod setGimnasticari) osvezava prikaz (poziva 
                // refreshItems). Ako nije sortiran, potrebno je eksplicitno osveziti 
                // prikaz
                // Dodatni razlog zasto je potrebno osveziti prikaz je cinjenica da
                // se metod setItems poziva sa parametrom tipa List, a u tom slucaju
                // DataGridViewUserControl ne kreira novu listu za DataSource i nece 
                // automatski osveziti prikaz.

                if (!getActiveDataGridViewUserControl().isSorted())
                    getActiveDataGridViewUserControl().refreshItems();

                getActiveDataGridViewUserControl().setSelectedItem<GimnasticarUcesnik>
                    (okGimnasticari[okGimnasticari.Count - 1]);
                updateGimnasticariCount();
            }

            if (illegalGimnasticari.Count > 0)
            {
                string msg = "Sledeci gimnasticari vec postoje medju " +
                    "prijavljenim gimansticarima: \n\n";
                msg += StringUtil.getListString(illegalGimnasticari.ToArray());
                MessageDialogs.showMessage(msg, this.Text);
            }
        }

        private bool canAddGimnasticar(GimnasticarUcesnik g, TakmicarskaKategorija kategorija)
        {
            foreach (IList<GimnasticarUcesnik> gimList in gimnasticari)
            {
                foreach (GimnasticarUcesnik g2 in gimList)
                {
                    if (g2.Equals(g))
                        return false;
                }
            }
            return true;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            editCmd();
        }

        private void editCmd()
        {
            IList<GimnasticarUcesnik> selItems = getActiveDataGridViewUserControl()
                .getSelectedItems<GimnasticarUcesnik>();
            if (selItems.Count != 1)
                return;

            GimnasticarUcesnik selectedItem = selItems[0];
            List<GimnasticarUcesnik> activeGimnasticari = gimnasticari[tabControl1.SelectedIndex];

            int index = activeGimnasticari.IndexOf(selectedItem);

            try
            {
                Nullable<short> oldBroj = selectedItem.TakmicarskiBroj;
                GimnasticarUcesnikForm form =
                    new GimnasticarUcesnikForm(selectedItem.Id, ActiveKategorija, takmicenje.Gimnastika, brojeviMap);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    GimnasticarUcesnik editedItem = (GimnasticarUcesnik)form.Entity;
                    activeGimnasticari[index] = editedItem;

                    // Ovde postoji mogucnost da su oldBroj i editedItem.TakmicarskiBroj ista vrednost, tako da cemo
                    // ukloniti i dodati ponovo isti kljuc, sto je reduntantno ali ne utice na korektnost.
                    if (!oldBroj.HasValue && !editedItem.TakmicarskiBroj.HasValue)
                    { 
                        // do nothing
                    }
                    else if (!oldBroj.HasValue && editedItem.TakmicarskiBroj.HasValue)
                    {
                        brojeviMap.Add(editedItem.TakmicarskiBroj.Value, editedItem);
                    }
                    else if (oldBroj.HasValue && !editedItem.TakmicarskiBroj.HasValue)
                    {
                        brojeviMap.Remove(oldBroj.Value);
                    }
                    else // oldBroj.HasValue && editedItem.TakmicarskiBroj.HasValue
                    {
                        if (oldBroj.Value != editedItem.TakmicarskiBroj.Value)
                        {
                            brojeviMap.Remove(oldBroj.Value);
                            brojeviMap.Add(editedItem.TakmicarskiBroj.Value, editedItem);
                        }
                    }

                    setGimnasticari(activeGimnasticari);
                    if (!getActiveDataGridViewUserControl().isSorted())
                        getActiveDataGridViewUserControl().refreshItems();
                    getActiveDataGridViewUserControl()
                        .setSelectedItem<GimnasticarUcesnik>(editedItem);
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            deleteCmd();
        }

        private void deleteCmd()
        {
            IList<GimnasticarUcesnik> selItems = getActiveDataGridViewUserControl().getSelectedItems<GimnasticarUcesnik>();
            if (selItems.Count == 0)
                return;

            string msg;
            if (selItems.Count == 1)
                msg = deleteConfirmationMessage(selItems[0].ImeSrednjeImePrezimeDatumRodjenja);
            else
                msg = deleteConfirmationMessage();
            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;

            if (selItems.Count == 1)
            {
                msg = String.Format("Bice izbrisane sve ocene za gimnasticara \"{0}\".",
                    selItems[0].ImeSrednjeImePrezimeDatumRodjenja);
            }
            else
                msg = "Bice izbrisane sve ocene za selektovane gimnasticare.";
            msg += " Da li zelite da nastavite?";
            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;

            List<GimnasticarUcesnik> activeGimnasticari = gimnasticari[tabControl1.SelectedIndex];
            foreach (GimnasticarUcesnik g in selItems)
            {
                try
                {
                    deleteGimnasticar(g);
                    activeGimnasticari.Remove(g);
                }
                catch (Exception ex)
                {
                    MessageDialogs.showError(ex.Message, this.Text);
                }
            }

            setGimnasticari(activeGimnasticari);
            if (!getActiveDataGridViewUserControl().isSorted())
                getActiveDataGridViewUserControl().refreshItems();
            updateGimnasticariCount();
        }

        private void deleteGimnasticar(GimnasticarUcesnik g)
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

                    GimnasticarUcesnikDAO gimUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
                    gimUcesnikDAO.Attach(g, false);

                    StartListaNaSpraviDAO startListaDAO = DAOFactoryFactory.DAOFactory.GetStartListaNaSpraviDAO();
                    EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();
                    RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
                    Takmicenje1DAO tak1DAO = DAOFactoryFactory.DAOFactory.GetTakmicenje1DAO();
                    Takmicenje3DAO tak3DAO = DAOFactoryFactory.DAOFactory.GetTakmicenje3DAO();
                    OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();

                    // Izbaci gimnasticara iz start lista
                    foreach (StartListaNaSpravi s in startListaDAO.FindByGimnasticar(g))
                    {
                        s.removeNastup(g);
                        startListaDAO.Update(s);
                    }

                    // Izbaci gimnasticara iz ekipa
                    foreach (Ekipa e in ekipaDAO.FindByGimnasticar(g))
                    {
                        e.removeGimnasticar(g);
                        ekipaDAO.Update(e);

                        RezultatskoTakmicenje rt = rezTakDAO.FindByEkipa(e);
                        rt.Takmicenje1.updateRezultatiOnEkipaUpdated(e, rt,
                            RezultatskoTakmicenjeService.findRezultatiUkupnoForEkipa(rt.Takmicenje.Id, e));
                        tak1DAO.Update(rt.Takmicenje1);
                    }

                    IList<Ocena> ocene1 = ocenaDAO.FindByGimnasticar(g, DeoTakmicenjaKod.Takmicenje1);
                    IList<Ocena> ocene3 = ocenaDAO.FindByGimnasticar(g, DeoTakmicenjaKod.Takmicenje3);

                    // Izbaci gimnasticara iz takmicenja 1
                    foreach (RezultatskoTakmicenje rt in rezTakDAO.FindByGimnasticar(g))
                    {
                        rt.Takmicenje1.removeGimnasticar(g);

                        // Izbaci gimnasticara iz svih poredaka na kojima je vezbao.
                        rt.Takmicenje1.updateRezultatiOnGimnasticarDeleted(g, ocene1, rt);

                        tak1DAO.Update(rt.Takmicenje1);
                        foreach (GimnasticarUcesnik g2 in rt.Takmicenje1.Gimnasticari)
                            gimUcesnikDAO.Evict(g2);
                    }

                    // Izbaci gimnasticara iz takmicenja 3
                    foreach (RezultatskoTakmicenje rt in rezTakDAO.FindByUcesnikTak3(g))
                    {
                        rt.Takmicenje3.clearUcesnik(g);
                        foreach (PoredakSprava p in rt.Takmicenje3.Poredak)
                            p.deleteGimnasticar(g, rt);
                        rt.Takmicenje3.PoredakPreskok.deleteGimnasticar(g, rt);

                        tak3DAO.Update(rt.Takmicenje3);
                    }

                    foreach (Ocena o in ocene1)
                        ocenaDAO.Delete(o);
                    foreach (Ocena o in ocene3)
                        ocenaDAO.Delete(o);
                    
                    // TODO: Brisi takmicara iz takmicenja II i IV.

                    if (g.TakmicarskiBroj.HasValue)
                        brojeviMap.Remove(g.TakmicarskiBroj.Value);

                    gimUcesnikDAO.Delete(g);
                    if (!gimUcesnikDAO.existsGimnasticarUcesnik(g.DrzavaUcesnik))
                    {
                        DrzavaUcesnikDAO drzavaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO();
                        drzavaUcesnikDAO.Delete(g.DrzavaUcesnik);
                    }
                    if (!gimUcesnikDAO.existsGimnasticarUcesnik(g.KlubUcesnik))
                    {
                        KlubUcesnikDAO klubUcesnikDAO = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO();
                        klubUcesnikDAO.Delete(g.KlubUcesnik);
                    }

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                    takmicenje.LastModified = DateTime.Now;
                    session.Transaction.Commit();
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
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private string deleteConfirmationMessage(string gimnasticar)
        {
            return String.Format("Da li zelite da izbrisete gimnasticara \"{0}\"?", gimnasticar);
        }

        private string deleteConfirmationMessage()
        {
            return String.Format("Da li zelite da izbrisete selektovane gimnasticare?");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string nazivIzvestaja;
            // TODO5: Dodaj ovo u jezik. Korsiti istu vrednost u TakmicariTakmicenjaForm
            if (takmicenje.Gimnastika == Gimnastika.MSG)
                nazivIzvestaja = "Gimnasti" + Jezik.chMalo + "ari";
            else
                nazivIzvestaja = "Gimnasti" + Jezik.chMalo + "arke";

            HeaderFooterForm form = new HeaderFooterForm(DeoTakmicenjaKod.Takmicenje1,
                false, false, false, false, false, false, false, false, false, false, false);
            if (!Opcije.Instance.HeaderFooterInitialized)
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);

                string mestoDatum = takmicenje.Mesto + "  "
                    + takmicenje.Datum.ToShortDateString();
                form.Header1Text = takmicenje.Naziv;
                form.Header2Text = mestoDatum;
                form.Header3Text = ActiveKategorija.Naziv;
                form.Header4Text = nazivIzvestaja;
                form.FooterText = mestoDatum;
            }
            else
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);
                form.Header3Text = ActiveKategorija.Naziv;
                form.Header4Text = nazivIzvestaja;
            }

            // TODO5: Datum rodjenja prikazuj sa svih 8 cifara dd.mm.gggg, osim ako je samo godina u pitanju. I u prozoru
            // i u izvestaju.

            // Takmicari kategorije imaju poseban font size za tekst
            form.TekstFontSize = Opcije.Instance.TakmicariKategorijeFontSize;

            if (form.ShowDialog() != DialogResult.OK)
                return;

            // Azuriraj Opcije. TekstFontSize treba da ostane nepromenjen, a TakmicariKategorijeFontSize treba da dobije
            // novu vrednost
            int oldTekstFontSize = Opcije.Instance.TekstFontSize;
            FormUtil.initOpcijeFromHeaderFooterForm(form);
            Opcije.Instance.TekstFontSize = oldTekstFontSize;
            Opcije.Instance.TakmicariKategorijeFontSize = form.TekstFontSize;

            Opcije.Instance.HeaderFooterInitialized = true;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                PreviewDialog p = new PreviewDialog();

                nazivIzvestaja = nazivIzvestaja + " - " + ActiveKategorija.Naziv;
                List<GimnasticarUcesnik> gimnasticari = getActiveDataGridViewUserControl().getItems<GimnasticarUcesnik>();

                /*PropertyDescriptor propDesc =
                    TypeDescriptor.GetProperties(typeof(GimnasticarUcesnik))["KlubDrzava"];
                gimnasticari.Sort(new SortComparer<GimnasticarUcesnik>(propDesc,
                    ListSortDirection.Ascending));*/

                PropertyDescriptor[] propDesc;
                ListSortDirection[] sortDir;
                if (takmicenje.TakBrojevi && hasAnyTakBroj(gimnasticari))
                {
                    propDesc = new PropertyDescriptor[] {
                       TypeDescriptor.GetProperties(typeof(GimnasticarUcesnik))["TakmicarskiBroj"],                    };
                    sortDir = new ListSortDirection[] {
                        ListSortDirection.Ascending
                    };
                }
                else
                {
                    propDesc = new PropertyDescriptor[] {
                        TypeDescriptor.GetProperties(typeof(GimnasticarUcesnik))["DrzavaString"],
                        TypeDescriptor.GetProperties(typeof(GimnasticarUcesnik))["KlubString"],
                        TypeDescriptor.GetProperties(typeof(GimnasticarUcesnik))["Prezime"],
                        TypeDescriptor.GetProperties(typeof(GimnasticarUcesnik))["Ime"]
                    };
                    sortDir = new ListSortDirection[] {
                     ListSortDirection.Ascending,
                     ListSortDirection.Ascending,
                        ListSortDirection.Ascending,
                        ListSortDirection.Ascending
                    };
                }
                gimnasticari.Sort(new SortComparer<GimnasticarUcesnik>(propDesc, sortDir));

                p.setIzvestaj(new TakmicariIzvestaj(gimnasticari, getActiveDataGridViewUserControl().DataGridView,
                    nazivIzvestaja, takmicenje, new Font(form.TekstFont, form.TekstFontSize), form.ResizeByGrid));
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

        private bool hasAnyTakBroj(List<GimnasticarUcesnik> gimnasticari)
        {
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                if (g.TakmicarskiBroj.HasValue)
                    return true;
            }
            return false;
        }

        private void TakmicariKategorijeForm_Load(object sender, EventArgs e)
        {
            // Morao sam da prebacim ovde, jer u initTabs() nema efekta
            if (!takmicenje.TakBrojevi)
                takmicariKategorijeUserControl1.DataGridViewUserControl.HideColumn(0);

            getActiveDataGridViewUserControl().clearSelection();
        }

        private bool promeniTakmicarskiBroj(GimnasticarUcesnik g, short broj)
        {
            if (g.TakmicarskiBroj.HasValue && g.TakmicarskiBroj.Value == broj)
                return false;
            if (brojeviMap.ContainsKey(broj))
            {
                MessageDialogs.showMessage("Postoji gimnasticar sa brojem " + broj.ToString() + ".", this.Text);
                return false;
            }
            if (g.TakmicarskiBroj.HasValue)
                brojeviMap.Remove(g.TakmicarskiBroj.Value);
            g.TakmicarskiBroj = broj;
            brojeviMap.Add(g.TakmicarskiBroj.Value, g);
            return true;
        }

        private void mnPromeniBrojeve_Click(object sender, EventArgs e)
        {
            IList<GimnasticarUcesnik> selItems = getActiveDataGridViewUserControl()
                .getSelectedItems<GimnasticarUcesnik>();
            if (selItems.Count == 0)
                return;

            PromeniBrojeveForm form = new PromeniBrojeveForm();
            if (form.ShowDialog() != DialogResult.OK)
                return;

            List<GimnasticarUcesnik> promenjeniGimnasticari = new List<GimnasticarUcesnik>();
            if (form.Brojevi != null)
            {
                int n = Math.Min(form.Brojevi.Count, selItems.Count);
                for (int i = 0; i < n; ++i)
                {
                    // Vidi dole komentar zasto selItems odbrojavam unazad (n-1, n-2, ...)
                    if (promeniTakmicarskiBroj(selItems[n - 1 - i], form.Brojevi[i]))
                        promenjeniGimnasticari.Add(selItems[i]);
                }
            }
            else
            {
                short broj = form.Broj;
                // TODO5: Odbrojavam unazad zato sto iz nekog razloga selektovani itemi se nalaze u obrnutom poretku
                // (prvi selektovani se nalazi poslednji u listi selItems). Proveri zasto se ovo desava. Isto i gore.
                for (int i = selItems.Count - 1; i >= 0; --i)
                {
                    GimnasticarUcesnik g = selItems[i];
                    if (promeniTakmicarskiBroj(g, broj++))
                        promenjeniGimnasticari.Add(g);
                }
            }
            if (promenjeniGimnasticari.Count == 0)
                return;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    GimnasticarUcesnikDAO gimUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
                    foreach (GimnasticarUcesnik g in promenjeniGimnasticari)
                        gimUcesnikDAO.Update(g);

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
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            // Moram ovo da uradim jer iz nekog razloga nece da osvezi prikaz sa novim vrednostima brojeva.
            getActiveDataGridViewUserControl().clearSelection();
        }

        // TODO5: Proveri da li jos negde treba da se azurira brojeviMap (kao sto je uradjeno kod brisanja)

        // TODO5: Ne radi dobro kada se medju gimnasticarima kojima menjamo broj nalaze i oni koji vec imaju broj

        // TODO5: Dodaj u header form check box za takmicarske brojeve, cija podrazumevana vrednost treba da se
        // inicializuje iz takmicenje.TakBrojevi

        private void mnPonistiBrojeve_Click(object sender, EventArgs e)
        {
            IList<GimnasticarUcesnik> selItems = getActiveDataGridViewUserControl()
                .getSelectedItems<GimnasticarUcesnik>();
            if (selItems.Count == 0)
                return;

            string msg = "Da li zelite da izbrisete brojeve za selektovane gimnasticare?";
            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;
            
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    GimnasticarUcesnikDAO gimUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
                    foreach (GimnasticarUcesnik g in selItems)
                    {
                        if (g.TakmicarskiBroj.HasValue)
                        {
                            brojeviMap.Remove(g.TakmicarskiBroj.Value);
                            g.TakmicarskiBroj = null;
                            gimUcesnikDAO.Update(g);
                        }
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
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            // Moram ovo da uradim jer iz nekog razloga nece da osvezi prikaz sa novim vrednostima brojeva.
            getActiveDataGridViewUserControl().clearSelection();
        }
    }
}