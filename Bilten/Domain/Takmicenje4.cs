using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using System.ComponentModel;
using NHibernate;

namespace Bilten.Domain
{
    public class Takmicenje4 : DomainObject
    {
        private ISet<UcesnikTakmicenja4> _ucesnici = new HashedSet<UcesnikTakmicenja4>();
        public virtual ISet<UcesnikTakmicenja4> Ucesnici
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

        public virtual void ocenaAdded(Ocena o, RezultatskoTakmicenje rezTak)
        {
            Poredak.addOcena(o, rezTak);
        }

        public virtual void ocenaDeleted(Ocena o, RezultatskoTakmicenje rezTak)
        {
            Poredak.deleteOcena(o, rezTak);
        }

        public virtual void ocenaEdited(Ocena o, Ocena old, RezultatskoTakmicenje rezTak)
        {
            Poredak.editOcena(o, old, rezTak);
        }
    }
}
