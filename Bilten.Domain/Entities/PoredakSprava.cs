using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Bilten.Util;

namespace Bilten.Domain
{
    public class PoredakSprava : DomainObject
    {
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

        protected PoredakSprava()
        {

        }

        public PoredakSprava(DeoTakmicenjaKod deoTakKod, Sprava sprava)
        {
            this.deoTakKod = deoTakKod;
            this._sprava = sprava;
        }

        public virtual void initRezultati(IList<GimnasticarUcesnik> gimnasticari)
        {
            Rezultati.Clear();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                RezultatSprava r = new RezultatSprava();
                r.Gimnasticar = g;
                Rezultati.Add(r);
            }

            // posto nepostoje ocene, sledeci poziv samo sortira po prezimenu i na
            // osnovu toga dodeljuje RedBroj
            rankRezultati();
        }

        public virtual void create(RezultatskoTakmicenje rezTak, IList<Ocena> ocene)
        {
            IList<GimnasticarUcesnik> gimnasticari;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                gimnasticari = getGimnasticari(ocene, Sprava, rezTak);
            else
                gimnasticari = rezTak.Takmicenje3.getUcesniciGimKvalifikanti(Sprava);

            IDictionary<int, RezultatSprava> rezultatiMap = new Dictionary<int, RezultatSprava>();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                RezultatSprava rezultat = new RezultatSprava();
                rezultat.Gimnasticar = g;
                rezultatiMap.Add(g.Id, rezultat);
            }

            foreach (Ocena o in ocene)
            {
                if (o.Sprava == Sprava && rezultatiMap.ContainsKey(o.Gimnasticar.Id))
                    rezultatiMap[o.Gimnasticar.Id].setOcena(o);
            }

            List<RezultatSprava> rezultati = new List<RezultatSprava>(rezultatiMap.Values);
            Rezultati.Clear();
            foreach (RezultatSprava rez in rezultati)
                Rezultati.Add(rez);

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
        }

        private void rankRezultati()
        {
            // TODO3: I u klasi PoredakSpravaFinaleKupa treba razresiti situaciju kada dva gimnasticara imaju iste ocene.
            // Pretpostavljam da i tamo treba gledati E ocene, tj. ko ima vecu taj je u prednosti.

            List<RezultatSprava> rezultati = new List<RezultatSprava>(Rezultati);

            PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                TypeDescriptor.GetProperties(typeof(RezultatSprava))["Total"],
                TypeDescriptor.GetProperties(typeof(RezultatSprava))["E"],
                TypeDescriptor.GetProperties(typeof(RezultatSprava))["PrezimeIme"]
            };
            ListSortDirection[] sortDir = new ListSortDirection[] {
                ListSortDirection.Descending,
                ListSortDirection.Descending,
                ListSortDirection.Ascending
            };
            rezultati.Sort(new SortComparer<RezultatSprava>(propDesc, sortDir));

            RezultatSprava prevRezultat = null;
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
                    if (!resultsAreEqual(rezultati[i], prevRezultat))
                        rezultati[i].Rank = (short)(i + 1);
                    else
                        rezultati[i].Rank = prevRank;

                    prevRezultat = rezultati[i];
                    prevRank = rezultati[i].Rank.Value;
                }
            }
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

        private void updateKvalStatus(Propozicije propozicije)
        {
            if (deoTakKod != DeoTakmicenjaKod.Takmicenje1 || !propozicije.PostojiTak3 || !propozicije.OdvojenoTak3)
            {
                foreach (RezultatSprava r in Rezultati)
                    r.KvalStatus = KvalifikacioniStatus.None;
                return;
            }

            List<RezultatSprava> rezultati = new List<RezultatSprava>(Rezultati);
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatSprava))["RedBroj"];
            rezultati.Sort(new SortComparer<RezultatSprava>(propDesc,
                ListSortDirection.Ascending));

            // moram da koristim dve mape zato sto je moguca situacija da klub i 
            // drzava imaju isti id
            IDictionary<int, int> brojTakmicaraKlubMap = new Dictionary<int, int>();
            IDictionary<int, int> brojTakmicaraDrzavaMap = new Dictionary<int, int>();

            int finCount = 0;
            int rezCount = 0;
            RezultatSprava prevFinRezultat = null;
            List<bool> porediDrzavu = new List<bool>();

            for (int i = 0; i < rezultati.Count; i++)
            {
                RezultatSprava rezultat = rezultati[i];
                porediDrzavu.Add(false);

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

                if (finCount < propozicije.BrojFinalistaTak3)
                {
                    if (propozicije.NeogranicenBrojTakmicaraIzKlubaTak3)
                    {
                        finCount++;
                        rezultat.KvalStatus = KvalifikacioniStatus.Q;
                        prevFinRezultat = rezultat;
                    }
                    else
                    {
                        if (brojTakmicaraMap[id] < propozicije.MaxBrojTakmicaraIzKlubaTak3)
                        {
                            finCount++;
                            brojTakmicaraMap[id]++;
                            rezultat.KvalStatus = KvalifikacioniStatus.Q;
                            prevFinRezultat = rezultat;
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
                else if (prevFinRezultat != null && resultsAreEqual(rezultat, prevFinRezultat))
                {
                    // Dodali smo predvidjeni broj finalista, ali postoji rezultat koji je identican zadnjem dodatom
                    // finalisti.
                    if (propozicije.NeogranicenBrojTakmicaraIzKlubaTak3)
                    {
                        rezultat.KvalStatus = KvalifikacioniStatus.Q;
                    }
                    else
                    {
                        if (brojTakmicaraMap[id] < propozicije.MaxBrojTakmicaraIzKlubaTak3)
                        {
                            brojTakmicaraMap[id]++;
                            rezultat.KvalStatus = KvalifikacioniStatus.Q;
                        }
                        else if (nadjiIstiFinRezultatIzKluba(rezultat, rezultati, porediDrzavu))
                        {
                            // Dostignut je limit broja takmicara iz kluba, a medju finalistima se nalazi
                            // i gimnasticar iz istog kluba koji ima istu ocenu. U tom slucaju moramo da dodamo i
                            // ovog finalistu.
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
                    // TODO: Uradi i za rezerve razresavanje situacije kada postoji vise rezervi sa identicnim
                    // rezultatom.
                    rezultat.KvalStatus = KvalifikacioniStatus.None;
                }
            }
        }

        private bool resultsAreEqual(RezultatSprava r1, RezultatSprava r2)
        {
            if (r1 == null || r2 == null)
                return false;
            return r1.Total == r2.Total && r1.E == r2.E;
        }

        private bool nadjiIstiFinRezultatIzKluba(RezultatSprava rezultat, List<RezultatSprava> rezultati, List<bool> porediDrzavu)
        {
            for (int i = 0; i < rezultati.Count; ++i)
            {
                RezultatSprava r = rezultati[i];
                if (r.KvalStatus != KvalifikacioniStatus.Q || !resultsAreEqual(r, rezultat))
                    continue;

                if (porediDrzavu[i])
                {
                    if (r.Gimnasticar.DrzavaUcesnik.Id == rezultat.Gimnasticar.DrzavaUcesnik.Id)
                        return true;
                }
                else
                {
                    if (r.Gimnasticar.KlubUcesnik.Id == rezultat.Gimnasticar.KlubUcesnik.Id)
                        return true;
                }
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
                rankRezultati();
                updateKvalStatus(rezTak.Propozicije);
            }
        }

        public virtual void deleteOcena(Ocena o, RezultatskoTakmicenje rezTak, 
            bool removeRezultat)
        {
            RezultatSprava r = getRezultat(o.Gimnasticar);
            if (r != null)
            {
                if (removeRezultat)
                    Rezultati.Remove(r);
                else
                    r.clearOcena(o);

                rankRezultati();
                updateKvalStatus(rezTak.Propozicije);
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
                rankRezultati();
                updateKvalStatus(rezTak.Propozicije);
            }
        }


        public virtual void addGimnasticar(GimnasticarUcesnik g, Ocena o, 
            RezultatskoTakmicenje rezTak)
        {
            RezultatSprava r = new RezultatSprava();
            r.Gimnasticar = g;
            r.setOcena(o);
            Rezultati.Add(r);

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
        }

        public virtual void deleteGimnasticar(GimnasticarUcesnik g, 
            RezultatskoTakmicenje rezTak)
        {
            RezultatSprava r = getRezultat(g);
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
            strBuilder.AppendLine(Sprava.ToString());

            if (Rezultati == null)
                strBuilder.AppendLine(NULL);
            else
            {
                strBuilder.AppendLine(Rezultati.Count.ToString());
                foreach (RezultatSprava r in Rezultati)
                    r.dump(strBuilder);
            }
        }
    }
}
