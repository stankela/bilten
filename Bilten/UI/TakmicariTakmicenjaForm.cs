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
using Iesi.Collections.Generic;
using System.Collections;
using NHibernate.Context;
using Bilten.Dao;
using NHibernate;
using Bilten.Util;
using Bilten.Services;
using Bilten.Report;

namespace Bilten.UI
{
    public partial class TakmicariTakmicenjaForm : Form
    {
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private bool[] tabOpened;
        private StatusBar statusBar;
        private Takmicenje takmicenje;
        
        public TakmicariTakmicenjaForm(int takmicenjeId)
        {
            InitializeComponent();

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    rezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                        .FindByTakmicenjeFetch_Tak1_Gimnasticari(takmicenjeId);
                    if (rezTakmicenja.Count == 0)
                        throw new BusinessException(Strings.NO_KATEGORIJE_I_TAKMICENJA_ERROR_MSG);

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                    NHibernateUtil.Initialize(takmicenje);
                    
                    initUI();
                    tabOpened = new bool[rezTakmicenja.Count];
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

        private void initUI()
        {
            Text = "Takmicari - takmicenja";
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
            tabPage1.Text = rezTakmicenja[0].Naziv;
            GridColumnsInitializer.initGimnasticarUcesnik(dataGridViewUserControl1);
            dataGridViewUserControl1.GridColumnHeaderMouseClick += 
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(dataGridViewUserControl_GridColumnHeaderMouseClick);
            dataGridViewUserControl1.DataGridView.MouseUp += DataGridView_MouseUp;

            // init other tabs
            for (int i = 1; i < rezTakmicenja.Count; i++)
            {
                TabPage newTab = new TabPage();
                tabControl1.Controls.Add(newTab);
                initTab(i, newTab, rezTakmicenja[i]);
            }
        }

        void DataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView dgw = (sender as DataGridView);
            if (e.Button == MouseButtons.Right && dgw.HitTest(e.X, e.Y).Type == DataGridViewHitTestType.Cell)
            {
                mnSpraveKojeSeBoduju.Enabled = false;  // TODO4: Ovo je privremeno dok se ne ispravi greska (vidi
                                                       // odgovarajuci TODO4 u Program.cs
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

        private void initTab(int i, TabPage tabPage, RezultatskoTakmicenje rezTakmicenje)
        {
            DataGridViewUserControl dataGridViewUserControl =
                new DataGridViewUserControl();
            dataGridViewUserControl.Anchor = this.dataGridViewUserControl1.Anchor;
            dataGridViewUserControl.Location = this.dataGridViewUserControl1.Location;
            dataGridViewUserControl.Size = this.dataGridViewUserControl1.Size;
            dataGridViewUserControl.TabIndex = this.dataGridViewUserControl1.TabIndex;
            GridColumnsInitializer.initGimnasticarUcesnik(dataGridViewUserControl);
            dataGridViewUserControl.GridColumnHeaderMouseClick += new EventHandler<GridColumnHeaderMouseClickEventArgs>(dataGridViewUserControl_GridColumnHeaderMouseClick);
            dataGridViewUserControl.DataGridView.MouseUp += DataGridView_MouseUp;

            tabPage.SuspendLayout();    // NOTE: ovo je obavezno, jer bez toga naredba
            // tabPage.Controls.Add(dataGridViewUserControl) pozicionira
            // user controlu unutar taba pre nego sto je zavrsena inicijalizacija 
            // taba, i kao rezultat se dobija pogresna pozicija
            tabPage.Controls.Add(dataGridViewUserControl);
            tabPage.Location = this.tabPage1.Location;
            tabPage.Padding = this.tabPage1.Padding;
            tabPage.Size = this.tabPage1.Size;
            tabPage.TabIndex = i;
            tabPage.Text = rezTakmicenje.Naziv;
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
            setGimnasticari(ActiveRezTakmicenje.Takmicenje1.Gimnasticari);
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

        private void setGimnasticari(Iesi.Collections.Generic.ISet<GimnasticarUcesnik> gimnasticari)
        {
            getActiveDataGridViewUserControl().setItems<GimnasticarUcesnik>(gimnasticari);
        }

        private DataGridViewUserControl getActiveDataGridViewUserControl()
        {
            foreach (Control c in tabControl1.SelectedTab.Controls)
            {
                DataGridViewUserControl c2 = c as DataGridViewUserControl;
                if (c2 != null)
                    return c2;
            }
            return null;
        }

        private RezultatskoTakmicenje ActiveRezTakmicenje
        {
            get
            {
                return rezTakmicenja[tabControl1.SelectedIndex];
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult = DialogResult.None;
            SelectGimnasticarUcesnikForm form = null;
            try
            {
                form = new SelectGimnasticarUcesnikForm(
                    ActiveRezTakmicenje.Takmicenje.Id, ActiveRezTakmicenje.Gimnastika,
                    ActiveRezTakmicenje.Kategorija);
                dlgResult = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (dlgResult != DialogResult.OK || form.SelectedEntities.Count == 0)
                return;

            IList<GimnasticarUcesnik> addedGimnasticari = new List<GimnasticarUcesnik>();

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    IList<GimnasticarUcesnik> selGimnasticari = new List<GimnasticarUcesnik>();
                    foreach (GimnasticarUcesnik g in form.SelectedEntities)
                        selGimnasticari.Add(g);
                    RezultatskoTakmicenjeService.addGimnasticariToRezTak(selGimnasticari, ActiveRezTakmicenje,
                        addedGimnasticari);
                    if (addedGimnasticari.Count > 0)
                    {
                        takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                        takmicenje.LastModified = DateTime.Now;
                        session.Transaction.Commit();
                    }
                }
            }
            catch (InfrastructureException ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                return;
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

            if (addedGimnasticari.Count > 0)
            {
                setGimnasticari(ActiveRezTakmicenje.Takmicenje1.Gimnasticari);
                updateGimnasticariCount();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            IList<GimnasticarUcesnik> selItems = getActiveDataGridViewUserControl()
                .getSelectedItems<GimnasticarUcesnik>();
            if (selItems == null || selItems.Count == 0)
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

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    RezultatskoTakmicenjeService.deleteGimnasticariFromRezTak(selItems, ActiveRezTakmicenje);

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                    takmicenje.LastModified = DateTime.Now;
                    session.Transaction.Commit();
                    
                    setGimnasticari(ActiveRezTakmicenje.Takmicenje1.Gimnasticari);
                    updateGimnasticariCount();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(
                    String.Format("{0} \n\n{1}", deleteErrorMessage(), ex.Message),
                    this.Text);
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
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

        private void mnSpraveKojeSeBoduju_Click(object sender, EventArgs e)
        {
            IList<GimnasticarUcesnik> selItems = getActiveDataGridViewUserControl()
                .getSelectedItems<GimnasticarUcesnik>();
            if (selItems.Count != 1)
                return;
            GimnasticarUcesnik g = selItems[0];

            List<int> checkedItems = new List<int>();
            foreach (Sprava s in Sprave.getSprave(ActiveRezTakmicenje.Gimnastika))
            {
                if (g.getSpravaSeBoduje(s))
                    checkedItems.Add(Sprave.indexOf(s, ActiveRezTakmicenje.Gimnastika));
            }

            CheckListForm form = new CheckListForm(
                new List<string>(Sprave.getSpraveNazivi(ActiveRezTakmicenje.Gimnastika)), checkedItems,
                "Izaberite sprave koje se boduju", "Sprave koje se boduju", true, "Izaberite sprave", true);
            if (form.ShowDialog() != DialogResult.OK)
                return;

            Sprava[] sprave = Sprave.getSprave(ActiveRezTakmicenje.Gimnastika);
            IList<Sprava> spraveKojeSeBoduju = new List<Sprava>();
            g.clearSpraveKojeSeBoduju();
            foreach (int i in form.CheckedIndices)
                g.setSpravaSeBoduje(sprave[i]);

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO().Update(g);

                    PoredakSpravaDAO poredakSpravaDAO = DAOFactoryFactory.DAOFactory.GetPoredakSpravaDAO();
                    PoredakPreskokDAO poredakPreskokDAO = DAOFactoryFactory.DAOFactory.GetPoredakPreskokDAO();

                    OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
                    IList<Ocena> ocene = ocenaDAO.FindByDeoTakmicenja(takmicenje.Id, DeoTakmicenjaKod.Takmicenje1);
                    
                    foreach (Sprava s in Sprave.getSprave(ActiveRezTakmicenje.Gimnastika))
                    {
                        if (s != Sprava.Preskok)
                        {
                            PoredakSprava p = ActiveRezTakmicenje.getPoredakSprava(DeoTakmicenjaKod.Takmicenje1, s);
                            poredakSpravaDAO.Attach(p, false);
                            p.create(ActiveRezTakmicenje, ocene);
                            poredakSpravaDAO.Update(p);
                        }
                        else
                        {
                            PoredakPreskok p = ActiveRezTakmicenje.getPoredakPreskok(DeoTakmicenjaKod.Takmicenje1);
                            poredakPreskokDAO.Attach(p, false);
                            p.create(ActiveRezTakmicenje, ocene);
                            poredakPreskokDAO.Update(p);
                        }
                    }
                    foreach (Ocena o in ocene)
                        ocenaDAO.Evict(o);

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                    takmicenje.LastModified = DateTime.Now;
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
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string nazivIzvestaja;
            if (takmicenje.Gimnastika == Gimnastika.MSG)
                nazivIzvestaja = "Gimnasti" + Jezik.chMalo + "ari";
            else
                nazivIzvestaja = "Gimnasti" + Jezik.chMalo + "arke";

            HeaderFooterForm form = new HeaderFooterForm(DeoTakmicenjaKod.Takmicenje1,
                false, false, false, false, false, false, false);
            if (!Opcije.Instance.HeaderFooterInitialized)
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);

                string mestoDatum = takmicenje.Mesto + "  "
                    + takmicenje.Datum.ToShortDateString();
                form.Header1Text = takmicenje.Naziv;
                form.Header2Text = mestoDatum;
                form.Header3Text = ActiveRezTakmicenje.Naziv;
                form.Header4Text = nazivIzvestaja;
                form.FooterText = mestoDatum;
            }
            else
            {
                FormUtil.initHeaderFooterFormFromOpcije(form);
                form.Header3Text = ActiveRezTakmicenje.Naziv;
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

                nazivIzvestaja = nazivIzvestaja + " - " + ActiveRezTakmicenje.Naziv;
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