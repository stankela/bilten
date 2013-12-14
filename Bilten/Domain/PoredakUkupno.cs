using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

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
            private set { _rezultati = value; }
        }

        protected PoredakUkupno()
        { 
        
        }

        public PoredakUkupno(DeoTakmicenjaKod deoTakKod)
        {
            this.deoTakKod = deoTakKod;
        }

        public virtual void initRezultati(IList<GimnasticarUcesnik> gimnasticari)
        {
            Rezultati.Clear();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                RezultatUkupno r = new RezultatUkupno();
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
                gimnasticari = new List<GimnasticarUcesnik>(rezTak.Takmicenje1.Gimnasticari);
            else
                gimnasticari = new List<GimnasticarUcesnik>(rezTak.Takmicenje2.getUcesniciGimKvalifikanti());

            IDictionary<int, RezultatUkupno> rezultatiMap = new Dictionary<int, RezultatUkupno>();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                RezultatUkupno rezultat = new RezultatUkupno();
                rezultat.Gimnasticar = g;
                rezultatiMap.Add(g.Id, rezultat);
            }

            foreach (Ocena o in ocene)
            {
                if (rezultatiMap.ContainsKey(o.Gimnasticar.Id))
                    rezultatiMap[o.Gimnasticar.Id].addOcena(o);
            }

            List<RezultatUkupno> rezultati = new List<RezultatUkupno>(rezultatiMap.Values);
            Rezultati.Clear();
            foreach (RezultatUkupno rez in rezultati)
                Rezultati.Add(rez);

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
        }

        private void rankRezultati()
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
            if (deoTakKod != DeoTakmicenjaKod.Takmicenje1)
                return;
            if (!propozicije.PostojiTak2)
                return;

            doUpdateKvalStatus(propozicije.OdvojenoTak2, 
                propozicije.BrojFinalistaTak2, 
                propozicije.NeogranicenBrojTakmicaraIzKlubaTak2, 
                propozicije.MaxBrojTakmicaraIzKlubaTak2, 
                propozicije.BrojRezerviTak2);

        }

        private void doUpdateKvalStatus(bool odvojenoFinale, int brojFinalista, 
            bool neogranicenBrojTakmicaraIzKluba, int maxBrojTakmicaraIzKluba, 
            int brojRezervi)
        {
            // TODO: Obradi situaciju kada ima npr. 24 finalista, a 24 i 25 takmicar
            // imaju isti total (uradi i za PoredakSprava i PoredakEkipno)

            List<RezultatUkupno> rezultati = new List<RezultatUkupno>(Rezultati);
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatUkupno))["RedBroj"];
            rezultati.Sort(new SortComparer<RezultatUkupno>(propDesc,
                ListSortDirection.Ascending));

            // moram da koristim dve mape zato sto je moguca situacija da klub i 
            // drzava imaju isti id
            IDictionary<int, int> brojTakmicaraKlubMap = new Dictionary<int, int>();
            IDictionary<int, int> brojTakmicaraDrzavaMap = new Dictionary<int, int>();

            int finCount = 0;
            int rezCount = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                RezultatUkupno rezulat = rezultati[i];
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

                if (odvojenoFinale)
                {
                    if (finCount < brojFinalista)
                    {
                        if (neogranicenBrojTakmicaraIzKluba)
                        {
                            finCount++;
                            rezulat.KvalStatus = KvalifikacioniStatus.Q;
                        }
                        else
                        {
                            if (brojTakmicaraMap[id] < maxBrojTakmicaraIzKluba)
                            {
                                finCount++;
                                brojTakmicaraMap[id]++;
                                rezulat.KvalStatus = KvalifikacioniStatus.Q;
                            }
                            else
                            {
                                if (Opcije.Instance.UzimajPrvuSlobodnuRezervu
                                && rezCount < brojRezervi)
                                {
                                    rezCount++;
                                    rezulat.KvalStatus = KvalifikacioniStatus.R;
                                }
                                else
                                    rezulat.KvalStatus = KvalifikacioniStatus.None;
                            }
                        }
                    }
                    else if (rezCount < brojRezervi)
                    {
                        if (neogranicenBrojTakmicaraIzKluba)
                        {
                            rezCount++;
                            rezulat.KvalStatus = KvalifikacioniStatus.R;
                        }
                        else
                        {
                            if (brojTakmicaraMap[id] < maxBrojTakmicaraIzKluba)
                            {
                                rezCount++;
                                brojTakmicaraMap[id]++;
                                rezulat.KvalStatus = KvalifikacioniStatus.R;
                            }
                            else
                            {
                                if (Opcije.Instance.UzimajPrvuSlobodnuRezervu
                                && rezCount < brojRezervi)
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
                else
                {
                    if (neogranicenBrojTakmicaraIzKluba)
                    {
                        // u ovom slucaju moze da se stavi i None, tj. svejedno je
                        // sta se stavlja posto ce svi takmicari imati istu oznaku
                        rezulat.KvalStatus = KvalifikacioniStatus.Q;
                    }
                    else
                    {
                        if (brojTakmicaraMap[id] < maxBrojTakmicaraIzKluba)
                        {
                            brojTakmicaraMap[id]++;
                            rezulat.KvalStatus = KvalifikacioniStatus.Q;
                        }
                        else
                        {
                            rezulat.KvalStatus = KvalifikacioniStatus.None;
                        }
                    }
                }
            }
        }

        public virtual List<RezultatUkupno> getRezultati()
        {
            List<RezultatUkupno> result = new List<RezultatUkupno>(Rezultati);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatUkupno))["RedBroj"];
            result.Sort(new SortComparer<RezultatUkupno>(propDesc,
                ListSortDirection.Ascending));

            return result;
        }

        public virtual List<RezultatUkupnoExtended> getRezultatiExtended(IList<Ocena> ocene, bool prikaziDEOcene)
        {
            if (prikaziDEOcene)
            {
                IDictionary<int, RezultatUkupnoExtended> rezultatiMap = new Dictionary<int, RezultatUkupnoExtended>();
                foreach (RezultatUkupno rez in Rezultati)
                {
                    RezultatUkupnoExtended rezEx = new RezultatUkupnoExtended(rez);
                    rezultatiMap.Add(rezEx.Gimnasticar.Id, rezEx);
                }

                foreach (Ocena o in ocene)
                {
                    if (rezultatiMap.ContainsKey(o.Gimnasticar.Id))
                    {
                        rezultatiMap[o.Gimnasticar.Id].setDOcena(o.Sprava, o.D);
                        rezultatiMap[o.Gimnasticar.Id].setEOcena(o.Sprava, o.E);
                    }
                }

                List<RezultatUkupnoExtended> result = new List<RezultatUkupnoExtended>(rezultatiMap.Values);

                PropertyDescriptor propDesc =
                    TypeDescriptor.GetProperties(typeof(RezultatUkupnoExtended))["RedBroj"];
                result.Sort(new SortComparer<RezultatUkupnoExtended>(propDesc,
                    ListSortDirection.Ascending));

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
                r.addOcena(o);
                rankRezultati();
                updateKvalStatus(rezTak.Propozicije);
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
                r.removeOcena(o);
                rankRezultati();
                updateKvalStatus(rezTak.Propozicije);
            }
        }

        public virtual void editOcena(Ocena o, Ocena old, RezultatskoTakmicenje rezTak)
        {
            RezultatUkupno r = getRezultat(o.Gimnasticar);
            if (r != null)
            {
                r.removeOcena(old);
                r.addOcena(o);
                rankRezultati();
                updateKvalStatus(rezTak.Propozicije);
            }
        }

        public virtual void addGimnasticar(GimnasticarUcesnik g, IList<Ocena> ocene, 
            RezultatskoTakmicenje rezTak)
        {
            RezultatUkupno r = new RezultatUkupno();
            r.Gimnasticar = g;
            if (ocene.Count > 0)
            {
                foreach (Ocena o in ocene)
                    r.addOcena(o);
            }
            Rezultati.Add(r);

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
        }

        public virtual void deleteGimnasticar(GimnasticarUcesnik g, 
            RezultatskoTakmicenje rezTak)
        {
            RezultatUkupno r = getRezultat(g);
            if (r != null)
            {
                Rezultati.Remove(r);
                rankRezultati();
                updateKvalStatus(rezTak.Propozicije);
            }
        }
    }
}
