using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class RasporedSudijaDAOImpl : GenericNHibernateDAO<RasporedSudija, int>, RasporedSudijaDAO
    {
        #region RasporedSudijaDAO Members

        public IList<RasporedSudija> FindByTakmicenje(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select distinct r
                        from RasporedSudija r
                        join r.Kategorije k
                        where k.Takmicenje.Id = :takmicenjeId");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<RasporedSudija>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public RasporedSudija FindByIdFetch(int id)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select distinct r
                    from RasporedSudija r
                    left join fetch r.Kategorije k
                    left join fetch r.Odbori o
                    left join fetch o.Sudije s
                    left join fetch s.DrzavaUcesnik dr
                    left join fetch s.KlubUcesnik kl
                    where r.Id = :id");
                q.SetInt32("id", id);
                IList<RasporedSudija> result = q.List<RasporedSudija>();
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