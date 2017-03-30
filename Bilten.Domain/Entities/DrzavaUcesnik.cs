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

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is DrzavaUcesnik)) return false;

            DrzavaUcesnik that = (DrzavaUcesnik)other;
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

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(Naziv != null ? Naziv : NULL);
            strBuilder.AppendLine(Kod != null ? Kod : NULL);
            strBuilder.AppendLine(Takmicenje != null ? Takmicenje.Id.ToString() : NULL);
        }
    }
}
