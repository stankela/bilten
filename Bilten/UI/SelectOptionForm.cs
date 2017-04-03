using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bilten.UI
{
    public partial class SelectOptionForm : Form
    {
        public int SelectedIndex;

        public SelectOptionForm(string header, string[] options, string caption)
        {
            InitializeComponent();
            this.Text = caption;

            this.SuspendLayout();
            int x = 22;
            Label lblHeader = new Label();
            lblHeader.AutoSize = true;
            lblHeader.Location = new Point(x, 19);
            lblHeader.Size = new Size(35, 13);
            lblHeader.Text = header;
            this.Controls.Add(lblHeader);

            int i = 0;
            foreach (string option in options)
            {
                RadioButton radioButton1 = new RadioButton();
                radioButton1.AutoSize = true;
                radioButton1.Location = new Point(x, 49 + i*28);
                radioButton1.Size = new Size(85, 17);
                radioButton1.TabIndex = i + 1;
                radioButton1.TabStop = true;
                radioButton1.Text = option;
                radioButton1.UseVisualStyleBackColor = true;
                radioButton1.Tag = i + 1;
                this.Controls.Add(radioButton1);
                ++i;
            }

            Button btnOK = new Button();
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Size = new Size(75, 23);
            btnOK.TabIndex = i + 1;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            this.AcceptButton = btnOK;
            this.Controls.Add(btnOK);

            Button btnCancel = new Button();
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = i + 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            this.CancelButton = btnCancel;
            this.Controls.Add(btnCancel);

            this.ResumeLayout(false);
            this.PerformLayout();

            // Ova podesavanja moraju nakon poziva this.PerformLayout(), inace nemaju efekta
            int clientWidth = calculateFormWidth(x);
            btnCancel.Location = new Point(clientWidth - 24 - btnCancel.Size.Width, getOptionBottom() + 32);
            btnOK.Location = new Point(btnCancel.Location.X - 24 - btnOK.Size.Width, getOptionBottom() + 32);
            ClientSize = new Size(clientWidth, btnOK.Location.Y + btnOK.Size.Height + 24);
        }

        private int getOptionBottom()
        {
            int result = 0;
            foreach (Control c in this.Controls)
            {
                if (c is RadioButton)
                {
                    int bottom = c.Location.Y + c.Size.Height;
                    if (bottom > result)
                        result = bottom;
                }
            }
            return result;
        }

        int calculateFormWidth(int x)
        {
            int maxWidth = 0;
            foreach (Control c in this.Controls)
            {
                if ((c is Label || c is RadioButton) && c.Size.Width > maxWidth)
                    maxWidth = c.Size.Width;
            }
            return maxWidth + 2 * x;
        }

        bool isChecked()
        {
            foreach (Control c in this.Controls)
            {
                RadioButton rbt = c as RadioButton;
                if (rbt != null && rbt.Checked)
                    return true;
            }
            return false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!isChecked())
            {
                DialogResult = DialogResult.None;
                return;
            }

            foreach (Control c in this.Controls)
            {
                RadioButton rbt = c as RadioButton;
                if (rbt != null && rbt.Checked)
                {
                    SelectedIndex = (int)rbt.Tag;
                    break;
                }
            }
        }
    }
}
