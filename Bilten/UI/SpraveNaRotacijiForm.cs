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
        private List<Sprava> aktivneSpraveRot1;

        private List<List<Sprava>> aktivneSprave;
        public List<List<Sprava>> AktivneSprave
        {
            get { return aktivneSprave; }
        }

        public SpraveNaRotacijiForm(Gimnastika gimnastika, List<Sprava> aktivneSpraveRot1)
        {
            InitializeComponent();

            this.gimnastika = gimnastika;
            this.aktivneSpraveRot1 = aktivneSpraveRot1;

            // Inicijalizuj gornji deo interfejsa.
            if (gimnastika == Gimnastika.MSG)
            {
                rbtCetiriSprave.Enabled = rbtCetiriSprave.Visible = false;
                rbtSestSprava.Location = rbtCetiriSprave.Location;
            }
            else
            {
                rbtTriSprave.Enabled = rbtTriSprave.Visible = false;
                rbtSestSprava.Enabled = rbtSestSprava.Visible = false;
                rbtCetiriSprave.Location = rbtTriSprave.Location;
            }

            // Inicijalizuj donji deo interfejsa.
            if (gimnastika == Gimnastika.MSG)
            {
                panel7.Visible = panel7.Enabled = false;
                panel8.Visible = panel8.Enabled = false;
                panel9.Visible = panel9.Enabled = false;
                panel10.Visible = panel10.Enabled = false;
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
            }
            
            setupRotacije();

            if (gimnastika == Gimnastika.MSG)
            {
                selektujSprave(aktivneSpraveRot1, panel1);
            }
            else
            {
                selektujSprave(aktivneSpraveRot1, panel7);
            }
        }

        private void setupRotacije()
        {
            if (gimnastika == Gimnastika.MSG)
            {
                if (getBrojSprava() == 1)
                {
                    panel1.Enabled = true;
                    panel2.Enabled = true;
                    panel3.Enabled = true;
                    panel4.Enabled = true;
                    panel5.Enabled = true;
                    panel6.Enabled = true;

                    label1.Visible = true;
                    label2.Visible = true;
                    label3.Visible = true;
                    label4.Visible = true;
                    label5.Visible = true;
                    label6.Visible = true;

                    label1.Text = "1. Rotacija";
                    label2.Text = "2. Rotacija";
                    label3.Text = "3. Rotacija";
                    label4.Text = "4. Rotacija";
                    label5.Text = "5. Rotacija";
                    label6.Text = "6. Rotacija";
                }
                if (getBrojSprava() == 2)
                {
                    panel1.Enabled = true;
                    panel2.Enabled = true;
                    panel3.Enabled = true;
                    panel4.Enabled = false;
                    panel5.Enabled = false;
                    panel6.Enabled = false;

                    label1.Visible = true;
                    label2.Visible = true;
                    label3.Visible = true;
                    label4.Visible = false;
                    label5.Visible = false;
                    label6.Visible = false;

                    label1.Text = "1. i 2. Rotacija";
                    label2.Text = "3. i 4. Rotacija";
                    label3.Text = "5. i 6. Rotacija";
                }
                else if (getBrojSprava() == 3)
                {
                    panel1.Enabled = true;
                    panel2.Enabled = true;
                    panel3.Enabled = false;
                    panel4.Enabled = false;
                    panel5.Enabled = false;
                    panel6.Enabled = false;

                    label1.Visible = true;
                    label2.Visible = true;
                    label3.Visible = false;
                    label4.Visible = false;
                    label5.Visible = false;
                    label6.Visible = false;

                    label1.Text = "1. 2. i 3. Rotacija";
                    label2.Text = "4. 5. i 6. Rotacija";
                }
                else if (getBrojSprava() == 6)
                {
                    panel1.Enabled = false;
                    panel2.Enabled = false;
                    panel3.Enabled = false;
                    panel4.Enabled = false;
                    panel5.Enabled = false;
                    panel6.Enabled = false;

                    label1.Visible = false;
                    label2.Visible = false;
                    label3.Visible = false;
                    label4.Visible = false;
                    label5.Visible = false;
                    label6.Visible = false;
                }
            }
            else
            {
                if (getBrojSprava() == 1)
                {
                    panel7.Enabled = true;
                    panel8.Enabled = true;
                    panel9.Enabled = true;
                    panel10.Enabled = true;

                    label1.Visible = true;
                    label2.Visible = true;
                    label3.Visible = true;
                    label4.Visible = true;

                    label1.Text = "1. Rotacija";
                    label2.Text = "2. Rotacija";
                    label3.Text = "3. Rotacija";
                    label4.Text = "4. Rotacija";
                }
                else if (getBrojSprava() == 2)
                {
                    panel7.Enabled = true;
                    panel8.Enabled = true;
                    panel9.Enabled = false;
                    panel10.Enabled = false;

                    label1.Visible = true;
                    label2.Visible = true;
                    label3.Visible = false;
                    label4.Visible = false;

                    label1.Text = "1. i 2. Rotacija";
                    label2.Text = "3. i 4. Rotacija";
                }
                else if (getBrojSprava() == 4)
                {
                    panel7.Enabled = false;
                    panel8.Enabled = false;
                    panel9.Enabled = false;
                    panel10.Enabled = false;

                    label1.Visible = false;
                    label2.Visible = false;
                    label3.Visible = false;
                    label4.Visible = false;
                }
            }
        }

        private void selektujSprave(List<Sprava> aktivneSpraveRot1, Panel panel)
        {
            if (gimnastika == Gimnastika.MSG)
            {
                if (aktivneSpraveRot1.Count < 2)
                    rbtJednaSprava.Checked = true;
                else if (aktivneSpraveRot1.Count == 2)
                    rbtDveSprave.Checked = true;
                else if (aktivneSpraveRot1.Count == 3)
                    rbtTriSprave.Checked = true;
                else
                    rbtSestSprava.Checked = true;
            }
            else
            {
                if (aktivneSpraveRot1.Count < 2)
                    rbtJednaSprava.Checked = true;
                else if (aktivneSpraveRot1.Count == 2)
                    rbtDveSprave.Checked = true;
                else
                    rbtCetiriSprave.Checked = true;
            }


            foreach (CheckBox ckb in getCheckBoxes(panel))
            {
                if (aktivneSpraveRot1.IndexOf(getSprava(ckb)) != -1)
                {
                    ckb.Checked = true;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!rbtJednaSprava.Checked && !rbtDveSprave.Checked && !rbtTriSprave.Checked
              && !rbtCetiriSprave.Checked && !rbtSestSprava.Checked)
            {
                MessageDialogs.showMessage("Izaberite broj sprava koje se rotiraju.", this.Text);
                DialogResult = DialogResult.None;
                return;
            }

            if (getBrojSprava() < aktivneSpraveRot1.Count)
            {
                string msg = "Niste izabrali dovoljan broj sprava koje se rotiraju.\n\n " +
                    "Broj sprava koje se rotiraju ne sme biti " +
                    "manji od broja sprava za koje postoje start liste na rotaciji 1.";
                MessageDialogs.showMessage(msg, this.Text);
                DialogResult = DialogResult.None;
                return;
            }

            foreach (Panel p in getAktivniPaneli())
            {
                if (getIzabraneSprave(p).Count != getBrojSprava())
                {
                    string msg;
                    if (getBrojSprava() != 1)
                        msg = String.Format("Izaberite {0} sprave po rotaciji.", getBrojSprava());
                    else
                        msg = String.Format("Izaberite jednu spravu po rotaciji.");
                    MessageDialogs.showMessage(msg, this.Text);
                    DialogResult = DialogResult.None;
                    return;
                }
            }

            if ((gimnastika == Gimnastika.MSG && getBrojSprava() != 6)
                || gimnastika == Gimnastika.ZSG && getBrojSprava() != 4)
            {
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
            }

            this.aktivneSprave = new List<List<Sprava>>();
            if (gimnastika == Gimnastika.MSG)
            {
                if (getBrojSprava() == 1)
                {
                    this.aktivneSprave.Add(getIzabraneSprave(panel1));
                    this.aktivneSprave.Add(getIzabraneSprave(panel2));
                    this.aktivneSprave.Add(getIzabraneSprave(panel3));
                    this.aktivneSprave.Add(getIzabraneSprave(panel4));
                    this.aktivneSprave.Add(getIzabraneSprave(panel5));
                    this.aktivneSprave.Add(getIzabraneSprave(panel6));
                }
                else if (getBrojSprava() == 2)
                {
                    this.aktivneSprave.Add(getIzabraneSprave(panel1));
                    this.aktivneSprave.Add(getIzabraneSprave(panel1));
                    this.aktivneSprave.Add(getIzabraneSprave(panel2));
                    this.aktivneSprave.Add(getIzabraneSprave(panel2));
                    this.aktivneSprave.Add(getIzabraneSprave(panel3));
                    this.aktivneSprave.Add(getIzabraneSprave(panel3));
                }
                else if (getBrojSprava() == 3)
                {
                    this.aktivneSprave.Add(getIzabraneSprave(panel1));
                    this.aktivneSprave.Add(getIzabraneSprave(panel1));
                    this.aktivneSprave.Add(getIzabraneSprave(panel1));
                    this.aktivneSprave.Add(getIzabraneSprave(panel2));
                    this.aktivneSprave.Add(getIzabraneSprave(panel2));
                    this.aktivneSprave.Add(getIzabraneSprave(panel2));
                }
                else if (getBrojSprava() == 6)
                {
                    this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                    this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                    this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                    this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                    this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                    this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                }
            }
            else
            {
                if (getBrojSprava() == 1)
                {
                    this.aktivneSprave.Add(getIzabraneSprave(panel7));
                    this.aktivneSprave.Add(getIzabraneSprave(panel8));
                    this.aktivneSprave.Add(getIzabraneSprave(panel9));
                    this.aktivneSprave.Add(getIzabraneSprave(panel10));
                }
                else if (getBrojSprava() == 2)
                {
                    this.aktivneSprave.Add(getIzabraneSprave(panel7));
                    this.aktivneSprave.Add(getIzabraneSprave(panel7));
                    this.aktivneSprave.Add(getIzabraneSprave(panel8));
                    this.aktivneSprave.Add(getIzabraneSprave(panel8));
                }
                else if (getBrojSprava() == 4)
                {
                    this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                    this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                    this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                    this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                }
            }

            // Proveri da li su sve sprave koje su aktivne u rotaciji 1 medju izabranim spravama za rotaciju 1.
            foreach (Sprava s in aktivneSpraveRot1)
            {
                if (this.aktivneSprave[0].IndexOf(s) == -1)
                {
                    string msg = String.Format("Greska - na rotaciji 1 postoji start lista za spravu {0} koju " +
                        "niste izabrali na rotaciji 1.", Sprave.toString(s));
                    MessageDialogs.showMessage(msg, "Greska");
                    this.aktivneSprave.Clear();
                    DialogResult = DialogResult.None;
                    return;
                }
            }
        }

        private int getBrojSprava()
        {
            int result = 0;
            if (rbtJednaSprava.Checked)
                result = 1;
            else if (rbtDveSprave.Checked)
                result = 2;
            else if (rbtTriSprave.Checked)
                result = 3;
            else if (rbtCetiriSprave.Checked)
                result = 4;
            else if (rbtSestSprava.Checked)
                result = 6;
            return result;
        }
        
        private List<CheckBox> getCheckBoxes(Panel p)
        {
            List<CheckBox> result = new List<CheckBox>();
            foreach (Control c in p.Controls)
            {
                CheckBox ckb = c as CheckBox;
                if (ckb != null)
                {
                    result.Add(ckb);
                }
            }
            return result;
        }

        private List<Sprava> getIzabraneSprave(Panel p)
        {
            List<Sprava> izabraneSprave = new List<Sprava>();
            foreach (CheckBox ckb in getCheckBoxes(p))
            {
                if (ckb.Checked)
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
                if (getBrojSprava() == 1)
                {
                    return new Panel[] { panel1, panel2, panel3, panel4, panel5, panel6 };
                }
                else if (getBrojSprava() == 2)
                {
                    return new Panel[] { panel1, panel2, panel3 };
                }
                else if (getBrojSprava() == 3)
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
                if (getBrojSprava() == 1)
                {
                    return new Panel[] { panel7, panel8, panel9, panel10 };
                }
                else if (getBrojSprava() == 2)
                {
                    return new Panel[] { panel7, panel8 };
                }
                else
                {
                    return new Panel[] { };
                }
            }
        }

        private void rbtJednaSprava_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtJednaSprava.Checked)
                setupRotacije();
        }

        private void rbtDveSprave_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtDveSprave.Checked)
                setupRotacije();
        }

        private void rbtTriSprave_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtTriSprave.Checked)
                setupRotacije();
        }

        private void rbtCetiriSprave_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtCetiriSprave.Checked)
                setupRotacije();
        }

        private void rbtSestSprava_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtSestSprava.Checked)
                setupRotacije();
        }
    }
}