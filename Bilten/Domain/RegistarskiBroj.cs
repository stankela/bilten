using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    public class RegistarskiBroj : IComparable
    {
        private static readonly char DELIMITER = '/';
        
        private int broj;
        public int Broj
        {
            get { return broj; }
            private set { broj = value; }
        }

        private short godinaRegistracije;
        public short GodinaRegistracije
        {
            get { return godinaRegistracije; }
            private set { godinaRegistracije = value; }
        }

        private RegistarskiBroj()
        { 
        
        }

        public RegistarskiBroj(int broj, short godinaRegistracije)
        {
            Datum d;
            if (broj < 1)
                throw new ArgumentException("broj", "broj ne sme da bude negativan.");
            if (!Datum.TryParse(godinaRegistracije.ToString(), out d)
            || d.Godina > DateTime.Today.Year + 1
            || d.Godina <= DateTime.Today.Year - 100 + 1)
            {
                // dodao sam 1 na tekucu godinu kao gornji limit za slucaj da se
                // npr. u decembru kreira reg. broj za sledecu godinu
                throw new ArgumentException("godinaRegistracije", "Nedozvoljena vrednost za godinu registracije.");
            }
            
            this.broj = broj;
            this.godinaRegistracije = godinaRegistracije;
        }

        public static RegistarskiBroj Parse(string s)
        {
            string msg = "Nedozvoljen format za registarski broj.";
            if (!hasTwoDelimitedParts(s))
                throw new FormatException(msg);

            string[] parts = s.Split(DELIMITER);
            int broj = 0;
            try
            {
                broj = Int32.Parse(parts[0].Trim());
            }
            catch (Exception)
            {
                throw new FormatException(msg);
            }

            bool twoDigitsYear = false;
            if (parts[1].Trim().Length == 2)
                twoDigitsYear = true;
            else if (parts[1].Length != 4)
                throw new FormatException(msg);

            short godina = 0;
            try
            {
                godina = Int16.Parse(parts[1].Trim());
                if (twoDigitsYear)
                {
                    // vidi napomenu za + 1 u konstruktoru
                    if (godina <= (DateTime.Today.Year + 1) % 100)
                        godina += 2000;
                    else
                        godina += 1900;
                }
            }
            catch (Exception)
            {
                throw new FormatException(msg);
            }

            try
            {
                return new RegistarskiBroj(broj, godina);
            }
            catch (Exception)
            {
                throw new FormatException(msg);
            }
        }

        private static bool hasTwoDelimitedParts(string s)
        {
            if (s.IndexOf(DELIMITER) == -1)
                return false;
            if (s.Split(DELIMITER).Length != 2)
                return false;
            return true;
        }

        public static bool TryParse(string s, out RegistarskiBroj result)
        {
            result = new RegistarskiBroj();
            if (!hasTwoDelimitedParts(s))
                return false;

            string[] parts = s.Split(DELIMITER);
            int broj;
            if (!Int32.TryParse(parts[0].Trim(), out broj))
                return false;

            bool twoDigitsYear = false;
            if (parts[1].Trim().Length == 2)
                twoDigitsYear = true;
            else if (parts[1].Length != 4)
                return false;
            
            short godina;
            if (!Int16.TryParse(parts[1].Trim(), out godina))
                return false;
            if (twoDigitsYear)
            {
                // vidi napomenu za + 1 u konstruktoru
                if (godina <= (DateTime.Today.Year + 1) % 100)
                    godina += 2000;
                else
                    godina += 1900;
            }
    
            try
            {
                result = new RegistarskiBroj(broj, godina);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return broj.ToString() + DELIMITER + (godinaRegistracije % 100).ToString("D2");
        }

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other))
                return true;
            // NOTE: Bitno je da se koristi operator is a ne GetType, zato sto
            // other moze da bude proxy.
            if (!(other is RegistarskiBroj))
                return false;
            RegistarskiBroj that = (RegistarskiBroj)other;
            return this.Broj == that.Broj;
                //&& this.GodinaRegistracije == that.GodinaRegistracije;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 14;
                result = 29 * result + Broj.GetHashCode();
                //result = 29 * result + GodinaRegistracije.GetHashCode();
                return result;
            }
        }

        // NOTE: Operator == (onako kako je implementiran u klasi Object)
        // ne poziva metod Equals. Da bi koriscenje operatora == dovelo do poziva
        // metoda Equals potrebno je eksplicitno implementirati operator == i u njemu
        // pozivati Equals.
        
        public static bool operator ==(RegistarskiBroj r1, RegistarskiBroj r2)
        {
            // NOTE: Bitno je da se najpre proveri da li r1 nije null, zato sto r1
            // predstavlja levu stranu u izrazu poredjenja, a na njoj moze da
            // se nalazi objekt koji ima vrednost null.

            // Takodje, kada je potrebno poredjenje po referenci (a ne po vrednosti)
            // trebalo bi koristiti iskljucivo metod ReferenceEquals a ne
            // operator == zato sto je moguce da je operator == redefinisan pa ce
            // se u stvari dobiti poredjenje po vrednosti

            if (object.ReferenceEquals(r1, null))
                return object.ReferenceEquals(r2, null);
            else
                return r1.Equals(r2);
        }

        public static bool operator !=(RegistarskiBroj r1, RegistarskiBroj r2)
        {
            return !(r1 == r2);
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (!(obj is RegistarskiBroj))
                throw new ArgumentException();

            RegistarskiBroj other = (RegistarskiBroj)obj;
            return this.Broj.CompareTo(other.Broj);
        }

        #endregion
    }
}
