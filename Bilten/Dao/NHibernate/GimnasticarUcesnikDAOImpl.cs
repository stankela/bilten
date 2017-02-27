using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class GimnasticarUcesnikDAOImpl : GenericNHibernateDAO<GimnasticarUcesnik, int>, GimnasticarUcesnikDAO
    {
        #region GimnasticarUcesnikDAO Members

        public bool existsGimnasticarTakBroj(int takBroj, Takmicenje takmicenje)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from GimnasticarUcesnik g
                    where g.TakmicarskiBroj = :takBroj
                    and g.Takmicenje = :takmicenje");
                q.SetInt32("takBroj", takBroj);
                q.SetEntity("takmicenje", takmicenje);
                return (long)q.UniqueResult() > 0;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }

        }

        #endregion
    }
}