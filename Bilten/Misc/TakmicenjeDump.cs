using Bilten.Dao;
using Bilten.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Misc
{
    public class TakmicenjeDump
    {
        private const string BILTEN_TAKMICENJE_DUMP = "BILTEN_TAKMICENJE_DUMP";

        public Takmicenje takmicenje;
        public IList<KlubUcesnik> klubovi;
        public IList<DrzavaUcesnik> drzave;
        public IList<GimnasticarUcesnik> gimnasticari;
        public IList<Ocena> ocene;
        public IList<RasporedNastupa> rasporediNastupa;
        public IList<SudijaUcesnik> sudije;
        public IList<RasporedSudija> rasporediSudija;
        public IList<RezultatskoTakmicenje> rezTakmicenja;

        public TakmicenjeDump()
        {

        }

        private string dumpTakmicenje(int takmicenjeId)
        {
            StringBuilder strBuilder = new StringBuilder();
            Takmicenje takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
            takmicenje.dump(strBuilder);
            return strBuilder.ToString();
        }

        private string dumpKlubovi(int takmicenjeId)
        {
            StringBuilder strBuilder = new StringBuilder();
            IList<KlubUcesnik> klubovi = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO().FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(klubovi.Count.ToString());
            foreach (KlubUcesnik k in klubovi)
                k.dump(strBuilder);
            return strBuilder.ToString();
        }

        private string dumpDrzave(int takmicenjeId)
        {
            StringBuilder strBuilder = new StringBuilder();
            IList<DrzavaUcesnik> drzave = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO().FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(drzave.Count.ToString());
            foreach (DrzavaUcesnik d in drzave)
                d.dump(strBuilder);
            return strBuilder.ToString();
        }

        private string dumpGimnasticari(int takmicenjeId)
        {
            StringBuilder strBuilder = new StringBuilder();
            IList<GimnasticarUcesnik> gimnasticari = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO()
                .FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(gimnasticari.Count.ToString());
            foreach (GimnasticarUcesnik g in gimnasticari)
                g.dump(strBuilder);
            return strBuilder.ToString();
        }

        private string dumpOcene(int takmicenjeId)
        {
            StringBuilder strBuilder = new StringBuilder();
            IList<Ocena> ocene = DAOFactoryFactory.DAOFactory.GetOcenaDAO().FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(ocene.Count.ToString());
            foreach (Ocena o in ocene)
                o.dump(strBuilder);
            return strBuilder.ToString();
        }

        private string dumpRasporediNastupa(int takmicenjeId)
        {
            StringBuilder strBuilder = new StringBuilder();
            IList<RasporedNastupa> rasporediNastupa = DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO()
                .FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(rasporediNastupa.Count.ToString());
            foreach (RasporedNastupa r in rasporediNastupa)
                r.dump(strBuilder);
            return strBuilder.ToString();
        }

        private string dumpSudije(int takmicenjeId)
        {
            StringBuilder strBuilder = new StringBuilder();
            IList<SudijaUcesnik> sudije = DAOFactoryFactory.DAOFactory.GetSudijaUcesnikDAO().FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(sudije.Count.ToString());
            foreach (SudijaUcesnik s in sudije)
                s.dump(strBuilder);
            return strBuilder.ToString();
        }

        private string dumpRasporediSudija(int takmicenjeId)
        {
            StringBuilder strBuilder = new StringBuilder();
            IList<RasporedSudija> rasporediSudija = DAOFactoryFactory.DAOFactory.GetRasporedSudijaDAO()
                .FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(rasporediSudija.Count.ToString());
            foreach (RasporedSudija r in rasporediSudija)
                r.dump(strBuilder);
            return strBuilder.ToString();
        }

        private string dumpRezTakmicenja(int takmicenjeId)
        {
            StringBuilder strBuilder = new StringBuilder();
            IList<RezultatskoTakmicenje> rezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                .FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(rezTakmicenja.Count.ToString());
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                rt.dump(strBuilder);
            return strBuilder.ToString();
        }

        public string dumpAll(int takmicenjeId)
        {
            return BILTEN_TAKMICENJE_DUMP + "\n" +
                Program.VERZIJA_PROGRAMA + "\n" +
                dumpTakmicenje(takmicenjeId) +
                dumpKlubovi(takmicenjeId) +
                dumpDrzave(takmicenjeId) +
                dumpGimnasticari(takmicenjeId) +
                dumpOcene(takmicenjeId) +
                dumpRasporediNastupa(takmicenjeId) +
                dumpSudije(takmicenjeId) +
                dumpRasporediSudija(takmicenjeId) +
                dumpRezTakmicenja(takmicenjeId);
        }

        public void dumpToFile(int takmicenjeId, string fileName)
        {
            File.WriteAllText(fileName, dumpAll(takmicenjeId));
        }

        public void loadFromDump(string dump)
        {
            IdMap map = new IdMap();

            // clear
            takmicenje = new Takmicenje();
            klubovi = new List<KlubUcesnik>();
            drzave = new List<DrzavaUcesnik>();
            gimnasticari = new List<GimnasticarUcesnik>();
            ocene = new List<Ocena>();
            rasporediNastupa = new List<RasporedNastupa>();
            sudije = new List<SudijaUcesnik>();
            rasporediSudija = new List<RasporedSudija>();
            rezTakmicenja = new List<RezultatskoTakmicenje>();

            using (StringReader reader = new StringReader(dump))
            {
                if (reader.ReadLine() != BILTEN_TAKMICENJE_DUMP)
                    throw new Exception("Neuspesno ucitavanje takmicenja.");

                // Verzija programa
                reader.ReadLine();

                int prvoKoloId, drugoKoloId, treceKoloId, cetvrtoKoloId;

                // load takmicenje
                string id = reader.ReadLine();
                map.takmicenjeMap.Add(int.Parse(id), takmicenje);
                takmicenje.loadFromDump(reader, map, out prvoKoloId, out drugoKoloId,
                    out treceKoloId, out cetvrtoKoloId);

                takmicenje.PrvoKolo = prvoKoloId == -1 ? null :
                    DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(prvoKoloId);
                takmicenje.DrugoKolo = drugoKoloId == -1 ? null :
                    DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(drugoKoloId);
                takmicenje.TreceKolo = treceKoloId == -1 ? null :
                    DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(treceKoloId);
                takmicenje.CetvrtoKolo = cetvrtoKoloId == -1 ? null :
                    DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(cetvrtoKoloId);

                // load klubovi
                int count = int.Parse(reader.ReadLine());
                for (int i = 0; i < count; ++i)
                {
                    id = reader.ReadLine();
                    KlubUcesnik k = new KlubUcesnik();
                    map.kluboviMap.Add(int.Parse(id), k);
                    k.loadFromDump(reader, map);
                    klubovi.Add(k);
                }

                // load drzave
                count = int.Parse(reader.ReadLine());
                for (int i = 0; i < count; ++i)
                {
                    id = reader.ReadLine();
                    DrzavaUcesnik d = new DrzavaUcesnik();
                    map.drzaveMap.Add(int.Parse(id), d);
                    d.loadFromDump(reader, map);
                    drzave.Add(d);
                }

                // load gimnasticari
                count = int.Parse(reader.ReadLine());
                for (int i = 0; i < count; ++i)
                {
                    id = reader.ReadLine();
                    GimnasticarUcesnik g = new GimnasticarUcesnik();
                    map.gimnasticariMap.Add(int.Parse(id), g);
                    g.loadFromDump(reader, map);
                    gimnasticari.Add(g);
                }

                // load ocene
                count = int.Parse(reader.ReadLine());
                for (int i = 0; i < count; ++i)
                {
                    id = reader.ReadLine();
                    Ocena o = new Ocena();
                    o.loadFromDump(reader, map);
                    ocene.Add(o);
                }

                // load rasporedi nastupa
                count = int.Parse(reader.ReadLine());
                for (int i = 0; i < count; ++i)
                {
                    id = reader.ReadLine();
                    RasporedNastupa r = new RasporedNastupa();
                    r.loadFromDump(reader, map);
                    rasporediNastupa.Add(r);
                }

                // load sudije
                count = int.Parse(reader.ReadLine());
                for (int i = 0; i < count; ++i)
                {
                    id = reader.ReadLine();
                    SudijaUcesnik s = new SudijaUcesnik();
                    map.sudijeMap.Add(int.Parse(id), s);
                    s.loadFromDump(reader, map);
                    sudije.Add(s);
                }

                // load rasporedi sudija
                count = int.Parse(reader.ReadLine());
                for (int i = 0; i < count; ++i)
                {
                    id = reader.ReadLine();
                    RasporedSudija r = new RasporedSudija();
                    r.loadFromDump(reader, map);
                    rasporediSudija.Add(r);
                }

                // load rezultatska takmicenja
                count = int.Parse(reader.ReadLine());
                for (int i = 0; i < count; ++i)
                {
                    id = reader.ReadLine();
                    RezultatskoTakmicenje rt = new RezultatskoTakmicenje();
                    rt.loadFromDump(reader, map);
                    rezTakmicenja.Add(rt);
                }
            }
        }

        public void loadFromFile(string fileName)
        {
            loadFromDump(File.ReadAllText(fileName));
        }
    }
}