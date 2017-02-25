using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class KlubUcesnikDAOImpl : GenericNHibernateDAO<KlubUcesnik, int>, KlubUcesnikDAO
    {
        #region KlubUcesnikDAO Members

        public KlubUcesnik FindKlubUcesnik(int takmicenjeId, string naziv)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from KlubUcesnik k
                    where k.Takmicenje.Id = :takmicenjeId
                    and k.Naziv like :naziv");
                q.SetInt32("takmicenjeId", takmicenjeId);
                q.SetString("naziv", naziv);
                IList<KlubUcesnik> result = q.List<KlubUcesnik>();
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