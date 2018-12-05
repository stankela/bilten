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
    public partial class MaxGimIzKlubaUFinaluForm : Form
    {
        int maxBrojTakmicaraIzKluba;
        public int MaxBrojTakmicaraIzKluba
        {
            get { return maxBrojTakmicaraIzKluba; }
        }

        bool maxBrojTakmicaraVaziZaDrzavu;
        public bool MaxBrojTakmicaraVaziZaDrzavu
        {
            get { return maxBrojTakmicaraVaziZaDrzavu; }
        }

        public MaxGimIzKlubaUFinaluForm()
        {
            InitializeComponent();
            this.Text = "Maksimalan broj takmicara iz istog kluba/drzave";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            byte dummyByte;
            if (!rbtKlub.Checked && !rbtDrzava.Checked)
            {
                MessageDialogs.showMessage("Izaberite klub ili drzavu.", this.Text);
                DialogResult = DialogResult.None;
                return;
            }
            if (!byte.TryParse(txtMaxTak.Text, out dummyByte) || byte.Parse(txtMaxTak.Text) <= 0)
            {
                MessageDialogs.showMessage("Neispravna vrednost za maksimalan broj takmicara iz istog kluba/drzave.",
                    this.Text);
                txtMaxTak.Focus();
                DialogResult = DialogResult.None;
                return;
            }

            maxBrojTakmicaraIzKluba = byte.Parse(txtMaxTak.Text);
            maxBrojTakmicaraVaziZaDrzavu = rbtDrzava.Checked;
        }
    }
}
