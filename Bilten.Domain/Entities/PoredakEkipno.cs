using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Bilten.Util;
using System.IO;

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

        public PoredakEkipno()
        {

        }

        public PoredakEkipno(DeoTakmicenjaKod deoTakKod)
        {
            this.deoTakKod = deoTakKod;
        }

        public virtual void create(RezultatskoTakmicenje rezTak, IList<RezultatskoTakmicenje> svaRezTakmicenja)
        {
            IList<Ekipa> ekipe;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                ekipe = new List<Ekipa>(rezTak.Takmicenje1.Ekipe);
            else
                ekipe = new List<Ekipa>(rezTak.Takmicenje4.getUcesnici());

            IDictionary<int, List<RezultatUkupno>> ekipaRezUkupnoMap = createEkipaRezultatiUkupnoMap(ekipe, svaRezTakmicenja);

            Rezultati.Clear();
            foreach (Ekipa e in ekipe)
            {
                Rezultati.Add(createRezultatEkipno(e, ekipaRezUkupnoMap[e.Id],
                    rezTak.Propozicije.BrojRezultataKojiSeBodujuZaEkipu, rezTak.Gimnastika));
            }

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
        }

        private RezultatEkipno createRezultatEkipno(Ekipa e, List<RezultatUkupno> rezultatiUkupno, int brojRezultata,
            Gimnastika gimnastika)
        {
            RezultatEkipno result = new RezultatEkipno();
            result.Ekipa = e;

            foreach (Sprava s in Sprave.getSprave(gimnastika))
            {
                if (!e.getSpravaSeBoduje(s))
                  continue;

                PropertyDescriptor[] propDesc = new PropertyDescriptor[] {
                        TypeDescriptor.GetProperties(typeof(RezultatEkipno))[s.ToString()]
                };
                ListSortDirection[] sortDir = new ListSortDirection[] {
                        ListSortDirection.Descending
                };
                rezultatiUkupno.Sort(new SortComparer<RezultatUkupno>(propDesc, sortDir));

                for (int i = 0; i < rezultatiUkupno.Count; i++)
                {
                    if (i < brojRezultata)
                    {
                        Nullable<float> ocena = rezultatiUkupno[i].getOcena(s);
                        if (ocena != null)
                            result.addOcena(s, ocena);
                    }
                }
            }

            if (e.Penalty != null)
                result.addPenalty(e.Penalty.Value);
            return result;
        }

        private IDictionary<int, List<RezultatUkupno>> createEkipaRezultatiUkupnoMap(IList<Ekipa> ekipe,
            IList<RezultatskoTakmicenje> svaRezTakmicenja)
        {
            // Najpre napravi mapu svih rezultata ukupno
            IDictionary<int, IList<Pair<RezultatskoTakmicenje, RezultatUkupno>>> sviRezultatiMap
                = Takmicenje.getRezultatiUkupnoMap(svaRezTakmicenja, deoTakKod);
            
            IDictionary<int, List<RezultatUkupno>> ekipaRezultatiMap = new Dictionary<int, List<RezultatUkupno>>();
            foreach (Ekipa e in ekipe)
                ekipaRezultatiMap.Add(e.Id, Takmicenje.getRezultatiUkupnoZaClanoveEkipe(e, sviRezultatiMap));
            return ekipaRezultatiMap;
        }

        private IDictionary<int, List<RezultatUkupno>> createEkipaRezultatiUkupnoMap(IList<Ekipa> ekipe,
            IList<Ocena> ocene)
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
                    gimRezUkupnoMap[o.Gimnasticar.Id].addOcena(o, /*TODO4*/false);
            }

            IDictionary<int, List<RezultatUkupno>> ekipaRezultatiMap = new Dictionary<int, List<RezultatUkupno>>();
            foreach (Ekipa e in ekipe)
            {
                List<RezultatUkupno> rezultati = new List<RezultatUkupno>();
                ekipaRezultatiMap.Add(e.Id, rezultati);
                foreach (GimnasticarUcesnik g in e.Gimnasticari)
                    rezultati.Add(gimRezUkupnoMap[g.Id]);
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
                    rezultati[i].Rank = null;
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
            if (deoTakKod != DeoTakmicenjaKod.Takmicenje1 || !propozicije.odvojenoTak4())
            {
                foreach (RezultatEkipno r in Rezultati)
                    r.KvalStatus = KvalifikacioniStatus.None;
                return;
            }

            List<RezultatEkipno> rezultati = new List<RezultatEkipno>(Rezultati);
            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatEkipno))["RedBroj"];
            rezultati.Sort(new SortComparer<RezultatEkipno>(propDesc,
                ListSortDirection.Ascending));

            int finCount = 0;
            int rezCount = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                Rezultat rezultat = rezultati[i];
                if (rezultat.Total == null)
                {
                    rezultat.KvalStatus = KvalifikacioniStatus.None;
                    continue;
                }

                if (finCount < propozicije.BrojEkipaUFinalu)
                {
                    finCount++;
                    rezultat.KvalStatus = KvalifikacioniStatus.Q;
                }
                else if (rezCount < 0 /* brojRezervi */)
                {
                    rezCount++;
                    rezultat.KvalStatus = KvalifikacioniStatus.R;
                }
                else
                    rezultat.KvalStatus = KvalifikacioniStatus.None;
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

        public virtual void addOcena(Ocena o, RezultatskoTakmicenje rezTak, List<Ocena> ocene)
        {
            ocenaChanged(o, rezTak, ocene);
        }

        public virtual void deleteOcena(Ocena o, RezultatskoTakmicenje rezTak, List<Ocena> ocene)
        {
            ocenaChanged(o, rezTak, ocene);
        }

        public virtual void editOcena(Ocena o, Ocena old, RezultatskoTakmicenje rezTak, List<Ocena> ocene)
        {
            ocenaChanged(o, rezTak, ocene);
        }

        private void ocenaChanged(Ocena o, RezultatskoTakmicenje rezTak, List<Ocena> ocene)
        {
            if (o.DeoTakmicenjaKod != this.DeoTakmicenjaKod)
                return;
            IList<RezultatEkipno> rezultati = getRezultati(o.Gimnasticar);
            if (rezultati.Count == 0)
                return;

            // pretpostavio sam da gimnasticar moze da bude clan vise ekipa.
            foreach (RezultatEkipno r in rezultati)
            {
                // NOTE: Mora ponovo da se izracuna rezultat zato sto npr. ako ekipa ima 4 gimnasticara a racunaju se 3
                // ocene, i trenutno su unesene 3 ocene, i ocena koja se dodaje nije najlosija, mora jedna ocena da se
                // izbaci, a posto ne vodim racuna o tome koja je najlosija ocena moram ponovo da racunam rezultat.
                Rezultati.Remove(r);
                Rezultati.Add(createRezultatEkipno(r.Ekipa, ocene, rezTak));
            }

            // izracunaj rank
            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
        }

        private RezultatEkipno createRezultatEkipno(Ekipa e, IList<Ocena> ocene, RezultatskoTakmicenje rezTak)
        {
            // kreiraj mapu ekipa -> rezultati_ukupno, sa samo jednom ekipom
            IList<Ekipa> ekipe = new List<Ekipa>();
            ekipe.Add(e);
            IDictionary<int, List<RezultatUkupno>> ekipaRezUkupnoMap = createEkipaRezultatiUkupnoMap(ekipe, ocene);

            return createRezultatEkipno(e, ekipaRezUkupnoMap[e.Id],
                rezTak.Propozicije.BrojRezultataKojiSeBodujuZaEkipu, rezTak.Gimnastika);
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

        public virtual void addEkipa(Ekipa e, IList<Ocena> ocene, RezultatskoTakmicenje rezTak)
        {
            Rezultati.Add(createRezultatEkipno(e, ocene, rezTak));
            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
        }

        public virtual void deleteEkipa(Ekipa e, RezultatskoTakmicenje rezTak)
        {
            RezultatEkipno r = getRezultat(e);
            if (r != null)
            {
                Rezultati.Remove(r);
                rankRezultati();
                updateKvalStatus(rezTak.Propozicije);
            }             
        }

        public virtual RezultatEkipno getRezultat(Ekipa e)
        {
            foreach (RezultatEkipno r in Rezultati)
            {
                if (r.Ekipa.Equals(e))
                    return r;
            }
            return null;
        }

        public virtual void recreateRezultForEkipa(Ekipa e, IList<Ocena> ocene, RezultatskoTakmicenje rezTak)
        {
            RezultatEkipno r = getRezultat(e);
            if (r != null)
            {
                // ponovo kreiraj rezultat za ekipu
                Rezultati.Remove(r);
                Rezultati.Add(createRezultatEkipno(e, ocene, rezTak));

                // izracunaj rank
                rankRezultati();
                updateKvalStatus(rezTak.Propozicije);
            }
        }

        public virtual void promeniEkipnuPenalizaciju(RezultatEkipno r, float? penalty, RezultatskoTakmicenje rezTak)
        {
            r.changePenalty(penalty);

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(DeoTakmicenjaKod.ToString());

            strBuilder.AppendLine(Rezultati.Count.ToString());
            foreach (RezultatEkipno r in Rezultati)
                r.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            DeoTakmicenjaKod = (DeoTakmicenjaKod)Enum.Parse(typeof(DeoTakmicenjaKod), reader.ReadLine());

            int brojRezultata = int.Parse(reader.ReadLine());
            for (int i = 0; i < brojRezultata; ++i)
            {
                reader.ReadLine();  // id
                RezultatEkipno r = new RezultatEkipno();
                r.loadFromDump(reader, map);
                Rezultati.Add(r);
            }
        }
    }
}
