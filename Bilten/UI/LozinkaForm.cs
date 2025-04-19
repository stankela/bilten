using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Bilten.UI
{
    public partial class LozinkaForm : Form
    {
        private string lozinka;

        private bool adminMode;
        public bool AdminMode
        {
            get { return adminMode; }
        }

        public LozinkaForm(string lozinka, bool usePasswordChar, bool showAdminCheckbox)
        {
            InitializeComponent();
            this.lozinka = lozinka;
            if (usePasswordChar)
            {
                txtLozinka.PasswordChar = '*';
                txtLozinka.UseSystemPasswordChar = true;
            }
            if (showAdminCheckbox)
            {
                ckbIskljuciAdminMode.Checked = false;
            }
            else
            {
                ckbIskljuciAdminMode.Visible = false;
                ckbIskljuciAdminMode.Enabled = false;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtLozinka.Text != lozinka)
            {
                MessageDialogs.showMessage("Neispravna lozinka", this.Text);
                txtLozinka.Clear();
                this.DialogResult = DialogResult.None;
                return;
            }
            adminMode = !ckbIskljuciAdminMode.Checked;
        }
    }
}
