using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Bilten.Util;
using System.IO;

namespace Bilten.Domain
{
    public class PoredakUkupno : DomainObject
    {
        private DeoTakmicenjaKod deoTakKod;
        public virtual DeoTakmicenjaKod DeoTakmicenjaKod
        {
            get { return deoTakKod; }
            set { deoTakKod = value; }
        }

        private IList<RezultatUkupno> _rezultati = new List<RezultatUkupno>();
        public virtual IList<RezultatUkupno> Rezultati
        {
            get { return _rezultati; }
            protected set { _rezultati = value; }
        }

        public PoredakUkupno()
        { 
        
        }

        public PoredakUkupno(DeoTakmicenjaKod deoTakKod)
        {
            this.deoTakKod = deoTakKod;
        }

        // Ako je finaleMemorijala == true, to znaci da postoji samo takmicenje 1 ali da se poredak izracunava tako
        // sto postoje ogranicenja za broj gimnasticara iz kluba/drzave.
        public virtual void create(RezultatskoTakmicenje rezTak, IList<Ocena> ocene, bool finaleMemorijala = false)
        {
            IList<GimnasticarUcesnik> gimnasticari;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                gimnasticari = new List<GimnasticarUcesnik>(rezTak.Takmicenje1.Gimnasticari);
            else
                gimnasticari = new List<GimnasticarUcesnik>(rezTak.Takmicenje2.getUcesniciGimKvalifikanti());

            IDictionary<int, RezultatUkupno> rezultatiMap = new Dictionary<int, RezultatUkupno>();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                RezultatUkupno r = new RezultatUkupno();
                r.Gimnasticar = g;
                rezultatiMap.Add(g.Id, r);
            }

            foreach (Ocena o in ocene)
            {
                if (rezultatiMap.ContainsKey(o.Gimnasticar.Id))
                    rezultatiMap[o.Gimnasticar.Id].addSprava(o, rezTak.Propozicije.ZaPreskokVisebojRacunajBoljuOcenu);
            }

            Rezultati.Clear();
            foreach (RezultatUkupno r in rezultatiMap.Values)
            {
                if (r.Gimnasticar.PenaltyViseboj != null)
                    r.addPenalty(r.Gimnasticar.PenaltyViseboj.Value);
                Rezultati.Add(r);
            }
            rankRezultati(rezTak.Propozicije, finaleMemorijala);
        }

        public virtual void rankRezultati(Propozicije propozicije, bool finaleMemorijala = false)
        {
            List<RezultatUkupno> rezultati = new List<RezultatUkupno>(Rezultati);

            PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                TypeDescriptor.GetProperties(typeof(RezultatUkupno))["Total"],
                TypeDescriptor.GetProperties(typeof(RezultatUkupno))["PrezimeIme"]
            };
            ListSortDirection[] sortDir = new ListSortDirection[] {
                ListSortDirection.Descending,
                ListSortDirection.Ascending
            };
            rezultati.Sort(new SortComparer<RezultatUkupno>(propDesc, sortDir));

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
            if (!finaleMemorijala)
            {
                updateKvalStatus(propozicije.odvojenoTak2(), propozicije.BrojFinalistaTak2, propozicije.BrojRezerviTak2,
                    propozicije.NeogranicenBrojTakmicaraIzKlubaTak2, propozicije.MaxBrojTakmicaraIzKlubaTak2,
                    propozicije.MaxBrojTakmicaraTak2VaziZaDrzavu);
            }
            else
            {
                updateKvalStatus(true, 1000, 0,
                    false, 2,
                    true);
                for (int i = Rezultati.Count - 1; i >= 0; --i)
                {
                    if (Rezultati[i].KvalStatus != KvalifikacioniStatus.Q)
                        Rezultati.RemoveAt(i);
                }
                // Ponovi rangiraj gimnasticare, nakon sto smo izbacili suvisne.
                rankRezultati(propozicije);
            }
        }

        private void updateKvalStatus(bool odvojenoTak2, int brojFinalistaTak2, int brojRezerviTak2,
            bool neogranicenBrojTakmicaraIzKlubaTak2, int maxBrojTakmicaraIzKlubaTak2,
            bool maxBrojTakmicaraTak2VaziZaDrzavu)
        {
            foreach (RezultatUkupno r in Rezultati)
                r.KvalStatus = KvalifikacioniStatus.None;

            if (deoTakKod != DeoTakmicenjaKod.Takmicenje1 || !odvojenoTak2)
                return;
            
            List<RezultatUkupno> rezultati = new List<RezultatUkupno>(Rezultati);
            PropertyDescriptor propDesc = TypeDescriptor.GetProperties(typeof(RezultatUkupno))["RedBroj"];
            rezultati.Sort(new SortComparer<RezultatUkupno>(propDesc, ListSortDirection.Ascending));

            // moram da koristim dve mape zato sto je moguca situacija da klub i 
            // drzava imaju isti id
            IDictionary<int, int> brojTakmicaraKlubMap = new Dictionary<int, int>();
            IDictionary<int, int> brojTakmicaraDrzavaMap = new Dictionary<int, int>();
            IDictionary<int, int> brojTakmicaraMap = null;
            int id = -1;

            int finCount = 0;
            int rezCount = 0;
            RezultatUkupno prevFinRezultat = null;
            List<bool> porediDrzavu = new List<bool>();
            
            for (int i = 0; i < rezultati.Count; i++)
            {
                RezultatUkupno rezultat = rezultati[i];
                if (rezultat.Total == null)
                {
                    rezultat.KvalStatus = KvalifikacioniStatus.None;
                    continue;
                }

                if (!neogranicenBrojTakmicaraIzKlubaTak2)
                {
                    porediDrzavu.Add(false);
                    if (maxBrojTakmicaraTak2VaziZaDrzavu)
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

                if (finCount < brojFinalistaTak2 || rezultat.Rank == prevFinRezultat.Rank)
                {
                    if (neogranicenBrojTakmicaraIzKlubaTak2
                        || brojTakmicaraMap[id] < maxBrojTakmicaraIzKlubaTak2
                        || postojiIstiKvalRezultatIzKluba(rezultat, rezultati, porediDrzavu))
                    {
                        // Poslednji uslov u if naredbi znaci da je dostignut limit broja takmicara iz kluba, a medju
                        // finalistima se nalazi i gimnasticar iz istog kluba koji ima istu ocenu. U tom slucaju moramo
                        // da dodamo i ovog finalistu. TODO4: Da li u ovom slucaju treba da povecavamo finCount? (i kod
                        // rezultata sprava isto).

                        finCount++;
                        rezultat.KvalStatus = KvalifikacioniStatus.Q;
                        prevFinRezultat = rezultat;
                        if (!neogranicenBrojTakmicaraIzKlubaTak2)
                            brojTakmicaraMap[id]++;
                    }
                    else if (rezCount < brojRezerviTak2 && Opcije.Instance.UzimajPrvuSlobodnuRezervu)
                    {
                        rezCount++;
                        rezultat.KvalStatus = KvalifikacioniStatus.R;
                    }
                    else
                        rezultat.KvalStatus = KvalifikacioniStatus.None;
                }
                else if (rezCount < brojRezerviTak2)
                {
                    if (neogranicenBrojTakmicaraIzKlubaTak2
                        || brojTakmicaraMap[id] < maxBrojTakmicaraIzKlubaTak2)
                    {
                        rezCount++;
                        rezultat.KvalStatus = KvalifikacioniStatus.R;
                        if (!neogranicenBrojTakmicaraIzKlubaTak2)
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
                    // rezultatom (isto i za rezultate sprave).
                    rezultat.KvalStatus = KvalifikacioniStatus.None;
                }
            }
        }

        private bool postojiIstiKvalRezultatIzKluba(RezultatUkupno rezultat, List<RezultatUkupno> rezultati,
            List<bool> porediDrzavu)
        {
            for (int i = 0; i < rezultati.Count; ++i)
            {
                RezultatUkupno r = rezultati[i];
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

        public virtual List<RezultatUkupno> getRezultati()
        {
            List<RezultatUkupno> result = new List<RezultatUkupno>(Rezultati);

            PropertyDescriptor propDesc = TypeDescriptor.GetProperties(typeof(RezultatUkupno))["RedBroj"];
            result.Sort(new SortComparer<RezultatUkupno>(propDesc, ListSortDirection.Ascending));

            return result;
        }

        public virtual List<RezultatUkupnoExtended> getRezultatiExtended(IList<Ocena> ocene, bool prikaziDEOcene,
            bool zaPreskokVisebojRacunajBoljuOcenu)
        {
            if (prikaziDEOcene)
            {
                IDictionary<int, RezultatUkupnoExtended> rezultatiMap = new Dictionary<int, RezultatUkupnoExtended>();
                foreach (RezultatUkupno r in Rezultati)
                {
                    RezultatUkupnoExtended rezEx = new RezultatUkupnoExtended(r);
                    rezultatiMap.Add(rezEx.Gimnasticar.Id, rezEx);
                }

                foreach (Ocena o in ocene)
                {
                    if (rezultatiMap.ContainsKey(o.Gimnasticar.Id))
                    {
                        float? d;
                        float? e;
                        if (o.Sprava != Sprava.Preskok || !zaPreskokVisebojRacunajBoljuOcenu || o.Ocena2 == null
                            || Math.Max(o.Total.Value, o.Ocena2.Total.Value) == o.Total.Value)
                        {
                            d = o.D;
                            e = o.E;
                        }
                        else
                        {
                            d = o.Ocena2.D;
                            e = o.Ocena2.E;
                        }
                        RezultatUkupnoExtended r = rezultatiMap[o.Gimnasticar.Id];
                        r.setDOcena(o.Sprava, d);
                        r.setEOcena(o.Sprava, e);
                    }
                }

                List<RezultatUkupnoExtended> result = new List<RezultatUkupnoExtended>(rezultatiMap.Values);

                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(typeof(RezultatUkupnoExtended))["RedBroj"];
                result.Sort(new SortComparer<RezultatUkupnoExtended>(propDesc, ListSortDirection.Ascending));

                return result;
            }
            else
            {
                List<RezultatUkupnoExtended> result = new List<RezultatUkupnoExtended>();
                foreach (RezultatUkupno r in getRezultati())
                    result.Add(new RezultatUkupnoExtended(r));
                return result;
            }
        }

        public virtual void addOcena(Ocena o, RezultatskoTakmicenje rezTak)
        {
            RezultatUkupno r = getRezultat(o.Gimnasticar);
            if (r != null)
            {
                r.addSprava(o, rezTak.Propozicije.ZaPreskokVisebojRacunajBoljuOcenu);
                rankRezultati(rezTak.Propozicije);
            }
        }

        private RezultatUkupno getRezultat(GimnasticarUcesnik g)
        {
            foreach (RezultatUkupno r in Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                    return r;
            }
            return null;
        }

        public virtual void deleteOcena(Ocena o, RezultatskoTakmicenje rezTak)
        {
            RezultatUkupno r = getRezultat(o.Gimnasticar);
            if (r != null)
            {
                r.clearSprava(o.Sprava);
                rankRezultati(rezTak.Propozicije);
            }
        }

        public virtual void editOcena(Ocena o, RezultatskoTakmicenje rezTak)
        {
            RezultatUkupno r = getRezultat(o.Gimnasticar);
            if (r != null)
            {
                r.clearSprava(o.Sprava);
                r.addSprava(o, rezTak.Propozicije.ZaPreskokVisebojRacunajBoljuOcenu);
                rankRezultati(rezTak.Propozicije);
            }
        }

        public virtual void addGimnasticar(GimnasticarUcesnik g, IList<Ocena> ocene, RezultatskoTakmicenje rezTak)
        {
            RezultatUkupno r = new RezultatUkupno();
            r.Gimnasticar = g;
            if (ocene.Count > 0)
            {
                foreach (Ocena o in ocene)
                {
                    if (o.Gimnasticar.Id == g.Id)
                        r.addSprava(o, rezTak.Propozicije.ZaPreskokVisebojRacunajBoljuOcenu);
                }
            }
            if (g.PenaltyViseboj != null)
                r.addPenalty(g.PenaltyViseboj.Value);
            Rezultati.Add(r);
            rankRezultati(rezTak.Propozicije);
        }

        public virtual void deleteGimnasticar(GimnasticarUcesnik g, RezultatskoTakmicenje rezTak)
        {
            RezultatUkupno r = getRezultat(g);
            if (r != null)
            {
                Rezultati.Remove(r);
                rankRezultati(rezTak.Propozicije);
            }
        }

        public virtual void promeniPenalizaciju(RezultatUkupno r, float? penalty, RezultatskoTakmicenje rezTak)
        {
            r.promeniPenalizacijuZaViseboj(penalty);
            rankRezultati(rezTak.Propozicije);
        }

        public virtual bool hasPenalty()
        {
            foreach (RezultatUkupno r in Rezultati)
            {
                if (r.Penalty != null)
                    return true;
            }
            return false;
        }

        public virtual void dumpRezultati(StreamWriter streamWriter)
        {
            string header = DeoTakmicenjaKod == DeoTakmicenjaKod.Takmicenje1 ? "VISEBOJ" : "VISEBOJ - FINALE";
            streamWriter.WriteLine(header);
            foreach (RezultatUkupno r in getRezultati())
            {
                string line = r.Rank + ". " + r.Gimnasticar.ImeSrednjeImePrezimeDatumRodjenja + "   " + r.Total;
                streamWriter.WriteLine(line);
            }
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(DeoTakmicenjaKod.ToString());

            strBuilder.AppendLine(Rezultati.Count.ToString());
            foreach (RezultatUkupno r in Rezultati)
                r.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            DeoTakmicenjaKod = (DeoTakmicenjaKod)Enum.Parse(typeof(DeoTakmicenjaKod), reader.ReadLine());

            int brojRezultata = int.Parse(reader.ReadLine());
            for (int i = 0; i < brojRezultata; ++i)
            {
                reader.ReadLine();  // id
                RezultatUkupno r = new RezultatUkupno();
                r.loadFromDump(reader, map);
                Rezultati.Add(r);
            }
        }
    }
}
