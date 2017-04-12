using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Bilten.Util;
using System.IO;

namespace Bilten.Domain
{
    public class PoredakEkipnoFinaleKupa : DomainObject
    {
        private DeoTakmicenjaKod deoTakKod;
        public virtual DeoTakmicenjaKod DeoTakmicenjaKod
        {
            get { return deoTakKod; }
            set { deoTakKod = value; }
        }

        private IList<RezultatEkipnoFinaleKupa> _rezultati = new List<RezultatEkipnoFinaleKupa>();
        public virtual IList<RezultatEkipnoFinaleKupa> Rezultati
        {
            get { return _rezultati; }
            protected set { _rezultati = value; }
        }

        public PoredakEkipnoFinaleKupa()
        {
            DeoTakmicenjaKod = DeoTakmicenjaKod.Takmicenje1;
        }

        public virtual void create(RezultatskoTakmicenje rezTak, RezultatskoTakmicenje rezTak1,
            RezultatskoTakmicenje rezTak2)
        {
            IList<Ekipa> ekipe = new List<Ekipa>(rezTak.Takmicenje1.Ekipe);

            // NOTE: Da bi Ekipa mogla da se koristi kao key u Dictionary, mora da implementira
            // interfejs IEquatable<Ekipa>.
            IDictionary<Ekipa, RezultatEkipnoFinaleKupa> rezultatiMap =
                new Dictionary<Ekipa, RezultatEkipnoFinaleKupa>();
            foreach (Ekipa e in ekipe)
            {
                RezultatEkipnoFinaleKupa rezultat = new RezultatEkipnoFinaleKupa();
                rezultat.Ekipa = e;
                rezultatiMap.Add(e, rezultat);
            }

            foreach (RezultatEkipno r in rezTak1.Takmicenje1.PoredakEkipno.Rezultati)
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
            foreach (RezultatEkipno r in rezTak2.Takmicenje1.PoredakEkipno.Rezultati)
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

            List<RezultatEkipnoFinaleKupa> rezultati = new List<RezultatEkipnoFinaleKupa>(rezultatiMap.Values);
            Rezultati.Clear();
            foreach (RezultatEkipnoFinaleKupa rez in rezultati)
                Rezultati.Add(rez);

            // Total moze da bude krajnja finalna ocena ili ulazna finalna ocena. U oba slucaja se Total izracunava
            // na isti nacin.
            foreach (RezultatEkipnoFinaleKupa rez in Rezultati)
            {
                if (rez.TotalPrvoKolo == null && rez.TotalDrugoKolo == null)
                {
                    rez.setTotal(null);
                    continue;
                }
                float total1 = rez.TotalPrvoKolo == null ? 0 : rez.TotalPrvoKolo.Value;
                float total2 = rez.TotalDrugoKolo == null ? 0 : rez.TotalDrugoKolo.Value;
                float total;

                if (rezTak.Propozicije.Tak4FinalnaOcenaJeZbirObaKola)
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
                }
                rez.setTotal(total);
            }

            rankRezultati();
            if (rezTak.Propozicije.OdvojenoTak4)
                updateKvalStatus(rezTak.Propozicije.BrojEkipaUFinalu, 0);
        }

        private void rankRezultati()
        {
            List<RezultatEkipnoFinaleKupa> rezultati = new List<RezultatEkipnoFinaleKupa>(Rezultati);

            PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                TypeDescriptor.GetProperties(typeof(RezultatEkipnoFinaleKupa))["Total"],
                TypeDescriptor.GetProperties(typeof(RezultatEkipnoFinaleKupa))["NazivEkipe"]
            };
            ListSortDirection[] sortDir = new ListSortDirection[] {
                ListSortDirection.Descending,
                ListSortDirection.Ascending
            };
            rezultati.Sort(new SortComparer<RezultatEkipnoFinaleKupa>(propDesc, sortDir));

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

        private void updateKvalStatus(int brojFinalista, int brojRezervi)
        {
            List<RezultatEkipnoFinaleKupa> rezultati = new List<RezultatEkipnoFinaleKupa>(Rezultati);
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatEkipnoFinaleKupa))["RedBroj"];
            rezultati.Sort(new SortComparer<RezultatEkipnoFinaleKupa>(propDesc,
                ListSortDirection.Ascending));

            int finCount = 0;
            int rezCount = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                Rezultat rezultat = rezultati[i];
                if (rezultat.Total == null)
                {
                    rezultat.KvalStatus = KvalifikacioniStatus.None;
                    continue;
                }

                if (finCount < brojFinalista)
                {
                    finCount++;
                    rezultat.KvalStatus = KvalifikacioniStatus.Q;
                }
                else if (rezCount < brojRezervi)
                {
                    rezCount++;
                    rezultat.KvalStatus = KvalifikacioniStatus.R;
                }
                else
                {
                    rezultat.KvalStatus = KvalifikacioniStatus.None;
                }
            }
        }

        public virtual List<RezultatEkipnoFinaleKupa> getRezultati()
        {
            List<RezultatEkipnoFinaleKupa> result = new List<RezultatEkipnoFinaleKupa>(Rezultati);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatEkipnoFinaleKupa))["RedBroj"];
            result.Sort(new SortComparer<RezultatEkipnoFinaleKupa>(propDesc, ListSortDirection.Ascending));

            return result;
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(DeoTakmicenjaKod.ToString());

            strBuilder.AppendLine(Rezultati.Count.ToString());
            foreach (RezultatEkipnoFinaleKupa r in Rezultati)
                r.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            DeoTakmicenjaKod = (DeoTakmicenjaKod)Enum.Parse(typeof(DeoTakmicenjaKod), reader.ReadLine());
            
            int brojRezultata = int.Parse(reader.ReadLine());
            for (int i = 0; i < brojRezultata; ++i)
            {
                reader.ReadLine();  // id
                RezultatEkipnoFinaleKupa r = new RezultatEkipnoFinaleKupa();
                r.loadFromDump(reader, map);
                Rezultati.Add(r);
            }
        }
    }
}
