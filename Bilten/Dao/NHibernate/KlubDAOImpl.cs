using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class KlubDAOImpl : GenericNHibernateDAO<Klub, int>, KlubDAO
    {
        #region KlubDAO Members

        public IList<Klub> FindAll()
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Klub k left join fetch k.Mesto");
                return q.List<Klub>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        #endregion
    }
}