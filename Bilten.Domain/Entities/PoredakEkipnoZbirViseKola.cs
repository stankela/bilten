using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Bilten.Util;
using System.IO;

namespace Bilten.Domain
{
    public class PoredakEkipnoZbirViseKola : DomainObject
    {
        private DeoTakmicenjaKod deoTakKod;
        public virtual DeoTakmicenjaKod DeoTakmicenjaKod
        {
            get { return deoTakKod; }
            set { deoTakKod = value; }
        }

        private IList<RezultatEkipnoZbirViseKola> _rezultati = new List<RezultatEkipnoZbirViseKola>();
        public virtual IList<RezultatEkipnoZbirViseKola> Rezultati
        {
            get { return _rezultati; }
            protected set { _rezultati = value; }
        }

        public PoredakEkipnoZbirViseKola()
        {
            DeoTakmicenjaKod = DeoTakmicenjaKod.Takmicenje1;
        }

        public virtual void create(RezultatskoTakmicenje rezTak, PoredakEkipno poredakPrvoKolo,
            PoredakEkipno poredakDrugoKolo, PoredakEkipno poredakTreceKolo, PoredakEkipno poredakCetvrtoKolo)
        {
            IList<Ekipa> ekipe = new List<Ekipa>(rezTak.Takmicenje1.Ekipe);

            // NOTE: Da bi Ekipa mogla da se koristi kao key u Dictionary, mora da implementira
            // interfejs IEquatable<Ekipa>.
            IDictionary<Ekipa, RezultatEkipnoZbirViseKola> rezultatiMap =
                new Dictionary<Ekipa, RezultatEkipnoZbirViseKola>();
            foreach (Ekipa e in ekipe)
            {
                RezultatEkipnoZbirViseKola rezultat = new RezultatEkipnoZbirViseKola();
                rezultat.Ekipa = e;
                rezultatiMap.Add(e, rezultat);
            }

            if (poredakPrvoKolo != null)
            {
                foreach (RezultatEkipno r in poredakPrvoKolo.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Ekipa))
                    {
                        rezultatiMap[r.Ekipa].ParterPrvoKolo = r.Parter;
                        rezultatiMap[r.Ekipa].KonjPrvoKolo = r.Konj;
                        rezultatiMap[r.Ekipa].KarikePrvoKolo = r.Karike;
                        rezultatiMap[r.Ekipa].PreskokPrvoKolo = r.Preskok;
                        rezultatiMap[r.Ekipa].RazbojPrvoKolo = r.Razboj;
                        rezultatiMap[r.Ekipa].VratiloPrvoKolo = r.Vratilo;
                        rezultatiMap[r.Ekipa].DvovisinskiRazbojPrvoKolo = r.DvovisinskiRazboj;
                        rezultatiMap[r.Ekipa].GredaPrvoKolo = r.Greda;
                        rezultatiMap[r.Ekipa].TotalPrvoKolo = r.Total;
                    }
                }
            }

            if (poredakDrugoKolo != null)
            {
                foreach (RezultatEkipno r in poredakDrugoKolo.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Ekipa))
                    {
                        rezultatiMap[r.Ekipa].ParterDrugoKolo = r.Parter;
                        rezultatiMap[r.Ekipa].KonjDrugoKolo = r.Konj;
                        rezultatiMap[r.Ekipa].KarikeDrugoKolo = r.Karike;
                        rezultatiMap[r.Ekipa].PreskokDrugoKolo = r.Preskok;
                        rezultatiMap[r.Ekipa].RazbojDrugoKolo = r.Razboj;
                        rezultatiMap[r.Ekipa].VratiloDrugoKolo = r.Vratilo;
                        rezultatiMap[r.Ekipa].DvovisinskiRazbojDrugoKolo = r.DvovisinskiRazboj;
                        rezultatiMap[r.Ekipa].GredaDrugoKolo = r.Greda;
                        rezultatiMap[r.Ekipa].TotalDrugoKolo = r.Total;
                    }
                }
            }

            if (poredakTreceKolo != null)
            {
                foreach (RezultatEkipno r in poredakTreceKolo.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Ekipa))
                    {
                        rezultatiMap[r.Ekipa].ParterTreceKolo = r.Parter;
                        rezultatiMap[r.Ekipa].KonjTreceKolo = r.Konj;
                        rezultatiMap[r.Ekipa].KarikeTreceKolo = r.Karike;
                        rezultatiMap[r.Ekipa].PreskokTreceKolo = r.Preskok;
                        rezultatiMap[r.Ekipa].RazbojTreceKolo = r.Razboj;
                        rezultatiMap[r.Ekipa].VratiloTreceKolo = r.Vratilo;
                        rezultatiMap[r.Ekipa].DvovisinskiRazbojTreceKolo = r.DvovisinskiRazboj;
                        rezultatiMap[r.Ekipa].GredaTreceKolo = r.Greda;
                        rezultatiMap[r.Ekipa].TotalTreceKolo = r.Total;
                    }
                }
            }

            if (poredakCetvrtoKolo != null)
            {
                foreach (RezultatEkipno r in poredakCetvrtoKolo.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Ekipa))
                    {
                        rezultatiMap[r.Ekipa].ParterCetvrtoKolo = r.Parter;
                        rezultatiMap[r.Ekipa].KonjCetvrtoKolo = r.Konj;
                        rezultatiMap[r.Ekipa].KarikeCetvrtoKolo = r.Karike;
                        rezultatiMap[r.Ekipa].PreskokCetvrtoKolo = r.Preskok;
                        rezultatiMap[r.Ekipa].RazbojCetvrtoKolo = r.Razboj;
                        rezultatiMap[r.Ekipa].VratiloCetvrtoKolo = r.Vratilo;
                        rezultatiMap[r.Ekipa].DvovisinskiRazbojCetvrtoKolo = r.DvovisinskiRazboj;
                        rezultatiMap[r.Ekipa].GredaCetvrtoKolo = r.Greda;
                        rezultatiMap[r.Ekipa].TotalCetvrtoKolo = r.Total;
                    }
                }
            }

            List<RezultatEkipnoZbirViseKola> rezultati = new List<RezultatEkipnoZbirViseKola>(rezultatiMap.Values);
            Rezultati.Clear();
            foreach (RezultatEkipnoZbirViseKola rez in rezultati)
                Rezultati.Add(rez);

            // Total moze da bude krajnja finalna ocena ili ulazna finalna ocena. U oba slucaja se Total izracunava
            // na isti nacin.
            foreach (RezultatEkipnoZbirViseKola rez in Rezultati)
            {
                if (rez.TotalPrvoKolo == null && rez.TotalDrugoKolo == null
                    && rez.TotalTreceKolo == null && rez.TotalCetvrtoKolo == null)
                {
                    rez.setTotal(null);
                    continue;
                }
                float total1 = rez.TotalPrvoKolo == null ? 0 : rez.TotalPrvoKolo.Value;
                float total2 = rez.TotalDrugoKolo == null ? 0 : rez.TotalDrugoKolo.Value;
                float total3 = rez.TotalTreceKolo == null ? 0 : rez.TotalTreceKolo.Value;
                float total4 = rez.TotalCetvrtoKolo == null ? 0 : rez.TotalCetvrtoKolo.Value;
                float total;

                /*if (rezTak.Propozicije.Tak4FinalnaOcenaJeZbirObaKola)
                    total = total1 + total2;
                else if (rezTak.Propozicije.Tak4FinalnaOcenaJeMaxObaKola)
                    total = total1 > total2 ? total1 : total2;
                else
                {
                    // TODO3: Proveri da li ovde treba podesavati broj decimala.
                    total = (total1 + total2) / 2;
                    if (rezTak.Propozicije.Tak4NeRacunajProsekAkoNemaOceneIzObaKola
                        && (rez.TotalPrvoKolo == null || rez.TotalDrugoKolo == null))
                    {
                        total = total1 + total2;
                    }
                }*/
                total = total1 + total2 + total3 + total4;
                rez.setTotal(total);
            }

            rankRezultati();
        }

        private void rankRezultati()
        {
            List<RezultatEkipnoZbirViseKola> rezultati = new List<RezultatEkipnoZbirViseKola>(Rezultati);

            PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                TypeDescriptor.GetProperties(typeof(RezultatEkipnoZbirViseKola))["Total"],
                TypeDescriptor.GetProperties(typeof(RezultatEkipnoZbirViseKola))["NazivEkipe"]
            };
            ListSortDirection[] sortDir = new ListSortDirection[] {
                ListSortDirection.Descending,
                ListSortDirection.Ascending
            };
            rezultati.Sort(new SortComparer<RezultatEkipnoZbirViseKola>(propDesc, sortDir));

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
                    if (rezultati[i].Total != prevTotal)
                        rezultati[i].Rank = (short)(i + 1);
                    else
                        rezultati[i].Rank = prevRank;

                    prevTotal = rezultati[i].Total.Value;
                    prevRank = rezultati[i].Rank.Value;
                }
            }
        }

        public virtual List<RezultatEkipnoZbirViseKola> getRezultati()
        {
            List<RezultatEkipnoZbirViseKola> result = new List<RezultatEkipnoZbirViseKola>(Rezultati);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatEkipnoZbirViseKola))["RedBroj"];
            result.Sort(new SortComparer<RezultatEkipnoZbirViseKola>(propDesc, ListSortDirection.Ascending));

            return result;
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(DeoTakmicenjaKod.ToString());

            strBuilder.AppendLine(Rezultati.Count.ToString());
            foreach (RezultatEkipnoZbirViseKola r in Rezultati)
                r.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            DeoTakmicenjaKod = (DeoTakmicenjaKod)Enum.Parse(typeof(DeoTakmicenjaKod), reader.ReadLine());

            int brojRezultata = int.Parse(reader.ReadLine());
            for (int i = 0; i < brojRezultata; ++i)
            {
                reader.ReadLine();  // id
                RezultatEkipnoZbirViseKola r = new RezultatEkipnoZbirViseKola();
                r.loadFromDump(reader, map);
                Rezultati.Add(r);
            }
        }
    }
}
