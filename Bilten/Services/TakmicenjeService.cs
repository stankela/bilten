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
            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            RezultatskoTakmicenjeDescriptionDAO rezTakDescDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDescriptionDAO();

            /*takmicenjeDAO.Attach(takmicenje, false);
            takmicenjeDAO.Attach(from, false);
            foreach (RezultatskoTakmicenjeDescription d in descriptionsFrom)
            {
                rezTakDescDAO.Attach(d, false);
            }*/


            
            const int MAX = 256;

            TakmicarskaKategorija[] kategorije = new TakmicarskaKategorija[MAX];
            for (int i = 0; i < MAX; ++i)
                kategorije[i] = null;
            foreach (RezultatskoTakmicenje rt in rezTakmicenjaFrom)
            {
                if (kategorije[rt.Kategorija.RedBroj] == null)
                    kategorije[rt.Kategorija.RedBroj] = new TakmicarskaKategorija(rt.Kategorija.Naziv, takmicenje.Gimnastika);
            }

            RezultatskoTakmicenjeDescription[] descriptions = new RezultatskoTakmicenjeDescription[MAX];
            for (int i = 0; i < MAX; ++i)
                descriptions[i] = null;
            foreach (RezultatskoTakmicenje rt in rezTakmicenjaFrom)
            {
                if (descriptions[rt.TakmicenjeDescription.RedBroj] == null)
                {
                    RezultatskoTakmicenjeDescription desc = new RezultatskoTakmicenjeDescription();
                    desc.Naziv = rt.TakmicenjeDescription.Naziv;
                    desc.Propozicije = new Propozicije();
                    clonePropozicije(desc.Propozicije, rt.TakmicenjeDescription.Propozicije);
                    descriptions[rt.TakmicenjeDescription.RedBroj] = desc;
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

            // TODO3: Ovaj metod bi trebalo updateovati svaki put kada se promene neka svojstva koja se kloniraju.

            takmicenje.BrojEOcena = from.BrojEOcena;
            takmicenje.BrojDecimalaD = from.BrojDecimalaD;
            takmicenje.BrojDecimalaE1 = from.BrojDecimalaE1;
            takmicenje.BrojDecimalaE = from.BrojDecimalaE;
            takmicenje.BrojDecimalaPen = from.BrojDecimalaPen;
            takmicenje.BrojDecimalaTotal = from.BrojDecimalaTotal;
            takmicenje.ZavrsenoTak1 = false;

            IList<RezultatskoTakmicenje> rezTakmicenja = new List<RezultatskoTakmicenje>();
            foreach (RezultatskoTakmicenje rtFrom in rezTakmicenjaFrom)
            {
                RezultatskoTakmicenje rt = new RezultatskoTakmicenje(takmicenje,
                    kategorije[rtFrom.Kategorija.RedBroj],
                    descriptions[rtFrom.TakmicenjeDescription.RedBroj],
                    new Propozicije());
                clonePropozicije(rt.Propozicije, rtFrom.Propozicije);
                rezTakmicenja.Add(rt);
            }

            RezultatskoTakmicenje.updateImaEkipnoTakmicenje(rezTakmicenja);
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                rt.updateTakmicenjaFromChangedPropozicije();
            }

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
                        g2 = GimnasticarUcesnikService.createGimnasticarUcesnik(g, g.TakmicarskaKategorija);
                        gimnasticariMap[g.Id] = g2;
                    }
                    else
                        g2 = gimnasticariMap[g.Id];
                    rt.Takmicenje1.addGimnasticar(g2);
                    rt.Takmicenje1.gimnasticarAdded(g2, new List<Ocena>(), rt);
                }
            }

            DrzavaUcesnikDAO drzavaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO();
            KlubUcesnikDAO klubUcesnikDAO = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO();
            RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();
            GimnasticarUcesnikDAO gimnasticarUcesnikDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO();

            for (int i = 0; i < rezTakmicenja.Count; ++i)
            {
                RezultatskoTakmicenje rt = rezTakmicenja[i];
                RezultatskoTakmicenje rtFrom = rezTakmicenjaFrom[i];
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

            takmicenjeDAO.Add(takmicenje);
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

        private static void clonePropozicije(Propozicije propozicije, Propozicije from)
        {
            // TODO3: Dodaj ono sto fali
            // TODO3: Probaj da koristis refleksiju za ovo (ili da ona izvrsi kopiranje, ili samo da te obavesti da li je
            // u medjuvremenu u klasi Propozicije dodato neko novo svojstvo, i ako jeste da generise izuzetak. Mogao bi i 
            // da generisem jednostavan test suite koji bi proveravao ovo)
            // TODO3: Uvedi komentar TODO9 za ono sto mora uvek da se proverava kada se menja program (kao naprimer ovde sto
            // mora da se proverava da li sam u medjuvremenu dodao novo svojstvo u klasu Propozicije.)
            propozicije.PostojiTak2 = from.PostojiTak2;
            propozicije.OdvojenoTak2 = from.OdvojenoTak2;
            propozicije.NeogranicenBrojTakmicaraIzKlubaTak2 = from.NeogranicenBrojTakmicaraIzKlubaTak2;
            propozicije.MaxBrojTakmicaraIzKlubaTak2 = from.MaxBrojTakmicaraIzKlubaTak2;
            propozicije.BrojFinalistaTak2 = from.BrojFinalistaTak2;
            propozicije.BrojRezerviTak2 = from.BrojRezerviTak2;
            propozicije.PostojiTak3 = from.PostojiTak3;
            propozicije.OdvojenoTak3 = from.OdvojenoTak3;
            propozicije.NeogranicenBrojTakmicaraIzKlubaTak3 = from.NeogranicenBrojTakmicaraIzKlubaTak3;
            propozicije.MaxBrojTakmicaraIzKlubaTak3 = from.MaxBrojTakmicaraIzKlubaTak3;
            propozicije.MaxBrojTakmicaraTak3VaziZaDrzavu = from.MaxBrojTakmicaraTak3VaziZaDrzavu;
            propozicije.BrojFinalistaTak3 = from.BrojFinalistaTak3;
            propozicije.BrojRezerviTak3 = from.BrojRezerviTak3;
            propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka = from.KvalifikantiTak3PreskokNaOsnovuObaPreskoka;
            propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka = from.PoredakTak3PreskokNaOsnovuObaPreskoka;
            propozicije.PostojiTak4 = from.PostojiTak4;
            propozicije.OdvojenoTak4 = from.OdvojenoTak4;
            propozicije.BrojRezultataKojiSeBodujuZaEkipu = from.BrojRezultataKojiSeBodujuZaEkipu;
            propozicije.BrojEkipaUFinalu = from.BrojEkipaUFinalu;
            propozicije.JednoTak4ZaSveKategorije = from.JednoTak4ZaSveKategorije;

            propozicije.Tak2FinalnaOcenaJeZbirObaKola = from.Tak2FinalnaOcenaJeZbirObaKola;
            propozicije.Tak2FinalnaOcenaJeMaxObaKola = from.Tak2FinalnaOcenaJeMaxObaKola;
            propozicije.Tak2FinalnaOcenaJeProsekObaKola = from.Tak2FinalnaOcenaJeProsekObaKola;
            propozicije.Tak2NeRacunajProsekAkoNemaOceneIzObaKola = from.Tak2NeRacunajProsekAkoNemaOceneIzObaKola;

            propozicije.Tak4FinalnaOcenaJeZbirObaKola = from.Tak4FinalnaOcenaJeZbirObaKola;
            propozicije.Tak4FinalnaOcenaJeMaxObaKola = from.Tak4FinalnaOcenaJeMaxObaKola;
            propozicije.Tak4FinalnaOcenaJeProsekObaKola = from.Tak4FinalnaOcenaJeProsekObaKola;
            propozicije.Tak4NeRacunajProsekAkoNemaOceneIzObaKola = from.Tak4NeRacunajProsekAkoNemaOceneIzObaKola;

        }


    }
}
