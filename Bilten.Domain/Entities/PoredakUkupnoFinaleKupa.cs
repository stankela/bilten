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

            List<RezultatUkupnoFinaleKupa> rezultati = new List<RezultatUkupnoFinaleKupa>(rezultatiMap.Values);
            Rezultati.Clear();
            foreach (RezultatUkupnoFinaleKupa rez in rezultati)
            {
                // Total moze da bude krajnja finalna ocena ili ulazna finalna ocena. U oba slucaja se Total izracunava
                // na isti nacin.
                rez.calculateTotal(rezTak.Propozicije.NacinRacunanjaOceneFinaleKupaTak2,
                    rezTak.Propozicije.Tak2NeRacunajProsekAkoNemaOceneIzObaKola);
                Rezultati.Add(rez);
            }

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
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

        private void updateKvalStatus(Propozicije propozicije)
        {
            if (!propozicije.PostojiTak2 || !propozicije.OdvojenoTak2)
            {
                foreach (RezultatUkupnoFinaleKupa r in Rezultati)
                    r.KvalStatus = KvalifikacioniStatus.None;
                return;
            }

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

                if (finCount < propozicije.BrojFinalistaTak2)
                {
                    if (propozicije.NeogranicenBrojTakmicaraIzKlubaTak2)
                    {
                        finCount++;
                        rezulat.KvalStatus = KvalifikacioniStatus.Q;
                    }
                    else
                    {
                        if (brojTakmicaraMap[id] < propozicije.MaxBrojTakmicaraIzKlubaTak2)
                        {
                            finCount++;
                            brojTakmicaraMap[id]++;
                            rezulat.KvalStatus = KvalifikacioniStatus.Q;
                        }
                        else
                        {
                            if (Opcije.Instance.UzimajPrvuSlobodnuRezervu
                            && rezCount < propozicije.BrojRezerviTak2)
                            {
                                rezCount++;
                                rezulat.KvalStatus = KvalifikacioniStatus.R;
                            }
                            else
                                rezulat.KvalStatus = KvalifikacioniStatus.None;
                        }
                    }
                }
                else if (rezCount < propozicije.BrojRezerviTak2)
                {
                    if (propozicije.NeogranicenBrojTakmicaraIzKlubaTak2)
                    {
                        rezCount++;
                        rezulat.KvalStatus = KvalifikacioniStatus.R;
                    }
                    else
                    {
                        if (brojTakmicaraMap[id] < propozicije.MaxBrojTakmicaraIzKlubaTak2)
                        {
                            rezCount++;
                            brojTakmicaraMap[id]++;
                            rezulat.KvalStatus = KvalifikacioniStatus.R;
                        }
                        else
                        {
                            if (Opcije.Instance.UzimajPrvuSlobodnuRezervu
                            && rezCount < propozicije.BrojRezerviTak2)
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

        public virtual void addGimnasticar(GimnasticarUcesnik g, RezultatskoTakmicenje rezTak,
            PoredakUkupno poredakPrvoKolo, PoredakUkupno poredakDrugoKolo)
        {
            RezultatUkupnoFinaleKupa rezultat = new RezultatUkupnoFinaleKupa();
            rezultat.Gimnasticar = g;

            foreach (RezultatUkupno r in poredakPrvoKolo.Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                {
                    rezultat.ParterPrvoKolo = r.Parter;
                    rezultat.KonjPrvoKolo = r.Konj;
                    rezultat.KarikePrvoKolo = r.Karike;
                    rezultat.PreskokPrvoKolo = r.Preskok;
                    rezultat.RazbojPrvoKolo = r.Razboj;
                    rezultat.VratiloPrvoKolo = r.Vratilo;
                    rezultat.DvovisinskiRazbojPrvoKolo = r.DvovisinskiRazboj;
                    rezultat.GredaPrvoKolo = r.Greda;
                    rezultat.TotalPrvoKolo = r.Total;
                    break;
                }
            }

            foreach (RezultatUkupno r in poredakDrugoKolo.Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                {
                    rezultat.ParterDrugoKolo = r.Parter;
                    rezultat.KonjDrugoKolo = r.Konj;
                    rezultat.KarikeDrugoKolo = r.Karike;
                    rezultat.PreskokDrugoKolo = r.Preskok;
                    rezultat.RazbojDrugoKolo = r.Razboj;
                    rezultat.VratiloDrugoKolo = r.Vratilo;
                    rezultat.DvovisinskiRazbojDrugoKolo = r.DvovisinskiRazboj;
                    rezultat.GredaDrugoKolo = r.Greda;
                    rezultat.TotalDrugoKolo = r.Total;
                    break;
                }
            }
            rezultat.calculateTotal(rezTak.Propozicije.NacinRacunanjaOceneFinaleKupaTak2,
                rezTak.Propozicije.Tak2NeRacunajProsekAkoNemaOceneIzObaKola);
            Rezultati.Add(rezultat);

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
        }

        public virtual List<RezultatUkupnoFinaleKupa> getRezultati()
        {
            List<RezultatUkupnoFinaleKupa> result = new List<RezultatUkupnoFinaleKupa>(Rezultati);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatUkupnoFinaleKupa))["RedBroj"];
            result.Sort(new SortComparer<RezultatUkupnoFinaleKupa>(propDesc, ListSortDirection.Ascending));

            return result;
        }

        private RezultatUkupnoFinaleKupa getRezultat(GimnasticarUcesnik g)
        {
            foreach (RezultatUkupnoFinaleKupa r in Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                    return r;
            }
            return null;
        }

        public virtual void deleteGimnasticar(GimnasticarUcesnik g, RezultatskoTakmicenje rezTak)
        {
            RezultatUkupnoFinaleKupa r = getRezultat(g);
            if (r != null)
            {
                Rezultati.Remove(r);
                rankRezultati();
                updateKvalStatus(rezTak.Propozicije);
            }
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
