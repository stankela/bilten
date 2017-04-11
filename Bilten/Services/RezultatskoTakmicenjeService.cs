using Bilten.Dao;
using Bilten.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bilten.Services
{
    public class RezultatskoTakmicenjeService
    {
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

        public static void addGimnasticariToRezTak(IList<GimnasticarUcesnik> gimnasticari, RezultatskoTakmicenje rezTak,
            IList<GimnasticarUcesnik> addedGimnasticari)
        {
            RezultatskoTakmicenjeDAO rezultatskoTakmicenjeDAO = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO();
            rezultatskoTakmicenjeDAO.Attach(rezTak, false);

            TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
            Takmicenje takmicenje = takmicenjeDAO.FindById(rezTak.Takmicenje.Id);

            IList<RezultatskoTakmicenje> rezTakmicenja1 = null;
            IList<RezultatskoTakmicenje> rezTakmicenja2 = null;
            PoredakUkupno poredak1 = null;
            PoredakUkupno poredak2 = null;
            if (takmicenje.FinaleKupa)
            {
                rezTakmicenja1 = rezultatskoTakmicenjeDAO.FindByTakmicenjeFetch_Tak1_Gimnasticari(takmicenje.PrvoKolo.Id);
                rezTakmicenja2 = rezultatskoTakmicenjeDAO.FindByTakmicenjeFetch_Tak1_Gimnasticari(takmicenje.DrugoKolo.Id);
                poredak1 = Takmicenje.getRezTakmicenje(rezTakmicenja1, 0, rezTak.Kategorija).Takmicenje1.PoredakUkupno;
                poredak2 = Takmicenje.getRezTakmicenje(rezTakmicenja2, 0, rezTak.Kategorija).Takmicenje1.PoredakUkupno;
            }

            OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
            foreach (GimnasticarUcesnik g in gimnasticari)
            {
                if (rezTak.Takmicenje1.addGimnasticar(g))
                {
                    IList<Ocena> ocene = ocenaDAO.FindByGimnasticar(g, DeoTakmicenjaKod.Takmicenje1);
                    if (takmicenje.FinaleKupa)
                        rezTak.Takmicenje1.updateRezultatiOnGimnasticarAdded(g, ocene, rezTak, poredak1, poredak2);
                    else
                        rezTak.Takmicenje1.updateRezultatiOnGimnasticarAdded(g, ocene, rezTak);
                    foreach (Ocena o in ocene)
                        ocenaDAO.Evict(o);

                    addedGimnasticari.Add(g);
                }
            }

            takmicenjeDAO.Evict(takmicenje);
            if (takmicenje.FinaleKupa)
            {
                foreach (RezultatskoTakmicenje rt in rezTakmicenja1)
                    rezultatskoTakmicenjeDAO.Evict(rt);
                foreach (RezultatskoTakmicenje rt in rezTakmicenja2)
                    rezultatskoTakmicenjeDAO.Evict(rt);
            }
            
            if (addedGimnasticari.Count > 0)
                DAOFactoryFactory.DAOFactory.GetTakmicenje1DAO().Update(rezTak.Takmicenje1);
        }
    }
}
