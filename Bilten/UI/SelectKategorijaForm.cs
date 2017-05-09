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
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;
using Bilten.Util;

namespace Bilten.UI
{
    public partial class SelectKategorijaForm : Form
    {
        private IList<TakmicarskaKategorija> selektovaneKategorije;
        public IList<TakmicarskaKategorija> SelektovaneKategorije
        {
            get { return selektovaneKategorije; }
        }

        public SelectKategorijaForm(int takmicenjeId, Gimnastika gimnastika,
            IList<TakmicarskaKategorija> nedozvoljeneKategorije, string labelText)
        {
            InitializeComponent();
            selektovaneKategorije = new List<TakmicarskaKategorija>();

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<TakmicarskaKategorija> kategorije;
                    bool sveKategorije;
                    if (takmicenjeId == -1)
                    {
                        kategorije = loadKategorijeGimnasticara(gimnastika);
                        sveKategorije = true;
                    }
                    else
                    {
                        kategorije = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO().FindByTakmicenje(takmicenjeId);
                        sveKategorije = false;
                    }
                    createUI(new List<TakmicarskaKategorija>(kategorije), nedozvoljeneKategorije,
                        sveKategorije, labelText);
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

        private IList<TakmicarskaKategorija> loadKategorijeGimnasticara(Gimnastika gimnastika)
        {
            IList<KategorijaGimnasticara> kategorije 
                = DAOFactoryFactory.DAOFactory.GetKategorijaGimnasticaraDAO().FindByGimnastika(gimnastika);

            IList<TakmicarskaKategorija> result = new List<TakmicarskaKategorija>();
            foreach (KategorijaGimnasticara kat in kategorije)
                result.Add(new TakmicarskaKategorija(kat.Naziv));
            return result;
        }

        private void createUI(List<TakmicarskaKategorija> kategorije, IList<TakmicarskaKategorija> nedozvoljeneKategorije,
            bool sveKategorije, string labelText)
        {
            Text = "Izaberite kategorije";
            label1.Text = labelText;

            int x = 3;
            int y = 3;
            int tabIndex = 0;

            this.panel1.SuspendLayout();
            this.SuspendLayout();

            string sortProperty = sveKategorije ? "Naziv" : "RedBroj";
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(TakmicarskaKategorija))[sortProperty];
            kategorije.Sort(new SortComparer<TakmicarskaKategorija>(
                propDesc, ListSortDirection.Ascending));

            foreach (TakmicarskaKategorija k in kategorije)
            {
                CheckBox c = createCheckBox(k, new Point(x, y), tabIndex, !nedozvoljeneKategorije.Contains(k));
                this.panel1.Controls.Add(c);
                y += 23;
                tabIndex++;
            }

            int MAX_PANEL_HEIGHT = 300;
            int panelHeight = y;
            if (panelHeight > MAX_PANEL_HEIGHT)
            {
                panelHeight = MAX_PANEL_HEIGHT;
                panel1.AutoScroll = true;
            }

            // sirina prozora pri kojoj je labela centrirana
            int formWidth = label1.Right + label1.Left;

            // sirina prozora pri kojoj Ok i Cancel staju u prozor
            int formWidth2 = btnCancel.Right + btnOK.Left;

            formWidth = Math.Max(formWidth, formWidth2);

            panel1.ClientSize = new Size(formWidth - 2 * panel1.Left, panelHeight);

            y = panel1.Bottom + 12;
            btnOK.Location = new Point(btnOK.Location.X, y);
            btnCancel.Location = new Point(btnCancel.Location.X, y);

            this.ClientSize = new Size(formWidth, btnOK.Bottom + 12);

            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private CheckBox createCheckBox(TakmicarskaKategorija k, Point location, int tabIndex, bool enabled)
        {
            CheckBox result = new CheckBox();
            result.AutoSize = true;
            result.Location = location;
            result.TabIndex = tabIndex;
            result.Text = k.ToString();
            result.UseVisualStyleBackColor = true;
            result.Tag = k;
            result.Checked = false;
            result.Enabled = enabled;
            return result;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            selektovaneKategorije.Clear();
            foreach (Control c in panel1.Controls)
            { 
                CheckBox ckb = c as CheckBox;
                if (ckb == null)
                    continue;

                if (ckb.Enabled && ckb.Checked)
                    selektovaneKategorije.Add((TakmicarskaKategorija)ckb.Tag);
            }

            if (selektovaneKategorije.Count == 0)
            {
                MessageDialogs.showMessage("Izaberite kategorije.", this.Text);
                DialogResult = DialogResult.None;
                return;
            }
        }

    }
}