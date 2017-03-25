using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Bilten.Util;

namespace Bilten.Domain
{
    public class PoredakEkipno : DomainObject
    {
        private DeoTakmicenjaKod deoTakKod;
        public virtual DeoTakmicenjaKod DeoTakmicenjaKod
        {
            get { return deoTakKod; }
            set { deoTakKod = value; }
        }
        
        private IList<RezultatEkipno> _rezultati = new List<RezultatEkipno>();
        public virtual IList<RezultatEkipno> Rezultati
        {
            get { return _rezultati; }
            protected set { _rezultati = value; }
        }

        protected PoredakEkipno()
        {

        }

        public PoredakEkipno(DeoTakmicenjaKod deoTakKod)
        {
            this.deoTakKod = deoTakKod;
        }

        public virtual void initRezultati(IList<Ekipa> ekipe)
        {
            Rezultati.Clear();
            foreach (Ekipa e in ekipe)
            {
                RezultatEkipno r = new RezultatEkipno();
                r.Ekipa = e;
                Rezultati.Add(r);
            }

            // posto nepostoje ocene, sledeci poziv samo sortira po prezimenu i na
            // osnovu toga dodeljuje RedBroj
            rankRezultati();
        }

        public virtual void create(RezultatskoTakmicenje rezTak, IList<Ocena> ocene)
        {
            IList<Ekipa> ekipe;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                ekipe = new List<Ekipa>(rezTak.Takmicenje1.Ekipe);
            else
                ekipe = new List<Ekipa>(rezTak.Takmicenje4.getUcesnici());

            IDictionary<int, List<RezultatUkupno>> ekipaRezUkupnoMap = 
                createEkipaRezultatiUkupnoMap(ekipe, ocene);

            Rezultati.Clear();
            foreach (Ekipa e in ekipe)
            {
                RezultatEkipno rezultatEkipno = new RezultatEkipno();
                rezultatEkipno.Ekipa = e;

                List<RezultatUkupno> rezultatiUkupno = ekipaRezUkupnoMap[e.Id];
                Sprava[] sprave = Sprave.getSprave(rezTak.Gimnastika);
                foreach (Sprava s in sprave)
                {
                    PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                        TypeDescriptor.GetProperties(typeof(RezultatEkipno))[s.ToString()]
                    };
                    ListSortDirection[] sortDir = new ListSortDirection[] {
                        ListSortDirection.Descending
                    };
                    rezultatiUkupno.Sort(new SortComparer<RezultatUkupno>(propDesc, sortDir));

                    for (int i = 0; i < rezultatiUkupno.Count; i++)
                    {
                        if (i < rezTak.Propozicije.BrojRezultataKojiSeBodujuZaEkipu)
                        {
                            Nullable<float> ocena = rezultatiUkupno[i].getOcena(s);
                            if (ocena != null)
                                rezultatEkipno.addOcena(s, ocena);
                        }
                    }
                }

                if (e.Penalty != null)
                    rezultatEkipno.addPenalty(e.Penalty);
                Rezultati.Add(rezultatEkipno);
            }

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
        }

        private IDictionary<int, List<RezultatUkupno>> createEkipaRezultatiUkupnoMap(
            IList<Ekipa> ekipe, IList<Ocena> ocene)
        {
            IDictionary<int, RezultatUkupno> gimRezUkupnoMap = new Dictionary<int, RezultatUkupno>();
            foreach (Ekipa e in ekipe)
            {
                foreach (GimnasticarUcesnik g in e.Gimnasticari)
                {
                    if (!gimRezUkupnoMap.ContainsKey(g.Id))
                    {
                        RezultatUkupno rez = new RezultatUkupno();
                        rez.Gimnasticar = g;
                        gimRezUkupnoMap.Add(g.Id, rez);
                    }
                }
            }
            foreach (Ocena o in ocene)
            {
                if (gimRezUkupnoMap.ContainsKey(o.Gimnasticar.Id))
                    gimRezUkupnoMap[o.Gimnasticar.Id].addOcena(o);
            }

            IDictionary<int, List<RezultatUkupno>> ekipaRezultatiMap = new Dictionary<int, List<RezultatUkupno>>();
            foreach (Ekipa e in ekipe)
            {
                List<RezultatUkupno> rezultati = new List<RezultatUkupno>();
                ekipaRezultatiMap.Add(e.Id, rezultati);
                foreach (GimnasticarUcesnik g in e.Gimnasticari)
                {
                    rezultati.Add(gimRezUkupnoMap[g.Id]);
                }
            }
            return ekipaRezultatiMap;
        }

        private void rankRezultati()
        {
            List<RezultatEkipno> rezultati = new List<RezultatEkipno>(Rezultati);

            PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                TypeDescriptor.GetProperties(typeof(RezultatEkipno))["Total"],
                TypeDescriptor.GetProperties(typeof(RezultatEkipno))["NazivEkipe"]
            };
            ListSortDirection[] sortDir = new ListSortDirection[] {
                ListSortDirection.Descending,
                ListSortDirection.Ascending
            };
            rezultati.Sort(new SortComparer<RezultatEkipno>(propDesc, sortDir));

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
            if (deoTakKod != DeoTakmicenjaKod.Takmicenje1)
                return;
            if (!propozicije.PostojiTak4)
                return;

            doUpdateKvalStatus(propozicije.OdvojenoTak4, propozicije.BrojEkipaUFinalu, 0);
        }

        // TODO3: izbaci parametar odvojenoFinale i podesi da se ovaj metod poziva samo kada je odvojenoFinale == true.
        // Kada je odvojenoFinale == false, kvalStatus nema smisla. Takodje i u Propozicijama promeni da kada ne postoji
        // odvojeno finale da se ne zadaje . Proveri sve ovo detaljnije. Isto uradi i u klasama
        // PoredakUkupno, PoredakSprava i PoredakPreskok.
        private void doUpdateKvalStatus(bool odvojenoFinale, int brojFinalista,
            int brojRezervi)
        {
            List<RezultatEkipno> rezultati = new List<RezultatEkipno>(Rezultati);
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatEkipno))["RedBroj"];
            rezultati.Sort(new SortComparer<RezultatEkipno>(propDesc,
                ListSortDirection.Ascending));

            int finCount = 0;
            int rezCount = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                Rezultat rezulat = rezultati[i];
                if (rezulat.Total == null)
                {
                    rezulat.KvalStatus = KvalifikacioniStatus.None;
                    continue;
                }

                if (odvojenoFinale)
                {
                    if (finCount < brojFinalista)
                    {
                        finCount++;
                        rezulat.KvalStatus = KvalifikacioniStatus.Q;
                    }
                    else if (rezCount < brojRezervi)
                    {
                        rezCount++;
                        rezulat.KvalStatus = KvalifikacioniStatus.R;
                    }
                    else
                    {
                        rezulat.KvalStatus = KvalifikacioniStatus.None;
                    }
                }
                else
                {
                    // u ovom slucaju moze da se stavi i None, tj. svejedno je
                    // sta se stavlja posto ce svi takmicari imati istu oznaku
                    rezulat.KvalStatus = KvalifikacioniStatus.Q;
                }
            }
        }

        public virtual List<RezultatEkipno> getRezultati()
        {
            List<RezultatEkipno> result = new List<RezultatEkipno>(Rezultati);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatEkipno))["RedBroj"];
            result.Sort(new SortComparer<RezultatEkipno>(propDesc,
                ListSortDirection.Ascending));

            return result;
        }

        public virtual void calculatePoredak(RezultatskoTakmicenje rezTak)
        {
            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
        }

        // TODO3: Proveri zasto sam zakomentarisao ovaj i sledece metode.
        public virtual void addOcena(Ocena o, RezultatskoTakmicenje rezTak)
        {
            /*
            IList<RezultatEkipno> rezultati = getRezultati(o.Gimnasticar);
            if (rezultati.Count == 0)
                return;

            foreach (RezultatEkipno r in rezultati)
                r.addOcena(o);
            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
             */
        }

        private IList<RezultatEkipno> getRezultati(GimnasticarUcesnik g)
        {
            IList<RezultatEkipno> result = new List<RezultatEkipno>();
            foreach (RezultatEkipno r in Rezultati)
            {
                if (r.Ekipa.Gimnasticari.Contains(g))
                    result.Add(r);
            }
            return result;
        }

        public virtual void deleteOcena(Ocena o, RezultatskoTakmicenje rezTak)
        {
            /*
            IList<RezultatEkipno> rezultati = getRezultati(o.Gimnasticar);
            if (rezultati.Count == 0)
                return;

            foreach (RezultatEkipno r in rezultati)
                r.removeOcena(o);
            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
             */
        }

        public virtual void editOcena(Ocena o, Ocena old, RezultatskoTakmicenje rezTak)
        {
            /*
            IList<RezultatEkipno> rezultati = getRezultati(o.Gimnasticar);
            if (rezultati.Count == 0)
                return;

            foreach (RezultatEkipno r in rezultati)
            {
                r.removeOcena(old);
                r.addOcena(o);
            }
            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
             */
        }

        public virtual void addEkipa(Ekipa e, IList<Ocena> ocene,
            RezultatskoTakmicenje rezTak)
        {
            /*
            RezultatEkipno r = new RezultatEkipno();
            r.Ekipa = e;
            Rezultati.Add(r);
       
            foreach (Ocena o in ocene)
            {
                if (e.Gimnasticari.Contains(o.Gimnasticar))
                    r.addOcena(o);
            }

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
             */
        }

        public virtual void deleteEkipa(Ekipa e, RezultatskoTakmicenje rezTak)
        {
            // Ovde je deo ostao otkometarisan zato sto mi je davao gresku kod brisanja ekipa.
            RezultatEkipno r = getRezultat(e);
            if (r != null)
            {
                Rezultati.Remove(r);
                //rankRezultati();
                //updateKvalStatus(rezTak.Propozicije);
            }
             
        }

        private RezultatEkipno getRezultat(Ekipa e)
        {
            foreach (RezultatEkipno r in Rezultati)
            {
                if (r.Ekipa.Equals(e))
                    return r;
            }
            return null;
        }

        public virtual void gimnasticarAddedToEkipa(GimnasticarUcesnik g, Ekipa e, 
            IList<Ocena> ocene, RezultatskoTakmicenje rezTak)
        {
            /*
            if (ocene.Count == 0)
                return;
            RezultatEkipno r = getRezultat(e);
            if (r != null)
            {
                foreach (Ocena o in ocene)
                    r.addOcena(o);

                rankRezultati();
                updateKvalStatus(rezTak.Propozicije);
            }
             */
        }

        public virtual void gimnasticarDeletedFromEkipa(GimnasticarUcesnik g, Ekipa e,
            IList<Ocena> ocene, RezultatskoTakmicenje rezTak)
        {
            /*
            if (ocene.Count == 0)
                return;
            RezultatEkipno r = getRezultat(e);
            if (r != null)
            {
                foreach (Ocena o in ocene)
                    r.removeOcena(o);

                rankRezultati();
                updateKvalStatus(rezTak.Propozicije);
            }
             */
        }
    }
}
