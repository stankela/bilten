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

namespace Bilten.UI
{
    public partial class SelectGimnasticariPrethTakmForm : Form
    {
        private List<Takmicenje> takmicenja;
        private IDataContext dataContext;
        Gimnastika gim;
        private Dictionary<int, List<GimnasticarUcesnik>> gimMap;

        protected IList<GimnasticarUcesnik> selectedGimnasticari = new List<GimnasticarUcesnik>();
        public IList<GimnasticarUcesnik> SelectedGimnasticari
        {
            get { return selectedGimnasticari; }
        }

        public SelectGimnasticariPrethTakmForm(Gimnastika gim)
        {
            InitializeComponent();
            this.gim = gim;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                initUI();
                gimMap = new Dictionary<int, List<GimnasticarUcesnik>>();
                takmicenja = loadTakmicenja(gim);
                setTakmicenja(takmicenja);
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

        private void initUI()
        {
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

        private List<Takmicenje> loadTakmicenja(Gimnastika gim)
        {
            string query = @"select distinct t
                    from Takmicenje t
                    left join fetch t.Kategorije
                    where t.Gimnastika = :gim
                    order by t.Datum desc";

            IList<Takmicenje> result = dataContext.
                ExecuteQuery<Takmicenje>(QueryLanguageType.HQL, query,
                        new string[] { "gim" },
                        new object[] { gim });
            return new List<Takmicenje>(result);
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

        private List<GimnasticarUcesnik> loadGimnasticari(Takmicenje tak)
        {
            string query = @"select distinct g
                    from GimnasticarUcesnik g
                    left join fetch g.TakmicarskaKategorija
                    left join fetch g.KlubUcesnik
                    left join fetch g.DrzavaUcesnik
                    where g.Takmicenje = :tak
                    order by g.Prezime asc, g.Ime asc";

            IList<GimnasticarUcesnik> result = dataContext.
                ExecuteQuery<GimnasticarUcesnik>(QueryLanguageType.HQL, query,
                        new string[] { "tak" },
                        new object[] { tak });
            return new List<GimnasticarUcesnik>(result);
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode selectedNode = e.Node;
            Takmicenje tak = selectedNode.Tag as Takmicenje;
            if (tak == null || gimMap.ContainsKey(tak.Id))
                return;

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                gimMap.Add(tak.Id, loadGimnasticari(tak));
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

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
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