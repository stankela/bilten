using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Bilten.Util;
using System.IO;

namespace Bilten.Domain
{
    public class PoredakUkupnoZbirViseKola : DomainObject
    {
        // TODO: Ovo svojstvo u sustini nema nikakvu ulogu zato sto ga uvek inicijalizujem na Takmicenje1. Morao sam da ga
        // uvedem zato sto mi inace NHibernate prijavljuje gresku kod snimanja u bazu zato sto tabela
        // poredak_ukupno_zbir_vise_kola ima samo jednu kolonu (primary key), i u insert naredbi se dobijaju prazne
        // zagrade - "INSERT INTO poredak_ukupno_zbir_vise_kola VALUES ( )". Isto i za PoredakEkipnoZbirViseKola.
        // Proveri da li moze drugacije.
        private DeoTakmicenjaKod deoTakKod;
        public virtual DeoTakmicenjaKod DeoTakmicenjaKod
        {
            get { return deoTakKod; }
            set { deoTakKod = value; }
        }

        private IList<RezultatUkupnoZbirViseKola> _rezultati = new List<RezultatUkupnoZbirViseKola>();
        public virtual IList<RezultatUkupnoZbirViseKola> Rezultati
        {
            get { return _rezultati; }
            protected set { _rezultati = value; }
        }

        public PoredakUkupnoZbirViseKola()
        {
            DeoTakmicenjaKod = DeoTakmicenjaKod.Takmicenje1;
        }

        public virtual void create(RezultatskoTakmicenje rezTak, PoredakUkupno p1, PoredakUkupno p2,
            PoredakUkupno p3, PoredakUkupno p4)
        {
            IList<GimnasticarUcesnik> gimnasticari = new List<GimnasticarUcesnik>(rezTak.Takmicenje1.Gimnasticari);

            // NOTE: Da bi GimnasticarUcesnik mogao da se koristi kao key u Dictionary, mora da implementira
            // interfejs IEquatable<GimnasticarUcesnik>.
            IDictionary<GimnasticarUcesnik, RezultatUkupnoZbirViseKola> rezultatiMap =
                new Dictionary<GimnasticarUcesnik, RezultatUkupnoZbirViseKola>();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                RezultatUkupnoZbirViseKola rezultat = new RezultatUkupnoZbirViseKola();
                rezultat.Gimnasticar = g;
                rezultatiMap.Add(g, rezultat);
            }

            foreach (RezultatUkupno r in p1.Rezultati)
            {
                if (rezultatiMap.ContainsKey(r.Gimnasticar))
                    rezultatiMap[r.Gimnasticar].initPrvoKolo(r);
            }
            foreach (RezultatUkupno r in p2.Rezultati)
            {
                if (rezultatiMap.ContainsKey(r.Gimnasticar))
                    rezultatiMap[r.Gimnasticar].initDrugoKolo(r);
            }
            if (p3 != null)
            {
                foreach (RezultatUkupno r in p3.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Gimnasticar))
                        rezultatiMap[r.Gimnasticar].initTreceKolo(r);
                }
            }
            if (p4 != null)
            {
                foreach (RezultatUkupno r in p4.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Gimnasticar))
                        rezultatiMap[r.Gimnasticar].initCetvrtoKolo(r);
                }
            }

            Rezultati.Clear();
            foreach (RezultatUkupnoZbirViseKola r in rezultatiMap.Values)
            {
                r.calculateTotal();
                Rezultati.Add(r);
            }

            rankRezultati();
        }

        private void rankRezultati()
        {
            List<RezultatUkupnoZbirViseKola> rezultati = new List<RezultatUkupnoZbirViseKola>(Rezultati);

            PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                TypeDescriptor.GetProperties(typeof(RezultatUkupnoZbirViseKola))["Total"],
                TypeDescriptor.GetProperties(typeof(RezultatUkupnoZbirViseKola))["PrezimeIme"]
            };
            ListSortDirection[] sortDir = new ListSortDirection[] {
                ListSortDirection.Descending,
                ListSortDirection.Ascending
            };
            rezultati.Sort(new SortComparer<RezultatUkupnoZbirViseKola>(propDesc, sortDir));

            float prevTotal = -1f;
            short prevRank = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                rezultati[i].KvalStatus = KvalifikacioniStatus.None;
                rezultati[i].RedBroj = (short)(i + 1);

                if (rezultati[i].Total == null)
                {
                    rezultati[i].Rank = null;
                }
                else
                {
                    // TODO: Razresi situaciju kada dva takmicara imaju isti total
                    // (uradi i za PoredakSprava, PoredakPreskok i PoredakEkipno)
                    if (rezultati[i].Total != prevTotal)
                        rezultati[i].Rank = (short)(i + 1);
                    else
                        rezultati[i].Rank = prevRank;

                    prevTotal = rezultati[i].Total.Value;
                    prevRank = rezultati[i].Rank.Value;
                }
            }
        }

        public virtual void addGimnasticar(GimnasticarUcesnik g, RezultatskoTakmicenje rezTak,
            PoredakUkupno p1, PoredakUkupno p2, PoredakUkupno p3, PoredakUkupno p4)
        {
            RezultatUkupnoZbirViseKola rezultat = new RezultatUkupnoZbirViseKola();
            rezultat.Gimnasticar = g;

            foreach (RezultatUkupno r in p1.Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                {
                    rezultat.initPrvoKolo(r);
                    break;
                }
            }
            foreach (RezultatUkupno r in p2.Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                {
                    rezultat.initDrugoKolo(r);
                    break;
                }
            }
            if (p3 != null)
            {
                foreach (RezultatUkupno r in p3.Rezultati)
                {
                    if (r.Gimnasticar.Equals(g))
                    {
                        rezultat.initTreceKolo(r);
                        break;
                    }
                }
            }
            if (p4 != null)
            {
                foreach (RezultatUkupno r in p4.Rezultati)
                {
                    if (r.Gimnasticar.Equals(g))
                    {
                        rezultat.initCetvrtoKolo(r);
                        break;
                    }
                }
            }

            rezultat.calculateTotal();
            Rezultati.Add(rezultat);

            rankRezultati();
        }

        public virtual void deleteGimnasticar(GimnasticarUcesnik g, RezultatskoTakmicenje rezTak)
        {
            RezultatUkupnoZbirViseKola r = getRezultat(g);
            if (r != null)
            {
                Rezultati.Remove(r);
                rankRezultati();
            }
        }

        private RezultatUkupnoZbirViseKola getRezultat(GimnasticarUcesnik g)
        {
            foreach (RezultatUkupnoZbirViseKola r in Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                    return r;
            }
            return null;
        }

        public virtual List<RezultatUkupnoZbirViseKola> getRezultati()
        {
            List<RezultatUkupnoZbirViseKola> result = new List<RezultatUkupnoZbirViseKola>(Rezultati);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatUkupnoZbirViseKola))["RedBroj"];
            result.Sort(new SortComparer<RezultatUkupnoZbirViseKola>(propDesc, ListSortDirection.Ascending));

            return result;
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(DeoTakmicenjaKod.ToString());

            strBuilder.AppendLine(Rezultati.Count.ToString());
            foreach (RezultatUkupnoZbirViseKola r in Rezultati)
                r.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            DeoTakmicenjaKod = (DeoTakmicenjaKod)Enum.Parse(typeof(DeoTakmicenjaKod), reader.ReadLine());
            
            int brojRezultata = int.Parse(reader.ReadLine());
            for (int i = 0; i < brojRezultata; ++i)
            {
                reader.ReadLine();  // id
                RezultatUkupnoZbirViseKola r = new RezultatUkupnoZbirViseKola();
                r.loadFromDump(reader, map);
                Rezultati.Add(r);
            }
        }
    }
}
