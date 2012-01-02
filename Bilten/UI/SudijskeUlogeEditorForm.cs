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
        private Takmicenje takmicenje;

        private List<CheckBox> dCheckBoxes = new List<CheckBox>();
        private List<CheckBox> eCheckBoxes = new List<CheckBox>();
        private List<CheckBox> merVremenaCheckBoxes = new List<CheckBox>();
        private List<CheckBox> linSudijeCheckBoxes = new List<CheckBox>();

        public SudijskeUlogeEditorForm(SudijskiOdborNaSpravi odbor, Takmicenje takmicenje)
        {
            InitializeComponent();
            this.odbor = odbor;
            this.takmicenje = takmicenje;

            createUI();
        }

        private void createUI()
        {
            Text = "Sudijske funkcije - " + Sprave.toString(odbor.Sprava);

            List<SudijskaUloga> uloge = new List<SudijskaUloga>(
                SudijskeUloge.getUloge(2, takmicenje.BrojESudija,
                takmicenje.BrojMeracaVremena, takmicenje.BrojLinijskihSudija));

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

        CheckBox createCheckBox(SudijskaUloga u, Point location, int tabIndex, bool check)
        {
            CheckBox result = new CheckBox();
            result.AutoSize = true;
            result.Location = location;
            result.TabIndex = tabIndex;
            result.Text = SudijskeUloge.toString(u);
            result.UseVisualStyleBackColor = true;
            result.Tag = u;
            result.Checked = check;

            if (SudijskeUloge.isDSudija(u))
                dCheckBoxes.Add(result);
            else if (SudijskeUloge.isESudija(u))
                eCheckBoxes.Add(result);
            else if (SudijskeUloge.isMeracVremena(u))
                merVremenaCheckBoxes.Add(result);
            else if (SudijskeUloge.isLinijskiSudija(u))
                linSudijeCheckBoxes.Add(result);

            return result;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!validate())
            {
                DialogResult = DialogResult.None;
                return;
            }

            odbor.setSupportedUloge(getNumChecked(dCheckBoxes), getNumChecked(eCheckBoxes),
                getNumChecked(merVremenaCheckBoxes), getNumChecked(linSudijeCheckBoxes));
        }

        private bool validate()
        {
            if (!validate(dCheckBoxes))
            {
                MessageDialogs.showMessage(
                    "Moguce je selektovati samo uzastopne D sudije.", this.Text);
                return false;
            }
            else if (!validate(eCheckBoxes))
            {
                MessageDialogs.showMessage(
                    "Moguce je selektovati samo uzastopne E sudije.", this.Text);
                return false;
            }
            else if (!validate(merVremenaCheckBoxes))
            {
                MessageDialogs.showMessage(
                    "Moguce je selektovati samo uzastopne merace vremena.", this.Text);
                return false;
            }
            else if (!validate(linSudijeCheckBoxes))
            {
                MessageDialogs.showMessage(
                    "Moguce je selektovati samo uzastopne linijske sudije.", this.Text);
                return false;
            }

            return true;
        }

        private bool validate(List<CheckBox> checkBoxes)
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

        private byte getNumChecked(List<CheckBox> checkBoxes)
        {
            int result = 0;
            foreach (CheckBox ckb in checkBoxes)
            {
                if (ckb.Checked)
                    result++;
            }
            return (byte)result;
        }
    }
}