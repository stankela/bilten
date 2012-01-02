using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Bilten.UI
{
    public partial class TakmicariKategorijeUserControl : UserControl
    {
        public TakmicariKategorijeUserControl()
        {
            InitializeComponent();
            GridColumnsInitializer.initGimnasticarUcesnik(DataGridViewUserControl);
        }

        public DataGridViewUserControl DataGridViewUserControl
        {
            get { return dataGridViewUserControl1; }
        }
    }
}
