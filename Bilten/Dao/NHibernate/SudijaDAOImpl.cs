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

        public IList<Sudija> FindAll()
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Sudija s
                    left join fetch s.Drzava
                    left join fetch s.Klub
                    order by s.Prezime asc, s.Ime asc");
                return q.List<Sudija>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

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

        public bool existsSudija(string ime, string prezime)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from Sudija s
                    where s.Ime like :ime
                    and s.Prezime like :prezime");
                q.SetString("ime", ime);
                q.SetString("prezime", prezime);
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