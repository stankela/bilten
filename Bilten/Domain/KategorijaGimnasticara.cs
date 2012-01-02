using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class KategorijaGimnasticara : DomainObject
    {
        private static readonly int NAZIV_MAX_LENGTH = 32;

        private string naziv;
        public virtual string Naziv
        {
            get { return naziv; }
            set { naziv = value; }
        }

        private Gimnastika gimnastika;
        public virtual Gimnastika Gimnastika
        {
            get { return gimnastika; }
            set { gimnastika = value; }
        }

        public KategorijaGimnasticara()
        { 
        
        }

        public override string ToString()
        {
            string result = Naziv;
            if (Gimnastika == Gimnastika.MSG)
                result += " (M)";
            else
                result += " (Z)";
            return result;
        }

        public override void validate(Notification notification)
        {
            // validate Naziv
            if (string.IsNullOrEmpty(Naziv))
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv kategorije je obavezan.");
            }
            else if (Naziv.Length > NAZIV_MAX_LENGTH)
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv kategorije moze da sadrzi maksimalno "
                    + NAZIV_MAX_LENGTH + " znakova.");
            }

            if (Gimnastika == Gimnastika.Undefined)
            {
                notification.RegisterMessage(
                    "Gimnastika", "Gimnastika je obavezna.");
            }
        }

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is KategorijaGimnasticara)) return false;
            KategorijaGimnasticara that = (KategorijaGimnasticara)other;
            return (this.Gimnastika == that.Gimnastika)
                && this.Naziv.ToUpper() == that.Naziv.ToUpper();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 14;
                result = 29 * result + Gimnastika.GetHashCode();
                result = 29 * result + Naziv.GetHashCode();
                return result;
            }
        }
    }
}
