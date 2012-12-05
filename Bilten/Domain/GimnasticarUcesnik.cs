using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class GimnasticarUcesnik : DomainObject, IEquatable<GimnasticarUcesnik>
    {
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

    /*    public virtual Pol Pol
        {
            get
            {
                if (Gimnastika == Gimnastika.MSG)
                    return Pol.Muski;
                else
                    return Pol.Zenski;
            }
        }*/

        private RegistarskiBroj registarskiBroj;
        public virtual RegistarskiBroj RegistarskiBroj
        {
            get { return registarskiBroj; }
            set { registarskiBroj = value; }
        }

        private Nullable<int> takmicarskiBroj;
        public virtual Nullable<int> TakmicarskiBroj
        {
            get { return takmicarskiBroj; }
            set { takmicarskiBroj = value; }
        }

        private Takmicenje takmicenje;
        public virtual Takmicenje Takmicenje
        {
            get { return takmicenje; }
            set { takmicenje = value; }
        }

        private KlubUcesnik klubUcesnik;
        public virtual KlubUcesnik KlubUcesnik
        {
            get { return klubUcesnik; }
            set { klubUcesnik = value; }
        }

        private DrzavaUcesnik drzavaUcesnik;
        public virtual DrzavaUcesnik DrzavaUcesnik
        {
            get { return drzavaUcesnik; }
            set { drzavaUcesnik = value; }
        }

        private TakmicarskaKategorija takmicarskaKategorija;
        public virtual TakmicarskaKategorija TakmicarskaKategorija
        {
            get { return takmicarskaKategorija; }
            set { takmicarskaKategorija = value; }
        }

        private bool _nastupaZaDrzavu;
        public virtual bool NastupaZaDrzavu
        {
            get { return _nastupaZaDrzavu; }
            set { _nastupaZaDrzavu = value; }
        }

        public GimnasticarUcesnik()
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

        public virtual string PrezimeIme
        {
            get { return Prezime + ' ' + ImeSrednjeIme; }
        }

        public virtual string PrezimeImeDrzava
        {
            get { return PrezimeIme + " (" + KlubDrzava + ")"; }
        }

        public virtual string KlubDrzava
        {
            get
            {
                if (NastupaZaDrzavu)
                {
                    if (DrzavaUcesnik != null)
                        return DrzavaUcesnik.Kod;
                    else if (KlubUcesnik != null)
                        return KlubUcesnik.Naziv;
                    else
                        return String.Empty;
                }
                else
                {
                    if (KlubUcesnik != null)
                        return KlubUcesnik.Naziv;
                    else if (DrzavaUcesnik != null)
                        return DrzavaUcesnik.Kod;
                    else
                        return String.Empty;
                }

            }
        }

        public override void validate(Notification notification)
        {
            // TODO: 
        }

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is GimnasticarUcesnik)) return false;

            GimnasticarUcesnik that = (GimnasticarUcesnik)other;
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

        #region IEquatable<GimnasticarUcesnik> Members

        public virtual bool Equals(GimnasticarUcesnik other)
        {
            return this.Equals((object)other);
        }

        #endregion
    }
}
