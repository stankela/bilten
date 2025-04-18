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
        private bool postojeObaPreskokaPrvoKolo = false;
        private bool propozicijePrvoKoloTak1PreskokNaOsnovuObaPreskoka;

        public virtual bool KoristiTotalObeOcenePrvoKolo
        {
            // TODO4: Razmisli da li treba uvoditi opciju u propozicijama sta da se radi kada u nekom od prethodnih kola
            // deo takmicara ima obe ocene za preskok a deo ima samo jednu (ili se kao i do sada oslanjati na to kako je
            // to specifikovano u propozicijama za 1. i 2. kolo, sto mislim da je bolja varijanta)

            // Ovo takodje obradjuje situaciju kada je u propozicijama za prvo kolo stavljeno
            // da se preskok racuna na osnovu oba preskoka, ali ni za jednog gimnasticara ne
            // postoji ocena za oba preskoka. Ova situacija najverovatnije nastaje kada se u
            // prvom kolu kao prvi preskok unosila konacna ocena za oba preskoka.
            // U tom slucaju, za ocenu prvog kola treba uzeti prvu ocenu.
            get { return propozicijePrvoKoloTak1PreskokNaOsnovuObaPreskoka && postojeObaPreskokaPrvoKolo; }
        }

        private bool postojeObaPreskokaDrugoKolo = false;
        private bool propozicijeDrugoKoloTak1PreskokNaOsnovuObaPreskoka;

        public virtual bool KoristiTotalObeOceneDrugoKolo
        {
            get { return propozicijeDrugoKoloTak1PreskokNaOsnovuObaPreskoka && postojeObaPreskokaDrugoKolo; }
        }

        // Ovo svojstvo se ne koristi nigde, ali morao sam da ga ostavim jer inace NHibernate generise prazan insert
        // statement "INSERT INTO poredak_sprava_finale_kupa VALUES ()", a SQL Server to prijavljuje kao gresku
        private Sprava _sprava = Sprava.Preskok;
        public virtual Sprava Sprava
        {
            get { return _sprava; }
            protected set { _sprava = value; }
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

            this.propozicijePrvoKoloTak1PreskokNaOsnovuObaPreskoka = rezTak1.Propozicije.Tak1PreskokNaOsnovuObaPreskoka;
            foreach (RezultatPreskok r in rezTak1.Takmicenje1.PoredakPreskok.Rezultati)
            {
                if (rezultatiMap.ContainsKey(r.Gimnasticar))
                {
                    rezultatiMap[r.Gimnasticar].initPrvoKolo(r);
                }
            }

            this.propozicijeDrugoKoloTak1PreskokNaOsnovuObaPreskoka = rezTak2.Propozicije.Tak1PreskokNaOsnovuObaPreskoka;
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
                Rezultati.Add(r);
            }
            calculateTotalAndRankRezultati(rezTak.Propozicije);
        }

        public virtual void rankRezultati(Propozicije propozicije)
        {
            PoredakSpravaFinaleKupa.rankRezultati(propozicije, new List<RezultatSpravaFinaleKupa>(Rezultati));
        }

        private void updateKvalStatus(Propozicije propozicije)
        {
            PoredakSpravaFinaleKupa.updateKvalStatus(propozicije, new List<RezultatSpravaFinaleKupa>(Rezultati));
        }

        public virtual void calculateTotalAndRankRezultati(Propozicije propozicije)
        {
            foreach (RezultatPreskokFinaleKupa r in Rezultati)
                r.calculateTotal(propozicije.NacinRacunanjaOceneFinaleKupaTak3);
            rankRezultati(propozicije);
        }

        private void calculatePostojeObaPreskoka()
        {
            postojeObaPreskokaPrvoKolo = false;
            foreach (RezultatPreskokFinaleKupa r in Rezultati)
            {
                if (r.TotalObeOcene_PrvoKolo != null)
                {
                    postojeObaPreskokaPrvoKolo = true;
                    break;
                }
            }
            postojeObaPreskokaDrugoKolo = false;
            foreach (RezultatPreskokFinaleKupa r in Rezultati)
            {
                if (r.TotalObeOcene_DrugoKolo != null)
                {
                    postojeObaPreskokaDrugoKolo = true;
                    break;
                }
            }
        }

        public virtual void addGimnasticar(GimnasticarUcesnik g, RezultatskoTakmicenje rezTak,
            RezultatskoTakmicenje rezTak1, RezultatskoTakmicenje rezTak2)
        {
            RezultatPreskokFinaleKupa rezultat = new RezultatPreskokFinaleKupa(this);
            rezultat.Gimnasticar = g;

            propozicijePrvoKoloTak1PreskokNaOsnovuObaPreskoka = rezTak1.Propozicije.Tak1PreskokNaOsnovuObaPreskoka;
            foreach (RezultatPreskok r in rezTak1.Takmicenje1.PoredakPreskok.Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                {
                    rezultat.initPrvoKolo(r);
                    if (r.TotalObeOcene != null)
                    {
                        postojeObaPreskokaPrvoKolo = true;
                    }
                    break;
                }
            }

            propozicijeDrugoKoloTak1PreskokNaOsnovuObaPreskoka = rezTak2.Propozicije.Tak1PreskokNaOsnovuObaPreskoka;
            foreach (RezultatPreskok r in rezTak2.Takmicenje1.PoredakPreskok.Rezultati)
            {
                if (r.Gimnasticar.Equals(g))
                {
                    rezultat.initDrugoKolo(r);
                    if (r.TotalObeOcene != null)
                    {
                        postojeObaPreskokaDrugoKolo = true;
                    }
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
                calculatePostojeObaPreskoka();
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
            strBuilder.AppendLine(Sprava.ToString());

            strBuilder.AppendLine(Rezultati.Count.ToString());
            foreach (RezultatPreskokFinaleKupa r in Rezultati)
                r.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            Sprava = (Sprava)Enum.Parse(typeof(Sprava), reader.ReadLine());
            
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
