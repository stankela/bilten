using System;
using System.Collections.Generic;
using System.IO;
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

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(Ime != null ? Ime : NULL);
            strBuilder.AppendLine(Prezime != null ? Prezime : NULL);
            strBuilder.AppendLine(Pol.ToString());
            strBuilder.AppendLine(KlubUcesnik != null ? KlubUcesnik.Id.ToString() : NULL);
            strBuilder.AppendLine(DrzavaUcesnik != null ? DrzavaUcesnik.Id.ToString() : NULL);
            strBuilder.AppendLine(Takmicenje != null ? Takmicenje.Id.ToString() : NULL);
            strBuilder.AppendLine(NastupaZaDrzavu.ToString());
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            string line = reader.ReadLine();
            Ime = line != NULL ? line : null;

            line = reader.ReadLine();
            Prezime = line != NULL ? line : null;

            Pol = (Pol)Enum.Parse(typeof(Pol), reader.ReadLine());

            line = reader.ReadLine();
            KlubUcesnik = line != NULL ? map.kluboviMap[int.Parse(line)] : null;

            line = reader.ReadLine();
            DrzavaUcesnik = line != NULL ? map.drzaveMap[int.Parse(line)] : null;
            
            line = reader.ReadLine();
            Takmicenje = line != NULL ? map.takmicenjeMap[int.Parse(line)] : null;
            
            NastupaZaDrzavu = bool.Parse(reader.ReadLine());
        }
    }
}
