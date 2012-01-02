using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class Drzava : DomainObject, IComparable<Drzava>
    {
        private static readonly int NAZIV_MAX_LENGTH = 32;
        private static readonly int KOD_MAX_LENGTH = 3;

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

        public Drzava()
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
                    "Naziv", "Naziv drzave je obavezan.");
            }
            else if (Naziv.Length > NAZIV_MAX_LENGTH)
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv drzave moze da sadrzi maksimalno "
                    + NAZIV_MAX_LENGTH + " znakova.");
            }

            // validate Kod
            if (string.IsNullOrEmpty(Kod))
            {
                notification.RegisterMessage(
                    "Kod", "Kod drzave je obavezan.");
            }
            else if (Kod.Length > KOD_MAX_LENGTH)
            {
                notification.RegisterMessage(
                    "Kod", "Kod drzave moze da sadrzi maksimalno "
                    + KOD_MAX_LENGTH + " znakova.");
            }
        }

        #region IComparable<Drzava> Members

        public virtual int CompareTo(Drzava other)
        {
            return this.Naziv.CompareTo(other.Naziv);
        }

        #endregion
    }
}
