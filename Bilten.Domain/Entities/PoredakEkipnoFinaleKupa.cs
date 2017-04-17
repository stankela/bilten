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
            // NOTE: Da bi Ekipa mogla da se koristi kao key u Dictionary, mora da implementira
            // interfejs IEquatable<Ekipa>.
            IDictionary<Ekipa, RezultatEkipnoFinaleKupa> rezultatiMap =
                new Dictionary<Ekipa, RezultatEkipnoFinaleKupa>();
            foreach (Ekipa e in rezTak.Takmicenje1.Ekipe)
            {
                RezultatEkipnoFinaleKupa rezultat = new RezultatEkipnoFinaleKupa();
                rezultat.Ekipa = e;
                rezultatiMap.Add(e, rezultat);
            }

            foreach (RezultatEkipno r in rezTak1.Takmicenje1.PoredakEkipno.Rezultati)
            {
                if (rezultatiMap.ContainsKey(r.Ekipa))
                    rezultatiMap[r.Ekipa].initPrvoKolo(r);
            }
            foreach (RezultatEkipno r in rezTak2.Takmicenje1.PoredakEkipno.Rezultati)
            {
                if (rezultatiMap.ContainsKey(r.Ekipa))
                    rezultatiMap[r.Ekipa].initDrugoKolo(r);
            }

            Rezultati.Clear();
            foreach (RezultatEkipnoFinaleKupa r in rezultatiMap.Values)
            {
                r.calculateTotal(rezTak.Propozicije.NacinRacunanjaOceneFinaleKupaTak4,
                    rezTak.Propozicije.Tak4NeRacunajProsekAkoNemaOceneIzObaKola);
                Rezultati.Add(r);
            }

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
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

        private void updateKvalStatus(Propozicije propozicije)
        {
            if (!propozicije.odvojenoTak4())
            {
                foreach (RezultatEkipnoFinaleKupa r in Rezultati)
                    r.KvalStatus = KvalifikacioniStatus.None;
                return;
            }

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

                if (finCount < propozicije.BrojEkipaUFinalu)
                {
                    finCount++;
                    rezultat.KvalStatus = KvalifikacioniStatus.Q;
                }
                else if (rezCount < 0 /*brojRezervi*/)
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

        public virtual void addEkipa(Ekipa e, RezultatskoTakmicenje rezTak, RezultatskoTakmicenje rezTak1,
            RezultatskoTakmicenje rezTak2)
        {
            RezultatEkipnoFinaleKupa rezultat = new RezultatEkipnoFinaleKupa();
            rezultat.Ekipa = e;

            foreach (RezultatEkipno r in rezTak1.Takmicenje1.PoredakEkipno.Rezultati)
            {
                if (r.Ekipa.Equals(e))
                {
                    rezultat.initPrvoKolo(r);
                    break;
                }
            }
            foreach (RezultatEkipno r in rezTak2.Takmicenje1.PoredakEkipno.Rezultati)
            {
                if (r.Ekipa.Equals(e))
                {
                    rezultat.initDrugoKolo(r);
                    break;
                }
            }

            rezultat.calculateTotal(rezTak.Propozicije.NacinRacunanjaOceneFinaleKupaTak4,
                rezTak.Propozicije.Tak4NeRacunajProsekAkoNemaOceneIzObaKola);
            Rezultati.Add(rezultat);

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
        }

        public virtual void deleteEkipa(Ekipa e, RezultatskoTakmicenje rezTak)
        {
            RezultatEkipnoFinaleKupa r = getRezultat(e);
            if (r != null)
            {
                Rezultati.Remove(r);
                rankRezultati();
                updateKvalStatus(rezTak.Propozicije);
            }
        }

        private RezultatEkipnoFinaleKupa getRezultat(Ekipa e)
        {
            foreach (RezultatEkipnoFinaleKupa r in Rezultati)
            {
                if (r.Ekipa.Equals(e))
                    return r;
            }
            return null;
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
