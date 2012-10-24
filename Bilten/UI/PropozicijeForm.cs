using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Data;
using Bilten.Exceptions;
using Bilten.Domain;
using Bilten.Data.QueryModel;

namespace Bilten.UI
{
    public partial class PropozicijeForm : Form
    {
        private Takmicenje takmicenje;
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private IDataContext dataContext;
        private PropertyPage _activePage;

        private List<PropertyPage> _pages = new List<PropertyPage>();
        private IList<PropertyPage> Pages
        {
            get { return _pages; }
        }

        public PropozicijeForm(int takmicenjeId)
        {
            InitializeComponent();
            treeView1.HideSelection = false;

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                takmicenje = loadTakmicenje(takmicenjeId);
                if (takmicenje.Kategorije.Count == 0)
                    throw new BusinessException("Morate najpre da unesete takmicarske kategorije.");
                rezTakmicenja = loadRezTakmicenja(takmicenjeId);
                
                addPages();
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

        private Takmicenje loadTakmicenje(int takmicenjeId)
        {
            string query = @"from Takmicenje t
                        left join fetch t.TakmicenjeDescriptions d
                        left join fetch d.Propozicije
                        left join fetch t.Kategorije
	                    where t.Id = :id";
            IList<Takmicenje> result = dataContext.ExecuteQuery<Takmicenje>(QueryLanguageType.HQL, query,
                    new string[] { "id" }, new object[] { takmicenjeId });
            if (result.Count == 0)
                return null;
            else
                return result[0];
        }

        private IList<RezultatskoTakmicenje> loadRezTakmicenja(int takmicenjeId)
        {
            string query = @"select distinct r
                from RezultatskoTakmicenje r
                left join fetch r.Propozicije
                left join fetch r.Kategorija kat
                left join fetch r.TakmicenjeDescription d
                left join fetch r.Takmicenje2
                left join fetch r.Takmicenje3
                left join fetch r.Takmicenje4
                where r.Takmicenje.Id = :takmicenjeId
                order by r.RedBroj";
            IList<RezultatskoTakmicenje> result = dataContext.
                ExecuteQuery<RezultatskoTakmicenje>(QueryLanguageType.HQL, query,
                        new string[] { "takmicenjeId" },
                        new object[] { takmicenjeId });
            return result;
        }

        private void addPages()
        {
            TreeNode node = treeView1.Nodes.Add("Sudije i ocene");
            PropertyPage page = new SudijeIOcenePage(takmicenje);
            node.Tag = page;
            Pages.Add(page);

            TreeNode takmicenjaRoot = treeView1.Nodes.Add("Takmicenja");
            foreach (RezultatskoTakmicenjeDescription d in takmicenje.TakmicenjeDescriptions)
            {
                TreeNode takmicenjeNode = takmicenjaRoot.Nodes.Add(d.Naziv);

                // takmicenje II
                node = takmicenjeNode.Nodes.Add("Takmicenje II");
                page = new Takmicenje2Page(d.Propozicije, 
                    getDependentPropozicije(d));
                node.Tag = page;
                Pages.Add(page);

                // takmicenje III
                node = takmicenjeNode.Nodes.Add("Takmicenje III");
                page = new Takmicenje3Page(d.Propozicije,
                    getDependentPropozicije(d));
                node.Tag = page;
                Pages.Add(page);

                // takmicenje IV
                node = takmicenjeNode.Nodes.Add("Takmicenje IV");
                page = new Takmicenje4Page(d.Propozicije,
                    getDependentPropozicije(d));
                node.Tag = page;
                Pages.Add(page);
            }

            TreeNode svaTakmicenjaRoot = treeView1.Nodes.Add("Sva takmicenja");
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                TreeNode takmicenjeNode = svaTakmicenjaRoot.Nodes.Add(rt.Naziv);

                // takmicenje II
                node = takmicenjeNode.Nodes.Add("Takmicenje II");
                page = new Takmicenje2Page(rt.Propozicije, null);
                node.Tag = page;
                Pages.Add(page);

                // takmicenje III
                node = takmicenjeNode.Nodes.Add("Takmicenje III");
                page = new Takmicenje3Page(rt.Propozicije, null);
                node.Tag = page;
                Pages.Add(page);

                // takmicenje IV
                node = takmicenjeNode.Nodes.Add("Takmicenje IV");
                page = new Takmicenje4Page(rt.Propozicije, null);
                node.Tag = page;
                Pages.Add(page);
            }
        }

        private IList<Propozicije> getDependentPropozicije(RezultatskoTakmicenjeDescription d)
        {
            IList<Propozicije> result = new List<Propozicije>();
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                if (rt.TakmicenjeDescription.Equals(d))
                    result.Add(rt.Propozicije);
            }
            return result;
        }

        private void okButton_Click(object sender, System.EventArgs e)
        {
          /*  foreach (PropertyPage page in _pages)
            {
                page.OnApply();
            }*/

            try
            {
                if (_activePage != null && !applyPage(_activePage))
                {
                    DialogResult = DialogResult.None;
                    return;
                }

                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                dataContext.Save(takmicenje);

                IDictionary<int, List<RezultatskoTakmicenje>> rezTakMap = new Dictionary<int, List<RezultatskoTakmicenje>>();
                foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                {
                    if (rezTakMap.ContainsKey(rt.TakmicenjeDescription.Id))
                    {
                        rezTakMap[rt.TakmicenjeDescription.Id].Add(rt);
                    }
                    else
                    {
                        List<RezultatskoTakmicenje> rezTakList = new List<RezultatskoTakmicenje>();
                        rezTakList.Add(rt);
                        rezTakMap.Add(rt.TakmicenjeDescription.Id, rezTakList);
                    }
                }
                foreach (List<RezultatskoTakmicenje> rezTakList in rezTakMap.Values)
                {
                    bool kombAdded = false;
                    foreach (RezultatskoTakmicenje rt in rezTakList)
                    {
                        if (!rt.TakmicenjeDescription.Propozicije.JednoTak4ZaSveKategorije)
                        {
                            rt.ImaEkipnoTakmicenje = true;
                            rt.KombinovanoEkipnoTak = false;
                        }
                        else
                        {
                            if (!kombAdded)
                            {
                                rt.ImaEkipnoTakmicenje = true;
                                rt.KombinovanoEkipnoTak = true;
                                kombAdded = true;
                            }
                            else
                            {
                                rt.ImaEkipnoTakmicenje = false;
                                rt.KombinovanoEkipnoTak = false;
                            }
                        }
                    }
                }


                foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                {
                    dataContext.Save(rt.Propozicije);

                    bool deletedTak2, deletedTak3, deletedTak4;

                    rt.updateTakmicenjaFromChangedPropozicije(
                        out deletedTak2, out deletedTak3, out deletedTak4);
                 
                    // TODO: Sledece tri Delete naredbe najverovatnije nemaju efekta zato sto ako je npr. deletedTak2 == true,
                    // tada je rt.Takmicenje2 == null
                    if (deletedTak2)
                        dataContext.Delete(rt.Takmicenje2);
                    if (deletedTak3)
                        dataContext.Delete(rt.Takmicenje3);
                    if (deletedTak4)
                        dataContext.Delete(rt.Takmicenje4);

                    // TODO: Potrebno je ponovo izracunati poretke i ucesnike zato
                    // sto su se mozda promenili brojevi finalista, rezervi, nacin
                    // racunanja preskoka itd.


                    dataContext.Save(rt);
                }

                dataContext.Commit();
            }
            catch (InfrastructureException ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                DialogResult = DialogResult.Cancel;
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                DialogResult = DialogResult.Cancel;
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }

        }

        private void PropozicijeForm_Load(object sender, System.EventArgs e)
        {
            // Iterate over the pages, adding each one to the right-hand 
            // panel and to the list box, and work out the size of the largest 
            // page, so that we can resize the dialog nicely.
            // Note that we set the initial value of maxPageSize to the size of the 
            // right-hand panel. This allows us to control the minimum size of the 
            // options dialog, in case the pages are very small. 
            Size maxPageSize = pagePanel.Size;
            foreach (PropertyPage page in _pages)
            {
                pagePanel.Controls.Add(page);

                if (page.Width > maxPageSize.Width)
                    maxPageSize.Width = page.Width;
                if (page.Height > maxPageSize.Height)
                    maxPageSize.Height = page.Height;

                page.Dock = DockStyle.Fill;
                page.Visible = false;
            }

            // Resize the Options Dialog so that the largest dialog page fits:
            Size newSize = new Size();
            newSize.Width = maxPageSize.Width + (this.Width - pagePanel.Width);
            newSize.Height = maxPageSize.Height + (this.Height - pagePanel.Height);
            this.Size = newSize;

            CenterToParent();

            treeView1.SelectedNode = treeView1.Nodes[0];
        }

        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (_activePage == null)
                return;

            try
            {
                e.Cancel = !applyPage(_activePage);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                Close();
                return;
            }
        }

        private bool applyPage(PropertyPage page)
        {
            try
            {
                page.OnApply();
                return true;
            }
            catch (BusinessException ex)
            {
                if (ex.Notification != null)
                {
                    NotificationMessage msg = ex.Notification.FirstMessage;
                    MessageDialogs.showMessage(msg.Message, this.Text);

                    // stavljanje fokusa nema efekta, jer fokus uvek ostaje na TreeView
                    //page.setFocus(msg.FieldName);
                }
                else if (!string.IsNullOrEmpty(ex.InvalidProperty))
                {
                    MessageDialogs.showMessage(ex.Message, this.Text);
                    //page.setFocus(ex.InvalidProperty);
                }
                else
                {
                    MessageDialogs.showMessage(ex.Message, this.Text);
                }
                return false;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (_activePage != null)
                _activePage.Visible = false;

            TreeNode selectedNode = treeView1.SelectedNode;
            PropertyPage page = (PropertyPage)selectedNode.Tag;
            _activePage = page;

            if (_activePage != null)
            {
                _activePage.Visible = true;
                try
                {
                    _activePage.OnSetActive();
                }
                catch (InfrastructureException ex)
                {
                    MessageDialogs.showError(ex.Message, this.Text);
                    Close();
                    return;
                }
            }
        }
    }
}