using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class DrzavaUcesnik : DomainObject
    {
        private string naziv;
        public virtual string Naziv
        {
            get { return naziv; }
            set { naziv = value; }
        }

        private string kod;
        public virtual string Kod
        {
            get { return kod; }
            set { kod = value; }
        }

        private Takmicenje takmicenje;
        public virtual Takmicenje Takmicenje
        {
            get { return takmicenje; }
            set { takmicenje = value; }
        }

        public DrzavaUcesnik()
        { 
        
        }

        public override string ToString()
        {
            return Naziv;
        }
    }
}
