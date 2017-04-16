using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using System.ComponentModel;
using Bilten.Util;
using System.IO;

namespace Bilten.Domain
{
    public class Takmicenje2 : DomainObject
    {
        private Iesi.Collections.Generic.ISet<UcesnikTakmicenja2> _ucesnici = new HashedSet<UcesnikTakmicenja2>();
        public virtual Iesi.Collections.Generic.ISet<UcesnikTakmicenja2> Ucesnici
        {
            get { return _ucesnici; }
            protected set { _ucesnici = value; }
        }

        public virtual void addUcesnik(UcesnikTakmicenja2 u)
        {
            Ucesnici.Add(u);
        }

        public virtual void removeUcesnik(UcesnikTakmicenja2 u)
        {
            Ucesnici.Remove(u);
        }

        public virtual UcesnikTakmicenja2 getUcesnikKvalifikant(GimnasticarUcesnik g)
        {
            foreach (UcesnikTakmicenja2 u in Ucesnici)
            {
                if (g.Equals(u.Gimnasticar) && u.KvalStatus == KvalifikacioniStatus.Q)
                    return u;
            }
            return null;
        }

        public virtual void clearUcesnici()
        {
            foreach (UcesnikTakmicenja2 u in new List<UcesnikTakmicenja2>(Ucesnici))
                removeUcesnik(u);
        }

        public virtual IList<GimnasticarUcesnik> getUcesniciGimKvalifikanti()
        {
            IList<GimnasticarUcesnik> result = new List<GimnasticarUcesnik>();
            foreach (UcesnikTakmicenja2 u in Ucesnici)
            {
                if (u.KvalStatus == KvalifikacioniStatus.Q)
                    result.Add(u.Gimnasticar);
            }
            return result;
        }

        public virtual IList<UcesnikTakmicenja2> getUcesniciKvalifikanti()
        {
            IList<UcesnikTakmicenja2> result = new List<UcesnikTakmicenja2>();
            foreach (UcesnikTakmicenja2 u in Ucesnici)
            {
                if (u.KvalStatus == KvalifikacioniStatus.Q)
                    result.Add(u);
            }
            return result;
        }

        public virtual IList<UcesnikTakmicenja2> getUcesniciRezerve()
        {
            IList<UcesnikTakmicenja2> result = new List<UcesnikTakmicenja2>();
            foreach (UcesnikTakmicenja2 u in Ucesnici)
            {
                if (u.KvalStatus == KvalifikacioniStatus.R)
                    result.Add(u);
            }
            return result;
        }

        public virtual void createUcesnici(Takmicenje1 takmicenje1)
        {
            clearUcesnici();

            List<RezultatUkupno> rezultati = 
                new List<RezultatUkupno>(takmicenje1.PoredakUkupno.Rezultati);
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatUkupno))["RedBroj"];
            rezultati.Sort(new SortComparer<RezultatUkupno>(propDesc,
                ListSortDirection.Ascending));

            int qualOrder = 0;
            foreach (RezultatUkupno rez in rezultati)
            {
                if (rez.KvalStatus == KvalifikacioniStatus.Q)
                {
                    UcesnikTakmicenja2 u = new UcesnikTakmicenja2(rez.Gimnasticar,
                        (short)(++qualOrder), rez.Total, rez.Rank, rez.KvalStatus);
                    addUcesnik(u);
                }
            }
            int rezOrder = 0;
            foreach (RezultatUkupno rez in rezultati)
            {
                if (rez.KvalStatus == KvalifikacioniStatus.R)
                {
                    UcesnikTakmicenja2 u = new UcesnikTakmicenja2(rez.Gimnasticar,
                        (short)(qualOrder + (++rezOrder)), rez.Total, rez.Rank, rez.KvalStatus);
                    addUcesnik(u);
                }
            }
        }

        private PoredakUkupno _poredak;
        public virtual PoredakUkupno Poredak
        {
            get { return _poredak; }
            set { _poredak = value; }
        }

        public Takmicenje2()
        {
            _poredak = new PoredakUkupno(DeoTakmicenjaKod.Takmicenje2);
        }

        public virtual void ocenaAdded(Ocena o, RezultatskoTakmicenje rezTak)
        {
            if (getUcesnikKvalifikant(o.Gimnasticar) != null)
                Poredak.addOcena(o, rezTak);
        }

        public virtual void ocenaDeleted(Ocena o, RezultatskoTakmicenje rezTak)
        {
            if (getUcesnikKvalifikant(o.Gimnasticar) != null)
                Poredak.deleteOcena(o, rezTak);
        }

        public virtual void ocenaEdited(Ocena o, Ocena old, RezultatskoTakmicenje rezTak)
        {
            if (getUcesnikKvalifikant(o.Gimnasticar) != null)
                Poredak.editOcena(o, old, rezTak);
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());

            strBuilder.AppendLine(Ucesnici.Count.ToString());
            foreach (UcesnikTakmicenja2 u in Ucesnici)
                u.dump(strBuilder);

            if (Poredak == null)
                strBuilder.AppendLine(NULL);
            else
                Poredak.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            int count = int.Parse(reader.ReadLine());
            for (int i = 0; i < count; ++i)
            {
                reader.ReadLine();  // id
                UcesnikTakmicenja2 u = new UcesnikTakmicenja2();
                u.loadFromDump(reader, map);
                Ucesnici.Add(u);
            }

            string id = reader.ReadLine();
            PoredakUkupno p = null;
            if (id != NULL)
            {
                p = new PoredakUkupno();
                p.loadFromDump(reader, map);
            }
            Poredak = p;
        }
    }
}
