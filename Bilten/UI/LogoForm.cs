using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using NHibernate;
using Bilten.Data;
using NHibernate.Context;
using Bilten.Dao;
using Bilten.Exceptions;
using System.IO;

namespace Bilten.UI
{
    public partial class LogoForm : Form
    {
        Takmicenje takmicenje;

        public LogoForm(int takmicenjeId)
        {
            InitializeComponent();

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    this.takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
                    NHibernateUtil.Initialize(takmicenje);
                    initUI();
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
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
            }            
        }

        private void initUI()
        {
            this.Text = "Logo";
            pictureBoxSlika.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxSlika.NoDistort = true;

            listViewLogo.Items.Clear();
            listViewLogo.View = View.Details;
            listViewLogo.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listViewLogo.FullRowSelect = true;
            listViewLogo.MultiSelect = false;
            listViewLogo.Columns.Add("Polozaj");
            listViewLogo.Columns.Add("Slika");
            listViewLogo.Columns[0].TextAlign = HorizontalAlignment.Left;
            listViewLogo.Columns[0].Width = 75;
            listViewLogo.Columns[1].TextAlign = HorizontalAlignment.Left;

            string path1 = String.IsNullOrEmpty(takmicenje.Logo1RelPath) ? "" : takmicenje.Logo1RelPath;
            string path2 = String.IsNullOrEmpty(takmicenje.Logo2RelPath) ? "" : takmicenje.Logo2RelPath;
            string path3 = String.IsNullOrEmpty(takmicenje.Logo3RelPath) ? "" : takmicenje.Logo3RelPath;
            string path4 = String.IsNullOrEmpty(takmicenje.Logo4RelPath) ? "" : takmicenje.Logo4RelPath;
            string path5 = String.IsNullOrEmpty(takmicenje.Logo5RelPath) ? "" : takmicenje.Logo5RelPath;
            string path6 = String.IsNullOrEmpty(takmicenje.Logo6RelPath) ? "" : takmicenje.Logo6RelPath;
            string path7 = String.IsNullOrEmpty(takmicenje.Logo7RelPath) ? "" : takmicenje.Logo7RelPath;

            listViewLogo.Items.Add(new ListViewItem(new string[] { "Heder levo", path1 }));
            listViewLogo.Items.Add(new ListViewItem(new string[] { "Heder desno", path2 }));
            listViewLogo.Items.Add(new ListViewItem(new string[] { "Futer 1", path3 }));
            listViewLogo.Items.Add(new ListViewItem(new string[] { "Futer 2", path4 }));
            listViewLogo.Items.Add(new ListViewItem(new string[] { "Futer 3", path5 }));
            listViewLogo.Items.Add(new ListViewItem(new string[] { "Futer 4", path6 }));
            listViewLogo.Items.Add(new ListViewItem(new string[] { "Futer 5", path7 }));
            listViewLogo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void selectFirstLogo()
        {
            for (int i = 0; i < listViewLogo.Items.Count; ++i)
            {
                ListViewItem item = listViewLogo.Items[i];
                if (!String.IsNullOrEmpty(item.SubItems[1].Text))
                {
                    item.Selected = true;
                    return;
                }
            }
        }

        private string getAppRelativeFileNamePathFromUser()
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.InitialDirectory = Application.ExecutablePath;
            openFileDlg.Filter = "All files (*.*)|*.*";
            openFileDlg.FilterIndex = 1;
            openFileDlg.RestoreDirectory = true;

            DialogResult dlgResult = DialogResult.None;
            while (true)
            {
                dlgResult = openFileDlg.ShowDialog();
                if (dlgResult != DialogResult.OK || isAppRelative(openFileDlg.FileName))
                    break;
                MessageBox.Show("Datoteka mora da se nalazi u nekom od " +
                    "poddirektorijuma glavnog direktorijuma aplikacije.", "Greska");
            }
            if (dlgResult != DialogResult.OK)
                return null;

            string appDir = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            return openFileDlg.FileName.Replace(appDir, "");
        }

        private bool isAppRelative(string fullName)
        {
            string dir = Path.GetDirectoryName(fullName);
            string appDir = Path.GetDirectoryName(Application.ExecutablePath);
            return dir.IndexOf(appDir) != -1;
        }

        private void btnDodajSliku_Click(object sender, EventArgs e)
        {
            ListViewItem selItem = getSelectedItem();
            if (selItem == null)
            {
                MessageDialogs.showMessage("Selektujte polozaj", this.Text);
                return;
            }
            
            string relFileNamePath = getAppRelativeFileNamePathFromUser();
            if (relFileNamePath == null)
                return;

            try
            {
                listViewLogo.Focus();
                pictureBoxSlika.Image = Image.FromFile(relFileNamePath);
                selItem.SubItems[1].Text = relFileNamePath;
                selItem.Selected = true;
            }
            catch (Exception)
            {
                MessageDialogs.showError("Ne mogu da ucitam sliku \"" + relFileNamePath + "\"", "Greska");
            }
        }

        private ListViewItem getSelectedItem()
        {
            if (listViewLogo.SelectedItems.Count == 0)
                return null;
            for (int i = 0; i < listViewLogo.Items.Count; ++i)
            {
                ListViewItem item = listViewLogo.Items[i];
                if (item.Selected)
                    return item;
            }
            return null;
        }

        private void btnBrisiSliku_Click(object sender, EventArgs e)
        {
            ListViewItem selItem = getSelectedItem();
            if (selItem != null)
            {
                listViewLogo.Focus();
                // TODO5: Dodaj proveru da li se zeli brisanje
                selItem.SubItems[1].Text = "";
                pictureBoxSlika.Image = null;
                selItem.Selected = true;
            }
        }

        private void listViewLogo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem selItem = getSelectedItem();
            if (selItem == null)
            {
                pictureBoxSlika.Image = null;
                return;
            }
            string path = selItem.SubItems[1].Text;
            if (!String.IsNullOrEmpty(path))
            {
                try
                {
                    // TODO5: Kesiraj slike, tj nemoj svaki put da ucitavas iz fajla kada se sel index promeni
                    pictureBoxSlika.Image = Image.FromFile(path);
                }
                catch (Exception)
                {
                    Bilten.UI.MessageDialogs.showError("Ne mogu da pronadjem sliku \"" + path + "\"", "Greska");
                    pictureBoxSlika.Image = null;
                }
            }
            else
            {
                pictureBoxSlika.Image = null;
            }
        }

        private bool validate(List<string> paths)
        {
            if (
                (String.IsNullOrEmpty(paths[2]) && (!String.IsNullOrEmpty(paths[3])
                                                    || !String.IsNullOrEmpty(paths[4])
                                                    || !String.IsNullOrEmpty(paths[5])
                                                    || !String.IsNullOrEmpty(paths[6])))
                || (String.IsNullOrEmpty(paths[3]) && (!String.IsNullOrEmpty(paths[4])
                                                       || !String.IsNullOrEmpty(paths[5])
                                                       || !String.IsNullOrEmpty(paths[6])))
                || (String.IsNullOrEmpty(paths[4]) && (!String.IsNullOrEmpty(paths[5])
                                                       || !String.IsNullOrEmpty(paths[6])))
                || (String.IsNullOrEmpty(paths[5]) && !String.IsNullOrEmpty(paths[6]))
             )
            {
                MessageDialogs.showError("Za futer je moguce izabrati samo uzastopne logoe.", this.Text);
                return false;
            }
            return true;
        }

        private void updateTakmicenjeFromUI(Takmicenje takmicenje, List<string> paths)
        {
            takmicenje.Logo1RelPath = String.IsNullOrEmpty(paths[0]) ? "" : paths[0];
            takmicenje.Logo2RelPath = String.IsNullOrEmpty(paths[1]) ? "" : paths[1];
            takmicenje.Logo3RelPath = String.IsNullOrEmpty(paths[2]) ? "" : paths[2];
            takmicenje.Logo4RelPath = String.IsNullOrEmpty(paths[3]) ? "" : paths[3];
            takmicenje.Logo5RelPath = String.IsNullOrEmpty(paths[4]) ? "" : paths[4];
            takmicenje.Logo6RelPath = String.IsNullOrEmpty(paths[5]) ? "" : paths[5];
            takmicenje.Logo7RelPath = String.IsNullOrEmpty(paths[6]) ? "" : paths[6];
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            List<string> paths = new List<string>();
            for (int i = 0; i < listViewLogo.Items.Count; ++i)
            {
                paths.Add(listViewLogo.Items[i].SubItems[1].Text);
            }
            if (!validate(paths))
            {
                DialogResult = DialogResult.None;
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
                    Takmicenje takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(this.takmicenje.Id);
                    updateTakmicenjeFromUI(takmicenje, paths);
                    takmicenje.LastModified = DateTime.Now;
                    DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().Update(takmicenje);
                    session.Transaction.Commit();
                }
            }
            catch (InfrastructureException ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                DialogResult = DialogResult.Cancel;
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                MessageDialogs.showError(ex.Message, this.Text);
                DialogResult = DialogResult.Cancel;
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void LogoForm_Shown(object sender, EventArgs e)
        {
            listViewLogo.Focus();
            selectFirstLogo();
        }
    }
}