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

        public virtual void create(RezultatskoTakmicenje rezTak, IDictionary<int, List<RezultatUkupno>> ekipaRezUkupnoMap)
        {
            IList<Ekipa> ekipe;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                ekipe = new List<Ekipa>(rezTak.Takmicenje1.Ekipe);
            else
                ekipe = new List<Ekipa>(rezTak.Takmicenje4.getUcesnici());

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
                        Nullable<float> ocena = rezultatiUkupno[i].getSprava(s);
                        if (ocena != null)
                            result.addOcena(s, ocena.Value);
                    }
                }
            }

            if (e.Penalty != null)
                result.addPenalty(e.Penalty.Value);
            return result;
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

        public virtual void recreateRezultat(Ekipa e, RezultatskoTakmicenje rezTak, List<RezultatUkupno> rezultati)
        {
            RezultatEkipno r = getRezultat(e);
            if (r == null)
                return;

            Rezultati.Remove(r);
            Rezultati.Add(createRezultatEkipno(e, rezultati, rezTak.Propozicije.BrojRezultataKojiSeBodujuZaEkipu,
                rezTak.Gimnastika));

            rankRezultati();
            updateKvalStatus(rezTak.Propozicije);
        }

        public virtual void addEkipa(Ekipa e, RezultatskoTakmicenje rezTak, List<RezultatUkupno> rezultati)
        {
            Rezultati.Add(createRezultatEkipno(e, rezultati, rezTak.Propozicije.BrojRezultataKojiSeBodujuZaEkipu,
                    rezTak.Gimnastika));
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

        public virtual void promeniPenalizaciju(RezultatEkipno r, float? penalty, RezultatskoTakmicenje rezTak)
        {
            r.promeniPenalizacijuZaEkipu(penalty);

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
