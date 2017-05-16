using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Bilten.Util;
using System.IO;

namespace Bilten.Domain
{
    public class PoredakSprava : DomainObject
    {
        public delegate bool IsTotalNull<T>(T rezultat) where T : RezultatSprava;

        private DeoTakmicenjaKod deoTakKod;
        public virtual DeoTakmicenjaKod DeoTakmicenjaKod
        {
            get { return deoTakKod; }
            set { deoTakKod = value; }
        }

        private Sprava _sprava;
        public virtual Sprava Sprava
        {
            get { return _sprava; }
            set { _sprava = value; }
        }

        private IList<RezultatSprava> _rezultati = new List<RezultatSprava>();
        public virtual IList<RezultatSprava> Rezultati
        {
            get { return _rezultati; }
            protected set { _rezultati = value; }
        }

        public PoredakSprava()
        {

        }

        public PoredakSprava(DeoTakmicenjaKod deoTakKod, Sprava sprava)
        {
            // TODO4: Proveri da li treba da se koristi DeoTakmicenjaKod i Sprava (zbog NHibernate)? Ako treba, izmeni
            // i na ostalim mestima.
            this.deoTakKod = deoTakKod;
            this._sprava = sprava;
        }

        public virtual void create(RezultatskoTakmicenje rezTak, IList<Ocena> ocene)
        {
            IList<GimnasticarUcesnik> gimnasticari;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                gimnasticari = getGimnasticari(ocene, Sprava, rezTak);
            else
                gimnasticari = rezTak.Takmicenje3.getGimnasticariKvalifikanti(Sprava);

            IDictionary<int, RezultatSprava> rezultatiMap = new Dictionary<int, RezultatSprava>();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                RezultatSprava r = new RezultatSprava();
                r.Gimnasticar = g;
                rezultatiMap.Add(g.Id, r);
            }

            foreach (Ocena o in ocene)
            {
                if (o.Sprava == Sprava && rezultatiMap.ContainsKey(o.Gimnasticar.Id))
                    rezultatiMap[o.Gimnasticar.Id].setOcena(o);
            }

            Rezultati.Clear();
            foreach (RezultatSprava r in rezultatiMap.Values)
                Rezultati.Add(r);
            rankRezultati(rezTak.Propozicije);
        }

        public virtual void rankRezultati(Propozicije propozicije)
        {
            // TODO3: I u klasi PoredakSpravaFinaleKupa treba razresiti situaciju kada dva gimnasticara imaju iste ocene.
            // Pretpostavljam da i tamo treba gledati E ocene, tj. ko ima vecu taj je u prednosti.

            List<RezultatSprava> rezultati = new List<RezultatSprava>(Rezultati);

            PropertyDescriptor[] propDesc;
            ListSortDirection[] sortDir;
            if (propozicije.VecaEOcenaImaPrednost)
            {
                propDesc = new PropertyDescriptor[] {
                    TypeDescriptor.GetProperties(typeof(RezultatSprava))["Total"],
                    TypeDescriptor.GetProperties(typeof(RezultatSprava))["E"],
                    TypeDescriptor.GetProperties(typeof(RezultatSprava))["PrezimeIme"]
                };
                sortDir = new ListSortDirection[] {
                    ListSortDirection.Descending,
                    ListSortDirection.Descending,
                    ListSortDirection.Ascending
                };
            }
            else
            {
                propDesc = new PropertyDescriptor[] {
                    TypeDescriptor.GetProperties(typeof(RezultatSprava))["Total"],
                    TypeDescriptor.GetProperties(typeof(RezultatSprava))["PrezimeIme"]
                };
                sortDir = new ListSortDirection[] {
                    ListSortDirection.Descending,
                    ListSortDirection.Ascending
                };
            }
            rezultati.Sort(new SortComparer<RezultatSprava>(propDesc, sortDir));

            RezultatSprava prevRezultat = null;
            short prevRank = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                rezultati[i].RedBroj = (short)(i + 1);

                if (rezultati[i].Total == null)
                    rezultati[i].Rank = null;
                else
                {
                    if (!ranksAreEqual(rezultati[i], prevRezultat, propozicije.VecaEOcenaImaPrednost))
                        rezultati[i].Rank = rezultati[i].RedBroj;
                    else
                        rezultati[i].Rank = prevRank;

                    prevRezultat = rezultati[i];
                    prevRank = rezultati[i].Rank.Value;
                }
            }
            updateKvalStatus<RezultatSprava>(propozicije, Rezultati, deoTakKod, x => x.Total == null);
        }

        private IList<GimnasticarUcesnik> getGimnasticari(IList<Ocena> ocene,
            Sprava sprava, RezultatskoTakmicenje rezTak)
        {
            ISet<int> idSet = new HashSet<int>();
            foreach (Ocena o in ocene)
            {
                if (o.Sprava == sprava)
                    idSet.Add(o.Gimnasticar.Id);
            }
            IList<GimnasticarUcesnik> result = new List<GimnasticarUcesnik>();
            foreach (GimnasticarUcesnik g in rezTak.Takmicenje1.Gimnasticari)
            {
                if (idSet.Contains(g.Id))
                    result.Add(g);
            }
            return result;
        }

        public static void updateKvalStatus<T>(Propozicije propozicije, IList<T> rezultatiList,
            DeoTakmicenjaKod deoTakKod, IsTotalNull<T> isTotalNull) where T : RezultatSprava
        {
            foreach (T r in rezultatiList)
                r.KvalStatus = KvalifikacioniStatus.None;

            if (deoTakKod != DeoTakmicenjaKod.Takmicenje1 || !propozicije.odvojenoTak3())
                return;

            List<T> rezultati = new List<T>(rezultatiList);
            PropertyDescriptor propDesc = TypeDescriptor.GetProperties(typeof(T))["RedBroj"];
            rezultati.Sort(new SortComparer<T>(propDesc, ListSortDirection.Ascending));

            // moram da koristim dve mape zato sto je moguca situacija da klub i 
            // drzava imaju isti id
            IDictionary<int, int> brojTakmicaraKlubMap = new Dictionary<int, int>();
            IDictionary<int, int> brojTakmicaraDrzavaMap = new Dictionary<int, int>();
            IDictionary<int, int> brojTakmicaraMap = null;
            int id = -1;

            int finCount = 0;
            int rezCount = 0;
            T prevFinRezultat = null;
            List<bool> porediDrzavu = new List<bool>();

            for (int i = 0; i < rezultati.Count; i++)
            {
                T rezultat = rezultati[i];
                if (isTotalNull(rezultat))
                {
                    rezultat.KvalStatus = KvalifikacioniStatus.None;
                    continue;
                }

                if (!propozicije.NeogranicenBrojTakmicaraIzKlubaTak3)
                {
                    porediDrzavu.Add(false);
                    if (propozicije.MaxBrojTakmicaraTak3VaziZaDrzavu)
                    {
                        if (rezultat.Gimnasticar.DrzavaUcesnik != null)
                        {
                            porediDrzavu[i] = true;
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
                            porediDrzavu[i] = true;
                            id = rezultat.Gimnasticar.DrzavaUcesnik.Id;
                            brojTakmicaraMap = brojTakmicaraDrzavaMap;
                        }
                    }
                    if (!brojTakmicaraMap.ContainsKey(id))
                        brojTakmicaraMap.Add(id, 0);
                }

                if (finCount < propozicije.BrojFinalistaTak3 || rezultat.Rank == prevFinRezultat.Rank)
                {
                    if (propozicije.NeogranicenBrojTakmicaraIzKlubaTak3
                        || brojTakmicaraMap[id] < propozicije.MaxBrojTakmicaraIzKlubaTak3
                        || postojiIstiKvalRezultatIzKluba<T>(rezultat, rezultati, porediDrzavu))
                    {
                        // Poslednji uslov u if naredbi znaci da je dostignut limit broja takmicara iz kluba, a medju
                        // finalistima se nalazi i gimnasticar iz istog kluba koji ima istu ocenu. U tom slucaju moramo
                        // da dodamo i ovog finalistu. TODO4: Da li u ovom slucaju treba da povecavamo finCount? (i kod
                        // preskoka isto).

                        finCount++;
                        rezultat.KvalStatus = KvalifikacioniStatus.Q;
                        prevFinRezultat = rezultat;
                        if (!propozicije.NeogranicenBrojTakmicaraIzKlubaTak3)
                            brojTakmicaraMap[id]++;
                    }
                    else if (rezCount < propozicije.BrojRezerviTak3 && Opcije.Instance.UzimajPrvuSlobodnuRezervu)
                    {
                        rezCount++;
                        rezultat.KvalStatus = KvalifikacioniStatus.R;
                    }
                    else
                        rezultat.KvalStatus = KvalifikacioniStatus.None;
                }
                else if (rezCount < propozicije.BrojRezerviTak3)
                {
                    if (propozicije.NeogranicenBrojTakmicaraIzKlubaTak3
                        || brojTakmicaraMap[id] < propozicije.MaxBrojTakmicaraIzKlubaTak3)
                    {
                        rezCount++;
                        rezultat.KvalStatus = KvalifikacioniStatus.R;
                        if (!propozicije.NeogranicenBrojTakmicaraIzKlubaTak3)
                            brojTakmicaraMap[id]++;
                    }
                    else if (Opcije.Instance.UzimajPrvuSlobodnuRezervu)
                    {
                        rezCount++;
                        rezultat.KvalStatus = KvalifikacioniStatus.R;
                    }
                    else
                        rezultat.KvalStatus = KvalifikacioniStatus.None;
                }
                else
                {
                    // TODO: Uradi i za rezerve razresavanje situacije kada postoji vise rezervi sa identicnim
                    // rezultatom (isto i za preskok).
                    rezultat.KvalStatus = KvalifikacioniStatus.None;
                }
            }
        }

        private bool ranksAreEqual(RezultatSprava r1, RezultatSprava r2, bool vecaEOcenaImaPrednost)
        {
            if (r1 == null || r2 == null)
                return false;
            return r1.Total == r2.Total && (!vecaEOcenaImaPrednost || r1.E == r2.E);
        }

        private static bool postojiIstiKvalRezultatIzKluba<T>(T rezultat, List<T> rezultati,
            List<bool> porediDrzavu) where T : RezultatSprava
        {
            for (int i = 0; i < rezultati.Count; ++i)
            {
                T r = rezultati[i];
                if (r.KvalStatus != KvalifikacioniStatus.Q || r.Rank != rezultat.Rank)
                    continue;

                if (porediDrzavu[i])
                {
                    if (r.Gimnasticar.DrzavaUcesnik.Id == rezultat.Gimnasticar.DrzavaUcesnik.Id)
                        return true;
                }
                else if (r.Gimnasticar.KlubUcesnik.Id == rezultat.Gimnasticar.KlubUcesnik.Id)
                    return true;
            }
            return false;
        }

        public virtual List<RezultatSprava> getRezultati()
        {
            List<RezultatSprava> result = new List<RezultatSprava>(Rezultati);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatSprava))["RedBroj"];
            result.Sort(new SortComparer<RezultatSprava>(propDesc,
                ListSortDirection.Ascending));

            return result;
        }

        public virtual List<RezultatSprava> getKvalifikantiIRezerve()
        {
            List<RezultatSprava> result = new List<RezultatSprava>();
            foreach (RezultatSprava r in getRezultati())
            {
                if (r.KvalStatus == KvalifikacioniStatus.Q)
                    result.Add(r);
            }
            foreach (RezultatSprava r in getRezultati())
            {
                if (r.KvalStatus == KvalifikacioniStatus.R)
                    result.Add(r);
            }

            return result;
        }

        public virtual void addOcena(Ocena o, RezultatskoTakmicenje rezTak,
            bool createRezultat)
        {
            RezultatSprava rezultat;
            if (createRezultat)
            {
                rezultat = new RezultatSprava();
                rezultat.Gimnasticar = o.Gimnasticar;
                Rezultati.Add(rezultat);
            }
            else
            {
                rezultat = getRezultat(o.Gimnasticar);
            }
            if (rezultat != null)
            {
                rezultat.setOcena(o);
                rankRezultati(rezTak.Propozicije);
            }
        }

        public virtual void deleteOcena(Ocena o, RezultatskoTakmicenje rezTak, bool removeRezultat)
        {
            RezultatSprava r = getRezultat(o.Gimnasticar);
            if (r != null)
            {
                if (removeRezultat)
                    Rezultati.Remove(r);
                else
                    r.clearOcena();
                rankRezultati(rezTak.Propozicije);
            }
        }

        private RezultatSprava getRezultat(GimnasticarUcesnik g)
        {
            foreach (RezultatSprava r in Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                    return r;
            }
            return null;
        }

        public virtual void editOcena(Ocena o, RezultatskoTakmicenje rezTak)
        {
            RezultatSprava r = getRezultat(o.Gimnasticar);
            if (r != null)
            {
                r.setOcena(o);
                rankRezultati(rezTak.Propozicije);
            }
        }


        public virtual void addGimnasticar(GimnasticarUcesnik g, Ocena o, 
            RezultatskoTakmicenje rezTak)
        {
            RezultatSprava r = new RezultatSprava();
            r.Gimnasticar = g;
            r.setOcena(o);
            Rezultati.Add(r);
            rankRezultati(rezTak.Propozicije);
        }

        public virtual void deleteGimnasticar(GimnasticarUcesnik g, 
            RezultatskoTakmicenje rezTak)
        {
            RezultatSprava r = getRezultat(g);
            if (r != null)
            {
                Rezultati.Remove(r);
                rankRezultati(rezTak.Propozicije);
            }
        }

        public virtual void dumpRezultati(StreamWriter streamWriter)
        {
            string sprava = Sprave.toString(Sprava).ToUpper();
            string header = DeoTakmicenjaKod == DeoTakmicenjaKod.Takmicenje1 ? sprava : sprava + " - FINALE";
            streamWriter.WriteLine(header);
            foreach (RezultatSprava r in getRezultati())
            {
                string line = r.Rank + ". " + r.Gimnasticar.ImeSrednjeImePrezimeDatumRodjenja + "   " + r.Total;
                streamWriter.WriteLine(line);
            }
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(DeoTakmicenjaKod.ToString());
            strBuilder.AppendLine(Sprava.ToString());

            strBuilder.AppendLine(Rezultati.Count.ToString());
            foreach (RezultatSprava r in Rezultati)
                r.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            DeoTakmicenjaKod = (DeoTakmicenjaKod)Enum.Parse(typeof(DeoTakmicenjaKod), reader.ReadLine());
            Sprava = (Sprava)Enum.Parse(typeof(Sprava), reader.ReadLine());

            int brojRezultata = int.Parse(reader.ReadLine());
            for (int i = 0; i < brojRezultata; ++i)
            {
                reader.ReadLine();  // id
                RezultatSprava r = new RezultatSprava();
                r.loadFromDump(reader, map);
                Rezultati.Add(r);
            }
        }
    }
}
