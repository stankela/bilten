using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Domain
{
    // Klasa koja moze da cuva ceo datum ili samo godinu
    public class Datum : IFormattable, IComparable
    {
        private Nullable<byte> dan;
        public virtual Nullable<byte> Dan
        {
            get { return dan; }
            private set { dan = value; }
        }

        private Nullable<byte> mesec;
        public virtual Nullable<byte> Mesec
        {
            get { return mesec; }
            private set { mesec = value; }
        }

        private Nullable<short> godina;
        public virtual Nullable<short> Godina
        {
            get { return godina; }
            private set { godina = value; }
        }

        private Datum()
        { 
        
        }

        public Datum(DateTime d)
        {
            Dan = (byte)d.Day;
            Mesec = (byte)d.Month;
            Godina = (short)d.Year;
        }

        public Datum(int godina)
        {
            try
            {
                DateTime d = new DateTime(godina, 1, 1);
                Godina = (short)d.Year;
            }
            catch (Exception)
            {
                throw new ArgumentException("godina", "Nedozvoljena vrednost za godinu.");
            }            
        }

        public bool hasFullDatum()
        {
            return dan != null && mesec != null && godina != null;
        }

        public static Datum Parse(string s)
        {
            s = s.Trim();
            // ukloni tacku iza godine
            if (s.Length > 0 && s[s.Length - 1] == '.')
                s = s.Remove(s.Length - 1);

            // najpre probaj da parsiras kao godinu
            int godina = 0;
            bool parsedInt = false;
            try
            {
                godina = Int32.Parse(s);
                parsedInt = true;
            }
            catch (Exception)
            { 
            
            }

            if (parsedInt)
            {
                try
                {
                    return new Datum(godina);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new FormatException("Nepravilna vrednost za datum.");
                }
            }

            // parsiraj kao datum
            try
            {
                return new Datum(Takmicenje.ParsirajDatum(s));
            }
            catch (Exception)
            {
                throw new FormatException("Nepravilna vrednost za datum.");
            }
        }

        public static bool TryParse(string s, out Datum result)
        {
            s = s.Trim();
            // ukloni tacku iza godine
            if (s.Length > 0 && s[s.Length - 1] == '.')
                s = s.Remove(s.Length - 1);

            // najpre probaj da parsiras kao godinu
            int godina;
            if (Int32.TryParse(s, out godina))
            {
                try
                {
                    result = new Datum(godina);
                    return true;
                }
                catch (ArgumentOutOfRangeException)
                {
                    result = new Datum();
                    return false;
                }
            }

            // parsiraj kao datum
            DateTime d;
            if (DateTime.TryParse(s, out d))
            {
                result = new Datum(d);
                return true;
            }
            else
            {
                result = new Datum();
                return false;
            }
        }

        public DateTime ToDateTime()
        {
            if (!hasFullDatum())
                throw new InvalidOperationException("Datum doesn't contain full date.");
            return new DateTime(godina.Value, mesec.Value, dan.Value);
        }

        public override String ToString()
        {
            return ToString(null, null);
        }

        public String ToString(string format)
        {
            return ToString(format, null);
        }

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (hasFullDatum())
                return ToDateTime().ToString(format, formatProvider);
            else if (godina != null)
                return godina.ToString();
            else
                return String.Empty;
        }

        #endregion

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other))
                return true;
            // NOTE: Bitno je da se koristi operator is a ne GetType, zato sto
            // other moze da bude proxy.
            if (!(other is Datum))
                return false;
            Datum that = (Datum)other;
            return this.Dan == that.Dan && this.Mesec == that.Mesec
                && this.Godina == that.Godina;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 14;
                if (Dan != null)
                    result = 29 * result + Dan.GetHashCode();
                if (Mesec != null)
                    result = 29 * result + Mesec.GetHashCode();
                if (Godina != null)
                    result = 29 * result + Godina.GetHashCode();
                return result;
            }
        }

        // NOTE: Operator == (onako kako je implementiran u klasi Object)
        // ne poziva metod Equals. Da bi koriscenje operatora == dovelo do poziva
        // metoda Equals potrebno je eksplicitno implementirati operator == i u njemu
        // pozivati Equals.
        
        public static bool operator ==(Datum d1, Datum d2)
        {
            // NOTE: Bitno je da se najpre proveri da li d1 nije null, zato sto d1
            // predstavlja levu stranu u izrazu poredjenja, a na njoj moze da
            // se nalazi objekt koji ima vrednost null.

            // Takodje, kada je potrebno poredjenje po referenci (a ne po vrednosti)
            // trebalo bi koristiti iskljucivo metod ReferenceEquals a ne
            // operator == zato sto je moguce da je operator == redefinisan pa ce
            // se u stvari dobiti poredjenje po vrednosti
            
            if (object.ReferenceEquals(d1, null))
                return object.ReferenceEquals(d2, null);
            else
                return d1.Equals(d2);
        }

        public static bool operator !=(Datum d1, Datum d2)
        {
            return !(d1 == d2);
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (!(obj is Datum))
                throw new ArgumentException();

            Datum other = (Datum)obj;
            int result = this.Godina.Value.CompareTo(other.Godina.Value);

            if (!hasFullDatum() || !other.hasFullDatum())
                return result;

            if (result == 0)
                result = this.Mesec.Value.CompareTo(other.Mesec.Value);
            if (result == 0)
                result = this.Dan.Value.CompareTo(other.Dan.Value);
            return result;
        }

        #endregion
    }
}
