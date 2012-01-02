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

        public SelectSpravaForm(Pol pol, Sprava sprava)
        {
            InitializeComponent();
            Text = "Sprava";

            cmbSprava.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSprava.Items.AddRange(Sprave.getSpraveNazivi(pol));
            if (sprava == Sprava.Undefined)
                cmbSprava.SelectedIndex = 0;
            else
                cmbSprava.SelectedIndex = Sprave.indexOf(sprava, pol);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            sprava = Sprave.parse(cmbSprava.SelectedItem.ToString());
        }
    }
}