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

        public virtual void create(RezultatskoTakmicenje rezTak, PoredakUkupno poredakPrvoKolo,
            PoredakUkupno poredakDrugoKolo, PoredakUkupno poredakTreceKolo, PoredakUkupno poredakCetvrtoKolo)
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

            if (poredakPrvoKolo != null)
            {
                foreach (RezultatUkupno r in poredakPrvoKolo.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Gimnasticar))
                    {
                        rezultatiMap[r.Gimnasticar].ParterPrvoKolo = r.Parter;
                        rezultatiMap[r.Gimnasticar].KonjPrvoKolo = r.Konj;
                        rezultatiMap[r.Gimnasticar].KarikePrvoKolo = r.Karike;
                        rezultatiMap[r.Gimnasticar].PreskokPrvoKolo = r.Preskok;
                        rezultatiMap[r.Gimnasticar].RazbojPrvoKolo = r.Razboj;
                        rezultatiMap[r.Gimnasticar].VratiloPrvoKolo = r.Vratilo;
                        rezultatiMap[r.Gimnasticar].DvovisinskiRazbojPrvoKolo = r.DvovisinskiRazboj;
                        rezultatiMap[r.Gimnasticar].GredaPrvoKolo = r.Greda;
                        rezultatiMap[r.Gimnasticar].TotalPrvoKolo = r.Total;
                    }
                }
            }

            if (poredakDrugoKolo != null)
            {
                foreach (RezultatUkupno r in poredakDrugoKolo.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Gimnasticar))
                    {
                        rezultatiMap[r.Gimnasticar].ParterDrugoKolo = r.Parter;
                        rezultatiMap[r.Gimnasticar].KonjDrugoKolo = r.Konj;
                        rezultatiMap[r.Gimnasticar].KarikeDrugoKolo = r.Karike;
                        rezultatiMap[r.Gimnasticar].PreskokDrugoKolo = r.Preskok;
                        rezultatiMap[r.Gimnasticar].RazbojDrugoKolo = r.Razboj;
                        rezultatiMap[r.Gimnasticar].VratiloDrugoKolo = r.Vratilo;
                        rezultatiMap[r.Gimnasticar].DvovisinskiRazbojDrugoKolo = r.DvovisinskiRazboj;
                        rezultatiMap[r.Gimnasticar].GredaDrugoKolo = r.Greda;
                        rezultatiMap[r.Gimnasticar].TotalDrugoKolo = r.Total;
                    }
                }
            }

            if (poredakTreceKolo != null)
            {
                foreach (RezultatUkupno r in poredakTreceKolo.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Gimnasticar))
                    {
                        rezultatiMap[r.Gimnasticar].ParterTreceKolo = r.Parter;
                        rezultatiMap[r.Gimnasticar].KonjTreceKolo = r.Konj;
                        rezultatiMap[r.Gimnasticar].KarikeTreceKolo = r.Karike;
                        rezultatiMap[r.Gimnasticar].PreskokTreceKolo = r.Preskok;
                        rezultatiMap[r.Gimnasticar].RazbojTreceKolo = r.Razboj;
                        rezultatiMap[r.Gimnasticar].VratiloTreceKolo = r.Vratilo;
                        rezultatiMap[r.Gimnasticar].DvovisinskiRazbojTreceKolo = r.DvovisinskiRazboj;
                        rezultatiMap[r.Gimnasticar].GredaTreceKolo = r.Greda;
                        rezultatiMap[r.Gimnasticar].TotalTreceKolo = r.Total;
                    }
                }
            }

            if (poredakCetvrtoKolo != null)
            {
                foreach (RezultatUkupno r in poredakCetvrtoKolo.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Gimnasticar))
                    {
                        rezultatiMap[r.Gimnasticar].ParterCetvrtoKolo = r.Parter;
                        rezultatiMap[r.Gimnasticar].KonjCetvrtoKolo = r.Konj;
                        rezultatiMap[r.Gimnasticar].KarikeCetvrtoKolo = r.Karike;
                        rezultatiMap[r.Gimnasticar].PreskokCetvrtoKolo = r.Preskok;
                        rezultatiMap[r.Gimnasticar].RazbojCetvrtoKolo = r.Razboj;
                        rezultatiMap[r.Gimnasticar].VratiloCetvrtoKolo = r.Vratilo;
                        rezultatiMap[r.Gimnasticar].DvovisinskiRazbojCetvrtoKolo = r.DvovisinskiRazboj;
                        rezultatiMap[r.Gimnasticar].GredaCetvrtoKolo = r.Greda;
                        rezultatiMap[r.Gimnasticar].TotalCetvrtoKolo = r.Total;
                    }
                }
            }

            List<RezultatUkupnoZbirViseKola> rezultati = new List<RezultatUkupnoZbirViseKola>(rezultatiMap.Values);
            Rezultati.Clear();
            foreach (RezultatUkupnoZbirViseKola rez in rezultati)
                Rezultati.Add(rez);

            // Total moze da bude krajnja finalna ocena ili ulazna finalna ocena. U oba slucaja se Total izracunava
            // na isti nacin.
            foreach (RezultatUkupnoZbirViseKola rez in Rezultati)
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

                /*if (rezTak.Propozicije.Tak2FinalnaOcenaJeZbirObaKola)
                    total = total1 + total2;
                else if (rezTak.Propozicije.Tak2FinalnaOcenaJeMaxObaKola)
                    total = total1 > total2 ? total1 : total2;
                else
                {
                    // TODO3: Proveri da li ovde treba podesavati broj decimala.
                    total = (total1 + total2) / 2;
                    if (rezTak.Propozicije.Tak2NeRacunajProsekAkoNemaOceneIzObaKola
                        && (rez.TotalPrvoKolo == null || rez.TotalDrugoKolo == null))
                    {
                        total = total1 + total2;
                    }
                }*/
                total = total1 + total2 + total3 + total4;
                rez.setTotal(total);
            }

            rankRezultati();
            /*if (rezTak.Propozicije.OdvojenoTak2)
                updateKvalStatus(rezTak.Propozicije.BrojFinalistaTak2,
                                 rezTak.Propozicije.NeogranicenBrojTakmicaraIzKlubaTak2,
                                 rezTak.Propozicije.MaxBrojTakmicaraIzKlubaTak2,
                                 rezTak.Propozicije.BrojRezerviTak2);*/
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

        /*private void updateKvalStatus(int brojFinalista, 
            bool neogranicenBrojTakmicaraIzKluba, int maxBrojTakmicaraIzKluba, 
            int brojRezervi)
        {
            // TODO: Obradi situaciju kada ima npr. 24 finalista, a 24 i 25 takmicar
            // imaju isti total (uradi i za PoredakSprava i PoredakEkipno)

            List<RezultatUkupnoZbirViseKola> rezultati = new List<RezultatUkupnoZbirViseKola>(Rezultati);
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatUkupnoZbirViseKola))["RedBroj"];
            rezultati.Sort(new SortComparer<RezultatUkupnoZbirViseKola>(propDesc,
                ListSortDirection.Ascending));

            // moram da koristim dve mape zato sto je moguca situacija da klub i 
            // drzava imaju isti id
            IDictionary<int, int> brojTakmicaraKlubMap = new Dictionary<int, int>();
            IDictionary<int, int> brojTakmicaraDrzavaMap = new Dictionary<int, int>();

            int finCount = 0;
            int rezCount = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                RezultatUkupnoZbirViseKola rezulat = rezultati[i];
                if (rezulat.Total == null)
                {
                    rezulat.KvalStatus = KvalifikacioniStatus.None;
                    continue;
                }

                int id;
                IDictionary<int, int> brojTakmicaraMap;

                if (rezulat.Gimnasticar.KlubUcesnik != null)
                {
                    id = rezulat.Gimnasticar.KlubUcesnik.Id;
                    brojTakmicaraMap = brojTakmicaraKlubMap;
                }
                else
                {
                    id = rezulat.Gimnasticar.DrzavaUcesnik.Id;
                    brojTakmicaraMap = brojTakmicaraDrzavaMap;
                }

                if (!brojTakmicaraMap.ContainsKey(id))
                    brojTakmicaraMap.Add(id, 0);

                if (finCount < brojFinalista)
                {
                    if (neogranicenBrojTakmicaraIzKluba)
                    {
                        finCount++;
                        rezulat.KvalStatus = KvalifikacioniStatus.Q;
                    }
                    else
                    {
                        if (brojTakmicaraMap[id] < maxBrojTakmicaraIzKluba)
                        {
                            finCount++;
                            brojTakmicaraMap[id]++;
                            rezulat.KvalStatus = KvalifikacioniStatus.Q;
                        }
                        else
                        {
                            if (Opcije.Instance.UzimajPrvuSlobodnuRezervu
                            && rezCount < brojRezervi)
                            {
                                rezCount++;
                                rezulat.KvalStatus = KvalifikacioniStatus.R;
                            }
                            else
                                rezulat.KvalStatus = KvalifikacioniStatus.None;
                        }
                    }
                }
                else if (rezCount < brojRezervi)
                {
                    if (neogranicenBrojTakmicaraIzKluba)
                    {
                        rezCount++;
                        rezulat.KvalStatus = KvalifikacioniStatus.R;
                    }
                    else
                    {
                        if (brojTakmicaraMap[id] < maxBrojTakmicaraIzKluba)
                        {
                            rezCount++;
                            brojTakmicaraMap[id]++;
                            rezulat.KvalStatus = KvalifikacioniStatus.R;
                        }
                        else
                        {
                            if (Opcije.Instance.UzimajPrvuSlobodnuRezervu
                            && rezCount < brojRezervi)
                            {
                                rezCount++;
                                rezulat.KvalStatus = KvalifikacioniStatus.R;
                            }
                            else
                                rezulat.KvalStatus = KvalifikacioniStatus.None;
                        }
                    }
                }
                else
                {
                    rezulat.KvalStatus = KvalifikacioniStatus.None;
                }
            }
        }*/

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
