using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class MestoDAOImpl : GenericNHibernateDAO<Mesto, int>, MestoDAO
    {
        #region MestoDAO Members

        public IList<Mesto> FindAll()
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Mesto");
                return q.List<Mesto>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        #endregion
    }
}