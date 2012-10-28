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

namespace Bilten.UI
{
    public partial class KopirajTakmicenjeForm : Form
    {
        IDataContext dataContext;
        private IList<RezultatskoTakmicenjeDescription> takmicenja = new List<RezultatskoTakmicenjeDescription>();
        private IList<TakmicarskaKategorija> kategorije = new List<TakmicarskaKategorija>();

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

        public KopirajTakmicenjeForm()
        {
            InitializeComponent();
            this.Text = "Izaberi takmicenje i kategorije";
        }

        private void btnIzaberi_Click(object sender, EventArgs e)
        {
            OtvoriTakmicenjeForm form;
            DialogResult result;
            try
            {
                form = new OtvoriTakmicenjeForm(null, true, 1);
                result = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (result != DialogResult.OK || form.SelTakmicenja.Count != 1)
                return;

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                takmicenje = form.SelTakmicenja[0];
                txtTakmicenje.Text = takmicenje.Naziv;
                dataContext.Attach(takmicenje, false);

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
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showMessage(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                return;
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
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