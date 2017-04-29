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
using Iesi.Collections.Generic;
using Bilten.Util;
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;
using Bilten.Services;

namespace Bilten.UI
{
    public partial class TakmicarskeKategorijeForm : Form
    {
        private int takmicenjeId;
        private Gimnastika gimnastika;
        
        public TakmicarskeKategorijeForm(int takmicenjeId)
        {
            // TODO: Dodaj strelice umesto "Pomeri gore" i "Pomeri dole"
            InitializeComponent();
            this.takmicenjeId = takmicenjeId;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<TakmicarskaKategorija> kategorije = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO()
                        .FindByTakmicenje(takmicenjeId);
                    IList<RezultatskoTakmicenje> rezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                        .FindByTakmicenje(takmicenjeId);
                    gimnastika = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId).Gimnastika;
                    initUI(kategorije, rezTakmicenja);
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

        private void initUI(IList<TakmicarskaKategorija> kategorije, IList<RezultatskoTakmicenje> rezTakmicenja)
        {
            Text = "Takmicarske kategorije";

            lstKategorije.DisplayMember = "Naziv";
            setKategorije(kategorije);
            SelectedKategorija = null;

            setTakmicenja(rezTakmicenja);
            SelectedTakmicenje = null;
        }

        private void setTakmicenja(IList<RezultatskoTakmicenje> rezTakmicenja)
        {
            treeViewTakmicenja.Nodes.Clear();
            const string BEZVEZE = "__BEZVEZE__";

            string lastDescription = BEZVEZE;
            TreeNode descNode = null;
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                if (rt.TakmicenjeDescription.Naziv != lastDescription)
                {
                    descNode = treeViewTakmicenja.Nodes.Add(rt.TakmicenjeDescription.Naziv);
                    descNode.Tag = rt.TakmicenjeDescription;
                    lastDescription = rt.TakmicenjeDescription.Naziv;
                }
                TreeNode katNode = descNode.Nodes.Add(rt.Kategorija.Naziv);
                katNode.Tag = rt;
            }
            treeViewTakmicenja.ExpandAll();
        }

        // TODO: Kreiraj metod u klasi TakmicarskaKategorija koji vraca kategorije sortirane po rednom broju.
        // Pronadji sva mesta na kojima sortiram kategorije po rednom broju, i zameni ih pozivom novog metoda.
        // Uradi isto i za klasu RezultatskoTakmicenjeDescription, a i za druge ako postoje.

        private void setKategorije(IList<TakmicarskaKategorija> kategorije)
        {
            List<TakmicarskaKategorija> katList = new List<TakmicarskaKategorija>(kategorije);

            PropertyDescriptor propDesc = TypeDescriptor.GetProperties(typeof(TakmicarskaKategorija))["RedBroj"];
            katList.Sort(new SortComparer<TakmicarskaKategorija>(propDesc, ListSortDirection.Ascending));

            lstKategorije.DataSource = katList;
        }

        private TakmicarskaKategorija SelectedKategorija
        {
            get { return lstKategorije.SelectedItem as TakmicarskaKategorija; }
            set { lstKategorije.SelectedItem = value; }
        }

        private RezultatskoTakmicenjeDescription SelectedTakmicenje
        {
            get
            {
                foreach (TreeNode n in treeViewTakmicenja.Nodes)
                {
                    if (n.IsSelected)
                        return n.Tag as RezultatskoTakmicenjeDescription;
                }
                return null;
            }
            set
            {
                TreeNode node = null;
                if (value != null)
                {
                    foreach (TreeNode n in treeViewTakmicenja.Nodes)
                    {
                        if (n.Text == value.Naziv)
                        {
                            node = n;
                            break;
                        }
                    }
                }
                treeViewTakmicenja.SelectedNode = node;
                treeViewTakmicenja.Focus();
            }
        }


        private void btnDodajIzPrethTak_Click(object sender, EventArgs e)
        {
            OtvoriTakmicenjeForm form = null;
            try
            {
                form = new OtvoriTakmicenjeForm(1, gimnastika);
                if (form.ShowDialog() != DialogResult.OK)
                    return;
            }
            catch (Exception ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            IList<TakmicarskaKategorija> kategorije = null;
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    kategorije = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO()
                        .FindByTakmicenje(form.SelTakmicenja[0].Id);
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
            IList<int> checkedItems = new List<int>();
            for (int i = 0; i < kategorijeStr.Count; ++i)
                checkedItems.Add(i);

            CheckListForm form2 = new CheckListForm(
                kategorijeStr, checkedItems, form.SelTakmicenja[0].ToString(), "Izaberite kategorije", "Izaberite kategorije");
            if (form2.ShowDialog() != DialogResult.OK)
                return;

            session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    IList<TakmicarskaKategorija> takKategorije = new List<TakmicarskaKategorija>();
                    int numAdded = 0;
                    foreach (int index in form2.CheckedIndices)
                    {
                        try
                        {
                            TakmicenjeService.addTakmicarskaKategorija(
                                new TakmicarskaKategorija(kategorijeStr[index], gimnastika), takmicenjeId);
                            ++numAdded;
                        }
                        catch (BusinessException)
                        {
                            // ignorisi
                        }
                    }
                    if (numAdded > 0)
                    {
                        Takmicenje t = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                        t.LastModified = DateTime.Now;
                        session.Transaction.Commit();

                        // reload kategorije
                        setKategorije(DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().FindByTakmicenje(takmicenjeId));
                    }
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
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }
        private void btnAddKategorija_Click(object sender, EventArgs e)
        {
            TakmicarskaKategorijaForm form = null;
            try
            {
                form = new TakmicarskaKategorijaForm(null, takmicenjeId);
                if (form.ShowDialog() != DialogResult.OK)
                    return;
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    // reload kategorije
                    setKategorije(DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().FindByTakmicenje(takmicenjeId));
                    SelectedKategorija = (TakmicarskaKategorija)form.Entity;
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
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }


        private void btnEditKategorija_Click(object sender, EventArgs e)
        {
            if (SelectedKategorija == null)
                return;

            TakmicarskaKategorijaForm form = null;
            try
            {
                form = new TakmicarskaKategorijaForm(SelectedKategorija.Id, takmicenjeId);
                if (form.ShowDialog() != DialogResult.OK)
                    return;
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    // reload kategorije
                    setKategorije(DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().FindByTakmicenje(takmicenjeId));
                    SelectedKategorija = (TakmicarskaKategorija)form.Entity;
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
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void btnDeleteKategorija_Click(object sender, EventArgs e)
        {
            if (SelectedKategorija == null)
                return;
            string msgFmt = "Da li zelite da izbrisete kategoriju '{0}'?";
            if (!MessageDialogs.queryConfirmation(String.Format(msgFmt, SelectedKategorija), this.Text))
                return;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    deleteKategorija(SelectedKategorija);

                    Takmicenje t = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                    t.LastModified = DateTime.Now;
                    session.Transaction.Commit();

                    // reload kategorije
                    setKategorije(DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().FindByTakmicenje(takmicenjeId));
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
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void deleteKategorija(TakmicarskaKategorija k)
        {
            TakmicarskaKategorijaDAO takKatDAO = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO();
            takKatDAO.Attach(k, false);

            IList<RezultatskoTakmicenje> rezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                .FindByKategorija(k);
            if (rezTakmicenja.Count > 0)
                throw new BusinessException("Morate najpre da izbrisete kategoriju iz svih takmicenja.");
            
            IList<GimnasticarUcesnik> gimnasticari = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO()
                .FindByKategorija(k);
            if (gimnasticari.Count > 0)
                throw new BusinessException("Morate najpre da izbrisete sve gimnasticare za datu kategoriju.");

            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            Takmicenje t = takmicenjeDAO.FindById(takmicenjeId);
            t.removeKategorija(SelectedKategorija);

            takKatDAO.Delete(k);
            takmicenjeDAO.Update(t);
        }

        private void btnMoveUpKategorija_Click(object sender, EventArgs e)
        {
            TakmicarskaKategorija k = SelectedKategorija;
            if (k == null)
                return;
            if (treeViewTakmicenja.Nodes.Count > 0)
            {
                MessageDialogs.showMessage("Kategorije je moguce pomerati pre nego sto se dodaju takmicenja.", this.Text);
                return;
            }

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().Attach(k, false);
                    TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
                    Takmicenje takmicenje = takmicenjeDAO.FindById(takmicenjeId);
                    if (takmicenje.moveKategorijaUp(k))
                    {
                        takmicenjeDAO.Update(takmicenje);

                        takmicenje.LastModified = DateTime.Now;
                        session.Transaction.Commit();

                        // reload kategorije
                        setKategorije(DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().FindByTakmicenje(takmicenjeId));
                        SelectedKategorija = k;
                    }
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
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void btnMoveDownKategorija_Click(object sender, EventArgs e)
        {
            TakmicarskaKategorija k = SelectedKategorija;
            if (k == null)
                return;
            if (treeViewTakmicenja.Nodes.Count > 0)
            {
                MessageDialogs.showMessage("Kategorije je moguce pomerati pre nego sto se dodaju takmicenja.", this.Text);
                return;
            }

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().Attach(k, false);
                    TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
                    Takmicenje takmicenje = takmicenjeDAO.FindById(takmicenjeId);
                    if (takmicenje.moveKategorijaDown(k))
                    {
                        takmicenjeDAO.Update(takmicenje);

                        takmicenje.LastModified = DateTime.Now;
                        session.Transaction.Commit();

                        // reload kategorije
                        setKategorije(DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().FindByTakmicenje(takmicenjeId));
                        SelectedKategorija = k;
                    }
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
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void btnAddTakmicenje_Click(object sender, EventArgs e)
        {
            if (lstKategorije.Items.Count == 0)
            {
                MessageDialogs.showMessage("Morate najpre da unesete kategorije.", this.Text);
                return;
            }

            RezultatskoTakmicenjeDescriptionForm form;
            try
            {
                form = new RezultatskoTakmicenjeDescriptionForm(null, takmicenjeId);
                if (form.ShowDialog() != DialogResult.OK)
                    return;
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    // reload rez. takmicenja
                    RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
                    setTakmicenja(rezTakDAO.FindByTakmicenje(takmicenjeId));
                    SelectedTakmicenje = (RezultatskoTakmicenjeDescription)form.Entity;
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
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void btnEditTakmicenje_Click(object sender, EventArgs e)
        {
            if (SelectedTakmicenje == null)
                return;

            RezultatskoTakmicenjeDescriptionForm form;
            try
            {
                form = new RezultatskoTakmicenjeDescriptionForm(SelectedTakmicenje.Id, takmicenjeId);
                if (form.ShowDialog() != DialogResult.OK)
                    return;
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    // reload rez. takmicenja
                    RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
                    setTakmicenja(rezTakDAO.FindByTakmicenje(takmicenjeId));
                    SelectedTakmicenje = (RezultatskoTakmicenjeDescription)form.Entity;
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
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void btnDeleteTakmicenje_Click(object sender, EventArgs e)
        {
            RezultatskoTakmicenjeDescription desc = SelectedTakmicenje;
            if (desc == null)
                return;

            string msgFmt = "Da li zelite da izbrisete takmicenje '{0}'?";
            if (!MessageDialogs.queryConfirmation(String.Format(msgFmt, desc.Naziv), this.Text))
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
                    deleteTakmicenje(desc);

                    Takmicenje t = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                    t.LastModified = DateTime.Now;
                    session.Transaction.Commit();

                    // reload rez. takmicenja
                    setTakmicenja(DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO().FindByTakmicenje(takmicenjeId));
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
        }

        private void deleteTakmicenje(RezultatskoTakmicenjeDescription desc)
        {
            RezultatskoTakmicenjeDescriptionDAO rezTakDescDAO
                = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDescriptionDAO();
            rezTakDescDAO.Attach(desc, false);

            RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            foreach (RezultatskoTakmicenje rt in rezTakDAO.FindByDescription(desc))
                rezTakDAO.Delete(rt);

            // Ne apdejtujem redne brojeve za preostala rez. takmicenja zato sto je redosled nepromenjen

            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            Takmicenje takmicenje = takmicenjeDAO.FindById(takmicenjeId);
            takmicenje.removeTakmicenjeDescription(desc);
            takmicenjeDAO.Update(takmicenje);

            rezTakDescDAO.Delete(desc);
        }

        private void btnZatvori_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}