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

            // init other tabs
            for (int i = 1; i < takmicarskeKategorije.Count; i++)
            {
                TabPage newTab = new TabPage();
                tabControl1.Controls.Add(newTab);
                initTab(i, newTab, takmicarskeKategorije[i]);
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
            // TODO: Kod u ovom metodu je prekopiran iz Designer.cs fajla. Proveri
            // da li je u Designer.cs fajlu nesto menjano, i ako jeste promeni ovde.
            TakmicariKategorijeUserControl takmicariKategorijeUserControl = 
                new TakmicariKategorijeUserControl();

            takmicariKategorijeUserControl.Anchor = this.takmicariKategorijeUserControl1.Anchor;
            takmicariKategorijeUserControl.Location = this.takmicariKategorijeUserControl1.Location;
            takmicariKategorijeUserControl.Size = this.takmicariKategorijeUserControl1.Size;
            takmicariKategorijeUserControl.TabIndex = this.takmicariKategorijeUserControl1.TabIndex;
            takmicariKategorijeUserControl.DataGridViewUserControl
                .GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(dataGridViewUserControl_GridColumnHeaderMouseClick);

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
                MessageDialogs.showMessage(ex.Message, this.Text);
                Close();
                return;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
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
            getActiveDataGridViewUserControl().sort<GimnasticarUcesnik>(
                new string[] { "Prezime", "Ime" },
                new ListSortDirection[] { ListSortDirection.Ascending, ListSortDirection.Ascending });
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
                    form3 = new SelectGimnasticariPrethTakmForm(ActiveKategorija.Gimnastika, false);
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
                    form = new SelectGimnasticarForm(ActiveKategorija.Gimnastika);
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
                        {
                            selGimnasticari.Add(createGimnasticarUcesnik(
                                g, ActiveKategorija));
                        }
                    }
                    else
                    {
                        foreach (Gimnasticar g in form.SelectedEntities)
                        {
                            selGimnasticari.Add(createGimnasticarUcesnik(
                                g, ActiveKategorija));
                        }
                    }

                    foreach (GimnasticarUcesnik g in selGimnasticari)
                    {
                        //GimnasticarUcesnik gimnasticar = createGimnasticarUcesnik(
                        //    g, ActiveKategorija);
                        if (canAddGimnasticar(g/*imnasticar*/, ActiveKategorija))
                            okGimnasticari.Add(g/*imnasticar*/);
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
                {
                    activeGimnasticari.Add(g);
                }

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

        private GimnasticarUcesnik createGimnasticarUcesnik(Gimnasticar g,
            TakmicarskaKategorija kategorija)
        {
            GimnasticarUcesnik result = new GimnasticarUcesnik();
            result.Ime = g.Ime;
            result.SrednjeIme = g.SrednjeIme;
            result.Prezime = g.Prezime;
            result.DatumRodjenja = g.DatumRodjenja;
            result.TakmicarskaKategorija = kategorija;
            if (g.Drzava == null)
                result.DrzavaUcesnik = null;
            else
            {
                DrzavaUcesnikDAO drzavaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO();
                DrzavaUcesnik drzavaUcesnik = drzavaUcesnikDAO.FindDrzavaUcesnik(kategorija.Takmicenje.Id, g.Drzava.Naziv);
                if (drzavaUcesnik == null)
                {
                    drzavaUcesnik = new DrzavaUcesnik();
                    drzavaUcesnik.Naziv = g.Drzava.Naziv;
                    drzavaUcesnik.Kod = g.Drzava.Kod;
                    drzavaUcesnik.Takmicenje = kategorija.Takmicenje;
                    drzavaUcesnikDAO.Add(drzavaUcesnik);
                }
                result.DrzavaUcesnik = drzavaUcesnik;
            }
            if (g.Klub == null)
                result.KlubUcesnik = null;
            else
            {
                KlubUcesnikDAO klubUcesnikDAO = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO();
                KlubUcesnik klubUcesnik = klubUcesnikDAO.FindKlubUcesnik(kategorija.Takmicenje.Id, g.Klub.Naziv);
                if (klubUcesnik == null)
                {
                    klubUcesnik = new KlubUcesnik();
                    klubUcesnik.Naziv = g.Klub.Naziv;
                    klubUcesnik.Kod = g.Klub.Kod;
                    klubUcesnik.Takmicenje = kategorija.Takmicenje;
                    klubUcesnikDAO.Add(klubUcesnik);
                }
                result.KlubUcesnik = klubUcesnik;
            }
            return result;
        }

        // TODO4: Dodati srednje ime u tabelama za gimnasticare ucesnike

        private GimnasticarUcesnik createGimnasticarUcesnik(GimnasticarUcesnik g,
            TakmicarskaKategorija kategorija)
        {
            GimnasticarUcesnik result = new GimnasticarUcesnik();
            result.Ime = g.Ime;
            result.SrednjeIme = g.SrednjeIme;
            result.Prezime = g.Prezime;
            result.DatumRodjenja = g.DatumRodjenja;
            result.TakmicarskaKategorija = kategorija;
            if (g.DrzavaUcesnik == null)
                result.DrzavaUcesnik = null;
            else
            {
                DrzavaUcesnikDAO drzavaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO();
                DrzavaUcesnik drzavaUcesnik = drzavaUcesnikDAO.FindDrzavaUcesnik(kategorija.Takmicenje.Id,
                    g.DrzavaUcesnik.Naziv);
                if (drzavaUcesnik == null)
                {
                    drzavaUcesnik = new DrzavaUcesnik();
                    drzavaUcesnik.Naziv = g.DrzavaUcesnik.Naziv;
                    drzavaUcesnik.Kod = g.DrzavaUcesnik.Kod;
                    drzavaUcesnik.Takmicenje = kategorija.Takmicenje;
                    drzavaUcesnikDAO.Add(drzavaUcesnik);
                }
                result.DrzavaUcesnik = drzavaUcesnik;
            }
            if (g.KlubUcesnik == null)
                result.KlubUcesnik = null;
            else
            {
                KlubUcesnikDAO klubUcesnikDAO = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO();
                KlubUcesnik klubUcesnik = klubUcesnikDAO.FindKlubUcesnik(kategorija.Takmicenje.Id, g.KlubUcesnik.Naziv);
                if (klubUcesnik == null)
                {
                    klubUcesnik = new KlubUcesnik();
                    klubUcesnik.Naziv = g.KlubUcesnik.Naziv;
                    klubUcesnik.Kod = g.KlubUcesnik.Kod;
                    klubUcesnik.Takmicenje = kategorija.Takmicenje;
                    klubUcesnikDAO.Add(klubUcesnik);
                }
                result.KlubUcesnik = klubUcesnik;
            }
            return result;
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
                GimnasticarUcesnikForm form =
                    new GimnasticarUcesnikForm(selectedItem.Id, ActiveKategorija);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    GimnasticarUcesnik editedItem = (GimnasticarUcesnik)form.Entity;
                    activeGimnasticari[index] = editedItem;

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

                    IList<Ocena> ocene = ocenaDAO.FindByGimnasticar(g, DeoTakmicenjaKod.Takmicenje1);

                    // Izbaci gimnasticara iz rez. takmicenja
                    foreach (RezultatskoTakmicenje rt in rezTakDAO.FindByGimnasticar(g))
                    {
                        rt.Takmicenje1.removeGimnasticar(g);

                        // Izbaci gimnasticara iz svih poredaka na kojima je vezbao.
                        rt.Takmicenje1.updateRezultatiOnGimnasticarDeleted(g, ocene, rt);

                        tak1DAO.Update(rt.Takmicenje1);
                        foreach (GimnasticarUcesnik g2 in rt.Takmicenje1.Gimnasticari)
                            gimUcesnikDAO.Evict(g2);
                    }

                    foreach (Ocena o in ocene)
                        ocenaDAO.Delete(o);
                    
                    // TODO4: Brisi takmicara iz takmicenja II i III.
                    
                    gimUcesnikDAO.Delete(g);

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
            //char shVeliko = '\u0160';
            char chMalo = '\u010d';
            string nazivIzvestaja;
            if (takmicenje.Gimnastika == Gimnastika.MSG)
                nazivIzvestaja = "Gimnasti" + chMalo + "ari";
            else
                nazivIzvestaja = "Gimnasti" + chMalo + "arke";

            HeaderFooterForm form = new HeaderFooterForm(DeoTakmicenjaKod.Takmicenje1,
                false, false, false, false, false, false, false);
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

            if (form.ShowDialog() != DialogResult.OK)
                return;
            FormUtil.initHeaderFooterFromForm(form);
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


                PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                    TypeDescriptor.GetProperties(typeof(GimnasticarUcesnik))["DrzavaString"],
                    TypeDescriptor.GetProperties(typeof(GimnasticarUcesnik))["KlubString"],
                    TypeDescriptor.GetProperties(typeof(GimnasticarUcesnik))["Prezime"],
                    TypeDescriptor.GetProperties(typeof(GimnasticarUcesnik))["Ime"]
                };
                ListSortDirection[] sortDir = new ListSortDirection[] {
                    ListSortDirection.Ascending,
                    ListSortDirection.Ascending,
                    ListSortDirection.Ascending,
                    ListSortDirection.Ascending
                };
                gimnasticari.Sort(new SortComparer<GimnasticarUcesnik>(propDesc, sortDir));

                p.setIzvestaj(new TakmicariIzvestaj(gimnasticari,
                    takmicenje.Gimnastika, getActiveDataGridViewUserControl().DataGridView, nazivIzvestaja));
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