using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;
using System.Windows.Forms;

namespace Bilten.UI
{
    public class SpravaGridMouseUpEventArgs : EventArgs
    {
        private readonly Sprava sprava;
        private readonly MouseEventArgs e;

        public SpravaGridMouseUpEventArgs(Sprava sprava,
            MouseEventArgs e)
        {
            this.sprava = sprava;
            this.e = e;
        }

        public Sprava Sprava
        {
            get { return sprava; }
        }

        public MouseEventArgs MouseEventArgs
        {
            get { return e; }
        }
    }
}
