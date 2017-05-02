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
    public partial class WaitForm : Form
    {
        public string Message
        {
            set { label1.Text = value; }
        }

        public WaitForm()
        {
            InitializeComponent();
        }
    }
}
