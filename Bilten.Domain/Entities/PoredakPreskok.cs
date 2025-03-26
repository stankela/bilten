using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Bilten.Util;
using System.IO;

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

        private Sprava _sprava;
        public virtual Sprava Sprava
        {
            get { return _sprava; }
            protected set { _sprava = value; }
        }

        private IList<RezultatPreskok> _rezultati = new List<RezultatPreskok>();
        public virtual IList<RezultatPreskok> Rezultati
        {
            get { return _rezultati; }
            protected set { _rezultati = value; }
        }

        public PoredakPreskok()
        { 
        
        }

        public PoredakPreskok(DeoTakmicenjaKod deoTakKod)
        {
            this.deoTakKod = deoTakKod;
            this._sprava = Sprava.Preskok;
        }

        public virtual void create(RezultatskoTakmicenje rezTak, IList<Ocena> ocene)
        {
            IList<GimnasticarUcesnik> gimnasticari;
            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                gimnasticari = getGimnasticari(ocene, rezTak);
            else
                gimnasticari = rezTak.Takmicenje3.getGimnasticariKvalifikanti(Sprava.Preskok);

            IDictionary<int, RezultatPreskok> rezultatiMap = new Dictionary<int, RezultatPreskok>();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                RezultatPreskok r = new RezultatPreskok();
                r.Gimnasticar = g;
                rezultatiMap.Add(g.Id, r);
            }

            foreach (Ocena o in ocene)
            {
                if (o.Sprava == Sprava.Preskok && rezultatiMap.ContainsKey(o.Gimnasticar.Id))
                    rezultatiMap[o.Gimnasticar.Id].setOcena(o);
            }

            Rezultati.Clear();
            foreach (RezultatPreskok r in rezultatiMap.Values)
                Rezultati.Add(r);
            rankRezultati(rezTak.Propozicije);
        }

        public virtual void rankRezultati(Propozicije propozicije)
        {
            if (!propozicije.racunajObaPreskoka(DeoTakmicenjaKod))
            {
                List<RezultatPreskok> rezultati = new List<RezultatPreskok>(Rezultati);
                rankByPrviPreskok(rezultati, propozicije.VecaEOcenaImaPrednost, 0);
            }
            else
            {
                List<RezultatPreskok> rezultatiObaPreskoka = new List<RezultatPreskok>();
                List<RezultatPreskok> rezultatiPrviPreskok = new List<RezultatPreskok>();
                foreach (RezultatPreskok r in Rezultati)
                {
                    if (r.TotalObeOcene != null)
                        rezultatiObaPreskoka.Add(r);
                    else
                        rezultatiPrviPreskok.Add(r);
                }

                // Ovakav nacin rangiranja rezultata (da se prvo rangiraju takmicari sa dva preskoka pa zatim sa jednim)
                // obezbedjuje da i u situaciji kada je u propozicijama navedeno da se preskok racuna na osnovu obe ocene,
                // a zatim se kod unosenja ocena unese samo prva ocena za sve takmicare, da ce i tada takmicari biti
                // ispravno rangirani (rezultatiObaPreskoka ce tada biti prazni)
                rankByObaPreskoka(rezultatiObaPreskoka, propozicije.VecaEOcenaImaPrednost);
                rankByPrviPreskok(rezultatiPrviPreskok, propozicije.VecaEOcenaImaPrednost, rezultatiObaPreskoka.Count);
            }

            // NOTE: Deo "!obaPreskoka && x.Total == null" obezbedjuje da se kvalifikanti racunaju na osnovu prvog skoka
            // u situacijama kada je u propozicijama stavljeno da se kvalifikanti racunaju na osnovu oba skoka, a u
            // takmicenju 1 je unesen samo prvi skok. (Inace nijedan gimnasticar ne bi bio oznacen sa Q).

            bool obaPreskoka = postojeObaPreskoka();
            PoredakSprava.updateKvalStatus<RezultatPreskok>(propozicije, Rezultati, deoTakKod,
                x => !propozicije.Tak1PreskokNaOsnovuObaPreskoka && x.Total == null
                     || propozicije.Tak1PreskokNaOsnovuObaPreskoka
                        && (obaPreskoka && x.TotalObeOcene == null || !obaPreskoka && x.Total == null));
        }

        private void rankByObaPreskoka(List<RezultatPreskok> rezultati, bool vecaEOcenaImaPrednost)
        {
            PropertyDescriptor[] propDesc;
            ListSortDirection[] sortDir;
            if (vecaEOcenaImaPrednost)
            {
                propDesc = new PropertyDescriptor[] {
                    TypeDescriptor.GetProperties(typeof(RezultatPreskok))["TotalObeOcene"],
                    TypeDescriptor.GetProperties(typeof(RezultatPreskok))["EObeOcene"],
                    TypeDescriptor.GetProperties(typeof(RezultatPreskok))["PrezimeIme"]
                };
                sortDir = new ListSortDirection[] {
                    ListSortDirection.Descending,
                    ListSortDirection.Descending,
                    ListSortDirection.Ascending
                };
            }
            else
            {
                propDesc = new PropertyDescriptor[] {
                    TypeDescriptor.GetProperties(typeof(RezultatPreskok))["TotalObeOcene"],
                    TypeDescriptor.GetProperties(typeof(RezultatPreskok))["PrezimeIme"]
                };
                sortDir = new ListSortDirection[] {
                    ListSortDirection.Descending,
                    ListSortDirection.Ascending
                };
            }
            rezultati.Sort(new SortComparer<RezultatPreskok>(propDesc, sortDir));

            RezultatPreskok prevRezultat = null;
            short prevRank = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                rezultati[i].RedBroj = (short)(i + 1);

                // TotalObeOcene != null za sve rezultate
                if (!ranksAreEqual(rezultati[i], prevRezultat, true, vecaEOcenaImaPrednost))
                    rezultati[i].Rank = rezultati[i].RedBroj;
                else
                    rezultati[i].Rank = prevRank;

                prevRezultat = rezultati[i];
                prevRank = rezultati[i].Rank.Value;
            }
        }

        private void rankByPrviPreskok(List<RezultatPreskok> rezultati, bool vecaEOcenaImaPrednost, int redBrojOffset)
        {
            PropertyDescriptor[] propDesc;
            ListSortDirection[] sortDir;
            if (vecaEOcenaImaPrednost)
            {
                propDesc = new PropertyDescriptor[] {
                    TypeDescriptor.GetProperties(typeof(RezultatPreskok))["Total"],
                    TypeDescriptor.GetProperties(typeof(RezultatPreskok))["E"],
                    TypeDescriptor.GetProperties(typeof(RezultatPreskok))["PrezimeIme"]
                };
                sortDir = new ListSortDirection[] {
                    ListSortDirection.Descending,
                    ListSortDirection.Descending,
                    ListSortDirection.Ascending
                };
            }
            else
            {
                propDesc = new PropertyDescriptor[] {
                    TypeDescriptor.GetProperties(typeof(RezultatPreskok))["Total"],
                    TypeDescriptor.GetProperties(typeof(RezultatPreskok))["PrezimeIme"]
                };
                sortDir = new ListSortDirection[] {
                    ListSortDirection.Descending,
                    ListSortDirection.Ascending
                };
            }
            rezultati.Sort(new SortComparer<RezultatPreskok>(propDesc, sortDir));

            RezultatPreskok prevRezultat = null;
            short prevRank = 0;
            for (int i = 0; i < rezultati.Count; i++)
            {
                rezultati[i].RedBroj = (short)(i + 1 + redBrojOffset);

                if (rezultati[i].Total == null)
                    rezultati[i].Rank = null;
                else
                {
                    if (!ranksAreEqual(rezultati[i], prevRezultat, false, vecaEOcenaImaPrednost))
                        rezultati[i].Rank = rezultati[i].RedBroj;
                    else
                        rezultati[i].Rank = prevRank;

                    prevRezultat = rezultati[i];
                    prevRank = rezultati[i].Rank.Value;
                }
            }
        }

        private IList<GimnasticarUcesnik> getGimnasticari(IList<Ocena> ocene,
            RezultatskoTakmicenje rezTak)
        {
            ISet<int> idSet = new HashSet<int>();
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

        private bool ranksAreEqual(RezultatPreskok r1, RezultatPreskok r2, bool obaPreskoka, bool vecaEOcenaImaPrednost)
        {
            if (r1 == null || r2 == null)
                return false;
            if (!obaPreskoka)
                return r1.Total == r2.Total && (!vecaEOcenaImaPrednost || r1.E == r2.E);
            else
                return r1.TotalObeOcene == r2.TotalObeOcene && (!vecaEOcenaImaPrednost || r1.EObeOcene == r2.EObeOcene);
        }

        // TODO: Ima dosta ponavljanja istog koda u klasama PoredakSprava i PoredakPreskok. Probaj da generalizujes.

        public virtual List<RezultatPreskok> getRezultati()
        {
            List<RezultatPreskok> result = new List<RezultatPreskok>(Rezultati);

            PropertyDescriptor propDesc =
                TypeDescriptor.GetProperties(typeof(RezultatPreskok))["RedBroj"];
            result.Sort(new SortComparer<RezultatPreskok>(propDesc,
                ListSortDirection.Ascending));

            return result;
        }

        public virtual bool postojeObaPreskoka()
        {
            foreach (RezultatPreskok r in Rezultati)
            {
                if (r.TotalObeOcene != null)
                    return true;
            }
            return false;
        }

        public virtual List<RezultatPreskok> getKvalifikantiIRezerve(bool obaPreskoka)
        {
            List<RezultatPreskok> result = new List<RezultatPreskok>();
            foreach (RezultatPreskok r in getRezultati())
            {
                if (r.KvalStatus == KvalifikacioniStatus.Q)
                    result.Add(r);
            }
            foreach (RezultatPreskok r in getRezultati())
            {
                if (r.KvalStatus == KvalifikacioniStatus.R)
                    result.Add(r);
            }

            return result;
        }

        public virtual int getBrojKvalifikanata()
        {
            int result = 0;
            foreach (RezultatPreskok r in Rezultati)
            {
                if (r.KvalStatus == KvalifikacioniStatus.Q)
                    ++result;
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
                rankRezultati(rezTak.Propozicije);
            }
        }

        public virtual void deleteOcena(Ocena o, RezultatskoTakmicenje rezTak, bool removeRezultat)
        {
            RezultatPreskok r = getRezultat(o.Gimnasticar);
            if (r != null)
            {
                if (removeRezultat)
                    Rezultati.Remove(r);
                else
                    r.clearOcena();
                rankRezultati(rezTak.Propozicije);
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
                rankRezultati(rezTak.Propozicije);
            }
        }

        public virtual void addGimnasticar(GimnasticarUcesnik g, Ocena o, 
            RezultatskoTakmicenje rezTak)
        {
            RezultatPreskok r = new RezultatPreskok();
            r.Gimnasticar = g;
            r.setOcena(o);
            Rezultati.Add(r);
            rankRezultati(rezTak.Propozicije);
        }

        public virtual void deleteGimnasticar(GimnasticarUcesnik g, 
            RezultatskoTakmicenje rezTak)
        {
            RezultatPreskok r = getRezultat(g);
            if (r != null)
            {
                Rezultati.Remove(r);
                rankRezultati(rezTak.Propozicije);
            }
        }

        // Ovo sluzi da apdejtujemo ocene E1, E2, ...
        public virtual void updateOcene(IList<Ocena> ocene)
        {
            IDictionary<int, RezultatPreskok> rezultatiMap = new Dictionary<int, RezultatPreskok>();
            foreach (RezultatPreskok r in Rezultati)
            {
                rezultatiMap.Add(r.Gimnasticar.Id, r);
            }
            foreach (Ocena o in ocene)
            {
                if (o.Sprava == Sprava.Preskok && rezultatiMap.ContainsKey(o.Gimnasticar.Id))
                    rezultatiMap[o.Gimnasticar.Id].setOcena(o);
            }
        }

        public virtual void dumpRezultati(StreamWriter streamWriter, Propozicije propozicije)
        {
            string header = DeoTakmicenjaKod == DeoTakmicenjaKod.Takmicenje1 ? "PRESKOK" : "PRESKOK - FINALE";
            streamWriter.WriteLine(header);
            foreach (RezultatPreskok r in getRezultati())
            {
                float? total;
                if (!propozicije.racunajObaPreskoka(DeoTakmicenjaKod) || r.TotalObeOcene == null)
                    total = r.Total;
                else
                    total = r.TotalObeOcene;
                string line = r.Rank + ". " + r.Gimnasticar.ImeSrednjeImePrezimeDatumRodjenja + "   " + total;
                streamWriter.WriteLine(line);
            }
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            strBuilder.AppendLine(Id.ToString());
            strBuilder.AppendLine(DeoTakmicenjaKod.ToString());
            strBuilder.AppendLine(Sprava.ToString());

            strBuilder.AppendLine(Rezultati.Count.ToString());
            foreach (RezultatPreskok r in Rezultati)
                r.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            DeoTakmicenjaKod = (DeoTakmicenjaKod)Enum.Parse(typeof(DeoTakmicenjaKod), reader.ReadLine());
            Sprava = (Sprava)Enum.Parse(typeof(Sprava), reader.ReadLine());

            int brojRezultata = int.Parse(reader.ReadLine());
            for (int i = 0; i < brojRezultata; ++i)
            {
                reader.ReadLine();  // id
                RezultatPreskok r = new RezultatPreskok();
                r.loadFromDump(reader, map);
                Rezultati.Add(r);
            }
        }
    }
}
