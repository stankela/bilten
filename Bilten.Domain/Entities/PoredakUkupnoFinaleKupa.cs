using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Bilten.Util;
using System.IO;

namespace Bilten.Domain
{
    public class PoredakUkupnoFinaleKupa : DomainObject
    {
        private DeoTakmicenjaKod deoTakKod;
        public virtual DeoTakmicenjaKod DeoTakmicenjaKod
        {
            get { return deoTakKod; }
            set { deoTakKod = value; }
        }

        private IList<RezultatUkupnoFinaleKupa> _rezultati = new List<RezultatUkupnoFinaleKupa>();
        public virtual IList<RezultatUkupnoFinaleKupa> Rezultati
        {
            get { return _rezultati; }
            protected set { _rezultati = value; }
        }

        public PoredakUkupnoFinaleKupa()
        {
            DeoTakmicenjaKod = DeoTakmicenjaKod.Takmicenje1;
        }

        public virtual void create(RezultatskoTakmicenje rezTak, PoredakUkupno poredakPrvoKolo,
            PoredakUkupno poredakDrugoKolo)
        {
            IList<GimnasticarUcesnik> gimnasticari = new List<GimnasticarUcesnik>(rezTak.Takmicenje1.Gimnasticari);

            // NOTE: Da bi GimnasticarUcesnik mogao da se koristi kao key u Dictionary, mora da implementira
            // interfejs IEquatable<GimnasticarUcesnik>.
            IDictionary<GimnasticarUcesnik, RezultatUkupnoFinaleKupa> rezultatiMap =
                new Dictionary<GimnasticarUcesnik, RezultatUkupnoFinaleKupa>();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                RezultatUkupnoFinaleKupa rezultat = new RezultatUkupnoFinaleKupa();
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

            List<RezultatUkupnoFinaleKupa> rezultati = new List<RezultatUkupnoFinaleKupa>(rezultatiMap.Values);
            Rezultati.Clear();
            foreach (RezultatUkupnoFinaleKupa rez in rezultati)
                Rezultati.Add(rez);

            // Total moze da bude krajnja finalna ocena ili ulazna finalna ocena. U oba slucaja se Total izracunava
            // na isti nacin.
            foreach (RezultatUkupnoFinaleKupa rez in Rezultati)
            {
                if (rez.TotalPrvoKolo == null && rez.TotalDrugoKolo == null)
                {
                    rez.setTotal(null);
                    continue;
                }
                float total1 = rez.TotalPrvoKolo == null ? 0 : rez.TotalPrvoKolo.Value;
                float total2 = rez.TotalDrugoKolo == null ? 0 : rez.TotalDrugoKolo.Value;
                float total;

                if (rezTak.Propozicije.Tak2FinalnaOcenaJeZbirObaKola)
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
                }
                rez.setTotal(total);
            }

            rankRezultati();
            if (rezTak.Propozicije.OdvojenoTak2)
                updateKvalStatus(rezTak.Propozicije.BrojFinalistaTak2,
                                 rezTak.Propozicije.NeogranicenBrojTakmicaraIzKlubaTak2,
                                 rezTak.Propozicije.MaxBrojTakmicaraIzKlubaTak2,
                                 rezTak.Propozicije.BrojRezerviTak2);
        }

        private void rankRezultati()
        {
            List<RezultatUkupnoFinaleKupa> rezultati = new List<RezultatUkupnoFinaleKupa>(Rezultati);

            PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                TypeDescriptor.GetProperties(typeof(RezultatUkupnoFinaleKupa))["Total"],
                TypeDescriptor.GetProperties(typeof(RezultatUkupnoFinaleKupa))["PrezimeIme"]
            };
            ListSortDirection[] sortDir = new ListSortDirection[] {
                ListSortDirection.Descending,
                ListSortDirection.Ascending
            };
            rezultati.Sort(new SortComparer<RezultatUkupnoFinaleKupa>(propDesc, sortDir));

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

        private void updateKvalStatus(int brojFinalista, 
            bool neogranicenBrojTakmicaraIzKluba, int maxBrojTakmicaraIzKluba, 
            int brojRezervi)
        {
            // TODO: Obradi situaciju kada ima npr. 24 finalista, a 24 i 25 takmicar
            // imaju isti total (uradi i za PoredakSprava i PoredakEkipno)

            List<RezultatUkupnoFinaleKupa> rezultati = new List<RezultatUkupnoFinaleKupa>(Rezultati);
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatUkupnoFinaleKupa))["RedBroj"];
            rezultati.Sort(new SortComparer<RezultatUkupnoFinaleKupa>(propDesc,
                ListSortDirection.Ascending));

            // moram da koristim dve mape zato sto je moguca situacija da klub i 
            // drzava imaju isti id
            IDictionary<int, int> brojTakmicaraKlubMap = new Dictionary<int, int>();
            IDictionary<int, int> brojTakmicaraDrzavaMap = new Dictionary<int, int>();

            int finCount = 0;
            int rezCount = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                RezultatUkupnoFinaleKupa rezulat = rezultati[i];
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
        }

        public virtual List<RezultatUkupnoFinaleKupa> getRezultati()
        {
            List<RezultatUkupnoFinaleKupa> result = new List<RezultatUkupnoFinaleKupa>(Rezultati);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatUkupnoFinaleKupa))["RedBroj"];
            result.Sort(new SortComparer<RezultatUkupnoFinaleKupa>(propDesc, ListSortDirection.Ascending));

            return result;
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(DeoTakmicenjaKod.ToString());

            strBuilder.AppendLine(Rezultati.Count.ToString());
            foreach (RezultatUkupnoFinaleKupa r in Rezultati)
                r.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            DeoTakmicenjaKod = (DeoTakmicenjaKod)Enum.Parse(typeof(DeoTakmicenjaKod), reader.ReadLine());
            
            int brojRezultata = int.Parse(reader.ReadLine());
            for (int i = 0; i < brojRezultata; ++i)
            {
                reader.ReadLine();  // id
                RezultatUkupnoFinaleKupa r = new RezultatUkupnoFinaleKupa();
                r.loadFromDump(reader, map);
                Rezultati.Add(r);
            }
        }
    }
}
