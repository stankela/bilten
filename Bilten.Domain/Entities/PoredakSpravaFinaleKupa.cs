using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Iesi.Collections.Generic;

namespace Bilten.Domain
{
    public class PoredakSpravaFinaleKupa : DomainObject
    {
        private IList<RezultatSpravaFinaleKupa> _rezultati = new List<RezultatSpravaFinaleKupa>();
        public virtual IList<RezultatSpravaFinaleKupa> Rezultati
        {
            get { return _rezultati; }
            protected set { _rezultati = value; }
        }

        private Sprava _sprava;
        public virtual Sprava Sprava
        {
            get { return _sprava; }
            set { _sprava = value; }
        }

        public PoredakSpravaFinaleKupa()
        {

        }

        // TODO3: Kod finala kupa bi trebalo kod kloniranja ubaciti gimnasticare iz oba kola.

        public virtual void create(RezultatskoTakmicenje rezTak, PoredakSprava poredakPrvoKolo,
            PoredakSprava poredakDrugoKolo, List<RezultatSpravaFinaleKupaUpdate> rezultatiUpdate)
        {
            IList<GimnasticarUcesnik> gimnasticari = new List<GimnasticarUcesnik>(rezTak.Takmicenje1.Gimnasticari);

            IDictionary<GimnasticarUcesnik, RezultatSpravaFinaleKupa> rezultatiMap =
                new Dictionary<GimnasticarUcesnik, RezultatSpravaFinaleKupa>();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                RezultatSpravaFinaleKupa rezultat = new RezultatSpravaFinaleKupa();
                rezultat.Gimnasticar = g;
                rezultatiMap.Add(g, rezultat);
            }

            if (poredakPrvoKolo != null)
            {
                foreach (RezultatSprava r in poredakPrvoKolo.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Gimnasticar))
                    {
                        RezultatSpravaFinaleKupa r2 = rezultatiMap[r.Gimnasticar];
                        r2.D_PrvoKolo = r.D;
                        r2.E_PrvoKolo = r.E;
                        r2.TotalPrvoKolo = r.Total;
                    }
                }
            }

            if (poredakDrugoKolo != null)
            {
                foreach (RezultatSprava r in poredakDrugoKolo.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Gimnasticar))
                    {
                        RezultatSpravaFinaleKupa r2 = rezultatiMap[r.Gimnasticar];
                        r2.D_DrugoKolo = r.D;
                        r2.E_DrugoKolo = r.E;
                        r2.TotalDrugoKolo = r.Total;
                    }
                }
            }

            List<RezultatSpravaFinaleKupa> rezultati = new List<RezultatSpravaFinaleKupa>(rezultatiMap.Values);
            Rezultati.Clear();
            foreach (RezultatSpravaFinaleKupa r in rezultati)
                Rezultati.Add(r);

            // Total moze da bude krajnja finalna ocena ili ulazna finalna ocena. U oba slucaja se Total izracunava
            // na isti nacin.
            foreach (RezultatSpravaFinaleKupa rez in Rezultati)
            {
                if (rez.TotalPrvoKolo == null && rez.TotalDrugoKolo == null)
                {
                    rez.setTotal(null);
                    continue;
                }
                float total1 = rez.TotalPrvoKolo == null ? 0 : rez.TotalPrvoKolo.Value;
                float total2 = rez.TotalDrugoKolo == null ? 0 : rez.TotalDrugoKolo.Value;
                float total;

                if (rezTak.Propozicije.Tak3FinalnaOcenaJeZbirObaKola)
                    total = total1 + total2;
                else if (rezTak.Propozicije.Tak3FinalnaOcenaJeMaxObaKola)
                    total = total1 > total2 ? total1 : total2;
                else
                {
                    // TODO3: Proveri da li ovde treba podesavati broj decimala.
                    total = (total1 + total2) / 2;
                    if (rezTak.Propozicije.Tak3NeRacunajProsekAkoNemaOceneIzObaKola
                        && (rez.TotalPrvoKolo == null || rez.TotalDrugoKolo == null))
                    {
                        total = total1 + total2;
                    }
                }
                rez.setTotal(total);
            }

            rankRezultati();
            if (rezTak.Propozicije.OdvojenoTak3)
            {
                updateKvalStatus(rezTak.Propozicije.BrojFinalistaTak3,
                                 rezTak.Propozicije.NeogranicenBrojTakmicaraIzKlubaTak3,
                                 rezTak.Propozicije.MaxBrojTakmicaraIzKlubaTak3,
                                 rezTak.Propozicije.MaxBrojTakmicaraTak3VaziZaDrzavu,
                                 rezTak.Propozicije.BrojRezerviTak3);
                applyRezultatiUpdate(rezultatiUpdate);
            }
        }

        private void applyRezultatiUpdate(List<RezultatSpravaFinaleKupaUpdate> rezultatiUpdate)
        {
            foreach (RezultatSpravaFinaleKupaUpdate rezultatUpdate in rezultatiUpdate)
            {
                if (rezultatUpdate.Sprava != this.Sprava)
                    continue;
                foreach (RezultatSpravaFinaleKupa rezultat in Rezultati)
                {
                    if (rezultatUpdate.GimnasticarId == rezultat.Gimnasticar.Id)
                    {
                        rezultat.KvalStatus = rezultatUpdate.KvalStatus;
                        break;
                    }
                }
            }
        }

        // TODO3: Za sve poretke (ukupno, sprava, ekipno, kako za finale kupa, tako i za obicna takmicenja) specifikuj
        // pravila razresavanja istih ocena.

        public virtual void create(RezultatskoTakmicenje rezTak, PoredakPreskok poredakPrvoKolo,
            PoredakPreskok poredakDrugoKolo, bool poredak1NaOsnovuObaPreskoka, bool poredak2NaOsnovuObaPreskoka,
            List<RezultatSpravaFinaleKupaUpdate> rezultatiUpdate)
        {
            IList<GimnasticarUcesnik> gimnasticari = new List<GimnasticarUcesnik>(rezTak.Takmicenje1.Gimnasticari);

            IDictionary<GimnasticarUcesnik, RezultatSpravaFinaleKupa> rezultatiMap =
                new Dictionary<GimnasticarUcesnik, RezultatSpravaFinaleKupa>();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                RezultatSpravaFinaleKupa rezultat = new RezultatSpravaFinaleKupa();
                rezultat.Gimnasticar = g;
                rezultatiMap.Add(g, rezultat);
            }

            if (poredakPrvoKolo != null)
            {
                bool postojiTotalObeOcene = false;
                foreach (RezultatPreskok r in poredakPrvoKolo.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Gimnasticar))
                    {
                        RezultatSpravaFinaleKupa r2 = rezultatiMap[r.Gimnasticar];
                        if (!poredak1NaOsnovuObaPreskoka)
                        {
                            r2.D_PrvoKolo = r.D;
                            r2.E_PrvoKolo = r.E;
                            r2.TotalPrvoKolo = r.Total;
                        }
                        else
                        {
                            r2.D_PrvoKolo = null;
                            r2.E_PrvoKolo = null;
                            r2.TotalPrvoKolo = r.TotalObeOcene;
                            postojiTotalObeOcene |= (r.TotalObeOcene != null);
                        }
                    }
                }
                if (poredak1NaOsnovuObaPreskoka && !postojiTotalObeOcene)
                {
                    // U propozicijama za prvo kolo je stavljeno da se preskok racuna na osnovu
                    // oba preskoka, ali ni za jednog gimnasticara ne postoji ocena za oba preskoka.
                    // Ova situacija najverovatnije nastaje kada se u prvom kolu kao prvi preskok
                    // unosila konacna ocena za oba preskoka.
                    // U tom slucaju, za ocenu prvog kola treba uzeti prvu ocenu.
                    foreach (RezultatPreskok r in poredakPrvoKolo.Rezultati)
                    {
                        if (rezultatiMap.ContainsKey(r.Gimnasticar))
                        {
                            RezultatSpravaFinaleKupa r2 = rezultatiMap[r.Gimnasticar];
                            r2.D_PrvoKolo = r.D;
                            r2.E_PrvoKolo = r.E;
                            r2.TotalPrvoKolo = r.Total;
                        }
                    }
                }
            }

            if (poredakDrugoKolo != null)
            {
                bool postojiTotalObeOcene = false;
                foreach (RezultatPreskok r in poredakDrugoKolo.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Gimnasticar))
                    {
                        RezultatSpravaFinaleKupa r2 = rezultatiMap[r.Gimnasticar];
                        if (!poredak2NaOsnovuObaPreskoka)
                        {
                            r2.D_DrugoKolo = r.D;
                            r2.E_DrugoKolo = r.E;
                            r2.TotalDrugoKolo = r.Total;
                        }
                        else
                        {
                            r2.D_DrugoKolo = null;
                            r2.E_DrugoKolo = null;
                            r2.TotalDrugoKolo = r.TotalObeOcene;
                            postojiTotalObeOcene |= (r.TotalObeOcene != null);
                        }
                    }
                }
                if (poredak2NaOsnovuObaPreskoka && !postojiTotalObeOcene)
                {
                    // Isti komentar kao za prvo kolo.
                    foreach (RezultatPreskok r in poredakDrugoKolo.Rezultati)
                    {
                        if (rezultatiMap.ContainsKey(r.Gimnasticar))
                        {
                            RezultatSpravaFinaleKupa r2 = rezultatiMap[r.Gimnasticar];
                            r2.D_DrugoKolo = r.D;
                            r2.E_DrugoKolo = r.E;
                            r2.TotalDrugoKolo = r.Total;
                        }
                    }
                }
            }

            List<RezultatSpravaFinaleKupa> rezultati = new List<RezultatSpravaFinaleKupa>(rezultatiMap.Values);
            Rezultati.Clear();
            foreach (RezultatSpravaFinaleKupa r in rezultati)
                Rezultati.Add(r);

            // Total moze da bude krajnja finalna ocena ili ulazna finalna ocena. U oba slucaja se Total izracunava
            // na isti nacin.
            foreach (RezultatSpravaFinaleKupa rez in Rezultati)
            {
                if (rez.TotalPrvoKolo == null && rez.TotalDrugoKolo == null)
                {
                    rez.setTotal(null);
                    continue;
                }
                float total1 = rez.TotalPrvoKolo == null ? 0 : rez.TotalPrvoKolo.Value;
                float total2 = rez.TotalDrugoKolo == null ? 0 : rez.TotalDrugoKolo.Value;
                float total;

                if (rezTak.Propozicije.Tak3FinalnaOcenaJeZbirObaKola)
                    total = total1 + total2;
                else if (rezTak.Propozicije.Tak3FinalnaOcenaJeMaxObaKola)
                    total = total1 > total2 ? total1 : total2;
                else
                {
                    // TODO3: Proveri da li ovde treba podesavati broj decimala.
                    total = (total1 + total2) / 2;
                    if (rezTak.Propozicije.Tak3NeRacunajProsekAkoNemaOceneIzObaKola
                        && (rez.TotalPrvoKolo == null || rez.TotalDrugoKolo == null))
                    {
                        total = total1 + total2;
                    }
                }
                rez.setTotal(total);
            }

            rankRezultati();
            if (rezTak.Propozicije.OdvojenoTak3)
            {
                updateKvalStatus(rezTak.Propozicije.BrojFinalistaTak3,
                                 rezTak.Propozicije.NeogranicenBrojTakmicaraIzKlubaTak3,
                                 rezTak.Propozicije.MaxBrojTakmicaraIzKlubaTak3,
                                 rezTak.Propozicije.MaxBrojTakmicaraTak3VaziZaDrzavu,
                                 rezTak.Propozicije.BrojRezerviTak3);
                applyRezultatiUpdate(rezultatiUpdate);
            }
        }

        private void rankRezultati()
        {
            List<RezultatSpravaFinaleKupa> rezultati = new List<RezultatSpravaFinaleKupa>(Rezultati);

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
        }

        private void updateKvalStatus(int brojFinalista,
            bool neogranicenBrojTakmicaraIzKluba, int maxBrojTakmicaraIzKluba,
            bool maxBrojTakmicaraVaziZaDrzavu, int brojRezervi)
        {
            List<RezultatSpravaFinaleKupa> rezultati = new List<RezultatSpravaFinaleKupa>(Rezultati);
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
                RezultatSpravaFinaleKupa rezulat = rezultati[i];
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
        }

        public List<RezultatSpravaFinaleKupa> getRezultati()
        {
            List<RezultatSpravaFinaleKupa> result = new List<RezultatSpravaFinaleKupa>(Rezultati);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatSpravaFinaleKupa))["RedBroj"];
            result.Sort(new SortComparer<RezultatSpravaFinaleKupa>(propDesc, ListSortDirection.Ascending));

            return result;
        }

        public List<RezultatSpravaFinaleKupa> getKvalifikanti()
        {
            List<RezultatSpravaFinaleKupa> result = new List<RezultatSpravaFinaleKupa>();
            foreach (RezultatSpravaFinaleKupa rez in getRezultati())
            {
                if (rez.KvalStatus == KvalifikacioniStatus.Q)
                    result.Add(rez);
            }
            return result;
        }
    }
}
