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

        // TODO4: Da li je ovo potrebno? Gimnastika je uvek kao i za Takmicenje.
        private Gimnastika gimnastika;
        public virtual Gimnastika Gimnastika
        {
            get { return gimnastika; }
            set { gimnastika = value; }
        }

        // TODO4: Izbaci ovo.
        private RegistarskiBroj registarskiBroj;
        public virtual RegistarskiBroj RegistarskiBroj
        {
            get { return registarskiBroj; }
            set { registarskiBroj = value; }
        }

        // TODO4: Izbaci ovo.
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

        private bool nastupaZaDrzavu;
        public virtual bool NastupaZaDrzavu
        {
            get { return nastupaZaDrzavu; }
            set { nastupaZaDrzavu = value; }
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
            strBuilder.AppendLine(Gimnastika.ToString());
            strBuilder.AppendLine(RegistarskiBroj != null ? RegistarskiBroj.ToString() : NULL);
            strBuilder.AppendLine(TakmicarskiBroj != null ? TakmicarskiBroj.ToString() : NULL);
            strBuilder.AppendLine(Takmicenje != null ? Takmicenje.Id.ToString() : NULL);
            strBuilder.AppendLine(KlubUcesnik != null ? KlubUcesnik.Id.ToString() : NULL);
            strBuilder.AppendLine(DrzavaUcesnik != null ? DrzavaUcesnik.Id.ToString() : NULL);
            strBuilder.AppendLine(TakmicarskaKategorija != null ? TakmicarskaKategorija.Id.ToString() : NULL);
            strBuilder.AppendLine(NastupaZaDrzavu.ToString());
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
            
            Gimnastika = (Gimnastika)Enum.Parse(typeof(Gimnastika), reader.ReadLine());

            line = reader.ReadLine();
            RegistarskiBroj = line != NULL ? RegistarskiBroj.Parse(line) : null;

            line = reader.ReadLine();
            TakmicarskiBroj = line != NULL ? int.Parse(line) : (int?)null;

            line = reader.ReadLine();
            Takmicenje = line != NULL ? map.takmicenjeMap[int.Parse(line)] : null;

            line = reader.ReadLine();
            KlubUcesnik = line != NULL ? map.kluboviMap[int.Parse(line)] : null;

            line = reader.ReadLine();
            DrzavaUcesnik = line != NULL ? map.drzaveMap[int.Parse(line)] : null;
            
            line = reader.ReadLine();
            TakmicarskaKategorija = line != NULL ? map.kategorijeMap[int.Parse(line)] : null;

            NastupaZaDrzavu = bool.Parse(reader.ReadLine());
        }
    }
}
