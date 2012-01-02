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
using Bilten.Data.QueryModel;

namespace Bilten.UI
{
    public partial class SelectKategorijaForm : Form
    {
        private List<TakmicarskaKategorija> sveKategorije;

        private IList<TakmicarskaKategorija> selektovaneKategorije;
        public IList<TakmicarskaKategorija> SelektovaneKategorije
        {
            get { return selektovaneKategorije; }
        }

        private IList<TakmicarskaKategorija> nedozvoljeneKategorije;
        private IDataContext dataContext;
        private string labelText;
        private bool kategorijeGimnasticara;

        public SelectKategorijaForm(int takmicenjeId,
            IList<TakmicarskaKategorija> nedozvoljeneKategorije, 
            bool kategorijeGimnasticara,
            string labelText)
        {
            InitializeComponent();
            this.kategorijeGimnasticara = kategorijeGimnasticara;
            this.labelText = labelText;
            this.nedozvoljeneKategorije = nedozvoljeneKategorije;
            selektovaneKategorije = new List<TakmicarskaKategorija>();
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();

                IList<TakmicarskaKategorija> kategorije;
                if (kategorijeGimnasticara)
                    kategorije = loadKategorijeGimnasticara(takmicenjeId);
                else
                    kategorije = loadTakKategorije(takmicenjeId);
                sveKategorije = new List<TakmicarskaKategorija>(kategorije);
                createUI();

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

        private IList<TakmicarskaKategorija> loadKategorijeGimnasticara(int takmicenjeId)
        {
            Query q = new Query();
            Gimnastika gimnastika = dataContext.GetById<Takmicenje>(takmicenjeId).Gimnastika;
            
            q = new Query();
            q.Criteria.Add(new Criterion("Gimnastika", CriteriaOperator.Equal, (byte)gimnastika));
            IList<KategorijaGimnasticara> kategorije =
                dataContext.GetByCriteria<KategorijaGimnasticara>(q);

            IList<TakmicarskaKategorija> result = new List<TakmicarskaKategorija>();
            foreach (KategorijaGimnasticara kat in kategorije)
            {
                result.Add(new TakmicarskaKategorija(kat.Naziv, kat.Gimnastika));
            }
            return result;
        }

        private IList<TakmicarskaKategorija> loadTakKategorije(int takmicenjeId)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje.Id", CriteriaOperator.Equal, takmicenjeId));
            return dataContext.GetByCriteria<TakmicarskaKategorija>(q);
        }

        private void createUI()
        {
            Text = "Izaberite kategorije";
            label1.Text = labelText;

            int x = 3;
            int y = 3;
            int tabIndex = 0;

            this.panel1.SuspendLayout();
            this.SuspendLayout();

            string sortProperty = "RedBroj";
            if (kategorijeGimnasticara)
                sortProperty = "Naziv";
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(TakmicarskaKategorija))[sortProperty];
            sveKategorije.Sort(new SortComparer<TakmicarskaKategorija>(
                propDesc, ListSortDirection.Ascending));

            foreach (TakmicarskaKategorija k in sveKategorije)
            {
                CheckBox c = createCheckBox(k, new Point(x, y), tabIndex);
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

        private CheckBox createCheckBox(TakmicarskaKategorija k, Point location, int tabIndex)
        {
            CheckBox result = new CheckBox();
            result.AutoSize = true;
            result.Location = location;
            result.TabIndex = tabIndex;
            result.Text = k.ToString();
            result.UseVisualStyleBackColor = true;
            result.Tag = k;
            result.Checked = false;
            result.Enabled = !nedozvoljeneKategorije.Contains(k);
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