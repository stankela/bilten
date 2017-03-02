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

        public override Takmicenje2DAO GetTakmicenje2DAO()
        {
            return new Takmicenje2DAOImpl();
        }

        public override Takmicenje3DAO GetTakmicenje3DAO()
        {
            return new Takmicenje3DAOImpl();
        }

        public override Takmicenje4DAO GetTakmicenje4DAO()
        {
            return new Takmicenje4DAOImpl();
        }

        public override GimnasticarUcesnikDAO GetGimnasticarUcesnikDAO()
        {
            return new GimnasticarUcesnikDAOImpl();
        }

        public override OcenaDAO GetOcenaDAO()
        {
            return new OcenaDAOImpl();
        }

        public override DrugaOcenaDAO GetDrugaOcenaDAO()
        {
            return new DrugaOcenaDAOImpl();
        }

        public override UcesnikTakmicenja2DAO GetUcesnikTakmicenja2DAO()
        {
            return new UcesnikTakmicenja2DAOImpl();
        }

        public override UcesnikTakmicenja3DAO GetUcesnikTakmicenja3DAO()
        {
            return new UcesnikTakmicenja3DAOImpl();
        }

        public override RezultatskoTakmicenjeDescriptionDAO GetRezultatskoTakmicenjeDescriptionDAO()
        {
            return new  RezultatskoTakmicenjeDescriptionDAOImpl();
        }

        public override TakmicarskaKategorijaDAO GetTakmicarskaKategorijaDAO()
        {
            return new TakmicarskaKategorijaDAOImpl();
        }

        public override OpcijeDAO GetOpcijeDAO()
        {
            return new OpcijeDAOImpl();
        }
    }
}