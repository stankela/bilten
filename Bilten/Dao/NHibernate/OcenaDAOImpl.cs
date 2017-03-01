using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class OcenaDAOImpl : GenericNHibernateDAO<Ocena, int>, OcenaDAO
    {
        #region OcenaDAO Members

        public bool existsOcene(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from Ocena o
	                       where o.Gimnasticar.Takmicenje.Id = :takmicenjeId");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return (long)q.UniqueResult() > 0;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        #endregion
    }
}