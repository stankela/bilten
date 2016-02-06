using Bilten.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class Gimnasticar : DomainObject
    {
        private static readonly int IME_MAX_LENGTH = 32;
        private static readonly int SR_IME_MAX_LENGTH = 32;
        private static readonly int PREZIME_MAX_LENGTH = 32;
        
        private string ime;
        public virtual string Ime
        {
            get { return ime; }
            set { ime = value; }
        }

        private string srednjeIme;
        public virtual string SrednjeIme
        {
            get { return srednjeIme; }
            set { srednjeIme = value; }
        }

        private string prezime;
        public virtual string Prezime
        {
            get { return prezime; }
            set { prezime = value; }
        }

        private Datum datumRodjenja;
        public virtual Datum DatumRodjenja
        {
            get { return datumRodjenja; }
            set { datumRodjenja = value; }
        }

        private Gimnastika gimnastika;
        public virtual Gimnastika Gimnastika
        {
            get { return gimnastika; }
            set { gimnastika = value; }
        }

        private KategorijaGimnasticara kategorija;
        public virtual KategorijaGimnasticara Kategorija
        {
            get { return kategorija; }
            set { kategorija = value; }
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

        private RegistarskiBroj registarskiBroj;
        public virtual RegistarskiBroj RegistarskiBroj
        {
            get { return registarskiBroj; }
            set { registarskiBroj = value; }
        }

        private Datum datumPoslednjeRegistracije;
        public virtual Datum DatumPoslednjeRegistracije
        {
            get { return datumPoslednjeRegistracije; }
            set { datumPoslednjeRegistracije = value; }
        }

        public Gimnasticar()
        { 
        
        }

        public virtual string ImeSrednjeIme
        {
            get
            {
                string result = Ime;
                if (!String.IsNullOrEmpty(SrednjeIme))
                    result += ' ' + SrednjeIme;
                return result;
            }
        }

        public virtual string ImeSrednjeImePrezime
        {
            get
            {
                return ImeSrednjeIme + ' ' + Prezime;
            }
        }

        public override string ToString()
        {
            return ImeSrednjeImePrezime;
        } 

        public override void validate(Notification notification)
        {
            // validate Ime
            if (string.IsNullOrEmpty(Ime))
            {
                notification.RegisterMessage(
                    "Ime", "Ime gimnasticara je obavezno.");
            }
            else if (Ime.Length > IME_MAX_LENGTH)
            {
                notification.RegisterMessage(
                    "Ime", "Ime gimnasticara moze da sadrzi maksimalno "
                    + IME_MAX_LENGTH + " znakova.");
            }

            if (SrednjeIme == String.Empty)
            {
                notification.RegisterMessage(
                    "SrednjeIme", "Srednje ime gimnasticara ne sme da bude prazno.");
            }

            if (!string.IsNullOrEmpty(SrednjeIme)
            && SrednjeIme.Length > SR_IME_MAX_LENGTH)
            {
                notification.RegisterMessage(
                    "SrednjeIme", "Srednje ime gimnasticara moze da sadrzi maksimalno "
                    + SR_IME_MAX_LENGTH + " znakova.");
            }

            // validate Prezime
            if (string.IsNullOrEmpty(Prezime))
            {
                notification.RegisterMessage(
                    "Prezime", "Prezime gimnasticara je obavezno.");
            }
            else if (Prezime.Length > PREZIME_MAX_LENGTH)
            {
                notification.RegisterMessage(
                    "Prezime", "Prezime gimnasticara moze da sadrzi maksimalno "
                    + PREZIME_MAX_LENGTH + " znakova.");
            }

            if (Gimnastika == Gimnastika.Undefined)
            {
                notification.RegisterMessage(
                    "Gimnastika", "Gimnastika je obavezna.");
            }

            // validate Drzava
            if (Drzava == null)
            {
                notification.RegisterMessage(
                    "Drzava", "Drzava je obavezna.");
            }
        }

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is Gimnasticar)) return false;
            
            Gimnasticar that = (Gimnasticar)other;
            bool result = this.Ime.ToUpper() == that.Ime.ToUpper()
                && this.Prezime.ToUpper() == that.Prezime.ToUpper();
            if (result)
            { 
                result = string.IsNullOrEmpty(this.SrednjeIme)
                    && string.IsNullOrEmpty(that.SrednjeIme)
                || (!string.IsNullOrEmpty(this.SrednjeIme)
                    && !string.IsNullOrEmpty(that.SrednjeIme)
                    && this.SrednjeIme.ToUpper() == that.SrednjeIme.ToUpper());
            }
            if (result)
            {
                result = this.DatumRodjenja == that.DatumRodjenja;
            }
            return result;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 14;
                result = 29 * result + Ime.GetHashCode();
                result = 29 * result + Prezime.GetHashCode();
                if (!string.IsNullOrEmpty(SrednjeIme))
                    result = 29 * result + SrednjeIme.GetHashCode();
                return result;
            }
        }
    }
}
