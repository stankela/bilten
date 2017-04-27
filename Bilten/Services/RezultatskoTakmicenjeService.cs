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
            IDictionary<int, Propozicije> origPropozicijeMap, IDictionary<int, Propozicije> origDescPropozicijeMap,
            Takmicenje takmicenje)
        {
            DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().Update(takmicenje); // ovo snima i propozicije za descriptions

            RezultatskoTakmicenjeDAO rezTakDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            PropozicijeDAO propozicijeDAO = DAOFactoryFactory.DAOFactory.GetPropozicijeDAO();
            OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
            IList<Ocena> oceneTak1 = null;

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                if (!rt.Propozicije.Equals(origPropozicijeMap[rt.Id]))
                    rezTakDAO.Update(rt); // ovo snima i propozicije i sve promene rezultata
                else
                    rezTakDAO.Attach(rt, false);  // moram da attachujem (ili da apdejtujem) sva rez. takmicenja zato
                                                  // sto se koriste u izracunavanju ekipnog poretka.
            }

            foreach (RezultatskoTakmicenjeDescription d in takmicenje.TakmicenjeDescriptions)
            {
                if (d.Propozicije.JednoTak4ZaSveKategorije != origDescPropozicijeMap[d.Id].JednoTak4ZaSveKategorije)
                {
                    // Posto je opcija JednoTak4ZaSveKategorije onemogucena u propozicijama za konkretna rez. takmicenja,
                    // moguce je da se promena u propozicijama za description ne prenosi na propozicije na konkretna
                    // rez. takmicenja. Zato ponovo radim Update za sva rez. takmicenja, da bih bio siguran da ce promene
                    // koje ce biti izvrsene u metodu updateImaEkipnoTakmicenje biti snimljene u bazu.
                    foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                    {
                        if (rt.TakmicenjeDescription.Equals(d))
                            rezTakDAO.Update(rt);
                    }
                    RezultatskoTakmicenje.updateImaEkipnoTakmicenje(rezTakmicenja, d);
                }
            }

            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
            {
                if (!rt.Propozicije.Equals(origPropozicijeMap[rt.Id]))
                {
                    if (oceneTak1 == null)
                        oceneTak1 = ocenaDAO.FindByDeoTakmicenja(takmicenje.Id, DeoTakmicenjaKod.Takmicenje1);
                    rt.updateRezultatiOnChangedPropozicije(origPropozicijeMap, takmicenje, rezTakmicenja, oceneTak1);
                }
            }

            if (oceneTak1 != null)
            { 
                foreach (Ocena o in oceneTak1)
                    ocenaDAO.Evict(o);
            }
        }
    }
}
