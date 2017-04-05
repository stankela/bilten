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
using Bilten.Util;

namespace Bilten.UI
{
    public partial class KopirajTakmicenjeForm : Form
    {
        private Takmicenje takmicenje;

        private List<RezultatskoTakmicenje> rezTakmicenja = new List<RezultatskoTakmicenje>();

        private List<RezultatskoTakmicenje> selRezTakmicenja = new List<RezultatskoTakmicenje>();
        public List<RezultatskoTakmicenje> SelRezTakmicenja
        {
            get { return selRezTakmicenja; }
        }

        public KopirajTakmicenjeForm(Takmicenje takmicenje, IList<RezultatskoTakmicenje> rezTakmicenja)
        {
            InitializeComponent();
            this.takmicenje = takmicenje;
            this.Text = "Izaberi takmicenje i kategorije";

            txtTakmicenje.Text = takmicenje.Naziv;

            this.rezTakmicenja = new List<RezultatskoTakmicenje>(rezTakmicenja);
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatskoTakmicenje))["RedBroj"];
            this.rezTakmicenja.Sort(new SortComparer<RezultatskoTakmicenje>(
                propDesc, ListSortDirection.Ascending));

            lstRezTakmicenja.Items.Clear();
            foreach (RezultatskoTakmicenje rt in this.rezTakmicenja)
            {
                lstRezTakmicenja.Items.Add(rt, true);
            }
        }

        private void btnIzaberi_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult = DialogResult.None;
            SelectGimnasticariPrethTakmForm form = null;
            try
            {
                form = new SelectGimnasticariPrethTakmForm(takmicenje.Gimnastika, true);
                dlgResult = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (dlgResult != DialogResult.OK || form.SelTakmicenje == null)
                return;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lstRezTakmicenja.CheckedItems.Count == 0)
            {
                MessageDialogs.showMessage("Selektujte takmicenja.", this.Text);
                DialogResult = DialogResult.None;
                return;
            }

            selRezTakmicenja.Clear();
            foreach (int index in lstRezTakmicenja.CheckedIndices)
            {
                selRezTakmicenja.Add(rezTakmicenja[index]);
            }
        }
    }
}