using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class KlubDAOImpl : GenericNHibernateDAO<Klub, int>, KlubDAO
    {
        public override IList<Klub> FindAll()
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Klub k left join fetch k.Mesto");
                return q.List<Klub>();
            }
            catch (HibernateException ex)
            {
                string message = String.Format(
                    "{0} \n\n{1}", Strings.DatabaseAccessExceptionMessage, ex.Message);
                throw new InfrastructureException(message, ex);
            }
        }

        #region KlubDAO Members

        #endregion
    }
}