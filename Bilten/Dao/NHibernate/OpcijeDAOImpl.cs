using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class OpcijeDAOImpl : GenericNHibernateDAO<Opcije, int>, OpcijeDAO
    {
        #region OpcijeDAO Members

        public Opcije FindOpcije()
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Opcije");
                IList<Opcije> result = q.List<Opcije>();
                if (result.Count > 0)
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