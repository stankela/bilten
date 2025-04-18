using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Bilten.Util;
using System.IO;

namespace Bilten.Domain
{
    public class PoredakSpravaFinaleKupa : DomainObject
    {
        private Sprava _sprava;
        public virtual Sprava Sprava
        {
            get { return _sprava; }
            set { _sprava = value; }
        }

        private IList<RezultatSpravaFinaleKupa> _rezultati = new List<RezultatSpravaFinaleKupa>();
        public virtual IList<RezultatSpravaFinaleKupa> Rezultati
        {
            get { return _rezultati; }
            protected set { _rezultati = value; }
        }

        public PoredakSpravaFinaleKupa()
        {

        }

        public virtual void create(RezultatskoTakmicenje rezTak, RezultatskoTakmicenje rezTak1,
            RezultatskoTakmicenje rezTak2)
        {
            IDictionary<GimnasticarUcesnik, RezultatSpravaFinaleKupa> rezultatiMap =
                new Dictionary<GimnasticarUcesnik, RezultatSpravaFinaleKupa>();
            foreach (GimnasticarUcesnik g in rezTak.Takmicenje1.Gimnasticari)
            {
                RezultatSpravaFinaleKupa rezultat = new RezultatSpravaFinaleKupa();
                rezultat.Gimnasticar = g;
                rezultatiMap.Add(g, rezultat);
            }

            foreach (RezultatSprava r in rezTak1.Takmicenje1.getPoredakSprava(Sprava).Rezultati)
            {
                if (rezultatiMap.ContainsKey(r.Gimnasticar))
                    rezultatiMap[r.Gimnasticar].initPrvoKolo(r);
            }
            foreach (RezultatSprava r in rezTak2.Takmicenje1.getPoredakSprava(Sprava).Rezultati)
            {
                if (rezultatiMap.ContainsKey(r.Gimnasticar))
                    rezultatiMap[r.Gimnasticar].initDrugoKolo(r);
            }

            Rezultati.Clear();
            foreach (RezultatSpravaFinaleKupa r in rezultatiMap.Values)
            {
                Rezultati.Add(r);
            }
            calculateTotalAndRankRezultati(rezTak.Propozicije);
        }

        // TODO3: Za sve poretke (ukupno, sprava, ekipno, kako za finale kupa, tako i za obicna takmicenja) specifikuj
        // pravila razresavanja istih ocena.

        public static void rankRezultati(Propozicije propozicije, List<RezultatSpravaFinaleKupa> rezultati)
        {
            PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                TypeDescriptor.GetProperties(typeof(RezultatSpravaFinaleKupa))["Total"],
                TypeDescriptor.GetProperties(typeof(RezultatSpravaFinaleKupa))["PrezimeIme"]
            };
            ListSortDirection[] sortDir = new ListSortDirection[] {
                ListSortDirection.Descending,
                ListSortDirection.Ascending
            };
            rezultati.Sort(new SortComparer<RezultatSpravaFinaleKupa>(propDesc, sortDir));

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
            updateKvalStatus(propozicije, rezultati);
        }

        public virtual void rankRezultati(Propozicije propozicije)
        {
            rankRezultati(propozicije, new List<RezultatSpravaFinaleKupa>(Rezultati));

        }

        public static void updateKvalStatus(Propozicije propozicije, List<RezultatSpravaFinaleKupa> rezultati)
        {
            if (!propozicije.odvojenoTak3())
            {
                foreach (RezultatSpravaFinaleKupa r in rezultati)
                    r.KvalStatus = KvalifikacioniStatus.None;
                return;
            }

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatSpravaFinaleKupa))["RedBroj"];
            rezultati.Sort(new SortComparer<RezultatSpravaFinaleKupa>(propDesc,
                ListSortDirection.Ascending));

            // moram da koristim dve mape zato sto je moguca situacija da klub i 
            // drzava imaju isti id
            IDictionary<int, int> brojTakmicaraKlubMap = new Dictionary<int, int>();
            IDictionary<int, int> brojTakmicaraDrzavaMap = new Dictionary<int, int>();

            int finCount = 0;
            int rezCount = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                RezultatSpravaFinaleKupa rezultat = rezultati[i];
                if (rezultat.Total == null)
                {
                    rezultat.KvalStatus = KvalifikacioniStatus.None;
                    continue;
                }

                int id;
                IDictionary<int, int> brojTakmicaraMap;

                if (propozicije.MaxBrojTakmicaraTak3VaziZaDrzavu)
                {
                    if (rezultat.Gimnasticar.DrzavaUcesnik != null)
                    {
                        id = rezultat.Gimnasticar.DrzavaUcesnik.Id;
                        brojTakmicaraMap = brojTakmicaraDrzavaMap;
                    }
                    else
                    {
                        id = rezultat.Gimnasticar.KlubUcesnik.Id;
                        brojTakmicaraMap = brojTakmicaraKlubMap;
                    }
                }
                else
                {
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
                }

                if (!brojTakmicaraMap.ContainsKey(id))
                    brojTakmicaraMap.Add(id, 0);

                if (finCount < propozicije.BrojFinalistaTak3)
                {
                    if (propozicije.NeogranicenBrojTakmicaraIzKlubaTak3)
                    {
                        finCount++;
                        rezultat.KvalStatus = KvalifikacioniStatus.Q;
                    }
                    else
                    {
                        if (brojTakmicaraMap[id] < propozicije.MaxBrojTakmicaraIzKlubaTak3)
                        {
                            finCount++;
                            brojTakmicaraMap[id]++;
                            rezultat.KvalStatus = KvalifikacioniStatus.Q;
                        }
                        else
                        {
                            if (Opcije.Instance.UzimajPrvuSlobodnuRezervu
                            && rezCount < propozicije.BrojRezerviTak3)
                            {
                                rezCount++;
                                rezultat.KvalStatus = KvalifikacioniStatus.R;
                            }
                            else
                                rezultat.KvalStatus = KvalifikacioniStatus.None;
                        }
                    }
                }
                else if (rezCount < propozicije.BrojRezerviTak3)
                {
                    if (propozicije.NeogranicenBrojTakmicaraIzKlubaTak3)
                    {
                        rezCount++;
                        rezultat.KvalStatus = KvalifikacioniStatus.R;
                    }
                    else
                    {
                        if (brojTakmicaraMap[id] < propozicije.MaxBrojTakmicaraIzKlubaTak3)
                        {
                            rezCount++;
                            brojTakmicaraMap[id]++;
                            rezultat.KvalStatus = KvalifikacioniStatus.R;
                        }
                        else
                        {
                            if (Opcije.Instance.UzimajPrvuSlobodnuRezervu
                            && rezCount < propozicije.BrojRezerviTak3)
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

        private void updateKvalStatus(Propozicije propozicije)
        {
            updateKvalStatus(propozicije, new List<RezultatSpravaFinaleKupa>(Rezultati));
        }

        public virtual void calculateTotalAndRankRezultati(Propozicije propozicije)
        {
            foreach (RezultatSpravaFinaleKupa r in Rezultati)
                r.calculateTotal(propozicije.NacinRacunanjaOceneFinaleKupaTak3);
            rankRezultati(propozicije);
        }

        public virtual void addGimnasticar(GimnasticarUcesnik g, RezultatskoTakmicenje rezTak,
            RezultatskoTakmicenje rezTak1, RezultatskoTakmicenje rezTak2)
        {
            RezultatSpravaFinaleKupa rezultat = new RezultatSpravaFinaleKupa();
            rezultat.Gimnasticar = g;

            foreach (RezultatSprava r in rezTak1.Takmicenje1.getPoredakSprava(Sprava).Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                {
                    rezultat.initPrvoKolo(r);
                    break;
                }
            }
            foreach (RezultatSprava r in rezTak2.Takmicenje1.getPoredakSprava(Sprava).Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                {
                    rezultat.initDrugoKolo(r);
                    break;
                }
            }

            rezultat.calculateTotal(rezTak.Propozicije.NacinRacunanjaOceneFinaleKupaTak3);
            Rezultati.Add(rezultat);
            rankRezultati(rezTak.Propozicije);
        }

        public virtual void deleteGimnasticar(GimnasticarUcesnik g, RezultatskoTakmicenje rezTak)
        {
            RezultatSpravaFinaleKupa r = getRezultat(g);
            if (r != null)
            {
                Rezultati.Remove(r);
                rankRezultati(rezTak.Propozicije);
            }
        }

        private RezultatSpravaFinaleKupa getRezultat(GimnasticarUcesnik g)
        {
            foreach (RezultatSpravaFinaleKupa r in Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                    return r;
            }
            return null;
        }

        public virtual List<RezultatSpravaFinaleKupa> getRezultati()
        {
            List<RezultatSpravaFinaleKupa> result = new List<RezultatSpravaFinaleKupa>(Rezultati);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatSpravaFinaleKupa))["RedBroj"];
            result.Sort(new SortComparer<RezultatSpravaFinaleKupa>(propDesc, ListSortDirection.Ascending));

            return result;
        }

        public virtual IList<GimnasticarUcesnik> getKvalifikanti()
        {
            IList<GimnasticarUcesnik> result = new List<GimnasticarUcesnik>();
            foreach (RezultatSpravaFinaleKupa r in getRezultati())
            {
                if (r.KvalStatus == KvalifikacioniStatus.Q)
                    result.Add(r.Gimnasticar);
            }
            return result;
        }

        // NOTE: Nisu implementirani Equals i GetHashCode (iako se PoredakSpravaFinaleKupa cuva u setovima) zato sto je
        // podrazumevani Equals dovoljan.

        public virtual void dumpRezultati(StreamWriter streamWriter)
        {
            string header = Sprave.toString(Sprava).ToUpper() + " - FINALE";
            streamWriter.WriteLine(header);
            foreach (RezultatSpravaFinaleKupa r in getRezultati())
            {
                string line = r.Rank + ". " + r.Gimnasticar.ImeSrednjeImePrezimeDatumRodjenja + "   " + r.Total;
                streamWriter.WriteLine(line);
            }
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(Sprava.ToString());

            strBuilder.AppendLine(Rezultati.Count.ToString());
            foreach (RezultatSpravaFinaleKupa r in Rezultati)
                r.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            Sprava = (Sprava)Enum.Parse(typeof(Sprava), reader.ReadLine());

            int brojRezultata = int.Parse(reader.ReadLine());
            for (int i = 0; i < brojRezultata; ++i)
            {
                reader.ReadLine();  // id
                RezultatSpravaFinaleKupa r = new RezultatSpravaFinaleKupa();
                r.loadFromDump(reader, map);
                Rezultati.Add(r);
            }
        }
    }
}
