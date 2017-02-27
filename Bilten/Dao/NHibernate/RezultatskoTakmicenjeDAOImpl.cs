using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class RezultatskoTakmicenjeDAOImpl : GenericNHibernateDAO<RezultatskoTakmicenje, int>, RezultatskoTakmicenjeDAO
    {
        #region RezultatskoTakmicenjeDAO Members

        public RezultatskoTakmicenje loadRezTakmicenje(int rezTakmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select distinct r
                    from RezultatskoTakmicenje r
                    left join fetch r.Kategorija kat
                    left join fetch r.TakmicenjeDescription d
                    left join fetch r.Takmicenje1 t
                    left join fetch t.Ekipe e
                    where r.Id = :id");
                q.SetInt32("id", rezTakmicenjeId);
                IList<RezultatskoTakmicenje> result = q.List<RezultatskoTakmicenje>();
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

        #endregion
    }
}