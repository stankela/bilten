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

        public SelectSpravaForm(Gimnastika gimnastika, Sprava sprava)
        {
            InitializeComponent();
            Text = "Sprava";

            cmbSprava.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSprava.Items.AddRange(Sprave.getSpraveNazivi(gimnastika));
            if (sprava == Sprava.Undefined || Sprave.isPraznaSprava(sprava))
                cmbSprava.SelectedIndex = 0;
            else
                cmbSprava.SelectedIndex = Sprave.indexOf(sprava, gimnastika);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            sprava = Sprave.parse(cmbSprava.SelectedItem.ToString());
        }
    }
}