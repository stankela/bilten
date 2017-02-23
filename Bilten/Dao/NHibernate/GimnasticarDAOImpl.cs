using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class GimnasticarDAOImpl : GenericNHibernateDAO<Gimnasticar, int>, GimnasticarDAO
    {
        #region GimnasticarDAO Members

        public IList<Gimnasticar> FindGimnasticariByKlub(Klub klub)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Gimnasticar g where g.Klub = :klub");
                q.SetEntity("klub", klub);
                return q.List<Gimnasticar>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Gimnasticar> FindGimnasticariByDrzava(Drzava drzava)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Gimnasticar g where g.Drzava = :drzava");
                q.SetEntity("drzava", drzava);
                return q.List<Gimnasticar>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool existsGimnasticar(Klub klub)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from Gimnasticar g where g.Klub = :klub");
                q.SetEntity("klub", klub);
                return (long)q.UniqueResult() > 0;
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool existsGimnasticar(Drzava drzava)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from Gimnasticar g where g.Drzava = :drzava");
                q.SetEntity("drzava", drzava);
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