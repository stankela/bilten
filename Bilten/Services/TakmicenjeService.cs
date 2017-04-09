using Bilten.Dao;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;
using NHibernate;
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
            const int MAX = 1024;

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

                    // Ne kopiram clanove ekipe zato sto dodavati clanove ekipe ima smisla jedino ako se znaju
                    // rezultati, a ovaj metod samo pravi pripremu takmicenja i nema nikakvih rezultata.

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

        public static void kreirajNaOsnovuViseKola(Takmicenje takmicenje)
        {
            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            takmicenjeDAO.Attach(takmicenje, false);

            List<Takmicenje> prethodnaKola = new List<Takmicenje>();
            takmicenjeDAO.Attach(takmicenje.PrvoKolo, false);
            takmicenjeDAO.Attach(takmicenje.DrugoKolo, false);

            prethodnaKola.Add(takmicenje.PrvoKolo);
            prethodnaKola.Add(takmicenje.DrugoKolo);
            if (takmicenje.TreceKolo != null)
            {
                takmicenjeDAO.Attach(takmicenje.TreceKolo, false);
                prethodnaKola.Add(takmicenje.TreceKolo);
            }
            if (takmicenje.CetvrtoKolo != null)
            {
                takmicenjeDAO.Attach(takmicenje.CetvrtoKolo, false);
                prethodnaKola.Add(takmicenje.CetvrtoKolo);
            }

            RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();

            List<List<RezultatskoTakmicenje>> rezTakmicenjaPrethodnaKola = new List<List<RezultatskoTakmicenje>>();
            foreach (Takmicenje prethKolo in prethodnaKola)
            {
                rezTakmicenjaPrethodnaKola.Add(new List<RezultatskoTakmicenje>(rezultatskoTakmicenjeDAO
                    .FindByTakmicenjeFetch_Tak1_Gimnasticari(prethKolo.Id)));
            }

            takmicenje.Kategorije.Clear();
            int i = 0;
            while (i < rezTakmicenjaPrethodnaKola[0].Count)
            {
                // Trenutno je implementiran najjednostavniji slucaj, gde se u svakom od prethodnih kola gleda samo prvo
                // takmicenje, i uzimaju se redom kategorije sve dok postoji poklapanje.
                if (rezTakmicenjaPrethodnaKola[0][i].TakmicenjeDescription.RedBroj != 0)
                    break;

                TakmicarskaKategorija kat1 = rezTakmicenjaPrethodnaKola[0][i].Kategorija;
                int j = 1;
                while (j < rezTakmicenjaPrethodnaKola.Count)
                {
                    TakmicarskaKategorija kat2 = rezTakmicenjaPrethodnaKola[j][i].Kategorija;
                    if (!kat2.Equals(kat1))
                        break;
                    ++j;
                }
                if (j < rezTakmicenjaPrethodnaKola.Count)
                    break;
                takmicenje.addKategorija(new TakmicarskaKategorija(kat1.Naziv, takmicenje.Gimnastika));
                ++i;
            }
            if (takmicenje.Kategorije.Count == 0)
                throw new BusinessException("Kategorije iz prethodnih kola se ne poklapaju");

            // prvi description je uvek kao naziv takmicenja.
            takmicenje.TakmicenjeDescriptions.Clear();            
            RezultatskoTakmicenjeDescription desc = new RezultatskoTakmicenjeDescription();
            desc.Naziv = takmicenje.Naziv;
            desc.Propozicije = new Propozicije();
            takmicenje.addTakmicenjeDescription(desc);

            // Takmicenje dodajem ovako rano zato sto se takmicenje.Id koristi dole u createGimnasticarUcesnik
            takmicenjeDAO.Add(takmicenje);

            IList<RezultatskoTakmicenje> rezTakmicenja = new List<RezultatskoTakmicenje>();
            foreach (TakmicarskaKategorija k in takmicenje.Kategorije)
            {
                RezultatskoTakmicenje rt = new RezultatskoTakmicenje(takmicenje, k, desc, new Propozicije());
                // HACK
                rt.Propozicije.PostojiTak2 = true;
                rt.Propozicije.PostojiTak4 = true;
                rt.ImaEkipnoTakmicenje = true;
                rt.KombinovanoEkipnoTak = false;
                rezTakmicenja.Add(rt);
            }

            IDictionary<GimnasticarUcesnik, GimnasticarUcesnik> gimnasticariMap =
                new Dictionary<GimnasticarUcesnik, GimnasticarUcesnik>();
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                foreach (List<RezultatskoTakmicenje> rezTakmicenjaPrethKolo in rezTakmicenjaPrethodnaKola)
                {
                    RezultatskoTakmicenje rtFrom = findRezTakmicenje(rezTakmicenjaPrethKolo,
                        rt.TakmicenjeDescription.Naziv, rt.Kategorija);
                    foreach (GimnasticarUcesnik g in rtFrom.Takmicenje1.Gimnasticari)
                    {
                        GimnasticarUcesnik g2;
                        if (!gimnasticariMap.ContainsKey(g))
                        {
                            g2 = GimnasticarUcesnikService.createGimnasticarUcesnik(g, rt.Kategorija);
                            gimnasticariMap.Add(g2, g2);
                        }
                        else
                            g2 = gimnasticariMap[g];
                        rt.Takmicenje1.addGimnasticar(g2);
                    }
                }
            }

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                foreach (List<RezultatskoTakmicenje> rezTakmicenjaPrethKolo in rezTakmicenjaPrethodnaKola)
                {
                    RezultatskoTakmicenje rtFrom = findRezTakmicenje(rezTakmicenjaPrethKolo,
                        rt.TakmicenjeDescription.Naziv, rt.Kategorija);
                    foreach (Ekipa e in rtFrom.Takmicenje1.Ekipe)
                    {
                        if (rt.Takmicenje1.Ekipe.Contains(e))
                            continue;

                        Ekipa ekipa = new Ekipa();
                        ekipa.Naziv = e.Naziv;
                        ekipa.Kod = e.Kod;
                        rt.Takmicenje1.addEkipa(ekipa);
                    }
                }
            }

            // TODO: Prebaci u domenske klase sto vise koda iz ove funkcije.

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                // TODO3: Ovo ce raditi samo ako su prvo i drugo kolo imali samo jedno takmicenje. (takodje i kod
                // poretka ekipa i sprava)
                PoredakUkupno poredak1
                    = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[0], rt.Kategorija).Takmicenje1.PoredakUkupno;
                PoredakUkupno poredak2
                    = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[1], rt.Kategorija).Takmicenje1.PoredakUkupno;
                PoredakUkupno poredak3 = null;
                PoredakUkupno poredak4 = null;
                if (takmicenje.TreceKolo != null)
                    poredak3 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[2], rt.Kategorija).Takmicenje1.PoredakUkupno;
                if (takmicenje.CetvrtoKolo != null)
                    poredak4 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[3], rt.Kategorija).Takmicenje1.PoredakUkupno;

                if (takmicenje.FinaleKupa)
                    rt.Takmicenje1.PoredakUkupnoFinaleKupa.create(rt, poredak1, poredak2);
                else if (takmicenje.ZbirViseKola)
                    rt.Takmicenje1.PoredakUkupnoZbirViseKola.create(rt, poredak1, poredak2, poredak3, poredak4);
            }

            if (takmicenje.FinaleKupa)
            {
                foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                {
                    RezultatskoTakmicenje rezTak1 =
                        Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[0], rt.Kategorija);
                    RezultatskoTakmicenje rezTak2 =
                        Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[1], rt.Kategorija);

                    rt.Takmicenje1.initPoredakSpravaFinaleKupa(takmicenje.Gimnastika);

                    foreach (Sprava s in Sprave.getSprave(takmicenje.Gimnastika))
                    {
                        if (s != Sprava.Preskok)
                        {
                            rt.Takmicenje1.getPoredakSpravaFinaleKupa(s).create(rt,
                                rezTak1.Takmicenje1.getPoredakSprava(s), rezTak2.Takmicenje1.getPoredakSprava(s));
                        }
                        else
                        {
                            rt.Takmicenje1.getPoredakSpravaFinaleKupa(s).create(rt,
                                rezTak1.Takmicenje1.PoredakPreskok,
                                rezTak2.Takmicenje1.PoredakPreskok,
                                rezTak1.Propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka,
                                rezTak2.Propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka);
                        }
                    }
                }
            }

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                // TODO4: Obradi slucaj kombinovanog ekipnog takmicenja.
                PoredakEkipno poredak1
                    = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[0], rt.Kategorija).Takmicenje1.PoredakEkipno;
                PoredakEkipno poredak2
                    = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[1], rt.Kategorija).Takmicenje1.PoredakEkipno;
                PoredakEkipno poredak3 = null;
                PoredakEkipno poredak4 = null;
                if (takmicenje.TreceKolo != null)
                    poredak3 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[2], rt.Kategorija).Takmicenje1.PoredakEkipno;
                if (takmicenje.CetvrtoKolo != null)
                    poredak4 = Takmicenje.getRezTakmicenje(rezTakmicenjaPrethodnaKola[3], rt.Kategorija).Takmicenje1.PoredakEkipno;

                if (takmicenje.FinaleKupa)
                    rt.Takmicenje1.PoredakEkipnoFinaleKupa.create(rt, poredak1, poredak2);
                else if (takmicenje.ZbirViseKola)
                    rt.Takmicenje1.PoredakEkipnoZbirViseKola.create(rt, poredak1, poredak2, poredak3, poredak4);
            }

            foreach (List<RezultatskoTakmicenje> rezTakmicenjaPrethKolo in rezTakmicenjaPrethodnaKola)
                foreach (RezultatskoTakmicenje rt in rezTakmicenjaPrethKolo)
                    rezultatskoTakmicenjeDAO.Evict(rt);

            EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();
            GimnasticarUcesnikDAO gimnasticarUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                rezultatskoTakmicenjeDAO.Add(rt);
                foreach (Ekipa e in rt.Takmicenje1.Ekipe)
                    ekipaDAO.Add(e);
            }
            foreach (GimnasticarUcesnik g in gimnasticariMap.Values)
            {
                gimnasticarUcesnikDAO.Add(g);
            }
        }

        private static RezultatskoTakmicenje findRezTakmicenje(IList<RezultatskoTakmicenje> rezTakmicenja,
          string nazivDesc, TakmicarskaKategorija kat)
        {
            foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
            {
                if (rezTak.TakmicenjeDescription.Naziv == nazivDesc
                && rezTak.Kategorija.Equals(kat))
                    return rezTak;
            }

            // Nije pronadjeno rez. takmicenje. Ovo je situacija koja se najcesce desava zato sto se najcesce kopira
            // takmicenje za koje postoji samo jedno takmicenje description, i data se description takmicenja koje se
            // kopira i description takmicenja u koje se kopira ne poklapaju (naziv descriptiona po defaultu je naziv
            // takmicenja). Pronadji sva descriptions za datu kategoriju. Description koji trazimo je descriptions sa
            // najnizim brojem.
            List<RezultatskoTakmicenjeDescription> descriptions = new List<RezultatskoTakmicenjeDescription>();
            foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
            {
                if (rezTak.Kategorija.Equals(kat))
                    descriptions.Add(rezTak.TakmicenjeDescription);
            }
            System.ComponentModel.PropertyDescriptor propDesc
                = System.ComponentModel.TypeDescriptor.GetProperties(typeof(RezultatskoTakmicenjeDescription))["RedBroj"];
            descriptions.Sort(new Bilten.Util.SortComparer<RezultatskoTakmicenjeDescription>(
                propDesc, System.ComponentModel.ListSortDirection.Ascending));

            // ponovljen kod sa pocetka funkcije
            foreach (RezultatskoTakmicenje rezTak in rezTakmicenja)
            {
                if (rezTak.TakmicenjeDescription.Naziv == descriptions[0].Naziv
                && rezTak.Kategorija.Equals(kat))
                    return rezTak;
            }

            return null;
        }

    }
}
