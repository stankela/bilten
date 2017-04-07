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
using Bilten.Util;
using Bilten.Dao;
using NHibernate;
using NHibernate.Context;

namespace Bilten.UI
{
    public partial class TakmicenjeForm : EntityDetailForm
    {
        private static readonly string STANDARDNO_TAKMICENJE = "Standardno takmicenje";
        private static readonly string FINALE_KUPA = "Finale kupa";
        private static readonly string ZBIR_VISE_KOLA = "Zbir vise kola, viseboj i ekipno";
        private static readonly string IZABERI_PRVO_I_DRUGO_KOLO = "Izaberi I kolo i II kolo";
        private static readonly string IZABERI_PRETHODNA_KOLA = "Izaberi prethodna kola";
        private static readonly int MAX_KOLA = 4;

        private static readonly string MSG = "MSG";
        private static readonly string ZSG = "ZSG";

        List<Takmicenje> prethodnaKola = new List<Takmicenje>();
        private bool uzmiOsnovnePodatke = false;

        private IList<RezultatskoTakmicenje> svaRezTakmicenja;

        public Takmicenje copyFromTakmicenje;
        public IList<RezultatskoTakmicenje> rezTakmicenja;
        public IDictionary<int, List<GimnasticarUcesnik>> rezTakToGimnasticarMap;

        public TakmicenjeForm()
        {
            InitializeComponent();
            initialize(null, true);
        }

        public TakmicenjeForm(string naziv, Gimnastika gimnastika, DateTime datum, string mesto, TipTakmicenja tipTakmicenja)
        {
            InitializeComponent();
            uzmiOsnovnePodatke = true;
            initialize(null, false);
                        
            txtNaziv.Text = naziv;
            if (gimnastika == Gimnastika.MSG)
                cmbGimnastika.SelectedItem = MSG;
            else if (gimnastika == Gimnastika.ZSG)
                cmbGimnastika.SelectedItem = ZSG;
            txtDatum.Text = datum.ToShortDateString();
            txtMesto.Text = mesto;
            if (tipTakmicenja == TipTakmicenja.StandardnoTakmicenje)
                cmbTipTakmicenja.SelectedItem = STANDARDNO_TAKMICENJE;
            else if (tipTakmicenja == TipTakmicenja.FinaleKupa)
                cmbTipTakmicenja.SelectedItem = FINALE_KUPA;
            else if (tipTakmicenja == TipTakmicenja.ZbirViseKola)
                cmbTipTakmicenja.SelectedItem = ZBIR_VISE_KOLA;

            ClientSize = new Size(txtNaziv.Location.X + txtNaziv.Size.Width + 24,
                cmbTipTakmicenja.Location.Y + cmbTipTakmicenja.Size.Height + 48);
            listBox1.Visible = false;
            btnIzaberiPrvaDvaKola.Visible = false;   
        }

        protected override void initUI()
        {
            base.initUI();
            this.Text = "Takmicenje";
            btnOk.Anchor = (AnchorStyles)(AnchorStyles.Bottom | AnchorStyles.Right);
            btnCancel.Anchor = (AnchorStyles)(AnchorStyles.Bottom | AnchorStyles.Right);

            txtNaziv.Text = String.Empty;
            txtDatum.Text = String.Empty;
            txtMesto.Text = String.Empty;

            txtPrethTak.ReadOnly = true;
            txtPrethTak.BackColor = SystemColors.Window;

            prethodnaKola.Clear();

            cmbGimnastika.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGimnastika.Items.AddRange(new string[] { MSG, ZSG });
            cmbGimnastika.SelectedIndex = -1;

            cmbTipTakmicenja.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipTakmicenja.Items.AddRange(new string[] { STANDARDNO_TAKMICENJE, FINALE_KUPA, ZBIR_VISE_KOLA });
            cmbTipTakmicenja.SelectedIndex = 0;

            listBox1.Items.Clear();
            btnIzaberiPrvaDvaKola.Text = IZABERI_PRVO_I_DRUGO_KOLO;

            treeView1.CheckBoxes = true;
            treeView1.AfterCheck += treeView1_AfterCheck;

            setEnabled();

            cmbTipTakmicenja.SelectedIndexChanged += new EventHandler(cmbTipTakmicenja_SelectedIndexChanged);
        }

        private void setEnabled()
        {
            if (uzmiOsnovnePodatke)
            {
                lblGimnastika.Enabled = false;
                cmbGimnastika.Enabled = false;
                lblTipTakmicenja.Enabled = false;
                cmbTipTakmicenja.Enabled = false;
                ckbKopirajPrethTak.Enabled = false;
                txtPrethTak.Enabled = false;
                btnIzaberiPrethTak.Enabled = false;
                listBox1.Enabled = false;
                btnIzaberiPrvaDvaKola.Enabled = false;
            }
            else
            {
                lblGimnastika.Enabled = true;
                cmbGimnastika.Enabled = true;
                lblTipTakmicenja.Enabled = true;
                cmbTipTakmicenja.Enabled = true;
                setEnabledTipTakmicenja();
            }
        }

        private void setEnabledTipTakmicenja()
        {
            ckbKopirajPrethTak.Enabled = !uzmiOsnovnePodatke && standardnoTakmicenje();
            setEnabledKopirajPrethTak();                

            listBox1.Enabled = !uzmiOsnovnePodatke && (finaleKupa() || zbirViseKola());
            btnIzaberiPrvaDvaKola.Enabled = !uzmiOsnovnePodatke && (finaleKupa() || zbirViseKola());
        }

        private void setEnabledKopirajPrethTak()
        {
            txtPrethTak.Enabled = !uzmiOsnovnePodatke && ckbKopirajPrethTak.Checked && standardnoTakmicenje();
            btnIzaberiPrethTak.Enabled = !uzmiOsnovnePodatke && ckbKopirajPrethTak.Checked && standardnoTakmicenje();
        }

        private bool standardnoTakmicenje()
        {
            return cmbTipTakmicenja.SelectedIndex == cmbTipTakmicenja.Items.IndexOf(STANDARDNO_TAKMICENJE);
        }

        private bool finaleKupa()
        {
            return cmbTipTakmicenja.SelectedIndex == cmbTipTakmicenja.Items.IndexOf(FINALE_KUPA);
        }

        private bool zbirViseKola()
        {
            return cmbTipTakmicenja.SelectedIndex == cmbTipTakmicenja.Items.IndexOf(ZBIR_VISE_KOLA);
        }

        private void cmbTipTakmicenja_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (uzmiOsnovnePodatke)
                return;

            setEnabledTipTakmicenja();

            if (finaleKupa())
            {
                btnIzaberiPrvaDvaKola.Text = IZABERI_PRVO_I_DRUGO_KOLO;
                ckbKopirajPrethTak.Checked = false;
            }
            else if (zbirViseKola())
            {
                btnIzaberiPrvaDvaKola.Text = IZABERI_PRETHODNA_KOLA;
                ckbKopirajPrethTak.Checked = false;
            }
            else
            {
                prethodnaKola.Clear();
                listBox1.Items.Clear();
            }
        }

        protected override void requiredFieldsAndFormatValidation(Notification notification)
        {
            if (txtNaziv.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv takmicenja je obavezan.");
            }
            if (cmbGimnastika.SelectedIndex == -1)
            {
                notification.RegisterMessage(
                    "Gimnastika", "Gimnastika je obavezna.");
            }
            if (txtDatum.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Datum", "Datum takmicenja je obavezan.");
            }
            else if (!tryParseDateTime(txtDatum.Text))
            {
                notification.RegisterMessage(
                    "Datum", "Neispravan format za datum takmicenja.");
            }
            else if (Datum.Parse(txtDatum.Text).ToDateTime().Year < 1753)
            {
                // NOTE: C# DateTime dozvoljava datume od 1.1.0001 dok SQL Serverov
                // tip datetime dozvoljava datume od 1.1.1753
                notification.RegisterMessage(
                    "Datum", "Neispravna vrednost za datum takmicenja.");
            }
            if (txtMesto.Text.Trim() == String.Empty)
            {
                notification.RegisterMessage(
                    "Mesto", "Mesto odrzavanja je obavezno.");
            }

            if (!uzmiOsnovnePodatke)
            {
                if (ckbKopirajPrethTak.Enabled && ckbKopirajPrethTak.Checked
                    && txtPrethTak.Text.Trim() == String.Empty)
                {
                    notification.RegisterMessage("PrethodnoTakmicenje", "Izaberite takmicenje koje zelite da kopirate.");
                }
                if (finaleKupa() && (prethodnaKola.Count != 2))
                {
                    notification.RegisterMessage(
                        "FinaleKupa", "Izaberite I i II kolo kupa.");
                }
                if (zbirViseKola())
                {
                    if (prethodnaKola.Count == 0)
                    {
                        notification.RegisterMessage("FinaleKupa", "Izaberite prethodna kola.");
                    }
                    else if (prethodnaKola.Count > MAX_KOLA)
                    {
                        string msg = String.Format("Maksimalno dozvoljen broj kola je {0}.", MAX_KOLA);
                        notification.RegisterMessage("FinaleKupa", msg);
                    }
                }
            }
        }

        private bool tryParseDateTime(string s)
        {
            // koristi se klasa Datum zato sto DateTime ne dozvoljava tacku na kraju
            // datuma
            Datum datum;
            bool result = Datum.TryParse(s, out datum);
            if (result)
                result = datum.hasFullDatum();
            return result;
        }

        protected override void setFocus(string propertyName)
        {
            switch (propertyName)
            {
                case "Naziv":
                    txtNaziv.Focus();
                    break;

                case "Gimnastika":
                    cmbGimnastika.Focus();
                    break;

                case "Datum":
                    txtDatum.Focus();
                    break;

                case "Mesto":
                    txtMesto.Focus();
                    break;

                case "FinaleKupa":
                    listBox1.Focus();
                    break;

                case "PrethodnoTakmicenje":
                    btnIzaberiPrethTak.Focus();
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        protected override DomainObject createNewEntity()
        {
            return new Takmicenje();
        }

        protected override void updateEntityFromUI(DomainObject entity)
        {
            Takmicenje takmicenje = (Takmicenje)entity;
            takmicenje.Naziv = txtNaziv.Text.Trim();
            takmicenje.Datum = Datum.Parse(txtDatum.Text).ToDateTime();
            takmicenje.Mesto = txtMesto.Text.Trim();

            if (cmbGimnastika.SelectedIndex == 0)
                takmicenje.Gimnastika = Gimnastika.MSG;
            else if (cmbGimnastika.SelectedIndex == 1)
                takmicenje.Gimnastika = Gimnastika.ZSG;

            if (!uzmiOsnovnePodatke)
            {
                takmicenje.PrvoKolo = null;
                takmicenje.DrugoKolo = null;
                takmicenje.TreceKolo = null;
                takmicenje.CetvrtoKolo = null;
                if (finaleKupa())
                {
                    takmicenje.TipTakmicenja = TipTakmicenja.FinaleKupa;
                    takmicenje.PrvoKolo = prethodnaKola[0];
                    takmicenje.DrugoKolo = prethodnaKola[1];
                }
                else if (zbirViseKola())
                {
                    takmicenje.TipTakmicenja = TipTakmicenja.ZbirViseKola;
                    if (prethodnaKola.Count > 0)
                        takmicenje.PrvoKolo = prethodnaKola[0];
                    if (prethodnaKola.Count > 1)
                        takmicenje.DrugoKolo = prethodnaKola[1];
                    if (prethodnaKola.Count > 2)
                        takmicenje.TreceKolo = prethodnaKola[2];
                    if (prethodnaKola.Count > 3)
                        takmicenje.CetvrtoKolo = prethodnaKola[3];
                }
                else
                {
                    takmicenje.TipTakmicenja = TipTakmicenja.StandardnoTakmicenje;
                }
            }
        }

        protected override void addEntity(DomainObject entity)
        {
            DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().Add((Takmicenje)entity);
        }

        protected override void checkBusinessRulesOnAdd(DomainObject entity)
        {
            Takmicenje takmicenje = (Takmicenje)entity;
            Notification notification = new Notification();

            if (DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().existsTakmicenje(
                takmicenje.Naziv, takmicenje.Gimnastika, takmicenje.Datum))
            {
                notification.RegisterMessage("Naziv",
                    "Takmicenje sa datim nazivom, gimnastikom i datumom vec postoji.");
                throw new BusinessException(notification);
            }
        }

        private void btnIzaberiPrvaDvaKola_Click(object sender, EventArgs e)
        {
            if (uzmiOsnovnePodatke)
                return;

            OtvoriTakmicenjeForm form = null;
            DialogResult result;
            try
            {
                if (finaleKupa())
                    form = new OtvoriTakmicenjeForm(null, true, 2, false);
                else if (zbirViseKola())
                    form = new OtvoriTakmicenjeForm(null, true, MAX_KOLA, true);
                result = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (result != DialogResult.OK)
                return;

            if (finaleKupa() || zbirViseKola())
            {
                prethodnaKola.Clear();
                for (int i = 0; i < form.SelTakmicenja.Count; ++i)
                    prethodnaKola.Add(form.SelTakmicenja[i]);

                PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                    TypeDescriptor.GetProperties(typeof(Takmicenje))["Datum"]
                };
                ListSortDirection[] sortDir = new ListSortDirection[] {
                    ListSortDirection.Ascending
                };
                prethodnaKola.Sort(new SortComparer<Takmicenje>(propDesc, sortDir));

                listBox1.Items.Clear();
                for (int i = 0; i < prethodnaKola.Count; ++i)
                    listBox1.Items.Add(prethodnaKola[i].Naziv);
            }
        }

        private void TakmicenjeForm_Shown(object sender, EventArgs e)
        {
            if (!uzmiOsnovnePodatke)
                txtNaziv.Focus();
            else
                lblNaziv.Focus();
        }

        private void ckbKopirajPrethTak_CheckedChanged(object sender, EventArgs e)
        {
            setEnabledKopirajPrethTak();
            if (!ckbKopirajPrethTak.Checked)
            {
                clearPrethodnoTakmicenje();
            }
            persistEntity = standardnoTakmicenje() && !ckbKopirajPrethTak.Checked;
        }

        private void clearPrethodnoTakmicenje()
        {
            txtPrethTak.Clear();
            treeView1.Nodes.Clear();
            copyFromTakmicenje = null;

        }

        private void btnIzaberiPrethTak_Click(object sender, EventArgs e)
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

            copyFromTakmicenje = form.SelTakmicenja[0];
            txtPrethTak.Text = copyFromTakmicenje.ToString();
            treeView1.Nodes.Clear();

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    svaRezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                        .FindByTakmicenjeFetch_Tak1_Gimnasticari(copyFromTakmicenje.Id);

                    const string BEZVEZE = "__BEZVEZE__";

                    string lastDescription = BEZVEZE;
                    TreeNode descNode = null;
                    foreach (RezultatskoTakmicenje rt in svaRezTakmicenja)
                    {
                        if (rt.TakmicenjeDescription.Naziv != lastDescription)
                        {
                            lastDescription = rt.TakmicenjeDescription.Naziv;
                            descNode = treeView1.Nodes.Add(rt.TakmicenjeDescription.Naziv);
                            descNode.Checked = true;
                        }
                        TreeNode katNode = descNode.Nodes.Add(rt.Kategorija.Naziv);
                        katNode.Tag = rt;
                        katNode.Checked = true;

                        List<GimnasticarUcesnik> gimnasticari = new List<GimnasticarUcesnik>(rt.Takmicenje1.Gimnasticari);
                        PropertyDescriptor propDesc =
                            TypeDescriptor.GetProperties(typeof(GimnasticarUcesnik))["KlubDrzava"];
                        gimnasticari.Sort(new SortComparer<GimnasticarUcesnik>(
                            propDesc, ListSortDirection.Ascending));

                        // nisam stavio String.Empty, za slucaj da neki gimnasticar nema ni klub ni drzavu
                        string lastKlub = BEZVEZE;
                        TreeNode klubNode = null;

                        foreach (GimnasticarUcesnik g in gimnasticari)
                        {
                            if (g.KlubDrzava != lastKlub)
                            {
                                lastKlub = g.KlubDrzava;
                                klubNode = katNode.Nodes.Add(g.KlubDrzava);
                                klubNode.Checked = true;
                            }
                            TreeNode gimNode = klubNode.Nodes.Add(g.ImeSrednjeImePrezimeDatumRodjenja);
                            gimNode.Tag = g;
                            gimNode.Checked = true;
                        }
                    }
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
        }

        void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // The code only executes if the user caused the checked state to change.
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                    CheckAllChildNodes(e.Node, e.Node.Checked);
            }
        }

        // Updates all child tree nodes recursively.
        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                    CheckAllChildNodes(node, nodeChecked);
            }
        }

        protected override void handleOkClick()
        {
            base.handleOkClick();
            if (DialogResult == DialogResult.OK && ckbKopirajPrethTak.Enabled && ckbKopirajPrethTak.Checked)
            {
                collectCheckedItems();
                if (rezTakmicenja.Count == 0)
                {
                    MessageDialogs.showMessage("Izaberite takmicenje/kategoriju sa prethodnog takmicenja.", this.Text);
                    DialogResult = DialogResult.None;
                }
            }
        }

        private void collectCheckedItems()
        {
            rezTakmicenja = new List<RezultatskoTakmicenje>();
            rezTakToGimnasticarMap = new Dictionary<int, List<GimnasticarUcesnik>>();
            foreach (TreeNode descNode in treeView1.Nodes)
            {
                if (!descNode.Checked)
                    continue;
                foreach (TreeNode katNode in descNode.Nodes)
                {
                    if (!katNode.Checked)
                        continue;

                    rezTakmicenja.Add((RezultatskoTakmicenje)katNode.Tag);

                    List<GimnasticarUcesnik> gimnasticari = new List<GimnasticarUcesnik>();
                    foreach (TreeNode klubNode in katNode.Nodes)
                    {
                        if (!klubNode.Checked)
                            continue;
                        foreach (TreeNode gimNode in klubNode.Nodes)
                        {
                            if (gimNode.Checked)
                                gimnasticari.Add((GimnasticarUcesnik)gimNode.Tag);
                        }
                    }
                    rezTakToGimnasticarMap.Add(((RezultatskoTakmicenje)katNode.Tag).Id, gimnasticari);
                }
            }
        }
    }
}