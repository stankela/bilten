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
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class SelectGimnasticariPrethTakmForm : Form
    {
        private List<Takmicenje> takmicenja;
        Gimnastika gim;
        private Dictionary<int, List<GimnasticarUcesnik>> gimMap;
        bool izborTakmicenja;

        private IList<GimnasticarUcesnik> selectedGimnasticari = new List<GimnasticarUcesnik>();
        public IList<GimnasticarUcesnik> SelectedGimnasticari
        {
            get { return selectedGimnasticari; }
        }

        private Takmicenje takmicenje;
        public Takmicenje SelTakmicenje
        {
            get { return takmicenje; }
        }

        public SelectGimnasticariPrethTakmForm(Gimnastika gim, bool izborTakmicenja)
        {
            InitializeComponent();
            this.gim = gim;
            this.izborTakmicenja = izborTakmicenja;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    initUI();
                    gimMap = new Dictionary<int, List<GimnasticarUcesnik>>();
                    takmicenja = new List<Takmicenje>(DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindByGimnastika(gim));
                    setTakmicenja(takmicenja);
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

        private void initUI()
        {
            if (izborTakmicenja)
                this.Text = "Izaberi takmicenje";
            else
                this.Text = "Izaberi gimnasticare";

            dataGridViewUserControl2.GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(dataGridViewUserControl2_GridColumnHeaderMouseClick);
            GridColumnsInitializer.initGimnasticarUcesnik2(dataGridViewUserControl2);
        }

        void dataGridViewUserControl2_GridColumnHeaderMouseClick(object sender, GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<GimnasticarUcesnik>(e.DataGridViewCellMouseEventArgs);
        }

        private void setGimnasticari(List<GimnasticarUcesnik> gimnasticari, TakmicarskaKategorija kat)
        {
            List<GimnasticarUcesnik> selGimnasticari = new List<GimnasticarUcesnik>();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                if (g.TakmicarskaKategorija.Equals(kat))
                    selGimnasticari.Add(g);
            }
            dataGridViewUserControl2.setItems<GimnasticarUcesnik>(selGimnasticari);
        }

        private void setTakmicenja(List<Takmicenje> takmicenja)
        {
            foreach (Takmicenje t in takmicenja)
            {
                TreeNode takmicenjeNode = treeView1.Nodes.Add(t.Naziv);
                takmicenjeNode.Tag = t;

                foreach (TakmicarskaKategorija k in t.Kategorije)
                {
                    TreeNode node = takmicenjeNode.Nodes.Add(k.Naziv);
                    node.Tag = k;
                }
            }
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode selectedNode = e.Node;
            Takmicenje tak = selectedNode.Tag as Takmicenje;
            if (tak == null || gimMap.ContainsKey(tak.Id))
                return;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    gimMap.Add(tak.Id, new List<GimnasticarUcesnik>(
                        DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO().FindByTakmicenjeFetch_Kat_Klub_Drzava(tak.Id)));
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

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            while (node.Parent != null)
                node = node.Parent;
            takmicenje = node.Tag as Takmicenje;

            if (e.Node.Parent == null)
                return;

            TakmicarskaKategorija kat = e.Node.Tag as TakmicarskaKategorija;
            if (kat != null)
            {
                setGimnasticari(gimMap[kat.Takmicenje.Id], kat);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (izborTakmicenja)
            {
                if (takmicenje == null)
                {
                    DialogResult = DialogResult.None;
                    return;
                }
            }
            else
            {
                IList<GimnasticarUcesnik> selItems
                    = dataGridViewUserControl2.getSelectedItems<GimnasticarUcesnik>();
                if (selItems.Count == 0)
                {
                    DialogResult = DialogResult.None;
                    return;
                }
                selectedGimnasticari = selItems;
                DialogResult = DialogResult.OK;
            }
        }
    }
}