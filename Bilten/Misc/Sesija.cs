using Bilten.Dao;
using Bilten.Dao.NHibernate;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;
using NHibernate;
using NHibernate.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Bilten.Misc
{
    public class Sesija
    {
        private static Sesija instance;
        public static Sesija Instance
        {
            get
            {
                if (instance == null)
                    instance = new Sesija();
                return instance;
            }
        }

        protected Sesija()
        {
            takmicenjeId = -1;
            clearOcene();
        }

        public void InitSession()
        {

        }

        public void EndSession()
        {

        }

        private int takmicenjeId;
        public int TakmicenjeId
        { 
            get { return takmicenjeId; }
        }

        private List<Ocena> oceneTak1;
        private List<Ocena> oceneTak2;
        private List<Ocena> oceneTak3;
        private List<Ocena> oceneTak4;

        public void onTakmicenjeChanged(int takmicenjeId)
        {
            this.takmicenjeId = takmicenjeId;
            clearOcene();
        }

        public void clearOcene()
        {
            oceneTak1 = null;
            oceneTak2 = null;
            oceneTak3 = null;
            oceneTak4 = null;
        }

        private void loadOcene()
        {
            IList<Ocena> sveOcene = null;
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    OcenaDAO ocenaDAO = DAOFactoryFactory.DAOFactory.GetOcenaDAO();
                    ocenaDAO.Session = session;
                    sveOcene = ocenaDAO.FindByTakmicenje(takmicenjeId);
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {

            }

            oceneTak1 = new List<Ocena>();
            oceneTak2 = new List<Ocena>();
            oceneTak3 = new List<Ocena>();
            oceneTak4 = new List<Ocena>();
            foreach (Ocena o in sveOcene)
            {
                if (o.DeoTakmicenjaKod == DeoTakmicenjaKod.Takmicenje1)
                    oceneTak1.Add(o);
                else if (o.DeoTakmicenjaKod == DeoTakmicenjaKod.Takmicenje2)
                    oceneTak2.Add(o);
                else if (o.DeoTakmicenjaKod == DeoTakmicenjaKod.Takmicenje3)
                    oceneTak3.Add(o);
                else
                    oceneTak4.Add(o);
            }
        }

        public virtual void addOcena(Ocena o)
        {
            getOcene(o.DeoTakmicenjaKod).Add(o);
        }

        public virtual List<Ocena> getOcene(DeoTakmicenjaKod deoTakKod)
        {
            if (oceneTak1 == null)
                loadOcene();

            if (deoTakKod == DeoTakmicenjaKod.Takmicenje1)
                return oceneTak1;
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje2)
                return oceneTak2;
            else if (deoTakKod == DeoTakmicenjaKod.Takmicenje3)
                return oceneTak3;
            else
                return oceneTak4;
        }

        public virtual void deleteOcena(Ocena o)
        {
            List<Ocena> ocene = getOcene(o.DeoTakmicenjaKod);

            bool removed = false;
            for (int i = 0; i < ocene.Count; ++i)
            {
                if (ocene[i].Id == o.Id)
                {
                    ocene.RemoveAt(i);
                    removed = true;
                    break;
                }
            }
            Trace.Assert(removed, "Greska u programu");
        }

        public virtual void updateOcena(Ocena o)
        {
            deleteOcena(o);
            addOcena(o);
        }

        public List<Ocena> getOcene(GimnasticarUcesnik g, DeoTakmicenjaKod deoTakKod)
        {
            List<Ocena> result = new List<Ocena>();
            foreach (Ocena o in getOcene(deoTakKod))
            {
                if (o.Gimnasticar.Id == g.Id)
                    result.Add(o);
            }
            return result;
        }
    }
}
