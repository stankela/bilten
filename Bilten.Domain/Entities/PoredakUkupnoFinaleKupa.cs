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

        public virtual void create(RezultatskoTakmicenje rezTak, RezultatskoTakmicenje rezTak1,
            RezultatskoTakmicenje rezTak2)
        {
            // NOTE: Da bi GimnasticarUcesnik mogao da se koristi kao key u Dictionary, mora da implementira
            // interfejs IEquatable<GimnasticarUcesnik>.
            IDictionary<GimnasticarUcesnik, RezultatUkupnoFinaleKupa> rezultatiMap =
                new Dictionary<GimnasticarUcesnik, RezultatUkupnoFinaleKupa>();
            foreach (GimnasticarUcesnik g in rezTak.Takmicenje1.Gimnasticari)
            {
                RezultatUkupnoFinaleKupa rezultat = new RezultatUkupnoFinaleKupa();
                rezultat.Gimnasticar = g;
                rezultatiMap.Add(g, rezultat);
            }

            foreach (RezultatUkupno r in rezTak1.Takmicenje1.PoredakUkupno.Rezultati)
            {
                if (rezultatiMap.ContainsKey(r.Gimnasticar))
                    rezultatiMap[r.Gimnasticar].initPrvoKolo(r);
            }
            foreach (RezultatUkupno r in rezTak2.Takmicenje1.PoredakUkupno.Rezultati)
            {
                if (rezultatiMap.ContainsKey(r.Gimnasticar))
                    rezultatiMap[r.Gimnasticar].initDrugoKolo(r);
            }

            Rezultati.Clear();
            foreach (RezultatUkupnoFinaleKupa r in rezultatiMap.Values)
            {
                // Total moze da bude krajnja finalna ocena ili ulazna finalna ocena. U oba slucaja se Total izracunava
                // na isti nacin.
                r.calculateTotal(rezTak.Propozicije.NacinRacunanjaOceneFinaleKupaTak2,
                    rezTak.Propozicije.Tak2NeRacunajProsekAkoNemaOceneIzObaKola);
                Rezultati.Add(r);
            }
            rankRezultati(rezTak.Propozicije);
        }

        public virtual void rankRezultati(Propozicije propozicije)
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
            updateKvalStatus(propozicije);
        }

        private void updateKvalStatus(Propozicije propozicije)
        {
            if (!propozicije.odvojenoTak2())
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
                RezultatUkupnoFinaleKupa rezultat = rezultati[i];
                if (rezultat.Total == null)
                {
                    rezultat.KvalStatus = KvalifikacioniStatus.None;
                    continue;
                }

                int id;
                IDictionary<int, int> brojTakmicaraMap;

                if (rezultat.Gimnasticar.KlubUcesnik != null)
                {
                    id = rezultat.Gimnasticar.KlubUcesnik.Id;
                    brojTakmicaraMap = brojTakmicaraKlubMap;
                }
                else
                {
                    id = rezultat.Gimnasticar.DrzavaUcesnik.Id;
                    brojTakmicaraMap = brojTakmicaraDrzavaMap;
                }

                if (!brojTakmicaraMap.ContainsKey(id))
                    brojTakmicaraMap.Add(id, 0);

                if (finCount < propozicije.BrojFinalistaTak2)
                {
                    if (propozicije.NeogranicenBrojTakmicaraIzKlubaTak2)
                    {
                        finCount++;
                        rezultat.KvalStatus = KvalifikacioniStatus.Q;
                    }
                    else
                    {
                        if (brojTakmicaraMap[id] < propozicije.MaxBrojTakmicaraIzKlubaTak2)
                        {
                            finCount++;
                            brojTakmicaraMap[id]++;
                            rezultat.KvalStatus = KvalifikacioniStatus.Q;
                        }
                        else
                        {
                            if (Opcije.Instance.UzimajPrvuSlobodnuRezervu
                            && rezCount < propozicije.BrojRezerviTak2)
                            {
                                rezCount++;
                                rezultat.KvalStatus = KvalifikacioniStatus.R;
                            }
                            else
                                rezultat.KvalStatus = KvalifikacioniStatus.None;
                        }
                    }
                }
                else if (rezCount < propozicije.BrojRezerviTak2)
                {
                    if (propozicije.NeogranicenBrojTakmicaraIzKlubaTak2)
                    {
                        rezCount++;
                        rezultat.KvalStatus = KvalifikacioniStatus.R;
                    }
                    else
                    {
                        if (brojTakmicaraMap[id] < propozicije.MaxBrojTakmicaraIzKlubaTak2)
                        {
                            rezCount++;
                            brojTakmicaraMap[id]++;
                            rezultat.KvalStatus = KvalifikacioniStatus.R;
                        }
                        else
                        {
                            if (Opcije.Instance.UzimajPrvuSlobodnuRezervu
                            && rezCount < propozicije.BrojRezerviTak2)
                            {
                                rezCount++;
                                rezultat.KvalStatus = KvalifikacioniStatus.R;
                            }
                            else
                                rezultat.KvalStatus = KvalifikacioniStatus.None;
                        }
                    }
                }
                else
                {
                    rezultat.KvalStatus = KvalifikacioniStatus.None;
                }
            }
        }

        public virtual void calculateTotal(Propozicije propozicije)
        {
            foreach (RezultatUkupnoFinaleKupa r in Rezultati)
            {
                r.calculateTotal(propozicije.NacinRacunanjaOceneFinaleKupaTak2,
                    propozicije.Tak2NeRacunajProsekAkoNemaOceneIzObaKola);
            }
            rankRezultati(propozicije);
        }

        public virtual void addGimnasticar(GimnasticarUcesnik g, RezultatskoTakmicenje rezTak,
            RezultatskoTakmicenje rezTak1, RezultatskoTakmicenje rezTak2)
        {
            RezultatUkupnoFinaleKupa rezultat = new RezultatUkupnoFinaleKupa();
            rezultat.Gimnasticar = g;

            foreach (RezultatUkupno r in rezTak1.Takmicenje1.PoredakUkupno.Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                {
                    rezultat.initPrvoKolo(r);
                    break;
                }
            }
            foreach (RezultatUkupno r in rezTak2.Takmicenje1.PoredakUkupno.Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                {
                    rezultat.initDrugoKolo(r);
                    break;
                }
            }
            rezultat.calculateTotal(rezTak.Propozicije.NacinRacunanjaOceneFinaleKupaTak2,
                rezTak.Propozicije.Tak2NeRacunajProsekAkoNemaOceneIzObaKola);
            Rezultati.Add(rezultat);
            rankRezultati(rezTak.Propozicije);
        }

        public virtual void deleteGimnasticar(GimnasticarUcesnik g, RezultatskoTakmicenje rezTak)
        {
            RezultatUkupnoFinaleKupa r = getRezultat(g);
            if (r != null)
            {
                Rezultati.Remove(r);
                rankRezultati(rezTak.Propozicije);
            }
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
