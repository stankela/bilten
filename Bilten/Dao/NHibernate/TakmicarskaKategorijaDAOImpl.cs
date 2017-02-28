using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class TakmicarskaKategorijaDAOImpl : GenericNHibernateDAO<TakmicarskaKategorija, int>, TakmicarskaKategorijaDAO
    {
        #region TakmicarskaKategorijaDAO Members

        public IList<TakmicarskaKategorija> FindByTakmicenje(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from TakmicarskaKategorija k
                where k.Takmicenje.Id = :takmicenjeId");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<TakmicarskaKategorija>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        #endregion
    }
}