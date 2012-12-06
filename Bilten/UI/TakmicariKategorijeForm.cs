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
using System.Collections;

namespace Bilten.UI
{
    public partial class TakmicariKategorijeForm : Form
    {
        private IList<TakmicarskaKategorija> takmicarskeKategorije;
        private IDataContext dataContext;
        private List<GimnasticarUcesnik>[] gimnasticari;
        private bool[] tabOpened;
        private StatusBar statusBar;

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
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                takmicarskeKategorije = loadKategorije(takmicenjeId);
                if (takmicarskeKategorije.Count == 0)
                    throw new BusinessException("Morate najpre da unesete takmicarske kategorije.");
                
                loadGimnasticari();
                
                initUI();
                tabOpened = new bool[takmicarskeKategorije.Count];
                onSelectedIndexChanged();

                //dataContext.Commit();
            }
            catch(BusinessException)
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

        private IList<TakmicarskaKategorija> loadKategorije(int takmicenjeId)
        {
            Query q = new Query();
            q.Criteria.Add(
                new Criterion("Takmicenje.Id", CriteriaOperator.Equal, takmicenjeId));
            q.OrderClauses.Add(new OrderClause("RedBroj", OrderClause.OrderClauseCriteria.Ascending));
            return dataContext.GetByCriteria<TakmicarskaKategorija>(q);
        }

        private void loadGimnasticari()
        {
            gimnasticari = new List<GimnasticarUcesnik>[takmicarskeKategorije.Count];
            for (int i = 0; i < takmicarskeKategorije.Count; i++)
            {
                List<GimnasticarUcesnik> gimList = loadGimnasticari(takmicarskeKategorije[i]);
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

        private List<GimnasticarUcesnik> loadGimnasticari(TakmicarskaKategorija kategorija)
        {
            Query q = new Query();
            q.Criteria.Add(
                new Criterion("TakmicarskaKategorija", CriteriaOperator.Equal, kategorija));
            // Nije potreban Criterion za Takmicenje zato sto TakmicarskaKategorija
            // odredjuje takmicenje (ima asocijaciju prema takmicenju)

            q.OrderClauses.Add(new OrderClause("Prezime", OrderClause.OrderClauseCriteria.Ascending));
            q.OrderClauses.Add(new OrderClause("Ime", OrderClause.OrderClauseCriteria.Ascending));
            q.FetchModes.Add(new AssociationFetch(
                "KlubUcesnik", AssociationFetchMode.Eager));
            q.FetchModes.Add(new AssociationFetch(
                "DrzavaUcesnik", AssociationFetchMode.Eager));
            return new List<GimnasticarUcesnik>(dataContext.GetByCriteria<GimnasticarUcesnik>(q));
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
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

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

                foreach (GimnasticarUcesnik g in okGimnasticari)
                    dataContext.Add(g);
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
            if (g.Gimnastika != kategorija.Gimnastika)
                return false;

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
            result.Gimnastika = g.Gimnastika;
            result.DatumRodjenja = g.DatumRodjenja;
            result.RegistarskiBroj = g.RegistarskiBroj;
            result.TakmicarskaKategorija = kategorija;
            result.Takmicenje = kategorija.Takmicenje;
            if (g.Drzava == null)
                result.DrzavaUcesnik = null;
            else
            {
                DrzavaUcesnik drzavaUcesnik = findDrzavaUcesnik(kategorija.Takmicenje.Id,
                    g.Drzava.Naziv);
                if (drzavaUcesnik == null)
                {
                    drzavaUcesnik = new DrzavaUcesnik();
                    drzavaUcesnik.Naziv = g.Drzava.Naziv;
                    drzavaUcesnik.Kod = g.Drzava.Kod;
                    drzavaUcesnik.Takmicenje = kategorija.Takmicenje;
                    dataContext.Add(drzavaUcesnik);
                }
                result.DrzavaUcesnik = drzavaUcesnik;
            }
            if (g.Klub == null)
                result.KlubUcesnik = null;
            else
            {
                KlubUcesnik klubUcesnik = findKlubUcesnik(kategorija.Takmicenje.Id,
                    g.Klub.Naziv);
                if (klubUcesnik == null)
                {
                    klubUcesnik = new KlubUcesnik();
                    klubUcesnik.Naziv = g.Klub.Naziv;
                    klubUcesnik.Kod = g.Klub.Kod;
                    klubUcesnik.Takmicenje = kategorija.Takmicenje;
                    dataContext.Add(klubUcesnik);
                }
                result.KlubUcesnik = klubUcesnik;
            }
            return result;
        }

        private GimnasticarUcesnik createGimnasticarUcesnik(GimnasticarUcesnik g,
            TakmicarskaKategorija kategorija)
        {
            GimnasticarUcesnik result = new GimnasticarUcesnik();
            result.Ime = g.Ime;
            result.SrednjeIme = g.SrednjeIme;
            result.Prezime = g.Prezime;
            result.Gimnastika = g.Gimnastika;
            result.DatumRodjenja = g.DatumRodjenja;
            result.RegistarskiBroj = g.RegistarskiBroj;
            result.TakmicarskaKategorija = kategorija;
            result.Takmicenje = kategorija.Takmicenje;
            if (g.DrzavaUcesnik == null)
                result.DrzavaUcesnik = null;
            else
            {
                DrzavaUcesnik drzavaUcesnik = findDrzavaUcesnik(kategorija.Takmicenje.Id,
                    g.DrzavaUcesnik.Naziv);
                if (drzavaUcesnik == null)
                {
                    drzavaUcesnik = new DrzavaUcesnik();
                    drzavaUcesnik.Naziv = g.DrzavaUcesnik.Naziv;
                    drzavaUcesnik.Kod = g.DrzavaUcesnik.Kod;
                    drzavaUcesnik.Takmicenje = kategorija.Takmicenje;
                    dataContext.Add(drzavaUcesnik);
                }
                result.DrzavaUcesnik = drzavaUcesnik;
            }
            if (g.KlubUcesnik == null)
                result.KlubUcesnik = null;
            else
            {
                KlubUcesnik klubUcesnik = findKlubUcesnik(kategorija.Takmicenje.Id,
                    g.KlubUcesnik.Naziv);
                if (klubUcesnik == null)
                {
                    klubUcesnik = new KlubUcesnik();
                    klubUcesnik.Naziv = g.KlubUcesnik.Naziv;
                    klubUcesnik.Kod = g.KlubUcesnik.Kod;
                    klubUcesnik.Takmicenje = kategorija.Takmicenje;
                    dataContext.Add(klubUcesnik);
                }
                result.KlubUcesnik = klubUcesnik;
            }
            return result;
        }

        private DrzavaUcesnik findDrzavaUcesnik(int takmicenjeId, string naziv)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje.Id", CriteriaOperator.Equal, takmicenjeId));
            q.Criteria.Add(new Criterion("Naziv", CriteriaOperator.Equal, naziv));
            q.Operator = QueryOperator.And;
            IList<DrzavaUcesnik> result = dataContext.GetByCriteria<DrzavaUcesnik>(q);
            if (result.Count == 0)
                return null;
            else
                return result[0];
        }

        private KlubUcesnik findKlubUcesnik(int takmicenjeId, string naziv)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje.Id", CriteriaOperator.Equal, takmicenjeId));
            q.Criteria.Add(new Criterion("Naziv", CriteriaOperator.Equal, naziv));
            q.Operator = QueryOperator.And;
            IList<KlubUcesnik> result = dataContext.GetByCriteria<KlubUcesnik>(q);
            if (result.Count == 0)
                return null;
            else
                return result[0];
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
            try
            {
                GimnasticarUcesnikForm form =
                    new GimnasticarUcesnikForm(selectedItem.Id, ActiveKategorija);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    GimnasticarUcesnik editedItem = (GimnasticarUcesnik)form.Entity;

                    List<GimnasticarUcesnik> activeGimnasticari = gimnasticari[tabControl1.SelectedIndex];
                    activeGimnasticari[activeGimnasticari.IndexOf(editedItem)] 
                        = editedItem;

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
            IList<GimnasticarUcesnik> selItems = getActiveDataGridViewUserControl()
                .getSelectedItems<GimnasticarUcesnik>();
            if (selItems.Count == 0)
                return;

            bool delete;
            if (selItems.Count == 1)
            {
                delete = MessageDialogs.queryConfirmation(
                    deleteConfirmationMessage(selItems[0]), this.Text);
            }
            else
            {
                delete = MessageDialogs.queryConfirmation(
                    deleteConfirmationMessage(), this.Text);
            }
            if (!delete)
                return;

            List<GimnasticarUcesnik> activeGimnasticari = gimnasticari[tabControl1.SelectedIndex];
            foreach (GimnasticarUcesnik g in selItems)
            {
                // TODO: Kada se brise vise takmicara, kursor u obliku pescanika se naizmenicno aktivira i deaktivira za
                // svakog gimnasticara. Izmeni da se pescanik kursor neprekidno prikazuje.
                if (deleteGimnasticar(g))
                    activeGimnasticari.Remove(g);
            }

            setGimnasticari(activeGimnasticari);
            if (!getActiveDataGridViewUserControl().isSorted())
                getActiveDataGridViewUserControl().refreshItems();
            updateGimnasticariCount();
        }

        private bool deleteGimnasticar(GimnasticarUcesnik g)
        {
            if (!canDeleteGimnasticar(g))
                return false;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                dataContext.Attach(g, false);
                IList<RezultatskoTakmicenje> rezTakmicenja = loadRezTakmicenja(g);
                foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
                {
                    rezTak.Takmicenje1.removeGimnasticar(g);

                    // najpre ucitavam sprave na kojima je gimnasticar vezbao, da bih
                    // azurirao samo te poretke. Inace bi se u metodu 
                    // Takmicenje1.gimnasticarDeleted ucitavali svi poretci (da bi se
                    // proverilo u kojima se gimnasticar nalazi) i zatim bi se svi 
                    // ponovo snimali u bazu.
                    IList sprave = loadVezbaneSpraveTak1(g);
                    rezTak.Takmicenje1.gimnasticarDeleted(g, sprave, rezTak);

                    dataContext.Save(rezTak.Takmicenje1);
                    foreach (GimnasticarUcesnik g2 in rezTak.Takmicenje1.Gimnasticari)
                      dataContext.Evict(g2);
                }

                dataContext.Delete(g);
                dataContext.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(
                    String.Format("{0} \n\n{1}", deleteErrorMessage(), ex.Message),
                    this.Text);
                return false;
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

        private bool canDeleteGimnasticar(GimnasticarUcesnik g)
        {
            // TODO: Potrebno je brisati gimnasticara i iz ekipa, rezultatskih
            // takmicenja, nastupa na spravi, ocena (i svih drugih mesta ako postoje). 
            // Takodje je potrebno obavestiti korisnika sta ce sve biti brisano.            
            // Generalno, kada zavrsis program (tj. kada model bude stabilizovan), 
            // potrebno je jos jednom pregledati sva mesta na kojima se nesto brise
            // i proveriti da li se brise sve sto treba da se brise

            if (ucesnikTakmicenja2(g))
            {
                string msg = "Nije dozvoljeno brisanje takmicara koji ucestvuje u takmicenju II.";
                MessageDialogs.showMessage(msg, this.Text);
                return false;
            }

            if (ucesnikTakmicenja3(g))
            {
                string msg = "Nije dozvoljeno brisanje takmicara koji ucestvuje u takmicenju III.";
                MessageDialogs.showMessage(msg, this.Text);
                return false;
            }

            if (hasEkipa(g))
            {
                string msg = "Nije dozvoljeno brisanje takmicara koji je clan ekipe.\n Morate najpre da izbrisete " +
                    "takmicara iz ekipe.";
                MessageDialogs.showMessage(msg, this.Text);
                return false;
            }

            if (hasNastup(g))
            {
                string msg = "Nije dozvoljeno brisanje takmicara koji je u start listi.\n Morate najpre da izbrisete " +
                    "takmicara iz svih start listi.";
                MessageDialogs.showMessage(msg, this.Text);
                return false;
            }

            // TODO: Proveri i eventualne ostale klase koje imaju asocijaciju prema GimnasticarUcesniku
            // (osim TakmicenjaI)

            if (hasOcene(g))
            {
                string msg = "Nije dozvoljeno brisanje takmicara za koga postoje unete ocene.";
                MessageDialogs.showMessage(msg, this.Text);
                return false;
            }

            return true;
        }

        private bool hasEkipa(GimnasticarUcesnik g)
        {
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                string query = @"select distinct e
                    from Ekipa e
                    join e.Gimnasticari g
                    where g.Id = :id";
                IList<Ekipa> result = dataContext.
                    ExecuteQuery<Ekipa>(QueryLanguageType.HQL, query,
                            new string[] { "id" }, new object[] { g.Id });
                return result.Count > 0;
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

        private bool hasNastup(GimnasticarUcesnik g)
        { 
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                string query = @"select distinct n
                    from NastupNaSpravi n
                    where n.Gimnasticar = :gim";
                IList<NastupNaSpravi> result = dataContext.
                    ExecuteQuery<NastupNaSpravi>(QueryLanguageType.HQL, query,
                            new string[] { "gim" }, new object[] { g });
                return result.Count > 0;
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

        private bool hasOcene(GimnasticarUcesnik g)
        {
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                IList<Ocena> result = dataContext.ExecuteNamedQuery<Ocena>(
                    "FindAllOceneForGimnasticar",
                    new string[] { "gim" },
                    new object[] { g });
                return result.Count > 0;
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

        // TODO: Nije moguce izbrisati takmicara zato sto se uvek vodi kao ucesnik takmicenja II, cak i ako ne postoji
        // odvojeno takmicenje II, verovatno zbog greske koja postoji u tom slucaju da kada ne postoji odvojeno
        // takmicenje II svi gimnasticari se oznace kao kvalifikovani. Ispravi ovo, tj. kada ne postoji odvojeno 
        // takmicenje II verovatno ne bi niko trebalo da bude oznacen kao kvalifikovan.
        // Druga mogucnost je da nije moguce brisati takmicara zato sto postoje njegovi rezultati u takmicenju II, cak i 
        // ako ne postoji odvojeno takmicenje II.
        // Mozda bi najbolje bilo da se ne dozvoli brisanje takmicara koji je ucesnik takmicenja II i III (zato sam gore
        // stavio da se prvo ispituje da li je gimnasticar ucesnik takmicenja II ili III)

        private bool ucesnikTakmicenja2(GimnasticarUcesnik g)
        {
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                string query = @"select count(*)
                    from Takmicenje2 t
                    join t.Ucesnici u
                    where u.Gimnasticar.Id = :id";
                IList result = dataContext.
                    ExecuteQuery(QueryLanguageType.HQL, query,
                            new string[] { "id" }, new object[] { g.Id });
                return (long)result[0] > 0;
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

        private bool ucesnikTakmicenja3(GimnasticarUcesnik g)
        {
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                string query = @"select count(*)
                    from Takmicenje3 t
                    join t.Ucesnici u
                    where u.Gimnasticar.Id = :id";
                IList result = dataContext.
                    ExecuteQuery(QueryLanguageType.HQL, query,
                            new string[] { "id" }, new object[] { g.Id });
                return (long)result[0] > 0;
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

        private IList<RezultatskoTakmicenje> loadRezTakmicenja(GimnasticarUcesnik g)
        {
            return dataContext.ExecuteNamedQuery<RezultatskoTakmicenje>(
                "FindRezTakmicenjaForGimnasticar",
                new string[] { "gimnasticar" },
                new object[] { g });
        }

        private IList loadVezbaneSpraveTak1(GimnasticarUcesnik g)
        {
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                return dataContext.ExecuteNamedQuery(
                    "FindVezbaneSpraveForGimnasticar",
                    new string[] { "gim", "deoTakKod" },
                    new object[] { g, DeoTakmicenjaKod.Takmicenje1 });
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

        private string deleteConfirmationMessage(GimnasticarUcesnik gimnasticar)
        {
            return String.Format("Da li zelite da izbrisete gimnasticara \"{0}\"?", gimnasticar);
        }

        private string deleteConfirmationMessage()
        {
            return String.Format("Da li zelite da izbrisete selektovane gimnasticare?");
        }

        private string deleteErrorMessage()
        {
            return "Neuspesno brisanje gimnasticara.";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}