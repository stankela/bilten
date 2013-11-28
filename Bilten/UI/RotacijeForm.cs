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
    public partial class RotacijeForm : Form
    {
        private Gimnastika gimnastika;

        private bool rotirajEkipeRotirajGimnasticare;
        public bool RotirajEkipeRotirajGimnasticare
        {
            get { return rotirajEkipeRotirajGimnasticare; }
        }

        private bool neRotirajEkipeRotirajGimnasticare;
        public bool NeRotirajEkipeRotirajGimnasticare
        {
            get { return neRotirajEkipeRotirajGimnasticare; }
        }

        private bool rotirajSveGimnasticare;
        public bool RotirajSveGimnasticare
        {
            get { return rotirajSveGimnasticare; }
        }

        private bool neRotirajNista;
        public bool NeRotirajNista
        {
            get { return neRotirajNista; }
        }

        private List<List<Sprava>> aktivneSprave;
        public List<List<Sprava>> AktivneSprave
        {
            get { return aktivneSprave; }
        }

        public RotacijeForm(Gimnastika gimnastika)
        {
            InitializeComponent();

            this.gimnastika = gimnastika;
            rbtRotirajEkipeRotirajGim.Checked = true;
            if (gimnastika == Gimnastika.MSG)
            {
                rbtCetiriSprave.Enabled = false;
                rbtCetiriSprave.Visible = false;
                rbtSestSprava.Location = rbtCetiriSprave.Location;
            }
            else
            {
                rbtTriSprave.Enabled = false;
                rbtTriSprave.Visible = false;
                rbtSestSprava.Enabled = false;
                rbtSestSprava.Visible = false;
                rbtCetiriSprave.Location = rbtTriSprave.Location;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!rbtRotirajEkipeRotirajGim.Checked && !rbtNerotirajEkipeRotGim.Checked
                && !rbtRotirajSve.Checked && !rbtNerotirajNista.Checked)
            {
                MessageDialogs.showMessage("Izaberite nacin rotacije.", this.Text);
                DialogResult = DialogResult.None;
                return;
            }
            if (!rbtJednaSprava.Checked && !rbtDveSprave.Checked && !rbtTriSprave.Checked
                && !rbtCetiriSprave.Checked && !rbtSestSprava.Checked)
            {
                MessageDialogs.showMessage("Izaberite broj sprava koje se rotiraju.", this.Text);
                DialogResult = DialogResult.None;
                return;
            }

            this.rotirajEkipeRotirajGimnasticare = rbtRotirajEkipeRotirajGim.Checked;
            this.neRotirajEkipeRotirajGimnasticare = rbtNerotirajEkipeRotGim.Checked;
            this.rotirajSveGimnasticare = rbtRotirajSve.Checked;
            this.neRotirajNista = rbtNerotirajNista.Checked;

            if (gimnastika == Gimnastika.MSG && rbtSestSprava.Checked)
            {
                this.aktivneSprave = new List<List<Sprava>>();
                this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
            }
            else if (gimnastika == Gimnastika.ZSG && rbtCetiriSprave.Checked)
            {
                this.aktivneSprave = new List<List<Sprava>>();
                this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
                this.aktivneSprave.Add(new List<Sprava>(Sprave.getSprave(gimnastika)));
            }
            else
            {
                SpraveNaRotacijiForm form = new SpraveNaRotacijiForm(gimnastika, getBrojSprava());
                if (form.ShowDialog() != DialogResult.OK)
                {
                    DialogResult = DialogResult.None;
                    return;
                }
                this.aktivneSprave = form.AktivneSprave;
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
    }
}