using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class StartListaNaSpraviDAOImpl : GenericNHibernateDAO<StartListaNaSpravi, int>, StartListaNaSpraviDAO
    {
        #region StartListaNaSpraviDAO Members

        public IList<StartListaNaSpravi> FindByGimnasticar(GimnasticarUcesnik gim)
        { 
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct s
                    from StartListaNaSpravi s
                    join s.Nastupi n
                    where n.Gimnasticar = :gim");
                q.SetEntity("gim", gim);
                return q.List<StartListaNaSpravi>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        #endregion
    }
}