using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class GimnasticarDAOImpl : GenericNHibernateDAO<Gimnasticar, int>, GimnasticarDAO
    {
        #region GimnasticarDAO Members

        public IList<Gimnasticar> FindGimnasticariByKlub(Klub klub)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Gimnasticar g where g.Klub = :klub");
                q.SetEntity("klub", klub);
                return q.List<Gimnasticar>();
            }
            catch (HibernateException ex)
            {
                string message = String.Format(
                    "{0} \n\n{1}", Strings.DatabaseAccessExceptionMessage, ex.Message);
                throw new InfrastructureException(message, ex);
            }
        }

        public bool existsGimnasticar(Klub klub)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from Gimnasticar g where g.Klub = :klub");
                q.SetEntity("klub", klub);
                return (long)q.UniqueResult() > 0;
            }
            catch (HibernateException ex)
            {
                string message = String.Format(
                    "{0} \n\n{1}", Strings.DatabaseAccessExceptionMessage, ex.Message);
                throw new InfrastructureException(message, ex);
            }
        }

        #endregion
    }
}