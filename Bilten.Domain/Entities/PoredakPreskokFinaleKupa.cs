using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Bilten.Util;
using System.IO;

namespace Bilten.Domain
{
    public class PoredakPreskokFinaleKupa : DomainObject
    {
        private bool koristiTotalObeOcenePrvoKolo;
        public virtual bool KoristiTotalObeOcenePrvoKolo
        {
            get { return koristiTotalObeOcenePrvoKolo; }
            set { koristiTotalObeOcenePrvoKolo = value; }
        }

        private bool koristiTotalObeOceneDrugoKolo;
        public virtual bool KoristiTotalObeOceneDrugoKolo
        {
            get { return koristiTotalObeOceneDrugoKolo; }
            set { koristiTotalObeOceneDrugoKolo = value; }
        }
        
        private IList<RezultatPreskokFinaleKupa> _rezultati = new List<RezultatPreskokFinaleKupa>();
        public virtual IList<RezultatPreskokFinaleKupa> Rezultati
        {
            get { return _rezultati; }
            protected set { _rezultati = value; }
        }

        public PoredakPreskokFinaleKupa()
        {

        }

        public virtual void create(RezultatskoTakmicenje rezTak, RezultatskoTakmicenje rezTak1,
            RezultatskoTakmicenje rezTak2)
        {
            IDictionary<GimnasticarUcesnik, RezultatPreskokFinaleKupa> rezultatiMap =
                new Dictionary<GimnasticarUcesnik, RezultatPreskokFinaleKupa>();
            foreach (GimnasticarUcesnik g in rezTak.Takmicenje1.Gimnasticari)
            {
                RezultatPreskokFinaleKupa rezultat = new RezultatPreskokFinaleKupa(this);
                rezultat.Gimnasticar = g;
                rezultatiMap.Add(g, rezultat);
            }

            // TODO4: Razmisli da li treba uvoditi opciju u propozicijama sta da se radi kada u nekom od prethodnih kola
            // deo takmicara ima obe ocene za preskok a deo ima samo jednu (ili se kao i do sada oslanjati na to kako je
            // to specifikovano u propozicijama za 1. i 2. kolo, sto mislim da je bolja varijanta)

            // Ovo takodje obradjuje situaciju kada je u propozicijama za prvo kolo stavljeno
            // da se preskok racuna na osnovu oba preskoka, ali ni za jednog gimnasticara ne
            // postoji ocena za oba preskoka. Ova situacija najverovatnije nastaje kada se u
            // prvom kolu kao prvi preskok unosila konacna ocena za oba preskoka.
            // U tom slucaju, za ocenu prvog kola treba uzeti prvu ocenu.
            this.koristiTotalObeOcenePrvoKolo = rezTak1.Propozicije.Tak1PreskokNaOsnovuObaPreskoka
                && rezTak1.Takmicenje1.PoredakPreskok.postojeObaPreskoka();

            foreach (RezultatPreskok r in rezTak1.Takmicenje1.PoredakPreskok.Rezultati)
            {
                if (rezultatiMap.ContainsKey(r.Gimnasticar))
                {
                    rezultatiMap[r.Gimnasticar].initPrvoKolo(r);
                }
            }

            this.koristiTotalObeOceneDrugoKolo = rezTak2.Propozicije.Tak1PreskokNaOsnovuObaPreskoka
                && rezTak2.Takmicenje1.PoredakPreskok.postojeObaPreskoka();
            foreach (RezultatPreskok r in rezTak2.Takmicenje1.PoredakPreskok.Rezultati)
            {
                if (rezultatiMap.ContainsKey(r.Gimnasticar))
                {
                    rezultatiMap[r.Gimnasticar].initDrugoKolo(r);
                }
            }

            Rezultati.Clear();
            foreach (RezultatPreskokFinaleKupa r in rezultatiMap.Values)
            {
                r.calculateTotal(rezTak.Propozicije.NacinRacunanjaOceneFinaleKupaTak3);
                Rezultati.Add(r);
            }
            rankRezultati(rezTak.Propozicije);
        }

        public virtual void rankRezultati(Propozicije propozicije)
        {
            List<RezultatPreskokFinaleKupa> rezultati = new List<RezultatPreskokFinaleKupa>(Rezultati);

            PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                TypeDescriptor.GetProperties(typeof(RezultatPreskokFinaleKupa))["Total"],
                TypeDescriptor.GetProperties(typeof(RezultatPreskokFinaleKupa))["PrezimeIme"]
            };
            ListSortDirection[] sortDir = new ListSortDirection[] {
                ListSortDirection.Descending,
                ListSortDirection.Ascending
            };
            rezultati.Sort(new SortComparer<RezultatPreskokFinaleKupa>(propDesc, sortDir));

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
            updateKvalStatus(propozicije);
        }

        private void updateKvalStatus(Propozicije propozicije)
        {
            if (!propozicije.odvojenoTak3())
            {
                foreach (RezultatPreskokFinaleKupa r in Rezultati)
                    r.KvalStatus = KvalifikacioniStatus.None;
                return;
            }

            List<RezultatPreskokFinaleKupa> rezultati = new List<RezultatPreskokFinaleKupa>(Rezultati);
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatPreskokFinaleKupa))["RedBroj"];
            rezultati.Sort(new SortComparer<RezultatPreskokFinaleKupa>(propDesc,
                ListSortDirection.Ascending));

            // moram da koristim dve mape zato sto je moguca situacija da klub i 
            // drzava imaju isti id
            IDictionary<int, int> brojTakmicaraKlubMap = new Dictionary<int, int>();
            IDictionary<int, int> brojTakmicaraDrzavaMap = new Dictionary<int, int>();

            int finCount = 0;
            int rezCount = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                RezultatPreskokFinaleKupa rezultat = rezultati[i];
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

        public virtual void calculateTotal(Propozicije propozicije)
        {
            foreach (RezultatPreskokFinaleKupa r in Rezultati)
                r.calculateTotal(propozicije.NacinRacunanjaOceneFinaleKupaTak3);
            rankRezultati(propozicije);
        }

        public virtual void addGimnasticar(GimnasticarUcesnik g, RezultatskoTakmicenje rezTak,
            RezultatskoTakmicenje rezTak1, RezultatskoTakmicenje rezTak2)
        {
            RezultatPreskokFinaleKupa rezultat = new RezultatPreskokFinaleKupa(this);
            rezultat.Gimnasticar = g;

            this.koristiTotalObeOcenePrvoKolo = rezTak1.Propozicije.Tak1PreskokNaOsnovuObaPreskoka
                && rezTak1.Takmicenje1.PoredakPreskok.postojeObaPreskoka();
            foreach (RezultatPreskok r in rezTak1.Takmicenje1.PoredakPreskok.Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                {
                    rezultat.initPrvoKolo(r);
                    break;
                }
            }

            this.koristiTotalObeOceneDrugoKolo = rezTak2.Propozicije.Tak1PreskokNaOsnovuObaPreskoka
                && rezTak2.Takmicenje1.PoredakPreskok.postojeObaPreskoka();
            foreach (RezultatPreskok r in rezTak2.Takmicenje1.PoredakPreskok.Rezultati)
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
            RezultatPreskokFinaleKupa r = getRezultat(g);
            if (r != null)
            {
                Rezultati.Remove(r);
                rankRezultati(rezTak.Propozicije);
            }
        }

        private RezultatPreskokFinaleKupa getRezultat(GimnasticarUcesnik g)
        {
            foreach (RezultatPreskokFinaleKupa r in Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                    return r;
            }
            return null;
        }

        public virtual List<RezultatPreskokFinaleKupa> getRezultati()
        {
            List<RezultatPreskokFinaleKupa> result = new List<RezultatPreskokFinaleKupa>(Rezultati);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatPreskokFinaleKupa))["RedBroj"];
            result.Sort(new SortComparer<RezultatPreskokFinaleKupa>(propDesc, ListSortDirection.Ascending));

            return result;
        }

        public virtual IList<GimnasticarUcesnik> getKvalifikanti()
        {
            IList<GimnasticarUcesnik> result = new List<GimnasticarUcesnik>();
            foreach (RezultatPreskokFinaleKupa r in getRezultati())
            {
                if (r.KvalStatus == KvalifikacioniStatus.Q)
                    result.Add(r.Gimnasticar);
            }
            return result;
        }

        // NOTE: Nisu implementirani Equals i GetHashCode (iako se PoredakPreskokFinaleKupa cuva u setovima) zato sto je
        // podrazumevani Equals dovoljan.

        public virtual void dumpRezultati(StreamWriter streamWriter)
        {
            string header = Sprava.Preskok.ToString().ToUpper() + " - FINALE";
            streamWriter.WriteLine(header);
            foreach (RezultatPreskokFinaleKupa r in getRezultati())
            {
                string line = r.Rank + ". " + r.Gimnasticar.ImeSrednjeImePrezimeDatumRodjenja + "   " + r.Total;
                streamWriter.WriteLine(line);
            }
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());

            strBuilder.AppendLine(Rezultati.Count.ToString());
            foreach (RezultatPreskokFinaleKupa r in Rezultati)
                r.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            int brojRezultata = int.Parse(reader.ReadLine());
            for (int i = 0; i < brojRezultata; ++i)
            {
                reader.ReadLine();  // id
                RezultatPreskokFinaleKupa r = new RezultatPreskokFinaleKupa(this);
                r.loadFromDump(reader, map);
                Rezultati.Add(r);
            }
        }
    }
}
