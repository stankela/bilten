using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Bilten.Util;
using System.IO;

namespace Bilten.Domain
{
    public class PoredakPreskok : DomainObject
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
            protected set { _sprava = value; }
        }

        private IList<RezultatPreskok> _rezultati = new List<RezultatPreskok>();
        public virtual IList<RezultatPreskok> Rezultati
        {
            get { return _rezultati; }
            protected set { _rezultati = value; }
        }

        public PoredakPreskok()
        { 
        
        }

        public PoredakPreskok(DeoTakmicenjaKod deoTakKod)
        {
            this.deoTakKod = deoTakKod;
            this._sprava = Sprava.Preskok;
        }

        public virtual void create(RezultatskoTakmicenje rezTak, IList<Ocena> ocene)
        {
            IList<GimnasticarUcesnik> gimnasticari;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                gimnasticari = getGimnasticari(ocene, rezTak);
            else
                gimnasticari = rezTak.Takmicenje3.getGimnasticariKvalifikanti(Sprava.Preskok);

            IDictionary<int, RezultatPreskok> rezultatiMap = new Dictionary<int, RezultatPreskok>();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                RezultatPreskok r = new RezultatPreskok();
                r.Gimnasticar = g;
                rezultatiMap.Add(g.Id, r);
            }

            foreach (Ocena o in ocene)
            {
                if (o.Sprava == Sprava.Preskok && rezultatiMap.ContainsKey(o.Gimnasticar.Id))
                    rezultatiMap[o.Gimnasticar.Id].setOcena(o);
            }

            Rezultati.Clear();
            foreach (RezultatPreskok r in rezultatiMap.Values)
                Rezultati.Add(r);
            rankRezultati(rezTak.Propozicije, rezTak.Takmicenje.FinaleKupa);
        }

        public virtual void rankRezultati(Propozicije propozicije, bool finaleKupa)
        {
            bool obaPreskoka = propozicije.racunajObaPreskoka(DeoTakmicenjaKod, finaleKupa);
            if (!obaPreskoka)
            {
                List<RezultatPreskok> rezultati = new List<RezultatPreskok>(Rezultati);
                rankByPrviPreskok(rezultati, 0);
            }
            else
            {
                List<RezultatPreskok> rezultatiObaPreskoka = new List<RezultatPreskok>();
                List<RezultatPreskok> rezultatiPrviPreskok = new List<RezultatPreskok>();
                foreach (RezultatPreskok r in Rezultati)
                {
                    if (r.TotalObeOcene != null)
                        rezultatiObaPreskoka.Add(r);
                    else
                        rezultatiPrviPreskok.Add(r);
                }

                // Ovakav nacin rangiranja rezultata (da se prvo rangiraju takmicari sa dva preskoka pa zatim sa jednim)
                // obezbedjuje da i u situaciji kada je u propozicijama navedeno da se preskok racuna na osnovu obe ocene,
                // a zatim se kod unosenja ocena unese samo prva ocena za sve takmicare, da ce i tada takmicari biti
                // ispravno rangirani (rezultatiObaPreskoka ce tada biti prazni)
                rankByObaPreskoka(rezultatiObaPreskoka);
                rankByPrviPreskok(rezultatiPrviPreskok, rezultatiObaPreskoka.Count);
            }
            updateKvalStatus(propozicije);
        }

        private void rankByObaPreskoka(List<RezultatPreskok> rezultati)
        {
            PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                TypeDescriptor.GetProperties(typeof(RezultatPreskok))["TotalObeOcene"],
                TypeDescriptor.GetProperties(typeof(RezultatPreskok))["EObeOcene"],
                TypeDescriptor.GetProperties(typeof(RezultatPreskok))["PrezimeIme"]
            };
            ListSortDirection[] sortDir = new ListSortDirection[] {
                ListSortDirection.Descending,
                ListSortDirection.Descending,
                ListSortDirection.Ascending
            };
            rezultati.Sort(new SortComparer<RezultatPreskok>(propDesc, sortDir));

            RezultatPreskok prevRezultat = null;
            short prevRank = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                rezultati[i].KvalStatus = KvalifikacioniStatus.None;
                rezultati[i].RedBroj = (short)(i + 1);

                // TotalObeOcene != null za sve rezultate
                if (!resultsAreEqual(rezultati[i], prevRezultat, true))
                    rezultati[i].Rank = rezultati[i].RedBroj;
                else
                    rezultati[i].Rank = prevRank;

                prevRezultat = rezultati[i];
                prevRank = rezultati[i].Rank.Value;
            }
        }

        private void rankByPrviPreskok(List<RezultatPreskok> rezultati, int redBrojOffset)
        {
            PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                TypeDescriptor.GetProperties(typeof(RezultatPreskok))["Total"],
                TypeDescriptor.GetProperties(typeof(RezultatPreskok))["E"],
                TypeDescriptor.GetProperties(typeof(RezultatPreskok))["PrezimeIme"]
            };
            ListSortDirection[] sortDir = new ListSortDirection[] {
                ListSortDirection.Descending,
                ListSortDirection.Descending,
                ListSortDirection.Ascending
            };
            rezultati.Sort(new SortComparer<RezultatPreskok>(propDesc, sortDir));

            RezultatPreskok prevRezultat = null;
            short prevRank = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                rezultati[i].KvalStatus = KvalifikacioniStatus.None;
                rezultati[i].RedBroj = (short)(i + 1 + redBrojOffset);

                if (rezultati[i].Total == null)
                    rezultati[i].Rank = null;
                else
                {
                    if (!resultsAreEqual(rezultati[i], prevRezultat, false))
                        rezultati[i].Rank = rezultati[i].RedBroj;
                    else
                        rezultati[i].Rank = prevRank;

                    prevRezultat = rezultati[i];
                    prevRank = rezultati[i].Rank.Value;
                }
            }
        }

        private IList<GimnasticarUcesnik> getGimnasticari(IList<Ocena> ocene,
            RezultatskoTakmicenje rezTak)
        {
            ISet<int> idSet = new HashSet<int>();
            foreach (Ocena o in ocene)
            {
                if (o.Sprava == Sprava.Preskok)
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

        // TODO4: Kada je u propozicijama stavljeno da se kvalifikanti racunaju na osnovu oba skoka, a u takmicenju 1 je
        // unesen samo prvi skok, tada nijedan gimnasticar nece biti oznacen sa Q. Promeni da oznacava na osnovu
        // prvog skoka. Takodje, i kod takmicenja 3, ako je u propozicijama navedeno da se racuna na osnovu oba
        // preskoka a unesen je samo prvi, racunaj poredak na osnovu prvog skoka. (Ovde uvek treba gledati da li je
        // unesena bar jedna ocena sa oba preskoka - ako jeste racuna se normalno; ako nije, racuna se samo prvi skok.)

        // TODO4: Kod ranijih takmicenja (recimo iz 2013 i ranije), na nekim mestima je tadasnja verzija programa pogresno
        // oznacila sve takmicare kao kvalifikante. Proveri sva takva mesta i eventualno probaj da ispravis.

        private void updateKvalStatus(Propozicije propozicije)
        {
            if (deoTakKod != DeoTakmicenjaKod.Takmicenje1 || !propozicije.odvojenoTak3())
            {
                foreach (RezultatPreskok r in Rezultati)
                    r.KvalStatus = KvalifikacioniStatus.None;
                return;
            }

            List<RezultatPreskok> rezultati = new List<RezultatPreskok>(Rezultati);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatPreskok))["RedBroj"];
            rezultati.Sort(new SortComparer<RezultatPreskok>(propDesc,
                ListSortDirection.Ascending));

            // moram da koristim dve mape zato sto je moguca situacija da klub i 
            // drzava imaju isti id
            IDictionary<int, int> brojTakmicaraKlubMap = new Dictionary<int, int>();
            IDictionary<int, int> brojTakmicaraDrzavaMap = new Dictionary<int, int>();

            int finCount = 0;
            int rezCount = 0;
            RezultatPreskok prevFinRezultat = null;
            List<bool> porediDrzavu = new List<bool>();

            for (int i = 0; i < rezultati.Count; i++)
            {
                RezultatPreskok rezultat = rezultati[i];
                porediDrzavu.Add(false);

                if (!propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka && rezultat.Total == null
                || propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka && rezultat.TotalObeOcene == null)
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
                else if (prevFinRezultat != null && resultsAreEqual(rezultat, prevFinRezultat,
                    propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka))
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
                        else if (nadjiIstiFinRezultatIzKluba(rezultat, rezultati, porediDrzavu,
                            propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka))
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

        private bool resultsAreEqual(RezultatPreskok r1, RezultatPreskok r2, bool obaPreskoka)
        {
            if (r1 == null || r2 == null)
                return false;
            return (!obaPreskoka && r1.Total == r2.Total && r1.E == r2.E)
                    || (obaPreskoka && r1.TotalObeOcene == r2.TotalObeOcene && r1.EObeOcene == r2.EObeOcene);
        }

        private bool nadjiIstiFinRezultatIzKluba(RezultatPreskok rezultat, List<RezultatPreskok> rezultati, List<bool> porediDrzavu,
            bool obaPreskoka)
        {
            for (int i = 0; i < rezultati.Count; ++i)
            {
                RezultatPreskok r = rezultati[i];
                if (r.KvalStatus != KvalifikacioniStatus.Q || !resultsAreEqual(r, rezultat, obaPreskoka))
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

        // TODO: Ima dosta ponavljanja istog koda u klasama PoredakSprava i PoredakPreskok. Probaj da generalizujes.

        public virtual List<RezultatPreskok> getRezultati()
        {
            List<RezultatPreskok> result = new List<RezultatPreskok>(Rezultati);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatPreskok))["RedBroj"];
            result.Sort(new SortComparer<RezultatPreskok>(propDesc,
                ListSortDirection.Ascending));

            return result;
        }

        public virtual bool postojeObaPreskoka()
        {
            foreach (RezultatPreskok r in Rezultati)
            {
                if (r.TotalObeOcene != null)
                    return true;
            }
            return false;
        }

        public virtual List<RezultatPreskok> getKvalifikantiIRezerve(bool obaPreskoka)
        {
            List<RezultatPreskok> result = new List<RezultatPreskok>();
            foreach (RezultatPreskok r in getRezultati())
            {
                if (r.KvalStatus == KvalifikacioniStatus.Q)
                    result.Add(r);
            }
            foreach (RezultatPreskok r in getRezultati())
            {
                if (r.KvalStatus == KvalifikacioniStatus.R)
                    result.Add(r);
            }

            return result;
        }

        public virtual void addOcena(Ocena o, RezultatskoTakmicenje rezTak,
            bool createRezultat)
        {
            RezultatPreskok rezultat;
            if (createRezultat)
            {
                rezultat = new RezultatPreskok();
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
                rankRezultati(rezTak.Propozicije, rezTak.Takmicenje.FinaleKupa);
            }
        }

        public virtual void deleteOcena(Ocena o, RezultatskoTakmicenje rezTak,
            bool removeRezultat)
        {
            RezultatPreskok r = getRezultat(o.Gimnasticar);
            if (r != null)
            {
                if (removeRezultat)
                    Rezultati.Remove(r);
                else
                    r.clearOcena(o);
                rankRezultati(rezTak.Propozicije, rezTak.Takmicenje.FinaleKupa);
            }
        }

        private RezultatPreskok getRezultat(GimnasticarUcesnik g)
        {
            foreach (RezultatPreskok r in Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                    return r;
            }
            return null;
        }

        public virtual void editOcena(Ocena o, RezultatskoTakmicenje rezTak)
        {
            RezultatPreskok r = getRezultat(o.Gimnasticar);
            if (r != null)
            {
                r.setOcena(o);
                rankRezultati(rezTak.Propozicije, rezTak.Takmicenje.FinaleKupa);
            }
        }

        public virtual void addGimnasticar(GimnasticarUcesnik g, Ocena o, 
            RezultatskoTakmicenje rezTak)
        {
            RezultatPreskok r = new RezultatPreskok();
            r.Gimnasticar = g;
            r.setOcena(o);
            Rezultati.Add(r);
            rankRezultati(rezTak.Propozicije, rezTak.Takmicenje.FinaleKupa);
        }

        public virtual void deleteGimnasticar(GimnasticarUcesnik g, 
            RezultatskoTakmicenje rezTak)
        {
            RezultatPreskok r = getRezultat(g);
            if (r != null)
            {
                Rezultati.Remove(r);
                rankRezultati(rezTak.Propozicije, rezTak.Takmicenje.FinaleKupa);
            }
        }

        public virtual void dumpRezultati(StreamWriter streamWriter, Propozicije propozicije, bool finaleKupa)
        {
            string header = DeoTakmicenjaKod == DeoTakmicenjaKod.Takmicenje1 ? "PRESKOK" : "PRESKOK - FINALE";
            streamWriter.WriteLine(header);
            foreach (RezultatPreskok r in getRezultati())
            {
                float? total;
                bool obaPreskoka = propozicije.racunajObaPreskoka(DeoTakmicenjaKod, finaleKupa);
                if (!obaPreskoka || r.TotalObeOcene == null)
                    total = r.Total;
                else
                    total = r.TotalObeOcene;
                string line = r.Rank + ". " + r.Gimnasticar.ImeSrednjeImePrezimeDatumRodjenja + "   " + total;
                streamWriter.WriteLine(line);
            }
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(DeoTakmicenjaKod.ToString());
            strBuilder.AppendLine(Sprava.ToString());

            strBuilder.AppendLine(Rezultati.Count.ToString());
            foreach (RezultatPreskok r in Rezultati)
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
                RezultatPreskok r = new RezultatPreskok();
                r.loadFromDump(reader, map);
                Rezultati.Add(r);
            }
        }
    }
}
