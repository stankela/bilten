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
using NHibernate.Context;
using Bilten.Dao;
using Bilten.Dao.NHibernate;
using Bilten.Misc;

namespace Bilten.UI
{
    public partial class OceneForm : Form
    {
        private IList<TakmicarskaKategorija> kategorije;
        private DeoTakmicenjaKod deoTakKod;
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

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    kategorije = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO()
                        .FindByTakmicenje(takmicenjeId);
                    if (kategorije.Count == 0)
                        throw new BusinessException(Strings.NO_KATEGORIJE_I_TAKMICENJA_ERROR_MSG);

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                    ocene = new Dictionary<int, List<Ocena>>();

                    initUI();
                    cmbKategorija.SelectedIndex = 0;
                    cmbSprava.SelectedIndex = 0;

                    cmbKategorija.SelectedIndexChanged += new EventHandler(selectedOceneChanged);
                    cmbSprava.SelectedIndexChanged += new EventHandler(selectedOceneChanged);

                    onSelectedOceneChanged();
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

        private void initUI()
        {
            Text = "Ocene";

            cmbKategorija.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKategorija.DataSource = kategorije;

            cmbSprava.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSprava.Items.AddRange(Sprave.getSpraveNazivi(kategorije[0].Gimnastika));

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
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    onSelectedOceneChanged();
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
                oceneList = new List<Ocena>(DAOFactoryFactory.DAOFactory.GetOcenaDAO().FindByKatSpravaDeoTak(
                    ActiveKategorija, ActiveSprava, deoTakKod));
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
                    ActiveKategorija.Gimnastika, ActiveKategorija);
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
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    ocena = DAOFactoryFactory.DAOFactory.GetOcenaDAO().FindOcena(g, deoTakKod, ActiveSprava);
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
            ocena = null;
            session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    ocena = DAOFactoryFactory.DAOFactory.GetOcenaDAO().FindByIdFetch(f.Entity.Id);
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
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
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
            Ocena editedItem = null;
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    // NOTE: Ne koristi se form.Entity nego se ponovo ucitava ocena 
                    // da bi se izbegla zavisnost od toga na koji nacin OcenaForm 
                    // ucitava ocenu, tj. da bi bio siguran da su inicijalizovane sve
                    // asocijacije kao i prilikom ucitavanja ocena u loadOcene.

                    editedItem = DAOFactoryFactory.DAOFactory.GetOcenaDAO().FindByIdFetch(selectedItem.Id);
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
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            List<Ocena> activeOcene = ocene[calculateOceneKey(ActiveKategorija, ActiveSprava)];
            activeOcene[activeOcene.IndexOf(editedItem)] = editedItem;

            setOcene(activeOcene);
            selectOcena(editedItem);
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
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    DAOFactoryFactory.DAOFactory.GetOcenaDAO().Delete(ocena);
                    Sesija.Instance.deleteOcena(ocena);

                    ISet<RezultatskoTakmicenje> rezTakSet
                        = findRezTakmicenjaForGimnasticar(ocena.Gimnasticar, takmicenje.Id, deoTakKod);
                    foreach (RezultatskoTakmicenje rezTak in rezTakSet)
                    {
                        if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                        {
                            rezTak.Takmicenje1.ocenaDeleted(ocena, rezTak, Sesija.Instance.getOcene(DeoTakmicenjaKod.Takmicenje1));
                            DAOFactoryFactory.DAOFactory.GetTakmicenje1DAO().Update(rezTak.Takmicenje1);
                        }
                        else if (deoTakKod == DeoTakmicenjaKod.Takmicenje2)
                        {
                            if (rezTak.Propozicije.OdvojenoTak2)
                            {
                                rezTak.Takmicenje2.ocenaDeleted(ocena, rezTak);
                                DAOFactoryFactory.DAOFactory.GetTakmicenje2DAO().Update(rezTak.Takmicenje2);
                            }
                        }
                        else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
                        {
                            if (rezTak.Propozicije.OdvojenoTak3)
                            {
                                rezTak.Takmicenje3.ocenaDeleted(ocena, rezTak);
                                DAOFactoryFactory.DAOFactory.GetTakmicenje3DAO().Update(rezTak.Takmicenje3);
                            }
                        }
                        else if (deoTakKod == DeoTakmicenjaKod.Takmicenje4)
                        {
                            if (rezTak.Propozicije.OdvojenoTak4)
                            {
                                rezTak.Takmicenje4.ocenaDeleted(ocena, rezTak, Sesija.Instance.getOcene(DeoTakmicenjaKod.Takmicenje4));
                                DAOFactoryFactory.DAOFactory.GetTakmicenje4DAO().Update(rezTak.Takmicenje4);
                            }
                        }
                    }

                    GimnasticarUcesnikDAO gimUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
                    UcesnikTakmicenja2DAO ucTak2DAO = DAOFactoryFactory.DAOFactory.GetUcesnikTakmicenja2DAO();
                    UcesnikTakmicenja3DAO ucTak3DAO = DAOFactoryFactory.DAOFactory.GetUcesnikTakmicenja3DAO();

                    foreach (RezultatskoTakmicenje rezTak in rezTakSet)
                    {
                        if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                        {
                            foreach (GimnasticarUcesnik g in rezTak.Takmicenje1.Gimnasticari)
                            {
                                if (gimUcesnikDAO.Contains(g))
                                    gimUcesnikDAO.Evict(g);
                            }
                        }
                        else if (deoTakKod == DeoTakmicenjaKod.Takmicenje2)
                        {
                            foreach (UcesnikTakmicenja2 u in rezTak.Takmicenje2.Ucesnici)
                            {
                                if (gimUcesnikDAO.Contains(u.Gimnasticar))
                                    gimUcesnikDAO.Evict(u.Gimnasticar);
                                ucTak2DAO.Evict(u);
                            }
                        }
                        else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
                        {
                            foreach (UcesnikTakmicenja3 u in rezTak.Takmicenje3.Ucesnici)
                            {
                                if (gimUcesnikDAO.Contains(u.Gimnasticar))
                                    gimUcesnikDAO.Evict(u.Gimnasticar);
                                ucTak3DAO.Evict(u);
                            }
                        }
                    }

                    session.Transaction.Commit();
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
                Sesija.Instance.clearOcene();
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            List<Ocena> activeOcene = ocene[calculateOceneKey(ActiveKategorija, ActiveSprava)];
            activeOcene.Remove(ocena);

            setOcene(activeOcene);
        }

        private ISet<RezultatskoTakmicenje> findRezTakmicenjaForGimnasticar(GimnasticarUcesnik g, int takmicenjeId,
            DeoTakmicenjaKod deoTakKod)
        {
            RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            ISet<RezultatskoTakmicenje> result
                = new HashSet<RezultatskoTakmicenje>(rezTakDAO.FindRezTakmicenjaForGimnasticar(g));
            foreach (RezultatskoTakmicenje rt in rezTakDAO.FindEkipnaTakmicenja(takmicenjeId))
            {
                if (rt.ucestvujeEkipno(g, deoTakKod))
                    result.Add(rt);
            }
            return result;
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