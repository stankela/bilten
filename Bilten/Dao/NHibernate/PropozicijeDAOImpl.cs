using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class PropozicijeDAOImpl : GenericNHibernateDAO<Propozicije, int>, PropozicijeDAO
    {
        #region PropozicijeDAO Members

        public IList<Propozicije> FindAll()
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    from Propozicije");
                return q.List<Propozicije>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        #endregion
    }
}