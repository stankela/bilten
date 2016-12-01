using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using Bilten.Util;

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

        private Nullable<float> penalty;
        public virtual Nullable<float> Penalty
        {
            get { return penalty; }
            set { penalty = value; }
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

        public virtual void addGimnasticar(GimnasticarUcesnik gimnasticar)
        {
            // TODO: Add business rules
            Gimnasticari.Add(gimnasticar);
        }

        public virtual void removeGimnasticar(GimnasticarUcesnik gimnasticar)
        {
            Gimnasticari.Remove(gimnasticar);
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

            // TODO: proveri klub i drzavu, u slucaju nije dozvoljeno da istovremeno
            // budu zadati i klub i drzava 
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
    }
}
