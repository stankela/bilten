using Bilten.Dao;
using Bilten.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bilten.Services
{
    public class TakmicenjeService
    {
        public static void addTakmicenje(Takmicenje t, IList<KlubUcesnik> klubovi, IList<DrzavaUcesnik> drzave,
            IList<GimnasticarUcesnik> gimnasticari, IList<RezultatskoTakmicenje> rezTakmicenja,
            IList<SudijaUcesnik> sudije, IList<RasporedSudija> rasporediSudija, IList<RasporedNastupa> rasporediNastupa,
            IList<Ocena> ocene)
        {
            // dodaj takmicenje
            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            takmicenjeDAO.Add(t);

            // kategorije i descriptions se dodaju pomocu transitive persistance

            // dodaj klubove ucesnike
            KlubUcesnikDAO klubUcesnikDAO = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO();
            foreach (KlubUcesnik k in klubovi)
                klubUcesnikDAO.Add(k);

            // dodaj drzave ucesnike
            DrzavaUcesnikDAO drzavaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO();
            foreach (DrzavaUcesnik d in drzave)
                drzavaUcesnikDAO.Add(d);

            // dodaj gimnasticare ucesnike
            GimnasticarUcesnikDAO gimnasticarUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
            foreach (GimnasticarUcesnik g in gimnasticari)
                gimnasticarUcesnikDAO.Add(g);

            // dodaj rezultatska takmicenja i ekipe
            RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();
            foreach (RezultatskoTakmicenje r in rezTakmicenja)
            {
                foreach (Ekipa e in r.Takmicenje1.Ekipe)
                    ekipaDAO.Add(e);
                rezultatskoTakmicenjeDAO.Add(r);
            }

            // dodaj sudije ucesnike
            SudijaUcesnikDAO sudijaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetSudijaUcesnikDAO();
            foreach (SudijaUcesnik s in sudije)
                sudijaUcesnikDAO.Add(s);

            // dodaj rasporede sudija
            RasporedSudijaDAO rasporedSudijaDAO = DAOFactoryFactory.DAOFactory.GetRasporedSudijaDAO();
            foreach (RasporedSudija r in rasporediSudija)
                rasporedSudijaDAO.Add(r);

            // dodaj rasporede nastupa
            RasporedNastupaDAO rasporedNastupaDAO = DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO();
            foreach (RasporedNastupa r in rasporediNastupa)
                rasporedNastupaDAO.Add(r);

            // dodaj ocene
            OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
            foreach (Ocena o in ocene)
                ocenaDAO.Add(o);
        }

        public static void deleteTakmicenje(int takmicenjeId)
        {
            // TODO4: Potrebna je provera da li se neko takmicenje (finale kupa ili zbir vise kola) referise na ovo
            // takmicenje.

            // brisi ocene
            OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
            foreach (Ocena o in ocenaDAO.FindByTakmicenje(takmicenjeId))
                ocenaDAO.Delete(o);

            // brisi rasporede nastupa
            RasporedNastupaDAO rasporedNastupaDAO = DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO();
            foreach (RasporedNastupa r in rasporedNastupaDAO.FindByTakmicenje(takmicenjeId))
                rasporedNastupaDAO.Delete(r);

            // brisi rasporede sudija
            RasporedSudijaDAO rasporedSudijaDAO = DAOFactoryFactory.DAOFactory.GetRasporedSudijaDAO();
            foreach (RasporedSudija r in rasporedSudijaDAO.FindByTakmicenje(takmicenjeId))
                rasporedSudijaDAO.Delete(r);

            // brisi sudije ucesnike
            SudijaUcesnikDAO sudijaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetSudijaUcesnikDAO();
            foreach (SudijaUcesnik s in sudijaUcesnikDAO.FindByTakmicenje(takmicenjeId))
                sudijaUcesnikDAO.Delete(s);

            // brisi rezultatska takmicenja i ekipe
            RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();
            foreach (RezultatskoTakmicenje r in rezultatskoTakmicenjeDAO.FindByTakmicenje(takmicenjeId))
            {
                foreach (Ekipa e in r.Takmicenje1.Ekipe)
                    ekipaDAO.Delete(e);
                rezultatskoTakmicenjeDAO.Delete(r);
            }

            // brisi gimnasticare ucesnike
            GimnasticarUcesnikDAO gimnasticarUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
            foreach (GimnasticarUcesnik g in gimnasticarUcesnikDAO.FindByTakmicenje(takmicenjeId))
                gimnasticarUcesnikDAO.Delete(g);

            // brisi klubove ucesnike
            KlubUcesnikDAO klubUcesnikDAO = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO();
            foreach (KlubUcesnik k in klubUcesnikDAO.FindByTakmicenje(takmicenjeId))
                klubUcesnikDAO.Delete(k);

            // brisi drzave ucesnike
            DrzavaUcesnikDAO drzavaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO();
            foreach (DrzavaUcesnik d in drzavaUcesnikDAO.FindByTakmicenje(takmicenjeId))
                drzavaUcesnikDAO.Delete(d);

            // brisi kategorije
            TakmicarskaKategorijaDAO takmicarskaKategorijaDAO = DAOFactoryFactory.DAOFactory.GetTakmicarskaKategorijaDAO();
            foreach (TakmicarskaKategorija k in takmicarskaKategorijaDAO.FindByTakmicenje(takmicenjeId))
                takmicarskaKategorijaDAO.Delete(k);

            // brisi descriptions
            RezultatskoTakmicenjeDescriptionDAO rezTakDescDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDescriptionDAO();
            foreach (RezultatskoTakmicenjeDescription d in rezTakDescDAO.FindByTakmicenje(takmicenjeId))
                rezTakDescDAO.Delete(d);

            // brisi takmicenje
            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            takmicenjeDAO.Delete(takmicenjeDAO.FindById(takmicenjeId));
        }

        public static bool deleteTakmicenje(string naziv, Gimnastika gimnastika, DateTime datum)
        {
            Takmicenje t = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO()
                .FindByNazivGimnastikaDatum(naziv, gimnastika, datum);
            if (t != null)
            {
                deleteTakmicenje(t.Id);
                return true;
            }
            return false;
        }

        public static void createFromPrevTakmicenje(Takmicenje takmicenje, Takmicenje from,
            IList<RezultatskoTakmicenje> rezTakmicenjaFrom,
            IDictionary<int, List<GimnasticarUcesnik>> rezTakToGimnasticarUcesnikMap)
        {
            const int MAX = 256;

            TakmicarskaKategorija[] kategorije = new TakmicarskaKategorija[MAX];
            for (int i = 0; i < MAX; ++i)
                kategorije[i] = null;
            foreach (RezultatskoTakmicenje rtFrom in rezTakmicenjaFrom)
            {
                if (kategorije[rtFrom.Kategorija.RedBroj] == null)
                    kategorije[rtFrom.Kategorija.RedBroj]
                        = new TakmicarskaKategorija(rtFrom.Kategorija.Naziv, takmicenje.Gimnastika);
            }

            PropozicijeDAO propozicijeDAO = DAOFactoryFactory.DAOFactory.GetPropozicijeDAO();
            foreach (RezultatskoTakmicenje rtFrom in rezTakmicenjaFrom)
            {
                propozicijeDAO.Attach(rtFrom.TakmicenjeDescription.Propozicije, false);
                propozicijeDAO.Attach(rtFrom.Propozicije, false);
            }

            RezultatskoTakmicenjeDescription[] descriptions = new RezultatskoTakmicenjeDescription[MAX];
            for (int i = 0; i < MAX; ++i)
                descriptions[i] = null;
            foreach (RezultatskoTakmicenje rtFrom in rezTakmicenjaFrom)
            {
                if (descriptions[rtFrom.TakmicenjeDescription.RedBroj] == null)
                {
                    RezultatskoTakmicenjeDescription desc = new RezultatskoTakmicenjeDescription();
                    desc.Naziv = rtFrom.TakmicenjeDescription.Naziv;
                    desc.Propozicije = new Propozicije();

                    // Apdejtujem jedino propozicije za takmicenje 4 zbog kombinovanog ekipnog takmicenja.
                    // Ostale propozicije su na inicijalnim vrednostima.
                    rtFrom.TakmicenjeDescription.Propozicije.copyTakmicenje4To(desc.Propozicije);                    
                    
                    descriptions[rtFrom.TakmicenjeDescription.RedBroj] = desc;
                }
            }

            takmicenje.Kategorije.Clear();
            for (int i = 0; i < MAX; ++i)
            {
                if (kategorije[i] != null)
                    takmicenje.addKategorija(kategorije[i]);
            }

            takmicenje.TakmicenjeDescriptions.Clear();
            bool first = true;
            for (int i = 0; i < MAX; ++i)
            {
                if (descriptions[i] != null)
                {
                    if (first)
                    {
                        // prvi description je uvek kao naziv takmicenja.
                        RezultatskoTakmicenjeDescription desc = new RezultatskoTakmicenjeDescription();
                        desc.Naziv = takmicenje.Naziv;
                        desc.Propozicije = descriptions[i].Propozicije;  // klonirane propozicije
                        descriptions[i] = desc;
                        first = false;
                    }
                    takmicenje.addTakmicenjeDescription(descriptions[i]);
                }
            }

            IList<RezultatskoTakmicenje> rezTakmicenja = new List<RezultatskoTakmicenje>();
            foreach (RezultatskoTakmicenje rtFrom in rezTakmicenjaFrom)
            {
                RezultatskoTakmicenje rt = new RezultatskoTakmicenje(takmicenje,
                    kategorije[rtFrom.Kategorija.RedBroj],
                    descriptions[rtFrom.TakmicenjeDescription.RedBroj],
                    new Propozicije());
                rtFrom.Propozicije.copyTakmicenje4To(rt.Propozicije);
                rezTakmicenja.Add(rt);
            }

            RezultatskoTakmicenje.updateImaEkipnoTakmicenje(rezTakmicenja);

            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            takmicenjeDAO.Add(takmicenje);

            IDictionary<int, GimnasticarUcesnik> gimnasticariMap = new Dictionary<int, GimnasticarUcesnik>();
            for (int i = 0; i < rezTakmicenja.Count; ++i)
            {
                RezultatskoTakmicenje rt = rezTakmicenja[i];
                RezultatskoTakmicenje rtFrom = rezTakmicenjaFrom[i];
                foreach (GimnasticarUcesnik g in rezTakToGimnasticarUcesnikMap[rtFrom.Id])
                {
                    GimnasticarUcesnik g2;
                    if (!gimnasticariMap.ContainsKey(g.Id))
                    {
                        g2 = GimnasticarUcesnikService.createGimnasticarUcesnik(g,
                            kategorije[g.TakmicarskaKategorija.RedBroj]);
                        gimnasticariMap[g.Id] = g2;
                    }
                    else
                        g2 = gimnasticariMap[g.Id];
                    rt.Takmicenje1.addGimnasticar(g2);
                    rt.Takmicenje1.gimnasticarAdded(g2, new List<Ocena>(), rt);
                }
            }

            Takmicenje1DAO takmicenje1DAO = DAOFactoryFactory.DAOFactory.GetTakmicenje1DAO();
            for (int i = 0; i < rezTakmicenja.Count; ++i)
            {
                RezultatskoTakmicenje rt = rezTakmicenja[i];
                RezultatskoTakmicenje rtFrom = rezTakmicenjaFrom[i];
                takmicenje1DAO.Attach(rtFrom.Takmicenje1, false);
                foreach (Ekipa e in rtFrom.Takmicenje1.Ekipe)
                {
                    Ekipa ekipa = new Ekipa();
                    ekipa.Naziv = e.Naziv;
                    ekipa.Kod = e.Kod;

                    foreach (GimnasticarUcesnik g in e.Gimnasticari)
                    {
                        if (gimnasticariMap.ContainsKey(g.Id))
                            ekipa.addGimnasticar(gimnasticariMap[g.Id]);
                    }

                    rt.Takmicenje1.addEkipa(ekipa);
                }
            }

            RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                rezultatskoTakmicenjeDAO.Add(rt);
                foreach (Ekipa e in rt.Takmicenje1.Ekipe)
                    ekipaDAO.Add(e);
            }

            GimnasticarUcesnikDAO gimnasticarUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();
            foreach (GimnasticarUcesnik g in gimnasticariMap.Values)
            {
                gimnasticarUcesnikDAO.Add(g);
            }
        }
    }
}
