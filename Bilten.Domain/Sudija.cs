using Bilten.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class Sudija : DomainObject
    {
        private static readonly int IME_MAX_LENGTH = 32;
        private static readonly int PREZIME_MAX_LENGTH = 32;

        private string ime;
        public virtual string Ime
        {
            get { return ime; }
            set { ime = value; }
        }

        private string prezime;
        public virtual string Prezime
        {
            get { return prezime; }
            set { prezime = value; }
        }

        private Pol pol;
        public virtual Pol Pol
        {
            get { return pol; }
            set { pol = value; }
        }

        private Klub klub;
        public virtual Klub Klub
        {
            get { return klub; }
            set { klub = value; }
        }

        private Drzava drzava;
        public virtual Drzava Drzava
        {
            get { return drzava; }
            set { drzava = value; }
        }

        public Sudija()
        { 
        
        }

        public override string ToString()
        {
            return Ime + ' ' + Prezime;
        }

        public override void validate(Notification notification)
        {
            // validate Ime
            if (string.IsNullOrEmpty(Ime))
            {
                notification.RegisterMessage(
                    "Ime", "Ime sudije je obavezno.");
            }
            else if (Ime.Length > IME_MAX_LENGTH)
            {
                notification.RegisterMessage(
                    "Ime", "Ime sudije moze da sadrzi maksimalno "
                    + IME_MAX_LENGTH + " znakova.");
            }

            // validate Prezime
            if (string.IsNullOrEmpty(Prezime))
            {
                notification.RegisterMessage(
                    "Prezime", "Prezime sudije je obavezno.");
            }
            else if (Prezime.Length > PREZIME_MAX_LENGTH)
            {
                notification.RegisterMessage(
                    "Prezime", "Prezime sudije moze da sadrzi maksimalno "
                    + PREZIME_MAX_LENGTH + " znakova.");
            }

            // validate Pol
            if (Pol != Pol.Muski && Pol != Pol.Zenski)
            {
                notification.RegisterMessage(
                    "Pol", "Neispravna vrednost za pol.");
            }

            // validate Drzava
            if (Drzava == null)
            {
                notification.RegisterMessage(
                    "Drzava", "Drzava je obavezna.");
            }
        }

        // NOTE: Metodi Equals i GetHashCode su mi trebali prilikom parsiranja
        // fajla sa rasporedom sudija (sudije sam stavljao u ISet, a on
        // zahteva ove metode)
        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is Sudija)) return false;
            Sudija that = (Sudija)other;
            return this.Ime.ToUpper() == that.Ime.ToUpper()
                && this.Prezime.ToUpper() == that.Prezime.ToUpper();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 14;
                result = 29 * result + Ime.GetHashCode();
                result = 29 * result + Prezime.GetHashCode();
                return result;
            }
        }
    }
}
