using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Bilten.UI
{
    public class GridColumnHeaderMouseClickEventArgs : EventArgs
    {
        private readonly DataGridViewCellMouseEventArgs e;

        public GridColumnHeaderMouseClickEventArgs(DataGridViewCellMouseEventArgs e)
        {
            this.e = e;
        }

        public DataGridViewCellMouseEventArgs DataGridViewCellMouseEventArgs
        {
            get { return e; }
        }
  }
}
