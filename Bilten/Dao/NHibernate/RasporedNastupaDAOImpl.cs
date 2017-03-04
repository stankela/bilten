using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class RasporedNastupaDAOImpl : GenericNHibernateDAO<RasporedNastupa, int>, RasporedNastupaDAO
    {
        #region RasporedNastupaDAO Members

        public IList<RasporedNastupa> FindByTakmicenje(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select distinct r
                        from RasporedNastupa r
                        join r.Kategorije k
                        where k.Takmicenje.Id = :takmicenjeId");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<RasporedNastupa>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public RasporedNastupa FindByIdFetch(int rasporedId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select distinct r
                    from RasporedNastupa r
                    join fetch r.Kategorije k
                    left join fetch r.StartListe s
                    left join fetch s.Nastupi n
                    left join fetch n.Gimnasticar g
                    left join fetch g.DrzavaUcesnik dr
                    left join fetch g.KlubUcesnik kl
                    where r.Id = :rasporedId");
                q.SetInt32("rasporedId", rasporedId);
                IList<RasporedNastupa> result = q.List<RasporedNastupa>();
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