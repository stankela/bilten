using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class SudijaUcesnikDAOImpl : GenericNHibernateDAO<SudijaUcesnik, int>, SudijaUcesnikDAO
    {
        #region SudijaUcesnikDAO Members

        public IList<SudijaUcesnik> FindByTakmicenje(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from SudijaUcesnik s
                    where s.Takmicenje.Id = :takmicenjeId");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<SudijaUcesnik>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<SudijaUcesnik> FindByTakmicenjeFetchKlubDrzava(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from SudijaUcesnik s
                    left join fetch s.DrzavaUcesnik
                    left join fetch s.KlubUcesnik
                    where s.Takmicenje.Id = :takmicenjeId
                    order by s.Prezime asc, s.Ime asc");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<SudijaUcesnik>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool existsSudijaUcesnik(Sudija s, Takmicenje takmicenje)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from SudijaUcesnik s
                    where s.Ime like :ime
                    and s.Prezime like :prezime
                    and s.Takmicenje = :takmicenje");
                q.SetString("ime", s.Ime);
                q.SetString("prezime", s.Prezime);
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