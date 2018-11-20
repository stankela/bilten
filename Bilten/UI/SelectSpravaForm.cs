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
    public partial class SelectSpravaForm : Form
    {
        private Sprava sprava;
        public Sprava Sprava
        {
            get { return sprava; }
        }

        public SelectSpravaForm(Sprava[] sprave, Sprava sprava)
        {
            InitializeComponent();
            Text = "Sprava";

            cmbSprava.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (Sprava s in sprave)
            {
                cmbSprava.Items.Add(Sprave.toString(s));
            }
            if (sprava == Sprava.Undefined)
                cmbSprava.SelectedIndex = 0;
            else
            {
                List<Sprava> sprave2 = new List<Sprava>(sprave);
                int selectedIndex = sprave2.IndexOf(sprava);
                if (selectedIndex < 0)
                {
                    // Ovo se desava kada je sprava pauza, a moze da se bira samo izmedju sprava koje nisu pauze.
                    // Tada selektujem prvu spravu.
                    selectedIndex = 0;
                }
                cmbSprava.SelectedIndex = selectedIndex;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            sprava = Sprave.parse(cmbSprava.SelectedItem.ToString());
        }
    }
}