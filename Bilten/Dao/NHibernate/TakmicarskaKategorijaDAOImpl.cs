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
                    where k.Takmicenje.Id = :takmicenjeId
                    order by k.RedBroj asc");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<TakmicarskaKategorija>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool existsKategorijaNaziv(string naziv, int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from TakmicarskaKategorija k
                    where k.Takmicenje.Id = :takmicenjeId
                    and k.Naziv = :naziv");
                q.SetInt32("takmicenjeId", takmicenjeId);
                q.SetString("naziv", naziv);                
                return (long)q.UniqueResult() > 0;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public int GetCountForTakmicenje(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"
                    select count(*) from TakmicarskaKategorija k
                    where k.Takmicenje.Id = :takmicenjeId");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return (int)(long)q.UniqueResult();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        #endregion
    }
}