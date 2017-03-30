using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using System.ComponentModel;
using NHibernate;
using Bilten.Util;

namespace Bilten.Domain
{
    public class Takmicenje4 : DomainObject
    {
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
            // TODO4: Domain klasa ne bi trebalo da koristi NHibernateUtil klasu. Ukloni i referencu na NHibernate.dll
            // iz Bilten.Domain.dll.
            NHibernateUtil.Initialize(Ucesnici);
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

        public virtual void ocenaAdded(Ocena o, RezultatskoTakmicenje rezTak, List<Ocena> sveOceneTak4)
        {
            if (rezTak.ImaEkipnoTakmicenje)
                Poredak.addOcena(o, rezTak, sveOceneTak4);
        }

        public virtual void ocenaDeleted(Ocena o, RezultatskoTakmicenje rezTak, List<Ocena> sveOceneTak4)
        {
            Poredak.deleteOcena(o, rezTak, sveOceneTak4);
        }

        public virtual void ocenaEdited(Ocena o, Ocena old, RezultatskoTakmicenje rezTak, List<Ocena> sveOceneTak4)
        {
            Poredak.editOcena(o, old, rezTak, sveOceneTak4);
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            throw new NotImplementedException();
        }
    }
}
