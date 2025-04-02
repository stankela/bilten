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
    public partial class PromeniBrojeveForm : Form
    {
        private short broj;
        public short Broj
        {
            get { return broj; }
        }

        private List<short> brojevi;
        public List<short> Brojevi
        {
            get { return brojevi; }
        }

        public PromeniBrojeveForm()
        {
            InitializeComponent();
            this.Text = "Promeni brojeve";
            rbtUzastopniBrojevi.Checked = true;
        }

        private void rbtUzastopniBrojevi_CheckedChanged(object sender, EventArgs e)
        {
            rbtCheckedChanged();
        }

        private void rbtProizvoljniBrojevi_CheckedChanged(object sender, EventArgs e)
        {
            rbtCheckedChanged();
        }

        private void rbtCheckedChanged()
        {
            if (rbtUzastopniBrojevi.Checked)
            {
                txtUzastopniBrojevi.Enabled = true;
                txtProizvoljniBrojevi.Enabled = false;
            }
            else
            {
                txtUzastopniBrojevi.Enabled = false;
                txtProizvoljniBrojevi.Enabled = true;
            }
        }

        public static List<short> parseBrojevi(string brojevi)
        {
            brojevi = brojevi.Trim();
            if (brojevi == String.Empty)
                return null;

            List<string> parts = new List<string>();
            char delimiter = ' ';
            int index = brojevi.IndexOf(delimiter);
            while (index != -1)
            {
                parts.Add(brojevi.Substring(0, index));
                brojevi = brojevi.Substring(index).Trim();
                index = brojevi.IndexOf(delimiter);
            }
            parts.Add(brojevi.Trim());

            List<short> result = new List<short>();
            short value;
            for (int i = 0; i < parts.Count; i++)
            {
                if (!short.TryParse(parts[i], out value))
                    return null;
                result.Add(value);
            }

            for (int i = 0; i < result.Count; i++)
            {
                short number = result[i];
                if (number < 1 || number > 999)
                    return null;
            }
            return result;
        }

        private bool validate()
        {
            if (rbtUzastopniBrojevi.Checked)
            {
                short broj;
                if (txtUzastopniBrojevi.Text.Trim() == String.Empty)
                {
                    MessageDialogs.showMessage("Unesite broj.", this.Text);
                    txtUzastopniBrojevi.Focus();
                    return false;
                }
                else if (!short.TryParse(txtUzastopniBrojevi.Text.Trim(), out broj) || broj < 1 || broj > 999)
                {
                    MessageDialogs.showMessage("Neispravan broj. Broj moze da ima maksimalno 3 cifre.", this.Text);
                    txtUzastopniBrojevi.Focus();
                    return false;
                }
            }
            else
            {
                if (txtProizvoljniBrojevi.Text.Trim() == String.Empty)
                {
                    MessageDialogs.showMessage("Unesite brojeve.", this.Text);
                    txtProizvoljniBrojevi.Focus();
                    return false;
                }
                if (parseBrojevi(txtProizvoljniBrojevi.Text) == null)
                {
                    MessageDialogs.showMessage("Neispravna lista brojeva. Broj moze da ima maksimalno 3 cifre.", this.Text);
                    txtProizvoljniBrojevi.Focus();
                    return false;
                }
            }
            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!validate())
            {
                DialogResult = DialogResult.None;
                return;
            }
            if (rbtUzastopniBrojevi.Checked)
            {
                broj = short.Parse(txtUzastopniBrojevi.Text);
                brojevi = null;
            }
            else
            {
                broj = -1;
                brojevi = parseBrojevi(txtProizvoljniBrojevi.Text);
            }
        }
    }
}
