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

namespace Bilten.UI
{
    public partial class MainForm : Form
    {
        private Rectangle rectNormal;
        protected string strProgName;
        protected string strFileName;
        private string nazivTakmicenja;
        private Nullable<int> takmicenjeId;

        //string strRegKey = "Software\\Sasa\\";
        const string strWinState = "WindowState";
        const string strLocationX = "LocationX";
        const string strLocationY = "LocationY";
        const string strWidth = "Width";
        const string strHeight = "Height";

        const string strFilter =
                          "Text Documents(*.txt)|*.txt|All Files(*.*)|*.*";

        // TODO: Remember to call ISessionFactory.Close() when you�re done using 
        // NHibernate. You�ll usually do it while closing the application.
        
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
            mnTakmicenje1StartListe.Visible = false;
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
            if (!OkToTrash())
                return;

            try
            {
                TakmicenjeForm form = new TakmicenjeForm(null);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Takmicenje t = (Takmicenje)form.Entity;
                    newTakmicenje(t);
                    loadBrojDecimalaUOpcije(t);
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
            takmicenjeId = takmicenje.Id;
            nazivTakmicenja = takmicenje.GimnastikaNaziv;
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
            MakeCaption();
        }

        private void mnOpen_Click(object sender, EventArgs e)
        {
            if (!OkToTrash())
                return;
            try
            {
                OtvoriTakmicenjeForm form = new OtvoriTakmicenjeForm(takmicenjeId);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    loadTakmicenje(form.Takmicenje);
                    loadBrojDecimalaUOpcije(form.Takmicenje);
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
            nazivTakmicenja = takmicenje.GimnastikaNaziv;
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
                    rt.Takmicenje1.PoredakUkupno.create(rt, ocene);
                    foreach (PoredakSprava p in rt.Takmicenje1.PoredakSprava)
                        p.create(rt, ocene);
                    rt.Takmicenje1.PoredakPreskok.create(rt, ocene);
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

    }
}