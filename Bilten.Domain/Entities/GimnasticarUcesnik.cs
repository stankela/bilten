using Bilten.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        private bool nastupaZaDrzavu;
        public virtual bool NastupaZaDrzavu
        {
            get { return nastupaZaDrzavu; }
            set { nastupaZaDrzavu = value; }
        }

        private Nullable<float> penaltyViseboj;
        public virtual Nullable<float> PenaltyViseboj
        {
            get { return penaltyViseboj; }
            set { penaltyViseboj = value; }
        }

        private int spraveMask = 510; // 0b111111110
        public virtual int SpraveMask
        {
            get { return spraveMask; }
            set { spraveMask = value; }
        }

        private Nullable<short> takmicarskiBroj;
        public virtual Nullable<short> TakmicarskiBroj
        {
            get { return takmicarskiBroj; }
            set { takmicarskiBroj = value; }
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

        public virtual string ImeSrednjeImePrezimeDatumRodjenja
        {
            get
            {
                string result = ImeSrednjeIme + ' ' + Prezime;
                if (DatumRodjenja != null)
                    result += ", " + DatumRodjenja.ToString("d");
                return result;
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

        public virtual string DrzavaString
        {
            get
            {
                if (DrzavaUcesnik != null)
                    return DrzavaUcesnik.Kod;
                else
                    return String.Empty;
            }
        }

        public virtual string KlubString
        {
            get
            {
                if (KlubUcesnik != null)
                    return klubUcesnik.Naziv;
                else
                    return String.Empty;
            }
        }

        public virtual bool getSpravaSeBoduje(Sprava sprava)
        {
            return ((1 << (int)sprava) & SpraveMask) != 0;
        }

        public virtual void setSpravaSeBoduje(Sprava sprava)
        {
            SpraveMask |= (1 << (int)sprava);
        }

        public virtual void clearSpraveKojeSeBoduju()
        {
            SpraveMask = 0;
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
                if (DatumRodjenja != null)
                    result = 29 * result + DatumRodjenja.GetHashCode();
                return result;
            }
        }

        #region IEquatable<GimnasticarUcesnik> Members

        public virtual bool Equals(GimnasticarUcesnik other)
        {
            return this.Equals((object)other);
        }

        #endregion

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(Ime != null ? Ime : NULL);
            strBuilder.AppendLine(SrednjeIme != null ? SrednjeIme : NULL);
            strBuilder.AppendLine(Prezime != null ? Prezime : NULL);
            strBuilder.AppendLine(DatumRodjenja != null ? DatumRodjenja.ToString() : NULL);
            strBuilder.AppendLine(KlubUcesnik != null ? KlubUcesnik.Id.ToString() : NULL);
            strBuilder.AppendLine(DrzavaUcesnik != null ? DrzavaUcesnik.Id.ToString() : NULL);
            strBuilder.AppendLine(TakmicarskaKategorija != null ? TakmicarskaKategorija.Id.ToString() : NULL);
            strBuilder.AppendLine(NastupaZaDrzavu.ToString());
            strBuilder.AppendLine(PenaltyViseboj != null ? PenaltyViseboj.Value.ToString() : NULL);
            strBuilder.AppendLine(SpraveMask.ToString());
            // TODO4: Proveri da li je ispravno da ovde koristim (TakmicarskiBroj != null) umesto TakmicarskiBroj.HasValue.
            // Ako nije ispravno, proveri na ostalim mestima da li sam napravio to gresku.
            strBuilder.AppendLine(TakmicarskiBroj.HasValue ? TakmicarskiBroj.Value.ToString() : NULL);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            string line = reader.ReadLine();
            Ime = line != NULL ? line : null;
            
            line = reader.ReadLine();
            SrednjeIme = line != NULL ? line : null;
            
            line = reader.ReadLine();
            Prezime = line != NULL ? line : null;
            
            line = reader.ReadLine();
            DatumRodjenja = line != NULL ? Datum.Parse(line) : null;
            
            line = reader.ReadLine();
            KlubUcesnik = line != NULL ? map.kluboviMap[int.Parse(line)] : null;

            line = reader.ReadLine();
            DrzavaUcesnik = line != NULL ? map.drzaveMap[int.Parse(line)] : null;
            
            line = reader.ReadLine();
            TakmicarskaKategorija = line != NULL ? map.kategorijeMap[int.Parse(line)] : null;

            NastupaZaDrzavu = bool.Parse(reader.ReadLine());

            string penalty = reader.ReadLine();
            PenaltyViseboj = penalty != NULL ? float.Parse(penalty) : (float?)null;
        
            SpraveMask = int.Parse(reader.ReadLine());

            string takBroj = reader.ReadLine();
            TakmicarskiBroj = takBroj != NULL ? short.Parse(takBroj) : (short?)null;
        }
    }
}
