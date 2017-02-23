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
    }
}