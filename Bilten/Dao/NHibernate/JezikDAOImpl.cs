using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class JezikDAOImpl : GenericNHibernateDAO<Jezik, int>, JezikDAO
    {
        #region JezikDAO Members

        public IList<Jezik> FindAll()
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Jezik");
                return q.List<Jezik>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public Jezik FindDefault()
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Jezik where Default = true");
                IList<Jezik> result = q.List<Jezik>();
                if (result != null && result.Count > 0)
                    return result[0];
                return null;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        #endregion
    }
}