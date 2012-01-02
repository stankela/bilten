using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Dao.NHibernate;

namespace Bilten.Dao
{
    class DAOFactoryFactory
    {
        public static readonly DAOFactory DAOFactory;

        static DAOFactoryFactory()
        {
            DAOFactory = new NHibernateDAOFactory();
        }
    }
}
