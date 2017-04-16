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
            if (Sprava == Sprava.Preskok)
            {
                createPreskok(rezTak, rezTak1, rezTak2);
                return;
            }

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
                // Total moze da bude krajnja finalna ocena ili ulazna finalna ocena. U oba slucaja se Total izracunava
                // na isti nacin.
                r.calculateTotal(rezTak.Propozicije.NacinRacunanjaOceneFinaleKupaTak3,
                    rezTak.Propozicije.Tak3NeRacunajProsekAkoNemaOceneIzObaKola);
                Rezultati.Add(r);
            }

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
        }

        // TODO3: Za sve poretke (ukupno, sprava, ekipno, kako za finale kupa, tako i za obicna takmicenja) specifikuj
        // pravila razresavanja istih ocena.

        private void createPreskok(RezultatskoTakmicenje rezTak, RezultatskoTakmicenje rezTak1,
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

            // TODO4: Razmisli da li treba uvoditi opciju u propozicijama sta da se radi kada u nekom od prethodnih kola
            // deo takmicara ima obe ocene za preskok a deo ima samo jednu (ili se kao i do sada oslanjati na to kako je
            // to specifikovano u propozicijama za 1. i 2. kolo, sto mislim da je bolja varijanta)

            bool postojiTotalObeOcene = false;
            foreach (RezultatPreskok r in rezTak1.Takmicenje1.PoredakPreskok.Rezultati)
            {
                if (rezultatiMap.ContainsKey(r.Gimnasticar))
                {
                    RezultatSpravaFinaleKupa r2 = rezultatiMap[r.Gimnasticar];
                    // TODO4: Koriscenje opcije PoredakTak3PreskokNaOsnovuObaPreskoka ce raditi samo ako u prvom kolu
                    // nije bilo odvojenog takmicena 3; ako je bilo odvojenog takmicenja 3, tada nam umesto
                    // PoredakTak3PreskokNaOsnovuObaPreskoka treba KvalifikantiTak3PreskokNaOsnovuObaPreskoka.
                    // Postoji vec jedan TODO4 da se resi ovaj problem i da vrednost koja se odnosi na takmicenje 1 uvek
                    // bude u samo jednoj opciji.
                    if (!rezTak1.Propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka)
                        r2.initPrvoKolo(r);
                    else
                    {
                        r2.D_PrvoKolo = null;
                        r2.E_PrvoKolo = null;
                        r2.TotalPrvoKolo = r.TotalObeOcene;
                        postojiTotalObeOcene |= (r.TotalObeOcene != null);
                    }
                }
            }
            if (rezTak1.Propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka && !postojiTotalObeOcene)
            {
                // U propozicijama za prvo kolo je stavljeno da se preskok racuna na osnovu
                // oba preskoka, ali ni za jednog gimnasticara ne postoji ocena za oba preskoka.
                // Ova situacija najverovatnije nastaje kada se u prvom kolu kao prvi preskok
                // unosila konacna ocena za oba preskoka.
                // U tom slucaju, za ocenu prvog kola treba uzeti prvu ocenu.
                foreach (RezultatPreskok r in rezTak1.Takmicenje1.PoredakPreskok.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Gimnasticar))
                        rezultatiMap[r.Gimnasticar].initPrvoKolo(r);
                }
            }

            postojiTotalObeOcene = false;
            foreach (RezultatPreskok r in rezTak2.Takmicenje1.PoredakPreskok.Rezultati)
            {
                if (rezultatiMap.ContainsKey(r.Gimnasticar))
                {
                    RezultatSpravaFinaleKupa r2 = rezultatiMap[r.Gimnasticar];
                    if (!rezTak2.Propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka)
                        r2.initDrugoKolo(r);
                    else
                    {
                        r2.D_DrugoKolo = null;
                        r2.E_DrugoKolo = null;
                        r2.TotalDrugoKolo = r.TotalObeOcene;
                        postojiTotalObeOcene |= (r.TotalObeOcene != null);
                    }
                }
            }
            if (rezTak2.Propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka && !postojiTotalObeOcene)
            {
                // Isti komentar kao za prvo kolo.
                foreach (RezultatPreskok r in rezTak2.Takmicenje1.PoredakPreskok.Rezultati)
                {
                    if (rezultatiMap.ContainsKey(r.Gimnasticar))
                        rezultatiMap[r.Gimnasticar].initDrugoKolo(r);
                }
            }

            Rezultati.Clear();
            foreach (RezultatSpravaFinaleKupa r in rezultatiMap.Values)
            {
                r.calculateTotal(rezTak.Propozicije.NacinRacunanjaOceneFinaleKupaTak3,
                    rezTak.Propozicije.Tak3NeRacunajProsekAkoNemaOceneIzObaKola);
                Rezultati.Add(r);
            }

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
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

        private void updateKvalStatus(Propozicije propozicije)
        {
            if (!propozicije.odvojenoTak3())
            {
                foreach (RezultatSpravaFinaleKupa r in Rezultati)
                    r.KvalStatus = KvalifikacioniStatus.None;
                return;
            }
            
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

        public virtual void addGimnasticar(GimnasticarUcesnik g, RezultatskoTakmicenje rezTak,
            RezultatskoTakmicenje rezTak1, RezultatskoTakmicenje rezTak2)
        {
            RezultatSpravaFinaleKupa rezultat = new RezultatSpravaFinaleKupa();
            rezultat.Gimnasticar = g;

            if (Sprava != Sprava.Preskok)
            {
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
            }
            else
            {
 
            }

            rezultat.calculateTotal(rezTak.Propozicije.NacinRacunanjaOceneFinaleKupaTak3,
                rezTak.Propozicije.Tak3NeRacunajProsekAkoNemaOceneIzObaKola);
            Rezultati.Add(rezultat);

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
        }

        public virtual List<RezultatSpravaFinaleKupa> getRezultati()
        {
            List<RezultatSpravaFinaleKupa> result = new List<RezultatSpravaFinaleKupa>(Rezultati);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatSpravaFinaleKupa))["RedBroj"];
            result.Sort(new SortComparer<RezultatSpravaFinaleKupa>(propDesc, ListSortDirection.Ascending));

            return result;
        }

        public virtual List<RezultatSpravaFinaleKupa> getKvalifikanti()
        {
            List<RezultatSpravaFinaleKupa> result = new List<RezultatSpravaFinaleKupa>();
            foreach (RezultatSpravaFinaleKupa rez in getRezultati())
            {
                if (rez.KvalStatus == KvalifikacioniStatus.Q)
                    result.Add(rez);
            }
            return result;
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
