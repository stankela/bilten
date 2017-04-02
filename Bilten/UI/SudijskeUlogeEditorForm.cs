using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;

namespace Bilten.UI
{
    public partial class SudijskeUlogeEditorForm : Form
    {
        private SudijskiOdborNaSpravi odbor;

        public SudijskeUlogeEditorForm(SudijskiOdborNaSpravi odbor)
        {
            InitializeComponent();
            this.odbor = odbor;

            createUI();
        }

        private void createUI()
        {
            Text = "Sudijske funkcije - " + Sprave.toString(odbor.Sprava);

            List<SudijskaUloga> uloge = new List<SudijskaUloga>(SudijskeUloge.getSveUloge());

            int x = 12;
            int y = 12;
            int tabIndex = 0;
            this.SuspendLayout();
            
            foreach (SudijskaUloga u in uloge)
            {
                this.Controls.Add(createCheckBox(
                    u, new Point(x, y), tabIndex, odbor.hasFunction(u)));
                y += 23;
                tabIndex++;
            }

            y += 12;
            btnOK.Location = new Point(btnOK.Location.X, y);
            btnOK.TabIndex = tabIndex;
            btnCancel.Location = new Point(btnCancel.Location.X, y);
            btnCancel.TabIndex = tabIndex + 1;

            this.ClientSize = new Size(ClientSize.Width, btnOK.Bottom + 23);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        CheckBox createCheckBox(SudijskaUloga uloga, Point location, int tabIndex, bool check)
        {
            CheckBox result = new CheckBox();
            result.AutoSize = true;
            result.Location = location;
            result.TabIndex = tabIndex;
            result.Text = SudijskeUloge.toString(uloga);
            result.UseVisualStyleBackColor = true;
            result.Tag = uloga;
            result.Checked = check;
            result.CheckedChanged += new EventHandler(ckb_CheckedChanged);
            return result;
        }

        void ckb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            if (ckb.Checked)
            {
                SudijskaUloga uloga = (SudijskaUloga)ckb.Tag;
                if (uloga == SudijskaUloga.D1 || uloga == SudijskaUloga.E1)
                {
                    setChecked(SudijskaUloga.D1_E1, false);
                }
                else if (uloga == SudijskaUloga.D2 || uloga == SudijskaUloga.E2)
                {
                    setChecked(SudijskaUloga.D2_E2, false);
                }
                else if (uloga == SudijskaUloga.D1_E1)
                {
                    setChecked(SudijskaUloga.D1, false);
                    setChecked(SudijskaUloga.E1, false);
                }
                else if (uloga == SudijskaUloga.D2_E2)
                {
                    setChecked(SudijskaUloga.D2, false);
                    setChecked(SudijskaUloga.E2, false);
                }
            }
        }

        private CheckBox getCheckBox(SudijskaUloga uloga)
        {
            foreach (Control c in this.Controls)
            {
                CheckBox ckb = c as CheckBox;
                if (ckb != null && ((SudijskaUloga)ckb.Tag == uloga))
                    return ckb;
            }
            return null;
        }

        private bool isChecked(SudijskaUloga uloga)
        {
            CheckBox ckb = getCheckBox(uloga);
            return ckb != null && ckb.Checked;
        }

        private void setChecked(SudijskaUloga uloga, bool value)
        {
            CheckBox ckb = getCheckBox(uloga);
            if (ckb != null)
                ckb.Checked = value;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!validate())
            {
                DialogResult = DialogResult.None;
                return;
            }

            byte brojDSudija = 0;
            if (isChecked(SudijskaUloga.D1) || isChecked(SudijskaUloga.D1_E1))
                brojDSudija += 1;
            if (isChecked(SudijskaUloga.D2) || isChecked(SudijskaUloga.D2_E2))
                brojDSudija += 1;

            byte brojESudija = 0;
            if (isChecked(SudijskaUloga.E1) || isChecked(SudijskaUloga.D1_E1))
                brojESudija += 1;
            if (isChecked(SudijskaUloga.E2) || isChecked(SudijskaUloga.D2_E2))
                brojESudija += 1;
            if (isChecked(SudijskaUloga.E3))
                brojESudija += 1;
            if (isChecked(SudijskaUloga.E4))
                brojESudija += 1;
            if (isChecked(SudijskaUloga.E5))
                brojESudija += 1;
            if (isChecked(SudijskaUloga.E6))
                brojESudija += 1;

            odbor.setSupportedUloge(brojDSudija, isChecked(SudijskaUloga.D1_E1), isChecked(SudijskaUloga.D2_E2), brojESudija);
        }

        private bool validate()
        {
           if (isChecked(SudijskaUloga.D1) && isChecked(SudijskaUloga.D1_E1))
            {
                MessageDialogs.showMessage(
                    "Moguce je selektovati samo jednog D1 sudiju.", this.Text);
                return false;
            }
            if (isChecked(SudijskaUloga.D2) && isChecked(SudijskaUloga.D2_E2))
            {
                MessageDialogs.showMessage(
                    "Moguce je selektovati samo jednog D2 sudiju.", this.Text);
                return false;
            }

            if (isChecked(SudijskaUloga.E1) && isChecked(SudijskaUloga.D1_E1))
            {
                MessageDialogs.showMessage(
                    "Moguce je selektovati samo jednog E1 sudiju.", this.Text);
                return false;
            }
            if (isChecked(SudijskaUloga.E2) && isChecked(SudijskaUloga.D2_E2))
            {
                MessageDialogs.showMessage(
                    "Moguce je selektovati samo jednog E2 sudiju.", this.Text);
                return false;
            }

            // Proveri da li je selektovan D2 a nije selektovan D1
            if (!isChecked(SudijskaUloga.D1) && !isChecked(SudijskaUloga.D1_E1)
                && (isChecked(SudijskaUloga.D2) || isChecked(SudijskaUloga.D2_E2)))
            {
                MessageDialogs.showMessage(
                    "Moguce je selektovati samo uzastopne D sudije.", this.Text);
                return false;
            }

            // Proveri da li je selektovan E2 a nije selektovan E1
            if (!isChecked(SudijskaUloga.E1) && !isChecked(SudijskaUloga.D1_E1)
                && (isChecked(SudijskaUloga.E2) || isChecked(SudijskaUloga.D2_E2)))
            {
                MessageDialogs.showMessage(
                    "Moguce je selektovati samo uzastopne E sudije.", this.Text);
                return false;
            }

            // Proveri da li je selektovan jedan od E3, E4, ..., a nije selektovan E1 ili E2
            if (!isChecked(SudijskaUloga.E1) && !isChecked(SudijskaUloga.D1_E1)
                && !isChecked(SudijskaUloga.E2) && !isChecked(SudijskaUloga.D2_E2)
                && (isChecked(SudijskaUloga.E3) || isChecked(SudijskaUloga.E4)
                    || isChecked(SudijskaUloga.E5) || isChecked(SudijskaUloga.E6)))
            {
                MessageDialogs.showMessage(
                    "Moguce je selektovati samo uzastopne E sudije.", this.Text);
                return false;
            }

            List<CheckBox> eSudije = new List<CheckBox>();
            SudijskaUloga[] eUloge = new SudijskaUloga[] { SudijskaUloga.E3, SudijskaUloga.E4, SudijskaUloga.E5,
                                                           SudijskaUloga.E6 };
            foreach (SudijskaUloga uloga in eUloge)
            {
                if (getCheckBox(uloga) != null)
                    eSudije.Add(getCheckBox(uloga));
            }

            if (!validateUzastopno(eSudije))
            {
                MessageDialogs.showMessage(
                    "Moguce je selektovati samo uzastopne E sudije.", this.Text);
                return false;
            }

            return true;
        }

        private bool validateUzastopno(List<CheckBox> checkBoxes)
        {
            bool shouldNotBeChecked = false;
            foreach (CheckBox ckb in checkBoxes)
            {
                if (ckb.Checked)
                {
                    if (shouldNotBeChecked)
                        return false;
                }
                else
                    shouldNotBeChecked = true;
            }
            return true;
        }
    }
}