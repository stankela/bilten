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
        public abstract RezultatskoTakmicenjeDAO GetRezultatskoTakmicenjeDAO();
        public abstract EkipaDAO GetEkipaDAO();
        public abstract Takmicenje1DAO GetTakmicenje1DAO();
        public abstract Takmicenje2DAO GetTakmicenje2DAO();
        public abstract Takmicenje3DAO GetTakmicenje3DAO();
        public abstract Takmicenje4DAO GetTakmicenje4DAO();
        public abstract GimnasticarUcesnikDAO GetGimnasticarUcesnikDAO();
        public abstract OcenaDAO GetOcenaDAO();
        public abstract DrugaOcenaDAO GetDrugaOcenaDAO();
        public abstract UcesnikTakmicenja2DAO GetUcesnikTakmicenja2DAO();
        public abstract UcesnikTakmicenja3DAO GetUcesnikTakmicenja3DAO();
        public abstract RezultatskoTakmicenjeDescriptionDAO GetRezultatskoTakmicenjeDescriptionDAO();
        public abstract TakmicarskaKategorijaDAO GetTakmicarskaKategorijaDAO();
    }
}