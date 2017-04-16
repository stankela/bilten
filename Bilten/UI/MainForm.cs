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
using Bilten.Util;
using Bilten.Misc;
using System.IO;
using Bilten.Services;

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

        private StatusBar statusBar;
        
        public MainForm()
        {
            InitializeComponent();

            statusBar = new StatusBar();
            statusBar.Parent = this;
            statusBar.ShowPanels = true;
            StatusBarPanel sbPanel1 = new StatusBarPanel();
            sbPanel1.Width = statusBar.Width - 100;
            statusBar.Panels.Add(sbPanel1);

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

            DialogResult result = DialogResult.None;
            TakmicenjeForm form = null;
            try
            {
                form = new TakmicenjeForm();
                result = form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageDialogs.showError(ex.Message, strProgName);
            }

            if (result != DialogResult.OK)
                return;

            Takmicenje t = (Takmicenje)form.Entity;
            if (t.StandardnoTakmicenje && form.copyFromTakmicenje == null)
            {
                onTakmicenjeCreated(t);
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
                    if (t.StandardnoTakmicenje && form.copyFromTakmicenje != null)
                    {
                        TakmicenjeService.createFromPrevTakmicenje(t, form.copyFromTakmicenje, form.rezTakmicenja,
                            form.rezTakToGimnasticarMap);
                        session.Transaction.Commit();
                        onTakmicenjeCreated(t);
                    }
                    else if (t.FinaleKupa)
                    {
                        TakmicenjeService.kreirajNaOsnovuViseKola(t);
                        session.Transaction.Commit();
                        string msg = "Takmicenje je kreirano, sa svim ocenama. Podesite u propozicijama " + 
                            "nacin kako se racuna finalna ocena na osnovu ocena iz 1. i 2. kola.";
                        MessageDialogs.showMessage(msg, strProgName);
                        onTakmicenjeCreated(t);
                    }
                    else // ZbirViseKola
                    {
                        TakmicenjeService.kreirajNaOsnovuViseKola(t);
                        session.Transaction.Commit();
                        onTakmicenjeCreated(t);
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

        private void onTakmicenjeCreated(Takmicenje takmicenje)
        {
            this.takmicenje = takmicenje;
            takmicenjeId = takmicenje.Id;
            nazivTakmicenja = takmicenje.GimnastikaNaziv;
            gimnastika = takmicenje.Gimnastika;

            Sesija.Instance.onTakmicenjeChanged(takmicenje.Id);
            refreshUI(takmicenje, true);

            loadBrojDecimalaUOpcije(takmicenje);
            Opcije.Instance.HeaderFooterInitialized = false;
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
            }
            else
            {

                mnTakmicenje2.Visible = takmicenje.ZavrsenoTak1;
                mnTakmicenje3.Visible = takmicenje.ZavrsenoTak1;
                mnTakmicenje4.Visible = takmicenje.ZavrsenoTak1;
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

            Sesija.Instance.onTakmicenjeChanged(takmicenje.Id);
            refreshUI(takmicenje, false);

            loadBrojDecimalaUOpcije(takmicenje);
            Opcije.Instance.HeaderFooterInitialized = false;
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
            return takmicenjeId != null ? takmicenje.ToString() : "";
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
                form.ShowDialog();
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
                       DeoTakmicenjaKod.Takmicenje1, -1, false);
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
                RezultatiUkupnoForm form = new RezultatiUkupnoForm(takmicenjeId.Value,
                    DeoTakmicenjaKod.Takmicenje2, -1, false);
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

        // TODO4: Kod racunanja preskoka za viseboj uvedi da se bira da li se racuna prvi preskok ili bolji.
        // TODO4: Uvedi da moze da se bira koje sprave ulaze u ekipni rezultat.
        // TODO4: Kod stampanja, kolonu sa nazivom ekipa stampaj u dva reda ako je naziv dugacak
        // TODO4: Na pocetku damp fajla neka se nalazi broj verzije programa. Uvozi samo ako se verzije poklapaju.
        // TODO4: Kod stampanja rezultata za ekipe promeni da cela ekipa uvek bude na istoj strani

        private void mnKreirajTakmicenja234_Click(object sender, EventArgs e)
        {
            // Ova komanda moze da se ponovljeno izvrsava (kada se klikne drugi put, prethodno dejstvo ce biti ponisteno
            // i ponovo ce se kreirati ucesnici takmicenja 2, 3 i 4. Ocene u takmicenjima 2, 3 i 4 nece biti izbrisane.

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
                        .FindByTakmicenjeFetch_Tak1_PoredakSprava(takmicenjeId.Value);
                    if (rezTakmicenja.Count == 0)
                        throw new BusinessException("Morate najpre da unesete takmicarske kategorije.");

                    bool postojiOdvojeno = false;
                    foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                    {
                        if (rt.odvojenoTak2() || rt.odvojenoTak3() || rt.odvojenoTak4())
                        {
                            postojiOdvojeno = true;
                            break;
                        }
                    }
                    if (!postojiOdvojeno)
                        throw new BusinessException("Ne postoji odvojeno takmicenje II, III ili IV ni za jednu kategoriju.");

                    TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
                    Takmicenje t = takmicenjeDAO.FindById(takmicenjeId.Value);
                    t.ZavrsenoTak1 = true;

                    foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                    {
                        // Poredak za takmicenje 1 je mozda rucno promenjen, pa ga ne treba ponovo kreirati.

                        if (rt.odvojenoTak2())
                        {
                            rt.Takmicenje2.createUcesnici(rt.Takmicenje1);
                            rt.Takmicenje2.Poredak.initRezultati(rt.Takmicenje2.getUcesniciGimKvalifikanti());
                        }
                        if (rt.odvojenoTak3())
                        {
                            rt.Takmicenje3.createUcesnici(rt.Takmicenje1, rt.Propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka);
                            foreach (PoredakSprava p in rt.Takmicenje3.Poredak)
                                p.initRezultati(rt.Takmicenje3.getUcesniciGimKvalifikanti(p.Sprava));
                            rt.Takmicenje3.PoredakPreskok.initRezultati(
                                rt.Takmicenje3.getUcesniciGimKvalifikanti(Sprava.Preskok), rt);
                        }
                        if (rt.odvojenoTak4())
                        {
                            // TODO: Proveri zasto je ovo i ono dole zakomentarisano
                            //rt.Takmicenje4.createUcesnici(rt.Takmicenje1);
                            //rt.Takmicenje4.Poredak.initRezultati(rt.Takmicenje4.getUcesnici());
                        }

                        if (rt.odvojenoTak2())
                            DAOFactoryFactory.DAOFactory.GetTakmicenje2DAO().Update(rt.Takmicenje2);
                        if (rt.odvojenoTak3())
                            DAOFactoryFactory.DAOFactory.GetTakmicenje3DAO().Update(rt.Takmicenje3);
                        //if (rt.odvojenoTak4())
                        //  DAOFactoryFactory.DAOFactory.GetTakmicenje4DAO().Update(rt.Takmicenje4);

                        mnTakmicenje2.Visible = true;
                        mnTakmicenje3.Visible = true;
                        mnTakmicenje4.Visible = true;
                    }
                    takmicenjeDAO.Update(t);

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

        private void mnTakmicenje1RezultatiSprave_Click(object sender, EventArgs e)
        {
            try
            {
                RezultatiSpravaForm form = new RezultatiSpravaForm(takmicenjeId.Value,
                    DeoTakmicenjaKod.Takmicenje1, -1, Sprava.Undefined, false, false);
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
                    DeoTakmicenjaKod.Takmicenje3, -1, Sprava.Undefined, false, false);
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
            // TODO: Prebaci ovo na neko drugo mesto (ApplicationExit ili nesto slicno ako postoji)
            Sesija.Instance.EndSession();

            NHibernateHelper.Instance.SessionFactory.Close();
        }

        private void mnIzveziTakmicenje_Click(object sender, EventArgs e)
        {
            OtvoriTakmicenjeForm form = null;
            DialogResult result;
            try
            {
                form = new OtvoriTakmicenjeForm(null, true, 1, false);
                result = form.ShowDialog();
            }
            catch (InfrastructureException ex)
            {
                MessageDialogs.showError(ex.Message, this.Text);
                return;
            }

            if (result != DialogResult.OK)
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

                    string fileName = "BILTEN.TAKMICENJE";
                    TakmicenjeDump takDump = new TakmicenjeDump();
                    takDump.dumpToFile(form.SelTakmicenja[0].Id, fileName);
                    MessageDialogs.showMessage(
                        "Takmicenje je izvezeno u fajl '" + fileName + "' u direktorijumu za bilten.", strProgName);
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showMessage(ex.Message, strProgName);
                return;
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void mnUveziTakmicenje_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    //CurrentSessionContext.Bind(session);

                    // NOTE: Izgleda da CurrentSessionContext ne radi kada se otvara vise prozora. Zato koristim globalnu
                    // promenljivu Sesija.Instance.Session.
                    Sesija.Instance.Session = session;
                    Takmicenje t;
                    if (uveziTakmicenje(ofd.FileName, out t))
                    {
                        session.Transaction.Commit();
                        MessageDialogs.showMessage("Takmicenje '" + t.ToString() + "' je uspesno uvezeno.", strProgName);
                    }
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showMessage(ex.Message, strProgName);
                return;
            }
            finally
            {
                Sesija.Instance.Session = null;
                //CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private bool uveziTakmicenje(string fileName, out Takmicenje t)
        {
            TakmicenjeDump takDump = new TakmicenjeDump();
            takDump.loadFromFile(fileName);

            t = takDump.takmicenje;
            if (!DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().existsTakmicenje(t.Naziv, t.Gimnastika, t.Datum))
            {
                // Uvezi takmicenje pod postojecim imenom
                Cursor.Current = Cursors.WaitCursor;
                Cursor.Show();
                try
                {
                    TakmicenjeService.addTakmicenje(t, takDump.klubovi, takDump.drzave, takDump.gimnasticari,
                        takDump.rezTakmicenja, takDump.sudije, takDump.rasporediSudija, takDump.rasporediNastupa,
                        takDump.ocene);
                }
                finally
                {
                    Cursor.Hide();
                    Cursor.Current = Cursors.Arrow;
                }
                return true;
            }

            string header = String.Format("Takmicenje '{0}' vec postoji", t.ToString());
            SelectOptionForm form = new SelectOptionForm(
                header, new string[] { "Prebrisi postojece takmicenje", "Uvezi takmicenje pod novim imenom" },
                strProgName);
            if (form.ShowDialog() != DialogResult.OK)
                return false;

            if (form.SelectedOption == 1)
            {
                // Prebrisi postojece takmicenje
                Cursor.Current = Cursors.WaitCursor;
                Cursor.Show();
                try
                {
                    if (TakmicenjeService.deleteTakmicenje(t.Naziv, t.Gimnastika, t.Datum))
                    {
                        TakmicenjeService.addTakmicenje(t, takDump.klubovi, takDump.drzave, takDump.gimnasticari,
                            takDump.rezTakmicenja, takDump.sudije, takDump.rasporediSudija, takDump.rasporediNastupa,
                            takDump.ocene);
                        return true;
                    }
                    else
                    {
                        // concurrency error
                        throw new Exception("Neuspesno uvozenje takmicenja");
                    }
                }
                finally
                {
                    Cursor.Hide();
                    Cursor.Current = Cursors.Arrow;
                }
            }
            else
            {
                // Uvezi takmicenje pod novim imenom
                TakmicenjeForm takForm = new TakmicenjeForm(t.Naziv, t.Gimnastika, t.Datum, t.Mesto, t.TipTakmicenja);
                if (takForm.ShowDialog() != DialogResult.OK)
                    return false;

                t.Naziv = (takForm.Entity as Takmicenje).Naziv;
                t.Datum = (takForm.Entity as Takmicenje).Datum;
                t.Mesto = (takForm.Entity as Takmicenje).Mesto;

                Cursor.Current = Cursors.WaitCursor;
                Cursor.Show();
                try
                {
                    TakmicenjeService.addTakmicenje(t, takDump.klubovi, takDump.drzave, takDump.gimnasticari,
                        takDump.rezTakmicenja, takDump.sudije, takDump.rasporediSudija, takDump.rasporediNastupa,
                        takDump.ocene);
                }
                finally
                {
                    Cursor.Hide();
                    Cursor.Current = Cursors.Arrow;
                }
                return true;
            }
        }
    }
}
