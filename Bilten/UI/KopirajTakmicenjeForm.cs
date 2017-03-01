using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Exceptions;
using Bilten.Domain;
using Bilten.Data;
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;
using Bilten.Dao.NHibernate;

namespace Bilten.UI
{
    public partial class KopirajTakmicenjeForm : Form
    {
        private IList<RezultatskoTakmicenjeDescription> takmicenja = new List<RezultatskoTakmicenjeDescription>();
        private IList<TakmicarskaKategorija> kategorije = new List<TakmicarskaKategorija>();
        Gimnastika gimnastika;

        private Takmicenje takmicenje;
        public Takmicenje Takmicenje
        {
            get { return takmicenje; }
        }

        private List<RezultatskoTakmicenjeDescription> selDescriptions = new List<RezultatskoTakmicenjeDescription>();
        public List<RezultatskoTakmicenjeDescription> SelDescriptions
        {
            get { return selDescriptions; }
        }

        private List<TakmicarskaKategorija> selKategorije = new List<TakmicarskaKategorija>();
        public List<TakmicarskaKategorija> SelKategorije
        {
            get { return selKategorije; }
        }

        public KopirajTakmicenjeForm(Gimnastika gim)
        {
            InitializeComponent();
            this.gimnastika = gim;
            this.Text = "Izaberi takmicenje i kategorije";
        }

        private void btnIzaberi_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult = DialogResult.None;
            SelectGimnasticariPrethTakmForm form = null;
            try
            {
                form = new SelectGimnasticariPrethTakmForm(gimnastika, true);
                dlgResult = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (dlgResult != DialogResult.OK || form.SelTakmicenje == null)
                return;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    takmicenje = form.SelTakmicenje;
                    txtTakmicenje.Text = takmicenje.Naziv;

                    GenericNHibernateDAO<Takmicenje, int> takmicenjeDAO
                        = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO() as GenericNHibernateDAO<Takmicenje, int>;
                    takmicenjeDAO.Attach(takmicenje, false);

                    lstTakmicenja.Items.Clear();
                    takmicenja.Clear();
                    foreach (RezultatskoTakmicenjeDescription d in takmicenje.TakmicenjeDescriptions)
                    {
                        lstTakmicenja.Items.Add(d, true);
                        takmicenja.Add(d);
                    }
                    lstKategorije.Items.Clear();
                    kategorije.Clear();
                    foreach (TakmicarskaKategorija k in takmicenje.Kategorije)
                    {
                        lstKategorije.Items.Add(k, true);
                        kategorije.Add(k);
                    }
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showMessage(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                return;
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (takmicenje == null)
            {
                MessageDialogs.showMessage("Izaberite takmicenje.", this.Text);
                DialogResult = DialogResult.None;
                return;
            }
            if (lstTakmicenja.CheckedItems.Count == 0)
            {
                MessageDialogs.showMessage("Selektujte takmicenja.", this.Text);
                DialogResult = DialogResult.None;
                return;
            }
            if (lstKategorije.CheckedItems.Count == 0)
            {
                MessageDialogs.showMessage("Selektujte kategorije.", this.Text);
                DialogResult = DialogResult.None;
                return;
            }

            selDescriptions.Clear();
            foreach (int index in lstTakmicenja.CheckedIndices)
            {
                selDescriptions.Add(takmicenja[index]);
            }
            selKategorije.Clear();
            foreach (int index in lstKategorije.CheckedIndices)
            {
                selKategorije.Add(kategorije[index]);
            }
        }
    }
}