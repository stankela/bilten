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

        private int takmicenjeId;
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

        private string dumpTakmicenje()
        {
            StringBuilder strBuilder = new StringBuilder();
            Takmicenje takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
            takmicenje.dump(strBuilder);
            return strBuilder.ToString();
        }

        private string dumpKlubovi()
        {
            StringBuilder strBuilder = new StringBuilder();
            IList<KlubUcesnik> klubovi = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO().FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(klubovi.Count.ToString());
            foreach (KlubUcesnik k in klubovi)
                k.dump(strBuilder);
            return strBuilder.ToString();
        }

        private string dumpDrzave()
        {
            StringBuilder strBuilder = new StringBuilder();
            IList<DrzavaUcesnik> drzave = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO().FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(drzave.Count.ToString());
            foreach (DrzavaUcesnik d in drzave)
                d.dump(strBuilder);
            return strBuilder.ToString();
        }

        private string dumpGimnasticari()
        {
            StringBuilder strBuilder = new StringBuilder();
            IList<GimnasticarUcesnik> gimnasticari = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO()
                .FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(gimnasticari.Count.ToString());
            foreach (GimnasticarUcesnik g in gimnasticari)
                g.dump(strBuilder);
            return strBuilder.ToString();
        }

        private string dumpOcene()
        {
            StringBuilder strBuilder = new StringBuilder();
            IList<Ocena> ocene = DAOFactoryFactory.DAOFactory.GetOcenaDAO().FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(ocene.Count.ToString());
            foreach (Ocena o in ocene)
                o.dump(strBuilder);
            return strBuilder.ToString();
        }

        private string dumpRasporediNastupa()
        {
            StringBuilder strBuilder = new StringBuilder();
            IList<RasporedNastupa> rasporediNastupa = DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO()
                .FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(rasporediNastupa.Count.ToString());
            foreach (RasporedNastupa r in rasporediNastupa)
                r.dump(strBuilder);
            return strBuilder.ToString();
        }

        private string dumpSudije()
        {
            StringBuilder strBuilder = new StringBuilder();
            IList<SudijaUcesnik> sudije = DAOFactoryFactory.DAOFactory.GetSudijaUcesnikDAO().FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(sudije.Count.ToString());
            foreach (SudijaUcesnik s in sudije)
                s.dump(strBuilder);
            return strBuilder.ToString();
        }

        private string dumpRasporediSudija()
        {
            StringBuilder strBuilder = new StringBuilder();
            IList<RasporedSudija> rasporediSudija = DAOFactoryFactory.DAOFactory.GetRasporedSudijaDAO()
                .FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(rasporediSudija.Count.ToString());
            foreach (RasporedSudija r in rasporediSudija)
                r.dump(strBuilder);
            return strBuilder.ToString();
        }

        private string dumpRezTakmicenja()
        {
            StringBuilder strBuilder = new StringBuilder();
            IList<RezultatskoTakmicenje> rezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                .FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(rezTakmicenja.Count.ToString());
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                rt.dump(strBuilder);
            return strBuilder.ToString();
        }

        private string dumpAll()
        {
            return BILTEN_TAKMICENJE_DUMP + "\n" +
                dumpTakmicenje() +
                dumpKlubovi() +
                dumpDrzave() +
                dumpGimnasticari() +
                dumpOcene() +
                dumpRasporediNastupa() +
                dumpSudije() +
                dumpRasporediSudija() +
                dumpRezTakmicenja();
        }

        public void dumpToFile(int takmicenjeId, string fileName)
        {
            this.takmicenjeId = takmicenjeId;
            File.WriteAllText(fileName, dumpAll());
        }

        private void loadFromDump(string dump)
        {
            IdMap map = new IdMap();

            using (StringReader reader = new StringReader(dump))
            {
                if (reader.ReadLine() != BILTEN_TAKMICENJE_DUMP)
                    throw new Exception("Neuspesno ucitavanje takmicenja.");

                int prvoKoloId, drugoKoloId, treceKoloId, cetvrtoKoloId;

                string id = reader.ReadLine();
                takmicenje = new Takmicenje();

                // odmah dodajem takmicenje u mapu, za slucaj da bude potrebno u loadFromDump
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

                klubovi = new List<KlubUcesnik>();
                int count = int.Parse(reader.ReadLine());
                for (int i = 0; i < count; ++i)
                {
                    id = reader.ReadLine();
                    KlubUcesnik k = new KlubUcesnik();
                    map.kluboviMap.Add(int.Parse(id), k);
                    k.loadFromDump(reader, map);
                    klubovi.Add(k);
                }

                drzave = new List<DrzavaUcesnik>();
                count = int.Parse(reader.ReadLine());
                for (int i = 0; i < count; ++i)
                {
                    id = reader.ReadLine();
                    DrzavaUcesnik d = new DrzavaUcesnik();
                    map.drzaveMap.Add(int.Parse(id), d);
                    d.loadFromDump(reader, map);
                    drzave.Add(d);
                }

                gimnasticari = new List<GimnasticarUcesnik>();
                count = int.Parse(reader.ReadLine());
                for (int i = 0; i < count; ++i)
                {
                    id = reader.ReadLine();
                    GimnasticarUcesnik g = new GimnasticarUcesnik();
                    map.gimnasticariMap.Add(int.Parse(id), g);
                    g.loadFromDump(reader, map);
                    gimnasticari.Add(g);
                }

                ocene = new List<Ocena>();
                count = int.Parse(reader.ReadLine());
                for (int i = 0; i < count; ++i)
                {
                    id = reader.ReadLine();
                    Ocena o = new Ocena();
                    o.loadFromDump(reader, map);
                    ocene.Add(o);
                }

                rasporediNastupa = new List<RasporedNastupa>();
                count = int.Parse(reader.ReadLine());
                for (int i = 0; i < count; ++i)
                {
                    id = reader.ReadLine();
                    RasporedNastupa r = new RasporedNastupa();
                    r.loadFromDump(reader, map);
                    rasporediNastupa.Add(r);
                }

                sudije = new List<SudijaUcesnik>();
                count = int.Parse(reader.ReadLine());
                for (int i = 0; i < count; ++i)
                {
                    id = reader.ReadLine();
                    SudijaUcesnik s = new SudijaUcesnik();
                    map.sudijeMap.Add(int.Parse(id), s);
                    s.loadFromDump(reader, map);
                    sudije.Add(s);
                }

                rasporediSudija = new List<RasporedSudija>();
                count = int.Parse(reader.ReadLine());
                for (int i = 0; i < count; ++i)
                {
                    id = reader.ReadLine();
                    RasporedSudija r = new RasporedSudija();
                    r.loadFromDump(reader, map);
                    rasporediSudija.Add(r);
                }

                rezTakmicenja = new List<RezultatskoTakmicenje>();
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