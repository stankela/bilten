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
    public partial class OceneForm : Form
    {
        private IList<TakmicarskaKategorija> kategorije;
        private DeoTakmicenjaKod deoTakKod;
        private IDataContext dataContext;
        private Takmicenje takmicenje;

        // kljuc je kategorije.IndexOf(kategorija) * (Sprava.Max + 1) + sprava
        private Dictionary<int, List<Ocena>> ocene;
        private int normalHeight;

        private TakmicarskaKategorija ActiveKategorija
        {
            get { return cmbKategorija.SelectedItem as TakmicarskaKategorija; }
            set { cmbKategorija.SelectedItem = value; }
        }

        private Sprava ActiveSprava
        {
            get { return Sprave.parse(cmbSprava.SelectedItem.ToString()); }
            set { cmbSprava.SelectedItem = Sprave.toString(value); }
        }

        public OceneForm(int takmicenjeId, DeoTakmicenjaKod deoTakKod)
        {
            InitializeComponent();
            this.deoTakKod = deoTakKod;

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                kategorije = loadKategorije(takmicenjeId);
                if (kategorije.Count == 0)
                    throw new BusinessException("Morate najpre da unesete takmicarske kategorije.");

                takmicenje = dataContext.GetById<Takmicenje>(takmicenjeId);
                ocene = new Dictionary<int, List<Ocena>>();

                initUI();
                cmbKategorija.SelectedIndex = 0;
                cmbSprava.SelectedIndex = 0;

                cmbKategorija.SelectedIndexChanged += new EventHandler(selectedOceneChanged);
                cmbSprava.SelectedIndexChanged += new EventHandler(selectedOceneChanged);

                onSelectedOceneChanged();

      //          dataContext.Commit();
            }
            catch (BusinessException)
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
            q.Criteria.Add(new Criterion("Takmicenje.Id", CriteriaOperator.Equal, takmicenjeId));
            q.OrderClauses.Add(new OrderClause("RedBroj", OrderClause.OrderClauseCriteria.Ascending));
            return dataContext.GetByCriteria<TakmicarskaKategorija>(q);
        }

        private void initUI()
        {
            Text = "Ocene";

            cmbKategorija.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKategorija.DataSource = kategorije;

            cmbSprava.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSprava.Items.AddRange(Sprave.getSpraveNazivi(kategorije[0].Pol));

            spravaGridUserControl1.DataGridViewUserControl.GridColumnHeaderMouseClick += new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);

            this.ClientSize = new Size(ClientSize.Width, 500);
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<Ocena>(e.DataGridViewCellMouseEventArgs);
        }

        private void selectedOceneChanged(object sender, EventArgs e)
        {
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();
                
                onSelectedOceneChanged();

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

        private void onSelectedOceneChanged()
        {
            initSpravaGridUserControl(ActiveSprava);
            
            List<Ocena> oceneList;
            if (gridOpened(ActiveKategorija, ActiveSprava))
            {
                oceneList = ocene[calculateOceneKey(ActiveKategorija, ActiveSprava)];
            }
            else
            {
                oceneList = loadOcene(ActiveKategorija,
                    ActiveSprava, deoTakKod);
                ocene[calculateOceneKey(ActiveKategorija, ActiveSprava)] = oceneList;
            }
            setOcene(oceneList);
        }

        private void initSpravaGridUserControl(Sprava sprava)
        {
            if (normalHeight == 0)
                normalHeight = getDataGridViewUserControl().DataGridView.RowTemplate.Height;

            spravaGridUserControl1.init(sprava);
            bool columnHeaderSorting;
            if (sprava != Sprava.Preskok)
            {
                columnHeaderSorting = true;
                getDataGridViewUserControl().DataGridView.RowTemplate.Height = normalHeight;
                //getDataGridViewUserControl().DataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            }
            else
            {
                columnHeaderSorting = false;
                getDataGridViewUserControl().DataGridView.RowTemplate.Height = 40;
                //getDataGridViewUserControl().DataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            }

            GridColumnsInitializer.initOcene(
                getDataGridViewUserControl(), takmicenje, sprava);
            getDataGridViewUserControl().ColumnHeaderSorting = columnHeaderSorting;
        }

        private DataGridViewUserControl getDataGridViewUserControl()
        {
            return spravaGridUserControl1.DataGridViewUserControl;
        }

        private bool gridOpened(TakmicarskaKategorija kategorija, Sprava sprava)
        {
            return ocene.ContainsKey(calculateOceneKey(kategorija, sprava));
        }

        // TODO: Dodati naziv grada u KlubUcesnik

        private int calculateOceneKey(TakmicarskaKategorija kat, Sprava sprava)
        {
            return kategorije.IndexOf(kat) * ((int)Sprava.Max + 1) + (int)sprava;
        }

        private List<Ocena> loadOcene(TakmicarskaKategorija kategorija, Sprava sprava, 
            DeoTakmicenjaKod deoTakKod)
        {
            string query = @"select o
                            from Ocena o
                            left join fetch o.Ocena2
                            join fetch o.Gimnasticar g
                            join fetch g.TakmicarskaKategorija kat
                            left join fetch g.DrzavaUcesnik dr
                            left join fetch g.KlubUcesnik kl
	                        where kat = :kat
                            and o.Sprava = :sprava
                            and o.DeoTakmicenjaKod = :deoTakKod";
            IList<Ocena> result = dataContext.ExecuteQuery<Ocena>(QueryLanguageType.HQL, query,
                    new string[] { "kat", "sprava", "deoTakKod" }, 
                    new object[] { kategorija, sprava, deoTakKod });
            return new List<Ocena>(result);
        }

        private Ocena loadOcena(int id)
        {
            string query = @"select o
                            from Ocena o
                            left join fetch o.Ocena2
                            join fetch o.Gimnasticar g
                            join fetch g.TakmicarskaKategorija kat
                            left join fetch g.DrzavaUcesnik dr
                            left join fetch g.KlubUcesnik kl
	                        where o.Id = :id";
            IList<Ocena> result = dataContext.ExecuteQuery<Ocena>(QueryLanguageType.HQL, query,
                    new string[] { "id" },
                    new object[] { id });
            if (result.Count == 0)
                return null;
            else
                return result[0];
        }

        private void setOcene(List<Ocena> oceneList)
        {
            getDataGridViewUserControl().setItems(oceneList);
            getDataGridViewUserControl().sort<Ocena>("PrezimeIme", ListSortDirection.Ascending);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SelectGimnasticarUcesnikForm form = null;
            try
            {
                form = new SelectGimnasticarUcesnikForm(ActiveKategorija.Takmicenje.Id, 
                    ActiveKategorija.Pol, ActiveKategorija);
                form.MultiSelect = false;
                if (form.ShowDialog() != DialogResult.OK
                || form.SelectedEntities.Count != 1)
                    return;
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            GimnasticarUcesnik g = (GimnasticarUcesnik)form.SelectedEntities[0];
            if (!ucestvuje(g, deoTakKod))
            {
                string msg = String.Format(
                    "Gimnasticar '{0}' ne ucestvuje u {1}.",
                    g.ImeSrednjeImePrezime, DeoTakmicenjaKodovi.toString(deoTakKod));
                MessageDialogs.showMessage(msg, this.Text);
                return;
            }

            
            Ocena ocena = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                ocena = findOcena(g, deoTakKod, ActiveSprava);
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
            }

            bool edit;
            OcenaForm f;
            if (ocena != null)
            {
                edit = true;
                f = new OcenaForm(ocena.Id, g, ActiveSprava, deoTakKod, takmicenje.Id);
            }
            else
            {
                edit = false;
                f = new OcenaForm(null, g, ActiveSprava, deoTakKod, takmicenje.Id);
            }
      
            try
            {
                if (f.ShowDialog() != DialogResult.OK)
                    return;
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                ocena = null;
                try
                {
                    DataAccessProviderFactory factory = new DataAccessProviderFactory();
                    dataContext = factory.GetDataContext();
                    dataContext.BeginTransaction();

                    ocena = loadOcena(f.Entity.Id);
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

                bool opened = gridOpened(g.TakmicarskaKategorija, ActiveSprava);
                showGridOcene(g.TakmicarskaKategorija, ActiveSprava);
                List<Ocena> activeOcene = ocene[calculateOceneKey(g.TakmicarskaKategorija, ActiveSprava)];
                if (!edit)
                {
                    // nova ocena se dodaje u listu activeOcene samo ako je grid vec bio 
                    // prikazivan; ako se grid prvi put prikazuje, ocene se ucitavaju i
                    // medju njima se nalazi i ona koja je upravo dodata.
                    if (opened)
                        activeOcene.Add(ocena);
                }
                else
                    activeOcene[activeOcene.IndexOf(ocena)] = ocena;

                setOcene(activeOcene);
                selectOcena(ocena);
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }
        }

        private bool ucestvuje(GimnasticarUcesnik g, DeoTakmicenjaKod deoTakKod)
        {
            // TODO2:
            return true;
        }

        private void selectOcena(Ocena ocena)
        {
            getDataGridViewUserControl().setSelectedItem<Ocena>(ocena);
        }

        private void showGridOcene(TakmicarskaKategorija kategorija, Sprava sprava)
        {
            ActiveKategorija = kategorija;
            ActiveSprava = sprava;
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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            IList<Ocena> selItems = getDataGridViewUserControl()
                .getSelectedItems<Ocena>();
            if (selItems.Count != 1)
                return;

            Ocena selectedItem = selItems[0];
            try
            {
                OcenaForm form = new OcenaForm(selectedItem.Id, 
                    selectedItem.Gimnasticar, ActiveSprava, deoTakKod, takmicenje.Id);
                if (form.ShowDialog() != DialogResult.OK)
                    return;
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                Ocena editedItem = null;
                try
                {
                    DataAccessProviderFactory factory = new DataAccessProviderFactory();
                    dataContext = factory.GetDataContext();
                    dataContext.BeginTransaction();

                    // NOTE: Ne koristi se form.Entity nego se ponovo ucitava ocena 
                    // da bi se izbegla zavisnost od toga na koji nacin OcenaForm 
                    // ucitava ocenu, tj. da bi bio siguran da su inicijalizovane sve
                    // asocijacije kao i prilikom ucitavanja ocena u loadOcene.

                    editedItem = loadOcena(selectedItem.Id);

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

                List<Ocena> activeOcene = ocene[calculateOceneKey(ActiveKategorija, ActiveSprava)];
                activeOcene[activeOcene.IndexOf(editedItem)] = editedItem;

                setOcene(activeOcene);
                selectOcena(editedItem);
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            IList<Ocena> selItems = getDataGridViewUserControl()
                .getSelectedItems<Ocena>();
            if (selItems.Count != 1)
                return;

            Ocena ocena = selItems[0];
            string msg = String.Format(
                "Da li zelite da izbrisete ocenu za gimnasticara '{0}', za spravu '{1}'?", 
                ocena.Gimnasticar.ImeSrednjeImePrezime, Sprave.toString(ActiveSprava));
            if (!MessageDialogs.queryConfirmation(msg, this.Text))
                return;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                try
                {
                    DataAccessProviderFactory factory = new DataAccessProviderFactory();
                    dataContext = factory.GetDataContext();
                    dataContext.BeginTransaction();

                    dataContext.Delete(ocena);

                    IList<RezultatskoTakmicenje> rezTakmicenja = loadRezTakmicenja(ocena.Gimnasticar);
                    foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
                    {
                        if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                        {
                            rezTak.Takmicenje1.ocenaDeleted(ocena, rezTak);
                            dataContext.Save(rezTak.Takmicenje1);
                        }
                        else if (deoTakKod == DeoTakmicenjaKod.Takmicenje2)
                        {
                            rezTak.Takmicenje2.ocenaDeleted(ocena, rezTak);
                            dataContext.Save(rezTak.Takmicenje2);
                        }
                        else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
                        {
                            rezTak.Takmicenje3.ocenaDeleted(ocena, rezTak);
                            dataContext.Save(rezTak.Takmicenje3);
                        }
                        else if (deoTakKod == DeoTakmicenjaKod.Takmicenje4)
                        {
                            rezTak.Takmicenje4.ocenaDeleted(ocena, rezTak);
                            dataContext.Save(rezTak.Takmicenje4);
                        }
                    }

                    foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
                    {
                        if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                        {
                            foreach (GimnasticarUcesnik g in rezTak.Takmicenje1.Gimnasticari)
                                dataContext.Evict(g);
                        }
                        else if (deoTakKod == DeoTakmicenjaKod.Takmicenje2)
                        {
                            foreach (UcesnikTakmicenja2 u in rezTak.Takmicenje2.Ucesnici)
                            {
                                if (dataContext.Contains(u.Gimnasticar))
                                    dataContext.Evict(u.Gimnasticar);
                                dataContext.Evict(u);
                            }
                        }
                        else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
                        {
                            foreach (UcesnikTakmicenja3 u in rezTak.Takmicenje3.Ucesnici)
                            {
                                if (dataContext.Contains(u.Gimnasticar))
                                    dataContext.Evict(u.Gimnasticar);
                                dataContext.Evict(u);
                            }
                        }
                    }

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

                List<Ocena> activeOcene = ocene[calculateOceneKey(ActiveKategorija, ActiveSprava)];
                activeOcene.Remove(ocena);

                setOcene(activeOcene);
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }
        }

        private IList<RezultatskoTakmicenje> loadRezTakmicenja(GimnasticarUcesnik g)
        {
            return dataContext.ExecuteNamedQuery<RezultatskoTakmicenje>(
                "FindRezTakmicenjaForGimnasticar",
                new string[] { "gimnasticar" },
                new object[] { g });
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cmbKategorija_DropDownClosed(object sender, EventArgs e)
        {
            getDataGridViewUserControl().Focus();
        }

        private void cmbSprava_DropDownClosed(object sender, EventArgs e)
        {
            getDataGridViewUserControl().Focus();
        }
    }
}