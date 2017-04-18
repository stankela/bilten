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
        private Takmicenje takmicenje;
        private IList<RezultatskoTakmicenje> rezTakmicenja;

        public TakmicarskeKategorijeForm(int takmicenjeId)
        {
            // TODO: Dodaj strelice umesto "Pomeri gore" i "Pomeri dole"

            InitializeComponent();
            btnDeleteKategorija.Enabled = false;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
                    takmicenje = takmicenjeDAO.FindByIdFetch_Kat_Desc(takmicenjeId);
                    rezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                        .FindByTakmicenjeFetch_KatDesc(takmicenje.Id);
                    if (takmicenje.TakmicenjeDescriptions.Count == 0)
                    {
                        // za novo takmicenje, automatski se dodaje takmicenje sa nazivom kao glavno takmicenje
                        RezultatskoTakmicenjeDescription desc = new RezultatskoTakmicenjeDescription();
                        desc.Naziv = takmicenje.Naziv;
                        desc.Propozicije = new Propozicije();
                        takmicenje.addTakmicenjeDescription(desc);
                        //takmicenjeDAO.Update(takmicenje);
                        //session.Transaction.Commit();
                    }
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
        
            initUI();
        }

        private void initUI()
        {
            Text = "Takmicarske kategorije";

            lstKategorije.DisplayMember = "Naziv";
            setKategorije(takmicenje.Kategorije);
            SelectedKategorija = null;

            setTakmicenja(takmicenje.TakmicenjeDescriptions);
            SelectedTakmicenje = null;
        }

        private void setTakmicenja(Iesi.Collections.Generic.ISet<RezultatskoTakmicenjeDescription> takmicenja)
        {
            List<RezultatskoTakmicenjeDescription> takList = new List<RezultatskoTakmicenjeDescription>(takmicenja);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatskoTakmicenjeDescription))["RedBroj"];
            takList.Sort(new SortComparer<RezultatskoTakmicenjeDescription>(
                propDesc, ListSortDirection.Ascending));

            treeViewTakmicenja.Nodes.Clear();

            const string BEZVEZE = "__BEZVEZE__";

            string lastDescription = BEZVEZE;
            TreeNode descNode = null;
            foreach (RezultatskoTakmicenjeDescription desc in takList)
            {
                descNode = addTakmicenje(desc, new List<TakmicarskaKategorija>(takmicenje.Kategorije));
            }
            /*foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                if (rt.TakmicenjeDescription.Naziv != lastDescription)
                {
                    lastDescription = rt.TakmicenjeDescription.Naziv;
                    descNode = treeViewTakmicenja.Nodes.Add(rt.TakmicenjeDescription.Naziv);
                }
                TreeNode katNode = descNode.Nodes.Add(rt.Kategorija.Naziv);
                katNode.Tag = rt;
            }*/
        }

        private void setKategorije(Iesi.Collections.Generic.ISet<TakmicarskaKategorija> kategorije)
        {
            List<TakmicarskaKategorija> katList = new List<TakmicarskaKategorija>(kategorije);

            // TODO: Kreiraj metod u klasi TakmicarskaKategorija koji vraca kategorije sortirane po rednom broju.
            // Pronadji sva mesta na kojima sortiram kategorije po rednom broju, i zameni ih pozivom novog metoda.
            // Uradi isto i za klasu RezultatskoTakmicenjeDescription, a i za druge ako postoje.

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(TakmicarskaKategorija))["RedBroj"];
            katList.Sort(new SortComparer<TakmicarskaKategorija>(
                propDesc, ListSortDirection.Ascending));

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

        private void btnAddKategorija_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult = DialogResult.None;
            TakmicarskaKategorijaForm form = null;
            try
            {
                form = new TakmicarskaKategorijaForm(takmicenje.Gimnastika);
                dlgResult = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }
            if (dlgResult != DialogResult.OK)
                return;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    if (takmicenje.addKategorija(new TakmicarskaKategorija(form.NazivKategorije, takmicenje.Gimnastika)))
                    {
                        DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().Update(takmicenje);
                        session.Transaction.Commit();
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

            setKategorije(takmicenje.Kategorije);
        }

        private void btnDeleteKategorija_Click(object sender, EventArgs e)
        {
            // TODO: Uradi brisanje kategorija
            if (SelectedKategorija == null)
                return;
            string msgFmt = "Da li zelite da izbrisete kategoriju '{0}'?";
            if (!MessageDialogs.queryConfirmation(String.Format(msgFmt, SelectedKategorija), this.Text))
                return;

            // TODO: Prikazi dijalog za potvrdu da ce biti izbrisane sve ocene,
            // rasporedi nastupa, rasporedi sudija, ekipe i gimnasticari za datu
            // kategoriju

            takmicenje.removeKategorija(SelectedKategorija);
            setKategorije(takmicenje.Kategorije);
        }

        private void btnMoveUpKategorija_Click(object sender, EventArgs e)
        {
            TakmicarskaKategorija k = SelectedKategorija;
            if (k == null)
                return;

            if (takmicenje.moveKategorijaUp(k))
            {
                setKategorije(takmicenje.Kategorije);
                SelectedKategorija = k;
            }
        }

        private void btnMoveDownKategorija_Click(object sender, EventArgs e)
        {
            TakmicarskaKategorija k = SelectedKategorija;
            if (k == null)
                return;

            if (takmicenje.moveKategorijaDown(k))
            {
                setKategorije(takmicenje.Kategorije);
                SelectedKategorija = k;
            }
        }

        private void btnAddTakmicenje_Click(object sender, EventArgs e)
        {
            RezultatskoTakmicenjeDescriptionForm form = new RezultatskoTakmicenjeDescriptionForm(takmicenje);
            if (form.ShowDialog() != DialogResult.OK)
                return;

            RezultatskoTakmicenjeDescription d = (RezultatskoTakmicenjeDescription)form.Entity;
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    takmicenje.addTakmicenjeDescription(d);
                    DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().Update(takmicenje);

                    RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
                    int redBroj = rezTakmicenja[rezTakmicenja.Count - 1].RedBroj + 1;
                    foreach (TakmicarskaKategorija k in form.Kategorije)
                        rezTakDAO.Add(createRezultatskoTakmicenje(takmicenje, k, d, redBroj++));

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
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            TreeNode n = addTakmicenje(d, form.Kategorije);
            n.Expand();
            SelectedTakmicenje = d;
        }

        private TreeNode addTakmicenje(RezultatskoTakmicenjeDescription desc, IList<TakmicarskaKategorija> kategorije)
        {
            TreeNode descNode = treeViewTakmicenja.Nodes.Add(desc.Naziv);
            descNode.Tag = desc;
            foreach (TakmicarskaKategorija kat in kategorije)
                descNode.Nodes.Add(kat.Naziv);
            return descNode;
        }

        private TreeNode updateTakmicenje(RezultatskoTakmicenjeDescription desc, IList<TakmicarskaKategorija> kategorije)
        {
            foreach (TreeNode n in treeViewTakmicenja.Nodes)
            {
                if (n.Text != desc.Naziv)
                    continue;
                n.Nodes.Clear();
                foreach (TakmicarskaKategorija k in kategorije)
                    n.Nodes.Add(k.Naziv);
                return n;
            }
            return null;
        }

        private void btnEditTakmicenje_Click(object sender, EventArgs e)
        {
            if (SelectedTakmicenje == null)
                return;
            try
            {
                RezultatskoTakmicenjeDescriptionForm form = new RezultatskoTakmicenjeDescriptionForm(
                    SelectedTakmicenje, getKategorije(SelectedTakmicenje), takmicenje);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // refresh
                    RezultatskoTakmicenjeDescription desc = (RezultatskoTakmicenjeDescription)form.Entity;
                    TreeNode n = updateTakmicenje(desc, form.Kategorije);
                    n.Expand();
                    SelectedTakmicenje = desc;
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
            }
        }

        private IList<TakmicarskaKategorija> getKategorije(RezultatskoTakmicenjeDescription desc)
        {
            IList<TakmicarskaKategorija> result = new List<TakmicarskaKategorija>();
            foreach (TreeNode n in treeViewTakmicenja.Nodes)
            {
                if (n.Text != desc.Naziv)
                    continue;
                foreach (TreeNode n2 in n.Nodes)
                    result.Add(takmicenje.getKategorija(n2.Text));
            }
            return result;
        }

        private void btnDeleteTakmicenje_Click(object sender, EventArgs e)
        {
            RezultatskoTakmicenjeDescription desc =
                SelectedTakmicenje;
            if (desc == null)
                return;

            string msgFmt = "Da li zelite da izbrisete takmicenje '{0}'?";
            if (!MessageDialogs.queryConfirmation(
                String.Format(msgFmt, desc.Naziv), this.Text))
                return;

            /*foreach (TakmicarskaKategorija kat in deletedKat)
            {
                foreach (RezultatskoTakmicenjeDescription desc in deletedTak)
                {
                    deleteRezultatskoTakmicenje(kat, desc);
                }
            }

            foreach (RezultatskoTakmicenjeDescription t in deletedTak)
                DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDescriptionDAO().Delete(t);
            foreach (TakmicarskaKategorija k in deletedKat)
                deleteKategorija(k);

            DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().Update(takmicenje);

            session.Transaction.Commit();*/

            takmicenje.removeTakmicenjeDescription(desc);
            foreach (TreeNode n in treeViewTakmicenja.Nodes)
            {
                if (n.Text != desc.Naziv)
                    continue;
                treeViewTakmicenja.Nodes.Remove(n);
                break;
            }
        }

        private void btnMoveUpTakmicenje_Click(object sender, EventArgs e)
        {
            RezultatskoTakmicenjeDescription takmicenjeDesc =
                SelectedTakmicenje;
            if (takmicenjeDesc == null)
                return;

            if (takmicenje.moveTakmicenjeDescriptionUp(takmicenjeDesc))
            {
                setTakmicenja(takmicenje.TakmicenjeDescriptions);
                SelectedTakmicenje = takmicenjeDesc;
            }
        }

        private void btnMoveDownTakmicenje_Click(object sender, EventArgs e)
        {
            RezultatskoTakmicenjeDescription takmicenjeDesc =
                SelectedTakmicenje;
            if (takmicenjeDesc == null)
                return;

            if (takmicenje.moveTakmicenjeDescriptionDown(takmicenjeDesc))
            {
                setTakmicenja(takmicenje.TakmicenjeDescriptions);
                SelectedTakmicenje = takmicenjeDesc;
            }
        }

        private void btnZatvori_Click(object sender, EventArgs e)
        {
            Close();
        }

        private RezultatskoTakmicenje createRezultatskoTakmicenje(Takmicenje takmicenje, TakmicarskaKategorija k,
            RezultatskoTakmicenjeDescription d, int redBroj)
        {
            RezultatskoTakmicenje result = new RezultatskoTakmicenje(takmicenje,
                k, d, new Propozicije());
            result.RedBroj = (byte)redBroj;
            return result;
        }

        private void deleteKategorija(TakmicarskaKategorija k)
        {
            // TODO2: Treba obrisati sve ocene, rasporede nastupa, rasporede sudija, 
            // ekipe i gimnasticari za datu kategoriju
            
            DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().Delete(k);
        }

        private void deleteRezultatskoTakmicenje(TakmicarskaKategorija kat,
            RezultatskoTakmicenjeDescription desc)
        {
            RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            rezTakDAO.Delete(rezTakDAO.FindByKatDesc(kat, desc));
        }

        private void btnDodajIzPrethTak_Click(object sender, EventArgs e)
        {
            OtvoriTakmicenjeForm form = null;
            DialogResult result;
            try
            {
                form = new OtvoriTakmicenjeForm(null, true, 1, false);
                result = form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }
            if (result != DialogResult.OK)
                return;

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

            CheckListForm form2;
            try
            {
                form2 = new CheckListForm(kategorijeStr, "Izaberite kategorije", "Izaberite kategorije");
                result = form2.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }
            if (result != DialogResult.OK)
                return;

            session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    foreach (int index in form2.CheckedIndices)
                        takmicenje.addKategorija(kategorije[index]);
                    DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().Update(takmicenje);
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
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
            setKategorije(takmicenje.Kategorije);
        }
    }
}