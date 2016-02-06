using Bilten.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class Mesto : DomainObject, IComparable<Mesto>
    {
        private static readonly int NAZIV_MAX_LENGTH = 32;

        private string naziv;
        public virtual string Naziv
        {
            get { return naziv; }
            set { naziv = value; }
        }

        public Mesto()
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
                    "Naziv", "Naziv mesta je obavezan.");
            }
            else if (Naziv.Length > NAZIV_MAX_LENGTH)
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv mesta moze da sadrzi maksimalno "
                    + NAZIV_MAX_LENGTH + " znakova.");
            }
        }

        #region IComparable<Mesto> Members

        public virtual int CompareTo(Mesto other)
        {
            return this.Naziv.CompareTo(other.Naziv);
        }

        #endregion
    }
}
