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
                if (!rt.Propozicije.Equals(origPropozicijeMap[rt.Id]))
                {
                    propozicijeDAO.Update(rt.Propozicije);
                    int updateMask = rt.updateRezultatiOnChangedPropozicije(origPropozicijeMap, takmicenje, rezTakmicenja);
                    updateRezTakmicenje(rt, updateMask);
                }
            }
        }

        private static void updateRezTakmicenje(RezultatskoTakmicenje rt, int updateMask)
        {
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

            if (UpdateKindUtil.shouldUpdate(UpdateKind.RezTak, updateMask))
                rezTakDAO.Update(rt);
            if (UpdateKindUtil.shouldUpdate(UpdateKind.PoredakUkupnoTak1, updateMask))
                poredakUkupnoDAO.Update(rt.Takmicenje1.PoredakUkupno);
            if (UpdateKindUtil.shouldUpdate(UpdateKind.PoredakSpravaTak1, updateMask))
            {
                foreach (PoredakSprava ps in rt.Takmicenje1.PoredakSprava)
                    poredakSpravaDAO.Update(ps);
            }
            if (UpdateKindUtil.shouldUpdate(UpdateKind.PoredakPreskokTak1, updateMask))
                poredakPreskokDAO.Update(rt.Takmicenje1.PoredakPreskok);
            if (UpdateKindUtil.shouldUpdate(UpdateKind.PoredakPreskokTak3, updateMask))
                poredakPreskokDAO.Update(rt.Takmicenje3.PoredakPreskok);
            if (UpdateKindUtil.shouldUpdate(UpdateKind.PoredakEkipeTak1, updateMask))
                poredakEkipnoDAO.Update(rt.Takmicenje1.PoredakEkipno);
            if (UpdateKindUtil.shouldUpdate(UpdateKind.PoredakUkupnoFinaleKupa, updateMask))
                poredakUkupnoFinaleKupaDAO.Update(rt.Takmicenje1.PoredakUkupnoFinaleKupa);
            if (UpdateKindUtil.shouldUpdate(UpdateKind.PoredakUkupnoZbirViseKola, updateMask))
                poredakUkupnoZbirViseKolaDAO.Update(rt.Takmicenje1.PoredakUkupnoZbirViseKola);
            if (UpdateKindUtil.shouldUpdate(UpdateKind.PoredakSpravaFinaleKupa, updateMask))
            {
                foreach (PoredakSpravaFinaleKupa p in rt.Takmicenje1.PoredakSpravaFinaleKupa)
                    poredakSpravaFinaleKupaDAO.Update(p);
            }
            if (UpdateKindUtil.shouldUpdate(UpdateKind.PoredakEkipnoFinaleKupa, updateMask))
                poredakEkipnoFinaleKupaDAO.Update(rt.Takmicenje1.PoredakEkipnoFinaleKupa);
            if (UpdateKindUtil.shouldUpdate(UpdateKind.PoredakEkipnoZbirViseKola, updateMask))
                poredakEkipnoZbirViseKolaDAO.Update(rt.Takmicenje1.PoredakEkipnoZbirViseKola);
        }
    }
}
