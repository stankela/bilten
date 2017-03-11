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
using NHibernate.Context;
using Bilten.Dao;
using Bilten.Dao.NHibernate;

namespace Bilten.UI
{
    public partial class MainForm : Form
    {
        private Rectangle rectNormal;
        protected string strProgName;
        protected string strFileName;

        // TODO: Zameni ove tri promenljive sa promenljivom tipa Takmicenje (problem moze da bude to sto je takmicenjeId
        // tipa Nullable<int>, a takmicenje.Id je tipa int. Proveri sva mesta gde se koristi takmicenjeId)
        private string nazivTakmicenja;
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

            mnTakmicenje1RasporedSudija.Visible = true;
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
            /*clear UI
            txtbox.Clear();
            txtbox.ClearUndo();
            txtbox.Modified = false;

            strFileName = null;
            MakeCaption();*/

            if (!OkToTrash())
                return;

            bool ok = false;
            try
            {
                TakmicenjeForm form = new TakmicenjeForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Takmicenje t = (Takmicenje)form.Entity;
                    newTakmicenje(t);
                    loadBrojDecimalaUOpcije(t);
                    Opcije.Instance.HeaderFooterInitialized = false;
                    ok = true;
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }

            if (!ok)
                return;
            if (!takmicenje.ZbirViseKola)
                return;

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    if (takmicenje.ZbirViseKola)
                    {
                        if (kreirajZbirViseKola(takmicenje))
                            session.Transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showMessage(ex.Message, this.Text);
                return;
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
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

            refreshUI(takmicenje, true);
        }

        private void refreshUI(Takmicenje takmicenje, bool newTakmicenje)
        {
            mnKategorijeITakmicenja.Enabled = !takmicenje.ZbirViseKola;
            mnPropozicije.Enabled = !takmicenje.ZbirViseKola;
            mnTakmicariKategorije.Enabled = true;
            mnTakmicariTakmicenja.Enabled = true;
            mnEkipe.Enabled = true;
            mnSudijeNaTakmicenju.Enabled = true;
            mnTakmicenje1.Visible = true;

            if (newTakmicenje)
            {
                mnTakmicenje2.Visible = false;
                mnTakmicenje3.Visible = false;
                mnTakmicenje4.Visible = false;
                mnKopirajPrethodnoTakmicenje.Enabled = true;
            }
            else
            {

                mnTakmicenje2.Visible = takmicenje.ZavrsenoTak1;
                mnTakmicenje3.Visible = takmicenje.ZavrsenoTak1;
                mnTakmicenje4.Visible = takmicenje.ZavrsenoTak1;
                mnKopirajPrethodnoTakmicenje.Enabled = false;
            }

            mnPrvoDrugoKoloViseboj.Visible = takmicenje.FinaleKupa;
            mnPrvoDrugoKoloSprave.Visible = takmicenje.FinaleKupa;
            mnPrvoDrugoKoloEkipno.Visible = takmicenje.FinaleKupa;
            mnKreirajTakmicenja234.Visible = !takmicenje.FinaleKupa && !takmicenje.ZbirViseKola;
            mnZrebZaFinaleKupa.Visible = takmicenje.FinaleKupa;

            mnTakmicenje1RasporedSudija.Visible = !takmicenje.ZbirViseKola;
            mnTakmicenje1StartListe.Visible = !takmicenje.ZbirViseKola;
            mnTakmicenje1Ocene.Visible = !takmicenje.ZbirViseKola;
            mnTakmicenje1RezultatiSprave.Visible = !takmicenje.ZbirViseKola;

            MakeCaption();
        }

        private void mnOpen_Click(object sender, EventArgs e)
        {
            /*     
            if (!OkToTrash())
                 return;

             OpenFileDialog ofd = new OpenFileDialog();
             ofd.Filter = strFilter;
             ofd.FileName = "*.txt";

             if (ofd.ShowDialog() == DialogResult.OK)
                 LoadFile(ofd.FileName);
             */

            if (!OkToTrash())
                return;
            try
            {
                OtvoriTakmicenjeForm form = new OtvoriTakmicenjeForm(takmicenjeId, false, 0, false);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    onTakmicenjeOpened(form.Takmicenje);
                    loadBrojDecimalaUOpcije(form.Takmicenje);
                    Opcije.Instance.HeaderFooterInitialized = false;
                }
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }
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

        private void onTakmicenjeOpened(Takmicenje takmicenje)
        {
            this.takmicenje = takmicenje;
            takmicenjeId = takmicenje.Id;
            nazivTakmicenja = takmicenje.GimnastikaNaziv;
            gimnastika = takmicenje.Gimnastika;

            refreshUI(takmicenje, false);
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
            }
            catch (BusinessException ex)
            {
                MessageDialogs.showMessage(ex.Message, strProgName);
                return;
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
                return;
            }

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    Takmicenje t = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId.Value);
                    loadBrojDecimalaUOpcije(t);
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
                if (takmicenje.ZbirViseKola)
                {
                    RezultatiUkupnoZbirViseKolaForm form = new RezultatiUkupnoZbirViseKolaForm(takmicenjeId.Value);
                    form.ShowDialog();
                }
                else
                {
                    RezultatiUkupnoForm form = new RezultatiUkupnoForm(takmicenjeId.Value,
                       DeoTakmicenjaKod.Takmicenje1);
                    form.ShowDialog();
                }
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

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<RezultatskoTakmicenje> rezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                        .FindByTakmicenjeFetch_Tak1_PoredakUkupno_KlubDrzava(takmicenjeId.Value);
                    if (rezTakmicenja.Count == 0)
                        throw new BusinessException("Morate najpre da unesete takmicarske kategorije.");
                    foreach (RezultatskoTakmicenje tak in rezTakmicenja)
                    {
                        NHibernateUtil.Initialize(tak.Takmicenje1.PoredakUkupno.Rezultati);

                        // potrebno u Poredak.create
                        NHibernateUtil.Initialize(tak.Propozicije);
                    }

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

                        DAOFactoryFactory.DAOFactory.GetTakmicenje1DAO().Update(rt.Takmicenje1);
                        if (rt.Propozicije.PostojiTak2)
                            DAOFactoryFactory.DAOFactory.GetTakmicenje2DAO().Update(rt.Takmicenje2);
                        if (rt.Propozicije.PostojiTak3)
                            DAOFactoryFactory.DAOFactory.GetTakmicenje3DAO().Update(rt.Takmicenje3);
                        //if (rt.Propozicije.PostojiTak4)
                        //  DAOFactoryFactory.DAOFactory.GetTakmicenje4DAO().Update(rt.Takmicenje4);

                        TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
                        Takmicenje t = takmicenjeDAO.FindById(takmicenjeId.Value);
                        t.ZavrsenoTak1 = true;
                        takmicenjeDAO.Update(t);

                        mnTakmicenje2.Visible = true;
                        mnTakmicenje3.Visible = true;
                        mnTakmicenje4.Visible = true;
                    }

                    session.Transaction.Commit();
                }
            }
            catch (BusinessException ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showMessage(ex.Message, strProgName);
            }
            catch (InfrastructureException ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, strProgName);
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, strProgName);
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private IList<Ocena> loadOceneTak1(int takmicenjeId)
        {
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
                    ocenaDAO.Session = session;
                    return ocenaDAO.FindOceneByDeoTakmicenja(takmicenjeId, DeoTakmicenjaKod.Takmicenje1);
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
                if (takmicenje.ZbirViseKola)
                {
                    RezultatiEkipeZbirViseKolaForm form = new RezultatiEkipeZbirViseKolaForm(takmicenjeId.Value);
                    form.ShowDialog();
                }
                else
                {
                    RezultatiEkipeForm form = new RezultatiEkipeForm(takmicenjeId.Value,
                        DeoTakmicenjaKod.Takmicenje1);
                    form.ShowDialog();
                }
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
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    cloneTakmicenje(takmicenje, form.Takmicenje, form.SelDescriptions, form.SelKategorije);
                    session.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showMessage(ex.Message, this.Text);
                return;
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }

            mnKopirajPrethodnoTakmicenje.Enabled = false;
        }

        private bool kreirajZbirViseKola(Takmicenje takmicenje)
        {
            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            takmicenjeDAO.Attach(takmicenje, false);
            List<List<TakmicarskaKategorija>> listaKategorija = new List<List<TakmicarskaKategorija>>();
            if (takmicenje.PrvoKolo != null)
            {
                takmicenjeDAO.Attach(takmicenje.PrvoKolo, false);
                listaKategorija.Add(new List<TakmicarskaKategorija>(takmicenje.PrvoKolo.Kategorije));
            }
            if (takmicenje.DrugoKolo != null)
            {
                takmicenjeDAO.Attach(takmicenje.DrugoKolo, false);
                listaKategorija.Add(new List<TakmicarskaKategorija>(takmicenje.DrugoKolo.Kategorije));
            }
            if (takmicenje.TreceKolo != null)
            {
                takmicenjeDAO.Attach(takmicenje.TreceKolo, false);
                listaKategorija.Add(new List<TakmicarskaKategorija>(takmicenje.TreceKolo.Kategorije));
            }
            if (takmicenje.CetvrtoKolo != null)
            {
                takmicenjeDAO.Attach(takmicenje.CetvrtoKolo, false);
                listaKategorija.Add(new List<TakmicarskaKategorija>(takmicenje.CetvrtoKolo.Kategorije));
            }

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(TakmicarskaKategorija))["RedBroj"];
            for (int i = 0; i < listaKategorija.Count; ++i)
            {
                listaKategorija[i].Sort(new SortComparer<TakmicarskaKategorija>(
                    propDesc, ListSortDirection.Ascending));
            }

            for (int i = 0; i < listaKategorija[0].Count; ++i)
            {
                TakmicarskaKategorija kat1 = listaKategorija[0][i];
                for (int j = 0; j < listaKategorija.Count; ++j)
                {
                    TakmicarskaKategorija kat2 = listaKategorija[j][i];
                    if (!kat1.Equals(kat2))
                    {
                        // TODO3: Ovde bi ustvari trebalo proveravati da li se rezultatska takmicenja iz
                        // prethodna cetiri kola poklapaju, i ako se ne poklapaju da se otvori prozor
                        // gde ce korisnik moci da upari odgovarajuca rezultatska takmicenja (i da izabere
                        // koja rez. takmicenja zeli da ukljuci u novo takmicenje). Slicno i za finale kupa.
                        MessageBox.Show("Kategorije iz prethodnih kola se ne poklapaju", "Bilten");
                        return false;
                    }
                }
            }
            foreach (TakmicarskaKategorija k in listaKategorija[0])
            {
                takmicenje.addKategorija(new TakmicarskaKategorija(k.Naziv, takmicenje.Gimnastika));
            }

            // prvi description je uvek kao naziv takmicenja.
            RezultatskoTakmicenjeDescription desc = new RezultatskoTakmicenjeDescription();
            desc.Naziv = takmicenje.Naziv;
            desc.Propozicije = new Propozicije();
            takmicenje.addTakmicenjeDescription(desc);

            IList<RezultatskoTakmicenje> rezTakmicenja = new List<RezultatskoTakmicenje>();
            foreach (RezultatskoTakmicenjeDescription d in takmicenje.TakmicenjeDescriptions)
            {
                foreach (TakmicarskaKategorija k in takmicenje.Kategorije)
                {
                    RezultatskoTakmicenje rt = new RezultatskoTakmicenje(takmicenje,
                        k, d, new Propozicije());
                    rt.Propozicije.PostojiTak2 = true;  // hack
                    rt.Propozicije.PostojiTak4 = true;  // hack
                    rt.ImaEkipnoTakmicenje = true;  // hack
                    rt.KombinovanoEkipnoTak = false;  // hack
                    rezTakmicenja.Add(rt);
                }
            }

            IDictionary<GimnasticarUcesnik, GimnasticarUcesnik> gimnasticariMap =
                new Dictionary<GimnasticarUcesnik, GimnasticarUcesnik>();
            List<Takmicenje> prethodnaKola = new List<Takmicenje>();
            if (takmicenje.PrvoKolo != null)
                prethodnaKola.Add(takmicenje.PrvoKolo);
            if (takmicenje.DrugoKolo != null)
                prethodnaKola.Add(takmicenje.DrugoKolo);
            if (takmicenje.TreceKolo != null)
                prethodnaKola.Add(takmicenje.TreceKolo);
            if (takmicenje.CetvrtoKolo != null)
                prethodnaKola.Add(takmicenje.CetvrtoKolo);
            foreach (TakmicarskaKategorija kat in takmicenje.Kategorije)
            {
                foreach (Takmicenje prethKolo in prethodnaKola)
                {
                    foreach (GimnasticarUcesnik g in loadGimnasticari(prethKolo, kat.Naziv))
                    {
                        if (!gimnasticariMap.ContainsKey(g))
                        {
                            GimnasticarUcesnik g2 = createGimnasticarUcesnik(g, kat);
                            gimnasticariMap.Add(g2, g2);
                        }
                    }
                }
            }

            List<List<RezultatskoTakmicenje>> rezTakmicenjaPrethodnaKola = new List<List<RezultatskoTakmicenje>>();
            foreach (Takmicenje prethKolo in prethodnaKola)
            {
                rezTakmicenjaPrethodnaKola.Add(new List<RezultatskoTakmicenje>(loadRezTakmicenja(prethKolo.Id)));
            }

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                foreach (List<RezultatskoTakmicenje> rezTakmicenjaPrethKolo in rezTakmicenjaPrethodnaKola)
                {
                    RezultatskoTakmicenje rtFrom = findRezTakmicenje(rezTakmicenjaPrethKolo, 
                        rt.TakmicenjeDescription.Naziv, rt.Kategorija);
                    foreach (GimnasticarUcesnik g in rtFrom.Takmicenje1.Gimnasticari)
                    {
                        GimnasticarUcesnik g2 = gimnasticariMap[g];
                        rt.Takmicenje1.addGimnasticar(g2);
                    }
                }
            }

            DrzavaUcesnikDAO drzavaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO();
            KlubUcesnikDAO klubUcesnikDAO = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO();

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                foreach (List<RezultatskoTakmicenje> rezTakmicenjaPrethKolo in rezTakmicenjaPrethodnaKola)
                {
                    RezultatskoTakmicenje rtFrom = findRezTakmicenje(rezTakmicenjaPrethKolo,
                        rt.TakmicenjeDescription.Naziv, rt.Kategorija);
                    foreach (Ekipa e in rtFrom.Takmicenje1.Ekipe)
                    {
                        if (rt.Takmicenje1.Ekipe.Contains(e))
                            continue;

                        Ekipa ekipa = new Ekipa();
                        ekipa.Naziv = e.Naziv;
                        ekipa.Kod = e.Kod;
                        if (e.DrzavaUcesnik == null)
                            ekipa.DrzavaUcesnik = null;
                        else
                        {
                            DrzavaUcesnik drzavaUcesnik
                                = drzavaUcesnikDAO.FindDrzavaUcesnik(takmicenje.Id, e.DrzavaUcesnik.Naziv);
                            if (drzavaUcesnik == null)
                            {
                                drzavaUcesnik = new DrzavaUcesnik();
                                drzavaUcesnik.Naziv = e.DrzavaUcesnik.Naziv;
                                drzavaUcesnik.Kod = e.DrzavaUcesnik.Kod;
                                drzavaUcesnik.Takmicenje = takmicenje;
                                drzavaUcesnikDAO.Add(drzavaUcesnik);
                            }
                            ekipa.DrzavaUcesnik = drzavaUcesnik;
                        }
                        if (e.KlubUcesnik == null)
                            ekipa.KlubUcesnik = null;
                        else
                        {
                            KlubUcesnik klubUcesnik = klubUcesnikDAO.FindKlubUcesnik(takmicenje.Id, e.KlubUcesnik.Naziv);
                            if (klubUcesnik == null)
                            {
                                klubUcesnik = new KlubUcesnik();
                                klubUcesnik.Naziv = e.KlubUcesnik.Naziv;
                                klubUcesnik.Kod = e.KlubUcesnik.Kod;
                                klubUcesnik.Takmicenje = takmicenje;
                                klubUcesnikDAO.Add(klubUcesnik);
                            }
                            ekipa.KlubUcesnik = klubUcesnik;
                        }

                        rt.Takmicenje1.addEkipa(ekipa);
                    }
                }
            }

            takmicenjeDAO.Add(takmicenje);

            RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();
            GimnasticarUcesnikDAO gimnasticarUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                rezultatskoTakmicenjeDAO.Add(rt);
                foreach (Ekipa e in rt.Takmicenje1.Ekipe)
                    ekipaDAO.Add(e);
            }
            foreach (GimnasticarUcesnik g in gimnasticariMap.Values)
            {
                gimnasticarUcesnikDAO.Add(g);
            }
            
            return true;
        }

        void cloneTakmicenje(Takmicenje takmicenje, Takmicenje from, List<RezultatskoTakmicenjeDescription> descriptionsFrom,
            List<TakmicarskaKategorija> kategorijeFrom)
        {
            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            RezultatskoTakmicenjeDescriptionDAO rezTakDescDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDescriptionDAO();

            takmicenjeDAO.Attach(takmicenje, false);
            takmicenjeDAO.Attach(from, false);
            foreach (RezultatskoTakmicenjeDescription d in descriptionsFrom)
            {
                rezTakDescDAO.Attach(d, false);
            }
            
            // TODO3: Ovaj metod bi trebalo updateovati svaki put kada se promene neka svojstva koja se kloniraju.

            takmicenje.BrojEOcena = from.BrojEOcena;
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
                ISet<TakmicarskaKategorija> katFromSet = new HashSet<TakmicarskaKategorija>(kategorijeFrom);
                ISet<RezultatskoTakmicenjeDescription> descFromSet
                    = new HashSet<RezultatskoTakmicenjeDescription>(descriptionsFrom);
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
                    DAOFactoryFactory.DAOFactory.GetTakmicenje2DAO().Delete(rt.Takmicenje2);
                if (deletedTak3)
                    DAOFactoryFactory.DAOFactory.GetTakmicenje3DAO().Delete(rt.Takmicenje3);
                if (deletedTak4)
                    DAOFactoryFactory.DAOFactory.GetTakmicenje4DAO().Delete(rt.Takmicenje4);*/
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

            DrzavaUcesnikDAO drzavaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO();
            KlubUcesnikDAO klubUcesnikDAO = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO();
            RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();
            GimnasticarUcesnikDAO gimnasticarUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();

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
                        DrzavaUcesnik drzavaUcesnik = drzavaUcesnikDAO.FindDrzavaUcesnik(takmicenje.Id, e.DrzavaUcesnik.Naziv);
                        if (drzavaUcesnik == null)
                        {
                            drzavaUcesnik = new DrzavaUcesnik();
                            drzavaUcesnik.Naziv = e.DrzavaUcesnik.Naziv;
                            drzavaUcesnik.Kod = e.DrzavaUcesnik.Kod;
                            drzavaUcesnik.Takmicenje = takmicenje;
                            drzavaUcesnikDAO.Add(drzavaUcesnik);
                        }
                        ekipa.DrzavaUcesnik = drzavaUcesnik;
                    }
                    if (e.KlubUcesnik == null)
                        ekipa.KlubUcesnik = null;
                    else
                    {
                        KlubUcesnik klubUcesnik = klubUcesnikDAO.FindKlubUcesnik(takmicenje.Id, e.KlubUcesnik.Naziv);
                        if (klubUcesnik == null)
                        {
                            klubUcesnik = new KlubUcesnik();
                            klubUcesnik.Naziv = e.KlubUcesnik.Naziv;
                            klubUcesnik.Kod = e.KlubUcesnik.Kod;
                            klubUcesnik.Takmicenje = takmicenje;
                            klubUcesnikDAO.Add(klubUcesnik);
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

            takmicenjeDAO.Add(takmicenje);
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                rezultatskoTakmicenjeDAO.Add(rt);
                foreach (Ekipa e in rt.Takmicenje1.Ekipe)
                    ekipaDAO.Add(e);
            }
            foreach (GimnasticarUcesnik g in gimnasticariMap.Values)
            {
                gimnasticarUcesnikDAO.Add(g);
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
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO
                        = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
                    rezultatskoTakmicenjeDAO.Session = session;
                    return rezultatskoTakmicenjeDAO.FindByTakmicenjeFetch_Tak1_Ekipe_Gimnasticari(takmicenjeId);
                }
            }
            catch (BusinessException)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw;
            }
            catch (InfrastructureException)
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
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    GimnasticarUcesnikDAO gimUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
                    gimUcesnikDAO.Session = session;
                    return gimUcesnikDAO.FindByTakmicenjeKatFetch_Klub_Drzava(tak, kategorija);
                }
            }
            catch (BusinessException)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw;
            }
            catch (InfrastructureException)
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
                DrzavaUcesnik drzavaUcesnik = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO()
                    .FindDrzavaUcesnik(kategorija.Takmicenje.Id, g.DrzavaUcesnik.Naziv);
                if (drzavaUcesnik == null)
                {
                    drzavaUcesnik = new DrzavaUcesnik();
                    drzavaUcesnik.Naziv = g.DrzavaUcesnik.Naziv;
                    drzavaUcesnik.Kod = g.DrzavaUcesnik.Kod;
                    drzavaUcesnik.Takmicenje = kategorija.Takmicenje;
                    DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO().Add(drzavaUcesnik);
                }
                result.DrzavaUcesnik = drzavaUcesnik;
            }
            if (g.KlubUcesnik == null)
                result.KlubUcesnik = null;
            else
            {
                KlubUcesnik klubUcesnik = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO()
                    .FindKlubUcesnik(kategorija.Takmicenje.Id, g.KlubUcesnik.Naziv);
                if (klubUcesnik == null)
                {
                    klubUcesnik = new KlubUcesnik();
                    klubUcesnik.Naziv = g.KlubUcesnik.Naziv;
                    klubUcesnik.Kod = g.KlubUcesnik.Kod;
                    klubUcesnik.Takmicenje = kategorija.Takmicenje;
                    DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO().Add(klubUcesnik);
                }
                result.KlubUcesnik = klubUcesnik;
            }
            return result;
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

        private void mnTakmicenje3RasporedSudija_Click(object sender, EventArgs e)
        {
            try
            {
                RasporedSudijaForm form = new RasporedSudijaForm(takmicenjeId.Value,
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

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            NHibernateHelper.Instance.SessionFactory.Close();
        }

    }
}
