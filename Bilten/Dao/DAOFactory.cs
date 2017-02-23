using Bilten.Dao.NHibernate;

namespace Bilten.Dao
{
    public abstract class DAOFactory
    {
        public abstract KlubDAO GetKlubDAO();
        public abstract GimnasticarDAO GetGimnasticarDAO();
    }
}