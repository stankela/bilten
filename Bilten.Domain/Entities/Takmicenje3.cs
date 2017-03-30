using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using System.ComponentModel;
using Bilten.Util;

namespace Bilten.Domain
{
    public class Takmicenje3 : DomainObject
    {
        private Iesi.Collections.Generic.ISet<UcesnikTakmicenja3> _ucesnici = new HashedSet<UcesnikTakmicenja3>();
        public virtual Iesi.Collections.Generic.ISet<UcesnikTakmicenja3> Ucesnici
        {
            get { return _ucesnici; }
            protected set { _ucesnici = value; }
        }

        public virtual void addUcesnik(UcesnikTakmicenja3 u)
        {
            Ucesnici.Add(u);
        }

        public virtual void removeUcesnik(UcesnikTakmicenja3 u)
        {
            Ucesnici.Remove(u);
        }

        public virtual UcesnikTakmicenja3 getUcesnikKvalifikant(GimnasticarUcesnik g, Sprava sprava)
        {
            foreach (UcesnikTakmicenja3 u in Ucesnici)
            {
                if (g.Equals(u.Gimnasticar) && u.Sprava == sprava && u.KvalStatus == KvalifikacioniStatus.Q)
                    return u;
            }
            return null;
        }

        public virtual void clearUcesnici()
        {
            foreach (UcesnikTakmicenja3 u in new List<UcesnikTakmicenja3>(Ucesnici))
                removeUcesnik(u);
        }

        public virtual IList<GimnasticarUcesnik> getUcesniciGimKvalifikanti(Sprava sprava)
        {
            IList<GimnasticarUcesnik> result = new List<GimnasticarUcesnik>();
            foreach (UcesnikTakmicenja3 u in Ucesnici)
            {
                if (u.Sprava == sprava && u.KvalStatus == KvalifikacioniStatus.Q)
                    result.Add(u.Gimnasticar);
            }
            return result;
        }

        public virtual IList<UcesnikTakmicenja3> getUcesniciKvalifikanti(Sprava sprava)
        {
            IList<UcesnikTakmicenja3> result = new List<UcesnikTakmicenja3>();
            foreach (UcesnikTakmicenja3 u in Ucesnici)
            {
                if (u.Sprava == sprava && u.KvalStatus == KvalifikacioniStatus.Q)
                    result.Add(u);
            }
            return result;
        }

        public virtual IList<UcesnikTakmicenja3> getUcesniciRezerve(Sprava sprava)
        {
            IList<UcesnikTakmicenja3> result = new List<UcesnikTakmicenja3>();
            foreach (UcesnikTakmicenja3 u in Ucesnici)
            {
                if (u.Sprava == sprava && u.KvalStatus == KvalifikacioniStatus.R)
                    result.Add(u);
            }
            return result;
        }

        public virtual void createUcesnici(Takmicenje1 takmicenje1, bool obaPreskoka)
        {
            clearUcesnici();

            int qualOrder;
            int rezOrder;
            PropertyDescriptor propDesc;
            foreach (PoredakSprava p in takmicenje1.PoredakSprava)
            {
                List<RezultatSprava> rezultati = new List<RezultatSprava>(p.Rezultati);
                propDesc = TypeDescriptor.GetProperties(typeof(RezultatSprava))["RedBroj"];
                rezultati.Sort(new SortComparer<RezultatSprava>(propDesc,
                    ListSortDirection.Ascending));

                qualOrder = 0;
                foreach (RezultatSprava rez in rezultati)
                {
                    if (rez.KvalStatus == KvalifikacioniStatus.Q)
                    {
                        UcesnikTakmicenja3 u = new UcesnikTakmicenja3(
                            rez.Gimnasticar, p.Sprava, (short)(++qualOrder), rez.Total, 
                            rez.Rank, rez.KvalStatus);
                        addUcesnik(u);
                    }
                }
                rezOrder = 0;
                foreach (RezultatSprava rez in rezultati)
                {
                    if (rez.KvalStatus == KvalifikacioniStatus.R)
                    {
                        UcesnikTakmicenja3 u = new UcesnikTakmicenja3(
                            rez.Gimnasticar, p.Sprava, (short)(qualOrder + (++rezOrder)), rez.Total,
                            rez.Rank, rez.KvalStatus);
                        addUcesnik(u);
                    }
                }
            }

            List<RezultatPreskok> rezultatiPreskok = new List<RezultatPreskok>(
                takmicenje1.PoredakPreskok.Rezultati);

            string propName = obaPreskoka ? "RedBroj2" : "RedBroj";
            propDesc = TypeDescriptor.GetProperties(typeof(RezultatPreskok))[propName];
            rezultatiPreskok.Sort(new SortComparer<RezultatPreskok>(propDesc,
                ListSortDirection.Ascending));

            qualOrder = 0;
            foreach (RezultatPreskok rez in rezultatiPreskok)
            {
                if (rez.KvalStatus == KvalifikacioniStatus.Q)
                {
                    Nullable<float> qualScore = obaPreskoka ? rez.TotalObeOcene : rez.Total;
                    short qualRank = obaPreskoka ? rez.Rank2.Value : rez.Rank.Value;
                    UcesnikTakmicenja3 u = new UcesnikTakmicenja3(
                        rez.Gimnasticar, Sprava.Preskok, (short)(++qualOrder), 
                        qualScore, qualRank, rez.KvalStatus);
                    addUcesnik(u);
                }
            }
            rezOrder = 0;
            foreach (RezultatPreskok rez in rezultatiPreskok)
            {
                if (rez.KvalStatus == KvalifikacioniStatus.R)
                {
                    Nullable<float> qualScore = obaPreskoka ? rez.TotalObeOcene : rez.Total;
                    short qualRank = obaPreskoka ? rez.Rank2.Value : rez.Rank.Value;
                    UcesnikTakmicenja3 u = new UcesnikTakmicenja3(
                        rez.Gimnasticar, Sprava.Preskok, (short)(qualOrder + (++rezOrder)),
                        qualScore, qualRank, rez.KvalStatus);
                    addUcesnik(u);
                }
            }

        }

        private Iesi.Collections.Generic.ISet<PoredakSprava> _poredak = new HashedSet<PoredakSprava>();
        public virtual Iesi.Collections.Generic.ISet<PoredakSprava> Poredak
        {
            get { return _poredak; }
            set { _poredak = value; }
        }

        public virtual PoredakSprava getPoredak(Sprava sprava)
        {
            foreach (PoredakSprava p in Poredak)
            {
                if (p.Sprava == sprava)
                    return p;
            }
            return null;
        }

        private PoredakPreskok _poredakPreskok;
        public virtual PoredakPreskok PoredakPreskok
        {
            get { return _poredakPreskok; }
            set { _poredakPreskok = value; }
        }

        protected Takmicenje3()
        {

        }

        public Takmicenje3(Gimnastika gimnastika)
        {
            foreach (Sprava s in Sprave.getSprave(gimnastika))
            {
                if (s != Sprava.Preskok)
                    _poredak.Add(new PoredakSprava(DeoTakmicenjaKod.Takmicenje3, s));
            }
            _poredakPreskok = new PoredakPreskok(DeoTakmicenjaKod.Takmicenje3);
        }

        public virtual void ocenaAdded(Ocena o, RezultatskoTakmicenje rezTak)
        {
            // TODO4: Trebalo bi obavestiti korisnika ako se unese ocena za gimnasticara koji nije medju kvalifikantima
            // da mu rezultat nece biti vidljiv.
            if (getUcesnikKvalifikant(o.Gimnasticar, o.Sprava) != null)
            {
                if (o.Sprava == Sprava.Preskok)
                    PoredakPreskok.addOcena(o, rezTak, false);
                else
                    getPoredak(o.Sprava).addOcena(o, rezTak, false);
            }
        }

        public virtual void ocenaDeleted(Ocena o, RezultatskoTakmicenje rezTak)
        {
            if (getUcesnikKvalifikant(o.Gimnasticar, o.Sprava) != null)
            {
                if (o.Sprava == Sprava.Preskok)
                    PoredakPreskok.deleteOcena(o, rezTak, false);
                else
                    getPoredak(o.Sprava).deleteOcena(o, rezTak, false);
            }
        }

        public virtual void ocenaEdited(Ocena o, RezultatskoTakmicenje rezTak)
        {
            if (getUcesnikKvalifikant(o.Gimnasticar, o.Sprava) != null)
            {
                if (o.Sprava == Sprava.Preskok)
                    PoredakPreskok.editOcena(o, rezTak);
                else
                    getPoredak(o.Sprava).editOcena(o, rezTak);
            }
        }

        public virtual UcesnikTakmicenja3 addKvalifikant(GimnasticarUcesnik gimnasticar, Sprava sprava,
            Nullable<float> qualScore, Nullable<short> qualRank)
        {
            if (getUcesnikKvalifikant(gimnasticar, sprava) != null)
                return null;

            short qualOrder = (short)(getUcesniciKvalifikanti(sprava).Count + 1);
            UcesnikTakmicenja3 u = new UcesnikTakmicenja3(gimnasticar, sprava,
                        qualOrder, qualScore, qualRank, KvalifikacioniStatus.Q);
            Ucesnici.Add(u);
            return u;
        }

        public virtual void removeKvalifikant(GimnasticarUcesnik gimnasticar, Sprava sprava)
        {
            UcesnikTakmicenja3 kvalifikant = getUcesnikKvalifikant(gimnasticar, sprava);
            if (kvalifikant != null)
            {
                foreach (UcesnikTakmicenja3 u in getUcesniciKvalifikanti(sprava))
                {
                    if (u.QualOrder > kvalifikant.QualOrder)
                        u.QualOrder--;
                }
                Ucesnici.Remove(kvalifikant);
            }
        }

        public virtual bool moveKvalifikantUp(UcesnikTakmicenja3 u, Sprava sprava)
        {
            if (getUcesnikKvalifikant(u.Gimnasticar, sprava) == null)
                return false;
            if (u.QualOrder == 1)
                return false;

            foreach (UcesnikTakmicenja3 u2 in getUcesniciKvalifikanti(sprava))
            {
                if (u2.QualOrder == u.QualOrder - 1)
                {
                    u2.QualOrder++;
                    break;
                }
            }
            u.QualOrder--;
            return true;
        }

        public virtual bool moveKvalifikantDown(UcesnikTakmicenja3 u, Sprava sprava)
        {
            if (getUcesnikKvalifikant(u.Gimnasticar, sprava) == null)
                return false;

            IList<UcesnikTakmicenja3> kvalifikanti = getUcesniciKvalifikanti(sprava);
            if (u.QualOrder == kvalifikanti.Count)
                return false;

            foreach (UcesnikTakmicenja3 u2 in kvalifikanti)
            {
                if (u2.QualOrder == u.QualOrder + 1)
                {
                    u2.QualOrder--;
                    break;
                }
            }
            u.QualOrder++;
            return true;
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());

            if (Ucesnici == null)
                strBuilder.AppendLine(NULL);
            else
            {
                strBuilder.AppendLine(Ucesnici.Count.ToString());
                foreach (UcesnikTakmicenja3 u in Ucesnici)
                    u.dump(strBuilder);
            }

            if (Poredak == null)
                strBuilder.AppendLine(NULL);
            else
            {
                strBuilder.AppendLine(Poredak.Count.ToString());
                foreach (PoredakSprava u in Poredak)
                    u.dump(strBuilder);
            }

            if (PoredakPreskok == null)
                strBuilder.AppendLine(NULL);
            else
                PoredakPreskok.dump(strBuilder);
        }

    }
}
