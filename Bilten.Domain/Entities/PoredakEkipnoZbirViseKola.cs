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

        public virtual void create(RezultatskoTakmicenje rezTak, RezultatskoTakmicenje rezTak1,
            RezultatskoTakmicenje rezTak2, RezultatskoTakmicenje rezTak3, RezultatskoTakmicenje rezTak4)
        {
            // NOTE: Da bi Ekipa mogla da se koristi kao key u Dictionary, mora da implementira
            // interfejs IEquatable<Ekipa>.
            IDictionary<Ekipa, RezultatEkipnoZbirViseKola> rezultatiMap =
                new Dictionary<Ekipa, RezultatEkipnoZbirViseKola>();
            foreach (Ekipa e in rezTak.Takmicenje1.Ekipe)
            {
                RezultatEkipnoZbirViseKola rezultat = new RezultatEkipnoZbirViseKola();
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
            if (rezTak3 != null)
            {
                foreach (RezultatEkipno r in rezTak3.Takmicenje1.PoredakEkipno.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Ekipa))
                        rezultatiMap[r.Ekipa].initTreceKolo(r);
                }
            }
            if (rezTak4 != null)
            {
                foreach (RezultatEkipno r in rezTak4.Takmicenje1.PoredakEkipno.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Ekipa))
                        rezultatiMap[r.Ekipa].initCetvrtoKolo(r);
                }
            }

            Rezultati.Clear();
            foreach (RezultatEkipnoZbirViseKola r in rezultatiMap.Values)
            {
                r.calculateTotal();
                Rezultati.Add(r);
            }
            rankRezultati();
        }

        public virtual void rankRezultati()
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

        public virtual void calculateTotal()
        {
            foreach (RezultatEkipnoZbirViseKola r in Rezultati)
                r.calculateTotal();
            rankRezultati();
        }

        public virtual List<RezultatEkipnoZbirViseKola> getRezultati()
        {
            List<RezultatEkipnoZbirViseKola> result = new List<RezultatEkipnoZbirViseKola>(Rezultati);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatEkipnoZbirViseKola))["RedBroj"];
            result.Sort(new SortComparer<RezultatEkipnoZbirViseKola>(propDesc, ListSortDirection.Ascending));

            return result;
        }

        public virtual void addEkipa(Ekipa e, RezultatskoTakmicenje rezTak, RezultatskoTakmicenje rezTak1,
            RezultatskoTakmicenje rezTak2, RezultatskoTakmicenje rezTak3, RezultatskoTakmicenje rezTak4)
        {
            RezultatEkipnoZbirViseKola rezultat = new RezultatEkipnoZbirViseKola();
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
            if (rezTak3 != null)
            {
                foreach (RezultatEkipno r in rezTak3.Takmicenje1.PoredakEkipno.Rezultati)
                {
                    if (r.Ekipa.Equals(e))
                    {
                        rezultat.initTreceKolo(r);
                        break;
                    }
                }
            }
            if (rezTak4 != null)
            {
                foreach (RezultatEkipno r in rezTak4.Takmicenje1.PoredakEkipno.Rezultati)
                {
                    if (r.Ekipa.Equals(e))
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

        public virtual void deleteEkipa(Ekipa e, RezultatskoTakmicenje rezTak)
        {
            RezultatEkipnoZbirViseKola r = getRezultat(e);
            if (r != null)
            {
                Rezultati.Remove(r);
                rankRezultati();
            }
        }

        private RezultatEkipnoZbirViseKola getRezultat(Ekipa e)
        {
            foreach (RezultatEkipnoZbirViseKola r in Rezultati)
            {
                if (r.Ekipa.Equals(e))
                    return r;
            }
            return null;
        }

        public virtual void dumpRezultati(StreamWriter streamWriter)
        {
            streamWriter.WriteLine("EKIPE ZBIR VISE KOLA");
            foreach (RezultatEkipnoZbirViseKola r in getRezultati())
            {
                string line = r.Rank + ". " + r.Ekipa.Naziv + "   " + r.Total;
                streamWriter.WriteLine(line);
            }
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
