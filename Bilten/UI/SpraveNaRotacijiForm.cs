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
    public partial class SpraveNaRotacijiForm : Form
    {
        // TODO: Kada budes prelazio na Visual Studio 2008, proveri delove koda koje sam kopirao iz Dizajnera (npr. gde
        // kreiram kontrole tako sto kopiram kod iz dizajnera). Ovo je potrebno zato sto je moguce da se kod koji dizajner
        // generise promenio.

        private Gimnastika gimnastika;
        private int brojSprava;

        private List<List<Sprava>> aktivneSprave;
        public List<List<Sprava>> AktivneSprave
        {
            get { return aktivneSprave; }
        }

        public SpraveNaRotacijiForm(Gimnastika gimnastika, int brojSprava)
        {
            InitializeComponent();

            this.gimnastika = gimnastika;
            this.brojSprava = brojSprava;

            if (gimnastika == Gimnastika.MSG)
            {
                panel7.Visible = panel7.Enabled = false;
                panel8.Visible = panel8.Enabled = false;
                panel9.Visible = panel9.Enabled = false;
                panel10.Visible = panel10.Enabled = false;

                if (brojSprava == 2)
                {
                    panel4.Visible = panel4.Enabled = false;
                    panel5.Visible = panel5.Enabled = false;
                    panel6.Visible = panel6.Enabled = false;
                    label4.Visible = false;
                    label5.Visible = false;
                    label6.Visible = false;

                    label1.Text = "1. i 2. Rotacija";
                    label2.Text = "3. i 4. Rotacija";
                    label3.Text = "5. i 6. Rotacija";
                }
                else if (brojSprava == 3)
                {
                    panel3.Visible = panel3.Enabled = false;
                    panel4.Visible = panel4.Enabled = false;
                    panel5.Visible = panel5.Enabled = false;
                    panel6.Visible = panel6.Enabled = false;
                    label3.Visible = false;
                    label4.Visible = false;
                    label5.Visible = false;
                    label6.Visible = false;

                    label1.Text = "1. 2. i 3. Rotacija";
                    label2.Text = "4. 5. i 6. Rotacija";
                }
            }
            else
            {
                panel1.Visible = panel1.Enabled = false;
                panel2.Visible = panel2.Enabled = false;
                panel3.Visible = panel3.Enabled = false;
                panel4.Visible = panel4.Enabled = false;
                panel5.Visible = panel5.Enabled = false;
                panel6.Visible = panel6.Enabled = false;
                label5.Visible = false;
                label6.Visible = false;

                panel7.Location = panel1.Location;
                panel8.Location = panel2.Location;
                panel9.Location = panel3.Location;
                panel10.Location = panel4.Location;

                if (brojSprava == 2)
                {
                    panel9.Visible = panel9.Enabled = false;
                    panel10.Visible = panel10.Enabled = false;
                    label3.Visible = false;
                    label4.Visible = false;
                    label5.Visible = false;
                    label6.Visible = false;

                    label1.Text = "1. i 2. Rotacija";
                    label2.Text = "3. i 4. Rotacija";
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            foreach (Panel p in getAktivniPaneli())
            {
                if (getIzabraneSprave(p).Count != brojSprava)
                {
                    string msg;
                    if (brojSprava != 1)
                        msg = String.Format("Izaberite {0} sprave po rotaciji.", brojSprava);
                    else
                        msg = String.Format("Izaberite jednu spravu po rotaciji.");
                    MessageDialogs.showMessage(msg, this.Text);
                    DialogResult = DialogResult.None;
                    return;
                }
            }
            List<Sprava> izabraneSprave = getSveIzabraneSprave();
            foreach (Sprava s in Sprave.getSprave(gimnastika))
            {
                if (izabraneSprave.IndexOf(s) == -1)
                {
                    string msg = String.Format("Niste izabrali sve sprave.");
                    MessageDialogs.showMessage(msg, this.Text);
                    DialogResult = DialogResult.None;
                    return;
                }
            }

            this.aktivneSprave = new List<List<Sprava>>();
            if (gimnastika == Gimnastika.MSG)
            {
                if (brojSprava == 1)
                {
                    this.aktivneSprave.Add(getIzabraneSprave(panel1));
                    this.aktivneSprave.Add(getIzabraneSprave(panel2));
                    this.aktivneSprave.Add(getIzabraneSprave(panel3));
                    this.aktivneSprave.Add(getIzabraneSprave(panel4));
                    this.aktivneSprave.Add(getIzabraneSprave(panel5));
                    this.aktivneSprave.Add(getIzabraneSprave(panel6));
                }
                else if (brojSprava == 2)
                {
                    this.aktivneSprave.Add(getIzabraneSprave(panel1));
                    this.aktivneSprave.Add(getIzabraneSprave(panel1));
                    this.aktivneSprave.Add(getIzabraneSprave(panel2));
                    this.aktivneSprave.Add(getIzabraneSprave(panel2));
                    this.aktivneSprave.Add(getIzabraneSprave(panel3));
                    this.aktivneSprave.Add(getIzabraneSprave(panel3));
                }
                else if (brojSprava == 3)
                {
                    this.aktivneSprave.Add(getIzabraneSprave(panel1));
                    this.aktivneSprave.Add(getIzabraneSprave(panel1));
                    this.aktivneSprave.Add(getIzabraneSprave(panel1));
                    this.aktivneSprave.Add(getIzabraneSprave(panel2));
                    this.aktivneSprave.Add(getIzabraneSprave(panel2));
                    this.aktivneSprave.Add(getIzabraneSprave(panel2));
                }
            }
            else
            {
                if (brojSprava == 1)
                {
                    this.aktivneSprave.Add(getIzabraneSprave(panel7));
                    this.aktivneSprave.Add(getIzabraneSprave(panel8));
                    this.aktivneSprave.Add(getIzabraneSprave(panel9));
                    this.aktivneSprave.Add(getIzabraneSprave(panel10));
                }
                else if (brojSprava == 2)
                {
                    this.aktivneSprave.Add(getIzabraneSprave(panel7));
                    this.aktivneSprave.Add(getIzabraneSprave(panel7));
                    this.aktivneSprave.Add(getIzabraneSprave(panel8));
                    this.aktivneSprave.Add(getIzabraneSprave(panel8));
                }
            }
        }

        private List<Sprava> getIzabraneSprave(Panel p)
        {
            List<Sprava> izabraneSprave = new List<Sprava>();
            foreach (Control c in p.Controls)
            {
                CheckBox ckb = c as CheckBox;
                if (ckb != null && ckb.Checked)
                {
                    izabraneSprave.Add(getSprava(ckb));
                }
            }

            // Vrati sprave po FIGA redosledu.
            List<Sprava> result = new List<Sprava>();
            foreach (Sprava s in Sprave.getSprave(gimnastika))
            {
                if (izabraneSprave.IndexOf(s) != -1)
                    result.Add(s);
            }
            return result;
        }

        private Sprava getSprava(CheckBox ckb)
        {
            Sprava result = Sprave.parse(ckb.Text);
            if (result != Sprava.Undefined)
                return result;
            else
            {
                MessageDialogs.showMessage("Greska u programu: Nepoznata sprava", "Greska");
                DialogResult = DialogResult.Cancel;
                Close();
                return Sprava.Undefined;
            }
        }

        private List<Sprava> getSveIzabraneSprave()
        {
            List<Sprava> result = new List<Sprava>();
            foreach (Panel p in getAktivniPaneli())
            {
                foreach (Sprava s in getIzabraneSprave(p))
                {
                    if (result.IndexOf(s) == -1)
                        result.Add(s);
                }
            }
            return result;
        }

        private Panel[] getAktivniPaneli()
        {
            if (gimnastika == Gimnastika.MSG)
            {
                if (brojSprava == 1)
                {
                    return new Panel[] { panel1, panel2, panel3, panel4, panel5, panel6 };
                }
                else if (brojSprava == 2)
                {
                    return new Panel[] { panel1, panel2, panel3 };
                }
                else if (brojSprava == 3)
                {
                    return new Panel[] { panel1, panel2 };
                }
                else
                {
                    return new Panel[] { };
                }
            }
            else
            {
                if (brojSprava == 1)
                {
                    return new Panel[] { panel7, panel8, panel9, panel10 };
                }
                else if (brojSprava == 2)
                {
                    return new Panel[] { panel7, panel8 };
                }
                else
                {
                    return new Panel[] { };
                }
            }
        }
    }
}