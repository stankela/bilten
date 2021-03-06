using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Exceptions;
using Bilten.Data;
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class GimnasticariForm : SingleEntityListForm<Gimnasticar>
    {
        private FilterGimnasticarUserControl filterGimnasticarUserControl1;

        public GimnasticariForm()
        {
            this.Text = "Gimnasticari";
            this.ClientSize = new Size(Screen.PrimaryScreen.WorkingArea.Width - 20, 540);
            this.btnPrintPreview.Visible = true;
            this.btnPrintPreview.Text = "Rezultati";
            this.btnPrintPreview.Click += btnRezultati_Click;

            filterGimnasticarUserControl1 = new FilterGimnasticarUserControl();
            this.pnlFilter.SuspendLayout();
            this.pnlFilter.Controls.Add(filterGimnasticarUserControl1);
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.Height = filterGimnasticarUserControl1.Height + 10;
            filterGimnasticarUserControl1.initialize(null);
            filterGimnasticarUserControl1.Filter += filterGimnasticarUserControl1_Filter;

            int offset = filterGimnasticarUserControl1.Bottom + 10 - btnNewItem.Location.Y;
            btnNewItem.Location = new Point(btnNewItem.Location.X, btnNewItem.Location.Y + offset);
            btnEditItem.Location = new Point(btnEditItem.Location.X, btnEditItem.Location.Y + offset);
            btnDeleteItem.Location = new Point(btnDeleteItem.Location.X, btnDeleteItem.Location.Y + offset);
            btnRefresh.Location = new Point(btnRefresh.Location.X, btnRefresh.Location.Y + offset);
            btnPrintPreview.Location = new Point(btnPrintPreview.Location.X, btnPrintPreview.Location.Y + offset);
            btnClose.Location = new Point(btnClose.Location.X, btnClose.Location.Y + offset);
    
            dataGridViewUserControl1.GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            InitializeGridColumns();
            prikaziSve();
        }

        protected override void prikaziSve()
        {
            filterGimnasticarUserControl1.Filter -= filterGimnasticarUserControl1_Filter;
            filterGimnasticarUserControl1.resetFilter();
            filterGimnasticarUserControl1.Filter += filterGimnasticarUserControl1_Filter;
            
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<Gimnasticar> gimnasticari = DAOFactoryFactory.DAOFactory.GetGimnasticarDAO().FindAll();
                    SetItems(gimnasticari);
                    updateEntityCount();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<Gimnasticar>(e.DataGridViewCellMouseEventArgs);
        }

        private void InitializeGridColumns()
        {
            AddColumn("Ime", "ImeSrednjeIme", 100);
            AddColumn("Prezime", "Prezime", 100);
            AddColumn("Datum rodjenja", "DatumRodjenja", 100, "{0:d}");
            AddColumn("Gimnastika", "Gimnastika", 70);
            AddColumn("Klub", "Klub", 150);
            AddColumn("Kategorija", "Kategorija", 100);
            AddColumn("Drzava", "Drzava", 100);
            AddColumn("Registarski broj", "RegistarskiBroj", 100);
            AddColumn("Poslednja registr.", "DatumPoslednjeRegistracije", 100, "{0:d}");
        }

        protected override EntityDetailForm createEntityDetailForm(Nullable<int> entityId)
        {
            return new GimnasticarForm(entityId);
        }

        protected override string deleteConfirmationMessage(Gimnasticar gimnasticar)
        {
            return String.Format("Da li zelite da izbrisete gimnasticara \"{0}\"?", gimnasticar);
        }

        protected override string deleteErrorMessage()
        {
            return "Neuspesno brisanje gimnasticara.";
        }

        protected override void delete(Gimnasticar g)
        {
            DAOFactoryFactory.DAOFactory.GetGimnasticarDAO().Delete(g);
        }

        private void filterGimnasticarUserControl1_Filter(object sender, EventArgs e)
        {
            GimnasticarFilter flt = filterGimnasticarUserControl1.getFilter();
            if (flt != null)
                filter(flt);
        }

        private void filter(GimnasticarFilter flt)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    IList<Gimnasticar> gimnasticari;
                    if (flt.isEmpty(true))
                        gimnasticari = DAOFactoryFactory.DAOFactory.GetGimnasticarDAO().FindAll();
                    else
                    {
                        gimnasticari = DAOFactoryFactory.DAOFactory.GetGimnasticarDAO().FindGimnasticari(
                            flt.Ime, flt.Prezime, flt.GodRodj, flt.Gimnastika, flt.Drzava, flt.Kategorija, flt.Klub);
                    }
                    SetItems(gimnasticari);
                    updateEntityCount();
                    dataGridViewUserControl1.Focus();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        protected override void AddNew()
        {
            try
            {
                GimnasticarForm form = (GimnasticarForm)createEntityDetailForm(null);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.GimnasticarToEdit == null)
                    {
                        Gimnasticar newEntity = (Gimnasticar)form.Entity;
                        List<Gimnasticar> items = dataGridViewUserControl1.getItems<Gimnasticar>();
                        items.Add(newEntity);
                        dataGridViewUserControl1.setItems<Gimnasticar>(items);
                        dataGridViewUserControl1.setSelectedItem<Gimnasticar>(newEntity);
                        updateEntityCount();
                    }
                    else
                    {
                        List<Gimnasticar> items = dataGridViewUserControl1.getItems<Gimnasticar>();
                        Gimnasticar g = form.GimnasticarToEdit;
                        if (items.IndexOf(g) == -1)
                        {
                            items.Add(g);
                            dataGridViewUserControl1.setItems<Gimnasticar>(items);
                            updateEntityCount();
                        }
                        dataGridViewUserControl1.setSelectedItem<Gimnasticar>(g);
                        Edit(g);
                    }
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }

        public override void Edit()
        {
            if (SelectedItem == null)
                return;
            int index = dataGridViewUserControl1.getSelectedItemIndex();

            try
            {
                GimnasticarForm form = (GimnasticarForm)createEntityDetailForm(SelectedItem.Id);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.GimnasticarToEdit == null)
                    {
                        Gimnasticar entity = (Gimnasticar)form.Entity;
                        List<Gimnasticar> items = dataGridViewUserControl1.getItems<Gimnasticar>();
                        items[index] = entity;
                        dataGridViewUserControl1.setItems<Gimnasticar>(items);  // ovo ponovo sortira items
                        dataGridViewUserControl1.setSelectedItem<Gimnasticar>(entity);
                    }
                    else
                    {
                        List<Gimnasticar> items = dataGridViewUserControl1.getItems<Gimnasticar>();
                        Gimnasticar g = form.GimnasticarToEdit;
                        if (items.IndexOf(g) == -1)
                        {
                            items.Add(g);
                            dataGridViewUserControl1.setItems<Gimnasticar>(items);
                            updateEntityCount();
                        }
                        dataGridViewUserControl1.setSelectedItem<Gimnasticar>(g);
                        Edit(g);
                    }                                                                                
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }

        // Ovaj metod je u sustini isti kao i Edit() metod. Poziva se kada se prilikom dodavanja novog gimnasticara ili
        // menjanja postojeceg predje u rezim rada kada se izabere novi gimnasticar ciji ce se podaci menjati. Glavni
        // razlog zasto tada pozivam ovaj metod a ne obican Edit() metod je sto postoji situacija kada naredba
        // dataGridViewUserControl1.setSelectedItem<Gimnasticar>(g) koja se poziva na kraju AddNew() i Edit() metoda
        // (u rezimu rada kada se u dijalogu izabrao novi gimnasticar ciji ce se podaci menjati) nece selektovati
        // element, i onda ce provera if (SelectedItem == null) na pocetku obicnog Edit() biti tacna i metod se nece
        // izvrsiti. A ta situacija je sledeca: kada je gimnasticar oznacen sa kursorom, a nije obojen u plavo (sto se
        // desi npr kada najpre selektujem gimnasticara, i onda kliknem van grida, tako da se boja izgubi i kursor ostaje.
        private void Edit(Gimnasticar selItem)
        {
            List<Gimnasticar> items = dataGridViewUserControl1.getItems<Gimnasticar>();
            int index = items.IndexOf(selItem);

            try
            {
                GimnasticarForm form = (GimnasticarForm)createEntityDetailForm(selItem.Id);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.GimnasticarToEdit == null)
                    {
                        Gimnasticar entity = (Gimnasticar)form.Entity;
                        items[index] = entity;
                        dataGridViewUserControl1.setItems<Gimnasticar>(items);  // ovo ponovo sortira items
                        dataGridViewUserControl1.setSelectedItem<Gimnasticar>(entity);
                    }
                    else
                    {
                        Gimnasticar g = form.GimnasticarToEdit;
                        if (items.IndexOf(g) == -1)
                        {
                            items.Add(g);
                            dataGridViewUserControl1.setItems<Gimnasticar>(items);
                            updateEntityCount();
                        }
                        dataGridViewUserControl1.setSelectedItem<Gimnasticar>(g);
                        Edit(g);
                    }
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }

        protected override void updateEntityCount()
        {
            int count = dataGridViewUserControl1.getItems<Gimnasticar>().Count;
            if (count == 1)
                StatusPanel.Panels[0].Text = count.ToString() + " gimnasticar";
            else
                StatusPanel.Panels[0].Text = count.ToString() + " gimnasticara";
        }

        void btnRezultati_Click(object sender, EventArgs e)
        {
            if (SelectedItem == null)
                return;

            // TODO4: Dodaj mogucnost izbora, ako ima vise gimnasticara ucesnika sa datim imenom i prezimenom.
            KonacanPlasmanDAO kpDAO = new KonacanPlasmanDAO();
            kpDAO.ConnectionString = ConfigurationParameters.ConnectionString;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            List<KonacanPlasman> plasmani;
            try
            {
                List<KonacanPlasman> viseboj = new List<KonacanPlasman>();
                viseboj.AddRange(kpDAO.findVisebojFinaleKupa(SelectedItem.Ime, SelectedItem.Prezime));
                viseboj.AddRange(kpDAO.findVisebojZbirViseKola(SelectedItem.Ime, SelectedItem.Prezime));
                viseboj.AddRange(kpDAO.findVisebojTak1(SelectedItem.Ime, SelectedItem.Prezime));
                viseboj.AddRange(kpDAO.findVisebojTak2(SelectedItem.Ime, SelectedItem.Prezime));

                List<KonacanPlasman> sprave = new List<KonacanPlasman>();
                // Dodajem najpre finale kupa da bi, ako je postojalo odvojeno takmicenje 3 finale kupa, rezultati prebrisali
                // ove rezultate (za one gimnasticare koji su ucestvovali u odvojenom finalu kupa). Iz istog razloga najpre
                // dodajem spraveTak1 pa spraveTak3.
                sprave.AddRange(kpDAO.findSpraveFinaleKupa(SelectedItem.Ime, SelectedItem.Prezime));
                sprave.AddRange(kpDAO.findSpraveTak1(SelectedItem.Ime, SelectedItem.Prezime));
                sprave.AddRange(kpDAO.findSpraveTak3(SelectedItem.Ime, SelectedItem.Prezime));
                sprave.AddRange(kpDAO.findPreskokTak1(SelectedItem.Ime, SelectedItem.Prezime));
                sprave.AddRange(kpDAO.findPreskokTak3(SelectedItem.Ime, SelectedItem.Prezime));

                Dictionary<int, KonacanPlasman> plasmaniMap = new Dictionary<int, KonacanPlasman>();
                foreach (KonacanPlasman kp in viseboj)
                {
                    if (plasmaniMap.ContainsKey(kp.RezultatskoTakmicenjeId))
                    {
                        if (kp.Viseboj != null)
                            plasmaniMap[kp.RezultatskoTakmicenjeId].Viseboj = kp.Viseboj;
                    }
                    else
                        plasmaniMap.Add(kp.RezultatskoTakmicenjeId, kp);
                }
                foreach (KonacanPlasman kp in sprave)
                {
                    if (plasmaniMap.ContainsKey(kp.RezultatskoTakmicenjeId))
                        updatePlasmanSprava(plasmaniMap[kp.RezultatskoTakmicenjeId], kp);
                    else
                        plasmaniMap.Add(kp.RezultatskoTakmicenjeId, kp);
                }

                plasmani = new List<KonacanPlasman>(plasmaniMap.Values);
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }

            if (plasmani.Count == 0)
            {
                MessageDialogs.showMessage("Ne postoje rezultati za gimnasticara '" + 
                    SelectedItem.ImeSrednjeImePrezimeDatumRodjenja + "'.", "Rezultati");
            }
            else
            {
                KonacanPlasmanForm form = new KonacanPlasmanForm(plasmani, SelectedItem.Gimnastika);
                form.ShowDialog();
            }
        }

        private void updatePlasmanSprava(KonacanPlasman totalPlasman, KonacanPlasman kp)
        {
            if (kp.Parter != null)
                totalPlasman.Parter = kp.Parter;
            else if (kp.Konj != null)
                totalPlasman.Konj = kp.Konj;
            else if (kp.Karike != null)
                totalPlasman.Karike = kp.Karike;
            else if (kp.Preskok != null)
                totalPlasman.Preskok = kp.Preskok;
            else if (kp.Razboj != null)
                totalPlasman.Razboj = kp.Razboj;
            else if (kp.Vratilo != null)
                totalPlasman.Vratilo = kp.Vratilo;
            else if (kp.DvovisinskiRazboj != null)
                totalPlasman.DvovisinskiRazboj = kp.DvovisinskiRazboj;
            else if (kp.Greda != null)
                totalPlasman.Greda = kp.Greda;
        }
    }
}
