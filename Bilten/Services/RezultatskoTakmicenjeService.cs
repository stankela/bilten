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

        public static void updateRezTakmicenjaFromChangedPropozicije(IList<RezultatskoTakmicenje> rezTakmicenja,
            IDictionary<int, Propozicije> origPropozicije)
        {
            // TODO4: Posto ova naredba moze da npr. izbrise celo takmicenje III (ako je postojalo
            // odvojeno takmicenje III, a u novim propozicijama je navedeno da ne postoji odvojeno
            // takmicenje III), mozda bi trebalo dati mogucnost korisniku da odustane od operacije.
            // Obratiti paznju i na ekipno takmicenje - sta ako je ranije bilo kombinovano a sad nije i obratno.
            // Takodje obratiti paznju da se za takmicenje 1 rezultati i poredak uvek racunaju za sprave i viseboj,
            // cak i ako su PostojiTak2 ili PostojiTak3 false (da bi bilo moguce pregledati rezultate).

            RezultatskoTakmicenje.updateImaEkipnoTakmicenje(rezTakmicenja);

            RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            PropozicijeDAO propozicijeDAO = DAOFactoryFactory.DAOFactory.GetPropozicijeDAO();

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                object diff = rt.Propozicije.getDiff(origPropozicije[rt.Id], null, null);
                if (diff != null)
                {
                    rezTakDAO.Update(rt); // mora update a ne attach da bi se snimile promenjene propozicije
                    rt.updateTakmicenjaFromChangedPropozicije(diff);
                    RezultatskoTakmicenjeService.izracunajSveRezultate(rt);
                    //rezTakDAO.Update(rt); // da li je potreban ovaj drugi update
                }
            }
        }

        private static void izracunajSveRezultate(RezultatskoTakmicenje rt)
        {
            OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
            IList<Ocena> ocene1 = ocenaDAO.FindByDeoTakmicenja(rt.Takmicenje.Id, DeoTakmicenjaKod.Takmicenje1);
            
            IDictionary<int, List<RezultatUkupno>> ekipaRezultatiUkupnoMap = null;

            PoredakUkupnoDAO poredakUkupnoDAO = DAOFactoryFactory.DAOFactory.GetPoredakUkupnoDAO();
            PoredakSpravaDAO poredakSpravaDAO = DAOFactoryFactory.DAOFactory.GetPoredakSpravaDAO();
            PoredakPreskokDAO poredakPreskokDAO = DAOFactoryFactory.DAOFactory.GetPoredakPreskokDAO();
            PoredakEkipnoDAO poredakEkipnoDAO = DAOFactoryFactory.DAOFactory.GetPoredakEkipnoDAO();

            RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();

            PoredakUkupno pu = rt.getPoredakUkupno(DeoTakmicenjaKod.Takmicenje1);
            pu.create(rt, ocene1);
            poredakUkupnoDAO.Update(pu);

            foreach (PoredakSprava ps in rt.Takmicenje1.PoredakSprava)
            {
                ps.create(rt, ocene1);
                poredakSpravaDAO.Update(ps);
            }
            PoredakPreskok pp = rt.getPoredakPreskok(DeoTakmicenjaKod.Takmicenje1);
            pp.create(rt, ocene1);
            poredakPreskokDAO.Update(pp);

            if (rt.ImaEkipnoTakmicenje)
            {
                PoredakEkipno pe = rt.getPoredakEkipno(DeoTakmicenjaKod.Takmicenje1);
                IList<RezultatskoTakmicenje> rezTakmicenja = rezTakDAO.FindByTakmicenje(rt.Takmicenje.Id);
                IList<RezultatskoTakmicenje> list = new List<RezultatskoTakmicenje>();
                list.Add(rt);
                ekipaRezultatiUkupnoMap
                    = Takmicenje.getEkipaRezultatiUkupnoMap(list, rezTakmicenja, DeoTakmicenjaKod.Takmicenje1);
                pe.create(rt, ekipaRezultatiUkupnoMap);
                poredakEkipnoDAO.Update(pe);
            }

            foreach (Ocena o in ocene1)
                ocenaDAO.Evict(o);
        }
    }
}
