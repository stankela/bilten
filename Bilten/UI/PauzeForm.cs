using Bilten.Domain;
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
    public partial class PauzeForm : Form
    {
        Gimnastika gimnastika;

        int pauzeMask = 0;
        public int PauzeMask
        {
            get { return pauzeMask; }
        }

        public PauzeForm(Gimnastika gimnastika)
        {
            InitializeComponent();
            this.Text = "Pauze u rotaciji";
            this.gimnastika = gimnastika;
        }

        public void init()
        {
            Sprava[] sprave = Sprave.getSprave(gimnastika);
            pictureBox1.Image = SlikeSprava.getImage(sprave[0]);
            pictureBox2.Image = SlikeSprava.getImage(sprave[1]);
            pictureBox3.Image = SlikeSprava.getImage(sprave[2]);
            pictureBox4.Image = SlikeSprava.getImage(sprave[3]);
            if (gimnastika == Gimnastika.MSG)
            {
                pictureBox5.Image = SlikeSprava.getImage(sprave[4]);
                pictureBox6.Image = SlikeSprava.getImage(sprave[5]);
            }
            else
            {
                pictureBox5.Visible = false;
                pictureBox6.Visible = false;
                textBox6.Visible = false;
                textBox7.Visible = false;
            }
        }

        private void rbtNe_CheckedChanged(object sender, EventArgs e)
        {
            rbtnCheckedChanged();
        }

        private void rbtDa_CheckedChanged(object sender, EventArgs e)
        {
            rbtnCheckedChanged();
        }

        private void rbtnCheckedChanged()
        {
            panel1.Enabled = rbtDa.Checked;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!rbtDa.Checked && !rbtNe.Checked)
            {
                MessageDialogs.showMessage("Izaberite da li zelite pauze u rotaciji.", this.Text);
                DialogResult = DialogResult.None;
                return;
            }
            if (rbtNe.Checked)
            {
                pauzeMask = 0;
                return;
            }

            List<TextBox> textBoxes = new List<TextBox>();
            textBoxes.Add(textBox1);
            textBoxes.Add(textBox2);
            textBoxes.Add(textBox3);
            textBoxes.Add(textBox4);
            textBoxes.Add(textBox5);
            if (gimnastika == Gimnastika.MSG)
            {
                textBoxes.Add(textBox6);
                textBoxes.Add(textBox7);
            }

            foreach (TextBox tb in textBoxes)
            {
                int dummyByte;
                if (tb.Text.Trim() != String.Empty && !int.TryParse(tb.Text, out dummyByte))
                {
                    MessageDialogs.showMessage("Neispravan format za broj pauza.", this.Text);
                    tb.Focus();
                    DialogResult = DialogResult.None;
                    return;
                }
            }

            int ukupanBrojPauza = 0;
            foreach (TextBox tb in textBoxes)
            {
                if (tb.Text.Trim() != String.Empty)
                {
                    ukupanBrojPauza += int.Parse(tb.Text);
                }
            }
            if (ukupanBrojPauza == 0)
            {
                MessageDialogs.showMessage("Unesite pauze.", this.Text);
                DialogResult = DialogResult.None;
                return;
            }
            else if (ukupanBrojPauza > 6)
            {
                MessageDialogs.showMessage("Maksimalno dozvoljen broj pauza je 6.", this.Text);
                DialogResult = DialogResult.None;
                return;
            }

            pauzeMask = 0;
            int pauzaIndex = 0;
            for (int i = 0; i < textBoxes.Count; ++i)
            {
                TextBox tb = textBoxes[i];
                if (tb.Text.Trim() != String.Empty)
                {
                    int brojPauza = int.Parse(tb.Text);
                    while (brojPauza-- > 0)
                    {
                        ++pauzaIndex;
                        pauzeMask |= (1 << pauzaIndex);
                    }
                    ++pauzaIndex;  // za spravu
                }
                else
                {
                    ++pauzaIndex;  // za spravu
                }
            }
        }

    }
}
