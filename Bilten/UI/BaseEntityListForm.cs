using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Bilten.UI
{
    public partial class BaseEntityListForm : Form
    {
        public BaseEntityListForm()
        {
            InitializeComponent();
        }

        public virtual object Selection
        {
            get { throw new NotSupportedException(); }
        }
    }
}