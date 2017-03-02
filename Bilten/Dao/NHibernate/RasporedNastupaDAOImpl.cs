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

        #endregion
    }
}