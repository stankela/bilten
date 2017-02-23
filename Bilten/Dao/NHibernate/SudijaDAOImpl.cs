using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class SudijaDAOImpl : GenericNHibernateDAO<Sudija, int>, SudijaDAO
    {
        #region SudijaDAO Members

        public bool existsSudija(Drzava drzava)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from Sudija s where s.Drzava = :drzava");
                q.SetEntity("drzava", drzava);
                return (long)q.UniqueResult() > 0;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Sudija> FindSudijeByDrzava(Drzava drzava)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Sudija s where s.Drzava = :drzava");
                q.SetEntity("drzava", drzava);
                return q.List<Sudija>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        #endregion
    }
}