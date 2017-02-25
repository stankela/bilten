using Bilten.Dao.NHibernate;

namespace Bilten.Dao
{
    public abstract class DAOFactory
    {
        public abstract KlubDAO GetKlubDAO();
        public abstract GimnasticarDAO GetGimnasticarDAO();
        public abstract DrzavaDAO GetDrzavaDAO();
        public abstract SudijaDAO GetSudijaDAO();
        public abstract KategorijaGimnasticaraDAO GetKategorijaGimnasticaraDAO();
        public abstract MestoDAO GetMestoDAO();
        public abstract TakmicenjeDAO GetTakmicenjeDAO();
        public abstract SudijaUcesnikDAO GetSudijaUcesnikDAO();
        public abstract DrzavaUcesnikDAO GetDrzavaUcesnikDAO();
        public abstract KlubUcesnikDAO GetKlubUcesnikDAO();
    }
}