using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Domain;

namespace Bilten.UI
{
    public class SpravaGridMouseDownEventArgs : EventArgs
    {
        private readonly Sprava sprava;

        public SpravaGridMouseDownEventArgs(Sprava sprava)
        {
            this.sprava = sprava;
        }

        public Sprava Sprava
        {
            get { return sprava; }
        }
    }
}
