using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class KlubUcesnik : DomainObject
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

        public KlubUcesnik()
        { 
        
        }

        public override string ToString()
        {
            return Naziv;
        }

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is KlubUcesnik)) return false;

            KlubUcesnik that = (KlubUcesnik)other;
            bool result = this.Naziv.ToUpper() == that.Naziv.ToUpper()
                && this.Kod.ToUpper() == that.Kod.ToUpper();
            return result;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 14;
                result = 29 * result + Naziv.GetHashCode();
                result = 29 * result + Kod.GetHashCode();
                return result;
            }
        }

    }
}
