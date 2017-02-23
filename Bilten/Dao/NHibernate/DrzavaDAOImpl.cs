using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class DrzavaDAOImpl : GenericNHibernateDAO<Drzava, int>, DrzavaDAO
    {
        #region DrzavaDAO Members

        public IList<Drzava> FindAll()
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Drzava");
                return q.List<Drzava>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        #endregion
    }
}