using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using Bilten.Util;
using System.IO;

namespace Bilten.Domain
{
    public class Ekipa : DomainObject, IEquatable<Ekipa>
    {
        private static readonly int NAZIV_MAX_LENGTH = 64;
        private static readonly int KOD_MAX_LENGTH = 7;

        private string naziv;
        public virtual string Naziv
        {
            get { return naziv; }
            set { naziv = value; }
        }

        private string kod;
        public virtual string Kod
        {
            get { return kod; }
            set { kod = value; }
        }

        private Nullable<float> penalty;
        public virtual Nullable<float> Penalty
        {
            get { return penalty; }
            set { penalty = value; }
        }

        private int spraveMask = 0;
        public virtual int SpraveMask
        {
            get { return spraveMask; }
            set { spraveMask = value; }
        }

        public Ekipa()
        { 
        
        }

        public override string ToString()
        {
            return Naziv;
        }

        private Iesi.Collections.Generic.ISet<GimnasticarUcesnik> gimnasticari = new HashedSet<GimnasticarUcesnik>();
        public virtual Iesi.Collections.Generic.ISet<GimnasticarUcesnik> Gimnasticari
        {
            get { return gimnasticari; }
            protected set { gimnasticari = value; }
        }

        public virtual bool addGimnasticar(GimnasticarUcesnik gimnasticar)
        {
            // TODO: Add business rules
            return Gimnasticari.Add(gimnasticar);
        }

        public virtual void removeGimnasticar(GimnasticarUcesnik gimnasticar)
        {
            Gimnasticari.Remove(gimnasticar);
        }

        public virtual IList<Sprava> getSpraveKojeSeBoduju(Gimnastika gimnastika)
        {
            IList<Sprava> sprave = new List<Sprava>(Sprave.getSprave(gimnastika));
            if (SpraveMask == 0)
                return sprave;

            List<Sprava> result = new List<Sprava>();
            foreach (Sprava s in sprave)
            {
                if (((1 << (int)s) & SpraveMask) != 0)
                    result.Add(s);
            }
            return result;
        }

        public virtual void setSpraveKojeSeBoduju(IList<Sprava> sprave, Gimnastika gimnastika)
        {
            SpraveMask = 0;
            if (Sprave.hasAllSprave(sprave, gimnastika))
                return;

            foreach (Sprava s in sprave)
                SpraveMask |= (1 << (int)s);
        }

        public override void validate(Notification notification)
        {
            // validate Naziv
            if (string.IsNullOrEmpty(Naziv))
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv ekipe je obavezan.");
            }
            else if (Naziv.Length > NAZIV_MAX_LENGTH)
            {
                notification.RegisterMessage(
                    "Naziv", "Naziv ekipe moze da sadrzi maksimalno "
                    + NAZIV_MAX_LENGTH + " znakova.");
            }

            // validate Kod
            if (string.IsNullOrEmpty(Kod))
            {
                notification.RegisterMessage(
                    "Kod", "Kod ekipe je obavezan.");
            }
            else if (Kod.Length > KOD_MAX_LENGTH)
            {
                notification.RegisterMessage(
                    "Kod", "Kod ekipe moze da sadrzi maksimalno "
                    + KOD_MAX_LENGTH + " znakova.");
            }
        }

        public override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (!(other is Ekipa)) return false;
            Ekipa that = (Ekipa)other;
            return this.Naziv.ToUpper() == that.Naziv.ToUpper();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 14;
                result = 29 * result + Naziv.GetHashCode();
                return result;
            }
        }

        #region IEquatable<Ekipa> Members

        public virtual bool Equals(Ekipa other)
        {
            return this.Equals((object)other);
        }

        #endregion

        public virtual IList<RezultatUkupno> getRezultatiUkupno(IList<RezultatUkupno> sviRezultatiUkupno)
        {
            IList<RezultatUkupno> result = new List<RezultatUkupno>();
            foreach (GimnasticarUcesnik g in this.Gimnasticari)
            {
                foreach (RezultatUkupno rez in sviRezultatiUkupno)
                {
                    if (g.Equals(rez.Gimnasticar))
                        result.Add(rez);
                }
            }
            return result;
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(Naziv != null ? Naziv : NULL);
            strBuilder.AppendLine(Kod != null ? Kod : NULL);
            strBuilder.AppendLine(Penalty != null ? Penalty.Value.ToString() : NULL);

            // gimnasticari
            strBuilder.AppendLine(Gimnasticari.Count.ToString());
            foreach (GimnasticarUcesnik g in Gimnasticari)
                strBuilder.AppendLine(g.Id.ToString());
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            string naziv = reader.ReadLine();
            Naziv = naziv != NULL ? naziv : null;
            string kod = reader.ReadLine();
            Kod = kod != NULL ? kod : null;
            string penalty = reader.ReadLine();
            Penalty = penalty != NULL ? float.Parse(penalty) : (float?)null;

            int brojGimnasticara = int.Parse(reader.ReadLine());
            for (int i = 0; i < brojGimnasticara; ++i)
                Gimnasticari.Add(map.gimnasticariMap[int.Parse(reader.ReadLine())]);
        }
    }
}
