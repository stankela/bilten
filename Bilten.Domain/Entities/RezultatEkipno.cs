using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatEkipno : RezultatUkupno
    {
        private Ekipa _ekipa;
        public virtual Ekipa Ekipa
        {
            get { return _ekipa; }
            set { _ekipa = value; }
        }

        public virtual string NazivEkipe
        {
            get
            {
                if (Ekipa != null)
                    return Ekipa.Naziv;
                else
                    return String.Empty;
            }
        }

    }
}
