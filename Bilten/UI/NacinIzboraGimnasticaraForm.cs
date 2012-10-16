using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Bilten.UI
{
    public partial class NacinIzboraGimnasticaraForm : Form
    {
        bool izPrethodnogTakmicenja = false;
        public bool IzPrethodnogTakmicenja
        {
            get { return izPrethodnogTakmicenja; }
        }

        public NacinIzboraGimnasticaraForm()
        {
            InitializeComponent();
            rbtIzRegistra.Checked = true;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            izPrethodnogTakmicenja = rbtIzTakmicenja.Checked;
        }
    }
}