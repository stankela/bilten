using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Exceptions;

namespace Bilten.Domain
{
    public class Klub : DomainObject, IComparable<Klub>
    {
        private static readonly int NAZIV_MAX_LENGTH = 128;
        private static readonly int KOD_MAX_LENGTH = 7;
    
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

        private Mesto mesto;
        public virtual Mesto Mesto
        {
            get { return mesto; }
            set { mesto = value; }
        }

        public Klub()
        { 
        
        }

        public override string ToString()
        {
            return Naziv;
        }

        public override void validate(Notification notification)
        {
            // validate Naziv
            if (string.IsNullOrEmpty(Naziv))
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv kluba je obavezan.");
            }
            else if (Naziv.Length > NAZIV_MAX_LENGTH)
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv kluba moze da sadrzi maksimalno "
                    + NAZIV_MAX_LENGTH + " znakova.");
            }
          
            // validate Kod
            if (string.IsNullOrEmpty(Kod))
            {
                notification.RegisterMessage(
                    "Kod", "Kod kluba je obavezan.");
            }
            else if (Kod.Length > KOD_MAX_LENGTH)
            {
                notification.RegisterMessage(
                    "Kod", "Kod kluba moze da sadrzi maksimalno "
                    + KOD_MAX_LENGTH + " znakova.");
            }

            // validate Mesto
            if (Mesto == null)
            {
                notification.RegisterMessage(
                    "Mesto", "Mesto kluba je obavezno.");
            }
        }

        #region IComparable<Klub> Members

        public virtual int CompareTo(Klub other)
        {
            return this.Naziv.CompareTo(other.Naziv);
        }

        #endregion
    }
}
