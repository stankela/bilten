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
using Bilten.Util;
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;
using Bilten.Services;

namespace Bilten.UI
{
    public partial class PropozicijeForm : Form
    {
        private Takmicenje takmicenje;
        private IList<RezultatskoTakmicenje> rezTakmicenja;
        private PropertyPage _activePage;

        private List<PropertyPage> _pages = new List<PropertyPage>();
        private IList<PropertyPage> Pages
        {
            get { return _pages; }
        }

        private IDictionary<int, Propozicije> origPropozicijeMap = new Dictionary<int, Propozicije>();
        private IDictionary<int, Propozicije> origDescPropozicijeMap = new Dictionary<int, Propozicije>();

        // TODO4: Oznaci crvenom bojom u propozicijama za description ona svojstva gde je vrednost u nekom od
        // propozicija za rez. takmicenje razlicita od vrednosti za description. Takodje, osmisli kako da se na
        // jednostavan nacin vrednost za sva rez. takmicenja vrati na vrednost koja postoji u descriptionu. (Trenutno
        // je najjednostavniji nacin da se promeni vrednost u descriptionu pa da se ponovo promeni, tj. vrati na staru
        // vrednost; ali da li ce korisnik znati za taj nacin)

        public PropozicijeForm(int takmicenjeId)
        {
            InitializeComponent();
            treeView1.HideSelection = false;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindByIdFetch_Prop_Kat_Desc(takmicenjeId);
                    if (takmicenje.Kategorije.Count == 0)
                        throw new BusinessException(Strings.NO_KATEGORIJE_I_TAKMICENJA_ERROR_MSG);
                    rezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                        .FindByTakmicenje(takmicenjeId);
                    foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                    {
                        origPropozicijeMap.Add(rt.Id, rt.Propozicije.clonePropozicije());
                        if (!origDescPropozicijeMap.ContainsKey(rt.TakmicenjeDescription.Id))
                            origDescPropozicijeMap.Add(rt.TakmicenjeDescription.Id,
                                                       rt.TakmicenjeDescription.Propozicije.clonePropozicije());
                    }
                    addPages();
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
                if (takmicenje.FinaleKupa)
                    page = new Takmicenje2FinaleKupaPage(d.Propozicije,
                        getDependentPropozicije(d));
                else
                    page = new Takmicenje2Page(d.Propozicije,
                        getDependentPropozicije(d));
                node.Tag = page;
                Pages.Add(page);

                // takmicenje III
                node = takmicenjeNode.Nodes.Add("Takmicenje III");
                if (takmicenje.FinaleKupa)
                    page = new Takmicenje3FinaleKupaPage(d.Propozicije,
                        getDependentPropozicije(d));
                else
                    page = new Takmicenje3Page(d.Propozicije,
                        getDependentPropozicije(d));
                node.Tag = page;
                Pages.Add(page);

                // takmicenje IV
                node = takmicenjeNode.Nodes.Add("Takmicenje IV");
                if (takmicenje.FinaleKupa)
                    page = new Takmicenje4FinaleKupaPage(d.Propozicije,
                        getDependentPropozicije(d));
                else
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
                if (takmicenje.FinaleKupa)
                    page = new Takmicenje2FinaleKupaPage(rt.Propozicije, null);
                else
                    page = new Takmicenje2Page(rt.Propozicije, null);
                node.Tag = page;
                Pages.Add(page);

                // takmicenje III
                node = takmicenjeNode.Nodes.Add("Takmicenje III");
                if (takmicenje.FinaleKupa)
                    page = new Takmicenje3FinaleKupaPage(rt.Propozicije, null);
                else
                    page = new Takmicenje3Page(rt.Propozicije, null);
                node.Tag = page;
                Pages.Add(page);

                // takmicenje IV
                node = takmicenjeNode.Nodes.Add("Takmicenje IV");
                if (takmicenje.FinaleKupa)
                    page = new Takmicenje4FinaleKupaPage(rt.Propozicije, null);
                else
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
            /*foreach (PropertyPage page in _pages)
                page.OnApply();*/

            if (_activePage != null && !applyPage(_activePage))
            {
                DialogResult = DialogResult.None;
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    RezultatskoTakmicenjeService.updateTakmicenjeOnChangedPropozicije(rezTakmicenja, origPropozicijeMap,
                        origDescPropozicijeMap, takmicenje);

                    takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenje.Id);
                    takmicenje.LastModified = DateTime.Now;
                    session.Transaction.Commit();
                }
            }
            catch (InfrastructureException ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                DialogResult = DialogResult.Cancel;
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                DialogResult = DialogResult.Cancel;
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
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