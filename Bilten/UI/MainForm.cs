using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Exceptions;
using Microsoft.Win32;
using Bilten.Domain;
using Bilten.Data;
using NHibernate;
using Bilten.Data.QueryModel;
using Iesi.Collections.Generic;

namespace Bilten.UI
{
    public partial class MainForm : Form
    {
        private Rectangle rectNormal;
        protected string strProgName;
        protected string strFileName;
        protected IDataContext dataContext;

        // TODO: Zameni ove tri promenljive sa promenljivom tipa Takmicenje (problem moze da bude to sto je takmicenjeId
        // tipa Nullable<int>, a takmicenje.Id je tipa int. Proveri sva mesta gde se koristi takmicenjeId)
        private string nazivTakmicenja;
        private bool finaleKupa;
        private Gimnastika gimnastika;
        private Nullable<int> takmicenjeId;
        Takmicenje takmicenje;

        //string strRegKey = "Software\\Sasa\\";
        const string strWinState = "WindowState";
        const string strLocationX = "LocationX";
        const string strLocationY = "LocationY";
        const string strWidth = "Width";
        const string strHeight = "Height";

        const string strFilter =
                          "Text Documents(*.txt)|*.txt|All Files(*.*)|*.*";

        // TODO: Remember to call ISessionFactory.Close() when you’re done using 
        // NHibernate. You’ll usually do it while closing the application.
        
        public MainForm()
        {
            InitializeComponent();
            mnSave.Visible = false;
            mnSaveAs.Visible = false;
            mnKategorijeITakmicenja.Enabled = false;
            mnPropozicije.Enabled = false;
            mnTakmicariKategorije.Enabled = false;
            mnTakmicariTakmicenja.Enabled = false;
            mnEkipe.Enabled = false;
            mnSudijeNaTakmicenju.Enabled = false;
        
            strProgName = Application.ProductName;
            mnTakmicenje1.Visible = false;
            mnTakmicenje2.Visible = false;
            mnTakmicenje3.Visible = false;
            mnTakmicenje4.Visible = false;
            rectNormal = DesktopBounds;
            MakeCaption();

            mnTakmicenje1RasporedSudija.Visible = false;
            mnTakmicenje1StartListe.Visible = true;
            mnOpcije.Visible = false;
            mnKopirajPrethodnoTakmicenje.Enabled = false;
        }

        protected override void OnMove(EventArgs ea)
        {
            base.OnMove(ea);

            if (WindowState == FormWindowState.Normal)
                rectNormal = DesktopBounds;
        }

        protected override void OnResize(EventArgs ea)
        {
            base.OnResize(ea);

            if (WindowState == FormWindowState.Normal)
                rectNormal = DesktopBounds;
        }

        protected override void OnLoad(EventArgs ea)
        {
            base.OnLoad(ea);
/*
            // Construct complete registry key.
            strRegKey = strRegKey + strProgName;

            // Load registry information.
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey(strRegKey);

            if (regkey != null)
            {
                LoadRegistryInfo(regkey);
                regkey.Close();
            }
        
            // Deal with the command-line argument.
            string[] astrArgs = Environment.GetCommandLineArgs();
            if (astrArgs.Length > 1)      // First argument is program name!
            {
                if (File.Exists(astrArgs[1]))
                {
                    LoadFile(astrArgs[1]);
                }
                else
                {
                    DialogResult dr =
                         MessageBox.Show("Cannot find the " +
                                         Path.GetFileName(astrArgs[1]) +
                                         " file.\r\n\r\n" +
                                         "Do you want to create a new file?",
                                         strProgName,
                                         MessageBoxButtons.YesNoCancel,
                                         MessageBoxIcon.Question);
                    switch (dr)
                    {
                        case DialogResult.Yes:   // Create and close file.
                            File.Create(strFileName = astrArgs[1]).Close();
                            MakeCaption();
                            break;

                        case DialogResult.No:
                            break;

                        case DialogResult.Cancel:
                            Close();
                            break;
                    }
                }
            }*/
        }

        protected override void OnClosing(CancelEventArgs cea)
        {
            base.OnClosing(cea);

            cea.Cancel = !OkToTrash();
        }
        
        protected override void OnClosed(EventArgs ea)
        {
            base.OnClosed(ea);
/*
            // Save registry information.
            RegistryKey regkey =
                           Registry.CurrentUser.OpenSubKey(strRegKey, true);

            if (regkey == null)
                regkey = Registry.CurrentUser.CreateSubKey(strRegKey);

            SaveRegistryInfo(regkey);
            regkey.Close();*/
        }

        protected virtual void SaveRegistryInfo(RegistryKey regkey)
        {
            regkey.SetValue(strWinState, (int)WindowState);
            regkey.SetValue(strLocationX, rectNormal.X);
            regkey.SetValue(strLocationY, rectNormal.Y);
            regkey.SetValue(strWidth, rectNormal.Width);
            regkey.SetValue(strHeight, rectNormal.Height);
        }

        protected virtual void LoadRegistryInfo(RegistryKey regkey)
        {
            int x = (int)regkey.GetValue(strLocationX, 100);
            int y = (int)regkey.GetValue(strLocationY, 100);
            int cx = (int)regkey.GetValue(strWidth, 300);
            int cy = (int)regkey.GetValue(strHeight, 300);

            rectNormal = new Rectangle(x, y, cx, cy);

            // Adjust rectangle for any change in desktop size.
            Rectangle rectDesk = SystemInformation.WorkingArea;

            rectNormal.Width = Math.Min(rectNormal.Width, rectDesk.Width);
            rectNormal.Height = Math.Min(rectNormal.Height, rectDesk.Height);
            rectNormal.X -= Math.Max(rectNormal.Right - rectDesk.Right, 0);
            rectNormal.Y -= Math.Max(rectNormal.Bottom - rectDesk.Bottom, 0);

            // Set form properties.
            DesktopBounds = rectNormal;
            WindowState = (FormWindowState)regkey.GetValue(strWinState, 0);
        }

        private void datotekaZatvoriMenu_Click(object sender, EventArgs e)
        {
            if (OkToTrash())
                Application.Exit();
        }

        private void registarGimnasticariMenu_Click(object sender, EventArgs e)
        {
            // NOTE: Postupak ukljucivanja kompletnog postojeceg foldera sa fajlovima
            // u projekt: Najpre se folder prebaci na odgovarajuce mesto. Zatim se
            // izabere Project->Show all files (ili precica u Solution Exploreru),
            // zatim se klikne desnim tasterom misa na odgovarajuci folder i izabere
            // Include in Project
            
            try
            {
                GimnasticariForm form = new GimnasticariForm();
                form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                // NOTE: Izuzetak moze da potice samo iz konstruktora. Kada se form
                // uspesno kreira i prikaze pozivom ShowDialog (ili Show), tada se
                // izuzetci unutar forma ne propagiraju do koda koji je pozvao
                // ShowDialog.
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void registarSudijeMenu_Click(object sender, EventArgs e)
        {
            try
            {
                SudijeForm form = new SudijeForm();
                form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void registarKluboviMenu_Click(object sender, EventArgs e)
        {
            try
            {
                KluboviForm form = new KluboviForm();
                form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void registarDrzaveMenu_Click(object sender, EventArgs e)
        {
            try
            {
                DrzaveForm form = new DrzaveForm();
                form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void registarKategorijeGimnasticaraMenu_Click(object sender, EventArgs e)
        {
            try
            {
                KategorijeGimnasticaraForm form = new KategorijeGimnasticaraForm();
                form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnNew_Click(object sender, EventArgs e)
        {
            if (!OkToTrash())
                return;

            try
            {
                TakmicenjeForm form = new TakmicenjeForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Takmicenje t = (Takmicenje)form.Entity;
                    newTakmicenje(t);
                    loadBrojDecimalaUOpcije(t);
                    Opcije.Instance.HeaderFooterInitialized = false;
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }


            // clear UI
            //txtbox.Clear();
            //txtbox.ClearUndo();
            //txtbox.Modified = false;

            //strFileName = null;
            //MakeCaption();
        }

        private void loadBrojDecimalaUOpcije(Takmicenje t)
        {
            Opcije.Instance.BrojDecimalaD = t.BrojDecimalaD;
            Opcije.Instance.BrojDecimalaE1 = t.BrojDecimalaE1;
            Opcije.Instance.BrojDecimalaE = t.BrojDecimalaE;
            Opcije.Instance.BrojDecimalaPen = t.BrojDecimalaPen;
            Opcije.Instance.BrojDecimalaTotal = t.BrojDecimalaTotal;
        }

        private void newTakmicenje(Takmicenje takmicenje)
        {
            this.takmicenje = takmicenje;
            takmicenjeId = takmicenje.Id;
            nazivTakmicenja = takmicenje.GimnastikaNaziv;
            gimnastika = takmicenje.Gimnastika;
            finaleKupa = takmicenje.FinaleKupa;
            mnKategorijeITakmicenja.Enabled = true;
            mnPropozicije.Enabled = true;
            mnTakmicariKategorije.Enabled = true;
            mnTakmicariTakmicenja.Enabled = true;
            mnEkipe.Enabled = true;
            mnSudijeNaTakmicenju.Enabled = true;
            mnTakmicenje1.Visible = true;
            mnTakmicenje2.Visible = false;
            mnTakmicenje3.Visible = false;
            mnTakmicenje4.Visible = false;

            mnKopirajPrethodnoTakmicenje.Enabled = true;

            mnPrvoDrugoKoloViseboj.Visible = takmicenje.FinaleKupa;
            mnPrvoDrugoKoloSprave.Visible = takmicenje.FinaleKupa;
            mnPrvoDrugoKoloEkipno.Visible = takmicenje.FinaleKupa;
            mnKreirajTakmicenja234.Visible = !takmicenje.FinaleKupa;
            mnZrebZaFinaleKupa.Visible = takmicenje.FinaleKupa;

            MakeCaption();
        }

        private void mnOpen_Click(object sender, EventArgs e)
        {
            if (!OkToTrash())
                return;
            try
            {
                OtvoriTakmicenjeForm form = new OtvoriTakmicenjeForm(takmicenjeId, false, 0);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    loadTakmicenje(form.Takmicenje);
                    loadBrojDecimalaUOpcije(form.Takmicenje);
                    Opcije.Instance.HeaderFooterInitialized = false;
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }

            /*     
            if (!OkToTrash())
                 return;

             OpenFileDialog ofd = new OpenFileDialog();
             ofd.Filter = strFilter;
             ofd.FileName = "*.txt";

             if (ofd.ShowDialog() == DialogResult.OK)
                 LoadFile(ofd.FileName);
             */
        }

        private void mnSave_Click(object sender, EventArgs e)
        {
            if (strFileName == null || strFileName.Length == 0)
                SaveFileDlg();
            else
                SaveFile();
        }

        private void mnSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDlg();
        }

        // Utility routines
        protected void LoadFile(string strFileName)
        {
    /*        StreamReader sr;
            try
            {
                sr = new StreamReader(strFileName);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, strProgName,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Asterisk);
                return;
            }
            txtbox.Text = sr.ReadToEnd();
            sr.Close();

            this.strFileName = strFileName;
            MakeCaption();

            txtbox.SelectionStart = 0;
            txtbox.SelectionLength = 0;
            txtbox.Modified = false;
            txtbox.ClearUndo();*/
        }

        private void loadTakmicenje(Takmicenje takmicenje)
        {
            takmicenjeId = takmicenje.Id;
            this.takmicenje = takmicenje;
            nazivTakmicenja = takmicenje.GimnastikaNaziv;
            gimnastika = takmicenje.Gimnastika;
            finaleKupa = takmicenje.FinaleKupa;
            mnKategorijeITakmicenja.Enabled = true;
            mnPropozicije.Enabled = true;
            mnTakmicariKategorije.Enabled = true;
            mnTakmicariTakmicenja.Enabled = true;
            mnEkipe.Enabled = true;
            mnSudijeNaTakmicenju.Enabled = true;
            mnTakmicenje1.Visible = true;
            mnTakmicenje2.Visible = takmicenje.ZavrsenoTak1;
            mnTakmicenje3.Visible = takmicenje.ZavrsenoTak1;
            mnTakmicenje4.Visible = takmicenje.ZavrsenoTak1;

            mnKopirajPrethodnoTakmicenje.Enabled = false;
            
            mnPrvoDrugoKoloViseboj.Visible = takmicenje.FinaleKupa;
            mnPrvoDrugoKoloSprave.Visible = takmicenje.FinaleKupa;
            mnPrvoDrugoKoloEkipno.Visible = takmicenje.FinaleKupa;
            mnKreirajTakmicenja234.Visible = !takmicenje.FinaleKupa;
            mnZrebZaFinaleKupa.Visible = takmicenje.FinaleKupa;

            MakeCaption();
        }

        void SaveFile()
        {
      /*      try
            {
                StreamWriter sw = new StreamWriter(strFileName, false,
                                                   mieChecked.Encoding);
                sw.Write(txtbox.Text);
                sw.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, strProgName,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Asterisk);
                return;
            }
            txtbox.Modified = false;*/
        }

        bool SaveFileDlg()
        {
            return false;
      /*      SaveFileDialog sfd = new SaveFileDialog();

            if (strFileName != null && strFileName.Length > 1)
                sfd.FileName = strFileName;
            else
                sfd.FileName = "*.txt";

            sfd.Filter = strFilter;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                strFileName = sfd.FileName;
                SaveFile();
                MakeCaption();
                return true;
            }
            else
            {
                return false;       // Return values are for OkToTrash.
            }*/
        }

        protected void MakeCaption()
        {
            Text = strProgName;
            if (!String.IsNullOrEmpty(FileTitle()))
                Text += " - " + FileTitle();
        }

        protected string FileTitle()
        {
            return takmicenjeId != null ? nazivTakmicenja : "";
            //return (strFileName != null && strFileName.Length > 1) ?
              //             Path.GetFileName(strFileName) : "Untitled";
        }

        protected bool OkToTrash()
        {
            return true;
   /*       if (!txtbox.Modified)
                return true;

            DialogResult dr =
                      MessageBox.Show("The text in the " + FileTitle() +
                                      " file has changed.\r\n\r\n" +
                                      "Do you want to save the changes?",
                                      strProgName,
                                      MessageBoxButtons.YesNoCancel,
                                      MessageBoxIcon.Exclamation);
            switch (dr)
            {
                case DialogResult.Yes:
                    return SaveFileDlg();

                case DialogResult.No:
                    return true;

                case DialogResult.Cancel:
                    return false;
            }
            return false;*/
        }

        private void mnKategorijeITakmicenja_Click(object sender, EventArgs e)
        {
            try
            {
                TakmicarskeKategorijeForm form = new TakmicarskeKategorijeForm(takmicenjeId.Value);
                DialogResult dlgResult = form.ShowDialog();
                if (dlgResult == DialogResult.OK)
                    mnKopirajPrethodnoTakmicenje.Enabled = false;
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnPropozicije_Click(object sender, EventArgs e)
        {
            try
            {
                PropozicijeForm dlg = new PropozicijeForm(takmicenjeId.Value);
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                Takmicenje t = loadTakmicenje(takmicenjeId.Value);
                loadBrojDecimalaUOpcije(t);
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private Takmicenje loadTakmicenje(int takmicenjeId)
        {
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                Takmicenje t = dataContext.GetById<Takmicenje>(takmicenjeId);
                NHibernateUtil.Initialize(t);
                return t;
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

        private void mnTakmicariKategorije_Click(object sender, EventArgs e)
        {
            try
            {
                TakmicariKategorijeForm form = 
                    new TakmicariKategorijeForm(takmicenjeId.Value);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnTakmicariTakmicenja_Click(object sender, EventArgs e)
        {
            try
            {
                TakmicariTakmicenjaForm form =
                    new TakmicariTakmicenjaForm(takmicenjeId.Value);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }
        
        private void mnEkipe_Click(object sender, EventArgs e)
        {
            try
            {
                EkipeForm form = new EkipeForm(takmicenjeId.Value);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnMesta_Click(object sender, EventArgs e)
        {
            try
            {
                MestaForm form = new MestaForm();
                form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnSudijeNaTakmicenju_Click(object sender, EventArgs e)
        {
            try
            {
                SudijeUcesniciForm form = new SudijeUcesniciForm(takmicenjeId.Value);
                form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnTakmicenje1RasporedSudija_Click(object sender, EventArgs e)
        {
            try
            {
                RasporedSudijaForm form = new RasporedSudijaForm(takmicenjeId.Value,
                    DeoTakmicenjaKod.Takmicenje1);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnTakmicenje1StartListe_Click(object sender, EventArgs e)
        {
            try
            {
                StartListeForm form = new StartListeForm(takmicenjeId.Value,
                    DeoTakmicenjaKod.Takmicenje1);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnTakmicenje1Ocene_Click(object sender, EventArgs e)
        {
            try
            {
                OceneForm form = new OceneForm(takmicenjeId.Value, DeoTakmicenjaKod.Takmicenje1);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnOpcijeOpcije_Click(object sender, EventArgs e)
        {
            OpcijeForm f = new OpcijeForm(takmicenjeId);
            f.ShowDialog();
        }

        private void mnZatvoriTakmicenje_Click(object sender, EventArgs e)
        {
            // TODO: 
        }

        private void mnRezultatiUkupnoTak1_Click(object sender, EventArgs e)
        {
            try
            {
                RezultatiUkupnoForm form = new RezultatiUkupnoForm(takmicenjeId.Value,
                    DeoTakmicenjaKod.Takmicenje1);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnTakmicenje2Rezultati_Click(object sender, EventArgs e)
        {
            try
            {
                RezultatiUkupnoForm form = new RezultatiUkupnoForm(
                    takmicenjeId.Value, DeoTakmicenjaKod.Takmicenje2);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnKreirajTakmicenja234_Click(object sender, EventArgs e)
        {
            string msg = "Da li zelite da kreirate takmicenja II, III i IV?";
            if (!MessageDialogs.queryConfirmation(msg, "Kreiraj takmicenja II, III i IV"))
                return;

            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                IList<RezultatskoTakmicenje> rezTakmicenja = loadRezTakmicenja(
                    dataContext, takmicenjeId.Value);
                if (rezTakmicenja.Count == 0)
                    throw new BusinessException("Morate najpre da unesete takmicarske kategorije.");

                Cursor.Current = Cursors.WaitCursor;
                Cursor.Show();

                IList<Ocena> ocene = loadOceneTak1(takmicenjeId.Value);

                foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                {
                    // Ovo je zakomentarisano zato sto je moguce da je poredak rucno promenjen, pa ga ne treba ponovo
                    // kreirati.
                    /*rt.Takmicenje1.PoredakUkupno.create(rt, ocene);
                    foreach (PoredakSprava p in rt.Takmicenje1.PoredakSprava)
                        p.create(rt, ocene);
                    rt.Takmicenje1.PoredakPreskok.create(rt, ocene);*/

                    // TODO3: Zakomentarisi ovo kada budes ponovo uveo da se ekipni poredak automatski azurira.
                    rt.Takmicenje1.PoredakEkipno.create(rt, ocene);

                    if (rt.Propozicije.PostojiTak2)
                    {
                        rt.Takmicenje2.createUcesnici(rt.Takmicenje1);
                        if (rt.Propozicije.OdvojenoTak2)
                            rt.Takmicenje2.Poredak.initRezultati(rt.Takmicenje2.getUcesniciGimKvalifikanti());
                        else
                            rt.Takmicenje2.Poredak.create(rt, ocene);
                    }
                    if (rt.Propozicije.PostojiTak3)
                    {
                        rt.Takmicenje3.createUcesnici(rt.Takmicenje1, rt.Propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka);
                        if (rt.Propozicije.OdvojenoTak3)
                        {
                            foreach (PoredakSprava p in rt.Takmicenje3.Poredak)
                                p.initRezultati(rt.Takmicenje3.getUcesniciGimKvalifikanti(p.Sprava));
                            rt.Takmicenje3.PoredakPreskok.initRezultati(rt.Takmicenje3.getUcesniciGimKvalifikanti(Sprava.Preskok));
                        }
                        else
                        {
                            foreach (PoredakSprava p in rt.Takmicenje3.Poredak)
                                p.create(rt, ocene);
                            rt.Takmicenje3.PoredakPreskok.create(rt, ocene);
                        }
                    }
                    if (rt.Propozicije.PostojiTak4)
                    {
                        /*
                        rt.Takmicenje4.createUcesnici(rt.Takmicenje1);
                        if (rt.Propozicije.OdvojenoTak4)
                            rt.Takmicenje4.Poredak.initRezultati(rt.Takmicenje4.getUcesnici());
                        else
                            rt.Takmicenje4.Poredak.create(rt, ocene);
                         */
                    }

                    dataContext.Save(rt.Takmicenje1);
                    if (rt.Propozicije.PostojiTak2)
                        dataContext.Save(rt.Takmicenje2);
                    if (rt.Propozicije.PostojiTak3)
                        dataContext.Save(rt.Takmicenje3);
                    //if (rt.Propozicije.PostojiTak4)
                      //  dataContext.Save(rt.Takmicenje4);
                    
                    Takmicenje t = dataContext.GetById<Takmicenje>(takmicenjeId.Value);
                    t.ZavrsenoTak1 = true;
                    dataContext.Save(t);
                
                    mnTakmicenje2.Visible = true;
                    mnTakmicenje3.Visible = true;
                    mnTakmicenje4.Visible = true;
                }

                dataContext.Commit();
            }
            catch (BusinessException ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(ex.Message, strProgName);
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(Strings.getFullDatabaseAccessExceptionMessage(ex), strProgName);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;

                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }

        }
        private IList<RezultatskoTakmicenje> loadRezTakmicenja(IDataContext dataContext,
            int takmicenjeId)
        {
            string query = @"select distinct r
                from RezultatskoTakmicenje r
                left join fetch r.Takmicenje1 t
                left join fetch t.PoredakUkupno
                left join fetch t.Gimnasticari g
                left join fetch g.DrzavaUcesnik dr
                left join fetch g.KlubUcesnik kl
                where r.Takmicenje.Id = :takmicenjeId";

            IList<RezultatskoTakmicenje> result = dataContext.
                ExecuteQuery<RezultatskoTakmicenje>(QueryLanguageType.HQL, query,
                        new string[] { "takmicenjeId" },
                        new object[] { takmicenjeId });
            foreach (RezultatskoTakmicenje tak in result)
            {
                NHibernateUtil.Initialize(tak.Takmicenje1.PoredakUkupno.Rezultati);

                // potrebno u Poredak.create
                NHibernateUtil.Initialize(tak.Propozicije);
            }
            return result;
        }

        private IList<Ocena> loadOceneTak1(int takmicenjeId)
        {
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                return dataContext.ExecuteNamedQuery<Ocena>(
                    "FindOceneByDeoTakmicenja",
                    new string[] { "takId", "deoTakKod" },
                    new object[] { takmicenjeId, DeoTakmicenjaKod.Takmicenje1 });
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

        private void mnTakmicenje1RezultatiSprave_Click(object sender, EventArgs e)
        {
            try
            {
                RezultatiSpravaForm form = new RezultatiSpravaForm(takmicenjeId.Value,
                    DeoTakmicenjaKod.Takmicenje1, false, null, Sprava.Undefined);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnTakmicenje3Rezultati_Click(object sender, EventArgs e)
        {
            try
            {
                RezultatiSpravaForm form = new RezultatiSpravaForm(takmicenjeId.Value,
                    DeoTakmicenjaKod.Takmicenje3, false, null, Sprava.Undefined);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnKvalifikantiTak2_Click(object sender, EventArgs e)
        {
            try
            {
                KvalifikantiTak2Form form = new KvalifikantiTak2Form(takmicenjeId.Value);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnKvalifikantiTak3_Click(object sender, EventArgs e)
        {
            try
            {
                KvalifikantiTak3Form form = new KvalifikantiTak3Form(takmicenjeId.Value);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnTakmicenje2Ocene_Click(object sender, EventArgs e)
        {
            try
            {
                OceneForm form = new OceneForm(takmicenjeId.Value, DeoTakmicenjaKod.Takmicenje2);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnTakmicenje3Ocene_Click(object sender, EventArgs e)
        {
            try
            {
                OceneForm form = new OceneForm(takmicenjeId.Value, DeoTakmicenjaKod.Takmicenje3);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnTakmicenje1RezultatiEkipe_Click(object sender, EventArgs e)
        {
            try
            {
                RezultatiEkipeForm form = new RezultatiEkipeForm(takmicenjeId.Value,
                    DeoTakmicenjaKod.Takmicenje1);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnTakmicenje3StartListe_Click(object sender, EventArgs e)
        {
            try
            {
                StartListeForm form = new StartListeForm(takmicenjeId.Value,
                    DeoTakmicenjaKod.Takmicenje3);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void zrebZaFinalePoSpravamaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ZrebForm form = new ZrebForm(takmicenjeId.Value);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnPrvoDrugoKoloViseboj_Click(object sender, EventArgs e)
        {
            try
            {
                RezultatiUkupnoFinaleKupaForm form = new RezultatiUkupnoFinaleKupaForm(takmicenjeId.Value);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnKopirajPrethodnoTakmicenje_Click(object sender, EventArgs e)
        {
            // TODO3: Za finale kupa treba da se ponudi da se kopiraju prva dva kola.
            KopirajTakmicenjeForm form;
            DialogResult result;
            try
            {
                form = new KopirajTakmicenjeForm(this.gimnastika);
                result = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (result != DialogResult.OK || form.SelDescriptions.Count == 0 || form.SelKategorije.Count == 0)
                return;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                cloneTakmicenje(takmicenje, form.Takmicenje, form.SelDescriptions, form.SelKategorije);
                dataContext.Commit();
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

                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }

            mnKopirajPrethodnoTakmicenje.Enabled = false;
        }

        void cloneTakmicenje(Takmicenje takmicenje, Takmicenje from, List<RezultatskoTakmicenjeDescription> descriptionsFrom,
            List<TakmicarskaKategorija> kategorijeFrom)
        {
            dataContext.Attach(takmicenje, false);
            dataContext.Attach(from, false);
            foreach (RezultatskoTakmicenjeDescription d in descriptionsFrom)
            {
                dataContext.Attach(d, false);
            }
            
            // TODO3: Ovaj metod bi trebalo updateovati svaki put kada se promene neka svojstva koja se kloniraju.

            takmicenje.BrojESudija = from.BrojESudija;
            takmicenje.BrojLinijskihSudija = from.BrojLinijskihSudija;
            takmicenje.BrojMeracaVremena = from.BrojMeracaVremena;
            takmicenje.BrojDecimalaD = from.BrojDecimalaD;
            takmicenje.BrojDecimalaE1 = from.BrojDecimalaE1;
            takmicenje.BrojDecimalaE = from.BrojDecimalaE;
            takmicenje.BrojDecimalaPen = from.BrojDecimalaPen;
            takmicenje.BrojDecimalaTotal = from.BrojDecimalaTotal;
            takmicenje.ZavrsenoTak1 = false;

            // TODO: Kreiraj metod u klasi TakmicarskaKategorija koji vraca kategorije sortirane po rednom broju.
            // Pronadji sva mesta na kojima sortiram kategorije po rednom broju, i zameni ih pozivom novog metoda.
            // Uradi isto i za klasu RezultatskoTakmicenjeDescription, a i za druge ako postoje.
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(TakmicarskaKategorija))["RedBroj"];
            kategorijeFrom.Sort(new SortComparer<TakmicarskaKategorija>(
                propDesc, ListSortDirection.Ascending));
            foreach (TakmicarskaKategorija k in kategorijeFrom)
            {
                takmicenje.addKategorija(new TakmicarskaKategorija(k.Naziv, takmicenje.Gimnastika));
            }

            PropertyDescriptor propDesc2 =
                TypeDescriptor.GetProperties(typeof(RezultatskoTakmicenjeDescription))["RedBroj"];
            descriptionsFrom.Sort(new SortComparer<RezultatskoTakmicenjeDescription>(
                propDesc2, ListSortDirection.Ascending));

            // prvi description je uvek kao naziv takmicenja.
            RezultatskoTakmicenjeDescription desc = new RezultatskoTakmicenjeDescription();
            desc.Naziv = takmicenje.Naziv;
            desc.Propozicije = new Propozicije();
            takmicenje.addTakmicenjeDescription(desc);
            for (int i = 1; i < descriptionsFrom.Count; i++)
            {
                desc = new RezultatskoTakmicenjeDescription();
                desc.Naziv = descriptionsFrom[i].Naziv;
                desc.Propozicije = new Propozicije();
                takmicenje.addTakmicenjeDescription(desc);
            }

            IList<RezultatskoTakmicenje> rezTakmicenja = new List<RezultatskoTakmicenje>();
            foreach (RezultatskoTakmicenjeDescription d in takmicenje.TakmicenjeDescriptions)
            {
                foreach (TakmicarskaKategorija k in takmicenje.Kategorije)
                {
                    RezultatskoTakmicenje rt = new RezultatskoTakmicenje(takmicenje,
                        k, d, new Propozicije());
                    rezTakmicenja.Add(rt);
                }
            }

            List<RezultatskoTakmicenjeDescription> descriptions =
                new List<RezultatskoTakmicenjeDescription>(takmicenje.TakmicenjeDescriptions);
            propDesc = TypeDescriptor.GetProperties(typeof(RezultatskoTakmicenjeDescription))["RedBroj"];
            descriptions.Sort(new SortComparer<RezultatskoTakmicenjeDescription>(
                propDesc, ListSortDirection.Ascending));
            for (int i = 0; i < descriptions.Count; i++)
            {
                clonePropozicije(descriptions[i].Propozicije, descriptionsFrom[i].Propozicije);
            }

            IList<RezultatskoTakmicenje> rezTakmicenjaFrom = new List<RezultatskoTakmicenje>();
            foreach (RezultatskoTakmicenje rt in loadRezTakmicenja(from.Id))
            {
                // filtriraj rez. takmicenja.
                ISet<TakmicarskaKategorija> katFromSet = new HashedSet<TakmicarskaKategorija>(kategorijeFrom);
                ISet<RezultatskoTakmicenjeDescription> descFromSet
                    = new HashedSet<RezultatskoTakmicenjeDescription>(descriptionsFrom);
                if (katFromSet.Contains(rt.Kategorija) && descFromSet.Contains(rt.TakmicenjeDescription))
                {
                    rezTakmicenjaFrom.Add(rt);
                }

            }
            foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
            {
                RezultatskoTakmicenje rezTak2 = findRezTakmicenje(rezTakmicenjaFrom, rezTak.TakmicenjeDescription.Naziv,
                    rezTak.Kategorija);
                clonePropozicije(rezTak.Propozicije, rezTak2.Propozicije);
            }

            bool kombAdded = false;
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
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
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                bool deletedTak2, deletedTak3, deletedTak4;

                rt.updateTakmicenjaFromChangedPropozicije(
                    out deletedTak2, out deletedTak3, out deletedTak4);

                /*if (deletedTak2)
                    dataContext.Delete(rt.Takmicenje2);
                if (deletedTak3)
                    dataContext.Delete(rt.Takmicenje3);
                if (deletedTak4)
                    dataContext.Delete(rt.Takmicenje4);*/
            }

            IDictionary<GimnasticarUcesnik, GimnasticarUcesnik> gimnasticariMap =
                new Dictionary<GimnasticarUcesnik, GimnasticarUcesnik>();
            List<GimnasticarUcesnik> dupliGimnasticari = new List<GimnasticarUcesnik>();
            foreach (TakmicarskaKategorija kat in takmicenje.Kategorije)
            {
                foreach (GimnasticarUcesnik g in loadGimnasticari(from, kat.Naziv))
                {
                    if (!gimnasticariMap.ContainsKey(g))
                    {
                        // Ovo proveravam zato sto se vec desilo da isti gimnasticar bude prijavljen u dve razlicite
                        // kategorije (program to ne proverava). Npr. na takmicenju I KOLO PGL SRBIJE MSG od 21.05.2011.
                        // Sinisa Jurkovic je bio prijavljen u dve kategorije. U novoj verziji programa nije moguce da
                        // isti gimnsticar bude prijavljen u dve razlicite kategorije.

                        GimnasticarUcesnik g2 = createGimnasticarUcesnik(g, kat);
                        gimnasticariMap.Add(g2, g2);
                    }
                    else
                        dupliGimnasticari.Add(g);
                }
            }
            // Proveravanje gimnasticara koji su prijavljeni u vise kategorija je samo za legacy baze. Program je vec
            // promenjen da ne dozvoljava da isti gimnasticar bude prijavljen u vise kategorija.
            foreach (GimnasticarUcesnik g in dupliGimnasticari)
            {
                if (gimnasticariMap.ContainsKey(g))
                    gimnasticariMap.Remove(g);
            }

            // TODO2: Izgleda da prikazuje rezultate za sprave u takmicenju I cak i kada se u propozicijama selektuje da
            // ne postoji takmicenje III. U stvari tako i treba zato sto ipak treba da postoji pregled nastupa po spravama
            // cak i kada se selektuje da ne postoji takmicenje III. Tako da ovo treba da ostane.

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                RezultatskoTakmicenje rtFrom = findRezTakmicenje(rezTakmicenjaFrom, rt.TakmicenjeDescription.Naziv,
                    rt.Kategorija);
                foreach (GimnasticarUcesnik g in rtFrom.Takmicenje1.Gimnasticari)
                {
                    if (gimnasticariMap.ContainsKey(g))
                    {
                        GimnasticarUcesnik g2 = gimnasticariMap[g];
                        rt.Takmicenje1.addGimnasticar(g2);
                        rt.Takmicenje1.gimnasticarAdded(g2, new List<Ocena>(), rt);

                    }
                }
            }

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                RezultatskoTakmicenje rtFrom = findRezTakmicenje(rezTakmicenjaFrom, rt.TakmicenjeDescription.Naziv,
                    rt.Kategorija);
                foreach (Ekipa e in rtFrom.Takmicenje1.Ekipe)
                {
                    Ekipa ekipa = new Ekipa();
                    ekipa.Naziv = e.Naziv;
                    ekipa.Kod = e.Kod;
                    if (e.DrzavaUcesnik == null)
                        ekipa.DrzavaUcesnik = null;
                    else
                    {
                        DrzavaUcesnik drzavaUcesnik = findDrzavaUcesnik(takmicenje, e.DrzavaUcesnik.Naziv);
                        if (drzavaUcesnik == null)
                        {
                            drzavaUcesnik = new DrzavaUcesnik();
                            drzavaUcesnik.Naziv = e.DrzavaUcesnik.Naziv;
                            drzavaUcesnik.Kod = e.DrzavaUcesnik.Kod;
                            drzavaUcesnik.Takmicenje = takmicenje;
                            dataContext.Add(drzavaUcesnik);
                        }
                        ekipa.DrzavaUcesnik = drzavaUcesnik;
                    }
                    if (e.KlubUcesnik == null)
                        ekipa.KlubUcesnik = null;
                    else
                    {
                        KlubUcesnik klubUcesnik = findKlubUcesnik(takmicenje, e.KlubUcesnik.Naziv);
                        if (klubUcesnik == null)
                        {
                            klubUcesnik = new KlubUcesnik();
                            klubUcesnik.Naziv = e.KlubUcesnik.Naziv;
                            klubUcesnik.Kod = e.KlubUcesnik.Kod;
                            klubUcesnik.Takmicenje = takmicenje;
                            dataContext.Add(klubUcesnik);
                        }
                        ekipa.KlubUcesnik = klubUcesnik;
                    }

                    foreach (GimnasticarUcesnik g in e.Gimnasticari)
                    {
                        if (gimnasticariMap.ContainsKey(g))
                        {
                            GimnasticarUcesnik g2 = gimnasticariMap[g];
                            ekipa.addGimnasticar(g2);
                        }
                    }

                    rt.Takmicenje1.addEkipa(ekipa);
                }
            }

            dataContext.Add(takmicenje);
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                dataContext.Add(rt);
                foreach (Ekipa e in rt.Takmicenje1.Ekipe)
                    dataContext.Add(e);
            }
            foreach (GimnasticarUcesnik g in gimnasticariMap.Values)
            {
                dataContext.Add(g);
            }

            if (dupliGimnasticari.Count > 0)
            {
                string msg = "Sledeci gimnasticari nisu dodati zato sto su prijavljeni u vise " +
                    "kategorija.  Ove gimnasticare morate da unesete rucno: \n\n";
                msg += StringUtil.getListString(dupliGimnasticari.ToArray());
                MessageDialogs.showMessage(msg, this.Text);
            }
        }

        private RezultatskoTakmicenje findRezTakmicenje(IList<RezultatskoTakmicenje> rezTakmicenja,
            string nazivDesc, TakmicarskaKategorija kat)
        {
            foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
            {
                if (rezTak.TakmicenjeDescription.Naziv == nazivDesc
                && rezTak.Kategorija.Equals(kat))
                    return rezTak;
            }

            // Nije pronadjeno rez. takmicenje. Ovo je situacija koja se najcesce desava zato sto se najcesce kopira
            // takmicenje za koje postoji samo jedno takmicenje description, i data se description takmicenja koje se
            // kopira i description takmicenja u koje se kopira ne poklapaju (naziv descriptiona po defaultu je naziv
            // takmicenja). Pronadji sva descriptions za datu kategoriju. Description koji trazimo je descriptions sa
            // najnizim brojem.
            List<RezultatskoTakmicenjeDescription> descriptions = new List<RezultatskoTakmicenjeDescription>();
            foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
            {
                if (rezTak.Kategorija.Equals(kat))
                    descriptions.Add(rezTak.TakmicenjeDescription);
            }
            PropertyDescriptor propDesc = TypeDescriptor.GetProperties(typeof(RezultatskoTakmicenjeDescription))["RedBroj"];
            descriptions.Sort(new SortComparer<RezultatskoTakmicenjeDescription>(
                propDesc, ListSortDirection.Ascending));

            // ponovljen kod sa pocetka funkcije
            foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
            {
                if (rezTak.TakmicenjeDescription.Naziv == descriptions[0].Naziv
                && rezTak.Kategorija.Equals(kat))
                    return rezTak;
            }

            return null;
        }

        IList<RezultatskoTakmicenje> loadRezTakmicenja(int takmicenjeId)
        {
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                string query = @"select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Propozicije p
                    left join fetch r.Takmicenje1 t
                    left join fetch t.Gimnasticari g
                    left join fetch t.Ekipe e
                    left join fetch e.Gimnasticari
                    left join fetch e.DrzavaUcesnik
                    left join fetch e.KlubUcesnik
                    where r.Takmicenje.Id = :takmicenjeId
                    order by r.RedBroj";

                IList<RezultatskoTakmicenje> result = dataContext.
                    ExecuteQuery<RezultatskoTakmicenje>(QueryLanguageType.HQL, query,
                            new string[] { "takmicenjeId" },
                            new object[] { takmicenjeId });
                return result;
            }
            catch (BusinessException)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw;
            }
            catch (InfrastructureException)
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

        private void clonePropozicije(Propozicije propozicije, Propozicije from)
        {
            // TODO3: Dodaj ono sto fali
            // TODO3: Probaj da koristis refleksiju za ovo (ili da ona izvrsi kopiranje, ili samo da te obavesti da li je
            // u medjuvremenu u klasi Propozicije dodato neko novo svojstvo, i ako jeste da generise izuzetak. Mogao bi i 
            // da generisem jednostavan test suite koji bi proveravao ovo)
            // TODO3: Uvedi komentar TODO9 za ono sto mora uvek da se proverava kada se menja program (kao naprimer ovde sto
            // mora da se proverava da li sam u medjuvremenu dodao novo svojstvo u klasu Propozicije.)
            propozicije.MaxBrojTakmicaraIzKlubaTak1 = from.MaxBrojTakmicaraIzKlubaTak1;
            propozicije.PostojiTak2 = from.PostojiTak2;
            propozicije.OdvojenoTak2 = from.OdvojenoTak2;
            propozicije.NeogranicenBrojTakmicaraIzKlubaTak2 = from.NeogranicenBrojTakmicaraIzKlubaTak2;
            propozicije.MaxBrojTakmicaraIzKlubaTak2 = from.MaxBrojTakmicaraIzKlubaTak2;
            propozicije.BrojFinalistaTak2 = from.BrojFinalistaTak2;
            propozicije.BrojRezerviTak2 = from.BrojRezerviTak2;
            propozicije.PostojiTak3 = from.PostojiTak3;
            propozicije.OdvojenoTak3 = from.OdvojenoTak3;
            propozicije.NeogranicenBrojTakmicaraIzKlubaTak3 = from.NeogranicenBrojTakmicaraIzKlubaTak3;
            propozicije.MaxBrojTakmicaraIzKlubaTak3 = from.MaxBrojTakmicaraIzKlubaTak3;
            propozicije.MaxBrojTakmicaraTak3VaziZaDrzavu = from.MaxBrojTakmicaraTak3VaziZaDrzavu;
            propozicije.BrojFinalistaTak3 = from.BrojFinalistaTak3;
            propozicije.BrojRezerviTak3 = from.BrojRezerviTak3;
            propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka = from.KvalifikantiTak3PreskokNaOsnovuObaPreskoka;
            propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka = from.PoredakTak3PreskokNaOsnovuObaPreskoka;
            propozicije.PostojiTak4 = from.PostojiTak4;
            propozicije.OdvojenoTak4 = from.OdvojenoTak4;
            propozicije.BrojRezultataKojiSeBodujuZaEkipu = from.BrojRezultataKojiSeBodujuZaEkipu;
            propozicije.BrojEkipaUFinalu = from.BrojEkipaUFinalu;
            propozicije.JednoTak4ZaSveKategorije = from.JednoTak4ZaSveKategorije;

            propozicije.Tak2FinalnaOcenaJeZbirObaKola = from.Tak2FinalnaOcenaJeZbirObaKola;
            propozicije.Tak2FinalnaOcenaJeMaxObaKola = from.Tak2FinalnaOcenaJeMaxObaKola;
            propozicije.Tak2FinalnaOcenaJeProsekObaKola = from.Tak2FinalnaOcenaJeProsekObaKola;
            propozicije.Tak2NeRacunajProsekAkoNemaOceneIzObaKola = from.Tak2NeRacunajProsekAkoNemaOceneIzObaKola;

            propozicije.Tak4FinalnaOcenaJeZbirObaKola = from.Tak4FinalnaOcenaJeZbirObaKola;
            propozicije.Tak4FinalnaOcenaJeMaxObaKola = from.Tak4FinalnaOcenaJeMaxObaKola;
            propozicije.Tak4FinalnaOcenaJeProsekObaKola = from.Tak4FinalnaOcenaJeProsekObaKola;
            propozicije.Tak4NeRacunajProsekAkoNemaOceneIzObaKola = from.Tak4NeRacunajProsekAkoNemaOceneIzObaKola;

        }

        private IList<GimnasticarUcesnik> loadGimnasticari(Takmicenje tak, string kategorija)
        {
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                string query = @"select distinct g
                 from GimnasticarUcesnik g
                 left join fetch g.KlubUcesnik
                 left join fetch g.DrzavaUcesnik
                 where g.Takmicenje = :tak
                 and g.TakmicarskaKategorija.Naziv = :kat
                 order by g.Prezime, g.Ime";

                IList<GimnasticarUcesnik> result = dataContext.
                    ExecuteQuery<GimnasticarUcesnik>(QueryLanguageType.HQL, query,
                            new string[] { "tak", "kat" },
                            new object[] { tak, kategorija });
                return result;
            }
            catch (BusinessException)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw;
            }
            catch (InfrastructureException)
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

        // TODO: Ovaj metod (i sledeca dva) je prekopiran iz klase TakmicariKategorijeForm. Probaj da oba metoda
        // spojis u jedan.
        private GimnasticarUcesnik createGimnasticarUcesnik(GimnasticarUcesnik g,
            TakmicarskaKategorija kategorija)
        {
            GimnasticarUcesnik result = new GimnasticarUcesnik();
            result.Ime = g.Ime;
            result.SrednjeIme = g.SrednjeIme;
            result.Prezime = g.Prezime;
            result.Gimnastika = g.Gimnastika;
            result.DatumRodjenja = g.DatumRodjenja;
            result.RegistarskiBroj = g.RegistarskiBroj;
            result.TakmicarskaKategorija = kategorija;
            result.Takmicenje = kategorija.Takmicenje;
            if (g.DrzavaUcesnik == null)
                result.DrzavaUcesnik = null;
            else
            {
                DrzavaUcesnik drzavaUcesnik = findDrzavaUcesnik(kategorija.Takmicenje,
                    g.DrzavaUcesnik.Naziv);
                if (drzavaUcesnik == null)
                {
                    drzavaUcesnik = new DrzavaUcesnik();
                    drzavaUcesnik.Naziv = g.DrzavaUcesnik.Naziv;
                    drzavaUcesnik.Kod = g.DrzavaUcesnik.Kod;
                    drzavaUcesnik.Takmicenje = kategorija.Takmicenje;
                    dataContext.Add(drzavaUcesnik);
                }
                result.DrzavaUcesnik = drzavaUcesnik;
            }
            if (g.KlubUcesnik == null)
                result.KlubUcesnik = null;
            else
            {
                KlubUcesnik klubUcesnik = findKlubUcesnik(kategorija.Takmicenje,
                    g.KlubUcesnik.Naziv);
                if (klubUcesnik == null)
                {
                    klubUcesnik = new KlubUcesnik();
                    klubUcesnik.Naziv = g.KlubUcesnik.Naziv;
                    klubUcesnik.Kod = g.KlubUcesnik.Kod;
                    klubUcesnik.Takmicenje = kategorija.Takmicenje;
                    dataContext.Add(klubUcesnik);
                }
                result.KlubUcesnik = klubUcesnik;
            }
            return result;
        }

        private DrzavaUcesnik findDrzavaUcesnik(Takmicenje takmicenje, string naziv)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, takmicenje));
            q.Criteria.Add(new Criterion("Naziv", CriteriaOperator.Equal, naziv));
            q.Operator = QueryOperator.And;
            IList<DrzavaUcesnik> result = dataContext.GetByCriteria<DrzavaUcesnik>(q);
            if (result.Count == 0)
                return null;
            else
                return result[0];
        }

        private KlubUcesnik findKlubUcesnik(Takmicenje takmicenje, string naziv)
        {
            Query q = new Query();
            q.Criteria.Add(new Criterion("Takmicenje", CriteriaOperator.Equal, takmicenje));
            q.Criteria.Add(new Criterion("Naziv", CriteriaOperator.Equal, naziv));
            q.Operator = QueryOperator.And;
            IList<KlubUcesnik> result = dataContext.GetByCriteria<KlubUcesnik>(q);
            if (result.Count == 0)
                return null;
            else
                return result[0];
        }

        private void mnPrvoDrugoKoloEkipno_Click(object sender, EventArgs e)
        {
            try
            {
                RezultatiEkipeFinaleKupaForm form = new RezultatiEkipeFinaleKupaForm(takmicenjeId.Value);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnPrvoDrugoKoloSprave_Click(object sender, EventArgs e)
        {
            try
            {
                RezultatiSpravaFinaleKupaForm form = new RezultatiSpravaFinaleKupaForm(takmicenjeId.Value);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

        private void mnZrebZaFinaleKupa_Click(object sender, EventArgs e)
        {
            try
            {
                ZrebForm form = new ZrebForm(takmicenjeId.Value);
                form.ShowDialog();
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
        }

    }
}
