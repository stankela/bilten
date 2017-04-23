using Bilten.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bilten.UI
{
    public partial class PenalizacijaForm : Form
    {
        private Takmicenje takmicenje;

        private string penalizacija;
        public String Penalizacija
        {
            get { return penalizacija; }
        }

        // TODO: Sledeca dva methoda postoje na jos jednom mestu. Prebaci ih u neku Util klasu.

        private bool isFloat(string s)
        {
            // NOTE: NumberStyles.Float sprecava situaciju da se umesto zareza unese
            // tacka (koja bi se tumacila kao celobrojni separator za grupe)
            NumberStyles numStyles = NumberStyles.Float & ~NumberStyles.AllowExponent;

            float dummy;
            return float.TryParse(s, numStyles, null, out dummy);
        }

        private bool checkDecimalPlaces(string s, int brojDecimala)
        {
            string decSeparator =
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string[] parts = null;
            if (s.IndexOf(decSeparator) != -1)
                parts = s.Split(new string[1] { decSeparator }, StringSplitOptions.None);

            if (parts != null)
                return parts[1].Trim().Length <= brojDecimala;
            else
                return true;
        }

        public PenalizacijaForm(float? penalty, Takmicenje takmicenje)
        {
            InitializeComponent();
            this.takmicenje = takmicenje;

            txtPenalizacija.Text = String.Empty;
            if (penalty != null)
                txtPenalizacija.Text = penalty.Value.ToString("F" + takmicenje.BrojDecimalaPen);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtPenalizacija.Text.Trim() != String.Empty)
            {
                string msg = String.Empty;
                if (!isFloat(txtPenalizacija.Text))
                {
                    msg = "Neispravan format za penalizaciju.";
                }
                else if (!checkDecimalPlaces(txtPenalizacija.Text, takmicenje.BrojDecimalaPen))
                {
                    msg = String.Format(
                        "Penalizacija moze da sadrzi najvise {0} decimala.", takmicenje.BrojDecimalaPen);
                }
                if (msg != String.Empty)
                {
                    MessageDialogs.showMessage(msg, this.Text);
                    txtPenalizacija.Focus();
                    this.DialogResult = DialogResult.None;
                }
            }
            penalizacija = txtPenalizacija.Text.Trim();
        }
    }
}
