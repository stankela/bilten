using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using System.ComponentModel;
using Bilten.Util;
using System.IO;

namespace Bilten.Domain
{
    public class Takmicenje4 : DomainObject
    {
        // TODO4: Fali Equals i GetHashCode za UcesnikTakmicenja4
        
        private Iesi.Collections.Generic.ISet<UcesnikTakmicenja4> _ucesnici = new HashedSet<UcesnikTakmicenja4>();
        public virtual Iesi.Collections.Generic.ISet<UcesnikTakmicenja4> Ucesnici
        {
            get { return _ucesnici; }
            protected set { _ucesnici = value; }
        }

        public virtual void addUcesnik(UcesnikTakmicenja4 u)
        {
            Ucesnici.Add(u);
        }

        public virtual void removeUcesnik(UcesnikTakmicenja4 u)
        {
            Ucesnici.Remove(u);
        }

        public virtual void clearUcesnici()
        {
            foreach (UcesnikTakmicenja4 u in new List<UcesnikTakmicenja4>(Ucesnici))
                removeUcesnik(u);
        }

        public virtual IList<Ekipa> getUcesnici()
        {
            IList<Ekipa> result = new List<Ekipa>();
            foreach (UcesnikTakmicenja4 u in Ucesnici)
                result.Add(u.Ekipa);
            return result;
        }

        public virtual void createUcesnici(Takmicenje1 takmicenje1)
        {
            List<RezultatEkipno> rezultati = new List<RezultatEkipno>(
                takmicenje1.PoredakEkipno.Rezultati);
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatEkipno))["RedBroj"];
            rezultati.Sort(new SortComparer<RezultatEkipno>(propDesc,
                ListSortDirection.Ascending));

            clearUcesnici();
            int qualOrder = 0;
            foreach (RezultatEkipno rez in rezultati)
            {
                if (rez.KvalStatus == KvalifikacioniStatus.Q)
                {
                    UcesnikTakmicenja4 u = new UcesnikTakmicenja4(rez.Ekipa,
                        (short)(++qualOrder), rez.Total, rez.Rank, rez.KvalStatus);
                    addUcesnik(u);
                }
            }
        }

        private PoredakEkipno _poredak;
        public virtual PoredakEkipno Poredak
        {
            get { return _poredak; }
            set { _poredak = value; }
        }

        public Takmicenje4()
        {
            _poredak = new PoredakEkipno(DeoTakmicenjaKod.Takmicenje4);
        }

        public virtual void updateRezultatEkipe(Ekipa e, RezultatskoTakmicenje rezTak, List<RezultatUkupno> rezultati)
        {
            if (rezTak.ImaEkipnoTakmicenje)
                Poredak.recreateRezultat(e, rezTak, rezultati);
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());

            strBuilder.AppendLine(Ucesnici.Count.ToString());
            foreach (UcesnikTakmicenja4 u in Ucesnici)
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
                UcesnikTakmicenja4 u = new UcesnikTakmicenja4();
                u.loadFromDump(reader, map);
                Ucesnici.Add(u);
            }

            string id = reader.ReadLine();
            PoredakEkipno p = null;
            if (id != NULL)
            {
                p = new PoredakEkipno();
                p.loadFromDump(reader, map);
            }
            Poredak = p;
        }
    }
}
