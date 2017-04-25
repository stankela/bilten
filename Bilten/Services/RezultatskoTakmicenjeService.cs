using Bilten.Dao;
using Bilten.Domain;
using Bilten.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bilten.Services
{
    public class RezultatskoTakmicenjeService
    {
        public static void addGimnasticariToRezTak(IList<GimnasticarUcesnik> gimnasticari, RezultatskoTakmicenje rezTak,
            IList<GimnasticarUcesnik> addedGimnasticari)
        {
            RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            rezultatskoTakmicenjeDAO.Attach(rezTak, false);

            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            Takmicenje takmicenje = takmicenjeDAO.FindById(rezTak.Takmicenje.Id);

            RezultatskoTakmicenje rezTak1 = null;
            RezultatskoTakmicenje rezTak2 = null;
            RezultatskoTakmicenje rezTak3 = null;
            RezultatskoTakmicenje rezTak4 = null;
            if (takmicenje.FinaleKupa || takmicenje.ZbirViseKola)
            {
                rezTak1 = rezultatskoTakmicenjeDAO.FindByTakmicenjeKatDescFetch_Tak1_Gimnasticari(
                    takmicenje.PrvoKolo.Id, rezTak.Kategorija.Naziv, 0);
                rezTak2 = rezultatskoTakmicenjeDAO.FindByTakmicenjeKatDescFetch_Tak1_Gimnasticari(
                    takmicenje.DrugoKolo.Id, rezTak.Kategorija.Naziv, 0);
                if (takmicenje.ZbirViseKola)
                {
                    if (takmicenje.TreceKolo != null)
                    {
                        rezTak3 = rezultatskoTakmicenjeDAO.FindByTakmicenjeKatDescFetch_Tak1_Gimnasticari(
                            takmicenje.TreceKolo.Id, rezTak.Kategorija.Naziv, 0);
                    }
                    if (takmicenje.CetvrtoKolo != null)
                    {
                        rezTak4 = rezultatskoTakmicenjeDAO.FindByTakmicenjeKatDescFetch_Tak1_Gimnasticari(
                            takmicenje.CetvrtoKolo.Id, rezTak.Kategorija.Naziv, 0);
                    }
                }
            }

            OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                if (rezTak.Takmicenje1.addGimnasticar(g))
                {
                    IList<Ocena> ocene = ocenaDAO.FindByGimnasticar(g, DeoTakmicenjaKod.Takmicenje1);
                    if (takmicenje.FinaleKupa)
                        rezTak.Takmicenje1.updateRezultatiOnGimnasticarAdded(g, ocene, rezTak, rezTak1, rezTak2);
                    else if (takmicenje.ZbirViseKola)
                        rezTak.Takmicenje1.updateRezultatiOnGimnasticarAdded(g, rezTak, rezTak1, rezTak2, rezTak3, rezTak4);
                    else
                        rezTak.Takmicenje1.updateRezultatiOnGimnasticarAdded(g, ocene, rezTak);

                    foreach (Ocena o in ocene)
                        ocenaDAO.Evict(o);

                    addedGimnasticari.Add(g);
                }
            }

            takmicenjeDAO.Evict(takmicenje);
            if (rezTak1 != null)
                rezultatskoTakmicenjeDAO.Evict(rezTak1);
            if (rezTak2 != null)
                rezultatskoTakmicenjeDAO.Evict(rezTak2);
            if (rezTak3 != null)
                rezultatskoTakmicenjeDAO.Evict(rezTak3);
            if (rezTak4 != null)
                rezultatskoTakmicenjeDAO.Evict(rezTak4);
            
            if (addedGimnasticari.Count > 0)
                DAOFactoryFactory.DAOFactory.GetTakmicenje1DAO().Update(rezTak.Takmicenje1);
        }

        public static void deleteGimnasticariFromRezTak(IList<GimnasticarUcesnik> gimnasticari, RezultatskoTakmicenje rezTak)
        {
            DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO().Attach(rezTak, false);

            OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                rezTak.Takmicenje1.removeGimnasticar(g);
                IList<Ocena> ocene = ocenaDAO.FindByGimnasticar(g, DeoTakmicenjaKod.Takmicenje1);
                rezTak.Takmicenje1.updateRezultatiOnGimnasticarDeleted(g, ocene, rezTak);
                foreach (Ocena o in ocene)
                    ocenaDAO.Evict(o);
            }

            DAOFactoryFactory.DAOFactory.GetTakmicenje1DAO().Update(rezTak.Takmicenje1);
        }

        public static void addEkipaToRezTak(Ekipa ekipa, RezultatskoTakmicenje rezTak)
        {
            RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            rezultatskoTakmicenjeDAO.Attach(rezTak, false);

            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            Takmicenje takmicenje = takmicenjeDAO.FindById(rezTak.Takmicenje.Id);

            RezultatskoTakmicenje rezTak1 = null;
            RezultatskoTakmicenje rezTak2 = null;
            RezultatskoTakmicenje rezTak3 = null;
            RezultatskoTakmicenje rezTak4 = null;
            if (takmicenje.FinaleKupa || takmicenje.ZbirViseKola)
            {
                rezTak1 = rezultatskoTakmicenjeDAO.FindByTakmicenjeFetch_Tak1_PoredakEkipno_Ekipe(
                    takmicenje.PrvoKolo.Id, rezTak.Kategorija.Naziv, 0);
                rezTak2 = rezultatskoTakmicenjeDAO.FindByTakmicenjeFetch_Tak1_PoredakEkipno_Ekipe(
                    takmicenje.DrugoKolo.Id, rezTak.Kategorija.Naziv, 0);
                if (takmicenje.ZbirViseKola)
                {
                    if (takmicenje.TreceKolo != null)
                    {
                        rezTak3 = rezultatskoTakmicenjeDAO.FindByTakmicenjeFetch_Tak1_PoredakEkipno_Ekipe(
                            takmicenje.TreceKolo.Id, rezTak.Kategorija.Naziv, 0);
                    }
                    if (takmicenje.CetvrtoKolo != null)
                    {
                        rezTak4 = rezultatskoTakmicenjeDAO.FindByTakmicenjeFetch_Tak1_PoredakEkipno_Ekipe(
                            takmicenje.CetvrtoKolo.Id, rezTak.Kategorija.Naziv, 0);
                    }
                }
            }

            if (rezTak.Takmicenje1.addEkipa(ekipa))
            {
                DAOFactoryFactory.DAOFactory.GetEkipaDAO().Add(ekipa);

                if (takmicenje.FinaleKupa)
                {
                    rezTak.Takmicenje1.updateRezultatiOnEkipaAdded(ekipa,
                        findRezultatiUkupnoForEkipa(takmicenje.Id, ekipa), rezTak, rezTak1, rezTak2);
                }
                else if (takmicenje.ZbirViseKola)
                    rezTak.Takmicenje1.updateRezultatiOnEkipaAdded(ekipa, rezTak, rezTak1, rezTak2, rezTak3, rezTak4);
                else
                {
                    rezTak.Takmicenje1.updateRezultatiOnEkipaAdded(ekipa, rezTak,
                        findRezultatiUkupnoForEkipa(takmicenje.Id, ekipa));
                }

                // snimi ekipe i poredak ekipno
                DAOFactoryFactory.DAOFactory.GetTakmicenje1DAO().Update(rezTak.Takmicenje1);
            }

            takmicenjeDAO.Evict(takmicenje);
            if (rezTak1 != null)
                rezultatskoTakmicenjeDAO.Evict(rezTak1);
            if (rezTak2 != null)
                rezultatskoTakmicenjeDAO.Evict(rezTak2);
            if (rezTak3 != null)
                rezultatskoTakmicenjeDAO.Evict(rezTak3);
            if (rezTak4 != null)
                rezultatskoTakmicenjeDAO.Evict(rezTak4);
        }

        public static void updateEkipa(Ekipa ekipa, RezultatskoTakmicenje rezTakmicenje)
        {
            RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            rezultatskoTakmicenjeDAO.Attach(rezTakmicenje, false);

            DAOFactoryFactory.DAOFactory.GetEkipaDAO().Update(ekipa);

            rezTakmicenje.Takmicenje1.updateRezultatiOnEkipaUpdated(ekipa, rezTakmicenje,
                findRezultatiUkupnoForEkipa(rezTakmicenje.Takmicenje.Id, ekipa));

            DAOFactoryFactory.DAOFactory.GetTakmicenje1DAO().Update(rezTakmicenje.Takmicenje1);
        }

        public static void deleteEkipaFromRezTak(Ekipa e, RezultatskoTakmicenje rezTak)
        {
            DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO().Attach(rezTak, false);

            EkipaDAO ekipaDAO = DAOFactoryFactory.DAOFactory.GetEkipaDAO();
            ekipaDAO.Attach(e, false);
            
            rezTak.Takmicenje1.removeEkipa(e);
            rezTak.Takmicenje1.updateRezultatiOnEkipaDeleted(e, rezTak);
            ekipaDAO.Delete(e);
            DAOFactoryFactory.DAOFactory.GetTakmicenje1DAO().Update(rezTak.Takmicenje1);
        }

        public static List<RezultatUkupno> findRezultatiUkupnoForEkipa(int takmicenjeId, Ekipa e)
        {
            List<RezultatUkupno> result = new List<RezultatUkupno>();
            RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            foreach (GimnasticarUcesnik g in e.Gimnasticari)
            {
                result.Add(Takmicenje.getRezultatUkupnoForEkipniRezultat(
                    g, rezTakDAO.FindRezultatiUkupnoForGimnasticar(takmicenjeId, g.Id)));
            }
            return result;
        }

        public static void updateTakmicenjeOnChangedPropozicije(IList<RezultatskoTakmicenje> rezTakmicenja,
            IDictionary<int, Propozicije> origPropozicijeMap, Takmicenje takmicenje)
        {
            DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().Update(takmicenje); // ovo snima i propozicije za descriptions
            
            PropozicijeDAO propozicijeDAO = DAOFactoryFactory.DAOFactory.GetPropozicijeDAO();

            RezultatskoTakmicenje.updateImaEkipnoTakmicenje(rezTakmicenja);
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                // TODO4: Implementiraj Propozicije.Equals
                if (!rt.Propozicije.Equals(origPropozicijeMap[rt.Id]))
                {
                    propozicijeDAO.Update(rt.Propozicije);
                    updateRezultatiOnChangedPropozicije(rt, origPropozicijeMap, takmicenje, rezTakmicenja);
                }
            }
        }

        private static void updateRezultatiOnChangedPropozicije(RezultatskoTakmicenje rt,
            IDictionary<int, Propozicije> origPropozicijeMap, Takmicenje takmicenje,
            IList<RezultatskoTakmicenje> rezTakmicenja)
        {
            Propozicije origPropozicije = origPropozicijeMap[rt.Id];
            RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();

            PoredakUkupnoDAO poredakUkupnoDAO = DAOFactoryFactory.DAOFactory.GetPoredakUkupnoDAO();
            PoredakSpravaDAO poredakSpravaDAO = DAOFactoryFactory.DAOFactory.GetPoredakSpravaDAO();
            PoredakPreskokDAO poredakPreskokDAO = DAOFactoryFactory.DAOFactory.GetPoredakPreskokDAO();
            PoredakEkipnoDAO poredakEkipnoDAO = DAOFactoryFactory.DAOFactory.GetPoredakEkipnoDAO();

            PoredakUkupnoFinaleKupaDAO poredakUkupnoFinaleKupaDAO
                = DAOFactoryFactory.DAOFactory.GetPoredakUkupnoFinaleKupaDAO();
            PoredakSpravaFinaleKupaDAO poredakSpravaFinaleKupaDAO
                = DAOFactoryFactory.DAOFactory.GetPoredakSpravaFinaleKupaDAO();
            PoredakEkipnoFinaleKupaDAO poredakEkipnoFinaleKupaDAO
                = DAOFactoryFactory.DAOFactory.GetPoredakEkipnoFinaleKupaDAO();
            PoredakUkupnoZbirViseKolaDAO poredakUkupnoZbirViseKolaDAO
                = DAOFactoryFactory.DAOFactory.GetPoredakUkupnoZbirViseKolaDAO();
            PoredakEkipnoZbirViseKolaDAO poredakEkipnoZbirViseKolaDAO
                = DAOFactoryFactory.DAOFactory.GetPoredakEkipnoZbirViseKolaDAO();
            
            bool changedPoredakUkupnoTak1 = false;
            bool changedPoredakSpravaTak1 = false;
            bool changedPoredakPreskokTak1 = false;
            bool changedPoredakPreskokTak3 = false;
            bool changedPoredakEkipeTak1 = false;
            bool changedPoredakUkupnoFinaleKupa = false;
            bool changedPoredakUkupnoZbirViseKola = false;
            bool changedPoredakSpravaFinaleKupa = false;
            bool changedPoredakEkipnoFinaleKupa = false;
            bool changedPoredakEkipnoZbirViseKola = false;

            if (rt.Propozicije.PostojiTak2 != origPropozicije.PostojiTak2)
            {
                // ignorisi, posto Takmicenje 2 uvek postoji, da bi se videli rezultati
            }
            if (rt.Propozicije.OdvojenoTak2 != origPropozicije.OdvojenoTak2)
            {
                if (takmicenje.ZavrsenoTak1)
                {
                    // ignorisi
                }
                else if (rt.Propozicije.OdvojenoTak2)
                {
                    // Nije postojalo odvojeno takmicenje II, a u novim propozicijama je OdvojenoTak2 postavljeno na true.
                    // Ignorisi, zato sto se Takmicenje2 kreira tek kada se pozove komanda kreirajTakmicnja234
                }
                else
                {
                    // Postojalo je odvojeno takmicenje II, ali je u novim propozicijama OdvojenoTak2 postavljeno na false
                    // Ignorisi, zato sto Takmicenje2 nije ni kreirano jer je ZavrsenoTak1 false.
                }
            }
            if (rt.Propozicije.ZaPreskokVisebojRacunajBoljuOcenu != origPropozicije.ZaPreskokVisebojRacunajBoljuOcenu
                || rt.Propozicije.NeogranicenBrojTakmicaraIzKlubaTak2 != origPropozicije.NeogranicenBrojTakmicaraIzKlubaTak2
                || rt.Propozicije.MaxBrojTakmicaraIzKlubaTak2 != origPropozicije.MaxBrojTakmicaraIzKlubaTak2
                || rt.Propozicije.BrojFinalistaTak2 != origPropozicije.BrojFinalistaTak2
                || rt.Propozicije.BrojRezerviTak2 != origPropozicije.BrojRezerviTak2)
            {
                // TODO: Fali kod za odvojeno takmicenje 2 finale kupa
                if (!changedPoredakUkupnoTak1)
                {
                    rt.Takmicenje1.PoredakUkupno.rankRezultati(rt.Propozicije);
                    poredakUkupnoDAO.Update(rt.Takmicenje1.PoredakUkupno);
                    changedPoredakUkupnoTak1 = true;
                }
            }

            if (rt.Propozicije.PostojiTak3 != origPropozicije.PostojiTak3)
            {
                // ignorisi, posto Takmicenje 3 uvek postoji, da bi se videli rezultati
            }
            if (rt.Propozicije.OdvojenoTak3 != origPropozicije.OdvojenoTak3)
            {
                if (takmicenje.ZavrsenoTak1)
                {
                    // ignorisi
                }
                else if (rt.Propozicije.OdvojenoTak3)
                {
                    // ignorisi (vidi komentare za takmicenje 2)
                }
                else
                {
                    // ignorisi (vidi komentare za takmicenje 2)
                }
            }
            if (rt.Propozicije.NeogranicenBrojTakmicaraIzKlubaTak3 != origPropozicije.NeogranicenBrojTakmicaraIzKlubaTak3
                || rt.Propozicije.MaxBrojTakmicaraIzKlubaTak3 != origPropozicije.MaxBrojTakmicaraIzKlubaTak3
                || rt.Propozicije.MaxBrojTakmicaraTak3VaziZaDrzavu != origPropozicije.MaxBrojTakmicaraTak3VaziZaDrzavu
                || rt.Propozicije.BrojFinalistaTak3 != origPropozicije.BrojFinalistaTak3
                || rt.Propozicije.BrojRezerviTak3 != origPropozicije.BrojRezerviTak3)
            {
                if (takmicenje.StandardnoTakmicenje)
                {
                    if (!changedPoredakSpravaTak1)
                    {
                        foreach (PoredakSprava ps in rt.Takmicenje1.PoredakSprava)
                        {
                            ps.rankRezultati(rt.Propozicije);
                            poredakSpravaDAO.Update(ps);
                        }
                        changedPoredakSpravaTak1 = true;
                    }
                    if (!changedPoredakPreskokTak1)
                    {
                        rt.Takmicenje1.PoredakPreskok.rankRezultati(rt.Propozicije, takmicenje.FinaleKupa);
                        poredakPreskokDAO.Update(rt.Takmicenje1.PoredakPreskok);
                        changedPoredakPreskokTak1 = true;
                    }
                }
                else if (takmicenje.FinaleKupa && !changedPoredakSpravaFinaleKupa)
                {
                    foreach (PoredakSpravaFinaleKupa p in rt.Takmicenje1.PoredakSpravaFinaleKupa)
                        p.rankRezultati(rt.Propozicije);
                    changedPoredakSpravaFinaleKupa = true;
                }
            }
            if (rt.Propozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka != origPropozicije.KvalifikantiTak3PreskokNaOsnovuObaPreskoka)
            {
                if (!changedPoredakPreskokTak1)
                {
                    rt.Takmicenje1.PoredakPreskok.rankRezultati(rt.Propozicije, takmicenje.FinaleKupa);
                    changedPoredakPreskokTak1 = true;
                }
            }
            if (rt.Propozicije.PoredakTak3PreskokNaOsnovuObaPreskoka != origPropozicije.PoredakTak3PreskokNaOsnovuObaPreskoka)
            {
                if (takmicenje.StandardnoTakmicenje)
                {
                    if (!changedPoredakPreskokTak1)
                    {
                        rt.Takmicenje1.PoredakPreskok.rankRezultati(rt.Propozicije, takmicenje.FinaleKupa);
                        changedPoredakPreskokTak1 = true;
                    }
                    if (takmicenje.ZavrsenoTak1 && rt.odvojenoTak3())
                    {
                        if (!changedPoredakPreskokTak3)
                        {
                            rt.Takmicenje3.PoredakPreskok.rankRezultati(rt.Propozicije, takmicenje.FinaleKupa);
                            changedPoredakPreskokTak3 = true;
                        }
                    }
                }
                else if (takmicenje.FinaleKupa)
                {
                    if (!changedPoredakPreskokTak1)
                    {
                        rt.Takmicenje1.PoredakPreskok.rankRezultati(rt.Propozicije, takmicenje.FinaleKupa);
                        changedPoredakPreskokTak1 = true;
                    }
                }
            }

            // TODO: Fali kod za odvojeno ekipno finale kupa

            if (rt.Propozicije.PostojiTak4 != origPropozicije.PostojiTak4)
            {
                if (takmicenje.ZavrsenoTak1)
                {
                    // ignorisi
                }
                else if (rt.Propozicije.PostojiTak4)
                {
                    // ignorisi, posto se PoredakEkipno za takmicenje 1 uvek kreira
                }
                else
                {
                    // prvo pitati korisnika za potvrdu, pa zatim izbrisati ekipe i poredak ekipno.
                }
            }
            if (rt.Propozicije.OdvojenoTak4 != origPropozicije.OdvojenoTak4)
            {
                if (takmicenje.ZavrsenoTak1)
                {
                    // ignorisi
                }
                else if (rt.Propozicije.OdvojenoTak4)
                {
                    // ignorisi (vidi komentare za takmicenje 2)
                }
                else
                {
                    // ignorisi (vidi komentare za takmicenje 2)
                }
            }
            if (rt.Propozicije.JednoTak4ZaSveKategorije != origPropozicije.JednoTak4ZaSveKategorije)
            {
                // rt.ImaEkipnoTakmicenje i rt.KombinovanoEkipnoTak su promenjeni u updateImaEkipnoTakmicenje.
                rezTakDAO.Update(rt);

                // PoredakEkipno i Ekipe ignorisem, ostavljam korisniku da to ispodesava.
            }
            bool recreatedPoredakEkipeTak1 = false;
            if (rt.Propozicije.BrojRezultataKojiSeBodujuZaEkipu != origPropozicije.BrojRezultataKojiSeBodujuZaEkipu)
            {
                if (rt.ImaEkipnoTakmicenje)
                {
                    rt.Takmicenje1.PoredakEkipno.create(rt,
                        Takmicenje.getEkipaRezultatiUkupnoMap(rt, rezTakmicenja, DeoTakmicenjaKod.Takmicenje1));
                    poredakEkipnoDAO.Update(rt.Takmicenje1.PoredakEkipno);
                    changedPoredakEkipeTak1 = true;
                    recreatedPoredakEkipeTak1 = true;
                }
            }
            if (!recreatedPoredakEkipeTak1 && rt.ImaEkipnoTakmicenje)
            {
                // Proveri da li treba ponovo racunati poredak zbog promenjenog nacina racunanja viseboja.
                bool recreate = false;
                if (!rt.KombinovanoEkipnoTak
                    && rt.Propozicije.ZaPreskokVisebojRacunajBoljuOcenu != origPropozicije.ZaPreskokVisebojRacunajBoljuOcenu)
                {
                    recreate = true;
                }
                if (!recreate && rt.KombinovanoEkipnoTak)
                {
                    // Proveri da li je nekom rez. takmicenju unutar istog descriptiona promenjen nacin racunanja
                    // viseboja.
                    foreach (RezultatskoTakmicenje rt2 in rezTakmicenja)
                    {
                        if (!rt2.TakmicenjeDescription.Equals(rt.TakmicenjeDescription))
                            continue;
                        if (rt2.Propozicije.ZaPreskokVisebojRacunajBoljuOcenu
                            != origPropozicijeMap[rt2.Id].ZaPreskokVisebojRacunajBoljuOcenu)
                        {
                            recreate = true;
                            break;
                        }
                    }
                }
                if (recreate)
                {
                    rt.Takmicenje1.PoredakEkipno.create(rt,
                        Takmicenje.getEkipaRezultatiUkupnoMap(rt, rezTakmicenja, DeoTakmicenjaKod.Takmicenje1));
                    poredakEkipnoDAO.Update(rt.Takmicenje1.PoredakEkipno);
                    changedPoredakEkipeTak1 = true;
                    recreatedPoredakEkipeTak1 = true;
                }
            }                        
            if (rt.Propozicije.BrojEkipaUFinalu != origPropozicije.BrojEkipaUFinalu)
            {
                if (rt.ImaEkipnoTakmicenje && !changedPoredakEkipeTak1)
                {
                    rt.Takmicenje1.PoredakEkipno.rankRezultati(rt.Propozicije);
                    poredakEkipnoDAO.Update(rt.Takmicenje1.PoredakEkipno);
                    changedPoredakEkipeTak1 = true;
                }
            }

            if (rt.Propozicije.Tak2FinalnaOcenaJeZbirObaKola != origPropozicije.Tak2FinalnaOcenaJeZbirObaKola
                || rt.Propozicije.Tak2FinalnaOcenaJeMaxObaKola != origPropozicije.Tak2FinalnaOcenaJeMaxObaKola
                || rt.Propozicije.Tak2FinalnaOcenaJeProsekObaKola != origPropozicije.Tak2FinalnaOcenaJeProsekObaKola
                || rt.Propozicije.Tak2NeRacunajProsekAkoNemaOceneIzObaKola != origPropozicije.Tak2NeRacunajProsekAkoNemaOceneIzObaKola)
            {
                if (takmicenje.FinaleKupa && !changedPoredakUkupnoFinaleKupa)
                {
                    rt.Takmicenje1.PoredakUkupnoFinaleKupa.rankRezultati(rt.Propozicije);
                    changedPoredakUkupnoFinaleKupa = true;
                }
                else if (takmicenje.ZbirViseKola && !changedPoredakUkupnoZbirViseKola)
                {
                    rt.Takmicenje1.PoredakUkupnoZbirViseKola.rankRezultati();
                    changedPoredakUkupnoZbirViseKola = true;
                }
            }

            if (rt.Propozicije.Tak3FinalnaOcenaJeZbirObaKola != origPropozicije.Tak3FinalnaOcenaJeZbirObaKola
                || rt.Propozicije.Tak3FinalnaOcenaJeMaxObaKola != origPropozicije.Tak3FinalnaOcenaJeMaxObaKola
                || rt.Propozicije.Tak3FinalnaOcenaJeProsekObaKola != origPropozicije.Tak3FinalnaOcenaJeProsekObaKola
                || rt.Propozicije.Tak3NeRacunajProsekAkoNemaOceneIzObaKola != origPropozicije.Tak3NeRacunajProsekAkoNemaOceneIzObaKola)
            {
                if (takmicenje.FinaleKupa && !changedPoredakSpravaFinaleKupa)
                {
                    foreach (PoredakSpravaFinaleKupa p in rt.Takmicenje1.PoredakSpravaFinaleKupa)
                        p.rankRezultati(rt.Propozicije);
                    changedPoredakSpravaFinaleKupa = true;
                }
            }

            if (rt.Propozicije.Tak4FinalnaOcenaJeZbirObaKola != origPropozicije.Tak4FinalnaOcenaJeZbirObaKola
                || rt.Propozicije.Tak4FinalnaOcenaJeMaxObaKola != origPropozicije.Tak4FinalnaOcenaJeMaxObaKola
                || rt.Propozicije.Tak4FinalnaOcenaJeProsekObaKola != origPropozicije.Tak4FinalnaOcenaJeProsekObaKola
                || rt.Propozicije.Tak4NeRacunajProsekAkoNemaOceneIzObaKola != origPropozicije.Tak4NeRacunajProsekAkoNemaOceneIzObaKola)
            {
                if (takmicenje.FinaleKupa && !changedPoredakEkipnoFinaleKupa)
                {
                    rt.Takmicenje1.PoredakEkipnoFinaleKupa.rankRezultati(rt.Propozicije);
                    changedPoredakEkipnoFinaleKupa = true;
                }
                else if (takmicenje.ZbirViseKola && !changedPoredakEkipnoZbirViseKola)
                {
                    rt.Takmicenje1.PoredakEkipnoZbirViseKola.rankRezultati();
                    changedPoredakEkipnoZbirViseKola = true;
                }
            }
        }
    }
}
