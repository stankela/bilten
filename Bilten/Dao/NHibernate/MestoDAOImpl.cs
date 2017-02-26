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

        public IList<Mesto> FindAll(bool sorted = false)
        {
            try
            {
                IQuery q;
                if (sorted)
                    q = Session.CreateQuery(@"from Mesto m order by m.Naziv asc");
                else
                    q = Session.CreateQuery(@"from Mesto m");
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