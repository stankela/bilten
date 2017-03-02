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

        #endregion
    }
}