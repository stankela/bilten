using System;
using Bilten.Exceptions;

namespace Bilten.Dao.NHibernate
{
    /**
	 * Returns NHibernate-specific instances of DAOs.
	 */

    public class NHibernateDAOFactory : DAOFactory
    {
        public override KlubDAO GetKlubDAO()
        {
            return new KlubDAOImpl();
        }

        public override GimnasticarDAO GetGimnasticarDAO()
        {
            return new GimnasticarDAOImpl();
        }

        public override DrzavaDAO GetDrzavaDAO()
        {
            return new DrzavaDAOImpl();
        }

        public override SudijaDAO GetSudijaDAO()
        {
            return new SudijaDAOImpl();
        }

        public override KategorijaGimnasticaraDAO GetKategorijaGimnasticaraDAO()
        {
            return new KategorijaGimnasticaraDAOImpl();
        }

        public override MestoDAO GetMestoDAO()
        {
            return new MestoDAOImpl();
        }

        public override TakmicenjeDAO GetTakmicenjeDAO()
        {
            return new TakmicenjeDAOImpl();
        }

        public override SudijaUcesnikDAO GetSudijaUcesnikDAO()
        {
            return new SudijaUcesnikDAOImpl();
        }

        public override DrzavaUcesnikDAO GetDrzavaUcesnikDAO()
        {
            return new DrzavaUcesnikDAOImpl();
        }

        public override KlubUcesnikDAO GetKlubUcesnikDAO()
        {
            return new KlubUcesnikDAOImpl();
        }

        public override RezultatskoTakmicenjeDAO GetRezultatskoTakmicenjeDAO()
        {
            return new RezultatskoTakmicenjeDAOImpl();
        }

        public override EkipaDAO GetEkipaDAO()
        {
            return new EkipaDAOImpl();
        }

        public override Takmicenje1DAO GetTakmicenje1DAO()
        {
            return new Takmicenje1DAOImpl();
        }
    }
}