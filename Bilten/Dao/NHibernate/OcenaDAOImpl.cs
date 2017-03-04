using System;
using System.Collections.Generic;
using NHibernate;
using Bilten.Exceptions;
using Bilten.Domain;

namespace Bilten.Dao.NHibernate
{
    public class OcenaDAOImpl : GenericNHibernateDAO<Ocena, int>, OcenaDAO
    {
        #region OcenaDAO Members

        public IList<Ocena> FindOceneForGimnasticar(GimnasticarUcesnik gim, DeoTakmicenjaKod deoTakKod)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select o from Ocena o
                    left join fetch o.Ocena2
                    join o.Gimnasticar g
                    where g = :gim
                    and o.DeoTakmicenjaKod = :deoTakKod");
                q.SetEntity("gim", gim);
                q.SetByte("deoTakKod", (byte)deoTakKod);
                return q.List<Ocena>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Ocena> FindByTakmicenje(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"from Ocena o
	                       where o.Gimnasticar.Takmicenje.Id = :takmicenjeId");
                q.SetInt32("takmicenjeId", takmicenjeId);
                return q.List<Ocena>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public IList<Ocena> FindOceneByDeoTakmicenja(int takmicenjeId, DeoTakmicenjaKod deoTakKod)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select o
                    from Ocena o
                    left join fetch o.Ocena2
                    join o.Gimnasticar g
                    where g.Takmicenje.Id = :takmicenjeId
                    and o.DeoTakmicenjaKod = :deoTakKod");
                q.SetInt32("takmicenjeId", takmicenjeId);
                q.SetByte("deoTakKod", (byte)deoTakKod);
                return q.List<Ocena>();
            }
            catch (HibernateException ex)
            {
                throw new InfrastructureException(Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
        }

        public bool existsOcene(int takmicenjeId)
        {
            try
            {
                IQuery q = Session.CreateQuery(@"select count(*) from Ocena o
	                       where o.Gimnasticar.Takmicenje.Id = :takmicenjeId");
                q.SetInt32("takmicenjeId", takmicenjeId);
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