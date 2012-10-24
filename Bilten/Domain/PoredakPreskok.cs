using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Iesi.Collections.Generic;

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

        private IList<RezultatPreskok> _rezultati = new List<RezultatPreskok>();
        public virtual IList<RezultatPreskok> Rezultati
        {
            get { return _rezultati; }
            private set { _rezultati = value; }
        }

        private Sprava _sprava;
        public virtual Sprava Sprava
        {
            get { return _sprava; }
            private set { _sprava = value; }
        }

        protected PoredakPreskok()
        { 
        
        }

        public PoredakPreskok(DeoTakmicenjaKod deoTakKod)
        {
            this.deoTakKod = deoTakKod;
            this._sprava = Sprava.Preskok;
        }

        public virtual void initRezultati(IList<GimnasticarUcesnik> gimnasticari)
        {
            Rezultati.Clear();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                RezultatPreskok r = new RezultatPreskok();
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
                gimnasticari = getGimnasticari(ocene, rezTak);
            else
                gimnasticari = rezTak.Takmicenje3.getUcesniciGimKvalifikanti(Sprava.Preskok);

            IDictionary<int, RezultatPreskok> rezultatiMap = new Dictionary<int, RezultatPreskok>();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                RezultatPreskok rezultat = new RezultatPreskok();
                rezultat.Gimnasticar = g;
                rezultatiMap.Add(g.Id, rezultat);
            }

            foreach (Ocena o in ocene)
            {
                if (o.Sprava == Sprava.Preskok && rezultatiMap.ContainsKey(o.Gimnasticar.Id))
                    rezultatiMap[o.Gimnasticar.Id].setOcena(o);
            }

            List<RezultatPreskok> rezultati = new List<RezultatPreskok>(rezultatiMap.Values);
            Rezultati.Clear();
            foreach (RezultatPreskok rez in rezultati)
                Rezultati.Add(rez);

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
        }

        private void rankRezultati()
        {
            List<RezultatPreskok> rezultati = new List<RezultatPreskok>(Rezultati);

            PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                TypeDescriptor.GetProperties(typeof(RezultatPreskok))["Total"],
                TypeDescriptor.GetProperties(typeof(RezultatPreskok))["PrezimeIme"]
            };
            ListSortDirection[] sortDir = new ListSortDirection[] {
                ListSortDirection.Descending,
                ListSortDirection.Ascending
            };
            rezultati.Sort(new SortComparer<RezultatPreskok>(propDesc, sortDir));

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

            propDesc = new PropertyDescriptor[] {
                TypeDescriptor.GetProperties(typeof(RezultatPreskok))["TotalObeOcene"],
                TypeDescriptor.GetProperties(typeof(RezultatPreskok))["PrezimeIme"]
            };
            rezultati.Sort(new SortComparer<RezultatPreskok>(propDesc, sortDir));

            prevTotal = -1f;
            prevRank = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                rezultati[i].RedBroj2 = (short)(i + 1);
                if (rezultati[i].TotalObeOcene == null)
                {
                    rezultati[i].Rank2 = null;
                }
                else
                {
                    if (rezultati[i].TotalObeOcene != prevTotal)
                        rezultati[i].Rank2 = (short)(i + 1);
                    else
                        rezultati[i].Rank2 = prevRank;

                    prevTotal = rezultati[i].TotalObeOcene.Value;
                    prevRank = rezultati[i].Rank2.Value;
                }
            }
        }

        private IList<GimnasticarUcesnik> getGimnasticari(IList<Ocena> ocene,
            RezultatskoTakmicenje rezTak)
        {
            ISet<int> idSet = new HashedSet<int>();
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

        private void updateKvalStatus(Propozicije propozicije)
        {
            if (deoTakKod != DeoTakmicenjaKod.Takmicenje1)
                return;
            if (!propozicije.PostojiTak3)
                return;

            doUpdateKvalStatus(propozicije.OdvojenoTak3,
                propozicije.BrojFinalistaTak3,
                propozicije.NeogranicenBrojTakmicaraIzKlubaTak3,
                propozicije.MaxBrojTakmicaraIzKlubaTak3,
                propozicije.MaxBrojTakmicaraTak3VaziZaDrzavu,
                propozicije.BrojRezerviTak3, propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka);

        }

        private void doUpdateKvalStatus(bool odvojenoFinale, int brojFinalista,
            bool neogranicenBrojTakmicaraIzKluba, int maxBrojTakmicaraIzKluba,
            bool maxBrojTakmicaraVaziZaDrzavu,
            int brojRezervi, bool obaPreskoka)
        {
            List<RezultatPreskok> rezultati = new List<RezultatPreskok>(Rezultati);
            
            string propName = obaPreskoka ? "RedBroj2" : "RedBroj";
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatPreskok))[propName];
            rezultati.Sort(new SortComparer<RezultatPreskok>(propDesc,
                ListSortDirection.Ascending));

            // moram da koristim dve mape zato sto je moguca situacija da klub i 
            // drzava imaju isti id
            IDictionary<int, int> brojTakmicaraKlubMap = new Dictionary<int, int>();
            IDictionary<int, int> brojTakmicaraDrzavaMap = new Dictionary<int, int>();

            int finCount = 0;
            int rezCount = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                RezultatPreskok rezulat = rezultati[i];
                if (!obaPreskoka && rezulat.Total == null
                || obaPreskoka && rezulat.TotalObeOcene == null)
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

        public virtual List<RezultatPreskok> getRezultatiDvaPreskoka()
        {
            List<RezultatPreskok> result = new List<RezultatPreskok>();
            foreach (RezultatPreskok rez in Rezultati)
            {
                if (rez.Rank2 != null)
                    result.Add(rez);
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
                rankRezultati();
                updateKvalStatus(rezTak.Propozicije);
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

                rankRezultati();
                updateKvalStatus(rezTak.Propozicije);
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
                rankRezultati();
                updateKvalStatus(rezTak.Propozicije);
            }
        }

        public virtual void addGimnasticar(GimnasticarUcesnik g, Ocena o, 
            RezultatskoTakmicenje rezTak)
        {
            RezultatPreskok r = new RezultatPreskok();
            r.Gimnasticar = g;
            r.setOcena(o);
            Rezultati.Add(r);

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
        }

        public virtual void deleteGimnasticar(GimnasticarUcesnik g, 
            RezultatskoTakmicenje rezTak)
        {
            RezultatPreskok r = getRezultat(g);
            if (r != null)
            {
                Rezultati.Remove(r);
                rankRezultati();
                updateKvalStatus(rezTak.Propozicije);
            }
        }
    }
}
