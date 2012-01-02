using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Iesi.Collections.Generic;

namespace Bilten.Domain
{
    public class PoredakSprava : DomainObject
    {
        private IList<RezultatSprava> _rezultati = new List<RezultatSprava>();
        public virtual IList<RezultatSprava> Rezultati
        {
            get { return _rezultati; }
            private set { _rezultati = value; }
        }

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
            if (DeoTakmicenjaKod == DeoTakmicenjaKod.Takmicenje1)
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
            List<RezultatSprava> rezultati = new List<RezultatSprava>(Rezultati);

            PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                TypeDescriptor.GetProperties(typeof(RezultatSprava))["Total"],
                TypeDescriptor.GetProperties(typeof(RezultatSprava))["PrezimeIme"]
            };
            ListSortDirection[] sortDir = new ListSortDirection[] {
                ListSortDirection.Descending,
                ListSortDirection.Ascending
            };
            rezultati.Sort(new SortComparer<RezultatSprava>(propDesc, sortDir));

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

        private IList<GimnasticarUcesnik> getGimnasticari(IList<Ocena> ocene,
            Sprava sprava, RezultatskoTakmicenje rezTak)
        {
            ISet<int> idSet = new HashedSet<int>();
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
            if (DeoTakmicenjaKod != DeoTakmicenjaKod.Takmicenje1)
                return;
            if (!propozicije.PostojiTak3)
                return;

            doUpdateKvalStatus(propozicije.OdvojenoTak3,
                propozicije.BrojFinalistaTak3,
                propozicije.NeogranicenBrojTakmicaraIzKlubaTak3,
                propozicije.MaxBrojTakmicaraIzKlubaTak3,
                propozicije.MaxBrojTakmicaraTak3VaziZaDrzavu,
                propozicije.BrojRezerviTak3);

        }

        private void doUpdateKvalStatus(bool odvojenoFinale, int brojFinalista,
            bool neogranicenBrojTakmicaraIzKluba, int maxBrojTakmicaraIzKluba,
            bool maxBrojTakmicaraVaziZaDrzavu, int brojRezervi)
        {
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
            for (int i = 0; i < rezultati.Count; i++)
            {
                RezultatSprava rezulat = rezultati[i];
                if (rezulat.Total == null)
                {
                    rezulat.KvalStatus = KvalifikacioniStatus.None;
                    continue;
                }

                int id;
                IDictionary<int, int> brojTakmicaraMap;

                if (maxBrojTakmicaraVaziZaDrzavu)
                {
                    if (rezulat.Gimnasticar.DrzavaUcesnik != null)
                    {
                        id = rezulat.Gimnasticar.DrzavaUcesnik.Id;
                        brojTakmicaraMap = brojTakmicaraDrzavaMap;
                    }
                    else
                    {
                        id = rezulat.Gimnasticar.KlubUcesnik.Id;
                        brojTakmicaraMap = brojTakmicaraKlubMap;
                    }
                }
                else
                {
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
    }
}
