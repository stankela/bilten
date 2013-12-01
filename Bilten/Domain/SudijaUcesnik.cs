using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class SudijaUcesnik : DomainObject
    {
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

        private SudijskaUloga ulogaUGlavnomSudijskomOdboru;
        public virtual SudijskaUloga UlogaUGlavnomSudijskomOdboru
        {
            get { return ulogaUGlavnomSudijskomOdboru; }
            set { ulogaUGlavnomSudijskomOdboru = value; }
        }

        private Takmicenje takmicenje;
        public virtual Takmicenje Takmicenje
        {
            get { return takmicenje; }
            set { takmicenje = value; }
        }

        private bool nastupaZaDrzavu;
        public virtual bool NastupaZaDrzavu
        {
            get { return nastupaZaDrzavu; }
            set { nastupaZaDrzavu = value; }
        }

        public SudijaUcesnik()
        { 
        
        }

        public override string ToString()
        {
            return Ime + ' ' + Prezime;
        }

        public virtual string PrezimeIme
        {
            get { return Prezime + ' ' + Ime; }
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

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is SudijaUcesnik)) return false;
            SudijaUcesnik that = (SudijaUcesnik)other;
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
