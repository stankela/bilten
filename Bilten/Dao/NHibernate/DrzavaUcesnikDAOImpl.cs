using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class DrzavaUcesnikDAOImpl : GenericNHibernateDAO<DrzavaUcesnik, int>, DrzavaUcesnikDAO
    {
        #region DrzavaUcesnikDAO Members

        public DrzavaUcesnik FindDrzavaUcesnik(int takmicenjeId, string naziv)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from DrzavaUcesnik d
                    where d.Takmicenje.Id = :takmicenjeId
                    and d.Naziv like :naziv");
                q.SetInt32("takmicenjeId", takmicenjeId);
                q.SetString("naziv", naziv);
                IList<DrzavaUcesnik> result = q.List<DrzavaUcesnik>();
                if (result.Count == 0)
                    return null;
                else
                    return result[0];
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<DrzavaUcesnik> FindByTakmicenje(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from DrzavaUcesnik d
                    where d.Takmicenje.Id = :takmicenjeId
                    order by d.Naziv asc");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<DrzavaUcesnik>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        #endregion
    }
}