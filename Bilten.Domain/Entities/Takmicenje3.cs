using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using System.ComponentModel;
using Bilten.Util;
using System.IO;
using Bilten.Exceptions;

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
                Ucesnici.Remove(u);
        }

        public virtual void clearUcesnik(GimnasticarUcesnik g)
        {
            foreach (UcesnikTakmicenja3 u in new List<UcesnikTakmicenja3>(Ucesnici))
            {
                if (!u.Gimnasticar.Equals(g))
                    continue;

                // TODO4: Ovo nece apdejtovati QualOrder od rezervi.
                foreach (UcesnikTakmicenja3 u2 in getUcesniciKvalifikanti(u.Sprava))
                {
                    if (u2.QualOrder > u.QualOrder)
                        u2.QualOrder--;
                }
                Ucesnici.Remove(u);
            }
        }

        public virtual IList<UcesnikTakmicenja3> getUcesniciKvalifikanti(Sprava sprava)
        {
            return doGetUcesnici(sprava, KvalifikacioniStatus.Q);
        }

        public virtual IList<UcesnikTakmicenja3> getUcesniciRezerve(Sprava sprava)
        {
            return doGetUcesnici(sprava, KvalifikacioniStatus.R);
        }

        public virtual IList<UcesnikTakmicenja3> doGetUcesnici(Sprava sprava, KvalifikacioniStatus kvalStatus)
        {
            List<UcesnikTakmicenja3> result = new List<UcesnikTakmicenja3>();
            foreach (UcesnikTakmicenja3 u in Ucesnici)
            {
                if (u.Sprava == sprava && u.KvalStatus == kvalStatus)
                    result.Add(u);
            }
            PropertyDescriptor propDesc = TypeDescriptor.GetProperties(typeof(UcesnikTakmicenja3))["QualOrder"];
            result.Sort(new SortComparer<UcesnikTakmicenja3>(propDesc, ListSortDirection.Ascending));
            return result;
        }

        public virtual IList<GimnasticarUcesnik> getGimnasticariKvalifikanti(Sprava sprava)
        {
            IList<GimnasticarUcesnik> result = new List<GimnasticarUcesnik>();
            foreach (UcesnikTakmicenja3 u in getUcesniciKvalifikanti(sprava))
                result.Add(u.Gimnasticar);
            return result;
        }

        public virtual void createUcesnici(Takmicenje1 takmicenje1, bool obaPreskoka)
        {
            clearUcesnici();
            short qualOrder;
            short rezOrder;
            int brojKvalifikanata;
            foreach (PoredakSprava p in takmicenje1.PoredakSprava)
            {
                brojKvalifikanata = p.getBrojKvalifikanata();
                qualOrder = 0;
                rezOrder = 0;
                foreach (RezultatSprava r in p.getRezultati())
                {
                    short order;
                    if (r.KvalStatus == KvalifikacioniStatus.Q)
                        order = ++qualOrder;
                    else if (r.KvalStatus == KvalifikacioniStatus.R)
                        order = (short)(brojKvalifikanata + (++rezOrder));
                    else
                        continue;
                    addUcesnik(r.Gimnasticar, p.Sprava, r.Total, r.Rank, r.KvalStatus, order);
                }
            }

            qualOrder = 0;
            rezOrder = 0;
            brojKvalifikanata = takmicenje1.PoredakPreskok.getBrojKvalifikanata();
            foreach (RezultatPreskok r in takmicenje1.PoredakPreskok.getRezultati())
            {
                short order;
                if (r.KvalStatus == KvalifikacioniStatus.Q)
                    order = ++qualOrder;
                else if (r.KvalStatus == KvalifikacioniStatus.R)
                    order = (short)(brojKvalifikanata + (++rezOrder));
                else
                    continue;
                Nullable<float> qualScore = obaPreskoka ? r.TotalObeOcene : r.Total;
                addUcesnik(r.Gimnasticar, Sprava.Preskok, qualScore, r.Rank, r.KvalStatus, order);
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

        public Takmicenje3()
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

        // Public metod koji se poziva samo kada se dodaje novi kvalifikant u prozoru za dodavanje kvalifikanata.
        public virtual UcesnikTakmicenja3 addKvalifikant(GimnasticarUcesnik gimnasticar, Sprava sprava,
            Nullable<float> qualScore, Nullable<short> qualRank)
        {
            foreach (UcesnikTakmicenja3 u in getUcesniciKvalifikanti(sprava))
            {
                if (u.Gimnasticar.Id == gimnasticar.Id)
                {
                    throw new BusinessException(
                        String.Format("Gimnasticar \"{0}\" je vec medju kvalifikantima.", gimnasticar));
                }
            }
            // Ne proveravam da li je gimnasticar medju rezervama. Kada se doda nov kvalifikant u takmicenju 3 koji je bio
            // rezerva, on i dalje ostaje rezerva (tj. nalazi se i na listi za kvalifikante i na listi za rezerve). Dakle,
            // u setu Ucesnici isti gimnasticar moze da se pojavljuje dva puta.
            short qualOrder = (short)(getUcesniciKvalifikanti(sprava).Count + 1);
            return addUcesnik(gimnasticar, sprava, qualScore, qualRank, KvalifikacioniStatus.Q, qualOrder);
        }

        private UcesnikTakmicenja3 addUcesnik(GimnasticarUcesnik gimnasticar, Sprava sprava,
            Nullable<float> qualScore, Nullable<short> qualRank, KvalifikacioniStatus kvalStatus, short qualOrder)
        {
            UcesnikTakmicenja3 u = new UcesnikTakmicenja3(gimnasticar, sprava, qualOrder, qualScore, qualRank, kvalStatus);
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

            strBuilder.AppendLine(Ucesnici.Count.ToString());
            foreach (UcesnikTakmicenja3 u in Ucesnici)
                u.dump(strBuilder);

            strBuilder.AppendLine(Poredak.Count.ToString());
            foreach (PoredakSprava u in Poredak)
                u.dump(strBuilder);

            if (PoredakPreskok == null)
                strBuilder.AppendLine(NULL);
            else
                PoredakPreskok.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            int count = int.Parse(reader.ReadLine());
            for (int i = 0; i < count; ++i)
            {
                reader.ReadLine();  // id
                UcesnikTakmicenja3 u = new UcesnikTakmicenja3();
                u.loadFromDump(reader, map);
                Ucesnici.Add(u);
            }

            count = int.Parse(reader.ReadLine());
            for (int i = 0; i < count; ++i)
            {
                reader.ReadLine();  // id
                PoredakSprava p = new PoredakSprava();
                p.loadFromDump(reader, map);
                Poredak.Add(p);
            }

            string id = reader.ReadLine();
            PoredakPreskok p2 = null;
            if (id != NULL)
            {
                p2 = new PoredakPreskok();
                p2.loadFromDump(reader, map);
            }
            PoredakPreskok = p2;
        }
    }
}
